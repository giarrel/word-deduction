using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Editor
{
    // Temporary disk fixtures; every phase transition below is a rendered input.
    public static class PolishEvidence
    {
        static Session session;
        static VisualElement Root => UnityEngine.Object.FindFirstObjectByType<GroupScreen>().GetComponent<UIDocument>().rootVisualElement;
        public static void Setup(Language language,int count=40,bool classic=true)
        {
            session=Session.Open(Path.Combine(Application.temporaryCachePath,"polish-evidence-"+Guid.NewGuid().ToString("N")),language,maximum=>0);
            var names=new[]{"Alexandra Maximiliane","😀","👩🏽‍🚀","李雷","Alex محمد","नमस्ते","Zoë","Борис"};
            for(int i=0;i<count;i++) session.AddPlayer(i<names.Length ? names[i] : "Person "+(i+1));
            session.SetMode(classic ? GameMode.Classic : GameMode.Quick); session.SetWhitePreference(true);
            UnityEngine.Object.FindFirstObjectByType<GroupScreen>().Initialize(session);
        }
        public static string SetupCollisions(Language language)
        {
            var directory = Path.Combine(Application.temporaryCachePath,"polish-collisions-"+Guid.NewGuid().ToString("N"));
            session = Session.Open(directory,language,maximum=>0);
            foreach (var name in new[]{"Alex","Alex","Alex · 1","Alex · 2","Alex · 1 · 1","Alex · 2 · 1","Alexandra Maximiliane","Alexandra Maximiliane"})
            {
                var result = session.AddPlayer(name);
                if (!result.Success) throw new InvalidOperationException(result.Error);
            }
            session.SetMode(GameMode.Classic); session.SetWhitePreference(true);
            UnityEngine.Object.FindFirstObjectByType<GroupScreen>().Initialize(session);
            return directory;
        }
        public static string[] PublicNames() => System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(session.View.Players,p=>p.DisplayName));
        public static void Click(string name)
        {
            var target=Root.Q<Button>(name);
            if(target==null || !target.enabledInHierarchy) throw new InvalidOperationException("Control unavailable: "+name);
            using(var e=NavigationSubmitEvent.GetPooled()) { e.target=target; target.SendEvent(e); }
        }
        public static void Suspect(int index) => Click("suspect-"+session.Match.Participants[index].Id);
        public static void Edit(int index) => Click("edit-"+session.View.Players[index].Id);
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
        public static void Scroll(string name,bool end)
        {
            var scroll=Root.Q<ScrollView>(name); scroll.scrollOffset=new Vector2(0,end ? scroll.verticalScroller.highValue : 0);
        }
        public static object Bounds() => Root.Query<Button>().ToList().FindAll(b=>b.resolvedStyle.display!=DisplayStyle.None && b.worldBound.height>0).ConvertAll(b=>new { b.name,bounds=b.worldBound.ToString(),b.enabledInHierarchy });
        public static string SafeStatus() => session.Match==null ? "Group: "+session.View.ActiveCount+" active / "+session.View.Players.Count+" saved" : session.Match.Phase+", round "+session.Match.Round+", survivors "+session.Match.Survivors.Count;
    }
}