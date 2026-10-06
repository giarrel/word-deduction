using WordDeduction;

static class RecoveryCases
{
    public static (string name, Action<string> run)[] All = {
        ("unreadable storage paths cannot masquerade as a new group", directory => {
            Directory.CreateDirectory(Path.Combine(directory, "session.json"));
            var session = Session.Open(directory, Language.English);
            Check(session.View.StorageBlocked && session.View.StorageNotice == "ReadFailed", "unreadable generation blocks with a clear read failure");
            Check(!session.AddPlayer("Overwrite").Success && !session.StartFreshAfterDamage().Success, "read failures cannot reset storage");
        }),
        ("an interrupted first save requires acknowledgement and preserves the unfinished file", directory => {
            Directory.CreateDirectory(directory);
            var pending = Path.Combine(directory, "session.pending.json");
            const string unfinished = "{unfinished first save";
            File.WriteAllText(pending, unfinished);
            var session = Session.Open(directory, Language.English);
            Check(session.View.StorageBlocked && session.View.StorageNotice == "DamagedData", "orphan pending is disclosed rather than silently reset");
            Check(!session.AddPlayer("Silent reset").Success && File.ReadAllText(pending) == unfinished, "unacknowledged data remains untouched");
            Check(session.StartFreshAfterDamage().Success, "explicit fresh start succeeds");
            Check(File.ReadAllText(Path.Combine(Directory.GetDirectories(directory, "damaged-*").Single(), "session.pending.json")) == unfinished, "unfinished evidence is archived before reuse");
            Check(session.AddPlayer("New group").Success && Session.Open(directory, Language.German).View.Players.Single().Name == "New group", "fresh state is durable");
        }),
        ("a newer backup cannot be overwritten by an older readable primary", directory => {
            var session = Session.Open(directory, Language.English);
            session.AddPlayer("A"); session.AddPlayer("B");
            var path = Path.Combine(directory, "session.previous.json");
            var envelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
            envelope["Version"] = 999;
            var future = envelope.ToString(); File.WriteAllText(path, future);
            session = Session.Open(directory, Language.German);
            Check(session.View.StorageBlocked && session.View.StorageNotice == "NewerVersion", "unsupported generation is explicitly blocked");
            Check(!session.AddPlayer("C").Success && !session.StartFreshAfterDamage().Success && File.ReadAllText(path) == future, "future backup is preserved");
        }),
        ("every Quick phase restores the same confirmed deal without drawing or exposing a card", directory => {
            var session = Quick(directory);
            ReopenUnchanged(directory, session);
            var owner = session.Match.Owner.Id; var word = session.RevealWord(owner);
            session = Session.Open(directory, Language.German, NoDraw);
            Check(!session.Match.CanAdvance && session.RevealWord(owner) == word, "open card restarts covered with the same private word");
            session.HideWord(); session.AdvanceHandoff(owner);
            ReopenUnchanged(directory, session);
            DealCards(session); ReopenUnchanged(directory, session);
            session.BeginVote(); ReopenUnchanged(directory, session);
            session.SelectSuspect(session.Match.Participants[0].Id); ReopenUnchanged(directory, session);
            session.CancelSuspect(); session.RecordTie(false); ReopenUnchanged(directory, session);
            session.RecordTie(true); ReopenUnchanged(directory, session);
        }),
        ("partial and complete pending writes never become confirmed actions on reopening", directory => {
            var session = Quick(directory); DealCards(session);
            var primary = Path.Combine(directory, "session.json");
            var pending = Path.Combine(directory, "session.pending.json");
            var confirmed = File.ReadAllText(primary);
            session.BeginVote();
            File.Move(primary, pending); File.WriteAllText(primary, confirmed);
            session = Session.Open(directory, Language.German, NoDraw);
            Check(session.Match.Phase == MatchPhase.Clues && session.View.StorageNotice == null, "fully flushed but uncommitted vote is ignored");
            File.WriteAllText(pending, "{partial");
            ReopenUnchanged(directory, session);
            Check(session.BeginVote().Success && Session.Open(directory, Language.English, NoDraw).Match.Phase == MatchPhase.Vote, "retry commits the actual action");
        }),
        ("a damaged backup leaves a healthy primary usable and a missing primary visibly recovers", directory => {
            var session = Quick(directory); DealCards(session); session.BeginVote();
            var primary = Path.Combine(directory, "session.json");
            var backup = Path.Combine(directory, "session.previous.json");
            var confirmed = File.ReadAllText(primary);
            File.WriteAllText(backup, "damaged backup"); ReopenUnchanged(directory, session);
            Check(Session.Open(directory, Language.English).View.StorageNotice == null, "healthy primary needs no recovery");
            File.WriteAllText(backup, confirmed); File.Delete(primary);
            session = Session.Open(directory, Language.German, NoDraw);
            Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Phase == MatchPhase.Vote, "missing primary restores the confirmed backup visibly");
            Check(session.SelectSuspect(session.Match.Participants[1].Id).Success, "recovered state can commit again");
        }),
        ("replacement denial after flushing leaves the previous vote intact and retryable", directory => {
            var session = Quick(directory); DealCards(session); session.BeginVote();
            var suspect = session.Match.Participants[1].Id; session.SelectSuspect(suspect);
            var primary = Path.Combine(directory, "session.json");
            using (var locked = new FileStream(primary, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var result = session.ConfirmSuspect(suspect);
                Check(!result.Success && result.Error == "SaveFailed" && session.Match.Result == null && session.Match.SelectedSuspect.Id == suspect, "denied atomic replace never acknowledges the result");
            }
            session = Session.Open(directory, Language.German, NoDraw);
            Check(session.Match.Result == null && session.Match.SelectedSuspect.Id == suspect, "a flushed pending result is never restored");
            Check(session.ConfirmSuspect(suspect).Success && Session.Open(directory, Language.English, NoDraw).Match.Result != null, "retry commits once lock is removed");
        }),
        ("missing required snapshot fields cannot silently become an empty saved group", directory => {
            var session = Session.Open(directory, Language.English);
            session.AddPlayer("A"); session.AddPlayer("B");
            var primary = Path.Combine(directory, "session.json");
            var envelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(primary));
            envelope["Payload"] = "{}";
            envelope["Checksum"] = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("{}")));
            File.WriteAllText(primary, envelope.ToString());
            session = Session.Open(directory, Language.German);
            Check(session.View.StorageNotice == "RecoveredBackup" && session.View.Players.Single().Name == "A", "missing fields recover the validated previous generation");
        }),
        ("denied reads never fall back past an inaccessible confirmed generation", directory => {
            var session = Quick(directory); DealCards(session); session.BeginVote();
            using (var locked = new FileStream(Path.Combine(directory, "session.json"), FileMode.Open, FileAccess.Read, FileShare.None)) {
                var blocked = Session.Open(directory, Language.German, NoDraw);
                Check(blocked.View.StorageBlocked && blocked.View.StorageNotice == "ReadFailed", "read denial is explicit rather than restoring an older phase");
                Check(!blocked.AddPlayer("Overwrite").Success && !blocked.StartFreshAfterDamage().Success, "inaccessible state cannot be overwritten");
            }
            ReopenUnchanged(directory, session);
        }),
        ("failed cancel and abandonment keep the selected vote and saved group", directory => {
            var session = Quick(directory); DealCards(session); session.BeginVote();
            var matchId = session.Match.Id; var suspect = session.Match.Participants[1].Id;
            session.SelectSuspect(suspect);
            var pending = Path.Combine(directory, "session.pending.json"); Directory.CreateDirectory(pending);
            Check(session.CancelSuspect().Error == "SaveFailed" && session.Match.SelectedSuspect.Id == suspect, "failed correction retains the selected suspect");
            Check(session.AbandonMatch(matchId).Error == "SaveFailed" && session.Match.Id == matchId && session.View.ActiveCount == 3, "failed abandonment retains match and group");
            ReopenUnchanged(directory, session); Directory.Delete(pending);
            Check(session.CancelSuspect().Success && session.Match.SelectedSuspect == null, "correction retry succeeds");
            Check(session.AbandonMatch(matchId).Success && Session.Open(directory, Language.English).Match == null && session.View.ActiveCount == 3, "explicit abandonment retry keeps group durable");
        }),
        ("every Classic phase including both White judgments preserves its deal and history across interrupted saves", directory => {
            foreach (bool correct in new[] { false, true }) {
                var folder = Path.Combine(directory, correct.ToString());
                var session = Session.Open(folder, Language.German, maximum => 0);
                foreach (var name in new[] { "A", "B", "C", "D", "E" }) Check(session.AddPlayer(name).Success, "Classic fixture person saves");
                session.SetMode(GameMode.Classic); session.SetWhitePreference(true); Check(session.StartMatch().Success, "Classic fixture starts");
                ClassicCheckpoint(folder, session);
                var owner = session.Match.Owner.Id; var privateWord = session.RevealWord(owner);
                var reopened = Session.Open(folder, Language.English, NoDraw);
                Check(!reopened.Match.CanAdvance && reopened.RevealWord(owner) == privateWord, "Classic handoff reopens covered with its original word");
                session.HideWord(); session.AdvanceHandoff(owner); ClassicCheckpoint(folder, session);
                DealCards(session); ClassicCheckpoint(folder, session);
                var people = session.Match.Participants;
                session.BeginVote(); ClassicCheckpoint(folder, session);
                session.SelectSuspect(people[2].Id); ClassicCheckpoint(folder, session);
                session.CancelSuspect(); session.RecordTie(false); ClassicCheckpoint(folder, session);
                session.RecordTie(true); ClassicCheckpoint(folder, session);
                session.BeginVote(); session.SelectSuspect(people[0].Id); session.ConfirmSuspect(people[0].Id);
                Check(session.Match.Phase == MatchPhase.Elimination, "first adversary elimination stays live"); ClassicCheckpoint(folder, session);
                ReplacementDenied(folder, session, () => session.ContinueRound(session.Match.Round));
                Check(session.ContinueRound(session.Match.Round).Success, "next-round retry saves"); ClassicCheckpoint(folder, session);
                session.BeginVote(); session.SelectSuspect(people[1].Id); session.ConfirmSuspect(people[1].Id);
                Check(session.Match.Phase == MatchPhase.WhiteGuess, "last adversary still receives White judgment"); ClassicCheckpoint(folder, session);
                ReplacementDenied(folder, session, () => session.ResolveWhiteGuess(people[1].Id, correct));
                Check(session.ResolveWhiteGuess(people[1].Id, correct).Success, "White judgment retry saves");
                Check(session.Match.Result.WinningRoles.Single() == (correct ? Role.White : Role.Civilian), "both judgments preserve their correct outcome"); ClassicCheckpoint(folder, session);
            }
        })
    };
    static void Check(bool actual, string expected) { if (!actual) throw new Exception(expected); }
    static int NoDraw(int maximum) => throw new Exception("Restoration must not draw random values.");
    static Session Quick(string directory)
    {
        var session = Session.Open(directory, Language.English, maximum => 0);
        foreach (var name in new[] { "A", "B", "C" }) Check(session.AddPlayer(name).Success, "fixture person saves");
        Check(session.StartMatch().Success, "fixture match starts");
        return session;
    }
    static void DealCards(Session session)
    {
        while (session.Match.Phase == MatchPhase.Handoff) {
            var owner = session.Match.Owner.Id; session.RevealWord(owner); session.HideWord();
            Check(session.AdvanceHandoff(owner).Success, "fixture card advances");
        }
    }
    static void ReopenUnchanged(string directory, Session session)
    {
        var path = Path.Combine(directory, "session.json");
        var saved = File.ReadAllText(path);
        var before = Newtonsoft.Json.JsonConvert.SerializeObject(session.Match);
        var group = Newtonsoft.Json.JsonConvert.SerializeObject(session.View.Players);
        var reopened = Session.Open(directory, Language.German, NoDraw);
        Check(Newtonsoft.Json.JsonConvert.SerializeObject(reopened.Match) == before, "same public phase, owners, selection and result reopen");
        Check(Newtonsoft.Json.JsonConvert.SerializeObject(reopened.View.Players) == group, "saved group identities and participation stay unchanged");
        Check(File.ReadAllText(path) == saved, "opening preserves the exact deal, word history and checksum bytes");
    }
    static void ClassicCheckpoint(string directory, Session session)
    {
        ReopenUnchanged(directory, session);
        var pending = Path.Combine(directory, "session.pending.json");
        File.WriteAllText(pending, "{interrupted write");
        ReopenUnchanged(directory, session); File.Delete(pending);
        Directory.CreateDirectory(pending);
        Check(session.AbandonMatch(session.Match.Id).Error == "SaveFailed", "uncommitted abandonment cannot discard this Classic phase");
        ReopenUnchanged(directory, session); Directory.Delete(pending);
    }
    static void ReplacementDenied(string directory, Session session, Func<CommandResult> action)
    {
        using (var locked = new FileStream(Path.Combine(directory, "session.json"), FileMode.Open, FileAccess.Read, FileShare.Read))
            Check(action().Error == "SaveFailed", "flushed next phase is not acknowledged when replacement is denied");
        ReopenUnchanged(directory, session);
        Check(File.Exists(Path.Combine(directory, "session.pending.json")), "real denial occurred after the pending generation was flushed");
    }
}
