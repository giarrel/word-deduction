using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    // The public menu tree deliberately never traverses a private card.
    internal sealed class AccessibleMenu : IDisposable
    {
        readonly VisualElement root;
        readonly Func<Language> language;
        readonly List<(VisualElement element, AccessibilityNode node)> nodes = new List<(VisualElement, AccessibilityNode)>();
        readonly List<(ScrollView scroll, Action<float> changed)> scrollers = new List<(ScrollView, Action<float>)>();
        bool queued, disposed;
        string focusName;
        public AccessibilityHierarchy Hierarchy { get; private set; }
        public AccessibleMenu(VisualElement root, Func<Language> language)
        {
            this.root = root; this.language = language;
            AssistiveSupport.screenReaderStatusChanged += ReaderChanged;
            root.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
        }
        void GeometryChanged(GeometryChangedEvent e) { Refresh(); }
        void ReaderChanged(bool enabled) { if (enabled) Refresh(); }
        public void Refresh()
        {
            if (disposed || queued) return;
            queued = true;
            root.schedule.Execute(() => { queued = false; if (!disposed) Build(); });
        }
        void Build()
        {
            foreach (var item in scrollers) item.scroll.verticalScroller.valueChanged -= item.changed;
            scrollers.Clear(); nodes.Clear(); Hierarchy = new AccessibilityHierarchy();
            Visit(root,null);
            if (AssistiveSupport.isScreenReaderEnabled)
            {
                AssistiveSupport.activeHierarchy = Hierarchy;
                foreach (var pair in nodes)
                    if (!string.IsNullOrEmpty(focusName) && pair.element.name == focusName)
                    { AssistiveSupport.notificationDispatcher.SendLayoutChanged(pair.node); break; }
            }
        }
        void Visit(VisualElement element, AccessibilityNode parent)
        {
            if (element.resolvedStyle.display == DisplayStyle.None || element.ClassListContains("hidden")) return;
            if (element.name == "cardDrag" || element.name == "secretWord" || element.ClassListContains("player-initial") ||
                element.ClassListContains("empty-symbol") || element.ClassListContains("wordmark")) return;
            var scrollParent = element.GetFirstAncestorOfType<ScrollView>();
            if (scrollParent != null && !element.worldBound.Overlaps(scrollParent.contentViewport.worldBound)) return;
            if (element is Button button)
            {
                var node = Add(element,string.IsNullOrEmpty(button.tooltip) ? button.text : button.tooltip,parent,AccessibilityRole.Button);
                node.invoked += () => {
                    if (!button.enabledInHierarchy || button.panel == null) return false;
                    using (var e = NavigationSubmitEvent.GetPooled()) { e.target = button; button.SendEvent(e); }
                    return true;
                };
                return;
            }
            if (element is TextField field)
            {
                var node = Add(field,field.tooltip,parent,AccessibilityRole.TextField);
                node.value = field.value;
                node.invoked += () => { if (!field.enabledInHierarchy) return false; field.Focus(); return true; };
                return;
            }
            if (element is ScrollView scroll)
            {
                Action<float> changed = _ => Refresh(); scroll.verticalScroller.valueChanged += changed; scrollers.Add((scroll,changed));
                string key = scroll.name == "players" ? "scrollPlayers" : "scrollContent";
                var node = Add(scroll.contentViewport,Copy.Get(language(),key),parent,AccessibilityRole.ScrollView);
                node.scrolled += direction => {
                    float sign = direction == AccessibilityScrollDirection.Up || direction == AccessibilityScrollDirection.Backward || direction == AccessibilityScrollDirection.Left ? -1 : 1;
                    float before = scroll.scrollOffset.y;
                    scroll.scrollOffset = new Vector2(0,Mathf.Clamp(before + sign * scroll.contentViewport.layout.height * 0.75f,0,scroll.verticalScroller.highValue));
                    if (Mathf.Approximately(before,scroll.scrollOffset.y)) return false;
                    focusName = scroll.contentViewport.name; Refresh(); return true;
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
            node.frameGetter = () => Frame(element);
            node.focusChanged += (_,focused) => { if (focused) focusName = element.name; };
            nodes.Add((element,node)); Update(element,node); return node;
        }
        Rect Frame(VisualElement element)
        {
            var bounds = element.worldBound;
            float scale = root.resolvedStyle.width > 0 ? Screen.width / root.resolvedStyle.width : 1;
            return new Rect(bounds.x * scale,bounds.y * scale,bounds.width * scale,bounds.height * scale);
        }
        static void Update(VisualElement element,AccessibilityNode node)
        {
            node.state = !element.enabledInHierarchy ? AccessibilityState.Disabled : element.ClassListContains("selected") ? AccessibilityState.Selected : AccessibilityState.None;
            if (element is TextField field) node.value = field.value;
        }
        public void Tick()
        {
            foreach (var pair in nodes) Update(pair.element,pair.node);
        }
        public void Dispose()
        {
            disposed = true; AssistiveSupport.screenReaderStatusChanged -= ReaderChanged;
            root.UnregisterCallback<GeometryChangedEvent>(GeometryChanged);
            foreach (var item in scrollers) item.scroll.verticalScroller.valueChanged -= item.changed;
            scrollers.Clear();
            if (AssistiveSupport.activeHierarchy == Hierarchy) AssistiveSupport.activeHierarchy = null;
            nodes.Clear();
        }
    }
}
