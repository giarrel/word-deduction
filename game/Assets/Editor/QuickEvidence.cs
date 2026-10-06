using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Editor
{
    // Reproducible visual fixture: the real app panel and public rendered inputs.
    public static class QuickEvidence
    {
        static Session session;
        static VisualElement Root => UnityEngine.Object.FindFirstObjectByType<GroupScreen>().GetComponent<UIDocument>().rootVisualElement;
        public static void Setup(Language language, int count = 3)
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play mode first.");
            session = Session.Open(Path.Combine(Application.temporaryCachePath,"quick-evidence-" + Guid.NewGuid().ToString("N")),language, maximum => 0);
            string[] names = { "Alex", "Bea", "Chris", "Alexandra Maximiliane", "Mia", "Noah", "Rene", "Zoë" };
            for (int i=0;i<count;i++) session.AddPlayer(i < names.Length ? names[i] : "Person " + (i+1));
            UnityEngine.Object.FindFirstObjectByType<GroupScreen>().Initialize(session);
        }
        public static void Click(string name)
        {
            var target=Root.Q<Button>(name);
            if (target == null || !target.enabledInHierarchy) throw new InvalidOperationException("Control unavailable: " + name);
            using (var e=NavigationSubmitEvent.GetPooled()) { e.target=target; target.SendEvent(e); }
        }
        public static void Suspect(int index) => Click("suspect-"+session.Match.Participants[index].Id);
        public static void Touch(string targetName, string phase, float y)
        {
            var target=Root.Q<VisualElement>(targetName);
            var touch=new Touch { fingerId=0,position=new Vector2(target.worldBound.center.x,y) };
            if (phase=="down") { touch.phase=TouchPhase.Began; using(var e=PointerDownEvent.GetPooled(touch)) { e.target=target; target.SendEvent(e); } }
            else if (phase=="move") { touch.phase=TouchPhase.Moved; using(var e=PointerMoveEvent.GetPooled(touch)) { e.target=target; target.SendEvent(e); } }
            else { touch.phase=TouchPhase.Ended; using(var e=PointerUpEvent.GetPooled(touch)) { e.target=target; target.SendEvent(e); } }
        }
        public static string SafeStatus() => session.Match == null ? "Group" : session.Match.Phase + ": " + session.Match.HandoffNumber;
    }
}
