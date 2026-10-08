using WordDeduction;

static class KingsLastChanceCases
{
    public static readonly (string name, Action<string> run)[] All = {
        ("Kings last chance word choice and spoken answer lock before target disclosure", directory => {
            var session = LastChance(directory); var id = session.Match.Id;
            Check(session.Match.Judgment?.Word == null, "choice exposes no word");
            Check(!session.ChooseLastChance("stale", LastChanceChoice.Word).Success, "old match cannot choose");
            Check(session.ChooseLastChance(id, LastChanceChoice.Word).Success, "word choice commits");
            Check(session.Match.Phase == MatchPhase.KingsWordAnswer && session.Match.Judgment?.Word == null, "committed choice still hides target");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.Phase == MatchPhase.KingsWordAnswer, "choice survives restart");
            Check(!session.ChooseLastChance(id, LastChanceChoice.King).Success && !session.ChooseLastChance(id, LastChanceChoice.Word).Success, "choice cannot switch or repeat");
            Check(!session.ResolveLastChanceWord(id, true).Success, "cannot judge before spoken answer");
            Check(session.ConfirmLastChanceAnswer(id).Success, "spoken answer acknowledgment commits");
            string word = session.Match.Judgment?.Word;
            Check(!string.IsNullOrEmpty(word) && session.Match.Phase == MatchPhase.KingsWordJudgment && session.Match.Result == null, "only target word now disclosed");
            Check(session.Match.Elimination == null && session.Match.SelectedKingTarget == null, "no King identity or assignments");
            session = Session.Open(directory, Language.German, _ => 0);
            Check(session.Match.Phase == MatchPhase.KingsWordJudgment && session.Match.Judgment?.Word == word, "pending judgment survives restart");
            Check(!session.ConfirmLastChanceAnswer(id).Success && !session.ChooseLastChance(id, LastChanceChoice.King).Success, "answer cannot reopen or switch");
        }),
        ("Kings last chance word judgment wins for the whole team and is final", directory => {
            foreach (bool correct in new[] { true, false })
            {
                string path = Path.Combine(directory, correct.ToString());
                var session = LastChance(path); string id = session.Match.Id;
                Check(session.ChooseLastChance(id, LastChanceChoice.Word).Success && session.ConfirmLastChanceAnswer(id).Success, "commit spoken answer");
                string target = session.Match.Judgment?.Word;
                Check(session.ResolveLastChanceWord(id, correct).Success, "group judgment commits");
                var result = session.Match.Result;
                Check(result != null && result.CivilianWord == target && !string.IsNullOrEmpty(result.UndercoverWord), "only terminal state discloses both words");
                Check(result.WinningRoles.SequenceEqual(correct ? new[] { Role.Undercover, Role.White } : new[] { Role.Civilian }), "success wins entire evil team; failure entire good team");
                Check(result.Roles.Count == 5 && result.Roles.Count(p => p.IsKing) == 2, "full assignments and both Kings disclosed");
                session = Session.Open(path, Language.German, _ => 0);
                Check(session.Match.Result.WinningRoles.SequenceEqual(result.WinningRoles), "terminal judgment survives restart");
                Check(!session.ResolveLastChanceWord(id, !correct).Success && !session.ConfirmLastChanceAnswer(id).Success && !session.ChooseLastChance(id, LastChanceChoice.King).Success, "no retry or changed outcome");
                Check(session.Rematch(id).Success && session.Match.Survivors.Count == 5, "all active people return for rematch");
                Check(!session.ResolveLastChanceWord(id, true).Success && !session.ChooseLastChance(id, LastChanceChoice.Word).Success, "old match actions cannot affect new deal");
            }
        }),
        ("Kings last chance King target is correctable until single stable identity commitment", directory => {
            foreach (bool correct in new[] { true, false })
            {
                string path = Path.Combine(directory, correct.ToString());
                var session = LastChance(path); var match = session.Match; string id = match.Id;
                string white = match.Participants[1].Id, king = match.Participants[2].Id, ordinary = match.Participants[3].Id;
                Check(!session.SelectLastChanceKing(id, king).Success, "no target before branch");
                Check(session.ChooseLastChance(id, LastChanceChoice.King).Success, "King branch commits");
                session = Session.Open(path, Language.German, _ => 0);
                Check(session.Match.Phase == MatchPhase.KingsKingTarget && session.Match.Judgment?.Word == null, "King branch persists with no word");
                Check(!session.ChooseLastChance(id, LastChanceChoice.Word).Success && !session.ConfirmLastChanceAnswer(id).Success, "cannot switch to word");
                Check(!session.SelectLastChanceKing(id, white).Success && !session.SelectLastChanceKing(id, "unknown").Success, "only living targets allowed");
                Check(session.SelectLastChanceKing(id, ordinary).Success, "pending target selected");
                session = Session.Open(path, Language.German, _ => 0);
                Check(session.Match.SelectedKingTarget.Id == ordinary && session.Match.Result == null, "pending target persists without comparison disclosure");
                Check(!session.ConfirmLastChanceKing(id, king).Success && !session.CancelLastChanceKing("stale", ordinary).Success, "stale target and match rejected");
                Check(session.CancelLastChanceKing(id, ordinary).Success, "pending target corrected");
                string target = correct ? king : ordinary;
                Check(session.SelectLastChanceKing(id, target).Success && session.ConfirmLastChanceKing(id, target).Success, "one target committed by ID");
                Check(session.Match.Result.WinningRoles.SequenceEqual(correct ? new[] { Role.Undercover, Role.White } : new[] { Role.Civilian }), "King outcome awards full team");
                session = Session.Open(path, Language.German, _ => 0);
                Check(session.Match.Phase == MatchPhase.Result && session.Match.Result.Roles.Count(p => p.IsKing) == 2, "final King outcome persists");
                Check(!session.ConfirmLastChanceKing(id, target).Success && !session.SelectLastChanceKing(id, king).Success && !session.CancelLastChanceKing(id, target).Success, "no duplicate or retry");
            }
        }),
        ("Kings last chance failed writes disclose no new word or outcome and retain every commitment", directory => {
            foreach (var choice in new[] { LastChanceChoice.Word, LastChanceChoice.King })
            {
                string path = Path.Combine(directory, choice.ToString()); var session = LastChance(path); string id = session.Match.Id;
                FailsSafely(session, path, () => session.ChooseLastChance(id, choice));
                Check(session.ChooseLastChance(id, choice).Success, "branch retry commits");
                if (choice == LastChanceChoice.Word)
                {
                    FailsSafely(session, path, () => session.ConfirmLastChanceAnswer(id));
                    Check(session.Match.Judgment?.Word == null, "failed answer save cannot expose target");
                    Check(session.ConfirmLastChanceAnswer(id).Success, "answer retry commits");
                    FailsSafely(session, path, () => session.ResolveLastChanceWord(id, false));
                    Check(session.ResolveLastChanceWord(id, false).Success, "judgment retry commits");
                }
                else
                {
                    string king = session.Match.Participants[2].Id;
                    FailsSafely(session, path, () => session.SelectLastChanceKing(id, king));
                    Check(session.SelectLastChanceKing(id, king).Success, "selection retry commits");
                    FailsSafely(session, path, () => session.CancelLastChanceKing(id, king));
                    FailsSafely(session, path, () => session.ConfirmLastChanceKing(id, king));
                    Check(session.ConfirmLastChanceKing(id, king).Success, "target retry commits");
                }
            }
        }),
        ("Kings last chance rejects impossible saved commitments even with valid checksums", directory => {
            var session = LastChance(directory); string id = session.Match.Id;
            Check(session.ChooseLastChance(id, LastChanceChoice.Word).Success, "word branch");
            SavedSessionFixture.RewritePrimary(directory, payload => payload["Match"]["LastChanceTarget"] = session.Match.Participants[2].Id);
            session = Session.Open(directory, Language.English);
            Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Phase == MatchPhase.KingsLastChance, "target in word branch rejected");
            Check(session.ChooseLastChance(id, LastChanceChoice.King).Success, "King branch");
            string target = session.Match.Participants[3].Id;
            Check(session.SelectLastChanceKing(id, target).Success && session.ConfirmLastChanceKing(id, target).Success, "wrong target committed");
            SavedSessionFixture.RewritePrimary(directory, payload => payload["Match"]["Outcome"] = (int)Outcome.KingsKingCorrect);
            session = Session.Open(directory, Language.English);
            Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Phase == MatchPhase.KingsKingTarget && session.Match.SelectedKingTarget.Id == target, "fabricated correct outcome rejected");
            Check(session.AbandonMatch(id).Success && session.SetMode(GameMode.Classic).Success && session.StartMatch().Success, "Classic regression setup");
            while (session.Match.Phase == MatchPhase.Handoff) { var owner = session.Match.Owner.Id; session.RevealCard(owner); session.HideWord(); Check(session.AdvanceHandoff(owner).Success, "Classic handoff"); }
            SavedSessionFixture.RewritePrimary(directory, payload => payload["Match"]["Phase"] = (int)MatchPhase.KingsWordJudgment);
            session = Session.Open(directory, Language.English);
            Check(session.View.StorageNotice == "RecoveredBackup" && session.Match.Judgment?.Word == null, "new Kings phase cannot expose an old-mode target");
        })
    };
    static Session LastChance(string directory)
    {
        var session = Session.Open(directory, Language.English, _ => 0);
        foreach (var name in new[] { "Alex", "Bea", "Chris", "Dana", "Eli" }) Check(session.AddPlayer(name).Success, "add participant");
        Check(session.SetMode(GameMode.Kings).Success && session.StartMatch().Success, "start Kings");
        while (session.Match.Phase == MatchPhase.Handoff) { string id = session.Match.Owner.Id; session.RevealCard(id); session.HideWord(); Check(session.AdvanceHandoff(id).Success, "handoff"); }
        var match = session.Match; string white = match.Participants[1].Id;
        Check(session.SelectElimination(match.Id, 0, white).Success && session.ConfirmElimination(match.Id, 0, white).Success, "eliminate White");
        return session;
    }
    static void Check(bool actual, string expected) { if (!actual) throw new Exception(expected); }
    static void FailsSafely(Session session, string directory, Func<CommandResult> command)
    {
        string before = Newtonsoft.Json.JsonConvert.SerializeObject(session.Match);
        using (var locked = new FileStream(Path.Combine(directory, "session.pending.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            Check(command().Error == "SaveFailed", "failed required commit reports SaveFailed");
            Check(Newtonsoft.Json.JsonConvert.SerializeObject(session.Match) == before, "failed write leaves live disclosure unchanged");
            Check(Newtonsoft.Json.JsonConvert.SerializeObject(Session.Open(directory, Language.English).Match) == before, "failed write leaves reopened disclosure unchanged");
        }
    }
}
