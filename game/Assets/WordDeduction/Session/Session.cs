using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace WordDeduction
{
    public enum Language { English, German }
    public enum GameMode { Quick, Classic }
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
        public bool CanUndo { get; internal set; }
        public int ActiveCount => Players.Count(p => p.Active);
        public int NeededPlayers => Math.Max(0, (Mode == GameMode.Quick ? 3 : 4) - ActiveCount);
        public bool ReadyToStart => NeededPlayers == 0 && ActiveCount <= 20;
        public string StorageNotice { get; internal set; }
        public bool StorageBlocked { get; internal set; }
    }
    public sealed class CommandResult
    {
        public bool Success { get; internal set; }
        public string Error { get; internal set; }
        public string Notice { get; internal set; }
    }
    public sealed class Session
    {
        private readonly string directory;
        private readonly SnapshotStore store;
        private SessionState state;
        private Session(string directory, SessionState state, SnapshotStore store) { this.directory = directory; this.state = state; this.store = store; }
        public SessionView View => new SessionView { Language = state.Language, Mode = state.Mode, CanUndo = state.Removed != null, StorageNotice = store.Notice, StorageBlocked = store.Blocked,
            Players = state.Players.Select(p => new PlayerView { Id = p.Id, Name = p.Name, DisplayName = p.Distinguished ? p.Name + " · " + p.Number : p.Name, Active = p.Active }).ToArray() };
        public static Session Open(string directory, Language initialLanguage)
        {
            var store = new SnapshotStore(directory);
            return new Session(directory, store.Read() ?? new SessionState { Language = initialLanguage }, store);
        }
        public CommandResult AddPlayer(string name)
        {
            name = NormalizeName(name);
            if (name == null) return new CommandResult { Error = "InvalidName" };
            if (state.Players.Count >= 40) return new CommandResult { Error = "GroupFull" };
            bool active = state.Players.Count(p => p.Active) < 20;
            var result = Change(next => next.Players.Add(new PlayerState { Id = Guid.NewGuid().ToString("N"), Name = name, Active = active, Number = next.NextNumber++ }));
            if (result.Success && !active) result.Notice = "AddedPaused";
            return result;
        }
        public CommandResult RenamePlayer(string id, string name)
        {
            if (!state.Players.Any(p => p.Id == id)) return new CommandResult { Error = "PlayerNotFound" };
            name = NormalizeName(name);
            if (name == null) return new CommandResult { Error = "InvalidName" };
            return Change(next => next.Players.First(p => p.Id == id).Name = name);
        }
        public CommandResult SetParticipation(string id, bool active)
        {
            if (!state.Players.Any(p => p.Id == id)) return new CommandResult { Error = "PlayerNotFound" };
            if (active && state.Players.Count(p => p.Active && p.Id != id) >= 20) return new CommandResult { Error = "ActiveFull" };
            return Change(next => next.Players.First(p => p.Id == id).Active = active);
        }
        public CommandResult RemovePlayer(string id)
        {
            if (!state.Players.Any(p => p.Id == id)) return new CommandResult { Error = "PlayerNotFound" };
            return Change(next => {
            next.RemovedIndex = next.Players.FindIndex(p => p.Id == id);
            next.Removed = next.Players[next.RemovedIndex];
            next.Players.RemoveAt(next.RemovedIndex);
            });
        }
        public CommandResult UndoRemove()
        {
            if (state.Removed == null) return new CommandResult { Error = "NothingToUndo" };
            if (state.Players.Count >= 40) return new CommandResult { Error = "GroupFull" };
            if (state.Removed.Active && state.Players.Count(p => p.Active) >= 20) return new CommandResult { Error = "ActiveFull" };
            return Change(next => { next.Players.Insert(Math.Min(next.RemovedIndex, next.Players.Count), next.Removed); next.Removed = null; });
        }
        public CommandResult SetLanguage(Language language) => Enum.IsDefined(typeof(Language), language) ? Change(next => next.Language = language) : new CommandResult { Error = "InvalidSetting" };
        public CommandResult SetMode(GameMode mode) => Enum.IsDefined(typeof(GameMode), mode) ? Change(next => next.Mode = mode) : new CommandResult { Error = "InvalidSetting" };
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
        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            try { name = name.Trim().Normalize(NormalizationForm.FormC); }
            catch (ArgumentException) { return null; }
            if (name.Any(char.IsControl)) return null;
            var elements = StringInfo.ParseCombiningCharacters(name);
            return elements.Length >= 1 && elements.Length <= 24 ? name : null;
        }
        internal static bool ValidSnapshot(SessionState value)
        {
            if (value == null || value.Players == null || value.Players.Count > 40 || value.Players.Count(p => p != null && p.Active) > 20 ||
                !Enum.IsDefined(typeof(Language),value.Language) || !Enum.IsDefined(typeof(GameMode),value.Mode) || value.NextNumber < 1) return false;
            var all = value.Removed == null ? value.Players.ToArray() : value.Players.Concat(new[] { value.Removed }).ToArray();
            if (all.Any(p => p == null || !Guid.TryParseExact(p.Id,"N",out _) || string.IsNullOrEmpty(p.Name) || NormalizeName(p.Name) != p.Name || p.Number < 1 || p.Number >= value.NextNumber)) return false;
            if (all.Select(p=>p.Id).Distinct().Count() != all.Length || all.Select(p=>p.Number).Distinct().Count() != all.Length) return false;
            if (value.Removed != null && (value.RemovedIndex < 0 || value.RemovedIndex > 39)) return false;
            if (value.Players.Select(p => p.Distinguished ? p.Name + " · " + p.Number : p.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != value.Players.Count) return false;
            return true;
        }
    }
    internal sealed class SessionState
    {
        public List<PlayerState> Players = new List<PlayerState>();
        public Language Language;
        public GameMode Mode;
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
