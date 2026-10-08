using System;
using System.Collections.Generic;
using System.Linq;

namespace WordDeduction
{
    public enum MatchPhase { Handoff, Clues, Vote, Result, Elimination, WhiteGuess, TablePlay, KingsElimination, KingsLastChance, KingsWordAnswer, KingsWordJudgment, KingsKingTarget }
    public enum LastChanceChoice { Word, King }
    public enum Role { Civilian, Undercover, White }
    public enum PrivateCardKind { Word, White, GoodKing, EvilKing }
    public enum Outcome { CaughtUndercover, AccusedCivilian, RepeatedTie, AllAdversariesEliminated, OneCivilianRemains, WhiteGuessed, GoodKingEliminated, OnlyKingsRemain, KingsWordCorrect, KingsWordIncorrect, KingsKingCorrect, KingsKingIncorrect }
    internal static class RoleCounts
    {
        internal static int Minimum(GameMode mode) => mode == GameMode.Quick ? 3 : mode == GameMode.Kings ? 5 : 4;
        internal static int AdversaryLimit(int participants) => Math.Max(0, (participants - 1) / 2);
        internal static int KingsUndercoverLimit(int participants) => Math.Max(0, AdversaryLimit(participants) - 1);
        internal static bool HasGoodMajority(int participants, int undercover, int white) => undercover >= 1 && white >= 0 && undercover + white <= AdversaryLimit(participants);
        internal static int Undercover(GameMode mode, int participants) => mode == GameMode.Quick || participants <= 7 ? 1 : participants <= 12 ? 2 : 3;
        internal static int WhiteLimit(GameMode mode, int participants) => mode != GameMode.Quick && participants >= 5 ? 1 : 0;
    }
    public sealed class ParticipantView
    {
        public string Id { get; internal set; }
        public string DisplayName { get; internal set; }
    }
    public sealed class PrivateCardView
    {
        public ParticipantView Owner { get; internal set; }
        public PrivateCardKind Kind { get; internal set; }
        public string Word { get; internal set; }
        public ParticipantView Leader { get; internal set; }
        public IReadOnlyList<ParticipantView> KnownParticipants { get; internal set; } = Array.Empty<ParticipantView>();
    }
    public sealed class RoleView
    {
        public ParticipantView Participant { get; internal set; }
        public Role Role { get; internal set; }
        public bool IsKing { get; internal set; }
    }
    public sealed class ResultView
    {
        public Outcome Reason { get; internal set; }
        public Role Winner { get; internal set; }
        public IReadOnlyList<Role> WinningRoles { get; internal set; }
        public string CivilianWord { get; internal set; }
        public string UndercoverWord { get; internal set; }
        public IReadOnlyList<RoleView> Roles { get; internal set; }
    }
    public sealed class JudgmentView
    {
        public string Word { get; internal set; }
    }
    public sealed class MatchView
    {
        public string Id { get; internal set; }
        public GameMode Mode { get; internal set; }
        public Language Language { get; internal set; }
        public MatchPhase Phase { get; internal set; }
        public IReadOnlyList<ParticipantView> Participants { get; internal set; }
        public IReadOnlyList<ParticipantView> Survivors { get; internal set; }
        public int Round { get; internal set; }
        public RoleView Elimination { get; internal set; }
        public ParticipantView EliminatedParticipant { get; internal set; }
        public ParticipantView Owner { get; internal set; }
        public ParticipantView StartingPlayer { get; internal set; }
        public int HandoffNumber { get; internal set; }
        public bool CanAdvance { get; internal set; }
        public bool Runoff { get; internal set; }
        public ParticipantView SelectedSuspect { get; internal set; }
        public ResultView Result { get; internal set; }
        public JudgmentView Judgment { get; internal set; }
        public ParticipantView SelectedKingTarget { get; internal set; }
    }
    public sealed partial class Session
    {
        bool revealed;
        string readOwner;
        public MatchView Match
        {
            get
            {
                var match = state.Match;
                if (match == null) return null;
                return new MatchView {
                    Id = match.Id, Mode = match.Mode, Language = match.Language, Phase = match.Phase,
                    Participants = match.Participants.Select(PublicParticipant).ToArray(),
                    Survivors = match.Participants.Where(p => !p.Eliminated).Select(PublicParticipant).ToArray(), Round = match.Round,
                    EliminatedParticipant = match.Phase == MatchPhase.KingsElimination || IsLastChance(match.Phase)
                        ? PublicParticipant(match.Participants.First(p => p.Id == match.Suspect)) : null,
                    Elimination = match.Phase != MatchPhase.Elimination && match.Phase != MatchPhase.WhiteGuess ? null : new RoleView {
                        Participant = PublicParticipant(match.Participants.First(p => p.Id == match.Suspect)), Role = match.Participants.First(p => p.Id == match.Suspect).Role },
                    Owner = match.Phase == MatchPhase.Handoff ? PublicParticipant(match.Participants[match.Handoff]) : null,
                    StartingPlayer = match.Mode == GameMode.Kings ? null : PublicParticipant(match.Participants[match.StartingIndex]),
                    HandoffNumber = match.Handoff + 1,
                    CanAdvance = !revealed && match.Phase == MatchPhase.Handoff && readOwner == match.Participants[match.Handoff].Id,
                    Runoff = match.Runoff,
                    SelectedSuspect = match.Suspect == null || (match.Mode == GameMode.Kings && match.Phase != MatchPhase.TablePlay) ? null : PublicParticipant(match.Participants.First(p => p.Id == match.Suspect)),
                    Judgment = match.Phase == MatchPhase.KingsWordJudgment ? new JudgmentView { Word = match.CivilianWord } : null,
                    SelectedKingTarget = match.Phase == MatchPhase.KingsKingTarget && match.LastChanceTarget != null ? PublicParticipant(match.Participants.First(p => p.Id == match.LastChanceTarget)) : null,
                    Result = match.Phase != MatchPhase.Result ? null : new ResultView {
                        Reason = match.Outcome.Value, Winner = Winners(match)[0], WinningRoles = Winners(match),
                        CivilianWord = match.CivilianWord, UndercoverWord = match.UndercoverWord,
                        Roles = match.Participants.Select(p => new RoleView { Participant = PublicParticipant(p), Role = p.Role,
                            IsKing = match.Mode == GameMode.Kings && (p.Id == match.GoodKingId || p.Role == Role.White) }).ToArray()
                    }
                };
            }
        }
        static ParticipantView PublicParticipant(ParticipantState participant) => new ParticipantView { Id = participant.Id, DisplayName = participant.DisplayName };
        static bool IsLastChance(MatchPhase phase) => phase == MatchPhase.KingsLastChance || phase == MatchPhase.KingsWordAnswer || phase == MatchPhase.KingsWordJudgment || phase == MatchPhase.KingsKingTarget;
        bool LastChanceAction(string expectedMatchId, MatchPhase phase) => state.Match?.Mode == GameMode.Kings && state.Match.Id == expectedMatchId && state.Match.Phase == phase;
        public CommandResult ChooseLastChance(string expectedMatchId, LastChanceChoice choice)
        {
            if (!LastChanceAction(expectedMatchId, MatchPhase.KingsLastChance) || !Enum.IsDefined(typeof(LastChanceChoice), choice)) return InvalidAction();
            return Change(next => next.Match.Phase = choice == LastChanceChoice.Word ? MatchPhase.KingsWordAnswer : MatchPhase.KingsKingTarget);
        }
        public CommandResult ConfirmLastChanceAnswer(string expectedMatchId) => LastChanceAction(expectedMatchId, MatchPhase.KingsWordAnswer)
            ? Change(next => next.Match.Phase = MatchPhase.KingsWordJudgment) : InvalidAction();
        public CommandResult ResolveLastChanceWord(string expectedMatchId, bool correct) => LastChanceAction(expectedMatchId, MatchPhase.KingsWordJudgment)
            ? Change(next => { next.Match.Outcome = correct ? Outcome.KingsWordCorrect : Outcome.KingsWordIncorrect; next.Match.Phase = MatchPhase.Result; }) : InvalidAction();
        public CommandResult SelectLastChanceKing(string expectedMatchId, string participantId)
        {
            if (!LastChanceAction(expectedMatchId, MatchPhase.KingsKingTarget) || state.Match.LastChanceTarget != null || !state.Match.Participants.Any(p => p.Id == participantId && !p.Eliminated)) return InvalidAction();
            return Change(next => next.Match.LastChanceTarget = participantId);
        }
        bool PendingKingTarget(string expectedMatchId, string expectedParticipantId) => LastChanceAction(expectedMatchId, MatchPhase.KingsKingTarget) && expectedParticipantId != null && state.Match.LastChanceTarget == expectedParticipantId;
        public CommandResult CancelLastChanceKing(string expectedMatchId, string expectedParticipantId) => PendingKingTarget(expectedMatchId, expectedParticipantId)
            ? Change(next => next.Match.LastChanceTarget = null) : InvalidAction();
        public CommandResult ConfirmLastChanceKing(string expectedMatchId, string expectedParticipantId) => PendingKingTarget(expectedMatchId, expectedParticipantId)
            ? Change(next => { next.Match.Outcome = expectedParticipantId == next.Match.GoodKingId ? Outcome.KingsKingCorrect : Outcome.KingsKingIncorrect; next.Match.Phase = MatchPhase.Result; }) : InvalidAction();
        public CommandResult StartMatch()
        {
            if (state.Match != null) return new CommandResult { Error = "MatchInProgress" };
            if (!View.ReadyToStart) return new CommandResult { Error = "NotEnoughPlayers" };
            return Deal();
        }
        bool KingsTableAction(string expectedMatchId, int expectedEliminationCount) =>
            state.Match?.Mode == GameMode.Kings && state.Match.Id == expectedMatchId &&
            (state.Match.Phase == MatchPhase.TablePlay || state.Match.Phase == MatchPhase.KingsElimination) &&
            state.Match.Participants.Count(p => p.Eliminated) == expectedEliminationCount;
        public CommandResult SelectElimination(string expectedMatchId, int expectedEliminationCount, string participantId)
        {
            if (!KingsTableAction(expectedMatchId, expectedEliminationCount) || !state.Match.Participants.Any(p => p.Id == participantId && !p.Eliminated)) return InvalidAction();
            return Change(next => { next.Match.Suspect = participantId; next.Match.Phase = MatchPhase.TablePlay; });
        }
        public CommandResult CancelElimination(string expectedMatchId, int expectedEliminationCount, string expectedParticipantId)
        {
            if (!KingsTableAction(expectedMatchId, expectedEliminationCount) || state.Match.Phase != MatchPhase.TablePlay || expectedParticipantId == null || state.Match.Suspect != expectedParticipantId) return InvalidAction();
            return Change(next => next.Match.Suspect = null);
        }
        public CommandResult ConfirmElimination(string expectedMatchId, int expectedEliminationCount, string expectedParticipantId)
        {
            if (!KingsTableAction(expectedMatchId, expectedEliminationCount) || state.Match.Phase != MatchPhase.TablePlay || expectedParticipantId == null || state.Match.Suspect != expectedParticipantId) return InvalidAction();
            return Change(next => {
                var eliminated = next.Match.Participants.First(p => p.Id == expectedParticipantId);
                eliminated.Eliminated = true;
                if (eliminated.Role == Role.White) { next.Match.Phase = MatchPhase.KingsLastChance; return; }
                next.Match.Phase = MatchPhase.KingsElimination;
                if (expectedParticipantId == next.Match.GoodKingId) { next.Match.Outcome = Outcome.GoodKingEliminated; next.Match.Phase = MatchPhase.Result; }
                else if (next.Match.Participants.Count(p => !p.Eliminated) == 2 && next.Match.Participants.Any(p => !p.Eliminated && p.Role == Role.White))
                { next.Match.Outcome = Outcome.OnlyKingsRemain; next.Match.Phase = MatchPhase.Result; }
            });
        }
        CommandResult Deal()
        {
            if (!View.ReadyToStart) return new CommandResult { Error = "NotEnoughPlayers" };
            var participants = View.Players.Where(p => p.Active).Select(p => new ParticipantState { Id = p.Id, DisplayName = p.DisplayName }).ToList();
            var available = participants.ToList();
            for (int i = 0; i < View.UndercoverCount + View.WhiteCount; i++)
            {
                int index = random(available.Count);
                available[index].Role = i < View.UndercoverCount ? Role.Undercover : Role.White;
                available.RemoveAt(index);
            }
            string goodKing = state.Mode == GameMode.Kings ? available[random(available.Count)].Id : null;
            var result = Change(next => {
                var pair = DrawPair(next.History);
                var words = next.Language == Language.German ? pair.German : pair.English;
                int side = random(2);
                next.Match = new MatchState {
                    Id = Guid.NewGuid().ToString("N"), Mode = next.Mode, Language = next.Language,
                    Participants = participants, PairId = pair.Id, CivilianWord = words[side], UndercoverWord = words[1 - side],
                    StartingIndex = random(participants.Count), GoodKingId = goodKing
                };
            });
            if (result.Success) { HideWord(); readOwner = null; }
            return result;
        }
        public string RevealWord(string ownerId)
        {
            var card = RevealCard(ownerId);
            return card == null ? null : card.Kind == PrivateCardKind.White || card.Kind == PrivateCardKind.EvilKing ? "Mr. White" : card.Word;
        }
        public PrivateCardView RevealCard(string ownerId)
        {
            var match = state.Match;
            if (store.Blocked || match == null || match.Phase != MatchPhase.Handoff || match.Participants[match.Handoff].Id != ownerId) return null;
            revealed = true; readOwner = ownerId;
            var participant = match.Participants[match.Handoff];
            var card = new PrivateCardView { Owner = PublicParticipant(participant),
                Kind = participant.Role == Role.White ? PrivateCardKind.White : PrivateCardKind.Word,
                Word = participant.Role == Role.White ? null : participant.Role == Role.Civilian ? match.CivilianWord : match.UndercoverWord };
            if (match.Mode != GameMode.Kings) return card;
            if (participant.Id == match.GoodKingId)
            {
                card.Kind = PrivateCardKind.GoodKing;
                card.KnownParticipants = match.Participants.Where(p => p.Role != Role.Civilian).Select(PublicParticipant).ToArray();
            }
            else if (participant.Role == Role.White)
            {
                card.Kind = PrivateCardKind.EvilKing;
                card.KnownParticipants = match.Participants.Where(p => p.Role == Role.Undercover).Select(PublicParticipant).ToArray();
            }
            else card.Leader = PublicParticipant(match.Participants.First(p => participant.Role == Role.Civilian ? p.Id == match.GoodKingId : p.Role == Role.White));
            return card;
        }
        public void HideWord() { revealed = false; }
        public CommandResult BeginVote() => state.Match?.Phase == MatchPhase.Clues
            ? Change(next => next.Match.Phase = MatchPhase.Vote) : InvalidAction();
        public CommandResult SelectSuspect(string participantId)
        {
            if (state.Match?.Phase != MatchPhase.Vote || !state.Match.Participants.Any(p => p.Id == participantId && !p.Eliminated)) return InvalidAction();
            return Change(next => next.Match.Suspect = participantId);
        }
        public CommandResult CancelSuspect() => state.Match?.Phase == MatchPhase.Vote
            ? Change(next => next.Match.Suspect = null) : InvalidAction();
        public CommandResult ConfirmSuspect(string expectedParticipantId)
        {
            if (state.Match?.Phase != MatchPhase.Vote || expectedParticipantId == null || state.Match.Suspect != expectedParticipantId) return InvalidAction();
            return Change(next => {
                if (next.Match.Mode == GameMode.Classic)
                {
                    var eliminated = next.Match.Participants.First(p => p.Id == expectedParticipantId);
                    eliminated.Eliminated = true;
                    if (eliminated.Role == Role.White) { next.Match.Phase = MatchPhase.WhiteGuess; return; }
                    next.Match.Phase = MatchPhase.Elimination;
                    EvaluateClassic(next.Match);
                    return;
                }
                next.Match.Outcome = next.Match.Participants.First(p => p.Id == expectedParticipantId).Role == Role.Undercover ? Outcome.CaughtUndercover : Outcome.AccusedCivilian;
                next.Match.Phase = MatchPhase.Result;
            });
        }
        public CommandResult Rematch(string expectedMatchId) => state.Match?.Phase == MatchPhase.Result && state.Match.Id == expectedMatchId ? Deal() : InvalidAction();
        public CommandResult RecordTie(bool expectedRunoff)
        {
            if (state.Match?.Phase != MatchPhase.Vote || state.Match.Runoff != expectedRunoff) return InvalidAction();
            return Change(next => {
                next.Match.Suspect = null;
                if (next.Match.Runoff && next.Match.Mode == GameMode.Classic) NextRound(next.Match);
                else if (next.Match.Runoff) { next.Match.Outcome = Outcome.RepeatedTie; next.Match.Phase = MatchPhase.Result; }
                else next.Match.Runoff = true;
            });
        }
        public CommandResult ReturnToGroup(string expectedMatchId) => state.Match?.Phase == MatchPhase.Result ? AbandonMatch(expectedMatchId) : InvalidAction();
        public CommandResult AbandonMatch(string expectedMatchId)
        {
            HideWord();
            if (state.Match == null || state.Match.Id != expectedMatchId) return InvalidAction();
            var result = Change(next => next.Match = null);
            if (result.Success) readOwner = null;
            return result;
        }
        bool LiveMatch => state.Match != null && state.Match.Phase != MatchPhase.Result;
        static Role[] Winners(MatchState match)
        {
            if (match.Mode == GameMode.Kings) return match.Outcome == Outcome.KingsWordIncorrect || match.Outcome == Outcome.KingsKingIncorrect ? new[] { Role.Civilian } : new[] { Role.Undercover, Role.White };
            if (match.Outcome == Outcome.OneCivilianRemains) return match.Participants.Where(p => !p.Eliminated && p.Role != Role.Civilian).Select(p => p.Role).Distinct().OrderBy(r => r).ToArray();
            return new[] { match.Outcome == Outcome.WhiteGuessed ? Role.White : match.Outcome == Outcome.CaughtUndercover || match.Outcome == Outcome.AllAdversariesEliminated ? Role.Civilian : Role.Undercover };
        }
        public CommandResult ResolveWhiteGuess(string expectedParticipantId, bool correct)
        {
            if (state.Match?.Phase != MatchPhase.WhiteGuess || expectedParticipantId == null || state.Match.Suspect != expectedParticipantId) return InvalidAction();
            return Change(next => {
                if (correct) { next.Match.Outcome = Outcome.WhiteGuessed; next.Match.Phase = MatchPhase.Result; }
                else { EvaluateClassic(next.Match); if (next.Match.Phase != MatchPhase.Result) NextRound(next.Match); }
            });
        }
        static void EvaluateClassic(MatchState match)
        {
            if (!match.Participants.Any(p => !p.Eliminated && p.Role != Role.Civilian)) match.Outcome = Outcome.AllAdversariesEliminated;
            else if (match.Participants.Count(p => !p.Eliminated && p.Role == Role.Civilian) == 1) match.Outcome = Outcome.OneCivilianRemains;
            if (match.Outcome.HasValue) match.Phase = MatchPhase.Result;
        }
        public CommandResult ContinueRound(int expectedRound) => state.Match?.Phase == MatchPhase.Elimination && state.Match.Round == expectedRound
            ? Change(next => NextRound(next.Match)) : InvalidAction();
        void NextRound(MatchState match)
        {
            var survivors = match.Participants.Where(p => !p.Eliminated).ToArray();
            match.StartingIndex = match.Participants.IndexOf(survivors[random(survivors.Length)]);
            match.Round++; match.Suspect = null; match.Runoff = false; match.Phase = MatchPhase.Clues;
        }
        static bool ValidMatch(MatchState match)
        {
            if (match == null) return true;
            if (!Guid.TryParseExact(match.Id,"N",out _) || !Enum.IsDefined(typeof(GameMode),match.Mode) || !Enum.IsDefined(typeof(Language),match.Language) ||
                !Enum.IsDefined(typeof(MatchPhase),match.Phase) || match.Participants == null || match.Participants.Count < RoleCounts.Minimum(match.Mode) || match.Participants.Count > 20 ||
                string.IsNullOrWhiteSpace(match.PairId) || string.IsNullOrWhiteSpace(match.CivilianWord) || string.IsNullOrWhiteSpace(match.UndercoverWord) ||
                match.CivilianWord == match.UndercoverWord || match.CivilianWord.Length > 100 || match.UndercoverWord.Length > 100 ||
                match.CivilianWord.Any(char.IsControl) || match.UndercoverWord.Any(char.IsControl)) return false;
            // Frozen labels include durable duplicate suffixes and names accepted by older
            // versions. Validate their structure by the same saved-name policy as the group.
            if (match.Participants.Any(p => p == null || !Guid.TryParseExact(p.Id,"N",out _) || string.IsNullOrEmpty(p.DisplayName) ||
                NormalizeName(p.DisplayName, existing: true) != p.DisplayName || !Enum.IsDefined(typeof(Role),p.Role))) return false;
            if (match.Participants.Select(p => p.Id).Distinct().Count() != match.Participants.Count ||
                match.Participants.Select(p => p.DisplayName).Distinct(StringComparer.OrdinalIgnoreCase).Count() != match.Participants.Count ||
                (match.Mode != GameMode.Kings && match.Participants.Count(p => p.Role == Role.Undercover) != RoleCounts.Undercover(match.Mode, match.Participants.Count)) ||
                match.Participants.Count(p => p.Role == Role.White) > RoleCounts.WhiteLimit(match.Mode, match.Participants.Count) ||
                match.StartingIndex < 0 || match.StartingIndex >= match.Participants.Count) return false;
            if (!RoleCounts.HasGoodMajority(match.Participants.Count, match.Participants.Count(p => p.Role == Role.Undercover), match.Participants.Count(p => p.Role == Role.White))) return false;
            if (match.Handoff < 0 || match.Handoff > match.Participants.Count ||
                (match.Phase == MatchPhase.Handoff ? match.Handoff == match.Participants.Count : match.Handoff != match.Participants.Count)) return false;
            if (match.Suspect != null && !match.Participants.Any(p => p.Id == match.Suspect)) return false;
            if (match.Mode == GameMode.Kings) return ValidKingsMatch(match);
            if (match.GoodKingId != null || match.LastChanceTarget != null || match.Phase > MatchPhase.WhiteGuess) return false;
            if (match.Round < 1 || match.Participants.Count(p => p.Eliminated) > match.Round) return false;
            if (match.Mode == GameMode.Classic)
            {
                int civilians = match.Participants.Count(p => !p.Eliminated && p.Role == Role.Civilian);
                int adversaries = match.Participants.Count(p => !p.Eliminated && p.Role != Role.Civilian);
                bool ongoing = civilians > 1 && adversaries > 0;
                var suspect = match.Participants.FirstOrDefault(p => p.Id == match.Suspect);
                if (match.Phase == MatchPhase.Elimination || match.Phase == MatchPhase.WhiteGuess)
                    return suspect != null && suspect.Eliminated && match.Outcome == null &&
                        (match.Phase == MatchPhase.WhiteGuess ? suspect.Role == Role.White && civilians > 1 : suspect.Role != Role.White && ongoing);
                if (match.Phase == MatchPhase.Result)
                {
                    if (suspect == null || !suspect.Eliminated) return false;
                    if (match.Outcome == Outcome.WhiteGuessed) return suspect.Role == Role.White && civilians > 1;
                    if (match.Outcome == Outcome.OneCivilianRemains) return civilians == 1 && adversaries > 0 && suspect.Role == Role.Civilian;
                    return match.Outcome == Outcome.AllAdversariesEliminated && adversaries == 0 && civilians > 1 && suspect.Role != Role.Civilian;
                }
                if (!ongoing || match.Participants[match.StartingIndex].Eliminated || (suspect != null && suspect.Eliminated)) return false;
                if (match.Phase == MatchPhase.Handoff && (match.Round != 1 || match.Participants.Any(p => p.Eliminated))) return false;
            }
            else if (match.Round != 1 || match.Participants.Any(p => p.Eliminated) || match.Phase > MatchPhase.Result) return false;
            if ((match.Phase == MatchPhase.Handoff || match.Phase == MatchPhase.Clues) && (match.Runoff || match.Suspect != null)) return false;
            if (match.Phase != MatchPhase.Result) return match.Outcome == null;
            if (!match.Outcome.HasValue || !Enum.IsDefined(typeof(Outcome),match.Outcome.Value)) return false;
            if (match.Outcome > Outcome.RepeatedTie) return false;
            if (match.Outcome == Outcome.RepeatedTie) return match.Runoff && match.Suspect == null;
            if (match.Suspect == null) return false;
            return (match.Participants.First(p => p.Id == match.Suspect).Role == Role.Undercover) == (match.Outcome == Outcome.CaughtUndercover);
        }
        static CommandResult InvalidAction() => new CommandResult { Error = "InvalidAction" };
        static bool ValidKingsMatch(MatchState match)
        {
            if (match.Participants.Count(p => p.Role == Role.White) != 1 ||
                !match.Participants.Any(p => p.Id == match.GoodKingId && p.Role == Role.Civilian) || match.Round != 1 || match.Runoff) return false;
            var suspect = match.Participants.FirstOrDefault(p => p.Id == match.Suspect);
            bool goodKingOut = match.Participants.First(p => p.Id == match.GoodKingId).Eliminated;
            bool evilKingOut = match.Participants.Single(p => p.Role == Role.White).Eliminated;
            int survivors = match.Participants.Count(p => !p.Eliminated);
            bool kingResult = match.Phase == MatchPhase.Result && (match.Outcome == Outcome.KingsKingCorrect || match.Outcome == Outcome.KingsKingIncorrect);
            if (match.LastChanceTarget != null && ((match.Phase != MatchPhase.KingsKingTarget && !kingResult) || !match.Participants.Any(p => p.Id == match.LastChanceTarget && !p.Eliminated))) return false;
            if (kingResult) return match.LastChanceTarget != null && suspect != null && suspect.Eliminated && suspect.Role == Role.White && !goodKingOut && survivors >= 2 &&
                (match.LastChanceTarget == match.GoodKingId) == (match.Outcome == Outcome.KingsKingCorrect);
            if (match.Phase == MatchPhase.Result && (match.Outcome == Outcome.KingsWordCorrect || match.Outcome == Outcome.KingsWordIncorrect))
                return suspect != null && suspect.Eliminated && suspect.Role == Role.White && !goodKingOut && survivors >= 2;
            if (match.Phase == MatchPhase.Result) return suspect != null && suspect.Eliminated && !evilKingOut &&
                (match.Outcome == Outcome.GoodKingEliminated ? suspect.Id == match.GoodKingId && survivors >= 2 :
                 match.Outcome == Outcome.OnlyKingsRemain && !goodKingOut && survivors == 2 && suspect.Id != match.GoodKingId && suspect.Role != Role.White);
            if (match.Outcome != null || match.Participants.First(p => p.Id == match.GoodKingId).Eliminated) return false;
            if (match.Phase == MatchPhase.Handoff) return suspect == null && !match.Participants.Any(p => p.Eliminated);
            if (IsLastChance(match.Phase)) return suspect != null && suspect.Eliminated && suspect.Role == Role.White && survivors >= 2;
            if (evilKingOut || survivors <= 2) return false;
            if (match.Phase == MatchPhase.TablePlay) return suspect == null || !suspect.Eliminated;
            return match.Phase == MatchPhase.KingsElimination && suspect != null && suspect.Eliminated && suspect.Role != Role.White && suspect.Id != match.GoodKingId;
        }
        public CommandResult AdvanceHandoff(string ownerId)
        {
            if (Match == null || !Match.CanAdvance || Match.Owner.Id != ownerId) return new CommandResult { Error = "ReadCardFirst" };
            var result = Change(next => {
                next.Match.Handoff++;
                if (next.Match.Handoff == next.Match.Participants.Count) next.Match.Phase = next.Match.Mode == GameMode.Kings ? MatchPhase.TablePlay : MatchPhase.Clues;
            });
            if (result.Success) readOwner = null;
            return result;
        }
    }
    internal sealed class MatchState
    {
        public string Id;
        public GameMode Mode;
        public Language Language;
        public List<ParticipantState> Participants;
        public string PairId;
        public string CivilianWord;
        public string UndercoverWord;
        public int StartingIndex;
        public MatchPhase Phase;
        public int Handoff;
        public bool Runoff;
        public string Suspect;
        public Outcome? Outcome;
        public int Round = 1;
        public string GoodKingId;
        public string LastChanceTarget;
    }
    internal sealed class ParticipantState
    {
        public string Id;
        public string DisplayName;
        public Role Role;
        public bool Eliminated;
    }
}
