using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Tests
{
    public class QuickScreenTests
    {
        [TearDown] public void Cleanup() { foreach (var fixture in Fixture.Live.ToArray()) fixture.Dispose(); }
        [UnityTest]
        public IEnumerator QuickStartsAtANamedCoveredCardAndRequiresIntentionalReveal()
        {
            var fixture = new Fixture(Language.English);
            yield return null;
            Submit(fixture.Root.Q<Button>("playButton")); yield return null;
            Assert.That(fixture.Root.Q<Label>("cardOwner"), Is.Not.Null, "Play opens a named secret card.");
            Assert.That(fixture.Root.Q<Label>("secretWord").text, Is.Empty);
            Assert.That(fixture.Root.Q<Button>("nextOwner").enabledSelf, Is.False);
            var drag = fixture.Root.Q<VisualElement>("cardDrag");
            Down(drag,1,300); Up(drag,1,300);
            Assert.That(fixture.Root.Q<Label>("secretWord").text,Is.Empty,"A tap never reveals.");
            Assert.That(fixture.Root.Q<Button>("nextOwner").enabledSelf,Is.False);
            Down(drag,1,300); Move(drag,1,220);
            Assert.That(fixture.Root.Q<Label>("secretWord").text,Is.Not.Empty,"Deliberate drag reveals.");
            var owner = fixture.Session.Match.Owner.Id;
            Submit(fixture.Root.Q<Button>("nextOwner"));
            Assert.That(fixture.Session.Match.Owner.Id,Is.EqualTo(owner),"Next while open cannot advance.");
            Up(drag,1,220);
            Assert.That(fixture.Root.Q<Label>("secretWord").text,Is.Empty,"Release erases before the next frame.");
            yield return new WaitForSecondsRealtime(0.25f);
            var next = fixture.Root.Q<Button>("nextOwner");
            Assert.That(next.enabledSelf,Is.True); Submit(next); Submit(next); yield return null;
            Assert.That(fixture.Session.Match.HandoffNumber,Is.EqualTo(2),"Duplicate Next cannot skip a card.");
            Assert.That(fixture.Root.Q<Label>("secretWord").text,Is.Empty);
            fixture.Dispose();
        }
        [UnityTest]
        public IEnumerator HoldCancelCaptureLossSecondFingerAndLifecycleEraseSecretsSynchronously()
        {
            var fixture = new Fixture(Language.English); yield return null;
            Submit(fixture.Root.Q<Button>("playButton")); yield return null;
            var root = fixture.Root; var hold = root.Q<VisualElement>("holdReveal");
            Down(hold,1,300); Assert.That(root.Q<Label>("secretWord").text,Is.Not.Empty);
            Move(hold,1,900); Up(hold,1,900); Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"Releasing outside conceals immediately.");
            Down(hold,1,300);
            using (var e = PointerCancelEvent.GetPooled(new Touch { fingerId=0,position=new Vector2(120,300),phase=TouchPhase.Canceled })) { e.target=hold; hold.SendEvent(e); }
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"Cancel conceals immediately.");
            Down(hold,1,300);
            using (var e = PointerCaptureOutEvent.GetPooled(hold,root,1)) { e.target=hold; hold.SendEvent(e); }
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"Capture loss conceals immediately.");
            Down(hold,1,300); Down(root.Q<VisualElement>("safeRoot"),2,400);
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"A second finger clears the secret before doing anything else.");
            Move(hold,1,200); Up(hold,1,200);
            Up(root.Q<VisualElement>("safeRoot"),2,400);
            Assert.That(root.Q<Label>("secretWord").text,Is.Empty,"Old pointer events cannot reopen.");
            Down(hold,1,300); var oldLabel = root.Q<Label>("secretWord");
            fixture.Host.SendMessage("OnApplicationFocus",false);
            Assert.That(oldLabel.text,Is.Empty,"Focus loss clears even the old detached label.");
            Assert.That(root.Q<Button>("resumeMatch"),Is.Not.Null);
            Submit(root.Q<Button>("resumeMatch")); Down(root.Q<VisualElement>("holdReveal"),1,300);
            oldLabel=root.Q<Label>("secretWord"); fixture.Host.SendMessage("OnApplicationPause",true);
            Assert.That(oldLabel.text,Is.Empty); fixture.Host.SendMessage("OnApplicationPause",false);
            Assert.That(root.Q<Button>("resumeMatch"),Is.Not.Null,"Resume requires an explicit choice.");
            Submit(root.Q<Button>("resumeMatch")); Down(root.Q<VisualElement>("holdReveal"),1,300);
            oldLabel=root.Q<Label>("secretWord"); fixture.Host.SetActive(false);
            Assert.That(oldLabel.text,Is.Empty,"Disabling the surface erases text.");
            fixture.Host.SetActive(true); yield return null;
            Assert.That(fixture.Root.Q<Button>("resumeMatch"),Is.Not.Null);
            fixture.Dispose();
        }
        [UnityTest]
        public IEnumerator BothLanguagesCompleteTheActualQuickFlowAndKeepTheGroup()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language); yield return null;
                Submit(fixture.Root.Q<Button>("playButton")); yield return null;
                for (int person=0;person<3;person++)
                {
                    var hold=fixture.Root.Q<VisualElement>("holdReveal"); Down(hold,1,300); Up(hold,1,300);
                    yield return new WaitForSecondsRealtime(0.25f);
                    Submit(fixture.Root.Q<Button>("nextOwner")); yield return null;
                }
                Assert.That(fixture.Root.Q<Label>("startingPlayer").text,Is.EqualTo("Alex"));
                Submit(fixture.Root.Q<Button>("matchHelp")); yield return null;
                Assert.That(fixture.Root.Q<Label>("helpText").text,Does.Contain(language==Language.German ? "Hinweis" : "clue"));
                Submit(fixture.Root.Q<Button>("closeHelp")); Submit(fixture.Root.Q<Button>("beginVote")); yield return null;
                Submit(fixture.Root.Q<Button>("suspect-"+fixture.Session.Match.Participants[1].Id)); yield return null;
                Assert.That(fixture.Session.Match.Result,Is.Null);
                Submit(fixture.Root.Q<Button>("changeSuspect")); yield return null;
                Submit(fixture.Root.Q<Button>("suspect-"+fixture.Session.Match.Participants[0].Id)); yield return null;
                Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
                Assert.That(fixture.Root.Q<Label>("resultTitle").text,Is.EqualTo(language==Language.German ? "Die Bürger gewinnen!" : "Civilians win!"));
                Assert.That(fixture.Root.Q<Label>("resultWordCivilian").text,Is.Not.Empty);
                string matchId=fixture.Session.Match.Id; Submit(fixture.Root.Q<Button>("rematch")); yield return null;
                Assert.That(fixture.Session.Match.Id,Is.Not.EqualTo(matchId));
                Submit(fixture.Root.Q<Button>("matchBack")); Submit(fixture.Root.Q<Button>("abandonMatch")); yield return null;
                Assert.That(fixture.Session.Match,Is.Not.Null,"Abandon is only a proposal until confirmed.");
                Submit(fixture.Root.Q<Button>("keepMatch")); Submit(fixture.Root.Q<Button>("abandonMatch")); Submit(fixture.Root.Q<Button>("confirmAbandon")); yield return null;
                Assert.That(fixture.Session.Match,Is.Null); Assert.That(fixture.Session.View.ActiveCount,Is.EqualTo(3));
                Assert.That(fixture.Root.Q<Button>("playButton").enabledSelf,Is.True);
                fixture.Dispose(); yield return null;
            }
        }
        [UnityTest]
        public IEnumerator SaveFailuresKeepUnconfirmedScreensAndSecretsCoveredForRetry()
        {
            var fixture = new Fixture(Language.German); yield return null;
            string pending=Path.Combine(fixture.DirectoryPath,"session.pending.json");
            Directory.CreateDirectory(pending); Submit(fixture.Root.Q<Button>("playButton")); yield return null;
            Assert.That(fixture.Session.Match,Is.Null); Assert.That(fixture.Root.Q<Label>("notice").text,Does.Contain("Speichern"));
            Directory.Delete(pending); Submit(fixture.Root.Q<Button>("playButton")); yield return null;
            var owner=fixture.Session.Match.Owner.Id;
            var hold=fixture.Root.Q<VisualElement>("holdReveal"); Down(hold,1,300); Up(hold,1,300); yield return new WaitForSecondsRealtime(0.25f);
            Directory.CreateDirectory(pending); Submit(fixture.Root.Q<Button>("nextOwner")); yield return null;
            Assert.That(fixture.Session.Match.Owner.Id,Is.EqualTo(owner)); Assert.That(fixture.Root.Q<Label>("secretWord").text,Is.Empty);
            Assert.That(fixture.Root.Q<Label>("matchNotice").text,Does.Contain("Speichern"));
            Directory.Delete(pending); Submit(fixture.Root.Q<Button>("nextOwner")); yield return null;
            while(fixture.Session.Match.Phase==MatchPhase.Handoff)
            {
                hold=fixture.Root.Q<VisualElement>("holdReveal"); Down(hold,1,300); Up(hold,1,300); yield return new WaitForSecondsRealtime(0.25f);
                Submit(fixture.Root.Q<Button>("nextOwner")); yield return null;
            }
            Submit(fixture.Root.Q<Button>("beginVote")); Submit(fixture.Root.Q<Button>("suspect-"+owner));
            Directory.CreateDirectory(pending); Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
            Assert.That(fixture.Root.Q<Label>("resultTitle"),Is.Null); Assert.That(fixture.Root.Q<Button>("confirmSuspect"),Is.Not.Null);
            Assert.That(Session.Open(fixture.DirectoryPath,Language.English).Match.Result,Is.Null);
            Directory.Delete(pending); Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
            string id=fixture.Session.Match.Id; Directory.CreateDirectory(pending); Submit(fixture.Root.Q<Button>("rematch")); yield return null;
            Assert.That(fixture.Session.Match.Id,Is.EqualTo(id)); Assert.That(fixture.Root.Q<Label>("resultTitle"),Is.Not.Null);
            Directory.Delete(pending); Submit(fixture.Root.Q<Button>("rematch")); yield return null;
            Assert.That(fixture.Session.Match.Id,Is.Not.EqualTo(id));
            fixture.Dispose();
        }
        [UnityTest]
        public IEnumerator TwentyPlayersCanReachTheLastSuspectAndResolveARepeatedTieOnAShortScreen()
        {
            var fixture = new Fixture(Language.English,20); fixture.Root.style.height=640; yield return null;
            Submit(fixture.Root.Q<Button>("playButton")); yield return null; yield return null;
            var next=fixture.Root.Q<Button>("nextOwner");
            Assert.That(next.worldBound.yMax,Is.LessThanOrEqualTo(640),"Primary action stays within the short screen.");
            for(int i=0;i<20;i++)
            {
                var hold=fixture.Root.Q<VisualElement>("holdReveal"); Down(hold,1,300); Up(hold,1,300); yield return new WaitForSecondsRealtime(0.15f);
                Submit(fixture.Root.Q<Button>("nextOwner")); yield return null;
            }
            Submit(fixture.Root.Q<Button>("beginVote")); yield return null;
            var scroll=fixture.Root.Q<ScrollView>("suspects"); var last=scroll.ElementAt(19); scroll.ScrollTo(last); yield return null;
            Assert.That(last.worldBound.yMax,Is.LessThanOrEqualTo(scroll.contentViewport.worldBound.yMax+1));
            Submit(last); Submit(fixture.Root.Q<Button>("changeSuspect")); Submit(fixture.Root.Q<Button>("recordTie")); yield return null;
            Assert.That(fixture.Root.Q<Label>("voteTitle").text,Is.EqualTo("One short runoff."));
            Submit(fixture.Root.Q<Button>("recordTie")); yield return null;
            Assert.That(fixture.Session.Match.Result.Reason,Is.EqualTo(Outcome.RepeatedTie));
            Submit(fixture.Root.Q<Button>("editGroup")); yield return null;
            Assert.That(fixture.Session.View.ActiveCount,Is.EqualTo(20)); fixture.Dispose();
        }
        static void Down(VisualElement element,int id,float y)
        {
            using (var e = PointerDownEvent.GetPooled(new Touch { fingerId=id-1,position=new Vector2(120,y),phase=TouchPhase.Began })) { e.target=element; element.SendEvent(e); }
        }
        static void Move(VisualElement element,int id,float y)
        {
            using (var e = PointerMoveEvent.GetPooled(new Touch { fingerId=id-1,position=new Vector2(120,y),phase=TouchPhase.Moved })) { e.target=element; element.SendEvent(e); }
        }
        static void Up(VisualElement element,int id,float y)
        {
            using (var e = PointerUpEvent.GetPooled(new Touch { fingerId=id-1,position=new Vector2(120,y),phase=TouchPhase.Ended })) { e.target=element; element.SendEvent(e); }
        }
        static void Submit(VisualElement element)
        {
            Assert.That(element,Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = element; element.SendEvent(e); }
        }
        sealed class Fixture : IDisposable
        {
            public static readonly List<Fixture> Live=new List<Fixture>();
            public readonly string DirectoryPath = Path.Combine(Application.temporaryCachePath,"quick-ui-" + Guid.NewGuid().ToString("N"));
            public readonly Session Session;
            public readonly GameObject Host;
            public readonly PanelSettings Panel;
            public VisualElement Root => Host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture(Language language,int count=3)
            {
                Session = Session.Open(DirectoryPath,language, maximum => 0);
                var names=new[] { "Alex", "Bea", "Chris" };
                for(int i=0;i<count;i++) Session.AddPlayer(i<names.Length ? names[i] : "Person " + (i+1));
                Host = new GameObject("Quick interaction test"); Host.SetActive(false);
                var document = Host.AddComponent<UIDocument>();
                Panel = ScriptableObject.CreateInstance<PanelSettings>(); document.panelSettings = Panel;
                Host.AddComponent<GroupScreen>().Initialize(Session); Host.SetActive(true);
                Root.style.width = 390; Root.style.height = 844;
                Live.Add(this);
            }
            public void Dispose() { if(!Live.Remove(this)) return; UnityEngine.Object.Destroy(Host); UnityEngine.Object.Destroy(Panel); if(Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath,true); }
        }
    }
}


