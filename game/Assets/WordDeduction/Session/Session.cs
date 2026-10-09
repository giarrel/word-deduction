using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace WordDeduction
{
    public enum Language { English, German }
    public enum GameMode { Quick, Classic, Kings }
    public sealed class PlayerView
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public string DisplayName { get; internal set; }
        public bool Active { get; internal set; }
    }
    public sealed class SessionView
    {
        public IReadOnlyList<PlayerView> Players { get; internal set; } = Array.Empty<PlayerView>();
        public Language Language { get; internal set; }
        public GameMode Mode { get; internal set; }
        public bool WhitePreferred { get; internal set; }
        public int? KingsUndercoverPreference { get; internal set; }
        public int KingsUndercoverLimit => RoleCounts.KingsUndercoverLimit(ActiveCount);
        public int UndercoverCount => Mode == GameMode.Kings ? Math.Max(1, Math.Min(KingsUndercoverPreference ?? RoleCounts.Undercover(Mode, ActiveCount), KingsUndercoverLimit)) : RoleCounts.Undercover(Mode, ActiveCount);
        public bool KingsCountAdjusted => Mode == GameMode.Kings && KingsUndercoverPreference.HasValue && KingsUndercoverPreference.Value != UndercoverCount;
        public int WhiteCount => Mode == GameMode.Kings || WhitePreferred ? RoleCounts.WhiteLimit(Mode, ActiveCount) : 0;
        public int CivilianCount => Math.Max(0,ActiveCount - UndercoverCount - WhiteCount);
        public bool CanUndo { get; internal set; }
        public int ActiveCount => Players.Count(p => p.Active);
        public int NeededPlayers => Math.Max(0, RoleCounts.Minimum(Mode) - ActiveCount);
        public bool ReadyToStart => NeededPlayers == 0 && ActiveCount <= 20 && RoleCounts.HasGoodMajority(ActiveCount, UndercoverCount, WhiteCount);
        public string StorageNotice { get; internal set; }
        public bool StorageBlocked { get; internal set; }
    }
    public sealed class CommandResult
    {
        public bool Success { get; internal set; }
        public string Error { get; internal set; }
        public string Notice { get; internal set; }
    }
    public sealed partial class Session
    {
        private readonly string directory;
        private readonly SnapshotStore store;
        private SessionState state;
        private readonly Func<int, int> random;
        private Session(string directory, SessionState state, SnapshotStore store, Func<int, int> random) { this.directory = directory; this.state = state; this.store = store; this.random = random; }
        public SessionView View => new SessionView { Language = state.Language, Mode = state.Mode, WhitePreferred = state.WhitePreferred, KingsUndercoverPreference = state.KingsUndercoverPreference, CanUndo = state.Removed != null, StorageNotice = store.Notice, StorageBlocked = store.Blocked,
            Players = state.Players.Select(p => new PlayerView { Id = p.Id, Name = p.Name, DisplayName = p.Distinguished ? p.Name + " · " + p.Number : p.Name, Active = p.Active }).ToArray() };
        public static Session Open(string directory, Language initialLanguage, Func<int, int> random = null)
        {
            var store = new SnapshotStore(directory);
            return new Session(directory, store.Read() ?? new SessionState { Language = initialLanguage }, store, random ?? new Random().Next);
        }
        public CommandResult AddPlayer(string name)
        {
            if (LiveMatch) return new CommandResult { Error = "MatchInProgress" };
            name = NormalizeName(name);
            if (name == null) return new CommandResult { Error = "InvalidName" };
            if (state.Players.Count >= 40) return new CommandResult { Error = "GroupFull" };
            if (state.Players.Count(p => p.Active) >= 20) return new CommandResult { Error = "ActiveFull" };
            return Change(next => next.Players.Add(new PlayerState { Id = Guid.NewGuid().ToString("N"), Name = name, Active = true, Number = next.NextNumber++ }));
        }
        public CommandResult RenamePlayer(string id, string name)
        {
            if (LiveMatch) return new CommandResult { Error = "MatchInProgress" };
            if (!state.Players.Any(p => p.Id == id)) return new CommandResult { Error = "PlayerNotFound" };
            name = NormalizeName(name);
            if (name == null) return new CommandResult { Error = "InvalidName" };
            return Change(next => next.Players.First(p => p.Id == id).Name = name);
        }
        public CommandResult SetParticipation(string id, bool active)
        {
            if (LiveMatch) return new CommandResult { Error = "MatchInProgress" };
            if (!state.Players.Any(p => p.Id == id)) return new CommandResult { Error = "PlayerNotFound" };
            if (active && state.Players.Count(p => p.Active && p.Id != id) >= 20) return new CommandResult { Error = "ActiveFull" };
            return Change(next => next.Players.First(p => p.Id == id).Active = active);
        }
        public CommandResult ReorderPlayers(IReadOnlyList<string> expectedOrder, IReadOnlyList<string> orderedIds)
        {
            if (LiveMatch) return new CommandResult { Error = "MatchInProgress" };
            var current = state.Players.Select(p => p.Id).ToArray();
            if (expectedOrder == null || !current.SequenceEqual(expectedOrder)) return new CommandResult { Error = "StaleOrder" };
            if (orderedIds == null || orderedIds.Count != current.Length || orderedIds.Distinct().Count() != current.Length || orderedIds.Any(id => !current.Contains(id)))
                return new CommandResult { Error = "InvalidOrder" };
            if (current.SequenceEqual(orderedIds)) return new CommandResult { Success = true };
            return Change(next => next.Players = orderedIds.Select(id => next.Players.Single(p => p.Id == id)).ToList());
        }
        public CommandResult RemovePlayer(string id)
        {
            if (LiveMatch) return new CommandResult { Error = "MatchInProgress" };
            if (!state.Players.Any(p => p.Id == id)) return new CommandResult { Error = "PlayerNotFound" };
            return Change(next => {
            next.RemovedIndex = next.Players.FindIndex(p => p.Id == id);
            next.Removed = next.Players[next.RemovedIndex];
            next.Players.RemoveAt(next.RemovedIndex);
            });
        }
        public CommandResult UndoRemove()
        {
            if (LiveMatch) return new CommandResult { Error = "MatchInProgress" };
            if (state.Removed == null) return new CommandResult { Error = "NothingToUndo" };
            if (state.Players.Count >= 40) return new CommandResult { Error = "GroupFull" };
            if (state.Removed.Active && state.Players.Count(p => p.Active) >= 20) return new CommandResult { Error = "ActiveFull" };
            return Change(next => { next.Players.Insert(Math.Min(next.RemovedIndex, next.Players.Count), next.Removed); next.Removed = null; });
        }
        public CommandResult SetLanguage(Language language) => LiveMatch ? new CommandResult { Error = "MatchInProgress" } : Enum.IsDefined(typeof(Language), language) ? Change(next => next.Language = language) : new CommandResult { Error = "InvalidSetting" };
        public CommandResult SetMode(GameMode mode) => LiveMatch ? new CommandResult { Error = "MatchInProgress" } : Enum.IsDefined(typeof(GameMode), mode) ? Change(next => next.Mode = mode) : new CommandResult { Error = "InvalidSetting" };
        public CommandResult SetWhitePreference(bool enabled) => LiveMatch ? new CommandResult { Error = "MatchInProgress" } : Change(next => next.WhitePreferred = enabled);
        public CommandResult SetKingsUndercoverPreference(int? count)
        {
            if (LiveMatch) return new CommandResult { Error = "MatchInProgress" };
            if (count.HasValue && (count < 1 || count > View.KingsUndercoverLimit)) return new CommandResult { Error = "InvalidSetting" };
            return Change(next => next.KingsUndercoverPreference = count);
        }
        public CommandResult StartFreshAfterDamage()
        {
            if (store.Notice != "DamagedData") return new CommandResult { Error = "StorageBlocked" };
            var next = new SessionState { Language = state.Language };
            try { store.StartFresh(next); }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException) { return new CommandResult { Error = "SaveFailed" }; }
            state = next;
            return new CommandResult { Success = true };
        }
        private CommandResult Change(Action<SessionState> change)
        {
            if (store.Blocked) return new CommandResult { Error = "StorageBlocked" };
            var next = JsonConvert.DeserializeObject<SessionState>(JsonConvert.SerializeObject(state));
            change(next);
            foreach (var group in next.Players.GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
                foreach (var player in group) player.Distinguished = true;
            while (true)
            {
                var collisions = next.Players.GroupBy(p => p.Distinguished ? p.Name + " · " + p.Number : p.Name, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).ToArray();
                if (collisions.Length == 0) break;
                foreach (var group in collisions) foreach (var player in group) player.Distinguished = true;
            }
            try { store.Write(next); }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            { return new CommandResult { Error = "SaveFailed" }; }
            state = next;
            return new CommandResult { Success = true };
        }
        private static string NormalizeName(string name, bool existing = false)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            try { NameText.EnsureWellFormed(name); name = name.Trim().Normalize(NormalizationForm.FormC); }
            catch (ArgumentException) { return null; }
            if (name.Any(char.IsControl)) return null;
            // Saved names were accepted under earlier Unicode/visibility policies. Preserve them
            // structurally; new adds and renames alone use the current visible-name and length rules.
            if (existing) return name;
            if (name.Contains("\u2028") || name.Contains("\u2029") || !NameText.HasVisibleBase(name)) return null;
            int elements = NameText.ElementCount(name);
            return elements >= 1 && elements <= 24 ? name : null;
        }
        internal static bool ValidSnapshot(SessionState value)
        {
            if (value == null || value.Players == null || value.Players.Count > 40 || value.Players.Count(p => p != null && p.Active) > 20 ||
                !Enum.IsDefined(typeof(Language),value.Language) || !Enum.IsDefined(typeof(GameMode),value.Mode) || value.NextNumber < 1) return false;
            if (value.KingsUndercoverPreference.HasValue && (value.KingsUndercoverPreference < 1 || value.KingsUndercoverPreference > RoleCounts.KingsUndercoverLimit(20))) return false;
            var all = value.Removed == null ? value.Players.ToArray() : value.Players.Concat(new[] { value.Removed }).ToArray();
            if (all.Any(p => p == null || !Guid.TryParseExact(p.Id,"N",out _) || string.IsNullOrEmpty(p.Name) || NormalizeName(p.Name, existing: true) != p.Name || p.Number < 1 || p.Number >= value.NextNumber)) return false;
            if (all.Select(p=>p.Id).Distinct().Count() != all.Length || all.Select(p=>p.Number).Distinct().Count() != all.Length) return false;
            if (value.Removed != null && (value.RemovedIndex < 0 || value.RemovedIndex > 39)) return false;
            if (value.Players.Select(p => p.Distinguished ? p.Name + " · " + p.Number : p.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != value.Players.Count) return false;
            return ValidMatch(value.Match) && ValidWordHistory(value.History) && (value.Match == null ||
                (value.History.UsedPairIds.Contains(value.Match.PairId) && value.History.RecentPairIds.LastOrDefault() == value.Match.PairId));
        }
    }
    internal sealed class SessionState
    {
        public WordHistoryState History = new WordHistoryState();
        public MatchState Match;
        [JsonProperty(Required = Required.Always)]
        public List<PlayerState> Players = new List<PlayerState>();
        public Language Language;
        public GameMode Mode;
        public bool WhitePreferred;
        public int? KingsUndercoverPreference;
        public PlayerState Removed;
        public int RemovedIndex;
        public int NextNumber = 1;
    }
    internal sealed class PlayerState
    {
        public string Id;
        public string Name;
        public bool Active;
        public int Number;
        public bool Distinguished;
    }
}
