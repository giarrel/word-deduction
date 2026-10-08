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
    public class KingsEliminationScreenTests
    {
        readonly List<Fixture> live = new List<Fixture>();
        [TearDown] public void Cleanup() { foreach (var fixture in live) fixture.Dispose(); live.Clear(); }
        [UnityTest] public IEnumerator PublicGroupHierarchyReturnsAfterAbandonAndResultWithoutAnotherAction()
        {
            foreach (int count in new[] { 5, 20 })
            foreach (bool result in new[] { false, true })
            {
                var fixture = new Fixture(Language.English,count); live.Add(fixture); yield return null; yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null; yield return null;
                if (result)
                {
                    Submit(fixture.Root.Q<Button>("eliminate-" + fixture.GoodKingId)); yield return null;
                    Submit(fixture.Root.Q<Button>("confirmElimination")); yield return null; yield return null;
                }
                else
                {
                    Submit(fixture.Root.Q<Button>("matchBack")); yield return null;
                    Submit(fixture.Root.Q<Button>("abandonMatch")); yield return null; yield return null;
                }
                Assert.That(AllLabels(fixture.Host.GetComponent<GroupScreen>().Accessibility.rootNodes), Does.Not.Contain("Add player "));
                Submit(fixture.Root.Q<Button>(result ? "editGroup" : "confirmAbandon"));
                yield return null; yield return null; yield return null;
                Assert.That(fixture.Session.Match, Is.Null);
                Assert.That(fixture.Root.Q<VisualElement>("screen").resolvedStyle.display, Is.EqualTo(DisplayStyle.Flex));
                string labels = string.Join("\n",AllLabels(fixture.Host.GetComponent<GroupScreen>().Accessibility.rootNodes));
                Assert.That(labels, Does.Contain("Add player").And.Contain("Kings").And.Contain("Alex"), "A newly visible Group must restore its public controls and nested player list without another redraw action.");
                var groupNodes = fixture.Host.GetComponent<GroupScreen>().Accessibility.rootNodes;
                var players = groupNodes.First(n => n.role == UnityEngine.Accessibility.AccessibilityRole.ScrollView);
                foreach (var node in new[] { groupNodes.First(n => n.label == "Add player"), players.children.First(n => n.label.Contains("Alex")) })
                {
                    Assert.That(node.isActive, Is.True);
                    Assert.That(node.frame.width, Is.GreaterThan(0));
                    Assert.That(node.frame.height, Is.GreaterThan(0));
                }
                Submit(fixture.Root.Q<Button>("playButton")); yield return null; yield return null; yield return null;
                Assert.That(fixture.Root.Q<Label>("cardOwner"), Is.Not.Null);
                labels = string.Join("\n",AllLabels(fixture.Host.GetComponent<GroupScreen>().Accessibility.rootNodes));
                Assert.That(labels, Does.Contain("Back").And.Contain("Help").And.Contain("Alex").And.Not.Contain("Add player"), "The newly shown Match must expose its public handoff controls without reviving the hidden Group.");
                var back = fixture.Host.GetComponent<GroupScreen>().Accessibility.rootNodes.First(n => n.label == "Back");
                Assert.That(back.isActive, Is.True);
                Assert.That(back.frame.width, Is.GreaterThan(0));
                Assert.That(back.frame.height, Is.GreaterThan(0));
            }
        }
        [UnityTest] public IEnumerator RapidRepeatedConfirmationDoesNotStartARematch()
        {
            var fixture = new Fixture(Language.English); live.Add(fixture); yield return null;
            Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
            string matchId = fixture.Session.Match.Id;
            Submit(fixture.Root.Q<Button>("eliminate-" + fixture.Session.Match.Participants[2].Id));
            yield return null; yield return null;
            var position = fixture.Root.Q<Button>("confirmElimination").worldBound.center;
            TapAt(fixture.Root, position);
            Assert.That(fixture.Session.Match.Phase, Is.EqualTo(MatchPhase.Result), "The first real pointer tap confirms the good King's elimination.");
            yield return new WaitForSecondsRealtime(0.075f);
            Assert.That(fixture.Root.Q<Button>("rematch").worldBound.Contains(position), Is.True, "The new Rematch action occupies the repeated tap location.");
            TapAt(fixture.Root, position);
            Assert.That(fixture.Session.Match.Id, Is.EqualTo(matchId), "A rapid second tap must not activate the new screen's Rematch.");
            Assert.That(fixture.Root.Q<Label>("resultTitle"), Is.Not.Null, "The complete result remains available to read.");
            yield return new WaitForSecondsRealtime(0.5f);
            TapAt(fixture.Root, fixture.Root.Q<Button>("rematch").worldBound.center);
            Assert.That(fixture.Session.Match.Id, Is.Not.EqualTo(matchId), "A later deliberate single tap still starts a rematch.");
        }
        [UnityTest] public IEnumerator OrdinaryEliminationCanBeCorrectedAndContinuesDirectlyAtTheTableInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                var people = fixture.Session.Match.Participants;
                Submit(fixture.Root.Q<Button>("eliminate-" + people[0].Id)); yield return null;
                Assert.That(fixture.Root.Q<Label>("confirmName").text, Is.EqualTo("Alex"));
                Assert.That(fixture.Session.Match.Survivors.Count, Is.EqualTo(5));
                var oldConfirm = fixture.Root.Q<Button>("confirmElimination");
                Submit(fixture.Root.Q<Button>("changeElimination")); yield return null;
                Submit(fixture.Root.Q<Button>("eliminate-" + people[3].Id)); yield return null;
                Submit(oldConfirm); yield return null;
                Assert.That(fixture.Session.Match.Survivors.Count, Is.EqualTo(5), "A detached stale target cannot confirm a corrected selection.");
                Submit(fixture.Root.Q<Button>("confirmElimination")); yield return null;
                Assert.That(fixture.Root.Q<Label>("kingsEliminatedName").text, Is.EqualTo("Dana"));
                Assert.That(fixture.Root.Q<Label>("kingsEliminatedStatus").text, Is.EqualTo(language == Language.German ? "Kein König" : "Not a king"));
                Assert.That(fixture.Root.Q<Button>("eliminate-" + people[3].Id), Is.Null);
                Assert.That(fixture.Root.Q<Button>("eliminate-" + people[0].Id), Is.Not.Null, "The next elimination is directly available without any continue action.");
                Assert.That(fixture.Root.Q<Button>("continueRound"), Is.Null);
                Assert.That(fixture.Root.Q<Button>("beginVote"), Is.Null);
                Assert.That(fixture.Root.Q<Button>("recordTie"), Is.Null);
                var publicText = string.Join("\n", fixture.Root.Q<VisualElement>("matchScreen").Query<Label>().ToList().Select(l => l.text));
                Assert.That(publicText, Does.Not.Contain(fixture.GoodWord).And.Not.Contain(fixture.EvilWord).And.Not.Contain("Undercover").And.Not.Contain("Mr. White").And.Not.Contain("Civilian").And.Not.Contain("Bürger"));
                fixture.Reopen(); yield return null; Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Assert.That(fixture.Root.Q<Label>("kingsEliminatedStatus").text, Is.EqualTo(language == Language.German ? "Kein König" : "Not a king"));
                Submit(fixture.Root.Q<Button>("eliminate-" + people[0].Id)); yield return null;
                Submit(fixture.Root.Q<Button>("confirmElimination")); yield return null;
                Assert.That(fixture.Root.Q<Label>("kingsEliminatedStatus").text, Is.EqualTo(language == Language.German ? "Kein König" : "Not a king"), "Ordinary Undercover receives identical neutral disclosure.");
                yield return fixture.Capture("ordinary-" + language);
            }
        }
        [UnityTest] public IEnumerator GoodKingResultNamesTheWholeTeamAndBothKingsInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                var people = fixture.Session.Match.Participants;
                Submit(fixture.Root.Q<Button>("eliminate-" + people[0].Id)); yield return null;
                Submit(fixture.Root.Q<Button>("confirmElimination")); yield return null;
                Submit(fixture.Root.Q<Button>("eliminate-" + people[2].Id)); yield return null;
                Submit(fixture.Root.Q<Button>("confirmElimination")); yield return null;
                Assert.That(fixture.Root.Q<Label>("resultTitle").text, Is.EqualTo(language == Language.German ? "Das böse Team gewinnt." : "The evil team wins."));
                Assert.That(fixture.Root.Q<Label>("kingsTeamResult").text, Does.Contain(language == Language.German ? "Ausgeschiedene" : "Eliminated"));
                Assert.That(fixture.Root.Q<Label>("role-" + people[1].Id).text, Is.EqualTo(language == Language.German ? "Böser König · Mr. White" : "Evil King · Mr. White"));
                Assert.That(fixture.Root.Q<Label>("role-" + people[2].Id).text, Is.EqualTo(language == Language.German ? "Guter König" : "Good King"));
                Assert.That(fixture.Root.Q<Label>("role-" + people[0].Id).text, Is.EqualTo(language == Language.German ? "Böses Team · Undercover" : "Evil team · Undercover"));
                Assert.That(fixture.Root.Q<Label>("resultWordCivilian").text, Is.EqualTo(fixture.GoodWord));
                Assert.That(fixture.Root.Q<Label>("resultWordUndercover").text, Is.EqualTo(fixture.EvilWord));
                yield return fixture.Capture("result-" + language);
                var oldMatch = fixture.Session.Match.Id;
                Submit(fixture.Root.Q<Button>("rematch")); yield return null;
                Assert.That(fixture.Session.Match.Id, Is.Not.EqualTo(oldMatch));
                Assert.That(fixture.Session.Match.Survivors.Count, Is.EqualTo(5));
                Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Empty);
            }
        }
        [UnityTest] public IEnumerator WhiteEliminationKeepsLastChanceSafeAcrossRestartAndFailedSaveInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                string white = fixture.Session.Match.Participants[1].Id;
                Submit(fixture.Root.Q<Button>("eliminate-" + white)); yield return null;
                using (var locked = new FileStream(Path.Combine(fixture.DirectoryPath, "session.pending.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    Submit(fixture.Root.Q<Button>("confirmElimination")); yield return null;
                    Assert.That(fixture.Session.Match.Survivors.Count, Is.EqualTo(5));
                    Assert.That(fixture.Root.Q<Button>("confirmElimination"), Is.Not.Null, "A failed save leaves the correctable confirmation visible.");
                    Assert.That(fixture.Root.Q<Label>("kingsLastChanceTitle"), Is.Null);
                    Assert.That(fixture.Root.Q<Label>("resultTitle"), Is.Null);
                }
                Submit(fixture.Root.Q<Button>("confirmElimination")); yield return null;
                Assert.That(fixture.Root.Q<Label>("kingsLastChanceTitle"), Is.Not.Null, "Confirmed White enters the safe last-chance surface.");
                Assert.That(fixture.Root.Q<Label>("kingsLastChanceTitle").text, Is.EqualTo(language == Language.German ? "Letzte Chance." : "Last chance."));
                Assert.That(fixture.Root.Q<Label>("eliminatedName").text, Is.EqualTo("Bea"));
                Assert.That(fixture.Root.Q<Button>("whiteCorrect"), Is.Null, "No Classic solo-win shortcut.");
                Assert.That(fixture.Root.Q<Button>("whiteIncorrect"), Is.Null);
                fixture.Reopen(); yield return null; Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
                Assert.That(fixture.Root.Q<Label>("kingsLastChanceTitle"), Is.Not.Null);
                var publicText = string.Join("\n", fixture.Root.Q<VisualElement>("matchScreen").Query<Label>().ToList().Select(l => l.text));
                Assert.That(publicText, Does.Not.Contain(fixture.GoodWord).And.Not.Contain(fixture.EvilWord).And.Not.Contain("Chris"));
                Assert.That(fixture.Root.Q<Label>("resultTitle"), Is.Null);
                yield return fixture.Capture("last-chance-" + language);
            }
        }
        sealed class Fixture : IDisposable
        {
            public readonly string DirectoryPath = Path.Combine(Application.temporaryCachePath, "kings-elimination-ui-" + Guid.NewGuid().ToString("N"));
            public Session Session;
            public readonly GameObject Host;
            readonly PanelSettings panel;
            readonly RenderTexture texture;
            public string GoodWord, EvilWord, GoodKingId;
            public VisualElement Root => Host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture(Language language, int count = 5)
            {
                Session = Session.Open(DirectoryPath, language, _ => 0);
                foreach (var name in new[] { "Alex", "Bea", "Chris", "Dana", "Eli" }) Session.AddPlayer(name);
                for (int i = 5; i < count; i++) Session.AddPlayer(new string('W',24));
                Session.SetMode(GameMode.Kings); Session.StartMatch();
                while (Session.Match.Phase == MatchPhase.Handoff)
                {
                    string id = Session.Match.Owner.Id; var card = Session.RevealCard(id);
                    if (card.Kind == PrivateCardKind.GoodKing) { GoodWord = card.Word; GoodKingId = card.Owner.Id; }
                    if (card.Owner.DisplayName == "Alex") EvilWord = card.Word;
                    Session.HideWord(); Session.AdvanceHandoff(id);
                }
                Host = new GameObject("Kings elimination interaction test"); Host.SetActive(false);
                panel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
                texture = new RenderTexture(390, 844, 0); texture.Create(); panel.targetTexture = texture;
                panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1;
                Host.AddComponent<UIDocument>().panelSettings = panel;
                Host.AddComponent<GroupScreen>().Initialize(Session); Host.SetActive(true);
                Root.style.width = 390; Root.style.height = 844;
            }
            public void Reopen() { Session = Session.Open(DirectoryPath, Language.German, _ => 0); Host.GetComponent<GroupScreen>().Initialize(Session); }
            public IEnumerator Capture(string name)
            {
                yield return null; yield return null;
                string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../artifacts/kings-elimination")); Directory.CreateDirectory(output);
                SaveScreenshot(texture,Path.Combine(output,name + ".png"));
            }
            public void Dispose() { UnityEngine.Object.Destroy(Host); UnityEngine.Object.Destroy(panel); UnityEngine.Object.Destroy(texture); if (Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath, true); }
        }
    }
}
