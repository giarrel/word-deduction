using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    // Public menus only: private card text never enters the native hierarchy.
    internal sealed class AccessibleMenu : IDisposable
    {
        readonly VisualElement root;
        readonly Func<Language> language;
        readonly List<(VisualElement element, AccessibilityNode node)> nodes = new List<(VisualElement, AccessibilityNode)>();
        bool disposed;
        int refreshFrame = -1;
        string focusName;
        public AccessibilityHierarchy Hierarchy { get; private set; }
        public AccessibleMenu(VisualElement root, Func<Language> language)
        {
            this.root = root; this.language = language;
            AssistiveSupport.screenReaderStatusChanged += ReaderChanged;
        }
        void ReaderChanged(bool enabled) { if (enabled) Refresh(); }
        public void Refresh()
        {
            if (!disposed) refreshFrame = Time.frameCount;
        }
        void Build()
        {
            nodes.Clear(); Hierarchy = new AccessibilityHierarchy();
            Visit(root,null);
            if (!AssistiveSupport.isScreenReaderEnabled) return;
            AssistiveSupport.activeHierarchy = Hierarchy;
            var current = Hierarchy;
            root.schedule.Execute(() => {
                if (disposed || Hierarchy != current) return;
                Tick();
                foreach (var pair in nodes)
                    if (pair.node.isActive && !string.IsNullOrEmpty(focusName) && pair.element.name == focusName)
                    { AssistiveSupport.notificationDispatcher.SendLayoutChanged(pair.node); break; }
            });
        }
        void Visit(VisualElement element, AccessibilityNode parent)
        {
            if (element.resolvedStyle.display == DisplayStyle.None || element.ClassListContains("hidden")) return;
            if (element.name == "cardDrag" || element.name == "secretWord" || element.ClassListContains("player-initial") ||
                element.ClassListContains("empty-symbol") || element.ClassListContains("wordmark")) return;
            if (element is Button button)
            {
                var node = Add(element,string.IsNullOrEmpty(button.tooltip) ? button.text : button.tooltip,parent,AccessibilityRole.Button);
                node.invoked += () => {
                    if (!node.isActive || !button.enabledInHierarchy || button.panel == null) return false;
                    using (var e = NavigationSubmitEvent.GetPooled()) { e.target = button; button.SendEvent(e); }
                    return true;
                };
                return;
            }
            if (element is TextField field)
            {
                var node = Add(field,field.tooltip,parent,AccessibilityRole.TextField);
                node.value = field.value;
                node.invoked += () => { if (!node.isActive || !field.enabledInHierarchy) return false; field.Focus(); return true; };
                return;
            }
            if (element is ScrollView scroll)
            {
                string key = scroll.name == "players" ? "scrollPlayers" : "scrollContent";
                var node = Add(scroll.contentViewport,Copy.Get(language(),key),parent,AccessibilityRole.ScrollView);
                node.scrolled += direction => {
                    if (!node.isActive) return false;
                    float sign = direction == AccessibilityScrollDirection.Up || direction == AccessibilityScrollDirection.Backward || direction == AccessibilityScrollDirection.Left ? -1 : 1;
                    float before = scroll.scrollOffset.y;
                    scroll.scrollOffset = new Vector2(0,Mathf.Clamp(before + sign * scroll.contentViewport.layout.height * 0.75f,0,scroll.verticalScroller.highValue));
                    return !Mathf.Approximately(before,scroll.scrollOffset.y);
                };
                foreach (var child in scroll.Children()) Visit(child,node);
                return;
            }
            if (element.name == "holdReveal")
            {
                Add(element,Copy.Get(language(),"privateCardAccessibility"),parent,AccessibilityRole.StaticText);
                return;
            }
            if (element is TextElement label && !string.IsNullOrWhiteSpace(label.text))
            {
                Add(element,label.text,parent,element.ClassListContains("match-title") || element.ClassListContains("title") ? AccessibilityRole.Header : AccessibilityRole.StaticText);
                return;
            }
            foreach (var child in element.Children()) Visit(child,parent);
        }
        AccessibilityNode Add(VisualElement element,string label,AccessibilityNode parent,AccessibilityRole role)
        {
            var node = Hierarchy.AddNode(label ?? "",parent); node.role = role;
            node.frame = Frame(element);
            node.focusChanged += (_,focused) => { if (focused) focusName = element.name; };
            nodes.Add((element,node)); Update(element,node); return node;
        }
        Rect Frame(VisualElement element)
        {
            var bounds = element.worldBound;
            var scroller = element.GetFirstAncestorOfType<ScrollView>();
            if (scroller != null) bounds = Intersection(bounds,scroller.contentViewport.worldBound);
            float scale = root.resolvedStyle.width > 0 ? Screen.width / root.resolvedStyle.width : 1;
            return new Rect(bounds.x * scale,bounds.y * scale,bounds.width * scale,bounds.height * scale);
        }
        static Rect Intersection(Rect a,Rect b)
        {
            float x = Mathf.Max(a.xMin,b.xMin), y = Mathf.Max(a.yMin,b.yMin);
            return Rect.MinMaxRect(x,y,Mathf.Max(x,Mathf.Min(a.xMax,b.xMax)),Mathf.Max(y,Mathf.Min(a.yMax,b.yMax)));
        }
        static void Update(VisualElement element,AccessibilityNode node)
        {
            bool visible = element.worldBound.width > 0 && element.worldBound.height > 0;
            for (var ancestor = element; ancestor != null && visible; ancestor = ancestor.parent) visible = ancestor.resolvedStyle.display != DisplayStyle.None && ancestor.resolvedStyle.visibility == Visibility.Visible;
            var scroll = element.GetFirstAncestorOfType<ScrollView>();
            if (scroll != null) visible &= element.worldBound.Overlaps(scroll.contentViewport.worldBound);
            node.isActive = visible;
            node.state = !element.enabledInHierarchy ? AccessibilityState.Disabled : element.ClassListContains("selected") ? AccessibilityState.Selected : AccessibilityState.None;
            if (element is TextField field) node.value = field.value;
        }
        public void Tick()
        {
            if (disposed) return;
            // Refresh can precede UI Toolkit's style/layout pass. The following
            // frame sees the newly shown panel instead of its stale display:none.
            if (refreshFrame >= 0 && Time.frameCount > refreshFrame)
            {
                refreshFrame = -1;
                Build();
            }
            foreach (var pair in nodes)
            {
                // Unity's Android native frames otherwise retain pre-layout zero bounds.
                // Mutate the frame without replacing the hierarchy or screen-reader focus.
                var frame = Frame(pair.element);
                if (pair.node.frame != frame) pair.node.frame = frame;
                Update(pair.element,pair.node);
            }
        }
        public void Dispose()
        {
            disposed = true; AssistiveSupport.screenReaderStatusChanged -= ReaderChanged;
            if (AssistiveSupport.activeHierarchy == Hierarchy) AssistiveSupport.activeHierarchy = null;
            nodes.Clear();
        }
    }
}
