using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    internal sealed class MatchSurface : IDisposable
    {
        readonly Session session;
        readonly VisualElement screen, body, actions;
        readonly Action renderApp;
        readonly VisualElement inputRoot;
        readonly HashSet<int> contacts = new HashSet<int>();
        SecretCard card;
        bool paused, help, abandon;
        string notice;
        Language language;
        public MatchSurface(Session session, VisualElement parent, Action renderApp)
        {
            this.session = session; this.renderApp = renderApp;
            inputRoot = parent;
            inputRoot.RegisterCallback<PointerDownEvent>(ContactDown, TrickleDown.TrickleDown);
            inputRoot.RegisterCallback<PointerUpEvent>(ContactUp, TrickleDown.TrickleDown);
            inputRoot.RegisterCallback<PointerCancelEvent>(ContactCancel, TrickleDown.TrickleDown);
            Resources.Load<VisualTreeAsset>("Match").CloneTree(parent);
            screen = parent.Q<VisualElement>("matchScreen"); body = screen.Q<VisualElement>("matchBody"); actions = screen.Q<VisualElement>("matchActions");
            paused = session.Match != null && session.Match.Phase != MatchPhase.Result;
            screen.Q<Button>("matchBack").clicked += Back;
            screen.Q<Button>("matchHelp").clicked += () => { card?.Hide(true); help = true; Render(); };
        }
        string T(string key, params object[] args) => Copy.Get(language,key,args);
        public void Render()
        {
            card?.Dispose(); card = null; body.Clear(); actions.Clear();
            var match = session.Match;
            screen.EnableInClassList("hidden",match == null);
            MobilePrivacy.Protect(match != null && match.Phase != MatchPhase.Result);
            if (match == null) { paused = help = abandon = false; return; }
            language = match.Language;
            screen.Q<Button>("matchBack").text = T("back");
            screen.Q<Button>("matchHelp").text = T("help");
            screen.Q<Label>("matchMode").text = T("quick");
            var noticeLabel = screen.Q<Label>("matchNotice");
            var message = notice ?? session.View.StorageNotice;
            noticeLabel.text = message == null ? "" : T(message); noticeLabel.EnableInClassList("hidden",message == null);
            if (help) { Help(); return; }
            if (abandon) { Abandon(match); return; }
            if (paused) { Paused(); return; }
            switch (match.Phase)
            {
                case MatchPhase.Handoff: Handoff(match); break;
                case MatchPhase.Clues: Clues(match); break;
                case MatchPhase.Vote: Vote(match); break;
                case MatchPhase.Result: Result(match); break;
            }
        }
        void Act(CommandResult result)
        {
            card?.Hide(true);
            notice = result.Success ? result.Notice : result.Error;
            if (result.Success && session.Match == null) paused = help = abandon = false;
            renderApp();
        }
        void ContactDown(PointerDownEvent e)
        {
            contacts.Add(e.pointerId);
            if (contacts.Count <= 1) return;
            card?.Hide(false);
            e.StopImmediatePropagation(); e.PreventDefault();
        }
        void ContactUp(PointerUpEvent e) { contacts.Remove(e.pointerId); }
        void ContactCancel(PointerCancelEvent e) { contacts.Remove(e.pointerId); }
        public void Pause(bool interrupted = false)
        {
            card?.Hide(true);
            // Android/Unity cancel their pointer stream on window interruption.
            // Navigation retains contacts until release; a new window starts fresh.
            if (interrupted) contacts.Clear();
            if (session.Match == null || session.Match.Phase == MatchPhase.Result) return;
            paused = true; help = false; abandon = false; Render();
        }
        public void Back()
        {
            card?.Hide(true);
            if (help) { help = false; Render(); }
            else if (abandon) { abandon = false; Render(); }
            else if (session.Match?.Phase == MatchPhase.Result) Act(session.ReturnToGroup(session.Match.Id));
            else if (!paused && session.Match?.SelectedSuspect != null) Act(session.CancelSuspect());
            else Pause();
        }
        public void Tick() { card?.Tick(); }
        public void Dispose()
        {
            card?.Dispose(); card = null;
            inputRoot.UnregisterCallback<PointerDownEvent>(ContactDown, TrickleDown.TrickleDown);
            inputRoot.UnregisterCallback<PointerUpEvent>(ContactUp, TrickleDown.TrickleDown);
            inputRoot.UnregisterCallback<PointerCancelEvent>(ContactCancel, TrickleDown.TrickleDown);
            contacts.Clear(); screen.RemoveFromHierarchy();
        }
        static Label Label(VisualElement parent, string name, string text, string css)
        {
            var label = new Label(text) { name = name }; label.AddToClassList(css); parent.Add(label); return label;
        }
        static VisualElement Box(VisualElement parent, string css)
        {
            var element = new VisualElement(); element.AddToClassList(css); parent.Add(element); return element;
        }
        Button Action(string name, string key, Action click, bool primary = true)
        {
            var button = new Button(click) { name = name, text = T(key) };
            button.AddToClassList(primary ? "play-button" : "match-secondary"); actions.Add(button); return button;
        }
        void Handoff(MatchView match)
        {
            Label(body,"cardProgress",T("cardProgress",match.HandoffNumber,match.Participants.Count),"card-progress");
            Label(body,"cardOwner",match.Owner.DisplayName,"card-owner");
            Label(body,"handoffHint",T("handoffHint"),"card-instruction");
            var slot = Box(body,"card-slot"); slot.name = "cardDrag";
            var face = Box(slot,"secret-card"); face.name = "cardFace"; face.pickingMode = PickingMode.Ignore;
            var symbol = Label(face,"cardSymbol","?","card-symbol"); symbol.pickingMode = PickingMode.Ignore;
            var caption = Label(face,"cardCaption",T("cardPrivate"),"card-caption"); caption.pickingMode = PickingMode.Ignore;
            var word = Label(face,"secretWord","","secret-word"); word.pickingMode = PickingMode.Ignore;
            Label(body,"dragHint",T("dragHint"),"card-instruction");
            var hold = Box(body,"hold-reveal"); hold.name = "holdReveal"; hold.focusable = true;
            Label(hold,"holdLabel",T("holdReveal"),"card-caption").pickingMode = PickingMode.Ignore;
            var next = Action("nextOwner",match.HandoffNumber == match.Participants.Count ? "beginClues" : "nextOwner",() => {
                if (card == null || !card.CanAdvance) return;
                card.Hide(true); Act(session.AdvanceHandoff(match.Owner.Id));
            });
            card = new SecretCard(session,match.Owner.Id,contacts,screen.parent,slot,hold,face,word,symbol,caption,next);
        }
        void Clues(MatchView match)
        {
            var center = Box(body,"match-center");
            Label(center,"clueTitle",T("clueTitle"),"match-title");
            Label(center,"clueInstructions",T("clueInstructions"),"match-text");
            var start = Box(center,"starting-person");
            Label(start,"startsLabel",T("starts"),"match-note");
            Label(start,"startingPlayer",match.StartingPlayer.DisplayName,"match-title");
            Label(center,"clueDiscussion",T("clueDiscussion"),"match-text");
            Action("beginVote","beginVote",() => Act(session.BeginVote()));
        }
        void Vote(MatchView match)
        {
            if (match.SelectedSuspect != null)
            {
                var center = Box(body,"match-center");
                Label(center,"confirmTitle",T("confirmTitle"),"match-title");
                Label(center,"confirmName",match.SelectedSuspect.DisplayName,"card-owner");
                Label(center,"confirmHint",T("confirmHint"),"match-text");
                string suspect = match.SelectedSuspect.Id;
                Action("confirmSuspect","confirmSuspect",() => Act(session.ConfirmSuspect(suspect)));
                Action("changeSuspect","changeSuspect",() => Act(session.CancelSuspect()),false);
                return;
            }
            Label(body,"voteTitle",T(match.Runoff ? "runoffTitle" : "voteTitle"),"match-title");
            Label(body,"voteInstructions",T(match.Runoff ? "runoffInstructions" : "voteInstructions"),"match-text");
            var list = new ScrollView { name = "suspects", horizontalScrollerVisibility = ScrollerVisibility.Hidden }; list.AddToClassList("vote-list"); body.Add(list);
            foreach (var person in match.Participants)
            {
                string id = person.Id;
                var button = new Button(() => Act(session.SelectSuspect(id))) { name = "suspect-" + id, text = person.DisplayName };
                button.AddToClassList("suspect-choice"); list.Add(button);
            }
            bool runoff = match.Runoff;
            Action("recordTie",runoff ? "secondTie" : "firstTie",() => Act(session.RecordTie(runoff)),false);
        }
        void Result(MatchView match)
        {
            var scroll = new ScrollView { name = "resultScroll", horizontalScrollerVisibility = ScrollerVisibility.Hidden }; scroll.AddToClassList("vote-list"); body.Add(scroll);
            var banner = Box(scroll,"result-banner");
            Label(banner,"resultTitle",T(match.Result.Winner == Role.Civilian ? "civilianWin" : "undercoverWin"),"match-title");
            Label(banner,"resultReason",T(match.Result.Reason.ToString()),"match-text");
            foreach (var role in new[] { Role.Civilian, Role.Undercover })
            {
                var box = Box(scroll,"result-word"); Label(box,"wordRole" + role,T(role.ToString()),"match-note");
                Label(box,"resultWord" + role,role == Role.Civilian ? match.Result.CivilianWord : match.Result.UndercoverWord,"match-title");
            }
            Label(scroll,"rolesTitle",T("rolesTitle"),"match-note");
            foreach (var assignment in match.Result.Roles)
            {
                var row = Box(scroll,"role-row"); Label(row,"roleName-" + assignment.Participant.Id,assignment.Participant.DisplayName,"role-name");
                Label(row,"role-" + assignment.Participant.Id,T(assignment.Role.ToString()),"role-label");
            }
            Action("rematch","rematch",() => { paused = false; Act(session.Rematch(match.Id)); });
            Action("editGroup","editGroup",() => Act(session.ReturnToGroup(match.Id)),false);
        }
        void Paused()
        {
            var center = Box(body,"match-center");
            Label(center,"pauseTitle",T("pauseTitle"),"match-title");
            Label(center,"pauseText",T("pauseText"),"match-text");
            Action("resumeMatch","resumeMatch",() => { paused = false; Render(); });
            Action("abandonMatch","abandonMatch",() => { abandon = true; Render(); },false).AddToClassList("match-danger");
        }
        void Abandon(MatchView match)
        {
            var center = Box(body,"match-center");
            Label(center,"abandonTitle",T("abandonTitle"),"match-title");
            Label(center,"abandonText",T("abandonText"),"match-text");
            Action("keepMatch","keepMatch",() => { abandon = false; Render(); });
            Action("confirmAbandon","confirmAbandon",() => Act(session.AbandonMatch(match.Id)),false).AddToClassList("match-danger");
        }
        void Help()
        {
            var scroll = new ScrollView { horizontalScrollerVisibility = ScrollerVisibility.Hidden }; scroll.AddToClassList("vote-list"); body.Add(scroll);
            Label(scroll,"helpTitle",T("helpTitle"),"match-title");
            Label(scroll,"helpText",T("helpText"),"match-text");
            Action("closeHelp","closeHelp",() => { help = false; Render(); });
        }
    }
}
