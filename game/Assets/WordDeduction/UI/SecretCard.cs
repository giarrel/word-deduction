using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    internal sealed class SecretCard : IDisposable
    {
        readonly Session session;
        readonly string owner;
        readonly VisualElement root, drag, hold, face;
        readonly Label word, symbol, caption;
        readonly Button next;
        int pointer = -1;
        VisualElement captured;
        float startY, lift;
        bool holding, disposed;
        public bool CanAdvance => pointer < 0 && lift <= 0.01f && session.Match != null && session.Match.CanAdvance;
        public SecretCard(Session session, string owner, VisualElement root, VisualElement drag, VisualElement hold, VisualElement face, Label word, Label symbol, Label caption, Button next)
        {
            this.session = session; this.owner = owner; this.root = root; this.drag = drag; this.hold = hold; this.face = face; this.word = word; this.symbol = symbol; this.caption = caption; this.next = next;
            root.RegisterCallback<PointerDownEvent>(OtherPointer,TrickleDown.TrickleDown);
            drag.RegisterCallback<PointerDownEvent>(DragDown); hold.RegisterCallback<PointerDownEvent>(HoldDown);
            foreach (var target in new[] { drag, hold })
            {
                target.RegisterCallback<PointerMoveEvent>(Move); target.RegisterCallback<PointerUpEvent>(Up);
                target.RegisterCallback<PointerCancelEvent>(Cancel); target.RegisterCallback<PointerCaptureOutEvent>(CaptureOut);
            }
            root.RegisterCallback<DetachFromPanelEvent>(Detach);
            Hide(true);
        }
        void OtherPointer(PointerDownEvent e)
        {
            if (pointer >= 0 && e.pointerId != pointer) { Hide(false); e.StopImmediatePropagation(); e.PreventDefault(); }
        }
        void DragDown(PointerDownEvent e) => Down(e,drag,false);
        void HoldDown(PointerDownEvent e) => Down(e,hold,true);
        void Down(PointerDownEvent e, VisualElement target, bool holdMode)
        {
            if (disposed || pointer >= 0 || e.button != 0) return;
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
        void Up(PointerUpEvent e) { if (e.pointerId == pointer) { Hide(false); e.StopPropagation(); } }
        void Cancel(PointerCancelEvent e) { if (e.pointerId == pointer) Hide(false); }
        void CaptureOut(PointerCaptureOutEvent e) { if (e.pointerId == pointer) Hide(false); }
        void Detach(DetachFromPanelEvent e) { Hide(true); }
        public void Hide(bool immediate)
        {
            // Remove the secret synchronously, before capture callbacks or decoration.
            Conceal();
            int oldPointer = pointer; var oldTarget = captured;
            pointer = -1; captured = null; holding = false;
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
            if (disposed || pointer >= 0 || lift <= 0) return;
            lift = Mathf.MoveTowards(lift,0,Time.unscaledDeltaTime * 700); Pose(); next.SetEnabled(CanAdvance);
        }
        public void Dispose()
        {
            Hide(true); disposed = true;
            root.UnregisterCallback<PointerDownEvent>(OtherPointer,TrickleDown.TrickleDown);
            root.UnregisterCallback<DetachFromPanelEvent>(Detach);
            drag.UnregisterCallback<PointerDownEvent>(DragDown); hold.UnregisterCallback<PointerDownEvent>(HoldDown);
            foreach (var target in new[] { drag, hold })
            {
                target.UnregisterCallback<PointerMoveEvent>(Move); target.UnregisterCallback<PointerUpEvent>(Up);
                target.UnregisterCallback<PointerCancelEvent>(Cancel); target.UnregisterCallback<PointerCaptureOutEvent>(CaptureOut);
            }
        }
    }
}
