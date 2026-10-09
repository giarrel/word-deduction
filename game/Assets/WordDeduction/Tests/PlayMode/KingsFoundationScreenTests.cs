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
using static WordDeduction.Tests.ScreenTestActions;

namespace WordDeduction.Tests
{
    public class KingsFoundationScreenTests
    {
        readonly List<Fixture> live = new List<Fixture>();
        [TearDown] public void Cleanup() { foreach (var fixture in live) fixture.Dispose(); live.Clear(); }
        [UnityTest] public IEnumerator KingsSelectionRetainsRequestedCountAndExplainsAdjustmentInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language, 9); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("kingsMode")); yield return null;
                Assert.That(fixture.Session.View.Mode, Is.EqualTo(GameMode.Kings));
                Assert.That(fixture.Root.Q<VisualElement>("whiteCountRow").ClassListContains("hidden"), Is.True);
                Submit(fixture.Root.Q<Button>("moreUndercover"));
                Assert.That(fixture.Session.View.KingsUndercoverPreference, Is.EqualTo(3));
                foreach (var id in fixture.Session.View.Players.Skip(5).Select(p => p.Id).ToArray())
                {
                    Submit(fixture.Root.Q<Button>("edit-" + id));
                    Submit(fixture.Root.Q<Button>("removePlayer"));
                }
                yield return null;
                Assert.That(fixture.Root.Q<Label>("kingsCountAdjustment").text, Does.Contain("3"));
                Assert.That(fixture.Root.Q<Label>("kingsCountAdjustment").ClassListContains("hidden"), Is.False);
                Assert.That(fixture.Session.View.UndercoverCount, Is.EqualTo(1));
                Assert.That(fixture.Root.Q<Button>("moreUndercover").enabledSelf, Is.False);
                Submit(fixture.Root.Q<Button>("playButton")); yield return null;
                Assert.That(fixture.Root.Q<Label>("matchMode").text, Is.EqualTo(language == Language.German ? "Könige" : "Kings"));
                Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Empty);
            }
        }
        [UnityTest] public IEnumerator PrivateCardsConcealAllKnowledgeAndFinishDirectlyAtTheTableInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("kingsMode")); Submit(fixture.Root.Q<Button>("playButton")); yield return null;
                for (int seat = 0; seat < 5; seat++)
                {
                    var hold = fixture.Root.Q<VisualElement>("holdReveal"); Touch(hold, true);
                    var role = fixture.Root.Q<Label>("secretRole"); var leader = fixture.Root.Q<Label>("secretLeader"); var known = fixture.Root.Q<Label>("secretKnown");
                    Assert.That(leader, Is.Not.Null, "The private card contains the owner's permitted leader information.");
                    if (seat == 0 || seat > 2)
                    {
                        Assert.That(role.text, Is.Empty, "Ordinary cards reveal no side or role.");
                        Assert.That(leader.text, Does.Contain(seat == 0 ? "Bea" : "Chris"));
                        Assert.That(known.text, Is.Empty);
                        Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Not.Empty);
                    }
                    else if (seat == 1)
                    {
                        Assert.That(role.text, Does.Contain("Mr. White"));
                        Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Empty, "White has neither word.");
                        Assert.That(known.text, Does.Contain("Alex").And.Not.Contain("Chris"));
                    }
                    else
                    {
                        Assert.That(role.text, Does.Contain(language == Language.German ? "gute König" : "good King"));
                        Assert.That(known.text, Does.Contain("Alex").And.Contain("Bea").And.Not.Contain("Mr. White"));
                    }
                    Touch(hold, false);
                    Assert.That(role.text + leader.text + known.text + fixture.Root.Q<Label>("secretWord").text, Is.Empty, "Release clears all secret content synchronously.");
                    yield return new WaitForSecondsRealtime(0.2f); Submit(fixture.Root.Q<Button>("nextOwner")); yield return null;
                }
                Assert.That(fixture.Session.Match.Phase, Is.EqualTo(MatchPhase.TablePlay));
                Assert.That(fixture.Root.Q<Label>("kingsTableTitle"), Is.Not.Null);
                Assert.That(fixture.Root.Q<Button>("beginVote"), Is.Null);
                Assert.That(fixture.Root.Q<Button>("recordTie"), Is.Null);
                Assert.That(fixture.Root.Q<VisualElement>("holdReveal"), Is.Null, "There is no card review after handoff completion.");
            }
        }
        static void Touch(VisualElement element, bool down)
        {
            var touch = new Touch { fingerId = 0, position = new Vector2(120, 300), phase = down ? TouchPhase.Began : TouchPhase.Ended };
            if (down) { using (var e = PointerDownEvent.GetPooled(touch)) { e.target = element; element.SendEvent(e); } }
            else { using (var e = PointerUpEvent.GetPooled(touch)) { e.target = element; element.SendEvent(e); } }
        }
        [UnityTest] public IEnumerator KingKnowledgeStaysOutsideEveryAccessibilityNodeAndClearsOnInterruption()
        {
            var fixture = new Fixture(Language.English); live.Add(fixture);
            fixture.Session.SetMode(GameMode.Kings); fixture.Session.StartMatch();
            for (int i = 0; i < 2; i++) { var id = fixture.Session.Match.Owner.Id; fixture.Session.RevealCard(id); fixture.Session.HideWord(); fixture.Session.AdvanceHandoff(id); }
            fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null; yield return null;
            Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null; yield return null;
            Touch(fixture.Root.Q<VisualElement>("holdReveal"), true); yield return null;
            var fields = new[] { "secretWord", "secretRole", "secretLeader", "secretKnown" }.Select(name => fixture.Root.Q<Label>(name)).ToArray();
            Assert.That(fields[3].text, Does.Contain("Alex").And.Contain("Bea"));
            string semantics = string.Join("\n", AllLabels(fixture.Host.GetComponent<GroupScreen>().Accessibility.rootNodes));
            Assert.That(semantics, Does.Not.Contain("Alex").And.Not.Contain("Bea").And.Not.Contain("good King").And.Not.Contain(fields[0].text), "Nested accessibility nodes must not expose card-only words, roles or name lists.");
            fixture.Host.SendMessage("OnApplicationPause", true);
            Assert.That(fields.All(label => label.text.Length == 0), Is.True, "Pause clears every old detached private label synchronously.");
            fixture.Host.SendMessage("OnApplicationPause", false);
            fixture.Session = Session.Open(fixture.DirectoryPath, Language.German, _ => 0);
            fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null;
            Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
            Assert.That(fixture.Root.Q<Label>("cardOwner").text, Is.EqualTo("Chris"));
            Assert.That(fixture.Root.Q<Label>("secretKnown").text, Is.Empty);
            Assert.That(fixture.Root.Q<Button>("nextOwner").enabledSelf, Is.False, "An interrupted unfinished handoff resumes covered and unread.");
            Submit(fixture.Root.Q<Button>("matchHelp")); yield return null;
            Assert.That(fixture.Root.Q<Label>("helpTitle").text, Is.EqualTo("Kings rules"), "Kings must never display Quick or Classic rules.");
        }
        sealed class Fixture : IDisposable
        {
            public readonly string DirectoryPath = Path.Combine(Application.temporaryCachePath, "kings-foundation-ui-" + Guid.NewGuid().ToString("N"));
            public Session Session;
            public readonly GameObject Host;
            readonly PanelSettings panel;
            bool disposed;
            public VisualElement Root => Host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture(Language language, int count = 5)
            {
                Session = Session.Open(DirectoryPath, language, maximum => 0);
                var names = new[] { "Alex", "Bea", "Chris", "Dana", "Eli" };
                for (int i = 0; i < count; i++) Session.AddPlayer(i < names.Length ? names[i] : "Person " + i);
                Host = new GameObject("Kings foundation interaction test"); Host.SetActive(false);
                panel = ScriptableObject.CreateInstance<PanelSettings>(); Host.AddComponent<UIDocument>().panelSettings = panel;
                Host.AddComponent<GroupScreen>().Initialize(Session); Host.SetActive(true);
                Root.style.width = 390; Root.style.height = 844;
            }
            public void Dispose() { if (disposed) return; disposed = true; UnityEngine.Object.Destroy(Host); UnityEngine.Object.Destroy(panel); if (Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath, true); }
        }
    }
}
