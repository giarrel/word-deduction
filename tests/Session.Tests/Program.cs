using WordDeduction;

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
    ("capacity keeps overflow players paused and allows a waiting player to join", directory => {
        var session = Session.Open(directory, Language.English);
        for (int i=1;i<=40;i++) Check(session.AddPlayer("Player " + i).Success, "saved capacity accepts 40");
        Check(session.View.Players.Count(p=>p.Active) == 20, "only 20 people active");
        Check(!session.AddPlayer("41").Success, "saved capacity rejects 41st");
        var waiting = session.View.Players[20].Id;
        Check(!session.SetParticipation(waiting,true).Success, "cannot activate above 20");
        session.SetParticipation(session.View.Players[0].Id,false);
        Check(session.SetParticipation(waiting,true).Success, "waiting person can take free place");
        session.RemovePlayer(waiting); session.AddPlayer("Replacement");
        Check(!session.UndoRemove().Success, "undo cannot exceed saved capacity");
        session = Session.Open(directory, Language.English);
        Check(session.View.Players.Count == 40 && session.View.Players.Count(p=>p.Active) == 20, "capacity survives restart");
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
