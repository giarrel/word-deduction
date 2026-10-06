using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Tests
{
    public class PolishScreenTests
    {
        GameObject host;
        PanelSettings panel;
        string directory;
        [TearDown] public void Cleanup()
        {
            if (host != null) UnityEngine.Object.Destroy(host);
            if (panel != null) UnityEngine.Object.Destroy(panel);
            if (directory != null && Directory.Exists(directory)) Directory.Delete(directory,true);
        }

        [UnityTest]
        public IEnumerator SmallGroupHasFingerSizedLanguageAndParticipationControls()
        {
            var root = Create(Language.German);
            yield return null; yield return null;
            foreach (var name in new[] { "Alexandra Maximiliane", "Bea", "Chris", "Dana", "Eli" })
            {
                root.Q<TextField>("nameInput").value = name;
                Submit(root.Q<Button>("addPlayer")); yield return null;
            }
            Submit(root.Q<Button>("classicMode")); yield return null; yield return null;
            foreach (var name in new[] { "german", "english", "addPlayer", "quickMode", "classicMode", "whitePreference", "playButton" })
            {
                var bounds = root.Q<Button>(name).worldBound;
                Assert.That(bounds.width, Is.GreaterThanOrEqualTo(48),name + " finger target width at 360dp");
                Assert.That(bounds.height, Is.GreaterThanOrEqualTo(48),name + " finger target height at 360dp");
                Assert.That(bounds.yMax,Is.LessThanOrEqualTo(640),name + " remains visible");
            }
            foreach (var button in root.Query<Button>(className:"participation").ToList())
                Assert.That(button.worldBound.height, Is.GreaterThanOrEqualTo(48));
        }

        VisualElement Create(Language language)
        {
            directory = Path.Combine(Application.temporaryCachePath,"polish-ui-"+Guid.NewGuid().ToString("N"));
            host = new GameObject("Polish rendered input test"); host.SetActive(false);
            panel = ScriptableObject.CreateInstance<PanelSettings>();
            // One logical unit per dp in this rendered 360 x 640 phone fixture.
            panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1;
            var document = host.AddComponent<UIDocument>(); document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(Session.Open(directory,language)); host.SetActive(true);
            var root = document.rootVisualElement; root.style.width = 360; root.style.height = 640;
            return root;
        }
        static void Submit(VisualElement target)
        {
            Assert.That(target,Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = target; target.SendEvent(e); }
        }
    }
}
