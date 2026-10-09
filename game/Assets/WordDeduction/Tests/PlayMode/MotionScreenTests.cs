using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Tests
{
    public class MotionScreenTests
    {
        Fixture fixture;
        [TearDown] public void Cleanup() { fixture?.Dispose(); fixture = null; }

        [UnityTest]
        public IEnumerator PartialPullReturnsGraduallyInsteadOfSnappingAndCannotRevealOrAdvance()
        {
            fixture = new Fixture(); yield return null;
            Submit(fixture.Root.Q<Button>("playButton")); yield return null; yield return null;
            var root = fixture.Root;
            var face = root.Q<VisualElement>("cardFace");
            var drag = root.Q<VisualElement>("cardDrag");
            float restingY = face.worldBound.center.y;
            var start = face.worldBound.center;
            Touch(drag,TouchPhase.Began,start);
            Touch(drag,TouchPhase.Moved,start + Vector2.down * 20);
            yield return null; yield return new WaitForEndOfFrame();
            float heldY = face.worldBound.center.y;
            Assert.That(restingY - heldY,Is.GreaterThan(5),"A small pull visibly follows the finger.");
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"A small pull remains below deliberate reveal.");
            Touch(drag,TouchPhase.Ended,start + Vector2.down * 20);
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"Release conceals synchronously.");
            yield return new WaitForSecondsRealtime(0.07f);
            yield return new WaitForEndOfFrame();
            float returningY = face.worldBound.center.y;
            Assert.That(returningY,Is.GreaterThan(heldY),"The covered card starts moving back promptly.");
            Assert.That(returningY,Is.LessThan(restingY - 0.5f),"Even a partial pull has a visible, gradual return instead of a one-frame snap.");
            Assert.That(root.Q<Button>("nextOwner").enabledSelf,Is.False,"An unread card cannot be advanced during its return.");
            yield return new WaitForSecondsRealtime(0.2f); yield return new WaitForEndOfFrame();
            Assert.That(face.worldBound.center.y,Is.EqualTo(restingY).Within(0.1f),"The bounded return settles completely.");
            Assert.That(root.Q<Button>("nextOwner").enabledSelf,Is.False);
        }

        [UnityTest]
        public IEnumerator RegrabbingAReturningCardKeepsItsPositionAndRequiresANewDeliberatePull()
        {
            fixture = new Fixture(); yield return null;
            Submit(fixture.Root.Q<Button>("playButton")); yield return null; yield return null;
            var root = fixture.Root;
            var face = root.Q<VisualElement>("cardFace");
            var drag = root.Q<VisualElement>("cardDrag");
            var start = face.worldBound.center;
            Touch(drag,TouchPhase.Began,start);
            Touch(drag,TouchPhase.Moved,start + Vector2.down * 100);
            Assert.That(root.Q<Label>("secretWord").text,Is.Not.Empty);
            yield return null; yield return new WaitForEndOfFrame();
            Touch(drag,TouchPhase.Ended,start + Vector2.down * 100);
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty);
            yield return new WaitForSecondsRealtime(0.02f); yield return new WaitForEndOfFrame();
            var caughtAt = face.worldBound.center;
            Touch(drag,TouchPhase.Began,caughtAt);
            Touch(drag,TouchPhase.Moved,caughtAt);
            yield return null; yield return new WaitForEndOfFrame();
            Assert.That(face.worldBound.center.y,Is.EqualTo(caughtAt.y).Within(0.1f),"A stationary finger catches the covered card without snapping it away.");
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"Residual decoration does not count as a fresh reveal gesture.");
            Touch(drag,TouchPhase.Moved,caughtAt + Vector2.down * 5);
            yield return null; yield return new WaitForEndOfFrame();
            Assert.That(face.worldBound.center.y,Is.LessThan(caughtAt.y - 3),"Further movement follows the finger directly.");
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty);
            Touch(drag,TouchPhase.Moved,caughtAt + Vector2.down * 100);
            Assert.That(root.Q<Label>("secretWord").text,Is.Not.Empty);
            Touch(drag,TouchPhase.Ended,caughtAt + Vector2.down * 100);
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty);
        }

        static void Touch(VisualElement element,TouchPhase phase,Vector2 position)
        {
            var touch = new Touch { fingerId = 0,position = position,phase = phase };
            if (phase == TouchPhase.Began) { using (var e = PointerDownEvent.GetPooled(touch)) { e.target=element; element.SendEvent(e); } }
            else if (phase == TouchPhase.Moved) { using (var e = PointerMoveEvent.GetPooled(touch)) { e.target=element; element.SendEvent(e); } }
            else if (phase == TouchPhase.Ended) { using (var e = PointerUpEvent.GetPooled(touch)) { e.target=element; element.SendEvent(e); } }
        }
        static void Submit(VisualElement element)
        {
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target=element; element.SendEvent(e); }
        }
        sealed class Fixture : IDisposable
        {
            readonly string directory = Path.Combine(Application.temporaryCachePath,"motion-ui-" + Guid.NewGuid().ToString("N"));
            readonly GameObject host;
            readonly PanelSettings panel;
            public VisualElement Root => host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture()
            {
                var session = Session.Open(directory,Language.English,maximum => 0);
                session.AddPlayer("Alex"); session.AddPlayer("Bea"); session.AddPlayer("Chris");
                host = new GameObject("Motion interaction test"); host.SetActive(false);
                var document = host.AddComponent<UIDocument>();
                panel = ScriptableObject.CreateInstance<PanelSettings>(); document.panelSettings = panel;
                host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
                Root.style.width = 360; Root.style.height = 640;
            }
            public void Dispose()
            {
                UnityEngine.Object.Destroy(host); UnityEngine.Object.Destroy(panel);
                if (Directory.Exists(directory)) Directory.Delete(directory,true);
            }
        }
    }
}
