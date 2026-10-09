using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace WordDeduction
{
    internal sealed class SnapshotStore
    {
        private readonly string directory;
        private bool preserveBackup;
        public string Notice { get; private set; }
        public bool Blocked { get; private set; }
        public SnapshotStore(string directory) { this.directory = directory; }
        private string Primary => Path.Combine(directory, "session.json");
        private string Backup => Path.Combine(directory, "session.previous.json");
        private string Pending => Path.Combine(directory, "session.pending.json");
        public SessionState Read()
        {
            try { return ReadGenerations(); }
            catch (NewerVersionException) { Notice = "NewerVersion"; Blocked = true; return null; }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            { Notice = "ReadFailed"; Blocked = true; return null; }
        }
        private SessionState ReadGenerations()
        {
            // A later app may have left its generation in either committed slot.
            // Never replace a version we cannot interpret on the next save.
            try { ReadEnvelope(Backup); }
            catch (Exception error) when (error is JsonException || Missing(error)) { }
            try { return ReadFile(Primary); }
            catch (Exception error) when (error is JsonException || error is InvalidDataException || Missing(error))
            {
                try
                {
                    var result = ReadFile(Backup);
                    Notice = "RecoveredBackup";
                    preserveBackup = true;
                    return result;
                }
                catch (Exception backupError) when (backupError is JsonException || backupError is InvalidDataException || Missing(backupError))
                {
                    if (Missing(error) && Missing(backupError) && !HasPending()) return null;
                    Notice = "DamagedData"; Blocked = true; return null;
                }
            }
        }
        private static bool Missing(Exception error) => error is FileNotFoundException || error is DirectoryNotFoundException;
        private bool HasPending()
        {
            try
            {
                if ((File.GetAttributes(Pending) & FileAttributes.Directory) != 0) throw new IOException("Invalid storage path.");
                return true;
            }
            catch (Exception error) when (Missing(error)) { return false; }
        }
        private static SessionState ReadFile(string path)
        {
            var envelope = ReadEnvelope(path);
            if (envelope == null || envelope.Version < 1 || envelope.Version > 6 || envelope.Payload == null || envelope.Checksum != Hash(envelope.Payload))
                throw new InvalidDataException("Invalid saved session.");
            var state = JsonConvert.DeserializeObject<SessionState>(envelope.Payload);
            var payload = Newtonsoft.Json.Linq.JObject.Parse(envelope.Payload);
            if (envelope.Version < 6 && (payload.Property("QuickRoles") != null || payload.Property("ClassicRoles") != null || (payload["Match"] as Newtonsoft.Json.Linq.JObject)?.Property("RulesVersion") != null)) throw new InvalidDataException("Invalid legacy session.");
            if (envelope.Version == 1 && state?.Match != null) throw new InvalidDataException("Invalid legacy session.");
            if (envelope.Version < 5 && state != null && (state.Mode == GameMode.Kings || state.KingsUndercoverPreference.HasValue || state.Match?.Mode == GameMode.Kings || state.Match?.GoodKingId != null || state.Match?.LastChanceTarget != null)) throw new InvalidDataException("Invalid legacy session.");
            if (envelope.Version < 4 && state != null) Session.MigrateWordHistory(state);
            if (envelope.Version >= 4 && Newtonsoft.Json.Linq.JObject.Parse(envelope.Payload)["History"] == null) throw new InvalidDataException("Missing word history.");
            if (!Session.ValidSnapshot(state)) throw new InvalidDataException("Invalid session snapshot.");
            return state;
        }
        private static Envelope ReadEnvelope(string path)
        {
            var envelope = JsonConvert.DeserializeObject<Envelope>(File.ReadAllText(path));
            if (envelope != null && envelope.Version > 6) throw new NewerVersionException();
            return envelope;
        }
        public void Write(SessionState state)
        {
            Directory.CreateDirectory(directory);
            var payload = JsonConvert.SerializeObject(state);
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Envelope { Version = 6, Payload = payload, Checksum = Hash(payload) }));
            var temporary = Pending;
            using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
            }
            if (File.Exists(Primary)) File.Replace(temporary, Primary, preserveBackup ? null : Backup);
            else File.Move(temporary, Primary);
            preserveBackup = false;
        }
        public void StartFresh(SessionState state)
        {
            var archive = Path.Combine(directory, "damaged-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(archive);
            foreach (var file in new[] { Primary, Backup, Pending })
                if (File.Exists(file)) File.Copy(file, Path.Combine(archive, Path.GetFileName(file)));
            preserveBackup = true;
            Write(state);
            Notice = null;
            Blocked = false;
        }
        private static string Hash(string payload)
        {
            using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(payload)));
        }
        private sealed class Envelope
        {
            public int Version;
            public string Payload;
            public string Checksum;
        }
        private sealed class NewerVersionException : Exception { }
    }
}
