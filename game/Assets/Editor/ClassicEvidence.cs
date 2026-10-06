using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Editor
{
    // Uses a new temporary session, the real App scene, and rendered input events.
    public static class ClassicEvidence
    {
        static Session session;
        static VisualElement Root => UnityEngine.Object.FindFirstObjectByType<GroupScreen>().GetComponent<UIDocument>().rootVisualElement;
        public static void Setup(Language language,int count=5)
        {
            session=Session.Open(Path.Combine(Application.temporaryCachePath,"classic-evidence-"+Guid.NewGuid().ToString("N")),language, maximum=>0);
            var names=new[]{"Alex","Bea","Chris","Alexandra Maximiliane","Zoë"};
            for(int i=0;i<count;i++) session.AddPlayer(i<names.Length ? names[i] : "Person "+(i+1));
            session.SetMode(GameMode.Classic); session.SetWhitePreference(true);
            UnityEngine.Object.FindFirstObjectByType<GroupScreen>().Initialize(session);
        }
        public static void Click(string name)
        {
            var target=Root.Q<Button>(name);
            if(target==null || !target.enabledInHierarchy) throw new InvalidOperationException("Control unavailable: "+name);
            using(var e=NavigationSubmitEvent.GetPooled()) { e.target=target; target.SendEvent(e); }
        }
        public static void Suspect(int index) => Click("suspect-"+session.Match.Participants[index].Id);
        public static void Reveal(bool show)
        {
            var target=Root.Q<VisualElement>("holdReveal");
            var touch=new Touch { fingerId=0,position=target.worldBound.center,phase=show ? TouchPhase.Began : TouchPhase.Ended };
            if(show) { using(var e=PointerDownEvent.GetPooled(touch)) { e.target=target; target.SendEvent(e); } }
            else { using(var e=PointerUpEvent.GetPooled(touch)) { e.target=target; target.SendEvent(e); } }
        }
        public static void ReadRemainingCards()
        {
            if(session.Match.Phase!=MatchPhase.Handoff) return;
            Reveal(true); Reveal(false);
            Root.schedule.Execute(()=>{ Click("nextOwner"); ReadRemainingCards(); }).StartingIn(250);
        }
        public static void Reopen()
        {
            // Disable/re-enable exercises the rendered covered resume surface.
            var host=UnityEngine.Object.FindFirstObjectByType<GroupScreen>().gameObject; host.SetActive(false); host.SetActive(true);
        }
        public static string SafeStatus() => session.Match==null ? "Group" : session.Match.Phase+", round "+session.Match.Round+", survivors "+session.Match.Survivors.Count;
    }
}
