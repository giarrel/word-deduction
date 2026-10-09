using WordDeduction;

internal static class KingsStarterCases
{
    public static readonly (string name, Action<string> run)[] All = {
        ("Kings starter reuses the frozen deal without drawing on handoff or reopen", directory => {
            bool dealt = false;
            var session = Session.Open(directory, Language.English, _ => dealt ? throw new Exception("handoff must not draw") : 0);
            AddGroup(session, 7);
            Check(session.SetMode(GameMode.Kings).Success && session.StartMatch().Success, "deal Kings");
            dealt = true;
            var people = session.Match.Participants;
            Check(session.Match.StartingPlayer == null, "private handoff has no public starter suggestion yet");
            FinishHandoff(session);
            Check(session.Match.StartingPlayer?.Id == people[0].Id && session.Match.Round == 1, "the frozen initial choice is publicly visible");
            session = Session.Open(directory, Language.German, _ => throw new Exception("reopen must not draw"));
            Check(session.Match.StartingPlayer?.Id == people[0].Id && session.Match.Round == 1, "reopen keeps the same starter and round");
        }),
        ("Kings starter changes atomically after ordinary elimination without selection rerolls", directory => {
            var session = Table(directory);
            var first = session.Match; string target = first.Participants[0].Id;
            Check(session.SelectElimination(first.Id, first.Round, 0, target).Success, "select current starter");
            Check(session.Match.StartingPlayer.Id == target && session.Match.Round == 1, "selection does not reroll");
            Check(session.CancelElimination(first.Id, first.Round, 0, target).Success, "cancel choice");
            Check(session.Match.StartingPlayer.Id == target && session.Match.Round == 1, "cancel does not reroll");
            Check(session.SelectElimination(first.Id, first.Round, 0, target).Success, "select again");
            using (var locked = new FileStream(Path.Combine(directory, "session.pending.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                Check(session.ConfirmElimination(first.Id, first.Round, 0, target).Error == "SaveFailed", "failed elimination reports failure");
                Check(session.Match.StartingPlayer.Id == target && session.Match.Round == 1 && session.Match.Survivors.Count == 7, "failed save preserves starter and participants");
            }
            Check(session.ConfirmElimination(first.Id, first.Round, 0, target).Success, "ordinary elimination succeeds");
            Check(session.Match.Phase == MatchPhase.KingsElimination && session.Match.Round == 2 && session.Match.StartingPlayer.Id == first.Participants[1].Id, "next living starter can be White, on the same neutral elimination surface");
            Check(session.Match.Elimination == null && session.Match.Result == null, "ordinary roles remain hidden");
            session = Session.Open(directory, Language.German, _ => throw new Exception("reopen must not draw"));
            Check(session.Match.Round == 2 && session.Match.StartingPlayer.Id == first.Participants[1].Id && session.Match.Survivors.Count == 6, "elimination and next starter reopen together");
        }),
        ("Kings starter optional next round includes every survivor and rejects stale actions", directory => {
            var session = Table(directory); var first = session.Match;
            for (int seat = 0; seat < 7; seat++)
            {
                int selectedSeat = seat;
                session = Session.Open(directory, Language.English, count => { Check(count == 7, "every living participant is in the draw"); return selectedSeat; });
                var before = session.Match;
                Check(session.NextKingsRound(before.Id, before.Round, 0).Success, "next clue round without elimination");
                Check(session.Match.StartingPlayer.Id == first.Participants[seat].Id && session.Match.Round == before.Round + 1 && session.Match.Survivors.Count == 7, "every seat including White can begin without removing anyone");
                Check(!session.NextKingsRound(before.Id, before.Round, 0).Success, "duplicate next rejected");
                Check(!session.SelectElimination(before.Id, before.Round, 0, first.Participants[0].Id).Success, "old selection callback rejected after no-elimination round");
            }
            var current = session.Match;
            Check(session.NextKingsRound(current.Id, current.Round, 0).Success && session.Match.StartingPlayer.Id == current.StartingPlayer.Id, "same-person repeats are permitted");
            current = session.Match; string target = first.Participants[0].Id;
            Check(session.SelectElimination(current.Id, current.Round, 0, target).Success, "select current round");
            Check(!session.NextKingsRound(current.Id, current.Round, 0).Success, "pending selection cannot skip round");
            Check(!session.ConfirmElimination(current.Id, current.Round - 1, 0, target).Success && !session.CancelElimination(current.Id, current.Round - 1, 0, target).Success, "stale confirmation and correction rejected");
            Check(session.CancelElimination(current.Id, current.Round, 0, target).Success, "correct pending choice");
            string beforeBytes = File.ReadAllText(Path.Combine(directory, "session.json"));
            using (var locked = new FileStream(Path.Combine(directory, "session.pending.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                Check(session.NextKingsRound(current.Id, current.Round, 0).Error == "SaveFailed", "failed next reports save failure");
                Check(session.Match.Round == current.Round && session.Match.StartingPlayer.Id == current.StartingPlayer.Id, "failed next preserves public suggestion");
                Check(File.ReadAllText(Path.Combine(directory, "session.json")) == beforeBytes && Session.Open(directory, Language.German).Match.Round == current.Round, "committed disk and reopened round unchanged");
            }
            Check(!session.NextKingsRound("stale", current.Round, 0).Success && !session.NextKingsRound(current.Id, current.Round, 1).Success, "identity and progress validated");
            session = Session.Open(directory, Language.English, _ => 0);
            Check(session.SelectElimination(current.Id, current.Round, 0, target).Success && session.ConfirmElimination(current.Id, current.Round, 0, target).Success, "direct elimination still works");
            current = session.Match;
            Check(session.NextKingsRound(current.Id, current.Round, 1).Success && session.Match.Phase == MatchPhase.TablePlay && session.Match.EliminatedParticipant == null && session.Match.Survivors.Count == 6, "optional next clears old notice, never removes another participant");
            Check(session.Match.StartingPlayer.Id == first.Participants[1].Id, "eliminated starter is excluded, White remains eligible");
        }),
        ("Kings starter authentic V5 dead starter migrates without a write and continues through V6", directory => {
            Directory.CreateDirectory(directory);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures/kings-v5-dead-starter.json"), Path.Combine(directory, "session.json"));
            File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures/kings-v5-dead-starter-previous.json"), Path.Combine(directory, "session.previous.json"));
            string primary = File.ReadAllText(Path.Combine(directory, "session.json")), previous = File.ReadAllText(Path.Combine(directory, "session.previous.json"));
            Session session = null;
            for (int i = 0; i < 2; i++)
            {
                session = Session.Open(directory, Language.German, _ => throw new Exception("legacy open must not draw"));
                Check(!session.View.StorageBlocked && session.View.StorageNotice == null, "authentic V5 primary is accepted without recovery");
                Check(session.Match.Id == "568697cced60495589703c9a0109aa82" && session.Match.Phase == MatchPhase.KingsElimination && session.Match.Round == 1, "legacy phase and identity are retained");
                Check(session.Match.StartingPlayer?.DisplayName == "Bea" && session.Match.EliminatedParticipant.DisplayName == "Ada" && session.Match.Survivors.Count == 6, "first living person deterministically replaces the dead starter");
                Check(session.View.Language == Language.English && session.View.Players.Select(p => p.DisplayName).SequenceEqual(new[] { "Ada", "Bea", "Chris", "Dario", "Emil", "Fiona", "Greta" }), "original group and language retained");
                Check(File.ReadAllText(Path.Combine(directory, "session.json")) == primary && File.ReadAllText(Path.Combine(directory, "session.previous.json")) == previous, "both generations byte-identical after open");
            }
            session = Session.Open(directory, Language.German, _ => 0);
            var match = session.Match;
            Check(session.NextKingsRound(match.Id, match.Round, 1).Success, "first real command safely advances migrated match");
            Check((int)Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Path.Combine(directory, "session.json")))["Version"] == 6, "first command writes V6");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.Round == 2 && session.Match.StartingPlayer.DisplayName == "Bea" && session.View.StorageNotice == null, "V6 round reopens normally");
            var dario = session.Match.Participants.Single(p => p.DisplayName == "Dario");
            Check(session.SelectElimination(match.Id, 2, 1, dario.Id).Success && session.ConfirmElimination(match.Id, 2, 1, dario.Id).Success, "further migrated selection and confirmation work");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.Round == 3 && session.Match.Survivors.Count == 5 && session.Match.StartingPlayer.DisplayName == "Bea", "second new transition remains durable");
            string goodKing = session.Match.Participants.Single(p => p.DisplayName == "Chris").Id;
            Check(session.SelectElimination(match.Id, 3, 2, goodKing).Success && session.ConfirmElimination(match.Id, 3, 2, goodKing).Success, "legacy match finishes by the unchanged rules");
            Check(session.Match.Result.Reason == Outcome.GoodKingEliminated && session.Match.Result.CivilianWord == "Peach" && session.Match.Result.UndercoverWord == "Apricot" && session.Match.Result.Roles.Single(p => p.Participant.DisplayName == "Bea").Role == Role.White, "frozen words and assignments remain intact through migration");
        }),
        ("Kings starter actions are unavailable during private handoff last chance and results", directory => {
            var session = Session.Open(directory, Language.English, _ => 0); AddGroup(session, 5);
            Check(session.SetMode(GameMode.Kings).Success && session.StartMatch().Success, "deal Kings");
            string id = session.Match.Id;
            Check(!session.NextKingsRound(id, 1, 0).Success && session.Match.StartingPlayer == null, "handoff has no next-round action or public suggestion");
            FinishHandoff(session);
            Check(session.SelectElimination(id, 1, 0, session.Match.Participants[1].Id).Success && session.ConfirmElimination(id, 1, 0, session.Match.Participants[1].Id).Success, "catch White");
            Check(!session.NextKingsRound(id, 1, 1).Success && session.Match.StartingPlayer == null && session.Match.Round == 1, "last chance neither rerolls nor exposes starter");
            Check(session.ChooseLastChance(id, LastChanceChoice.Word).Success && !session.NextKingsRound(id, 1, 1).Success, "word answer cannot skip last chance");
            Check(session.ConfirmLastChanceAnswer(id).Success && !session.NextKingsRound(id, 1, 1).Success, "word judgment cannot skip last chance");
            Check(session.ResolveLastChanceWord(id, false).Success && !session.NextKingsRound(id, 1, 1).Success && session.Match.StartingPlayer == null, "result cannot advance");
            Check(session.ReturnToGroup(id).Success && session.SetMode(GameMode.Quick).Success && session.StartMatch().Success, "Quick starts normally");
            FinishHandoff(session);
            Check(!session.NextKingsRound(session.Match.Id, 1, 0).Success, "base modes reject Kings next round");
        }),
        ("Kings starter invalid new suggestions and false legacy rounds recover safely", directory => {
            foreach (string fault in new[] { "zero-round", "dead-starter", "legacy-round" })
            {
                string path = Path.Combine(directory, fault);
                if (fault == "legacy-round")
                {
                    Directory.CreateDirectory(path);
                    File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures/kings-v5-dead-starter.json"), Path.Combine(path, "session.json"));
                    SavedSessionFixture.RewritePrimary(path, payload => payload["Match"]["Round"] = 2);
                    Check(Session.Open(path, Language.English).View.StorageNotice == "DamagedData", "old schema never had advancing Kings rounds");
                    continue;
                }
                var session = Table(path); var match = session.Match;
                Check(session.SelectElimination(match.Id, 1, 0, match.Participants[0].Id).Success && session.ConfirmElimination(match.Id, 1, 0, match.Participants[0].Id).Success, "commit nonterminal elimination");
                SavedSessionFixture.RewritePrimary(path, payload => payload["Match"][fault == "zero-round" ? "Round" : "StartingIndex"] = 0);
                session = Session.Open(path, Language.English, _ => throw new Exception("recovery must not draw"));
                Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Round == 1 && session.Match.Survivors.Count == 7 && session.Match.SelectedSuspect.Id == match.Participants[0].Id, "bad new state recovers valid pending choice without guessing");
            }
        })
    };
    static Session Table(string directory)
    {
        var session = Session.Open(directory, Language.English, _ => 0); AddGroup(session, 7);
        Check(session.SetMode(GameMode.Kings).Success && session.StartMatch().Success, "deal Kings"); FinishHandoff(session); return session;
    }
    static void AddGroup(Session session, int count) { for (int i = 0; i < count; i++) Check(session.AddPlayer("Person " + i).Success, "add participant"); }
    static void FinishHandoff(Session session)
    {
        while (session.Match.Phase == MatchPhase.Handoff) { var id = session.Match.Owner.Id; session.RevealCard(id); session.HideWord(); Check(session.AdvanceHandoff(id).Success, "complete handoff"); }
    }
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
}
