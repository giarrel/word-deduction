using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Tests
{
    public class RecoveryScreenTests
    {
        [TearDown] public void Cleanup() { foreach (var fixture in Fixture.Live.ToArray()) fixture.Dispose(); }
        [UnityTest]
        public IEnumerator AdditionalContactsCannotRevealUntilEveryFingerHasLifted()
        {
            var fixture = new Fixture(Language.English); yield return null;
            Submit(fixture.Root.Q<Button>("playButton")); yield return null;
            var hold = fixture.Root.Q<VisualElement>("holdReveal");
            var word = fixture.Root.Q<Label>("secretWord");
            Down(hold, 1); Assert.That(word.text, Is.Not.Empty);
            var background = fixture.Root.Q<VisualElement>("safeRoot");
            Down(background, 2); Assert.That(word.text, Is.Empty);
            Up(background, 2); Down(hold, 2);
            Assert.That(word.text, Is.Empty, "A returning second finger stays covered while the first finger remains held.");
            Up(hold, 2); Up(hold, 1);
            Down(hold, 1); Assert.That(word.text, Is.Not.Empty, "A fresh single contact works after all fingers lift.");
            Up(hold, 1); Assert.That(word.text, Is.Empty);
            yield return new WaitForSecondsRealtime(0.2f);
            var next = fixture.Root.Q<Button>("nextOwner");
            Assert.That(next.enabledSelf, Is.True, "Next is enabled after the single-contact reveal ends.");
            Down(next, 1); yield return null;
            Assert.That(next.enabledSelf, Is.True, "Next retains its own valid press until release.");
            Up(next, 1); yield return null;
            Assert.That(fixture.Session.Match.HandoffNumber, Is.EqualTo(2), "Released single-contact Next advances exactly one owner.");
            Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Empty);
        }
        [UnityTest]
        public IEnumerator NavigationRetainsHeldContactsAndFreshTouchesWorkAfterReleaseOrInterruption()
        {
            var fixture = new Fixture(Language.English); yield return null;
            Submit(fixture.Root.Q<Button>("playButton")); yield return null;
            var hold = fixture.Root.Q<VisualElement>("holdReveal");
            Down(hold, 1); var oldWord = fixture.Root.Q<Label>("secretWord");
            Submit(fixture.Root.Q<Button>("matchBack"));
            Assert.That(oldWord.text, Is.Empty);
            Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
            hold = fixture.Root.Q<VisualElement>("holdReveal");
            Down(hold, 2); Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Empty, "Navigation must not forget a held first contact.");
            Up(hold, 2); Up(hold, 1);
            Down(hold, 1); Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Not.Empty); Up(hold, 1);
            yield return new WaitForSecondsRealtime(0.2f);
            var back = fixture.Root.Q<Button>("matchBack"); Down(back, 1); yield return null; Up(back, 1); yield return null;
            var resume = fixture.Root.Q<Button>("resumeMatch"); Assert.That(resume, Is.Not.Null);
            Down(resume, 1); yield return null; Up(resume, 1); yield return null;
            hold = fixture.Root.Q<VisualElement>("holdReveal"); Down(hold, 1);
            Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Not.Empty, "Fresh touch navigation leaves no stale captured contacts.");
            oldWord = fixture.Root.Q<Label>("secretWord"); fixture.Host.SendMessage("OnApplicationPause", true);
            Assert.That(oldWord.text, Is.Empty); fixture.Host.SendMessage("OnApplicationPause", false);
            Submit(fixture.Root.Q<Button>("resumeMatch")); yield return null;
            hold = fixture.Root.Q<VisualElement>("holdReveal"); Down(hold, 2);
            Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Not.Empty, "Window interruption cancels the old contact stream.");
            Up(hold, 2);
        }
        [UnityTest]
        public IEnumerator FailedFreshStartShowsTheWriteFailureAndAllowsExplicitRetry()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language, "damaged"); yield return null;
                Assert.That(fixture.Session.View.StorageBlocked, Is.True);
                Assert.That(fixture.Root.Q<Label>("notice").text, Does.Contain(language == Language.German ? "wiederhergestellt" : "recovered"));
                var pending = Path.Combine(fixture.DirectoryPath, "session.pending.json");
                Directory.CreateDirectory(pending);
                Submit(fixture.Root.Q<Button>("resetDamaged")); yield return null;
                Assert.That(fixture.Root.Q<Label>("notice").text, Does.Contain(language == Language.German ? "Speichern nicht möglich" : "Couldn't save"));
                Assert.That(fixture.Session.View.StorageBlocked, Is.True);
                Assert.That(fixture.Root.Q<TextField>("nameInput").enabledSelf, Is.False);
                Assert.That(File.ReadAllText(Path.Combine(fixture.DirectoryPath, "session.json")), Is.EqualTo("damaged primary"));
                Directory.Delete(pending);
                Submit(fixture.Root.Q<Button>("resetDamaged")); yield return null;
                Assert.That(fixture.Session.View.StorageBlocked, Is.False);
                Assert.That(fixture.Root.Q<TextField>("nameInput").enabledSelf, Is.True);
                Assert.That(fixture.Root.Q<Label>("notice").text, Is.Empty);
                Assert.That(Directory.GetDirectories(fixture.DirectoryPath, "damaged-*").Length, Is.GreaterThanOrEqualTo(1));
                fixture.Dispose(); yield return null;
            }
        }
        [UnityTest]
        public IEnumerator RecoveryExplainsRestoredBlockedAndInaccessibleFilesInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            foreach (var fault in new[] { "backup", "newer", "unreadable" })
            {
                var fixture = new Fixture(language, fault); yield return null;
                bool restored = fault == "backup";
                Assert.That(fixture.Session.View.StorageBlocked, Is.EqualTo(!restored));
                Assert.That(fixture.Session.View.Players.Count, Is.EqualTo(restored ? 2 : 0));
                Assert.That(fixture.Root.Q<Button>("resetDamaged").ClassListContains("hidden"), Is.True, "Neither a readable backup nor unsupported/unreadable files invite a reset.");
                Assert.That(fixture.Root.Q<TextField>("nameInput").enabledSelf, Is.EqualTo(restored));
                var message = fixture.Root.Q<Label>("notice").text;
                if (fault == "backup") Assert.That(message, Does.Contain(language == Language.German ? "vorherige Stand" : "previous saved state"));
                else if (fault == "newer") Assert.That(message, Does.Contain(language == Language.German ? "neueren App-Version" : "newer app version"));
                else Assert.That(message, Does.Contain(language == Language.German ? "zugegriffen" : "accessible"));
                fixture.Dispose(); yield return null;
            }
        }
        static void Down(VisualElement element, int id)
        {
            using (var e = PointerDownEvent.GetPooled(new Touch { fingerId = id - 1, position = element.worldBound.center, phase = TouchPhase.Began })) { e.target = element; element.SendEvent(e); }
        }
        static void Up(VisualElement element, int id)
        {
            using (var e = PointerUpEvent.GetPooled(new Touch { fingerId = id - 1, position = element.worldBound.center, phase = TouchPhase.Ended })) { e.target = element; element.SendEvent(e); }
        }
        static void Submit(VisualElement element)
        {
            Assert.That(element, Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = element; element.SendEvent(e); }
        }
        sealed class Fixture : IDisposable
        {
            public static readonly List<Fixture> Live = new List<Fixture>();
            public readonly string DirectoryPath = Path.Combine(Application.temporaryCachePath, "recovery-ui-" + Guid.NewGuid().ToString("N"));
            public readonly Session Session;
            public readonly GameObject Host;
            public readonly PanelSettings Panel;
            public VisualElement Root => Host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture(Language language, string fault = null)
            {
                Session = Session.Open(DirectoryPath, language, maximum => 0);
                foreach (var name in new[] { "Alex", "Bea", "Chris" }) Session.AddPlayer(name);
                if (fault == "damaged")
                {
                    File.WriteAllText(Path.Combine(DirectoryPath, "session.json"), "damaged primary");
                    File.WriteAllText(Path.Combine(DirectoryPath, "session.previous.json"), "damaged backup");
                }
                else if (fault == "backup") File.WriteAllText(Path.Combine(DirectoryPath, "session.json"), "damaged primary");
                else if (fault == "newer") File.WriteAllText(Path.Combine(DirectoryPath, "session.json"), "{\"Version\":999}");
                else if (fault == "unreadable") { File.Delete(Path.Combine(DirectoryPath, "session.json")); Directory.CreateDirectory(Path.Combine(DirectoryPath, "session.json")); }
                if (fault != null) Session = Session.Open(DirectoryPath, language, maximum => 0);
                Host = new GameObject("Recovery interaction test"); Host.SetActive(false);
                var document = Host.AddComponent<UIDocument>();
                Panel = ScriptableObject.CreateInstance<PanelSettings>(); document.panelSettings = Panel;
                Host.AddComponent<GroupScreen>().Initialize(Session); Host.SetActive(true);
                Root.style.width = 390; Root.style.height = 844; Live.Add(this);
            }
            public void Dispose()
            {
                if (!Live.Remove(this)) return;
                UnityEngine.Object.Destroy(Host); UnityEngine.Object.Destroy(Panel);
                if (Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath, true);
            }
        }
    }
}
