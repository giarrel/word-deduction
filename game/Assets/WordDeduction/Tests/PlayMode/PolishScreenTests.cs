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
        public IEnumerator LargeTextExplainsWhyQuickAndClassicCannotStart()
        {
            foreach (var language in new[] { Language.English, Language.German })
            foreach (var scenario in new[] { (GameMode.Quick, false), (GameMode.Classic, false), (GameMode.Classic, true) })
            {
                var mode = scenario.Item1;
                var root = Create(language, scenario.Item2); yield return null; yield return null;
                foreach (var name in mode == GameMode.Quick ? new[] { "Alex", "Bea" } : new[] { "Alex", "Bea", "Cora" })
                {
                    root.Q<TextField>("nameInput").value = name; Submit(root.Q<Button>("addPlayer")); yield return null;
                }
                Submit(root.Q<Button>(mode == GameMode.Quick ? "quickMode" : "classicMode")); yield return null; yield return null;
                Enlarge(root); yield return null; yield return null;
                var hint = root.Q<Label>("startHint"); var play = root.Q<Button>("playButton");
                Assert.That(play.enabledInHierarchy, Is.False);
                Assert.That(hint.resolvedStyle.display, Is.Not.EqualTo(DisplayStyle.None), "A disabled Play action must visibly explain the missing person at 150%.");
                Assert.That(hint.text, Does.Contain(language == Language.English ? "one" : "eine"));
                AssertReadable(hint, 616);
                Assert.That(hint.worldBound.yMin, Is.GreaterThanOrEqualTo(play.worldBound.yMax));
                foreach (var name in new[] { "german", "english", "addPlayer", "quickMode", "classicMode", "playButton" })
                    Assert.That(root.Q<Button>(name).worldBound.height, Is.GreaterThanOrEqualTo(48), name);
                Cleanup(); host = null; panel = null; directory = null; yield return null;
            }
        }
        static void Enlarge(VisualElement root)
        {
            root.AddToClassList("large-type");
            foreach (var element in root.Query<TextElement>().ToList())
                if (element.GetFirstAncestorOfType<TextField>() == null) element.style.fontSize = element.resolvedStyle.fontSize * 1.5f;
            foreach (var field in root.Query<TextField>().ToList()) field.style.fontSize = field.resolvedStyle.fontSize * 1.5f;
            root.Q<VisualElement>("safeRoot").style.paddingTop = 40;
            root.Q<VisualElement>("safeRoot").style.paddingBottom = 24;
        }
        [UnityTest]
        public IEnumerator LargeTextEmptyGroupKeepsTheWholeFirstStepVisible()
        {
            foreach (var language in new[] { Language.German, Language.English })
            {
                var root = Create(language); yield return null; yield return null;
                Enlarge(root); yield return null; yield return null;
                var hint = root.Q<Label>("emptyHint");
                AssertReadable(hint, 616);
                Assert.That(hint.worldBound.yMax, Is.LessThanOrEqualTo(root.Q<TextField>("nameInput").worldBound.yMin), "The complete introductory instruction stays above the name field.");
                Assert.That(hint.worldBound.yMin, Is.GreaterThanOrEqualTo(root.Q<Label>("emptyTitle").worldBound.yMax));
                var reason = root.Q<Label>("startHint");
                Assert.That(reason.resolvedStyle.display, Is.Not.EqualTo(DisplayStyle.None));
                AssertReadable(reason, 616);
                Cleanup(); host = null; panel = null; directory = null; yield return null;
            }
        }
        static void AssertReadable(Label label, float bottom)
        {
            Assert.That(label.worldBound.xMin, Is.GreaterThanOrEqualTo(0));
            Assert.That(label.worldBound.xMax, Is.LessThanOrEqualTo(360));
            Assert.That(label.worldBound.yMax, Is.LessThanOrEqualTo(bottom));
            Assert.That(label.contentRect.height, Is.GreaterThanOrEqualTo(label.MeasureTextSize(label.text, label.contentRect.width, VisualElement.MeasureMode.AtMost, 0, VisualElement.MeasureMode.Undefined).y - 1), "Every line fits in the label.");
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

        VisualElement Create(Language language, bool whitePreferred = false)
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
            var session = Session.Open(directory,language);
            if (whitePreferred) session.SetWhitePreference(true);
            host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
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
        [UnityTest]
        public IEnumerator MenuFramesFollowResizingAndScrollWithoutReplacingTheHierarchy()
        {
            var root = Create(Language.English); yield return null; yield return null;
            foreach (var name in new[]{"Alex","Bea","Chris","Dana","Eli"})
            {
                root.Q<TextField>("nameInput").value = name; Submit(root.Q<Button>("addPlayer")); yield return null;
            }
            Submit(root.Q<Button>("classicMode")); yield return null; yield return null;
            var screen = host.GetComponent<GroupScreen>(); var hierarchy = screen.Accessibility;
            var scroll = root.Q<ScrollView>("players");
            var menu = hierarchy.rootNodes.First(n => n.role == UnityEngine.Accessibility.AccessibilityRole.ScrollView);
            float originalHeight = menu.frame.height;
            root.style.height = 560; yield return null; yield return null; yield return null;
            Assert.That(screen.Accessibility,Is.SameAs(hierarchy),"Viewport changes preserve native node identity and focus.");
            Assert.That(menu.frame.height,Is.LessThan(originalHeight));
            Assert.That(menu.children.Any(n => !n.isActive),Is.True,"Offscreen rows are excluded from exploration.");
            scroll.scrollOffset = new Vector2(0,scroll.verticalScroller.highValue);
            yield return null; yield return null; yield return null;
            Assert.That(screen.Accessibility,Is.SameAs(hierarchy),"Scrolling must not announce a replacement screen.");
            foreach (var node in menu.children.Where(n => n.isActive))
            {
                Assert.That(node.frame.width,Is.GreaterThan(0));
                Assert.That(node.frame.yMin,Is.GreaterThanOrEqualTo(menu.frame.yMin-1));
                Assert.That(node.frame.yMax,Is.LessThanOrEqualTo(menu.frame.yMax+1));
            }
            Assert.That(menu.children.Any(n => n.isActive && n.label.Contains("Eli")),Is.True,"The last row becomes reachable after scroll.");
        }
        [UnityTest]
        public IEnumerator LargeTextCardKeepsDecorativeMarkClearOfItsCaption()
        {
            var root = Create(Language.English); yield return null; yield return null;
            foreach (var name in new[]{"Alex","Bea","Chris","Dana","Eli"})
            {
                root.Q<TextField>("nameInput").value = name; Submit(root.Q<Button>("addPlayer")); yield return null;
            }
            Submit(root.Q<Button>("classicMode")); Submit(root.Q<Button>("playButton"));
            yield return null; yield return null;
            // Reproduce the native 1.5x caption geometry inside the small-phone card.
            // Android itself remains the test of the OS preference event.
            root.AddToClassList("large-type");
            var caption = root.Q<Label>("cardCaption"); caption.style.fontSize = 24;
            yield return null; yield return null;
            var back = root.Q<VisualElement>(className:"mark-back");
            var front = root.Q<VisualElement>(className:"mark-front");
            Assert.That(Mathf.Max(back.worldBound.yMax,front.worldBound.yMax),Is.LessThanOrEqualTo(caption.worldBound.yMin),"Decorative cards must not collide with enlarged public text.");
            Assert.That(caption.worldBound.yMax,Is.LessThanOrEqualTo(root.Q<VisualElement>("cardFace").worldBound.yMax));
        }
        [UnityTest]
        public IEnumerator LargeGermanEditingKeepsAllActionsVisibleAboveTheKeyboard()
        {
            var root = Create(Language.German); yield return null; yield return null;
            for(int i=0;i<40;i++)
            {
                root.Q<TextField>("nameInput").value = "Alexandria-Maximilian-"+i.ToString("D2");
                Submit(root.Q<Button>("addPlayer")); yield return null;
            }
            root.AddToClassList("large-type"); yield return null; yield return null;
            Assert.That(root.Q<Label>("editHint").resolvedStyle.display,Is.Not.EqualTo(DisplayStyle.None),"Capacity remains explained at enlarged text.");
            var last = Session.Open(directory,Language.German).View.Players.Last();
            Submit(root.Q<Button>("edit-"+last.Id)); yield return null; yield return null;
            root.style.height = 380; yield return null; yield return null;
            root.AddToClassList("large-type"); root.Q<VisualElement>("safeRoot").AddToClassList("typing");
            foreach(var name in new[]{"removePlayer","cancelRename","saveRename"}) root.Q<Button>(name).style.fontSize=22.5f;
            root.Q<TextField>("renameInput").style.fontSize=25.5f;
            yield return null; yield return null;
            var list=root.Q<ScrollView>("players"); list.ScrollTo(root.Q<VisualElement>("player-"+last.Id));
            yield return null; yield return null;
            foreach(var name in new[]{"removePlayer","cancelRename","saveRename"})
            {
                var bounds=root.Q<Button>(name).worldBound;
                Assert.That(bounds.xMin,Is.GreaterThanOrEqualTo(list.contentViewport.worldBound.xMin),name);
                Assert.That(bounds.xMax,Is.LessThanOrEqualTo(list.contentViewport.worldBound.xMax),name+" has a complete visible label");
                Assert.That(bounds.yMin,Is.GreaterThanOrEqualTo(list.contentViewport.worldBound.yMin),name);
                Assert.That(bounds.yMax,Is.LessThanOrEqualTo(list.contentViewport.worldBound.yMax),name+" remains above the keyboard");
                Assert.That(bounds.height,Is.GreaterThanOrEqualTo(48),name);
            }
            root.Q<TextField>("renameInput").value="Mina"; Submit(root.Q<Button>("saveRename")); yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Last().Name,Is.EqualTo("Mina"));
        }
        [UnityTest]
        public IEnumerator LargeLongOwnerKeepsCardAndActionsSeparateInsidePhoneInsets()
        {
            var root=Create(Language.German); yield return null; yield return null;
            string wide=new string('M',24);
            foreach(var name in new[]{wide,wide,"Bea","Chris","Dana"})
            {
                root.Q<TextField>("nameInput").value=name; Submit(root.Q<Button>("addPlayer")); yield return null;
            }
            Submit(root.Q<Button>("classicMode")); Submit(root.Q<Button>("playButton")); yield return null; yield return null;
            root.AddToClassList("large-type"); yield return null;
            foreach(var element in root.Query<TextElement>().ToList())
                if(element.name!="secretWord" && element.name!="markQuestion" && element.GetFirstAncestorOfType<TextField>()==null) element.style.fontSize=element.resolvedStyle.fontSize*1.5f;
            var safe=root.Q<VisualElement>("safeRoot"); safe.style.paddingTop=40; safe.style.paddingBottom=24;
            yield return null; yield return null;
            var owner=root.Q<Label>("cardOwner"); var face=root.Q<VisualElement>("cardFace"); var hold=root.Q<VisualElement>("holdReveal");
            Assert.That(root.Q<Label>("cardProgress").worldBound.yMin,Is.GreaterThanOrEqualTo(root.Q<VisualElement>(className:"match-toolbar").worldBound.yMax));
            Assert.That(owner.text,Does.Contain("· 1"),"The whole generated discriminator remains visible.");
            Assert.That(owner.contentRect.height,Is.GreaterThanOrEqualTo(owner.MeasureTextSize(owner.text,owner.contentRect.width,VisualElement.MeasureMode.AtMost,0,VisualElement.MeasureMode.Undefined).y-1),"All wrapped owner lines fit.");
            Assert.That(owner.worldBound.yMax,Is.LessThan(face.worldBound.yMin));
            Assert.That(face.worldBound.yMax,Is.LessThanOrEqualTo(hold.worldBound.yMin));
            Assert.That(hold.worldBound.yMax,Is.LessThanOrEqualTo(root.Q<Button>("nextOwner").worldBound.yMin));
            Assert.That(root.Q<Button>("nextOwner").worldBound.yMax,Is.LessThanOrEqualTo(616));
            using(var e=PointerDownEvent.GetPooled(new Touch{fingerId=0,position=hold.worldBound.center,phase=TouchPhase.Began})) {e.target=hold;hold.SendEvent(e);}
            yield return null;
            Assert.That(root.Q<Label>("secretWord").text,Is.Not.Empty);
            Assert.That(face.worldBound.yMin,Is.GreaterThan(owner.worldBound.yMax),"The lifted private card must retain the complete owner above it.");
            using(var e=PointerUpEvent.GetPooled(new Touch{fingerId=0,position=hold.worldBound.center,phase=TouchPhase.Ended})) {e.target=hold;hold.SendEvent(e);}
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty);
        }
        static void Submit(VisualElement target)
        {
            Assert.That(target,Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = target; target.SendEvent(e); }
        }
    }
}
