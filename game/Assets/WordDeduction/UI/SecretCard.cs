using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    internal sealed class SecretCard : IDisposable
    {
        readonly Session session;
        readonly string owner;
        readonly VisualElement root, drag, hold, face;
        readonly Label word, caption;
        readonly Label role, leader, known;
        readonly ScrollView privateInformation;
        readonly VisualElement symbol;
        readonly Button next;
        readonly HashSet<int> contacts;
        int pointer = -1;
        int nextPointer = -1;
        VisualElement captured;
        float startY, lift, grabLift, returnFrom;
        double returnStarted;
        const double ReturnDuration = 0.14;
        bool holding, disposed, showing, readyToAdvance;
        bool CardReady => pointer < 0 && lift <= 0.01f && readyToAdvance;
        public bool CanAdvance => contacts.Count == 0 && CardReady;
        public SecretCard(Session session, string owner, HashSet<int> contacts, VisualElement root, VisualElement drag, VisualElement hold, VisualElement face, Label word, VisualElement symbol, Label caption, Button next, ScrollView privateInformation = null)
        {
            this.contacts = contacts;
            this.session = session; this.owner = owner; this.root = root; this.drag = drag; this.hold = hold; this.face = face; this.word = word; this.symbol = symbol; this.caption = caption; this.next = next;
            this.privateInformation = privateInformation;
            role = face.Q<Label>("secretRole"); leader = face.Q<Label>("secretLeader"); known = face.Q<Label>("secretKnown");
            foreach (var label in new[] { role, leader, known }) if (label != null) label.enableRichText = false;
            drag.RegisterCallback<PointerDownEvent>(DragDown); hold.RegisterCallback<PointerDownEvent>(HoldDown);
            next.RegisterCallback<PointerDownEvent>(NextDown, TrickleDown.TrickleDown);
            next.RegisterCallback<PointerUpEvent>(NextUp, TrickleDown.TrickleDown);
            next.RegisterCallback<PointerCancelEvent>(NextCancel, TrickleDown.TrickleDown);
            foreach (var target in new[] { drag, hold })
            {
                target.RegisterCallback<PointerMoveEvent>(Move); target.RegisterCallback<PointerUpEvent>(Up);
                target.RegisterCallback<PointerCancelEvent>(Cancel); target.RegisterCallback<PointerCaptureOutEvent>(CaptureOut);
            }
            root.RegisterCallback<DetachFromPanelEvent>(Detach);
            word.RegisterCallback<GeometryChangedEvent>(WordGeometryChanged);
            Hide(true);
        }
        void DragDown(PointerDownEvent e) => Down(e,drag,false);
        void HoldDown(PointerDownEvent e) => Down(e,hold,true);
        void NextDown(PointerDownEvent e) { if (contacts.Count == 1 && CardReady) nextPointer = e.pointerId; }
        void NextUp(PointerUpEvent e) { contacts.Remove(e.pointerId); if (e.pointerId == nextPointer) nextPointer = -1; }
        void NextCancel(PointerCancelEvent e) { contacts.Remove(e.pointerId); if (e.pointerId == nextPointer) nextPointer = -1; }
        void Down(PointerDownEvent e, VisualElement target, bool holdMode)
        {
            if (disposed || contacts.Count != 1 || pointer >= 0 || e.button != 0) return;
            pointer = e.pointerId; startY = e.position.y; grabLift = VisibleLift; holding = holdMode; captured = target;
            captured.CapturePointer(pointer); next.SetEnabled(false);
            if (holding) { lift = 60; Show(); Pose(); }
            e.StopPropagation();
        }
        void Move(PointerMoveEvent e)
        {
            if (e.pointerId != pointer) return;
            if (holding)
            {
                ScrollPrivateInformation(e.position.y, 0);
                e.StopPropagation(); return;
            }
            float maximum = Mathf.Clamp(drag.resolvedStyle.height * 0.35f,60,110);
            float distance = startY - e.position.y;
            // Catch a returning card where it is, while a fresh deliberate pull
            // remains necessary to reveal. The visual never trails the finger.
            lift = Mathf.Clamp(grabLift + distance,0,maximum);
            if (distance >= maximum * 0.5f) Show(); else Conceal();
            ScrollPrivateInformation(e.position.y, maximum);
            Pose(); e.StopPropagation();
        }
        void ScrollPrivateInformation(float pointerY, float revealDistance)
        {
            if (privateInformation == null) return;
            // Map the full list to a reachable one-finger path. Reversing that
            // same held finger reads earlier names without a second contact.
            float safeTop = root.worldBound.yMin + root.resolvedStyle.paddingTop;
            float travel = Mathf.Clamp(startY - revealDistance - safeTop - 24, 48, 280);
            float range = privateInformation.verticalScroller.highValue;
            float progress = Mathf.Clamp01((startY - pointerY - revealDistance) / travel);
            privateInformation.scrollOffset = new Vector2(0, progress * range);
        }
        void Show()
        {
            if (showing || !MobilePrivacy.Ready) return;
            var card = session.RevealCard(owner);
            if (card == null) return;
            showing = true; readyToAdvance = false;
            var language = session.Match.Language;
            var text = card.Word;
            bool white = card.Kind == PrivateCardKind.White;
            root.AddToClassList("reading-card");
            word.text = white ? Copy.Get(language,"whitePrivate") : text ?? "";
            word.EnableInClassList("white-private",white); word.EnableInClassList("hidden",!white && text == null);
            word.EnableInClassList("whole-word",!white && text != null && !text.Contains(" "));
            if (white) word.style.fontSize = StyleKeyword.Null;
            else if (text != null) FitWord(text);
            if (privateInformation != null)
            {
                role.text = card.Kind == PrivateCardKind.GoodKing ? Copy.Get(language,"goodKingPrivate") : card.Kind == PrivateCardKind.EvilKing ? Copy.Get(language,"evilKingPrivate") : "";
                leader.text = card.Leader == null ? "" : Copy.Get(language,"privateLeader",card.Leader.DisplayName);
                known.text = card.KnownParticipants.Count == 0 ? "" : Copy.Get(language,card.Kind == PrivateCardKind.GoodKing ? "privateEvilNames" : "privateTeamNames") + "\n" + string.Join("\n",card.KnownParticipants.Select(p => p.DisplayName));
                role.EnableInClassList("hidden",role.text.Length == 0); leader.EnableInClassList("hidden",leader.text.Length == 0); known.EnableInClassList("hidden",known.text.Length == 0);
                privateInformation.RemoveFromClassList("hidden");
            }
            symbol.EnableInClassList("hidden",true); caption.EnableInClassList("hidden",true); face.AddToClassList("revealed");
        }
        void WordGeometryChanged(GeometryChangedEvent e)
        {
            if (word.text.Length > 0 && !word.ClassListContains("white-private")) FitWord(word.text);
        }
        void FitWord(string text)
        {
            // Keep each token whole; phrases may wrap only between words. The current
            // bilingual catalog fits the small card without going below readable 22dp.
            float available = word.contentRect.width;
            if (float.IsNaN(available) || available <= 0)
                available = face.contentRect.width - word.resolvedStyle.marginLeft - word.resolvedStyle.marginRight
                    - word.resolvedStyle.paddingLeft - word.resolvedStyle.paddingRight;
            available -= 2;
            float measuredSize = word.resolvedStyle.fontSize;
            float widest = 0;
            foreach (var token in text.Split(' '))
                widest = Mathf.Max(widest,word.MeasureTextSize(token,0,VisualElement.MeasureMode.Undefined,0,VisualElement.MeasureMode.Undefined).x);
            if (available > 0 && measuredSize > 0 && widest > 0)
                word.style.fontSize = Mathf.Clamp(Mathf.Floor(measuredSize * available / widest),22,32);
        }
        void Conceal(bool force = false)
        {
            if (!showing && !force) return;
            showing = false;
            word.text = ""; session.HideWord(); word.EnableInClassList("hidden",true);
            if (privateInformation != null)
            {
                role.text = leader.text = known.text = "";
                privateInformation.AddToClassList("hidden"); privateInformation.scrollOffset = Vector2.zero;
            }
            root.RemoveFromClassList("reading-card");
            symbol.EnableInClassList("hidden",false); caption.EnableInClassList("hidden",false); face.RemoveFromClassList("revealed");
            // A card owns this presentation until the next rendered transition.
            // Read the detached match projection only when concealment changes;
            // the authoritative handoff command still validates advancement.
            readyToAdvance = session.Match?.CanAdvance == true;
        }
        // Captured UITK events can dispatch directly to this target without
        // traversing the root. End the shared contact here as well.
        void Up(PointerUpEvent e) { contacts.Remove(e.pointerId); if (e.pointerId == pointer) { Hide(false); e.StopPropagation(); } }
        void Cancel(PointerCancelEvent e) { contacts.Remove(e.pointerId); if (e.pointerId == pointer) Hide(false); }
        void CaptureOut(PointerCaptureOutEvent e) { if (e.pointerId == pointer) Hide(false); }
        void Detach(DetachFromPanelEvent e) { Hide(true); }
        public void Hide(bool immediate)
        {
            // Remove the secret synchronously, before capture callbacks or decoration.
            Conceal(true);
            int oldPointer = pointer; var oldTarget = captured;
            pointer = -1; captured = null; holding = false;
            nextPointer = -1;
            if (oldTarget != null && oldTarget.HasPointerCapture(oldPointer)) oldTarget.ReleasePointer(oldPointer);
            if (immediate || MobilePrivacy.ReduceMotion) { lift = 0; Pose(); }
            else
            {
                // Start at the visible position, including a short partial pull.
                // Hidden gesture distance must not delay the decorative return.
                lift = returnFrom = VisibleLift; returnStarted = Time.unscaledTimeAsDouble;
            }
            next.SetEnabled(CanAdvance);
        }
        float VisibleLift => float.IsNaN(face.layout.y) ? 0 : Mathf.Min(lift,Mathf.Max(0,face.layout.y - 6));
        void Pose()
        {
            // The gesture threshold stays unchanged; only decoration is bounded so
            // a long owner above the slot is never covered by the lifted card.
            float visualLift = VisibleLift;
            face.style.translate = new Translate(0,-visualLift,0);
            face.style.rotate = new Rotate(new Angle(MobilePrivacy.ReduceMotion ? 0 : -visualLift / 45));
        }
        public void Tick()
        {
            if (disposed) return;
            if (pointer < 0 && lift > 0)
            {
                float remaining = 1 - Mathf.Clamp01((float)((Time.unscaledTimeAsDouble - returnStarted) / ReturnDuration));
                lift = returnFrom * remaining * remaining; Pose();
            }
            // Keep an already pressed Next button enabled through its own Up.
            // The action still requires every contact to have ended.
            bool pressingNext = contacts.Count == 1 && contacts.Contains(nextPointer);
            next.SetEnabled(CardReady && (contacts.Count == 0 || pressingNext));
        }
        public void Dispose()
        {
            Hide(true); disposed = true;
            root.UnregisterCallback<DetachFromPanelEvent>(Detach);
            word.UnregisterCallback<GeometryChangedEvent>(WordGeometryChanged);
            drag.UnregisterCallback<PointerDownEvent>(DragDown); hold.UnregisterCallback<PointerDownEvent>(HoldDown);
            next.UnregisterCallback<PointerDownEvent>(NextDown, TrickleDown.TrickleDown);
            next.UnregisterCallback<PointerUpEvent>(NextUp, TrickleDown.TrickleDown);
            next.UnregisterCallback<PointerCancelEvent>(NextCancel, TrickleDown.TrickleDown);
            foreach (var target in new[] { drag, hold })
            {
                target.UnregisterCallback<PointerMoveEvent>(Move); target.UnregisterCallback<PointerUpEvent>(Up);
                target.UnregisterCallback<PointerCancelEvent>(Cancel); target.UnregisterCallback<PointerCaptureOutEvent>(CaptureOut);
            }
        }
    }
}
