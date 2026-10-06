using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using WordDeduction.UI;

namespace WordDeduction.Editor
{
    // Editor-only fault fixtures for the real application and its recovery controls.
    public static class RecoveryEvidence
    {
        static string directory;
        public static void Setup(Language language, string fault)
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play mode first.");
            directory = Path.Combine(Application.temporaryCachePath, "recovery-evidence-" + Guid.NewGuid().ToString("N"));
            var session = Session.Open(directory, language);
            foreach (var name in new[] { "Alex", "Bea", "Chris" }) session.AddPlayer(name);
            var primary = Path.Combine(directory, "session.json");
            var backup = Path.Combine(directory, "session.previous.json");
            if (fault == "backup") File.WriteAllText(primary, "{interrupted");
            else if (fault == "damage") { File.WriteAllText(primary, "{damaged"); File.WriteAllText(backup, "{damaged"); }
            else if (fault == "newer") File.WriteAllText(primary, "{\"Version\":999}");
            else throw new ArgumentException("Unknown recovery fixture.");
            UnityEngine.Object.FindFirstObjectByType<GroupScreen>().Initialize(Session.Open(directory, language));
        }
        public static void DenyWrite(bool denied)
        {
            var pending = Path.Combine(directory, "session.pending.json");
            if (denied) Directory.CreateDirectory(pending);
            else if (Directory.Exists(pending)) Directory.Delete(pending);
        }
    }
}
