using WordDeduction;

if (args.Length == 3 && args[0] == "--export-native-visual-fixtures")
    return NativeVisualFixtures.Export(args[1], args[2]);

var cases = new (string name, Action<string> run)[] {
    ("confirmed player survives reopening with stable identity", directory => {
        var session = Session.Open(directory, Language.English);
        var result = session.AddPlayer("  Zoë  ");
        Check(result.Success, "add succeeds");
        var first = session.View.Players.Single();
        var reopened = Session.Open(directory, Language.German).View;
        Check(reopened.Players.Single().Id == first.Id, "identity is durable");
        Check(reopened.Players.Single().Name == "Zoë", "trimmed Unicode name is durable");
        Check(reopened.Language == Language.English, "first locale is retained");
    }),
    ("rename pause resume remove and undo preserve the group across restarts", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex"); session.AddPlayer("Rene");
        var id = session.View.Players[0].Id;
        Check(session.RenamePlayer(id, "Alexandra").Success, "rename succeeds");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players[0].Id == id && session.View.Players[0].Name == "Alexandra", "rename keeps ID");
        Check(session.SetParticipation(id, false).Success, "pause succeeds");
        session = Session.Open(directory, Language.English);
        Check(!session.View.Players[0].Active, "pause is durable");
        Check(session.SetParticipation(id, true).Success, "resume succeeds");
        Check(session.RemovePlayer(id).Success, "remove succeeds");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Count == 1 && session.View.CanUndo, "removal and undo survive reopening");
        Check(session.UndoRemove().Success, "undo succeeds");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players[0].Id == id && session.View.Players[0].Active, "undo restores ID order and participation");
    }),
    ("duplicate names remain distinguishable after remove undo and restart", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex"); session.AddPlayer("Alex");
        var first = session.View.Players[0]; var second = session.View.Players[1];
        Check(first.Name == "Alex" && second.Name == "Alex", "entered names stay intact");
        Check(first.DisplayName != second.DisplayName, "two Alex players can be distinguished");
        session.RemovePlayer(first.Id);
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Single().DisplayName == second.DisplayName, "suffix stays stable after removal");
        session.UndoRemove();
        Check(session.View.Players[0].DisplayName == first.DisplayName, "suffix stays stable after undo");
        session.RenamePlayer(first.Id, "Jamie");
        Check(session.View.Players[0].Id == first.Id, "rename preserves identity");
    }),
    ("names accept Unicode text elements and reject invalid edits without saving", directory => {
        var session = Session.Open(directory, Language.English);
        Check(!session.AddPlayer(" \t ").Success, "blank rejected");
        Check(!session.AddPlayer("A\nB").Success, "control rejected");
        Check(!session.AddPlayer(new string('a',25)).Success, "overlong rejected");
        Check(session.AddPlayer(string.Concat(Enumerable.Repeat("e\u0301",24))).Success, "24 combining names accepted");
        var id = session.View.Players.Single().Id;
        Check(!session.RenamePlayer(id," ").Success, "invalid rename rejected");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Count == 1 && session.View.Players[0].Name == new string('é',24), "valid normalized name is retained");
    }),
    ("capacity rejects a new person without silently pausing anyone", directory => {
        var session = Session.Open(directory, Language.English);
        for (int i=1;i<=20;i++) Check(session.AddPlayer("Player " + i).Success, "20 current people accepted");
        var result = session.AddPlayer("Overflow");
        Check(!result.Success && result.Error == "ActiveFull", "capacity is reported, never a silent paused add");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Count == 20 && session.View.Players.All(p=>p.Active), "no inactive overflow saved");
        var id = session.View.Players[3].Id;
        session.RemovePlayer(id); session.AddPlayer("Replacement");
        Check(!session.UndoRemove().Success, "undo cannot exceed current capacity");
    }),
    ("reordering is durable atomic and respects stable Unicode identities", directory => {
        var session = Session.Open(directory, Language.German);
        foreach (var name in new[] { "Zoë", "李明", "Zoë", "Γιάννης" }) session.AddPlayer(name);
        var before = session.View.Players.ToArray();
        var ids = before.Select(p => p.Id).ToArray();
        var next = new[] { ids[3], ids[1], ids[0], ids[2] };
        Check(session.ReorderPlayers(ids, next).Success, "full stable-ID reorder succeeds");
        session.SetMode(GameMode.Classic);
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Select(p => p.Id).SequenceEqual(next), "order survives mode change and reopen");
        Check(session.View.Players.All(p => before.Single(old => old.Id == p.Id).DisplayName == p.DisplayName), "disambiguation unchanged");
        Check(!session.ReorderPlayers(ids, ids).Success, "stale pre-move request rejected");
        Check(!session.ReorderPlayers(next, new[] { ids[0], ids[0], ids[1], ids[2] }).Success, "duplicate IDs rejected");
        Check(!session.ReorderPlayers(next, ids.Take(3).ToArray()).Success, "omitted player rejected");
        Directory.CreateDirectory(Path.Combine(directory, "session.pending.json"));
        var failed = session.ReorderPlayers(next, ids);
        Check(!failed.Success && failed.Error == "SaveFailed", "denied write is reported");
        Check(session.View.Players.Select(p => p.Id).SequenceEqual(next), "failed write keeps visible order");
        Check(Session.Open(directory, Language.English).View.Players.Select(p => p.Id).SequenceEqual(next), "failed write keeps disk order");
        Directory.Delete(Path.Combine(directory, "session.pending.json"));
        Check(session.StartMatch().Success && session.Match.Owner.Id == ids[3], "next deal follows new first player");
        var match = Newtonsoft.Json.JsonConvert.SerializeObject(session.Match);
        Check(!session.ReorderPlayers(next, ids).Success, "live match blocks group reordering");
        Check(Newtonsoft.Json.JsonConvert.SerializeObject(session.Match) == match, "frozen deal unchanged");
    }),
    ("legacy inactive people remain saved and require explicit restoration", directory => {
        var session = Session.Open(directory, Language.German);
        for (int i = 0; i < 20; i++) session.AddPlayer("Saved " + i);
        foreach (var player in session.View.Players) session.SetParticipation(player.Id, false);
        for (int i = 0; i < 20; i++) session.AddPlayer("Current " + i);
        var before = session.View.Players.ToArray();
        string primary = File.ReadAllText(Path.Combine(directory, "session.json"));
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Count == 40 && session.View.ActiveCount == 20, "old 40-name compatibility envelope retained");
        Check(File.ReadAllText(Path.Combine(directory, "session.json")) == primary, "opening does not rewrite or activate legacy names");
        Check(!session.SetParticipation(before[0].Id, true).Success, "explicit restoration respects active capacity");
        Check(session.RemovePlayer(before[20].Id).Success, "remove current person");
        Check(session.SetParticipation(before[0].Id, true).Success, "explicit restoration fills vacancy");
        Check(session.RenamePlayer(before[1].Id, "Old friend").Success, "inactive name can be renamed");
        var ids = session.View.Players.Select(p => p.Id).ToArray();
        Check(session.ReorderPlayers(ids, ids.Reverse().ToArray()).Success, "legacy names remain part of exact atomic order");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Select(p => p.Id).SequenceEqual(ids.Reverse()), "legacy and current order durable");
        Check(session.View.Players.Single(p=>p.Id == before[0].Id).Active && !session.View.Players.Single(p=>p.Id == before[1].Id).Active, "only explicitly restored person becomes active");
    }),
    ("language and mode are durable and expose valid group readiness", directory => {
        var session = Session.Open(directory, Language.German);
        session.AddPlayer("A"); session.AddPlayer("B"); session.AddPlayer("C");
        Check(session.View.ReadyToStart, "three people can start Quick");
        Check(session.SetMode(GameMode.Classic).Success, "Classic selected");
        Check(!session.View.ReadyToStart && session.View.NeededPlayers == 1, "Classic needs four");
        Check(session.SetLanguage(Language.English).Success, "language changes");
        session = Session.Open(directory, Language.German);
        Check(session.View.Mode == GameMode.Classic && session.View.Language == Language.English, "settings retained");
        session.AddPlayer("D");
        Check(session.View.ReadyToStart, "four people can start Classic");
    }),
    ("a damaged primary restores a validated previous generation", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex"); session.AddPlayer("Bea");
        File.WriteAllText(Path.Combine(directory,"session.json"), "{interrupted");
        session = Session.Open(directory, Language.German);
        Check(session.View.Players.Count == 1 && session.View.Players[0].Name == "Alex", "previous confirmed generation recovered");
        Check(session.View.StorageNotice == "RecoveredBackup", "recovery visible to presentation");
        Check(session.AddPlayer("Chris").Success, "recovered session writable");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Count == 2, "recovery can be saved and reopened");
    }),
    ("failed writes retain memory and disk and can be retried", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex");
        Directory.CreateDirectory(Path.Combine(directory,"session.pending.json"));
        var result = session.AddPlayer("Bea");
        Check(!result.Success && result.Error == "SaveFailed", "failure returned instead of confirmed");
        Check(session.View.Players.Count == 1, "memory unchanged");
        Check(Session.Open(directory,Language.German).View.Players.Count == 1, "disk unchanged");
        Directory.Delete(Path.Combine(directory,"session.pending.json"));
        Check(session.AddPlayer("Bea").Success, "retry succeeds");
    }),
    ("unrecoverable data stays blocked until an explicit fresh-start acknowledgement", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex"); session.AddPlayer("Bea");
        File.WriteAllText(Path.Combine(directory,"session.json"), "broken");
        File.WriteAllText(Path.Combine(directory,"session.previous.json"), "also broken");
        session = Session.Open(directory, Language.English);
        Check(session.View.StorageNotice == "DamagedData", "damage is visible");
        Check(!session.AddPlayer("Silent reset").Success, "no implicit overwrite");
        Check(session.StartFreshAfterDamage().Success, "explicit reset available");
        Check(session.AddPlayer("New group").Success, "fresh group usable");
        Check(Session.Open(directory,Language.English).View.Players.Single().Name == "New group", "fresh group durable");
        Check(Directory.GetDirectories(directory,"damaged-*").Length == 1, "damaged generations preserved");
    }),
    ("newer save schemas are never rolled back or overwritten", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex"); session.AddPlayer("Bea");
        var path = Path.Combine(directory,"session.json");
        var futureEnvelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path)); futureEnvelope["Version"] = 99;
        var future = futureEnvelope.ToString();
        File.WriteAllText(path,future);
        session = Session.Open(directory,Language.German);
        Check(session.View.StorageNotice == "NewerVersion" && session.View.StorageBlocked, "newer version gets explicit block");
        Check(!session.AddPlayer("C").Success && !session.StartFreshAfterDamage().Success, "cannot overwrite newer save");
        Check(File.ReadAllText(path) == future, "future file remains intact");
    }),
    ("entered names cannot impersonate generated disambiguators", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex"); session.AddPlayer("Alex");
        var firstLabel = session.View.Players[0].DisplayName;
        session.AddPlayer(firstLabel);
        Check(session.View.Players.Select(p=>p.DisplayName).Distinct().Count() == 3, "all three display labels distinct");
        var labels = session.View.Players.Select(p=>p.DisplayName).ToArray();
        session = Session.Open(directory,Language.English);
        Check(session.View.Players.Select(p=>p.DisplayName).SequenceEqual(labels), "disambiguation durable");
    }),
    ("valid checksums do not make invalid group snapshots safe", directory => {
        var session = Session.Open(directory, Language.English);
        session.AddPlayer("Alex"); session.AddPlayer("Bea");
        var path = Path.Combine(directory,"session.json");
        var envelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
        var payload = Newtonsoft.Json.Linq.JObject.Parse((string)envelope["Payload"]);
        payload["Players"] = null;
        var invalid = payload.ToString(Newtonsoft.Json.Formatting.None);
        envelope["Payload"] = invalid;
        envelope["Checksum"] = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(invalid)));
        File.WriteAllText(path,envelope.ToString());
        session = Session.Open(directory,Language.English);
        Check(session.View.StorageNotice == "RecoveredBackup" && session.View.Players.Single().Name == "Alex", "invalid payload recovers the prior valid group");
    }),
    ("stale player actions are harmless and report what happened", directory => {
        var session = Session.Open(directory,Language.English); session.AddPlayer("Alex");
        var id = session.View.Players[0].Id;
        session.RemovePlayer(id);
        Check(session.RemovePlayer(id).Error == "PlayerNotFound", "double remove rejected");
        Check(session.RenamePlayer(id,"Ghost").Error == "PlayerNotFound", "stale rename rejected");
        Check(session.SetParticipation(id,true).Error == "PlayerNotFound", "stale toggle rejected");
        Check(session.UndoRemove().Success && session.View.Players.Single().Name == "Alex", "failed actions preserve undo");
    })
};
cases = cases.Concat(QuickCases.All).ToArray();
cases = cases.Concat(RecoveryCases.All).ToArray();
cases = cases.Concat(ClassicCases.All).ToArray();
cases = cases.Concat(ContentCases.All).ToArray();
cases = cases.Concat(NameCases.All).ToArray();
cases = cases.Concat(KingsCases.All).ToArray();
cases = cases.Concat(KingsEliminationCases.All).ToArray();
cases = cases.Concat(KingsLastChanceCases.All).ToArray();
cases = cases.Concat(KingsRecoveryCases.All).ToArray();
cases = cases.Concat(RoleCountCases.All).ToArray();
cases = cases.Concat(KingsStarterCases.All).ToArray();
if (args.Length > 0) cases = cases.Where(test => test.name.Contains(args[0],StringComparison.OrdinalIgnoreCase)).ToArray();
int failures = 0;
foreach (var test in cases) {
    var directory = Path.Combine(Path.GetTempPath(), "WordDeduction-tests", Guid.NewGuid().ToString("N"));
    var ioDiagnostics = new List<string>();
    bool failed = false;
    bool diagnose = Environment.GetEnvironmentVariable("WD_TEST_IO_DIAGNOSTICS") == "1";
    EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs> recordIo = (_, error) => {
        if (error.Exception is IOException && error.Exception is not FileNotFoundException && error.Exception is not DirectoryNotFoundException)
            ioDiagnostics.Add(error.Exception.ToString());
    };
    if (diagnose) AppDomain.CurrentDomain.FirstChanceException += recordIo;
    try { test.run(directory); Console.WriteLine("PASS " + test.name); }
    catch (Exception error) {
        failed = true; failures++; Console.WriteLine("FAIL " + test.name + ": " + error.Message);
        if (diagnose) { foreach (var io in ioDiagnostics.TakeLast(3)) Console.WriteLine("HOST IO: " + io); Console.WriteLine("Retained failed test snapshot: " + directory); }
    }
    finally {
        if (diagnose) AppDomain.CurrentDomain.FirstChanceException -= recordIo;
        if (!(failed && diagnose) && Directory.Exists(directory)) Directory.Delete(directory, true);
    }
}
Console.WriteLine($"{cases.Length - failures}/{cases.Length} passed");
return failures == 0 ? 0 : 1;

static void Check(bool actual, string expected) { if (!actual) throw new Exception(expected); }
