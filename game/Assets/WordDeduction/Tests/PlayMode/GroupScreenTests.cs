using System.Collections;
using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;
namespace WordDeduction.Tests
{
    public class GroupScreenTests
    {
        [UnityTest]
        public IEnumerator GroupScreenExposesTheNameEntryAndBothModes()
        {
            var host = new GameObject("Group screen test");
            var document = host.AddComponent<UIDocument>();
            var panel = ScriptableObject.CreateInstance<PanelSettings>();
            document.panelSettings = panel;
            host.AddComponent<GroupScreen>();
            yield return null;
            Assert.That(document.rootVisualElement.Q<TextField>("nameInput"), Is.Not.Null, "Name entry is the start of group setup.");
            Assert.That(document.rootVisualElement.Q<Button>("quickMode"), Is.Not.Null);
            Assert.That(document.rootVisualElement.Q<Button>("classicMode"), Is.Not.Null);
            UnityEngine.Object.Destroy(host); UnityEngine.Object.Destroy(panel);
        }
        [UnityTest]
        public IEnumerator GroupActionsThroughRenderedControlsSurviveReopening()
        {
            string directory = Path.Combine(Application.temporaryCachePath,"group-ui-" + Guid.NewGuid().ToString("N"));
            var session = Session.Open(directory,Language.English);
            var host = new GameObject("Group interaction test"); host.SetActive(false);
            var document = host.AddComponent<UIDocument>();
            var panel = ScriptableObject.CreateInstance<PanelSettings>(); document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
            yield return null;
            var root = document.rootVisualElement;
            foreach (var name in new[] { "Alex", "Bea", "Chris" })
            {
                root.Q<TextField>("nameInput").value = name; Submit(root.Q<Button>("addPlayer")); yield return null;
                Assert.That(root.Q<TextField>("nameInput").value, Is.Empty, "Adding keeps the field ready for the next person.");
            }
            var id = session.View.Players[0].Id;
            Assert.That(root.Q<ScrollView>("players").childCount,Is.EqualTo(3));
            Submit(root.Q<Button>("participation-" + id)); yield return null;
            Submit(root.Q<Button>("edit-" + id)); yield return null;
            root.Q<TextField>("renameInput").value = "Alexandra";
            Submit(root.Q<Button>("saveRename")); yield return null;
            Submit(root.Q<Button>("edit-" + session.View.Players[1].Id)); yield return null;
            Submit(root.Q<Button>("removePlayer")); yield return null;
            Submit(root.Q<Button>("undo")); yield return null;
            Submit(root.Q<Button>("german")); yield return null;
            Submit(root.Q<Button>("classicMode")); yield return null;
            var reopened = Session.Open(directory,Language.English).View;
            Assert.That(reopened.Players.Count,Is.EqualTo(3));
            Assert.That(reopened.Players[0].Name,Is.EqualTo("Alexandra"));
            Assert.That(reopened.Players[0].Active,Is.False);
            Assert.That(reopened.Mode,Is.EqualTo(GameMode.Classic)); Assert.That(reopened.Language,Is.EqualTo(Language.German));
            Assert.That(root.Q<Label>("groupTitle").text,Is.EqualTo("Eure Gruppe"));
            UnityEngine.Object.Destroy(host); UnityEngine.Object.Destroy(panel);
            Directory.Delete(directory,true);
        }
        static void Submit(VisualElement element)
        {
            Assert.That(element,Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = element; element.SendEvent(e); }
        }
        [UnityTest]
        public IEnumerator AddingAPlayerScrollsTheNewRowIntoViewAfterLayout()
        {
            string directory = Path.Combine(Application.temporaryCachePath,"group-scroll-" + Guid.NewGuid().ToString("N"));
            var session = Session.Open(directory,Language.English);
            for (int i = 1; i <= 4; i++) session.AddPlayer("Player " + i);
            var host = new GameObject("Group scroll test"); host.SetActive(false);
            var document = host.AddComponent<UIDocument>();
            var panel = ScriptableObject.CreateInstance<PanelSettings>(); document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
            var root = document.rootVisualElement; root.style.width = 390; root.style.height = 640;
            var list = root.Q<ScrollView>("players"); list.style.flexGrow = 0; list.style.flexShrink = 0; list.style.height = 296;
            yield return null; yield return null;
            root.Q<TextField>("nameInput").value = "Last player";
            Submit(root.Q<Button>("addPlayer"));
            for (int frame = 0; frame < 5; frame++) yield return null;
            var scroll = root.Q<ScrollView>("players"); var last = scroll.ElementAt(scroll.childCount - 1);
            Assert.That(scroll.contentViewport.worldBound.height,Is.GreaterThan(last.worldBound.height));
            Assert.That(last.worldBound.yMin,Is.GreaterThanOrEqualTo(scroll.contentViewport.worldBound.yMin - 1));
            Assert.That(last.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax + 1),"The newly added person must be visible, not hidden below the list.");
            UnityEngine.Object.Destroy(host); UnityEngine.Object.Destroy(panel); Directory.Delete(directory,true);
        }
        [UnityTest]
        public IEnumerator EditingTheLastPlayerKeepsAllActionsVisibleAfterTheViewportShrinks()
        {
            string directory = Path.Combine(Application.temporaryCachePath,"group-edit-scroll-" + Guid.NewGuid().ToString("N"));
            var session = Session.Open(directory,Language.English);
            for (int i = 1; i <= 4; i++) session.AddPlayer("Player " + i);
            var host = new GameObject("Group edit scroll test"); host.SetActive(false);
            var document = host.AddComponent<UIDocument>();
            var panel = ScriptableObject.CreateInstance<PanelSettings>(); document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
            var root = document.rootVisualElement; root.style.width = 390; root.style.height = 640;
            var scroll = root.Q<ScrollView>("players"); scroll.style.flexGrow = 0; scroll.style.flexShrink = 0; scroll.style.height = 296;
            yield return null; yield return null;
            Submit(root.Q<Button>("edit-" + session.View.Players[3].Id));
            yield return null; yield return null;
            scroll.style.height = 200;
            for (int frame = 0; frame < 5; frame++) yield return null;
            var row = root.Q<VisualElement>("player-" + session.View.Players[3].Id);
            Assert.That(row.worldBound.yMin,Is.GreaterThanOrEqualTo(scroll.contentViewport.worldBound.yMin - 1));
            Assert.That(row.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax + 1),"Saving and cancelling must remain visible when the keyboard shrinks the viewport.");
            UnityEngine.Object.Destroy(host); UnityEngine.Object.Destroy(panel); Directory.Delete(directory,true);
        }
        [UnityTest]
        public IEnumerator FailedRenameKeepsTheTypedNameForRetry()
        {
            string directory = Path.Combine(Application.temporaryCachePath,"group-failure-" + Guid.NewGuid().ToString("N"));
            var session = Session.Open(directory,Language.English); session.AddPlayer("Alex");
            var host = new GameObject("Group failure test"); host.SetActive(false);
            var document = host.AddComponent<UIDocument>();
            var panel = ScriptableObject.CreateInstance<PanelSettings>(); document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true); yield return null;
            var root = document.rootVisualElement;
            Submit(root.Q<Button>("edit-" + session.View.Players[0].Id)); yield return null;
            root.Q<TextField>("renameInput").value = "Alexandra";
            Directory.CreateDirectory(Path.Combine(directory,"session.pending.json"));
            Submit(root.Q<Button>("saveRename")); yield return null;
            Assert.That(root.Q<TextField>("renameInput").value,Is.EqualTo("Alexandra"),"A save failure must not discard the typed name.");
            Assert.That(session.View.Players[0].Name,Is.EqualTo("Alex"));
            Directory.Delete(Path.Combine(directory,"session.pending.json"));
            Submit(root.Q<Button>("saveRename")); yield return null;
            Assert.That(Session.Open(directory,Language.English).View.Players[0].Name,Is.EqualTo("Alexandra"));
            UnityEngine.Object.Destroy(host); UnityEngine.Object.Destroy(panel); Directory.Delete(directory,true);
        }
    }
}
