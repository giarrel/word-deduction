using WordDeduction;

internal static class KingsEliminationCases
{
    public static readonly (string name, Action<string> run)[] All = {
        ("Kings elimination selection is correctable durable and ordinary disclosure stays neutral", directory => {
            var session = Table(directory);
            var match = session.Match; var people = match.Participants;
            Check(session.SelectElimination(match.Id, 0, people[0].Id).Success, "select an ordinary participant from table play");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.SelectedSuspect.Id == people[0].Id && session.Match.Survivors.Count == 5 && session.Match.Result == null, "pending choice reopens without disclosure");
            Check(session.CancelElimination(match.Id, 0, people[0].Id).Success, "selection can be corrected");
            Check(session.SelectElimination(match.Id, 0, people[3].Id).Success, "choose another person");
            Check(!session.ConfirmElimination(match.Id, 0, people[0].Id).Success, "stale target cannot confirm a corrected choice");
            Check(session.ConfirmElimination(match.Id, 0, people[3].Id).Success, "confirm ordinary elimination");
            Check(session.Match.Phase == MatchPhase.KingsElimination && session.Match.EliminatedParticipant.Id == people[3].Id, "only not-a-king elimination surface is exposed");
            Check(session.Match.Elimination == null && session.Match.Result == null && session.Match.Owner == null && session.Match.StartingPlayer == null, "no side, words, private owner or starter disclosed");
            Check(session.Match.Survivors.Count == 4 && session.View.Players.All(p => p.Active), "elimination changes match survivors only");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.Phase == MatchPhase.KingsElimination && session.Match.Survivors.Count == 4, "confirmed elimination survives reopening");
            Check(!session.ConfirmElimination(match.Id, 0, people[3].Id).Success, "duplicate confirmation rejected");
            Check(!session.SelectElimination(match.Id, 1, people[3].Id).Success, "an eliminated participant cannot be selected again");
            Check(!session.SelectElimination(match.Id, 0, people[4].Id).Success, "stale table cannot select after progression");
            Check(!session.SelectElimination("stale-match", 1, people[4].Id).Success, "stale match cannot select");
            Check(!session.BeginVote().Success && !session.RecordTie(false).Success, "table play has no required vote or runoff states");
            Check(session.SelectElimination(match.Id, 1, people[0].Id).Success, "the next actual elimination needs no intermediate acknowledgment");
        }),
        ("Kings elimination of good King wins for whole evil team and rematch restores the group", directory => {
            var session = Table(directory); var id = session.Match.Id; var people = session.Match.Participants;
            Eliminate(session, people[0].Id);
            Eliminate(session, people[2].Id);
            Check(session.Match.Phase == MatchPhase.Result && session.Match.Result.Reason == Outcome.GoodKingEliminated, "good King elimination immediately ends the match");
            var result = session.Match.Result;
            Check(result.WinningRoles.SequenceEqual(new[] { Role.Undercover, Role.White }), "entire evil team wins, including the eliminated Undercover");
            Check(result.Roles.Count == 5 && result.Roles[0].Role == Role.Undercover && result.Roles[1].IsKing && result.Roles[2].IsKing && !result.Roles[0].IsKing, "final assignments identify both Kings and retain eliminated teammates");
            Check(!string.IsNullOrWhiteSpace(result.CivilianWord) && !string.IsNullOrWhiteSpace(result.UndercoverWord) && result.CivilianWord != result.UndercoverWord, "both words become available at result");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.Result.Reason == Outcome.GoodKingEliminated, "final team result reopens");
            Check(session.Rematch(id).Success && session.Match.Id != id && session.Match.Survivors.Count == 5, "one action rematch restores all active people");
            Check(session.Match.Mode == GameMode.Kings && session.Match.Language == Language.English, "rematch keeps mode and language");
            Check(!session.ConfirmElimination(id, 1, people[2].Id).Success, "old confirmation cannot affect the rematch");
            Check(session.AbandonMatch(session.Match.Id).Success && session.View.Players.All(p => p.Active), "group remains editable after leaving the new match");
        }),
        ("Kings elimination continues through parity and lone good King until exactly two Kings remain", directory => {
            var session = Table(directory); var people = session.Match.Participants;
            Eliminate(session, people[3].Id);
            Check(session.Match.Phase == MatchPhase.KingsElimination && session.Match.Result == null, "two good versus two evil is still live");
            Eliminate(session, people[4].Id);
            Check(session.Match.Phase == MatchPhase.KingsElimination && session.Match.Survivors.Count == 3 && session.Match.Result == null, "lone good King with two evil survivors is still live");
            session = Session.Open(directory, Language.English, _ => 0);
            Check(session.Match.Survivors.Count == 3 && session.Match.Result == null, "lone good King live state is durable");
            Eliminate(session, people[0].Id);
            Check(session.Match.Phase == MatchPhase.Result && session.Match.Result.Reason == Outcome.OnlyKingsRemain, "only the two Kings triggers evil victory");
            Check(session.Match.Result.WinningRoles.SequenceEqual(new[] { Role.Undercover, Role.White }), "even eliminated ordinary evil teammates win");
            session = Session.Open(directory, Language.English, _ => 0);
            Check(session.Match.Result.Reason == Outcome.OnlyKingsRemain, "two-King result is durable");
            Check(session.ReturnToGroup(session.Match.Id).Success && session.View.Mode == GameMode.Kings && session.View.Players.Count(p => p.Active) == 5, "result returns to intact editable group");
            Check(session.RenamePlayer(people[0].Id, "Renamed").Success && session.StartMatch().Success, "group can be edited and dealt again");
        }),
        ("Kings elimination of White durably enters last chance before any general reveal", directory => {
            var session = Table(directory); var match = session.Match; var white = match.Participants[1];
            Eliminate(session, white.Id);
            Check(session.Match.Phase == MatchPhase.KingsLastChance && session.Match.EliminatedParticipant.Id == white.Id, "White elimination enters last chance");
            Check(session.Match.Result == null && session.Match.Elimination == null && session.Match.Owner == null && session.Match.SelectedSuspect == null, "entry reveals neither side assignments nor either word nor good King");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.Phase == MatchPhase.KingsLastChance && session.Match.EliminatedParticipant.Id == white.Id && session.Match.Survivors.Count == 4, "last chance and White elimination survive restart");
            Check(!session.ConfirmElimination(match.Id, 0, white.Id).Success && !session.SelectElimination(match.Id, 1, match.Participants[2].Id).Success, "no duplicate elimination or other table action during last chance");
            Check(!session.ResolveWhiteGuess(white.Id, true).Success && !session.ResolveWhiteGuess(white.Id, false).Success, "Classic White judgment cannot bypass Kings choice");
            Check(!session.Rematch(match.Id).Success && !session.ReturnToGroup(match.Id).Success, "last chance cannot accidentally reveal or rematch");
        }),
        ("Kings elimination failed writes keep the committed selection and disclose no new consequence", directory => {
            foreach (int seat in new[] { 0, 1, 2 })
            {
                string path = Path.Combine(directory, "target-" + seat);
                var session = Table(path); var match = session.Match; string target = match.Participants[seat].Id;
                using (var locked = new FileStream(Path.Combine(path, "session.pending.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    Check(session.SelectElimination(match.Id, 0, target).Error == "SaveFailed", "failed selection is reported");
                    Check(session.Match.SelectedSuspect == null && Session.Open(path, Language.English).Match.SelectedSuspect == null, "failed selection changes neither live nor reopened view");
                }
                Check(session.SelectElimination(match.Id, 0, target).Success, "retry selection");
                string before = Newtonsoft.Json.JsonConvert.SerializeObject(session.Match);
                using (var locked = new FileStream(Path.Combine(path, "session.pending.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    Check(session.CancelElimination(match.Id, 0, target).Error == "SaveFailed", "failed cancel is reported");
                    Check(session.ConfirmElimination(match.Id, 0, target).Error == "SaveFailed", "failed confirmation is reported");
                    Check(Newtonsoft.Json.JsonConvert.SerializeObject(session.Match) == before, "failed confirmation exposes exactly the prior safe view");
                    Check(Newtonsoft.Json.JsonConvert.SerializeObject(Session.Open(path, Language.English).Match) == before, "reopen preserves committed pending selection with no consequence");
                }
                Check(session.ConfirmElimination(match.Id, 0, target).Success, "confirmation can be retried after storage recovers");
                Check(session.Match.Phase == (seat == 0 ? MatchPhase.KingsElimination : seat == 1 ? MatchPhase.KingsLastChance : MatchPhase.Result), "only the successful retry exposes the consequence");
            }
        })
    };
    static Session Table(string directory, int count = 5)
    {
        var session = Session.Open(directory, Language.English, _ => 0);
        for (int i = 0; i < count; i++) Check(session.AddPlayer("Person " + i).Success, "add participant");
        Check(session.SetMode(GameMode.Kings).Success && session.StartMatch().Success, "deal Kings");
        while (session.Match.Phase == MatchPhase.Handoff) { var id = session.Match.Owner.Id; session.RevealCard(id); session.HideWord(); Check(session.AdvanceHandoff(id).Success, "complete handoff"); }
        return session;
    }
    static void Eliminate(Session session, string participantId)
    {
        var match = session.Match; int count = match.Participants.Count - match.Survivors.Count;
        Check(session.SelectElimination(match.Id, count, participantId).Success, "select elimination");
        Check(session.ConfirmElimination(match.Id, count, participantId).Success, "confirm elimination");
    }
    static void Check(bool actual, string expected) { if (!actual) throw new Exception(expected); }
}
