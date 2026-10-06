using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Editor
{
    // Searches via ordinary Session actions; never edits a saved deal or catalog.
    public static class ContentEvidence
    {
        public static void Setup(Language language,string expectedWord)
        {
            if(!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play mode first.");
            var session=Session.Open(Path.Combine(Application.temporaryCachePath,"content-evidence-"+Guid.NewGuid().ToString("N")),language,maximum=>maximum-1);
            foreach(var name in new[]{"Alexandra Maximiliane","Bea","Chris"}) session.AddPlayer(name);
            for(int draw=0;draw<520;draw++)
            {
                session.StartMatch();
                while(session.Match.Phase==MatchPhase.Handoff)
                {
                    string owner=session.Match.Owner.Id;
                    bool found=session.RevealWord(owner)==expectedWord; session.HideWord();
                    if(found) { UnityEngine.Object.FindFirstObjectByType<GroupScreen>().Initialize(session); return; }
                    session.AdvanceHandoff(owner);
                }
                session.AbandonMatch(session.Match.Id);
            }
            throw new InvalidOperationException("Word not found in one complete catalog cycle.");
        }
        public static void Click(string name)
        {
            var root=UnityEngine.Object.FindFirstObjectByType<GroupScreen>().GetComponent<UIDocument>().rootVisualElement;
            var target=root.Q<Button>(name);
            if(target==null || !target.enabledInHierarchy) throw new InvalidOperationException("Control unavailable: "+name);
            using(var e=NavigationSubmitEvent.GetPooled()) { e.target=target; target.SendEvent(e); }
        }
    }
}
