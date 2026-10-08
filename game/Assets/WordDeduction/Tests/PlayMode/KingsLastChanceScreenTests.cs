using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEditor;
using WordDeduction.UI;
using static WordDeduction.Tests.ScreenTestActions;

namespace WordDeduction.Tests
{
    public class KingsLastChanceScreenTests
    {
        readonly List<Fixture> live = new List<Fixture>();
        [TearDown] public void Cleanup() { foreach (var fixture in live) fixture.Dispose(); live.Clear(); }
        [UnityTest] public IEnumerator RapidRepeatedLastChanceTapsCannotJudgeOrSkipTheResult()
        {
            foreach (bool word in new[] { true, false })
            {
                var fixture = new Fixture(Language.English); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                string matchId = fixture.Session.Match.Id;
                Submit(fixture.Root.Q<Button>(word ? "chooseLastChanceWord" : "chooseLastChanceKing")); yield return null;
                if (!word) Submit(fixture.Root.Q<Button>("kingTarget-" + fixture.Session.Match.Participants[2].Id));
                yield return null; yield return null;
                var position = fixture.Root.Q<Button>(word ? "confirmLastChanceAnswer" : "confirmLastChanceKing").worldBound.center;
                ScreenTestActions.TapAt(fixture.Root,position);
                Assert.That(fixture.Session.Match.Phase, Is.EqualTo(word ? MatchPhase.KingsWordJudgment : MatchPhase.Result));
                yield return new WaitForSecondsRealtime(0.075f);
                Assert.That(fixture.Root.Q<Button>(word ? "lastChanceIncorrect" : "rematch").worldBound.Contains(position), Is.True, "The new irreversible action occupies the old tap location.");
                ScreenTestActions.TapAt(fixture.Root,position);
                Assert.That(fixture.Session.Match.Id, Is.EqualTo(matchId));
                Assert.That(fixture.Session.Match.Phase, Is.EqualTo(word ? MatchPhase.KingsWordJudgment : MatchPhase.Result), "A second tap cannot judge an answer or start a new match.");
                if (word)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    ScreenTestActions.TapAt(fixture.Root,position);
                    Assert.That(fixture.Session.Match.Phase, Is.EqualTo(MatchPhase.Result), "A later deliberate judgment still works.");
                    yield return new WaitForSecondsRealtime(0.075f);
                    ScreenTestActions.TapAt(fixture.Root,position);
                    Assert.That(fixture.Session.Match?.Id, Is.EqualTo(matchId), "Repeating the judgment must not dismiss the result to the Group.");
                }
                yield return fixture.Capture("repeat-protected-" + (word ? "word" : "king"));
            }
        }
        [UnityTest] public IEnumerator WordChoiceAndSpokenAnswerCommitBeforeJudgmentInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            foreach (bool correct in new[] { true, false })
            {
                var fixture = new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Assert.That(fixture.Root.Q<Button>("chooseLastChanceWord"), Is.Not.Null, "White can bind the word branch.");
                Assert.That(fixture.Root.Q<Button>("chooseLastChanceKing"), Is.Not.Null);
                yield return fixture.Capture("choice-" + language + "-" + correct);
                Submit(fixture.Root.Q<Button>("chooseLastChanceWord")); yield return null;
                Assert.That(fixture.Root.Q<Button>("chooseLastChanceKing"), Is.Null, "The committed branch cannot be changed.");
                Assert.That(fixture.Root.Q<Label>("lastChanceWord"), Is.Null, "Target is hidden until the answer has been spoken.");
                Assert.That(fixture.AccessibleText(), Does.Not.Contain(fixture.GoodWord).And.Not.Contain(fixture.EvilWord).And.Not.Contain("Chris"), "Accessibility has no early target or good King.");
                fixture.Reopen(); yield return null; Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Submit(fixture.Root.Q<Button>("matchBack")); yield return null; Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Assert.That(fixture.Root.Q<Button>("chooseLastChanceKing"), Is.Null);
                using (var locked = fixture.BlockWrite())
                {
                    Submit(fixture.Root.Q<Button>("confirmLastChanceAnswer")); yield return null;
                    Assert.That(fixture.Root.Q<Label>("lastChanceWord"), Is.Null, "Failed answer save reveals no target.");
                    Assert.That(fixture.Root.Q<Button>("confirmLastChanceAnswer"), Is.Not.Null);
                }
                yield return fixture.Capture("spoken-answer-" + language + "-" + correct);
                Submit(fixture.Root.Q<Button>("confirmLastChanceAnswer")); yield return null;
                Assert.That(fixture.Root.Q<Label>("lastChanceWord").text, Is.EqualTo(fixture.GoodWord));
                Assert.That(fixture.Root.Q<Button>("lastChanceCorrect").text, Is.EqualTo(language == Language.German ? "Richtig" : "Correct"), "Kings judgment must not promise the Classic White solo win.");
                Assert.That(fixture.Root.Q<Label>("resultTitle"), Is.Null);
                Assert.That(fixture.PublicText(), Does.Not.Contain(fixture.EvilWord).And.Not.Contain("Chris"), "No evil word or good King disclosure during judgment.");
                yield return null;
                Assert.That(fixture.AccessibleText(), Does.Contain(fixture.GoodWord).And.Not.Contain(fixture.EvilWord).And.Not.Contain("Chris"), "Judgment exposes exactly the permitted word in public accessibility.");
                fixture.Reopen(); yield return null; Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Assert.That(fixture.Root.Q<Label>("lastChanceWord").text, Is.EqualTo(fixture.GoodWord));
                yield return fixture.Capture("judgment-" + language + "-" + correct);
                Submit(fixture.Root.Q<Button>(correct ? "lastChanceCorrect" : "lastChanceIncorrect")); yield return null;
                Assert.That(fixture.Root.Q<Label>("resultTitle").text, Is.EqualTo(Copy.Get(language, correct ? "kingsEvilWin" : "kingsGoodWin")));
                Assert.That(fixture.Root.Q<Label>("resultWordCivilian").text, Is.EqualTo(fixture.GoodWord));
                Assert.That(fixture.Root.Q<Label>("resultWordUndercover").text, Is.EqualTo(fixture.EvilWord));
                yield return fixture.Capture("word-result-" + language + "-" + correct);
            }
        }
        [UnityTest] public IEnumerator KingChoiceHasCorrectableUnmarkedTargetsAndOneTeamResultInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            foreach (bool correct in new[] { true, false })
            {
                var fixture = new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Submit(fixture.Root.Q<Button>("chooseLastChanceKing")); yield return null;
                var people = fixture.Session.Match.Participants;
                Assert.That(fixture.Root.Q<Button>("kingTarget-" + people[2].Id), Is.Not.Null, "Committed King branch offers living unmarked targets.");
                Assert.That(fixture.Root.Q<Button>("kingTarget-" + people[1].Id), Is.Null, "Eliminated White cannot be targeted.");
                Assert.That(fixture.Root.Q<Button>("chooseLastChanceWord"), Is.Null);
                foreach (var person in fixture.Session.Match.Survivors)
                    Assert.That(fixture.Root.Q<Button>("kingTarget-" + person.Id).text, Is.EqualTo(person.DisplayName), "No side or King marks in choices.");
                yield return fixture.Capture("king-targets-" + language + "-" + correct);
                Submit(fixture.Root.Q<Button>("kingTarget-" + people[3].Id)); yield return null;
                Assert.That(fixture.Root.Q<Label>("kingTargetName").text, Is.EqualTo("Dana"));
                fixture.Reopen(); yield return null; Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Assert.That(fixture.Root.Q<Label>("kingTargetName").text, Is.EqualTo("Dana"));
                Submit(fixture.Root.Q<Button>("matchBack")); yield return null;
                Assert.That(fixture.Root.Q<Button>("kingTarget-" + people[2].Id), Is.Not.Null, "Back corrects only the pending target, never the chosen branch.");
                string target = people[correct ? 2 : 3].Id;
                Submit(fixture.Root.Q<Button>("kingTarget-" + target)); yield return null;
                using (var locked = fixture.BlockWrite())
                {
                    Submit(fixture.Root.Q<Button>("confirmLastChanceKing")); yield return null;
                    Assert.That(fixture.Root.Q<Label>("resultTitle"), Is.Null, "Failed target commit discloses no outcome.");
                    Assert.That(fixture.Root.Q<Button>("confirmLastChanceKing"), Is.Not.Null);
                }
                yield return fixture.Capture("king-confirm-" + language + "-" + correct);
                Submit(fixture.Root.Q<Button>("confirmLastChanceKing")); yield return null;
                Assert.That(fixture.Root.Q<Label>("resultTitle").text, Is.EqualTo(Copy.Get(language, correct ? "kingsEvilWin" : "kingsGoodWin")));
                Assert.That(fixture.Root.Q<Label>("role-" + people[2].Id).text, Is.EqualTo(Copy.Get(language,"kingsFinalGoodKing")));
                fixture.Reopen(); yield return null;
                Assert.That(fixture.Root.Q<Label>("resultTitle"), Is.Not.Null, "Terminal result survives reopening.");
                yield return fixture.Capture("king-result-" + language + "-" + correct);
                Submit(fixture.Root.Q<Button>("rematch")); yield return null;
                Assert.That(fixture.Session.Match.Survivors.Count, Is.EqualTo(5));
            }
        }
        [UnityTest] public IEnumerator LastChanceScreensKeepLongNamesAndActionsReachableAtLargeText()
        {
            foreach (var language in new[] { Language.English, Language.German })
            foreach (var choice in new[] { LastChanceChoice.Word, LastChanceChoice.King })
            {
                var fixture = new Fixture(language, true); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null; yield return Enlarge(fixture.Root);
                AssertActions(fixture.Root); yield return fixture.Capture("large-choice-" + language + "-" + choice);
                Submit(fixture.Root.Q<Button>(choice == LastChanceChoice.Word ? "chooseLastChanceWord" : "chooseLastChanceKing")); yield return null; yield return Enlarge(fixture.Root);
                if (choice == LastChanceChoice.Word)
                {
                    AssertActions(fixture.Root); yield return fixture.Capture("large-answer-" + language);
                    Submit(fixture.Root.Q<Button>("confirmLastChanceAnswer")); yield return null; yield return Enlarge(fixture.Root);
                    AssertActions(fixture.Root); yield return fixture.Capture("large-judgment-" + language);
                    Submit(fixture.Root.Q<Button>("lastChanceCorrect")); yield return null; yield return Enlarge(fixture.Root);
                }
                else
                {
                    var list = fixture.Root.Q<ScrollView>("kingTargets");
                    Assert.That(list.contentViewport.worldBound.height, Is.GreaterThan(48), "A usable target viewport remains at large text.");
                    var target = fixture.Session.Match.Survivors.Last(); var button = fixture.Root.Q<Button>("kingTarget-" + target.Id);
                    list.ScrollTo(button); yield return null; yield return null;
                    Assert.That(button.worldBound.yMax, Is.LessThanOrEqualTo(list.contentViewport.worldBound.yMax + 1), "Last long duplicate name is reachable.");
                    yield return fixture.Capture("large-last-target-" + language);
                    Submit(button); yield return null; yield return Enlarge(fixture.Root);
                    AssertActions(fixture.Root); yield return fixture.Capture("large-king-confirm-" + language);
                    Submit(fixture.Root.Q<Button>("confirmLastChanceKing")); yield return null; yield return Enlarge(fixture.Root);
                }
                AssertActions(fixture.Root); yield return fixture.Capture("large-result-" + language + "-" + choice);
            }
        }
        static IEnumerator Enlarge(VisualElement root)
        {
            root.AddToClassList("large-type"); yield return null;
            foreach (var element in root.Query<TextElement>().ToList())
                if (element.GetFirstAncestorOfType<TextField>() == null && !element.ClassListContains("test-enlarged"))
                { element.style.fontSize = element.resolvedStyle.fontSize * 1.5f; element.AddToClassList("test-enlarged"); }
            root.Q<VisualElement>("safeRoot").style.paddingTop = 40;
            root.Q<VisualElement>("safeRoot").style.paddingBottom = 24;
            yield return null; yield return null;
        }
        static void AssertActions(VisualElement root)
        {
            foreach (var button in root.Q<VisualElement>("matchActions").Query<Button>().ToList())
            {
                Assert.That(button.worldBound.height, Is.GreaterThanOrEqualTo(48), button.name + " touch height");
                Assert.That(button.worldBound.yMax, Is.LessThanOrEqualTo(616), button.name + " inside bottom inset");
                Assert.That(button.worldBound.xMin, Is.GreaterThanOrEqualTo(0)); Assert.That(button.worldBound.xMax, Is.LessThanOrEqualTo(360));
            }
        }
        sealed class Fixture : IDisposable
        {
            public readonly string DirectoryPath = Path.Combine(Application.temporaryCachePath, "kings-last-chance-ui-" + Guid.NewGuid().ToString("N"));
            public Session Session;
            public readonly GameObject Host;
            readonly PanelSettings panel;
            readonly RenderTexture texture;
            public string GoodWord, EvilWord;
            public VisualElement Root => Host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture(Language language, bool crowded = false)
            {
                Session = Session.Open(DirectoryPath, language, _ => 0);
                foreach (var name in crowded ? Enumerable.Repeat(new string('M',24),20) : new[] { "Alex", "Bea", "Chris", "Dana", "Eli" }) Session.AddPlayer(name);
                Session.SetMode(GameMode.Kings); Session.StartMatch();
                string white = null;
                while (Session.Match.Phase == MatchPhase.Handoff)
                {
                    string id = Session.Match.Owner.Id; var card = Session.RevealCard(id);
                    if (card.Kind == PrivateCardKind.GoodKing) GoodWord = card.Word;
                    if (card.Owner.Id == Session.Match.Participants[0].Id) EvilWord = card.Word;
                    if (card.Kind == PrivateCardKind.EvilKing) white = card.Owner.Id;
                    Session.HideWord(); Session.AdvanceHandoff(id);
                }
                var match = Session.Match;
                Session.SelectElimination(match.Id, 0, white); Session.ConfirmElimination(match.Id, 0, white);
                Host = new GameObject("Kings last chance interaction test"); Host.SetActive(false);
                panel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
                texture = new RenderTexture(360, 640, 0); texture.Create(); panel.targetTexture = texture;
                panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1;
                Host.AddComponent<UIDocument>().panelSettings = panel;
                Host.AddComponent<GroupScreen>().Initialize(Session); Host.SetActive(true);
                Root.style.width = 360; Root.style.height = 640;
            }
            public FileStream BlockWrite() => new FileStream(Path.Combine(DirectoryPath, "session.pending.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            public string PublicText() => string.Join("\n", Root.Q<VisualElement>("matchScreen").Query<Label>().ToList().Select(l => l.text));
            public string AccessibleText() => string.Join("\n", AllLabels(Host.GetComponent<GroupScreen>().Accessibility.rootNodes));
            public void Reopen() { Session = Session.Open(DirectoryPath, Language.German, _ => 0); Host.GetComponent<GroupScreen>().Initialize(Session); }
            public IEnumerator Capture(string name)
            {
                yield return null; yield return null; yield return new WaitForSeconds(0.15f);
                string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../artifacts/kings-last-chance/screenshots")); Directory.CreateDirectory(output);
                SaveScreenshot(texture,Path.Combine(output,name + ".png"));
                if (name.StartsWith("large-",StringComparison.Ordinal))
                {
                    var scroll = Root.Q<VisualElement>("matchBody").Q<ScrollView>();
                    var last = scroll.contentContainer.Query<Label>().ToList().LastOrDefault();
                    if (last != null)
                    {
                        scroll.ScrollTo(last); yield return null; yield return new WaitForSeconds(0.15f);
                        Assert.That(last.worldBound.yMax, Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax + 1), "Final explanatory line is reachable at150%.");
                        SaveScreenshot(texture,Path.Combine(output,name + "-last.png"));
                    }
                }
            }
            public void Dispose() { UnityEngine.Object.Destroy(Host); UnityEngine.Object.Destroy(panel); UnityEngine.Object.Destroy(texture); if (Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath, true); }
        }
    }
}
