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
        public SessionState Read()
        {
            try { return ReadGenerations(); }
            catch (NewerVersionException) { Notice = "NewerVersion"; Blocked = true; return null; }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            { Notice = "ReadFailed"; Blocked = true; return null; }
        }
        private SessionState ReadGenerations()
        {
            if (!File.Exists(Primary) && !File.Exists(Backup)) return null;
            try { return ReadFile(Primary); }
            catch (Exception error) when (error is JsonException || error is InvalidDataException || error is FileNotFoundException)
            {
                try
                {
                    var result = ReadFile(Backup);
                    Notice = "RecoveredBackup";
                    preserveBackup = true;
                    return result;
                }
                catch (Exception backupError) when (backupError is JsonException || backupError is InvalidDataException || backupError is FileNotFoundException)
                { Notice = "DamagedData"; Blocked = true; return null; }
            }
        }
        private static SessionState ReadFile(string path)
        {
            var envelope = JsonConvert.DeserializeObject<Envelope>(File.ReadAllText(path));
            if (envelope != null && envelope.Version > 2) throw new NewerVersionException();
            if (envelope == null || (envelope.Version != 1 && envelope.Version != 2) || envelope.Payload == null || envelope.Checksum != Hash(envelope.Payload))
                throw new InvalidDataException("Invalid saved session.");
            var state = JsonConvert.DeserializeObject<SessionState>(envelope.Payload);
            if (envelope.Version == 1 && state?.Match != null) throw new InvalidDataException("Invalid legacy session.");
            if (!Session.ValidSnapshot(state)) throw new InvalidDataException("Invalid session snapshot.");
            return state;
        }
        public void Write(SessionState state)
        {
            Directory.CreateDirectory(directory);
            var payload = JsonConvert.SerializeObject(state);
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Envelope { Version = 2, Payload = payload, Checksum = Hash(payload) }));
            var temporary = Path.Combine(directory, "session.pending.json");
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
            foreach (var file in new[] { Primary, Backup })
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
