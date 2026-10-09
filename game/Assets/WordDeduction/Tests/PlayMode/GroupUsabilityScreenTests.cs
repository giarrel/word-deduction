using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Tests
{
    public class GroupUsabilityScreenTests
    {
        GameObject host;
        PanelSettings panel;
        RenderTexture texture;
        readonly System.Collections.Generic.Dictionary<TextElement,float> originalSizes = new System.Collections.Generic.Dictionary<TextElement,float>();
        string directory;
        Session session;
        VisualElement root;
        [TearDown] public void Cleanup()
        {
            if (host != null) UnityEngine.Object.Destroy(host);
            if (panel != null) UnityEngine.Object.Destroy(panel);
            if (texture != null) UnityEngine.Object.Destroy(texture);
            if (directory != null && Directory.Exists(directory)) Directory.Delete(directory,true);
        }
        void Create(Language language, int count = 12)
        {
            originalSizes.Clear();
            directory = Path.Combine(Application.temporaryCachePath,"group-usability-" + Guid.NewGuid().ToString("N"));
            session = Session.Open(directory, language);
            for (int i = 0; i < count; i++) session.AddPlayer(i < 2 ? "Zoë 李明" : "Player " + (i + 1));
            host = new GameObject("Group usability test"); host.SetActive(false);
            var document = host.AddComponent<UIDocument>();
            panel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
            texture = new RenderTexture(360,640,0); texture.Create(); panel.targetTexture = texture;
            panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1; document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
            root = document.rootVisualElement; root.style.width = 360; root.style.height = 640;
        }
        [UnityTest]
        public IEnumerator NameDragsNeverEditAndOnlyTheExplicitButtonEdits()
        {
            Create(Language.German); yield return null; yield return null;
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            var row = root.Q<VisualElement>("player-" + ids[0]);
            var name = row.Q<TextElement>(className: "player-name");
            var start = name.worldBound.center;
            Touch(name, TouchPhase.Began, start);
            Touch(name, TouchPhase.Moved, start + new Vector2(0,-8));
            Touch(name, TouchPhase.Ended, start + new Vector2(0,-8));
            yield return null;
            Assert.That(root.Q<TextField>("renameInput"), Is.Null, "A short name drag must not edit.");
            Assert.That(name is Button, Is.False, "The name itself has no editing action.");
            Assert.That(root.Query<Button>(className: "participation").ToList(), Is.Empty);
            var edit = root.Q<Button>("edit-" + ids[0]);
            Assert.That(edit.text, Is.EqualTo("Bearbeiten"));
            Submit(edit); yield return null; yield return null;
            root.Q<TextField>("renameInput").value = "Uncommitted";
            Submit(root.Q<Button>("cancelRename")); yield return null;
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(ids));
            Assert.That(session.View.Players[0].Name, Is.EqualTo("Zoë 李明"));
        }
        [UnityTest]
        public IEnumerator LegacyPeopleAreCollapsedAndOnlyExplicitlyRestored()
        {
            Create(Language.English, 3);
            var saved = session.View.Players[1]; session.SetParticipation(saved.Id, false);
            host.GetComponent<GroupScreen>().Initialize(session);
            yield return null; yield return null;
            Assert.That(root.Q<VisualElement>("player-" + saved.Id), Is.Null, "Inactive legacy people start collapsed.");
            Assert.That(session.View.Players.Count, Is.EqualTo(3));
            Submit(root.Q<Button>("savedPeople")); yield return null; yield return null;
            Assert.That(root.Q<Label>("savedPeopleHint").text, Does.Contain("earlier version"));
            Submit(root.Q<Button>("edit-" + saved.Id)); yield return null;
            Submit(root.Q<Button>("restorePlayer")); yield return null; yield return null;
            Assert.That(Session.Open(directory, Language.German).View.Players.Single(p => p.Id == saved.Id).Active, Is.True);
            Assert.That(root.Q<Button>("savedPeople"), Is.Null, "Compatibility section disappears when no longer needed.");
        }
        [UnityTest]
        public IEnumerator HandleReordersOnlyOnValidDropAndMoveButtonsReachBothEnds()
        {
            Create(Language.English, 4); yield return null; yield return null;
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            var handle = root.Q<VisualElement>("reorder-" + ids[0]);
            Assert.That(handle, Is.Not.Null, "There is a distinct reorder handle.");
            var start = handle.worldBound.center;
            var destination = root.Q<VisualElement>("player-" + ids[2]).worldBound.center + new Vector2(0,10);
            Touch(handle, TouchPhase.Began, start);
            Touch(handle, TouchPhase.Moved, destination); yield return null;
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(ids), "Dragging is only a preview.");
            Touch(handle, TouchPhase.Ended, destination); yield return null; yield return null;
            var expected = new[] { ids[1], ids[2], ids[0], ids[3] };
            Assert.That(Session.Open(directory, Language.German).View.Players.Select(p => p.Id), Is.EqualTo(expected));
            handle = root.Q<VisualElement>("reorder-" + ids[0]); start = handle.worldBound.center;
            Touch(handle, TouchPhase.Began, start); Touch(handle, TouchPhase.Moved, start - new Vector2(0,100));
            Touch(handle, TouchPhase.Canceled, start); yield return null;
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(expected), "Cancel does not commit.");
            Touch(handle, TouchPhase.Began, start); Touch(handle, TouchPhase.Moved, new Vector2(-10,20)); Touch(handle, TouchPhase.Ended, new Vector2(-10,20)); yield return null;
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(expected), "Outside drop does not commit.");
            Submit(root.Q<Button>("edit-" + ids[0])); yield return null;
            Submit(root.Q<Button>("movePlayerDown")); yield return null;
            Assert.That(root.Q<Button>("movePlayerDown").enabledInHierarchy, Is.False, "Last person cannot move further down.");
            for (int i = 0; i < 3; i++) { Submit(root.Q<Button>("movePlayerUp")); yield return null; }
            Assert.That(root.Q<Button>("movePlayerUp").enabledInHierarchy, Is.False, "First person cannot move further up.");
            Submit(root.Q<Button>("cancelRename")); yield return null;
            Assert.That(Session.Open(directory, Language.German).View.Players.Select(p => p.Id), Is.EqualTo(ids));
            Submit(root.Q<Button>("playButton")); yield return null;
            Assert.That(session.Match.Owner.Id, Is.EqualTo(ids[0]), "Next game follows the chosen order.");
        }
        [UnityTest]
        public IEnumerator OrdinaryScrollDoesNotReorderAndInterruptedOrFailedDropKeepsCommittedGroup()
        {
            Create(Language.German); yield return null; yield return null;
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            var list = root.Q<ScrollView>("players");
            var name = root.Q<Label>("name-" + ids[1]);
            var start = name.worldBound.center;
            Touch(name, TouchPhase.Began, start);
            for (int i = 1; i <= 6; i++) { Touch(name, TouchPhase.Moved, start - new Vector2(0,i * 18)); yield return null; }
            Touch(name, TouchPhase.Ended, start - new Vector2(0,108)); yield return null;
            Assert.That(list.scrollOffset.y, Is.GreaterThan(0), "Ordinary name drags scroll the list.");
            Assert.That(root.Q<TextField>("renameInput"), Is.Null);
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(ids));
            list.scrollOffset = Vector2.zero; yield return null; yield return null;
            var handle = root.Q<VisualElement>("reorder-" + ids[0]); start = handle.worldBound.center;
            var destination = root.Q<VisualElement>("player-" + ids[1]).worldBound.center + new Vector2(0,10);
            Touch(handle, TouchPhase.Began, start); Touch(handle, TouchPhase.Moved, destination);
            host.SendMessage("OnApplicationFocus", false);
            Touch(handle, TouchPhase.Ended, destination); yield return null;
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(ids), "Focus interruption cancels the preview.");
            host.SendMessage("OnApplicationFocus", true);
            Directory.CreateDirectory(Path.Combine(directory,"session.pending.json"));
            Touch(handle, TouchPhase.Began, start); Touch(handle, TouchPhase.Moved, destination); Touch(handle, TouchPhase.Ended, destination);
            yield return null; yield return null;
            Assert.That(root.Q<Label>("notice").text, Is.EqualTo(Copy.Get(Language.German,"SaveFailed")));
            Assert.That(Session.Open(directory,Language.English).View.Players.Select(p => p.Id), Is.EqualTo(ids));
            Directory.Delete(Path.Combine(directory,"session.pending.json"));
        }
        [UnityTest]
        public IEnumerator LargeBilingualGroupsKeepEditAndMoveControlsReachable()
        {
            foreach (var language in new[] { Language.German, Language.English })
            {
                Create(language,20);
                var last = session.View.Players.Last().Id;
                session.RenamePlayer(last,new string('W',24));
                host.GetComponent<GroupScreen>().Initialize(session);
                yield return null; yield return null;
                yield return Capture("group-" + language);
                Enlarge(); yield return null; yield return null;
                Assert.That(root.Q<Label>("editHint").resolvedStyle.display, Is.Not.EqualTo(DisplayStyle.None));
                Assert.That(root.Q<Button>("addPlayer").enabledInHierarchy, Is.False, "Capacity is explicit.");
                var list = root.Q<ScrollView>("players");
                list.ScrollTo(root.Q<VisualElement>("player-" + last)); yield return null; yield return null;
                var name = root.Q<Label>("name-" + last);
                Assert.That(name.contentRect.height, Is.GreaterThanOrEqualTo(name.MeasureTextSize(name.text,name.contentRect.width,VisualElement.MeasureMode.AtMost,0,VisualElement.MeasureMode.Undefined).y - 1));
                Assert.That(root.Q<VisualElement>("player-" + last).worldBound.height, Is.LessThanOrEqualTo(list.contentViewport.worldBound.height), "A long name and its Edit control fit together.");
                yield return Capture("group-large-last-" + language);
                Submit(root.Q<Button>("edit-" + last)); yield return null; yield return null;
                texture.Release(); texture.height = 380; texture.Create();
                root.style.height = 380; yield return null; yield return null;
                root.Q<VisualElement>("safeRoot").AddToClassList("typing");
                Enlarge(); yield return null; yield return null;
                list.ScrollTo(root.Q<VisualElement>("player-" + last)); yield return null; yield return null;
                yield return Capture("editing-large-" + language);
                foreach (var key in new[] { "movePlayerUp", "movePlayerDown", "removePlayer", "cancelRename", "saveRename" })
                {
                    var button = root.Q<Button>(key);
                    Assert.That(button.worldBound.yMin, Is.GreaterThanOrEqualTo(list.contentViewport.worldBound.yMin - 1),key);
                    Assert.That(button.worldBound.yMax, Is.LessThanOrEqualTo(list.contentViewport.worldBound.yMax + 1),key);
                    Assert.That(button.worldBound.height, Is.GreaterThanOrEqualTo(48),key);
                    Assert.That(button.contentRect.height, Is.GreaterThanOrEqualTo(button.MeasureTextSize(button.text,button.contentRect.width,VisualElement.MeasureMode.AtMost,0,VisualElement.MeasureMode.Undefined).y - 1),key);
                }
                Cleanup(); host = null; panel = null; texture = null; directory = null; yield return null;
            }
        }
        [UnityTest]
        public IEnumerator HandleAutoScrollReachesTheEndWithoutReorderingDuringPreview()
        {
            Create(Language.English,8); yield return null; yield return null;
            var ids = session.View.Players.Select(p=>p.Id).ToArray();
            var list = root.Q<ScrollView>("players");
            var handle = root.Q<VisualElement>("reorder-" + ids[0]);
            var start = handle.worldBound.center;
            var edge = new Vector2(start.x,list.contentViewport.worldBound.yMax - 3);
            Touch(handle,TouchPhase.Began,start); Touch(handle,TouchPhase.Moved,edge);
            float deadline = Time.realtimeSinceStartup + 4;
            while (list.scrollOffset.y < list.verticalScroller.highValue - 1 && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(list.scrollOffset.y, Is.GreaterThanOrEqualTo(list.verticalScroller.highValue - 1), "Holding at the edge scrolls to the final person.");
            Assert.That(session.View.Players.Select(p=>p.Id), Is.EqualTo(ids));
            Touch(handle,TouchPhase.Ended,edge); yield return null;
            Assert.That(session.View.Players.Last().Id, Is.EqualTo(ids[0]));
        }
        void Enlarge()
        {
            root.AddToClassList("large-type");
            foreach (var element in root.Query<TextElement>().ToList())
            {
                if (element.GetFirstAncestorOfType<TextField>() != null) continue;
                if (!originalSizes.TryGetValue(element,out float size)) { size = element.resolvedStyle.fontSize; originalSizes[element] = size; }
                element.style.fontSize = size * 1.5f;
            }
            foreach (var field in root.Query<TextField>().ToList()) field.style.fontSize = 25.5f;
        }
        IEnumerator Capture(string name)
        {
            yield return null; yield return null;
            var previous = RenderTexture.active; RenderTexture.active = texture;
            var pixels = new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
            pixels.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0); pixels.Apply(); RenderTexture.active = previous;
            string path = Path.GetFullPath(Path.Combine(Application.dataPath,"../../artifacts/group-usability/screenshots")); Directory.CreateDirectory(path);
            File.WriteAllBytes(Path.Combine(path,name + ".png"),pixels.EncodeToPNG()); UnityEngine.Object.Destroy(pixels);
        }
        static void Submit(VisualElement element)
        {
            Assert.That(element, Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = element; element.SendEvent(e); }
        }
        static void Touch(VisualElement target, TouchPhase phase, Vector2 position)
        {
            var touch = new Touch { fingerId = 0, position = position, phase = phase };
            if (phase == TouchPhase.Began) { using (var e = PointerDownEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else if (phase == TouchPhase.Moved) { using (var e = PointerMoveEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else if (phase == TouchPhase.Canceled) { using (var e = PointerCancelEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
            else { using (var e = PointerUpEvent.GetPooled(touch)) { e.target = target; target.SendEvent(e); } }
        }
    }
}
