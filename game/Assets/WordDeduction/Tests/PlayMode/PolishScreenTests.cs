using System;
using System.Collections;
using System.IO;
using System.Linq;
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
#if UNITY_EDITOR
            panel = UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
#else
            Assert.Ignore("Android testing uses the built production scene.");
#endif
            // One logical unit per dp in this rendered 360 x 640 phone fixture.
            panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1;
            var document = host.AddComponent<UIDocument>(); document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(Session.Open(directory,language)); host.SetActive(true);
            var root = document.rootVisualElement; root.style.width = 360; root.style.height = 640;
            return root;
        }
        [UnityTest]
        public IEnumerator InvisibleNamesAreRejectedAndExtendedNamesSurviveTheUnityRuntime()
        {
            var root = Create(Language.English);
            yield return null;
            foreach (var invisible in new[] { "\u200b", "\u200e", "\u0301", "\u200d\ufe0f" })
            {
                root.Q<TextField>("nameInput").value = invisible;
                Submit(root.Q<Button>("addPlayer")); yield return null;
                Assert.That(Session.Open(directory,Language.English).View.Players.Count,Is.Zero,"Invisible-only input must not create a blank identity.");
            }
            foreach (var element in new[] { "e\u0301", "👩🏽‍🚀" })
            {
                root.Q<TextField>("nameInput").value = string.Concat(Enumerable.Repeat(element,24));
                Submit(root.Q<Button>("addPlayer")); yield return null;
                Assert.That(root.Q<TextField>("nameInput").value,Is.Empty,"24 extended text elements are accepted in Unity, not just .NET.");
                int count = Session.Open(directory,Language.English).View.Players.Count;
                root.Q<TextField>("nameInput").value = string.Concat(Enumerable.Repeat(element,25));
                Submit(root.Q<Button>("addPlayer")); yield return null;
                Assert.That(Session.Open(directory,Language.English).View.Players.Count,Is.EqualTo(count),"25 elements cannot silently exceed the name limit.");
            }
        }
        [UnityTest]
        public IEnumerator PublicMenuSemanticsFollowNavigationAndNeverContainThePrivateWord()
        {
            var root = Create(Language.English);
            yield return null; yield return null;
            foreach (var name in new[]{"Alex","Bea","Chris"})
            {
                root.Q<TextField>("nameInput").value = name; Submit(root.Q<Button>("addPlayer")); yield return null;
            }
            yield return null;
            var screen = host.GetComponent<GroupScreen>();
            Assert.That(screen.Accessibility.rootNodes.Any(n => n.label == "Add player"),Is.True);
            Submit(root.Q<Button>("playButton")); yield return null; yield return null;
            var hold = root.Q<VisualElement>("holdReveal");
            using (var e = PointerDownEvent.GetPooled(new Touch{fingerId=0,position=hold.worldBound.center,phase=TouchPhase.Began})) { e.target=hold; hold.SendEvent(e); }
            yield return null;
            var word = root.Q<Label>("secretWord").text; Assert.That(word,Is.Not.Empty);
            var spoken = string.Join("\n",screen.Accessibility.rootNodes.Select(n => n.label + " " + n.value));
            Assert.That(spoken,Does.Not.Contain(word),"Private card text must never enter native menu semantics.");
            Assert.That(screen.Accessibility.rootNodes.Any(n => n.label == "Add player"),Is.False,"The hidden group cannot receive accessibility actions during a match.");
            using (var e = PointerUpEvent.GetPooled(new Touch{fingerId=0,position=hold.worldBound.center,phase=TouchPhase.Ended})) { e.target=hold; hold.SendEvent(e); }
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty);
            Submit(root.Q<Button>("matchHelp")); yield return null; yield return null;
            Assert.That(screen.Accessibility.rootNodes.Any(n => n.label == "Got it"),Is.True);
            Assert.That(screen.Accessibility.rootNodes.Any(n => n.label == "Hide & pass on"),Is.False,"Help contains only its own current actions.");
        }
        static void Submit(VisualElement target)
        {
            Assert.That(target,Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = target; target.SendEvent(e); }
        }
    }
}
