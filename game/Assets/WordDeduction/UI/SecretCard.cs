using System;
using System.Collections.Generic;
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
        readonly VisualElement symbol;
        readonly Button next;
        readonly HashSet<int> contacts;
        int pointer = -1;
        int nextPointer = -1;
        VisualElement captured;
        float startY, lift;
        bool holding, disposed;
        bool CardReady => pointer < 0 && lift <= 0.01f && session.Match != null && session.Match.CanAdvance;
        public bool CanAdvance => contacts.Count == 0 && CardReady;
        public SecretCard(Session session, string owner, HashSet<int> contacts, VisualElement root, VisualElement drag, VisualElement hold, VisualElement face, Label word, VisualElement symbol, Label caption, Button next)
        {
            this.contacts = contacts;
            this.session = session; this.owner = owner; this.root = root; this.drag = drag; this.hold = hold; this.face = face; this.word = word; this.symbol = symbol; this.caption = caption; this.next = next;
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
            pointer = e.pointerId; startY = e.position.y; holding = holdMode; captured = target;
            captured.CapturePointer(pointer); next.SetEnabled(false);
            if (holding) { lift = 60; Show(); Pose(); }
            e.StopPropagation();
        }
        void Move(PointerMoveEvent e)
        {
            if (e.pointerId != pointer || holding) return;
            float maximum = Mathf.Clamp(drag.resolvedStyle.height * 0.35f,60,110);
            lift = Mathf.Clamp(startY - e.position.y,0,maximum);
            if (lift >= maximum * 0.5f) Show(); else Conceal();
            Pose(); e.StopPropagation();
        }
        void Show()
        {
            if (!MobilePrivacy.Ready) return;
            var text = session.RevealWord(owner);
            if (text == null) return;
            bool white = text == "Mr. White";
            word.text = white ? Copy.Get(session.Match.Language,"whitePrivate") : text;
            word.EnableInClassList("white-private",white);
            symbol.EnableInClassList("hidden",true); caption.EnableInClassList("hidden",true); face.AddToClassList("revealed");
        }
        void Conceal()
        {
            word.text = ""; session.HideWord();
            symbol.EnableInClassList("hidden",false); caption.EnableInClassList("hidden",false); face.RemoveFromClassList("revealed");
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
            Conceal();
            int oldPointer = pointer; var oldTarget = captured;
            pointer = -1; captured = null; holding = false;
            nextPointer = -1;
            if (oldTarget != null && oldTarget.HasPointerCapture(oldPointer)) oldTarget.ReleasePointer(oldPointer);
            if (immediate || MobilePrivacy.ReduceMotion) { lift = 0; Pose(); }
            next.SetEnabled(CanAdvance);
        }
        void Pose()
        {
            face.style.translate = new Translate(0,-lift,0);
            face.style.rotate = new Rotate(new Angle(MobilePrivacy.ReduceMotion ? 0 : -lift / 45));
        }
        public void Tick()
        {
            if (disposed) return;
            if (pointer < 0 && lift > 0) { lift = Mathf.MoveTowards(lift,0,Time.unscaledDeltaTime * 700); Pose(); }
            // Keep an already pressed Next button enabled through its own Up.
            // The action still requires every contact to have ended.
            bool pressingNext = contacts.Count == 1 && contacts.Contains(nextPointer);
            next.SetEnabled(CardReady && (contacts.Count == 0 || pressingNext));
        }
        public void Dispose()
        {
            Hide(true); disposed = true;
            root.UnregisterCallback<DetachFromPanelEvent>(Detach);
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
