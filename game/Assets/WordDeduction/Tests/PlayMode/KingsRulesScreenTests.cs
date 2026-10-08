using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Tests
{
    public class KingsRulesScreenTests
    {
        readonly List<Fixture> live = new List<Fixture>();
        [TearDown] public void Cleanup() { foreach (var fixture in live) fixture.Dispose(); live.Clear(); }

        [UnityTest] public IEnumerator OptionalKingsRulesAreReadableFromInformationAndMatchHelpInBothLanguages()
        {
            foreach (var language in new[] { Language.German, Language.English })
            {
                var fixture = new Fixture(language); live.Add(fixture); yield return null; yield return null;
                var root = fixture.Root;
                root.Q<TextField>("nameInput").value = "Unsaved name";
                Submit(root.Q<Button>("appInfo")); yield return null; yield return null;
                Submit(root.Q<Button>("showKingsRules")); yield return null; yield return null;
                root.AddToClassList("large-type"); yield return null;
                Enlarge(root); yield return null; yield return null;
                Assert.That(root.Q<Label>("helpTitle").text, Is.EqualTo(language == Language.German ? "Könige: die Regeln" : "Kings rules"));
                var rules = root.Query<Label>(className: "kings-rule-copy").ToList();
                Assert.That(rules.Count, Is.GreaterThanOrEqualTo(6), "Short titled sections make the complete optional rules scannable.");
                var scroll = root.Q<ScrollView>("kingsRulesScroll");
                Assert.That(scroll, Is.Not.Null);
                yield return new WaitForSecondsRealtime(0.15f); fixture.Capture("help-" + language + "-start");
                foreach (var label in rules)
                {
                    scroll.ScrollTo(label); yield return null;
                    AssertTextFits(label);
                }
                var lastRule = rules.Last(); scroll.ScrollTo(lastRule); yield return null;
                Assert.That(lastRule.worldBound.yMax, Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax + 1), "The end of the optional rules can be reached.");
                yield return null; yield return new WaitForSecondsRealtime(0.15f); fixture.Capture("help-" + language + "-last");
                Assert.That(root.Q<Button>("closeInfo").worldBound.yMax, Is.LessThanOrEqualTo(640));
                Assert.That(AllSemantics(fixture).Any(text => text.Contains("Unsaved name")), Is.False, "The group behind help is absent from the public hierarchy.");
                var informationRules = string.Join("\n", rules.Select(label => label.text));
                Submit(root.Q<Button>("closeInfo")); yield return null;
                Assert.That(root.Q<TextField>("nameInput").value, Is.EqualTo("Unsaved name"));
                Assert.That(fixture.Session.Match, Is.Null, "Reading optional rules never starts a match.");
                Submit(root.Q<Button>("kingsMode")); Submit(root.Q<Button>("playButton")); yield return null;
                var owner = fixture.Session.Match.Owner.Id;
                Submit(root.Q<Button>("matchHelp")); yield return null; yield return null;
                Assert.That(string.Join("\n", root.Query<Label>(className: "kings-rule-copy").ToList().Select(label => label.text)), Is.EqualTo(informationRules), "Setup and in-match help show the same rules.");
                Assert.That(root.Q<VisualElement>("holdReveal"), Is.Null);
                Submit(root.Q<Button>("closeHelp")); yield return null;
                Assert.That(fixture.Session.Match.Owner.Id, Is.EqualTo(owner));
                Assert.That(root.Q<Label>("secretWord").text, Is.Empty);
                fixture.Dispose(); yield return null;
            }
        }

        [UnityTest] public IEnumerator MaximalKingListsCanBeReadWithOneFingerInsideTheSmallPhoneAtLargeText()
        {
            foreach (var language in new[] { Language.German, Language.English })
            foreach (var kind in new[] { PrivateCardKind.GoodKing, PrivateCardKind.EvilKing })
            foreach (bool holdMode in new[] { true, false })
            {
                var fixture = new Fixture(language, 20); live.Add(fixture);
                for (int i = 0; i < 20; i++)
                {
                    string name = i < 4 ? "Alexandra Maximiliane" : i == 4 ? "Éléonore " + new string('M', 15) : i == 5 ? new string('山', 24) : i == 6 ? string.Concat(Enumerable.Repeat("👩🏽‍🚀", 24)) : new string('M', 24);
                    Assert.That(fixture.Session.RenamePlayer(fixture.Session.View.Players[i].Id, name).Success, Is.True);
                }
                Assert.That(fixture.Session.SetMode(GameMode.Kings).Success, Is.True);
                Assert.That(fixture.Session.SetKingsUndercoverPreference(8).Success, Is.True);
                Assert.That(fixture.Session.StartMatch().Success, Is.True);
                PrivateCardView expected;
                while (true)
                {
                    var owner = fixture.Session.Match.Owner.Id; expected = fixture.Session.RevealCard(owner); fixture.Session.HideWord();
                    if (expected.Kind == kind) break;
                    Assert.That(fixture.Session.AdvanceHandoff(owner).Success, Is.True);
                }
                fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null; yield return null;
                var root = fixture.Root; Submit(root.Q<Button>("resumeMatch")); yield return null; yield return null;
                root.AddToClassList("large-type"); yield return null;
                Enlarge(root); yield return null; yield return null;
                var hold = root.Q<VisualElement>("holdReveal");
                var target = holdMode ? hold : root.Q<VisualElement>("cardDrag"); var start = target.worldBound.center;
                string capture = "max-list-" + language + "-" + kind + (holdMode ? "-hold" : "-drag");
                Assert.That(new Rect(0, 0, 360, 640).Contains(start), Is.True, "The reveal target stays inside the physical phone.");
                Touch(target, TouchPhase.Began, start);
                if (!holdMode) Touch(target, TouchPhase.Moved, new Vector2(start.x, start.y - 60));
                yield return null; yield return null;
                yield return new WaitForSecondsRealtime(0.15f); fixture.Capture(capture + "-start");
                var viewport = root.Q<ScrollView>("secretInformation").contentViewport;
                var known = root.Q<Label>("secretKnown");
                Assert.That(expected.KnownParticipants.Count, Is.EqualTo(kind == PrivateCardKind.GoodKing ? 9 : 8));
                foreach (var person in expected.KnownParticipants) Assert.That(known.text, Does.Contain(person.DisplayName));
                for (float y = start.y - (holdMode ? 8 : 64); y >= 48; y -= 8)
                {
                    Touch(target, TouchPhase.Moved, new Vector2(start.x, y)); yield return null;
                }
                Touch(target, TouchPhase.Moved, new Vector2(start.x, 48)); yield return null; yield return null;
                yield return new WaitForSecondsRealtime(0.15f); fixture.Capture(capture + "-last");
                fixture.Note(capture, "start=" + start + "; target=" + target.worldBound + "; viewport=" + viewport.worldBound + "; known=" + known.worldBound + "; offset=" + root.Q<ScrollView>("secretInformation").scrollOffset + "; limit=" + root.Q<ScrollView>("secretInformation").verticalScroller.highValue);
                Assert.That(known.worldBound.yMax, Is.LessThanOrEqualTo(viewport.worldBound.yMax + 1), "A real in-bounds pointer path reaches the last complete name; assigning a scroll offset is not proof.");
                var lastName = expected.KnownParticipants.Last().DisplayName;
                float lastHeight = known.MeasureTextSize(lastName, known.contentRect.width, VisualElement.MeasureMode.AtMost, 0, VisualElement.MeasureMode.Undefined).y;
                Assert.That(known.worldBound.yMax - lastHeight, Is.GreaterThanOrEqualTo(viewport.worldBound.yMin - 1), "The final name fits fully, including its duplicate suffix.");
                Assert.That(root.Q<Button>("nextOwner").worldBound.yMax, Is.LessThanOrEqualTo(616));
                Assert.That(hold.worldBound.yMax, Is.LessThanOrEqualTo(root.Q<Button>("nextOwner").worldBound.yMin - 4), "Hold and Next remain separate finger targets.");
                Assert.That(root.Q<Button>("nextOwner").enabledSelf, Is.False);
                Touch(target, TouchPhase.Moved, new Vector2(start.x, start.y - (holdMode ? 0 : 60))); yield return null; yield return null;
                Assert.That(root.Q<Label>("secretRole").worldBound.yMin, Is.GreaterThanOrEqualTo(viewport.worldBound.yMin - 1), "The same held finger can return to the top.");
                Touch(target, TouchPhase.Ended, new Vector2(start.x, start.y - (holdMode ? 0 : 60)));
                Assert.That(PrivateLabels(root).All(label => label.text.Length == 0), Is.True, "Release immediately clears the complete projection.");
                yield return null; yield return null; yield return new WaitForSecondsRealtime(0.15f); fixture.Capture(capture + "-covered");
                var help = root.Q<Button>("matchHelp");
                float helpWidth = help.MeasureTextSize(help.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined).x;
                fixture.Note(capture + "-header", "help=" + help.text + "; bounds=" + help.worldBound + "; content=" + help.contentRect + "; textWidth=" + helpWidth);
                Assert.That(helpWidth, Is.LessThanOrEqualTo(help.contentRect.width + 1), "The complete public Help label fits after card release.");
                fixture.Dispose(); yield return null;
            }
        }
        [UnityTest] public IEnumerator AllThreeModesAndKingsCountControlsFitAtLargeTextWithTwentyPlayers()
        {
            foreach (var language in new[] { Language.German, Language.English })
            {
                var fixture = new Fixture(language, 20); live.Add(fixture); yield return null; yield return null;
                var root = fixture.Root; Submit(root.Q<Button>("kingsMode")); yield return null; yield return null;
                root.AddToClassList("large-type"); yield return null; Enlarge(root); yield return null; yield return null;
                yield return new WaitForSecondsRealtime(0.15f); fixture.Capture("setup-" + language + "-large");
                foreach (var name in new[] { "quickMode", "classicMode", "kingsMode", "lessUndercover", "moreUndercover", "automaticUndercover", "playButton" })
                {
                    var button = root.Q<Button>(name);
                    Assert.That(button.worldBound.xMin, Is.GreaterThanOrEqualTo(0), name);
                    Assert.That(button.worldBound.xMax, Is.LessThanOrEqualTo(360), name);
                    Assert.That(button.worldBound.yMax, Is.LessThanOrEqualTo(616), name);
                    Assert.That(button.worldBound.height, Is.GreaterThanOrEqualTo(48), name);
                    Assert.That(button.MeasureTextSize(button.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined).x, Is.LessThanOrEqualTo(button.contentRect.width + 1), name + " has its complete readable label.");
                }
                AssertTextFits(root.Q<Label>("kingsUndercoverCount"));
                Assert.That(root.Q<ScrollView>("players").contentViewport.worldBound.height, Is.GreaterThanOrEqualTo(48), "The saved group remains usable alongside configuration.");
                fixture.Dispose(); yield return null;
            }
        }
        [UnityTest] public IEnumerator LongKingsWordsFitBesideThePrivateScrollbarAndLongLeader()
        {
            foreach (var text in new[] { "Nuss-Nougat-Creme", "Rollkragenpullover", "Chocolate hazelnut spread" })
            {
                var language = text.Contains(" ") ? Language.English : Language.German;
                var fixture = new Fixture(language, 5, maximum => maximum - 1); live.Add(fixture);
                foreach (var player in fixture.Session.View.Players) Assert.That(fixture.Session.RenamePlayer(player.Id, new string('M', 24)).Success, Is.True);
                Assert.That(fixture.Session.SetMode(GameMode.Kings).Success, Is.True);
                bool found = false;
                for (int draw = 0; draw < 520 && !found; draw++)
                {
                    Assert.That(fixture.Session.StartMatch().Success, Is.True);
                    while (fixture.Session.Match.Phase == MatchPhase.Handoff)
                    {
                        var owner = fixture.Session.Match.Owner.Id; var card = fixture.Session.RevealCard(owner);
                        found = card.Kind == PrivateCardKind.Word && card.Word == text; fixture.Session.HideWord();
                        if (found) break;
                        Assert.That(fixture.Session.AdvanceHandoff(owner).Success, Is.True);
                    }
                    if (!found) Assert.That(fixture.Session.AbandonMatch(fixture.Session.Match.Id).Success, Is.True);
                }
                Assert.That(found, Is.True, "The word comes from a real Kings deal.");
                fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null; yield return null;
                var root = fixture.Root; Submit(root.Q<Button>("resumeMatch")); yield return null; yield return null;
                root.AddToClassList("large-type"); yield return null; Enlarge(root); yield return null; yield return null;
                var hold = root.Q<VisualElement>("holdReveal"); var start = hold.worldBound.center;
                Touch(hold, TouchPhase.Began, start); yield return null; yield return null;
                yield return new WaitForSecondsRealtime(0.15f); fixture.Capture("word-" + text.Replace(' ', '-') + "-large");
                var word = root.Q<Label>("secretWord");
                Assert.That(word.text, Is.EqualTo(text));
                Assert.That(word.resolvedStyle.fontSize, Is.GreaterThanOrEqualTo(22));
                foreach (var token in text.Split(' '))
                    Assert.That(word.MeasureTextSize(token, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined).x, Is.LessThanOrEqualTo(word.contentRect.width + 0.5f), token + " fits whole inside the real private viewport.");
                AssertTextFits(word);
                Touch(hold, TouchPhase.Moved, new Vector2(start.x, 48)); yield return null; yield return null;
                var leader = root.Q<Label>("secretLeader"); var viewport = root.Q<ScrollView>("secretInformation").contentViewport;
                Assert.That(leader.worldBound.yMax, Is.LessThanOrEqualTo(viewport.worldBound.yMax + 1));
                Touch(hold, TouchPhase.Ended, new Vector2(start.x, 48));
                Assert.That(PrivateLabels(root).All(label => label.text.Length == 0), Is.True);
                fixture.Dispose(); yield return null;
            }
        }
        [UnityTest] public IEnumerator EveryKingsCardKeepsNeutralOrdinaryPresentationAndClearsAllFieldsOnInterruption()
        {
            foreach (var language in new[] { Language.German, Language.English })
            {
                Color? ordinaryColor = null; string ordinaryGuidance = null;
                for (int seat = 0; seat < 4; seat++)
                {
                    var fixture = new Fixture(language); live.Add(fixture);
                    var names = new[] { "Zoë", "👩🏽‍🚀", "Éléonore", "Alexandra Maximiliane", "Alexandra Maximiliane" };
                    for (int i = 0; i < names.Length; i++) Assert.That(fixture.Session.RenamePlayer(fixture.Session.View.Players[i].Id, names[i]).Success, Is.True);
                    fixture.Session.SetMode(GameMode.Kings); fixture.Session.StartMatch();
                    for (int i = 0; i < seat; i++) { var owner = fixture.Session.Match.Owner.Id; fixture.Session.RevealCard(owner); fixture.Session.HideWord(); fixture.Session.AdvanceHandoff(owner); }
                    var expected = fixture.Session.RevealCard(fixture.Session.Match.Owner.Id); fixture.Session.HideWord();
                    fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null; yield return null;
                    var root = fixture.Root; Submit(root.Q<Button>("resumeMatch")); yield return null; yield return null;
                    root.AddToClassList("large-type"); yield return null; Enlarge(root); yield return null; yield return null;
                    var hold = root.Q<VisualElement>("holdReveal"); var start = hold.worldBound.center;
                    Touch(hold, TouchPhase.Began, start); yield return null; yield return null;
                    var fields = PrivateLabels(root);
                    if (expected.Kind == PrivateCardKind.Word)
                    {
                        Assert.That(fields[0].text, Is.EqualTo(expected.Word));
                        Assert.That(fields[1].text + fields[3].text, Is.Empty, "Ordinary cards contain no side, role or teammate list.");
                        Assert.That(fields[2].text, Does.Contain(expected.Leader.DisplayName));
                        var color = root.Q<VisualElement>("cardFace").resolvedStyle.backgroundColor;
                        var guidance = root.Q<Label>("holdLabel").text;
                        if (ordinaryColor.HasValue) { Assert.That(color, Is.EqualTo(ordinaryColor.Value)); Assert.That(guidance, Is.EqualTo(ordinaryGuidance)); }
                        ordinaryColor = color; ordinaryGuidance = guidance;
                        yield return new WaitForSecondsRealtime(0.15f); fixture.Capture("ordinary-" + language + "-" + (seat == 0 ? "undercover" : "civilian"));
                    }
                    var publicText = string.Join("\n", AllSemantics(fixture));
                    foreach (var secret in fields.SelectMany(label => label.text.Split('\n')).Where(text => text.Length > 0))
                        Assert.That(publicText, Does.Not.Contain(secret), "No nested public accessibility node contains a card-only word, role, leader association or list.");
                    Touch(hold, TouchPhase.Canceled, start); AssertCleared(fields);
                    Touch(hold, TouchPhase.Began, start);
                    using (var e = PointerCaptureOutEvent.GetPooled(hold, root, 1)) { e.target = hold; hold.SendEvent(e); }
                    AssertCleared(fields); Touch(hold, TouchPhase.Ended, start);
                    Touch(hold, TouchPhase.Began, start); Touch(root.Q<VisualElement>("safeRoot"), TouchPhase.Began, new Vector2(30, 100), 1);
                    AssertCleared(fields); Touch(hold, TouchPhase.Moved, new Vector2(start.x, 80)); AssertCleared(fields);
                    Touch(hold, TouchPhase.Ended, start); Touch(root.Q<VisualElement>("safeRoot"), TouchPhase.Ended, new Vector2(30, 100), 1);
                    Touch(hold, TouchPhase.Began, start); fixture.Host.SendMessage("OnApplicationFocus", false); AssertCleared(fields);
                    fixture.Host.SendMessage("OnApplicationFocus", true); yield return null;
                    Submit(root.Q<Button>("resumeMatch")); yield return null; yield return null;
                    hold = root.Q<VisualElement>("holdReveal"); start = hold.worldBound.center; fields = PrivateLabels(root);
                    AssertCleared(fields); Touch(hold, TouchPhase.Began, start);
                    fixture.Host.SendMessage("OnApplicationPause", true); AssertCleared(fields);
                    fixture.Host.SendMessage("OnApplicationPause", false); yield return null;
                    Submit(root.Q<Button>("resumeMatch")); yield return null; yield return null;
                    hold = root.Q<VisualElement>("holdReveal"); start = hold.worldBound.center;
                    Touch(hold, TouchPhase.Began, start); Touch(hold, TouchPhase.Ended, start); AssertCleared(PrivateLabels(root));
                    yield return new WaitForSecondsRealtime(0.2f);
                    var completedOwner = fixture.Session.Match.Owner.Id; Submit(root.Q<Button>("nextOwner")); yield return null;
                    Assert.That(fixture.Session.Match.Owner.Id, Is.Not.EqualTo(completedOwner));
                    Assert.That(fixture.Session.RevealCard(completedOwner), Is.Null, "Completing the handoff cannot create a later review path.");
                    AssertCleared(PrivateLabels(root));
                    fixture.Dispose(); yield return null;
                }
            }
        }
        static void AssertCleared(IEnumerable<Label> labels) => Assert.That(labels.All(label => label.text.Length == 0), Is.True, "Every private field is erased synchronously, including detached labels.");
        static Label[] PrivateLabels(VisualElement root) => new[] { "secretWord", "secretRole", "secretLeader", "secretKnown" }.Select(name => root.Q<Label>(name)).ToArray();
        static void Touch(VisualElement target, TouchPhase phase, Vector2 position, int finger = 0)
        {
            var touch = new Touch { fingerId = finger, position = position, phase = phase };
            if (phase == TouchPhase.Began) { using (var e = PointerDownEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else if (phase == TouchPhase.Moved) { using (var e = PointerMoveEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else if (phase == TouchPhase.Canceled) { using (var e = PointerCancelEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else { using (var e = PointerUpEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
        }
        static IEnumerable<string> AllSemantics(Fixture fixture) => AllLabels(fixture.Host.GetComponent<GroupScreen>().Accessibility.rootNodes);
        static IEnumerable<string> AllLabels(IEnumerable<UnityEngine.Accessibility.AccessibilityNode> nodes)
        {
            foreach (var node in nodes)
            {
                yield return node.label + " " + node.value;
                foreach (var label in AllLabels(node.children)) yield return label;
            }
        }
        static void Submit(VisualElement target)
        {
            Assert.That(target, Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = target; target.SendEvent(e); }
        }
        static void Enlarge(VisualElement root)
        {
            root.AddToClassList("large-type");
            foreach (var label in root.Query<TextElement>().ToList())
                if (label.name != "secretWord" && label.name != "markQuestion" && label.GetFirstAncestorOfType<TextField>() == null)
                    label.style.fontSize = label.resolvedStyle.fontSize * 1.5f;
            root.Q<VisualElement>("safeRoot").style.paddingTop = 40;
            root.Q<VisualElement>("safeRoot").style.paddingBottom = 24;
        }
        static void AssertTextFits(Label label)
        {
            Assert.That(label.worldBound.xMin, Is.GreaterThanOrEqualTo(0));
            Assert.That(label.worldBound.xMax, Is.LessThanOrEqualTo(360));
            Assert.That(label.contentRect.height, Is.GreaterThanOrEqualTo(label.MeasureTextSize(label.text, label.contentRect.width, VisualElement.MeasureMode.AtMost, 0, VisualElement.MeasureMode.Undefined).y - 1), label.name + " displays every line.");
        }
        sealed class Fixture : IDisposable
        {
            public readonly string DirectoryPath = Path.Combine(Application.temporaryCachePath, "kings-rules-ui-" + Guid.NewGuid().ToString("N"));
            public readonly Session Session;
            public readonly GameObject Host;
            readonly PanelSettings panel;
            readonly RenderTexture texture;
            bool disposed;
            public VisualElement Root => Host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture(Language language, int count = 5, Func<int, int> random = null)
            {
                Session = Session.Open(DirectoryPath, language, random ?? (_ => 0));
                for (int i = 0; i < count; i++) Assert.That(Session.AddPlayer("Person " + (i + 1)).Success, Is.True);
                Host = new GameObject("Kings rules and private card test"); Host.SetActive(false);
                panel = UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
                panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1;
                texture = new RenderTexture(360, 640, 24); texture.Create(); panel.targetTexture = texture;
                Host.AddComponent<UIDocument>().panelSettings = panel;
                Host.AddComponent<GroupScreen>().Initialize(Session); Host.SetActive(true);
                Root.style.width = 360; Root.style.height = 640;
            }
            public void Capture(string name)
            {
                var destination = Path.GetFullPath(Path.Combine(Application.dataPath, "../../artifacts/kings-rules-cards/screenshots"));
                Directory.CreateDirectory(destination);
                var previous = RenderTexture.active; RenderTexture.active = texture;
                var pixels = new Texture2D(360, 640, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0, 0, 360, 640), 0, 0); pixels.Apply();
                File.WriteAllBytes(Path.Combine(destination, name + ".png"), pixels.EncodeToPNG());
                RenderTexture.active = previous; UnityEngine.Object.Destroy(pixels);
            }
            public void Note(string name, string text)
            {
                var destination = Path.GetFullPath(Path.Combine(Application.dataPath, "../../artifacts/kings-rules-cards"));
                Directory.CreateDirectory(destination); File.WriteAllText(Path.Combine(destination, name + ".txt"), text);
            }
            public void Dispose() { if (disposed) return; disposed = true; UnityEngine.Object.Destroy(Host); UnityEngine.Object.Destroy(panel); UnityEngine.Object.Destroy(texture); if (Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath, true); }
        }
    }
}
