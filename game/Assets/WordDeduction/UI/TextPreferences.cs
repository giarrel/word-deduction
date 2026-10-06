using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    // Keep text larger without shrinking Android's physical touch targets.
    internal sealed class TextPreferences : IDisposable
    {
        readonly VisualElement root;
        Dictionary<VisualElement,float> sizes = new Dictionary<VisualElement,float>();
        bool queued, disposed;
        float scale;
        public TextPreferences(VisualElement root)
        {
            this.root = root; scale = Mathf.Clamp(AccessibilitySettings.fontScale,1,1.5f);
            AccessibilitySettings.fontScaleChanged += Changed;
        }
        void Changed(float value)
        {
            scale = Mathf.Clamp(value,1,1.5f);
            foreach (var pair in sizes) pair.Key.style.fontSize = StyleKeyword.Null;
            sizes.Clear(); Refresh();
        }
        public void Refresh()
        {
            if (disposed || queued) return;
            root.EnableInClassList("large-type",scale > 1.1f);
            queued = true;
            root.schedule.Execute(() => {
                queued = false; if (disposed) return;
                var active = new Dictionary<VisualElement,float>();
                foreach (var element in root.Query<TextElement>().ToList())
                    if (element.name != "secretWord" && element.name != "markQuestion" && element.GetFirstAncestorOfType<TextField>() == null) Apply(element,active);
                foreach (var field in root.Query<TextField>().ToList()) Apply(field,active);
                sizes = active;
            });
        }
        void Apply(VisualElement element,Dictionary<VisualElement,float> active)
        {
            if (!sizes.TryGetValue(element,out float size)) size = element.resolvedStyle.fontSize;
            if (float.IsNaN(size) || size <= 0) return;
            active[element] = size;
            if (scale > 1) element.style.fontSize = size * scale;
        }
        public void Dispose() { disposed = true; AccessibilitySettings.fontScaleChanged -= Changed; sizes.Clear(); }
    }
}