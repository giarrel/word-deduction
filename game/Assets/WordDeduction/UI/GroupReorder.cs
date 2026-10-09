using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
            int drop = !WasCanceledTouch(e) && moved && list.contentViewport.worldBound.Contains(position) ? destination : -1;
            endContact(e.pointerId);
            Cancel(); e.StopPropagation();
            if (drop >= 0) commit(drop);
        }
        static bool WasCanceledTouch(PointerUpEvent e)
        {
            // InputForUI reports canceled native touches as PointerUp, not PointerCancel.
            // Its touch pointer index is the Touchscreen slot, not the native touchId.
            var screen = Touchscreen.current;
            int index = e.pointerId - PointerId.touchPointerIdBase;
            return e.pointerType == UnityEngine.UIElements.PointerType.touch && screen != null && index >= 0 && index < screen.touches.Count
                && screen.touches[index].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled;
        }
        void CancelPointer(PointerCancelEvent e) { if (e.pointerId == pointer) { endContact(e.pointerId); Cancel(); } }
        void CaptureLost(PointerCaptureOutEvent e) { if (e.pointerId == pointer) Cancel(); }
        void Detached(DetachFromPanelEvent e) { Cancel(); }
        public void Cancel()
        {
            int previous = pointer; pointer = -1;
            scrolling?.Pause(); scrolling = null;
            row.style.translate = new Translate(0,0,0); row.RemoveFromClassList("reordering");
            candidate?.RemoveFromClassList("reorder-target"); candidate = null;
            if (previous >= 0 && target.HasPointerCapture(previous)) target.ReleasePointer(previous);
        }
    }
}
