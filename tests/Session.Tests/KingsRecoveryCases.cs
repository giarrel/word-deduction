using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WordDeduction;

static class KingsRecoveryCases
{
    public static readonly (string name, Action<string> run)[] All = {
        ("Kings recovery rejects a saved evil King without any ordinary teammate", directory => {
            var session = Kings(directory);
            Advance(session);
            var backup = File.ReadAllText(Path.Combine(directory, "session.previous.json"));
            SavedSessionFixture.RewritePrimary(directory, payload => {
                foreach (var person in payload["Match"]["Participants"].OfType<JObject>())
                    if ((int)person["Role"] == (int)Role.Undercover) person["Role"] = (int)Role.Civilian;
            });
            session = Session.Open(directory, Language.German, NoDraw);
            Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Owner != null,
                "invalid initial composition restores the earlier covered handoff with an explicit notice");
            Check(File.ReadAllText(Path.Combine(directory, "session.previous.json")) == backup, "recovery preserves valid backup bytes");
        }),
        ("Kings recovery preserves every durable phase and all six endings across interrupted replacement", directory => {
            foreach (string ending in new[] { "good-king", "two-kings", "word-correct", "word-incorrect", "king-correct", "king-incorrect" })
            {
                string path = Path.Combine(directory, ending); var session = Kings(path);
                Checkpoint(path, session);
                var cards = new List<PrivateCardView>();
                while (session.Match.Phase == MatchPhase.Handoff)
                {
                    string owner = session.Match.Owner.Id; cards.Add(session.RevealCard(owner)); session.HideWord();
                    Commit(path, session, () => session.AdvanceHandoff(owner));
                }
                string good = cards.Single(c => c.Kind == PrivateCardKind.GoodKing).Owner.Id;
                string evil = cards.Single(c => c.Kind == PrivateCardKind.EvilKing).Owner.Id;
                var ordinary = cards.Where(c => c.Kind == PrivateCardKind.Word).Select(c => c.Owner.Id).ToArray();
                string id = session.Match.Id;
                Commit(path, session, () => session.SelectElimination(id, 0, ordinary[0]));
                Commit(path, session, () => session.CancelElimination(id, 0, ordinary[0]));
                if (ending == "two-kings") foreach (string person in ordinary) Eliminate(path, session, person);
                else if (ending == "good-king") Eliminate(path, session, good);
                else
                {
                    Eliminate(path, session, ordinary[0]);
                    Eliminate(path, session, evil);
                    bool correct = ending.EndsWith("-correct", StringComparison.Ordinal);
                    if (ending.StartsWith("word-", StringComparison.Ordinal))
                    {
                        Commit(path, session, () => session.ChooseLastChance(id, LastChanceChoice.Word));
                        Commit(path, session, () => session.ConfirmLastChanceAnswer(id));
                        Commit(path, session, () => session.ResolveLastChanceWord(id, correct));
                    }
                    else
                    {
                        Commit(path, session, () => session.ChooseLastChance(id, LastChanceChoice.King));
                        Commit(path, session, () => session.SelectLastChanceKing(id, ordinary[1]));
                        Commit(path, session, () => session.CancelLastChanceKing(id, ordinary[1]));
                        string target = correct ? good : ordinary[1];
                        Commit(path, session, () => session.SelectLastChanceKing(id, target));
                        Commit(path, session, () => session.ConfirmLastChanceKing(id, target));
                    }
                }
                Check(session.Match.Phase == MatchPhase.Result && session.Match.Result.Roles.Count == 5, "complete team result restores");
                Commit(path, session, () => session.Rematch(id));
                Check(session.Match.Participants.Count == 5 && session.View.ActiveCount == 5, "rematch restores the entire group");
            }
        }),
        ("Kings recovery distinguishes damaged missing unreadable and newer committed generations", directory => {
            foreach (string fault in new[] { "broken-primary", "missing-primary", "both-broken", "newer-primary", "newer-backup", "unreadable-primary" })
            {
                string path = Path.Combine(directory, fault); var session = Kings(path); Advance(session);
                string primary = Path.Combine(path, "session.json"), backup = Path.Combine(path, "session.previous.json");
                string previous = File.ReadAllText(backup); string oldOwner = session.Match.Participants[0].Id;
                if (fault == "missing-primary") File.Delete(primary);
                else if (fault == "broken-primary" || fault == "both-broken") File.WriteAllText(primary, "broken");
                if (fault == "both-broken") File.WriteAllText(backup, "broken too");
                if (fault.StartsWith("newer-", StringComparison.Ordinal))
                {
                    string target = fault == "newer-primary" ? primary : backup;
                    var envelope = JObject.Parse(File.ReadAllText(target)); envelope["Version"] = 999; File.WriteAllText(target, envelope.ToString());
                }
                using var locked = fault == "unreadable-primary" ? new FileStream(primary, FileMode.Open, FileAccess.Read, FileShare.None) : null;
                session = Session.Open(path, Language.German, NoDraw);
                if (fault == "broken-primary" || fault == "missing-primary")
                {
                    Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Owner.Id == oldOwner && !session.Match.CanAdvance,
                        "backup recovery is visibly older and covered; zero rollback is not promised");
                    Check(session.RevealCard(oldOwner) != null, "recovered unfinished handoff is usable"); session.HideWord();
                    Check(session.AdvanceHandoff(oldOwner).Success && File.ReadAllText(backup) == previous, "first repair preserves the validated backup");
                }
                else
                {
                    string expected = fault == "both-broken" ? "DamagedData" : fault == "unreadable-primary" ? "ReadFailed" : "NewerVersion";
                    Check(session.View.StorageBlocked && session.View.StorageNotice == expected && session.Match == null, "unsafe files block play and disclosure explicitly");
                    Check(!session.AddPlayer("Overwrite").Success && !session.StartMatch().Success, "blocked files cannot become a fresh match silently");
                    if (fault != "both-broken") Check(!session.StartFreshAfterDamage().Success, "unreadable and future data cannot be reset");
                    else
                    {
                        Check(session.StartFreshAfterDamage().Success, "damaged data needs deliberate reset");
                        string archive = Directory.GetDirectories(path, "damaged-*").Single();
                        Check(File.ReadAllText(Path.Combine(archive, "session.json")) == "broken" && File.ReadAllText(Path.Combine(archive, "session.previous.json")) == "broken too", "both damaged generations are preserved");
                    }
                }
            }
        })
    };
    static Session Kings(string directory)
    {
        var session = Session.Open(directory, Language.English, _ => 0);
        foreach (var name in new[] { "Alex", "Alex", "König 👑", "Dana", "Eli" }) Check(session.AddPlayer(name).Success, "group saves");
        Check(session.SetMode(GameMode.Kings).Success && session.StartMatch().Success, "Kings starts");
        return session;
    }
    static void Advance(Session session)
    {
        string owner = session.Match.Owner.Id;
        Check(session.RevealCard(owner) != null, "current card opens"); session.HideWord();
        Check(session.AdvanceHandoff(owner).Success, "handoff saves");
    }
    static int NoDraw(int maximum) => throw new Exception("Recovery must not draw new roles or words.");
    static void Eliminate(string path, Session session, string person)
    {
        string id = session.Match.Id; int count = session.Match.Participants.Count - session.Match.Survivors.Count;
        Commit(path, session, () => session.SelectElimination(id, count, person));
        Commit(path, session, () => session.ConfirmElimination(id, count, person));
    }
    static void Checkpoint(string path, Session session)
    {
        string primary = Path.Combine(path, "session.json"), saved = File.ReadAllText(primary);
        var reopened = Session.Open(path, Language.German, NoDraw);
        Check(!reopened.View.StorageBlocked && reopened.View.StorageNotice == null && reopened.Match.Id == session.Match.Id && reopened.Match.Phase == session.Match.Phase, "same match and phase reopen without new draws");
        Check(JsonConvert.SerializeObject(reopened.View) == JsonConvert.SerializeObject(session.View), "group, identities, names, preferences and language remain unchanged");
        Check(File.ReadAllText(primary) == saved, "opening preserves complete frozen deal, history and both checksum bytes");
        if (reopened.Match.Phase == MatchPhase.Handoff) Check(!reopened.Match.CanAdvance, "resumed handoff is covered and must be read");
        else Check(JsonConvert.SerializeObject(reopened.Match) == JsonConvert.SerializeObject(session.Match), "pending selections, disclosure and result persist exactly");
        File.WriteAllText(Path.Combine(path, "session.pending.json"), "{interrupted write");
        Check(JsonConvert.SerializeObject(Session.Open(path, Language.English, NoDraw).Match) == JsonConvert.SerializeObject(reopened.Match), "an incomplete pending generation cannot become committed progress");
    }
    static void Commit(string path, Session session, Func<CommandResult> command)
    {
        string before = JsonConvert.SerializeObject(session.Match);
        string primary = Path.Combine(path, "session.json"), saved = File.ReadAllText(primary);
        using (var locked = new FileStream(primary, FileMode.Open, FileAccess.Read, FileShare.Read))
            Check(command().Error == "SaveFailed", "failed atomic replacement reports SaveFailed after flushing the pending generation");
        Check(File.ReadAllText(primary) == saved && JsonConvert.SerializeObject(session.Match) == before, "failed replacement retains exact committed bytes and live disclosure");
        Checkpoint(path, session);
        Check(command().Success, "the same action commits once storage permits it");
        Checkpoint(path, session);
    }
    static void Check(bool condition, string expected) { if (!condition) throw new Exception(expected); }
}
