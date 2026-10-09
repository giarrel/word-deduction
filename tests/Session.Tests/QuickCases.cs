using WordDeduction;

static class QuickCases
{
    public static (string name, Action<string> run)[] All = {
        ("Quick deals are durable and private for every supported group size", directory => {
            for (int count = 3; count <= 20; count++) {
                string folder = Path.Combine(directory, count.ToString());
                var session = Session.Open(folder, Language.German, maximum => 0);
                for (int i = 0; i < count; i++) session.AddPlayer("Person " + i);
                Check(session.StartMatch().Success, "start succeeds");
                var matchId = session.Match.Id;
                var first = session.Match.Owner.Id;
                var firstWord = session.RevealWord(first);
                Check(!string.IsNullOrEmpty(firstWord), "word deliberately revealed");
                session = Session.Open(folder, Language.English, maximum => maximum - 1);
                Check(session.Match.Id == matchId && session.Match.Owner.Id == first, "same deal and owner reopen");
                Check(session.Match.Language == Language.German && !session.Match.CanAdvance, "reopened card starts covered");
                Check(session.RevealWord(first) == firstWord, "reopen never draws another word");
                session.HideWord();
                var words = new List<string>();
                while (session.Match.Phase == MatchPhase.Handoff) {
                    var owner = session.Match.Owner.Id;
                    words.Add(session.RevealWord(owner)); session.HideWord();
                    var advanced = session.AdvanceHandoff(owner);
                    Check(advanced.Success, "read and hidden card can advance: " + advanced.Error + "; size=" + count + "; owner=" + session.Match.HandoffNumber);
                }
                Check(words.Distinct().Count() == 2 && words.GroupBy(w => w).Any(g => g.Count() == 1), "exactly one different word, no White");
                Check(session.Match.Phase == MatchPhase.Clues && session.Match.Result == null, "one public clue round without secret results");
                Check(session.Match.Participants.Any(p => p.Id == session.Match.StartingPlayer.Id), "starting person belongs to group");
            }
        }),
        ("Quick spoken vote can be corrected and only confirmation reveals the result", directory => {
            var session = Dealt(directory);
            var people = session.Match.Participants;
            Check(session.BeginVote().Success, "vote opens after clues");
            Check(session.SelectSuspect(people[1].Id).Success, "first suspect selected");
            session = Session.Open(directory, Language.English, maximum => 0);
            Check(session.Match.SelectedSuspect.Id == people[1].Id && session.Match.Result == null, "reopened selection remains unconfirmed");
            Check(session.SelectSuspect(people[0].Id).Success, "suspect can be corrected");
            Check(session.ConfirmSuspect(people[1].Id).Error == "InvalidAction", "stale confirmation cannot confirm another selection");
            Check(session.ConfirmSuspect(people[0].Id).Success, "corrected selection confirmed");
            Check(session.Match.Result.Winner == Role.Civilian && session.Match.Result.Reason == Outcome.AllAdversariesEliminated, "catching the only adversary wins");
            Check(session.Match.Result.Roles.Count(r => r.Role == Role.Undercover) == 1, "full role reveal");
            Check(session.Match.Result.CivilianWord != session.Match.Result.UndercoverWord, "both words revealed");
            string oldId = session.Match.Id;
            Check(session.Rematch(oldId).Success && session.Match.Id != oldId, "direct rematch is a new deal");
            Check(!session.Rematch(oldId).Success && session.Match.Participants.Count == 3, "double rematch never skips a deal");
            while (session.Match.Phase == MatchPhase.Handoff) { var id = session.Match.Owner.Id; session.RevealWord(id); session.HideWord(); session.AdvanceHandoff(id); }
            session.BeginVote(); session.SelectSuspect(session.Match.Participants[1].Id); session.ConfirmSuspect(session.Match.Participants[1].Id);
            Check(session.Match.Result.Winner == Role.Undercover, "accusing a civilian lets Undercover win");
            Check(session.View.ActiveCount == 3, "all players remain active");
        }),
        ("Quick has one runoff and protects the active group until confirmed abandonment", directory => {
            var session = Dealt(directory);
            var id = session.Match.Id;
            Check(!session.AddPlayer("Late").Success && !session.RenamePlayer(session.View.Players[0].Id,"Changed").Success, "live participants frozen");
            Check(!session.SetLanguage(Language.German).Success && !session.SetMode(GameMode.Classic).Success, "live language and mode frozen");
            Check(!session.SetParticipation(session.View.Players[0].Id,false).Success && !session.RemovePlayer(session.View.Players[0].Id).Success, "participation frozen");
            session.BeginVote();
            Check(session.RecordTie(false).Success && session.Match.Runoff, "first tie starts runoff");
            Check(!session.RecordTie(false).Success && session.Match.Result == null, "double first-tie action cannot end the match");
            session = Session.Open(directory, Language.German);
            Check(session.Match.Runoff && session.Match.Result == null, "runoff is durable");
            Check(session.RecordTie(true).Success, "second tie ends match");
            Check(session.Match.Result.Reason == Outcome.RepeatedTie && session.Match.Result.Winner == Role.Undercover, "Undercover escapes tied vote");
            Check(session.ReturnToGroup(id).Success && session.View.ActiveCount == 3, "group persists after result");
            session.StartMatch(); var nextId = session.Match.Id;
            Check(!session.AbandonMatch(id).Success && session.Match.Id == nextId, "old abandon cannot end a new deal");
            Check(session.AbandonMatch(nextId).Success && session.Match == null && session.View.ActiveCount == 3, "explicit abandon retains group");
        }),
        ("failed deal handoff confirmation and rematch never expose uncommitted progress", directory => {
            var session = Session.Open(directory,Language.English, maximum => 0);
            foreach (var name in new[] { "A", "B", "C" }) session.AddPlayer(name);
            var pending = Path.Combine(directory,"session.pending.json");
            Directory.CreateDirectory(pending);
            Check(session.StartMatch().Error == "SaveFailed" && session.Match == null && Session.Open(directory,Language.German).Match == null, "failed deal stays outside match");
            Directory.Delete(pending); session.StartMatch();
            var owner = session.Match.Owner.Id;
            Check(!session.AdvanceHandoff(owner).Success, "unread card cannot advance");
            var word = session.RevealWord(owner);
            Check(!session.AdvanceHandoff(owner).Success, "open card cannot advance"); session.HideWord();
            Directory.CreateDirectory(pending);
            Check(session.AdvanceHandoff(owner).Error == "SaveFailed" && session.Match.Owner.Id == owner, "failed handoff retains owner");
            Check(Session.Open(directory,Language.English).RevealWord(owner) == word, "failed handoff keeps persisted card");
            Directory.Delete(pending); Check(session.AdvanceHandoff(owner).Success, "handoff retry succeeds");
            Check(!session.AdvanceHandoff(owner).Success, "stale duplicate does not skip owner");
            while (session.Match.Phase == MatchPhase.Handoff) { owner = session.Match.Owner.Id; session.RevealWord(owner); session.HideWord(); session.AdvanceHandoff(owner); }
            session.BeginVote(); owner = session.Match.Participants[0].Id; session.SelectSuspect(owner);
            Directory.CreateDirectory(pending);
            Check(session.ConfirmSuspect(owner).Error == "SaveFailed" && session.Match.Result == null, "failed confirmation reveals nothing");
            Check(Session.Open(directory,Language.English).Match.Result == null, "unconfirmed vote persists");
            Directory.Delete(pending); session.ConfirmSuspect(owner);
            string matchId = session.Match.Id; Directory.CreateDirectory(pending);
            Check(session.Rematch(matchId).Error == "SaveFailed" && session.Match.Id == matchId && session.Match.Result != null, "failed rematch retains result");
            Directory.Delete(pending); Check(session.Rematch(matchId).Success, "rematch retry succeeds");
        }),
        ("legacy group migration keeps identities settings and undo without rewriting on open", directory => {
            var session = Session.Open(directory,Language.German);
            foreach (var name in new[] { "Alex", "Alex", "Chris", "Dana" }) session.AddPlayer(name);
            session.SetMode(GameMode.Classic); session.RemovePlayer(session.View.Players[3].Id);
            var path = Path.Combine(directory,"session.json");
            SavedSessionFixture.RewritePrimary(directory, payload => { payload.Remove("QuickRoles"); payload.Remove("ClassicRoles"); });
            var legacyEnvelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path)); legacyEnvelope["Version"] = 1;
            var legacy = legacyEnvelope.ToString(); File.WriteAllText(path,legacy);
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            session = Session.Open(directory,Language.English);
            Check(session.View.Players.Select(p => p.Id).SequenceEqual(ids) && session.View.CanUndo && session.View.Mode == GameMode.Classic && session.View.Language == Language.German, "V1 state imported intact");
            Check(File.ReadAllText(path) == legacy, "opening does not rewrite legacy file");
            Check(session.StartMatch().Error == "NotEnoughPlayers", "Classic cannot silently start Quick");
            session.UndoRemove(); session.SetMode(GameMode.Quick); Check(session.StartMatch().Success, "migrated group can start Quick");
            Check(Session.Open(directory,Language.English).Match.Participants.Count == 4, "new format reopens full deal");
        }),
        ("valid checksums do not admit invalid match snapshots", directory => {
            var session = Dealt(directory); session.BeginVote();
            var path = Path.Combine(directory,"session.json");
            var envelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
            var payload = Newtonsoft.Json.Linq.JObject.Parse((string)envelope["Payload"]);
            payload["Match"]["Participants"][0]["Role"] = 99;
            var invalid = payload.ToString(Newtonsoft.Json.Formatting.None);
            envelope["Payload"] = invalid;
            envelope["Checksum"] = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(invalid)));
            File.WriteAllText(path,envelope.ToString());
            session = Session.Open(directory,Language.English);
            Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Phase == MatchPhase.Clues, "invalid match recovers previous valid phase");
        }),
        ("rematch validates changed group size and mode through the same start rules", directory => {
            var session = Dealt(directory); session.BeginVote(); session.RecordTie(false); session.RecordTie(true);
            string id = session.Match.Id;
            session.SetParticipation(session.View.Players[0].Id,false);
            Check(!session.Rematch(id).Success && session.Match.Phase == MatchPhase.Result, "insufficient group cannot start a rematch");
            session.SetParticipation(session.View.Players[0].Id,true); session.SetMode(GameMode.Classic);
            Check(session.Rematch(id).Error == "NotEnoughPlayers" && session.Match.Phase == MatchPhase.Result, "Classic cannot start Quick rules from results");
            session.SetMode(GameMode.Quick); Check(session.Rematch(id).Success, "valid group rematches");
        }),
        ("all seats can be Undercover and start without leaking secret fields through safe views", directory => {
            for (int seat=0;seat<4;seat++) {
                int draw=0;
                var session=Session.Open(Path.Combine(directory,seat.ToString()),Language.English, maximum => draw++==0 ? seat : maximum-1);
                foreach(var name in new[]{"A","B","C","D"}) session.AddPlayer(name);
                session.StartMatch();
                Check(session.Match.StartingPlayer.Id==session.Match.Participants[3].Id,"starting seat independently selected, including Undercover");
                string safe=Newtonsoft.Json.JsonConvert.SerializeObject(session.Match);
                Check(!safe.Contains("Word") && !safe.Contains("Role") && !safe.Contains("PairId"),"ordinary match view has no private payload fields");
                while(session.Match.Phase==MatchPhase.Handoff) { var owner=session.Match.Owner.Id; session.RevealWord(owner); session.HideWord(); session.AdvanceHandoff(owner); }
                session.BeginVote(); session.SelectSuspect(session.Match.Participants[seat].Id); session.ConfirmSuspect(session.Match.Participants[seat].Id);
                Check(session.Match.Result.Roles[seat].Role==Role.Undercover && session.Match.Result.Winner==Role.Civilian,"chosen seat receives the sole undercover role");
            }
        })
    };
    static void Check(bool actual, string expected) { if (!actual) throw new Exception(expected); }
    static Session Dealt(string directory)
    {
        var session = Session.Open(directory, Language.English, maximum => 0);
        foreach (var name in new[] { "Alex", "Bea", "Chris" }) session.AddPlayer(name);
        session.StartMatch();
        while (session.Match.Phase == MatchPhase.Handoff) { var id = session.Match.Owner.Id; session.RevealWord(id); session.HideWord(); session.AdvanceHandoff(id); }
        return session;
    }
}
