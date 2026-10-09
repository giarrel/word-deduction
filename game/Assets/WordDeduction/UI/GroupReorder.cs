using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    // A drag is a disposable preview; Session receives one complete order only on a valid drop.
    internal sealed class GroupReorder : PointerManipulator
    {
        readonly ScrollView list;
        readonly VisualElement row;
        readonly string[] activeIds;
        readonly Action<int> commit, endContact;
        int pointer = -1, destination;
        Vector2 start, position;
        float startScroll;
        bool moved;
        TouchControl nativeTouch;
        IInputStateChangeMonitor nativeMonitor;
        int nativeTouchId;
        bool nativeContactLive, nativeCanceled, nativeEnded;
        VisualElement candidate;
        IVisualElementScheduledItem scrolling;
        public GroupReorder(ScrollView list, VisualElement row, string[] activeIds, Action<int> commit, Action<int> endContact)
        { this.list = list; this.row = row; this.activeIds = activeIds; this.commit = commit; this.endContact = endContact; }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(Down);
            target.RegisterCallback<PointerMoveEvent>(Move);
            target.RegisterCallback<PointerUpEvent>(Up);
            target.RegisterCallback<PointerCancelEvent>(CancelPointer);
            target.RegisterCallback<PointerCaptureOutEvent>(CaptureLost);
            target.RegisterCallback<DetachFromPanelEvent>(Detached);
        }
        protected override void UnregisterCallbacksFromTarget()
        {
            Cancel();
            target.UnregisterCallback<PointerDownEvent>(Down);
            target.UnregisterCallback<PointerMoveEvent>(Move);
            target.UnregisterCallback<PointerUpEvent>(Up);
            target.UnregisterCallback<PointerCancelEvent>(CancelPointer);
            target.UnregisterCallback<PointerCaptureOutEvent>(CaptureLost);
            target.UnregisterCallback<DetachFromPanelEvent>(Detached);
        }
        void Down(PointerDownEvent e)
        {
            if (pointer >= 0 || e.button != 0 || !target.enabledInHierarchy) return;
            pointer = e.pointerId; start = position = e.position; startScroll = list.scrollOffset.y;
            moved = false; destination = -1;
            nativeContactLive = nativeCanceled = nativeEnded = false;
            var screen = Touchscreen.current;
            int index = pointer - PointerId.touchPointerIdBase;
            if (e.pointerType == UnityEngine.UIElements.PointerType.touch && screen != null && index >= 0 && index < screen.touches.Count)
            {
                nativeTouch = screen.touches[index]; nativeTouchId = nativeTouch.touchId.ReadValue();
                var phase = nativeTouch.phase.ReadValue();
                nativeContactLive = phase == UnityEngine.InputSystem.TouchPhase.Began || phase == UnityEngine.InputSystem.TouchPhase.Moved
                    || phase == UnityEngine.InputSystem.TouchPhase.Stationary;
                nativeMonitor = InputState.AddChangeMonitor(nativeTouch.phase,ObserveNativeTouch);
            }
            target.CapturePointer(pointer); e.StopPropagation();
            scrolling = target.schedule.Execute(AutoScroll).Every(16);
        }
        void Move(PointerMoveEvent e)
        {
            if (e.pointerId != pointer) return;
            position = e.position; moved |= Vector2.Distance(start, position) >= 6;
            Preview(); e.StopPropagation();
        }
        void Preview()
        {
            if (!moved) return;
            row.AddToClassList("reordering");
            row.style.translate = new Translate(0,position.y - start.y + list.scrollOffset.y - startScroll,0);
            candidate?.RemoveFromClassList("reorder-target"); candidate = null;
            if (!list.contentViewport.worldBound.Contains(position)) { destination = -1; return; }
            var others = activeIds.Select(id => list.Q<VisualElement>("player-" + id)).Where(item => item != row).ToArray();
            destination = others.Count(item => position.y > item.worldBound.center.y);
            candidate = others.Length == 0 ? null : others[Mathf.Min(destination,others.Length - 1)];
            candidate?.AddToClassList("reorder-target");
        }
        void AutoScroll()
        {
            if (pointer < 0 || !moved) return;
            var bounds = list.contentViewport.worldBound;
            if (position.x < bounds.xMin || position.x > bounds.xMax) return;
            float direction = position.y < bounds.yMin + 36 ? -1 : position.y > bounds.yMax - 36 ? 1 : 0;
            if (direction != 0)
            {
                list.scrollOffset = new Vector2(0,Mathf.Clamp(list.scrollOffset.y + direction * 5,0,list.verticalScroller.highValue));
                Preview();
            }
        }
        void Up(PointerUpEvent e)
        {
            if (e.pointerId != pointer) return;
            position = e.position; Preview();
            // A reused or already terminal slot cannot identify a buffered Down safely.
            // Require the captured contact's deliberate end, not merely an arbitrary UI Up.
            bool released = nativeTouch == null || (nativeContactLive && (nativeEnded
                || (nativeTouch.touchId.ReadValue() == nativeTouchId && nativeTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended)));
            int drop = released && !nativeCanceled && moved && list.contentViewport.worldBound.Contains(position) ? destination : -1;
            endContact(e.pointerId);
            Cancel(); e.StopPropagation();
            if (drop >= 0) commit(drop);
        }
        void ObserveNativeTouch(InputControl control, double time, InputEventPtr input, long monitorIndex)
        {
            // InputForUI buffers a canceled touch as PointerUp. Observe each state write
            // before the next native Begin can reuse this slot within the same update.
            if (nativeTouch == null || nativeTouch.touchId.ReadValue() != nativeTouchId) return;
            var phase = nativeTouch.phase.ReadValue();
            nativeCanceled |= phase == UnityEngine.InputSystem.TouchPhase.Canceled;
            nativeEnded |= phase == UnityEngine.InputSystem.TouchPhase.Ended;
        }
        void CancelPointer(PointerCancelEvent e) { if (e.pointerId == pointer) { endContact(e.pointerId); Cancel(); } }
        void CaptureLost(PointerCaptureOutEvent e) { if (e.pointerId == pointer) Cancel(); }
        void Detached(DetachFromPanelEvent e) { Cancel(); }
        public void Cancel()
        {
            if (nativeTouch != null && nativeMonitor != null) InputState.RemoveChangeMonitor(nativeTouch.phase,nativeMonitor);
            nativeMonitor = null; nativeTouch = null;
            int previous = pointer; pointer = -1;
            scrolling?.Pause(); scrolling = null;
            row.style.translate = new Translate(0,0,0); row.RemoveFromClassList("reordering");
            candidate?.RemoveFromClassList("reorder-target"); candidate = null;
            if (previous >= 0 && target.HasPointerCapture(previous)) target.ReleasePointer(previous);
        }
    }
}
