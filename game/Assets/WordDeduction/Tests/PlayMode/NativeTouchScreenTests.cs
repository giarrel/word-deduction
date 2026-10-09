using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;
using NativePhase = UnityEngine.InputSystem.TouchPhase;

namespace WordDeduction.Tests
{
    public class NativeTouchScreenTests
    {
        GameObject host;
        PanelSettings panel;
        Touchscreen touchscreen;
        string directory;
        Session session;
        VisualElement root;

        [TearDown] public void Cleanup()
        {
            if (touchscreen != null) InputSystem.RemoveDevice(touchscreen);
            if (host != null) UnityEngine.Object.Destroy(host);
            if (panel != null) UnityEngine.Object.Destroy(panel);
            if (directory != null && Directory.Exists(directory)) Directory.Delete(directory,true);
        }

        void Create()
        {
            directory = Path.Combine(Application.temporaryCachePath,"native-touch-" + Guid.NewGuid().ToString("N"));
            session = Session.Open(directory,Language.English);
            foreach (var name in new[] { "Nora", "Luca", "Bea", "Cora", "Emil" }) session.AddPlayer(name);
            host = new GameObject("Native touch screen test"); host.SetActive(false);
            var document = host.AddComponent<UIDocument>();
            panel = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
            // An actual screen panel receives the installed InputSystem -> InputForUI provider.
            // RenderTexture fixtures and directly pooled pointer events bypass that native seam.
            panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1; document.panelSettings = panel;
            host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
            root = document.rootVisualElement; root.style.width = 360; root.style.height = 640;
            touchscreen = InputSystem.AddDevice<Touchscreen>();
        }

        void Touch(int id, NativePhase phase, Vector2 panelPosition)
        {
            InputSystem.QueueStateEvent(touchscreen,new TouchState {
                touchId = id, phase = phase, pressure = 1,
                position = new Vector2(panelPosition.x,Screen.height - panelPosition.y)
            });
        }

        [UnityTest]
        public IEnumerator CanceledNativeTouchNeverCommitsThePreviewOrder()
        {
            Create(); yield return null; yield return null;
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            var handle = root.Q<VisualElement>("reorder-" + ids[0]);
            int downs = 0, moves = 0, ups = 0, cancels = 0;
            string releasePhase = "none";
            handle.RegisterCallback<PointerDownEvent>(_ => downs++);
            handle.RegisterCallback<PointerMoveEvent>(_ => moves++);
            handle.RegisterCallback<PointerUpEvent>(_ => { ups++; releasePhase = touchscreen.touches[0].phase.ReadValue().ToString(); },TrickleDown.TrickleDown);
            handle.RegisterCallback<PointerCancelEvent>(_ => cancels++);
            var start = handle.worldBound.center;
            var destination = root.Q<VisualElement>("player-" + ids[1]).worldBound.center + new Vector2(0,10);
            Touch(23,NativePhase.Began,start); yield return null; yield return null;
            Touch(23,NativePhase.Moved,destination); yield return null; yield return null;
            Assert.That(downs, Is.GreaterThan(0), "The actual provider must deliver the touch to the rendered handle.");
            Assert.That(moves, Is.GreaterThan(0));
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(ids), "Dragging remains a preview.");
            Touch(23,NativePhase.Canceled,destination); yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(ids),
                "Canceled native touch must keep the order. Provider events: up=" + ups + ", cancel=" + cancels + ", release phase=" + releasePhase);

            var outside = new Vector2(root.Q<ScrollView>("players").contentViewport.worldBound.xMax + 10,destination.y);
            Touch(24,NativePhase.Began,start); yield return null; yield return null;
            Touch(24,NativePhase.Moved,outside); yield return null; yield return null;
            Touch(24,NativePhase.Ended,outside); yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(ids), "An actual outside release also preserves the order.");

            Touch(25,NativePhase.Began,start); yield return null; yield return null;
            Touch(25,NativePhase.Moved,destination); yield return null; yield return null;
            Touch(25,NativePhase.Ended,destination); yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(new[] { ids[1],ids[0],ids[2],ids[3],ids[4] }),
                "A following deliberate native release must still commit exactly once.");
        }

        [UnityTest]
        public IEnumerator CancelAndNextBeginInOneInputUpdateKeepTheOrder()
        {
            Create(); yield return null; yield return null;
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            var handle = root.Q<VisualElement>("reorder-" + ids[0]);
            var start = handle.worldBound.center;
            var destination = root.Q<VisualElement>("player-" + ids[1]).worldBound.center + new Vector2(0,10);
            Touch(23,NativePhase.Began,start); yield return null; yield return null;
            Touch(23,NativePhase.Moved,destination); yield return null; yield return null;
            Assert.That(session.View.Players.Select(p => p.Id), Is.EqualTo(ids));
            // Both events reach InputSystem before InputForUI dispatches its buffered release.
            Touch(23,NativePhase.Canceled,destination);
            Touch(24,NativePhase.Began,start);
            yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(ids),
                "Reusing a canceled touch slot in the same update cannot commit the canceled preview.");
            Touch(24,NativePhase.Canceled,start); yield return null; yield return null;
            Touch(25,NativePhase.Began,start); yield return null; yield return null;
            Touch(25,NativePhase.Moved,destination); yield return null; yield return null;
            Touch(25,NativePhase.Ended,destination); yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(new[] { ids[1],ids[0],ids[2],ids[3],ids[4] }),
                "The cancellation latch must be cleared for a following deliberate drag.");
        }

        [UnityTest]
        public IEnumerator FullyBufferedCanceledDragKeepsTheOrder()
        {
            Create(); yield return null; yield return null;
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            var start = root.Q<VisualElement>("reorder-" + ids[0]).worldBound.center;
            var destination = root.Q<VisualElement>("player-" + ids[1]).worldBound.center + new Vector2(0,10);
            Touch(23,NativePhase.Began,start);
            Touch(23,NativePhase.Moved,destination);
            Touch(23,NativePhase.Canceled,destination);
            Touch(24,NativePhase.Began,start);
            yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(ids),
                "An entirely buffered canceled drag cannot commit using the replacement contact's state.");
            Touch(24,NativePhase.Canceled,start); yield return null; yield return null;
            Touch(25,NativePhase.Began,start); yield return null; yield return null;
            Touch(25,NativePhase.Moved,destination); yield return null; yield return null;
            Touch(25,NativePhase.Ended,destination); yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(new[] { ids[1],ids[0],ids[2],ids[3],ids[4] }),
                "An ambiguous buffered contact must not disable a later deliberate drag.");
        }

        [UnityTest]
        public IEnumerator LaterCanceledContactDoesNotCancelAnEndedDrag()
        {
            Create(); yield return null; yield return null;
            var ids = session.View.Players.Select(p => p.Id).ToArray();
            var start = root.Q<VisualElement>("reorder-" + ids[0]).worldBound.center;
            var destination = root.Q<VisualElement>("player-" + ids[1]).worldBound.center + new Vector2(0,10);
            Touch(23,NativePhase.Began,start); yield return null; yield return null;
            Touch(23,NativePhase.Moved,destination); yield return null; yield return null;
            Touch(23,NativePhase.Ended,destination);
            Touch(24,NativePhase.Began,start);
            Touch(24,NativePhase.Canceled,start);
            yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(new[] { ids[1],ids[0],ids[2],ids[3],ids[4] }),
                "A different contact's later cancellation cannot discard an intentional completed drag.");
            start = root.Q<VisualElement>("reorder-" + ids[1]).worldBound.center;
            destination = root.Q<VisualElement>("player-" + ids[0]).worldBound.center + new Vector2(0,10);
            Touch(25,NativePhase.Began,start); yield return null; yield return null;
            Touch(25,NativePhase.Moved,destination); yield return null; yield return null;
            Touch(25,NativePhase.Ended,destination); yield return null; yield return null;
            Assert.That(Session.Open(directory,Language.German).View.Players.Select(p => p.Id), Is.EqualTo(ids),
                "A later normal drag must remain usable after the buffered valid release.");
        }
    }
}
