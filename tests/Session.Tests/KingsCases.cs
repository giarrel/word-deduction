using WordDeduction;

internal static class KingsCases
{
    public static readonly (string name, Action<string> run)[] All = {
        ("Kings selects a durable third mode with a five person minimum and adaptive majority", directory => {
            var session = Session.Open(directory, Language.German);
            for (int i = 0; i < 4; i++) Check(session.AddPlayer("Person " + i).Success, "add player");
            Check(session.SetMode((GameMode)2).Success, "Kings can be selected");
            Check(!session.View.ReadyToStart && session.View.NeededPlayers == 1, "Kings needs five people");
            Check(!session.StartMatch().Success, "four people cannot deal Kings");
            session.AddPlayer("Fifth");
            session = Session.Open(directory, Language.English);
            Check(session.View.Mode == (GameMode)2 && session.View.Language == Language.German, "mode and language persist");
            Check(session.View.ReadyToStart && session.View.UndercoverCount == 1 && session.View.WhiteCount == 1 && session.View.CivilianCount == 3, "five people means three good, one ordinary Undercover and White");
            for (int i = 5; i < 20; i++) session.AddPlayer("Person " + i);
            Check(session.View.UndercoverCount == 3 && session.View.WhiteCount == 1 && session.View.CivilianCount == 16, "twenty-person adaptive default");
        }),
        ("Kings explicit count survives smaller groups and mode changes without losing the request", directory => {
            var session = Session.Open(directory, Language.English);
            for (int i = 0; i < 20; i++) session.AddPlayer("Player " + i);
            session.SetMode(GameMode.Kings);
            Check(session.View.KingsUndercoverPreference == null && session.View.UndercoverCount == 3, "default stays adaptive until chosen");
            Check(session.View.KingsUndercoverLimit == 8, "twenty people allow eight ordinary Undercover plus White");
            Check(!session.SetKingsUndercoverPreference(0).Success && !session.SetKingsUndercoverPreference(9).Success, "out of bounds explicit counts rejected");
            Check(session.SetKingsUndercoverPreference(8).Success, "maximum request accepted");
            var paused = session.View.Players.Skip(5).Select(p => p.Id).ToArray();
            foreach (var id in paused) session.SetParticipation(id, false);
            session = Session.Open(directory, Language.German);
            Check(session.View.KingsUndercoverPreference == 8 && session.View.UndercoverCount == 1 && session.View.KingsCountAdjusted, "effective count clamps but saved request survives");
            Check(!session.SetKingsUndercoverPreference(2).Success && session.View.KingsUndercoverPreference == 8, "an invalid replacement cannot erase the saved preference");
            session.SetMode(GameMode.Quick);
            Check(session.View.UndercoverCount == 1 && session.View.WhiteCount == 0, "Quick ignores Kings preference");
            session.SetMode(GameMode.Classic); session.SetWhitePreference(true);
            Check(session.View.UndercoverCount == 1 && session.View.WhiteCount == 1, "Classic keeps its existing effective counts");
            foreach (var id in paused) session.SetParticipation(id, true);
            session.SetMode(GameMode.Kings);
            Check(session.View.UndercoverCount == 8 && !session.View.KingsCountAdjusted, "returning group restores saved request");
            Check(session.SetKingsUndercoverPreference(null).Success && session.View.UndercoverCount == 3, "automatic default can be restored");
            Check(Session.Open(directory, Language.English).View.KingsUndercoverPreference == null, "automatic setting is durable");
        }),
        ("Kings private cards disclose only their owners knowledge and cannot revisit completed handoffs", directory => {
            var session = Session.Open(directory, Language.English, _ => 0);
            foreach (var name in new[] { "Alex", "Alex", "König 👑", "Dana", "Eli" }) session.AddPlayer(name);
            session.SetMode(GameMode.Kings);
            Check(session.StartMatch().Success, "five-person Kings deal starts");
            var people = session.Match.Participants.ToArray();
            Check(people[0].DisplayName != people[1].DisplayName, "duplicate labels remain distinguishable");
            var cards = new List<PrivateCardView>();
            foreach (var owner in people)
            {
                Check(!session.Match.CanAdvance && session.Match.Result == null && session.Match.Elimination == null, "new handoff is covered and has no role/result disclosure");
                Check(session.RevealCard(people.First(p => p.Id != owner.Id).Id) == null, "another owner cannot read now");
                var card = session.RevealCard(owner.Id);
                Check(card != null && card.Owner.Id == owner.Id, "card is owner scoped");
                cards.Add(card);
                Check(!session.AdvanceHandoff(owner.Id).Success, "open card cannot advance");
                session = Session.Open(directory, Language.German, _ => 0);
                Check(session.Match.Owner.Id == owner.Id && !session.Match.CanAdvance, "interruption resumes same owner covered");
                var resumed = session.RevealCard(owner.Id);
                Check(Newtonsoft.Json.JsonConvert.SerializeObject(resumed) == Newtonsoft.Json.JsonConvert.SerializeObject(card), "unfinished card stays exactly the original deal");
                session.HideWord();
                Check(session.AdvanceHandoff(owner.Id).Success, "completed covered card advances");
                session = Session.Open(directory, Language.German, _ => 0);
                Check(session.RevealCard(owner.Id) == null && !session.AdvanceHandoff(owner.Id).Success, "old owner cannot revisit after restart");
            }
            Check(cards[0].Kind == PrivateCardKind.Word && cards[0].Leader.Id == people[1].Id && cards[0].KnownParticipants.Count == 0, "ordinary Undercover sees only own word and White leader");
            Check(cards[3].Kind == cards[0].Kind && cards[3].Leader.Id == people[2].Id && cards[3].KnownParticipants.Count == 0 && cards[3].Word != cards[0].Word, "ordinary Civilian has neutral identical kind but own word and good leader");
            Check(cards[1].Kind == PrivateCardKind.EvilKing && cards[1].Word == null && cards[1].Leader == null && cards[1].KnownParticipants.Select(p => p.Id).SequenceEqual(new[] { people[0].Id }), "White knows his identity and ordinary team without either word or enemy king");
            Check(cards[2].Kind == PrivateCardKind.GoodKing && cards[2].Word == cards[3].Word && cards[2].Leader == null && cards[2].KnownParticipants.Select(p => p.Id).SequenceEqual(new[] { people[0].Id, people[1].Id }), "good King knows good word and evil names in seating order");
            Check(Newtonsoft.Json.JsonConvert.SerializeObject(cards[2].KnownParticipants).IndexOf("Role", StringComparison.OrdinalIgnoreCase) < 0, "known names do not mark the evil King");
            Check(session.Match.Phase == MatchPhase.TablePlay && session.Match.Owner == null && session.Match.StartingPlayer != null && session.Match.Result == null, "handoffs lead directly to neutral table play with a public starter");
            Check(!session.BeginVote().Success && !session.RecordTie(false).Success, "Kings never falls through into Quick or Classic voting");
            Check(session.View.Players.All(p => p.Active), "dealing does not change group participation");
        }),
        ("Kings V5 storage is understood while V4 live deals remain frozen and usable", directory => {
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "session.json");
            File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures", "legacy-v4-names-match.json"), path);
            var original = File.ReadAllBytes(path);
            var session = Session.Open(directory, Language.English, _ => 0);
            var originalMatch = Newtonsoft.Json.JsonConvert.SerializeObject(session.Match);
            var originalGroup = session.View.Players.Select(p => (p.Id, p.Name, p.DisplayName, p.Active)).ToArray();
            Check(!session.View.StorageBlocked && session.Match.Mode == GameMode.Classic && session.View.KingsUndercoverPreference == null, "V4 live Classic opens with automatic Kings preference");
            Check(original.SequenceEqual(File.ReadAllBytes(path)), "opening never rewrites the old deal");
            var envelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
            envelope["Version"] = 5;
            File.WriteAllText(path, envelope.ToString());
            session = Session.Open(directory, Language.English, _ => 0);
            Check(!session.View.StorageBlocked && Newtonsoft.Json.JsonConvert.SerializeObject(session.Match) == originalMatch, "V5 can represent the exact existing deal");
            Check(session.AbandonMatch(session.Match.Id).Success && session.SetMode(GameMode.Kings).Success && session.AddPlayer("第五人").Success, "upgraded group can choose Kings without renaming old identities");
            Check(session.SetKingsUndercoverPreference(1).Success && session.StartMatch().Success, "new Kings deal persists in upgraded session");
            session = Session.Open(directory, Language.German);
            Check(!session.View.StorageBlocked && session.Match.Mode == GameMode.Kings && session.View.Players.Take(4).Select(p => (p.Id, p.Name, p.DisplayName, p.Active)).SequenceEqual(originalGroup), "both old Unicode names and new Kings identities survive V5 reopen");
            Check((int)Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path))["Version"] == 6, "new writes use V6 so older apps refuse them");
        }),
        ("Kings supports every legal size and count with neutral ordered knowledge and fresh random assignments", directory => {
            var goodKings = new HashSet<int>(); var evilKings = new HashSet<int>();
            for (int count = 5; count <= 20; count++)
            {
                int maximum = (count - 1) / 2 - 1;
                for (int evil = 1; evil <= maximum; evil++)
                {
                    string folder = Path.Combine(directory, count + "-" + evil);
                    var session = Session.Open(folder, count % 2 == 0 ? Language.English : Language.German, new Random(count * 100 + evil).Next);
                    for (int i = 0; i < count; i++) session.AddPlayer("Person " + i);
                    session.SetMode(GameMode.Kings);
                    Check(session.SetKingsUndercoverPreference(evil).Success && session.StartMatch().Success, "every legal selected count deals");
                    session = Session.Open(folder, Language.English);
                    Check(session.View.StorageNotice == null && session.Match?.Participants.Count == count, "every frozen composition reopens normally");
                    var people = session.Match.Participants.ToArray();
                    var cards = ReadCards(session);
                    var good = cards.Single(c => c.Kind == PrivateCardKind.GoodKing);
                    var white = cards.Single(c => c.Kind == PrivateCardKind.EvilKing);
                    goodKings.Add(Array.FindIndex(people, p => p.Id == good.Owner.Id)); evilKings.Add(Array.FindIndex(people, p => p.Id == white.Owner.Id));
                    Check(cards.Count(c => c.Kind == PrivateCardKind.Word && c.Leader.Id == white.Owner.Id) == evil, "configured ordinary Undercover count");
                    Check(cards.Count(c => c.Word == good.Word) == count - evil - 1 && count - evil - 1 > evil + 1, "good word belongs to a strict good majority including its King");
                    Check(good.KnownParticipants.Select(p => p.Id).SequenceEqual(people.Where(p => p.Id == white.Owner.Id || white.KnownParticipants.Any(k => k.Id == p.Id)).Select(p => p.Id)), "good knowledge order follows seats, not hidden roles");
                    Check(white.KnownParticipants.Count == evil && white.Word == null && cards.Where(c => c.Kind == PrivateCardKind.Word).All(c => c.KnownParticipants.Count == 0), "no private side lists on ordinary cards and no White word");
                    Check(Session.Open(folder, Language.English).Match?.Phase == MatchPhase.TablePlay, "completed handoffs also restore at every count");
                }
            }
            Check(goodKings.Count > 1 && evilKings.Count > 1, "Kings are not pinned to fixed seats");
            var repeated = Session.Open(Path.Combine(directory, "repeat"), Language.English, _ => 0);
            for (int i = 0; i < 5; i++) repeated.AddPlayer("Person " + i);
            repeated.SetMode(GameMode.Kings); repeated.StartMatch(); var first = ReadCards(repeated);
            repeated.AbandonMatch(repeated.Match.Id); repeated.SetLanguage(Language.German); repeated.StartMatch(); var second = ReadCards(repeated);
            Check(first.Select(c => (c.Kind, c.Owner.Id, c.Leader?.Id)).SequenceEqual(second.Select(c => (c.Kind, c.Owner.Id, c.Leader?.Id))), "repeated assignments remain possible without role rotation guarantees");
        }),
        ("Kings failed preference deal and handoff saves never commit progress or consume a read", directory => {
            var session = Session.Open(directory, Language.English, _ => 0);
            for (int i = 0; i < 7; i++) session.AddPlayer("Person " + i);
            session.SetMode(GameMode.Kings);
            var pending = Path.Combine(directory, "session.pending.json");
            Directory.CreateDirectory(pending);
            Check(session.SetKingsUndercoverPreference(2).Error == "SaveFailed" && session.View.KingsUndercoverPreference == null, "failed count save retains prior preference");
            Check(session.StartMatch().Error == "SaveFailed" && session.Match == null, "failed start reveals no deal");
            Directory.Delete(pending);
            Check(session.StartMatch().Success, "retry deals normally");
            var id = session.Match.Owner.Id; var card = session.RevealCard(id); session.HideWord();
            Directory.CreateDirectory(pending);
            Check(session.AdvanceHandoff(id).Error == "SaveFailed" && session.Match.Owner.Id == id, "failed advance keeps current owner");
            var reopened = Session.Open(directory, Language.German);
            Check(reopened.Match.Owner.Id == id && !reopened.Match.CanAdvance && reopened.RevealCard(id).Word == card.Word, "only the same unfinished covered card recovers");
            Directory.Delete(pending);
            Check(session.AdvanceHandoff(id).Success && session.RevealCard(id) == null, "successful retry closes the previous handoff");
        })
    };
    static List<PrivateCardView> ReadCards(Session session)
    {
        var cards = new List<PrivateCardView>();
        while (session.Match.Phase == MatchPhase.Handoff)
        {
            var owner = session.Match.Owner.Id; cards.Add(session.RevealCard(owner)); session.HideWord();
            Check(session.AdvanceHandoff(owner).Success, "read and covered card advances");
        }
        return cards;
    }
    static void Check(bool condition, string expected) { if (!condition) throw new Exception(expected); }
}
