using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UIElements;

namespace WordDeduction.Tests
{
    internal static class ScreenTestActions
    {
        public static void Submit(VisualElement element)
        {
            Assert.That(element, Is.Not.Null, "Required user action must be rendered.");
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = element; element.SendEvent(e); }
        }
        public static void Touch(VisualElement target, TouchPhase phase, Vector2 position, int finger = 0)
        {
            Assert.That(target, Is.Not.Null, "Required touch target must be rendered.");
            var touch = new Touch { fingerId = finger, position = position, phase = phase };
            if (phase == TouchPhase.Began) { using (var e = PointerDownEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else if (phase == TouchPhase.Moved) { using (var e = PointerMoveEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else if (phase == TouchPhase.Ended) { using (var e = PointerUpEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else if (phase == TouchPhase.Canceled) { using (var e = PointerCancelEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else throw new System.ArgumentOutOfRangeException(nameof(phase), phase, "Unsupported test touch phase.");
        }
        public static void TapAt(VisualElement root, Vector2 position)
        {
            var target = root.panel.Pick(position);
            Assert.That(target, Is.Not.Null, "The tap must hit the rendered screen.");
            using (var e = PointerDownEvent.GetPooled(new Touch { fingerId = 0, position = position, phase = TouchPhase.Began })) { e.target = target; target.SendEvent(e); }
            using (var e = PointerUpEvent.GetPooled(new Touch { fingerId = 0, position = position, phase = TouchPhase.Ended })) { e.target = target; target.SendEvent(e); }
        }
        public static IEnumerable<string> AllLabels(IEnumerable<AccessibilityNode> nodes)
        {
            foreach (var node in nodes)
            {
                yield return node.label + " " + node.value;
                foreach (var label in AllLabels(node.children)) yield return label;
            }
        }
        public static void SaveScreenshot(RenderTexture texture, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var previous = RenderTexture.active;
            var pixels = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            try
            {
                RenderTexture.active = texture;
                pixels.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0); pixels.Apply();
                File.WriteAllBytes(path, pixels.EncodeToPNG());
            }
            finally { RenderTexture.active = previous; Object.Destroy(pixels); }
        }
    }
}
