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
    public class ClassicScreenTests
    {
        readonly List<Fixture> live = new List<Fixture>();
        [TearDown] public void Cleanup() { foreach (var fixture in live) fixture.Dispose(); live.Clear(); }
        [UnityTest] public IEnumerator BaseRoleControlsAllowAtomicSingleSlotReplacementInBothLanguages()
        {
            foreach (var language in new[] { Language.English, Language.German })
            {
                var fixture = new Fixture(language,3); live.Add(fixture); yield return null;
                Assert.That(fixture.Root.Q<Button>("swapRole"), Is.Not.Null, "One-slot groups can replace Undercover with White directly.");
                Submit(fixture.Root.Q<Button>("swapRole")); yield return null;
                Assert.That(fixture.Session.View.WhiteCount, Is.EqualTo(1));
                Assert.That(fixture.Session.View.UndercoverCount, Is.Zero);
                Assert.That(fixture.Root.Q<Label>("roleLimit").text, Does.Contain("1"));
                Assert.That(fixture.Root.Q<Button>("moreWhite").enabledSelf, Is.False);
                Submit(fixture.Root.Q<Button>("automaticUndercover")); yield return null;
                Assert.That(fixture.Session.View.WhiteCount, Is.Zero);
                Assert.That(fixture.Session.View.ManualRoleCounts, Is.False);
                fixture.Dispose();
            }
        }
        [UnityTest] public IEnumerator QuickContinuesAfterACatchAndEachWhiteGetsASecretSafeJudgment()
        {
            foreach(var language in new[]{Language.English,Language.German})
            {
                var fixture=new Fixture(language,7); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("moreWhite")); Submit(fixture.Root.Q<Button>("moreWhite"));
                Submit(fixture.Root.Q<Button>("playButton")); ReadCards(fixture.Session);
                fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); Submit(fixture.Root.Q<Button>("beginVote"));
                var people=fixture.Session.Match.Participants;
                Submit(fixture.Root.Q<Button>("suspect-"+people[0].Id)); Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
                Assert.That(fixture.Root.Q<Label>("eliminatedRole").text,Is.EqualTo("Undercover"));
                Assert.That(fixture.Root.Q<Button>("continueAccusations"),Is.Not.Null,"Quick continues directly, without a new clue round.");
                Submit(fixture.Root.Q<Button>("continueAccusations"));
                Assert.That(fixture.Root.Q<Button>("suspect-"+people[0].Id),Is.Null);
                Submit(fixture.Root.Q<Button>("suspect-"+people[1].Id)); Submit(fixture.Root.Q<Button>("confirmSuspect"));
                Assert.That(fixture.Root.Q<Label>("resultWordCivilian"),Is.Null);
                Submit(fixture.Root.Q<Button>("whiteIncorrect"));
                fixture.Session=Session.Open(fixture.DirectoryPath,Language.German,_=>0); fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); Submit(fixture.Root.Q<Button>("continueAccusations"));
                Submit(fixture.Root.Q<Button>("suspect-"+people[2].Id)); Submit(fixture.Root.Q<Button>("confirmSuspect"));
                Assert.That(fixture.Root.Q<Label>("resultWordCivilian"),Is.Null);
                Submit(fixture.Root.Q<Button>("whiteCorrect")); yield return null;
                Assert.That(fixture.Root.Q<Label>("resultReason").text,Does.Contain(language==Language.German ? "Alle Mr. Whites" : "All Mr. Whites"));
                Assert.That(fixture.Session.Match.Result.WinningRoles,Is.EquivalentTo(new[]{Role.White}));
                fixture.Dispose();
            }
        }
        [UnityTest] public IEnumerator ClassicCanStartFromTheSavedGroupInBothLanguages()
        {
            foreach(var language in new[]{Language.English,Language.German})
            {
                var fixture=new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("classicMode")); yield return null;
                Assert.That(fixture.Root.Q<Button>("playButton").enabledSelf,Is.True,"Four or more people can start Classic.");
                Assert.That(fixture.Root.Q<Button>("moreWhite"),Is.Not.Null);
                Submit(fixture.Root.Q<Button>("moreWhite"));
                Assert.That(fixture.Session.View.DesiredWhiteCount,Is.EqualTo(1));
                var fifth=fixture.Session.View.Players[4].Id;
                Submit(fixture.Root.Q<Button>("edit-"+fifth));
                Submit(fixture.Root.Q<Button>("removePlayer"));
                Assert.That(fixture.Root.Q<Button>("moreWhite").enabledSelf,Is.False);
                Assert.That(fixture.Session.View.DesiredWhiteCount,Is.EqualTo(1),"Saved preference survives falling to four people.");
                Assert.That(fixture.Session.View.WhiteCount,Is.Zero);
                Submit(fixture.Root.Q<Button>("undo"));
                Assert.That(fixture.Session.View.WhiteCount,Is.EqualTo(1),"Returning fifth person restores effective White.");
                Submit(fixture.Root.Q<Button>("playButton")); yield return null;
                Assert.That(fixture.Root.Q<Label>("matchMode").text,Is.EqualTo(language==Language.German ? "Klassisch" : "Classic"));
                Assert.That(fixture.Root.Q<Label>("secretWord").text,Is.Empty);
                fixture.Dispose();
            }
        }
        [UnityTest] public IEnumerator WhitePrivateCardAndSpokenGuessRemainPrivateAcrossResumeInBothLanguages()
        {
            foreach(var language in new[]{Language.English,Language.German})
            {
                var fixture=new Fixture(language); live.Add(fixture); yield return null;
                Submit(fixture.Root.Q<Button>("classicMode")); Submit(fixture.Root.Q<Button>("moreWhite")); Submit(fixture.Root.Q<Button>("playButton")); yield return null;
                for(int seat=0;seat<5;seat++)
                {
                    var hold=fixture.Root.Q<VisualElement>("holdReveal"); Touch(hold,true);
                    var secret=fixture.Root.Q<Label>("secretWord");
                    if(seat==1) Assert.That(secret.text,Does.Contain(language==Language.German ? "kein Wort" : "no word"),"White receives a private explanation.");
                    else Assert.That(secret.text,Does.Not.Contain("Undercover"));
                    Touch(hold,false); Assert.That(secret.text,Is.Empty,"White instructions hide synchronously too.");
                    yield return new WaitForSecondsRealtime(0.2f); Submit(fixture.Root.Q<Button>("nextOwner")); yield return null;
                }
                Submit(fixture.Root.Q<Button>("beginVote"));
                var white=fixture.Session.Match.Participants[1].Id; Submit(fixture.Root.Q<Button>("suspect-"+white)); Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
                Assert.That(fixture.Root.Q<Label>("whiteGuessText"),Is.Not.Null,"Eliminated White gets spoken judgment.");
                Assert.That(fixture.Root.Q<Label>("resultWordCivilian"),Is.Null);
                fixture.Session=Session.Open(fixture.DirectoryPath,Language.German,maximum=>0); fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null;
                Assert.That(fixture.Root.Q<Button>("resumeMatch"),Is.Not.Null); Submit(fixture.Root.Q<Button>("resumeMatch"));
                Submit(fixture.Root.Q<Button>("whiteIncorrect")); yield return null;
                Assert.That(fixture.Session.Match.Round,Is.EqualTo(2)); Assert.That(fixture.Root.Q<Label>("startingPlayer"),Is.Not.Null);
                Submit(fixture.Root.Q<Button>("beginVote")); Assert.That(fixture.Root.Q<Button>("suspect-"+white),Is.Null,"White cannot be selected again.");
                Submit(fixture.Root.Q<Button>("suspect-"+fixture.Session.Match.Participants[0].Id)); Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
                Assert.That(fixture.Root.Q<Label>("resultTitle").text,Is.EqualTo(language==Language.German ? "Die Bürger gewinnen!" : "Civilians win!"));
                Submit(fixture.Root.Q<Button>("rematch")); yield return null; Assert.That(fixture.Session.Match.Survivors.Count,Is.EqualTo(5));
                fixture.Dispose();
            }
        }
        [UnityTest] public IEnumerator EliminationsAndRepeatedTiesKeepOnlySurvivorsOnAShortScreen()
        {
            var fixture=new Fixture(Language.German,20); live.Add(fixture);
            fixture.Session.SetMode(GameMode.Classic); fixture.Session.SetWhitePreference(true); fixture.Session.StartMatch(); ReadCards(fixture.Session);
            fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); fixture.Root.style.height=640; yield return null;
            Submit(fixture.Root.Q<Button>("resumeMatch")); Submit(fixture.Root.Q<Button>("beginVote"));
            var eliminated=fixture.Session.Match.Participants[0].Id;
            Submit(fixture.Root.Q<Button>("suspect-"+eliminated)); Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
            Assert.That(fixture.Root.Q<Label>("eliminatedRole").text,Is.EqualTo("Undercover"));
            Assert.That(fixture.Root.Q<Label>("resultWordCivilian"),Is.Null);
            Submit(fixture.Root.Q<Button>("matchBack")); Assert.That(fixture.Root.Q<Button>("resumeMatch"),Is.Not.Null,"Back pauses after irreversible elimination.");
            Submit(fixture.Root.Q<Button>("resumeMatch")); Submit(fixture.Root.Q<Button>("continueRound")); Submit(fixture.Root.Q<Button>("beginVote")); yield return null;
            Assert.That(fixture.Root.Q<Button>("suspect-"+eliminated),Is.Null);
            var list=fixture.Root.Q<ScrollView>("suspects"); var last=list.ElementAt(18); list.ScrollTo(last); yield return null;
            Assert.That(last.worldBound.yMax,Is.LessThanOrEqualTo(list.contentViewport.worldBound.yMax+1));
            Submit(last); Submit(fixture.Root.Q<Button>("changeSuspect")); Submit(fixture.Root.Q<Button>("recordTie")); yield return null;
            Assert.That(fixture.Root.Q<Button>("recordTie").worldBound.yMax,Is.LessThanOrEqualTo(640));
            Submit(fixture.Root.Q<Button>("recordTie")); yield return null;
            Assert.That(fixture.Session.Match.Round,Is.EqualTo(3)); Assert.That(fixture.Session.Match.Survivors.Count,Is.EqualTo(19));
            Assert.That(fixture.Root.Q<Label>("clueTitle").text,Is.EqualTo("Hinweisrunde 3."));
        }
        [UnityTest] public IEnumerator WhiteAloneAndBothSurvivingAdversaryRolesAreNamedInTheResult()
        {
            foreach(bool correctWhite in new[]{true,false})
            {
                var fixture=new Fixture(Language.English); live.Add(fixture);
                fixture.Session.SetMode(GameMode.Classic); fixture.Session.SetWhitePreference(true); fixture.Session.StartMatch(); ReadCards(fixture.Session);
                fixture.Host.GetComponent<GroupScreen>().Initialize(fixture.Session); yield return null;
                Submit(fixture.Root.Q<Button>("resumeMatch")); Submit(fixture.Root.Q<Button>("beginVote"));
                Submit(fixture.Root.Q<Button>("suspect-"+fixture.Session.Match.Participants[correctWhite ? 1 : 2].Id)); Submit(fixture.Root.Q<Button>("confirmSuspect")); yield return null;
                if(correctWhite) {
                    Submit(fixture.Root.Q<Button>("matchBack")); Assert.That(fixture.Root.Q<Button>("resumeMatch"),Is.Not.Null,"Back pauses pending White judgment.");
                    Submit(fixture.Root.Q<Button>("resumeMatch")); Submit(fixture.Root.Q<Button>("whiteCorrect"));
                }
                else {
                    Submit(fixture.Root.Q<Button>("continueRound")); Submit(fixture.Root.Q<Button>("beginVote"));
                    Submit(fixture.Root.Q<Button>("suspect-"+fixture.Session.Match.Participants[3].Id)); Submit(fixture.Root.Q<Button>("confirmSuspect"));
                }
                yield return null;
                Assert.That(fixture.Root.Q<Label>("resultTitle").text,Is.EqualTo(correctWhite ? "Mr. White wins!" : "Undercover & Mr. White win!"));
                Assert.That(fixture.Root.Q<Label>("resultWordCivilian").text,Is.Not.Empty);
                fixture.Dispose();
            }
        }
        static void ReadCards(Session session) { while(session.Match.Phase==MatchPhase.Handoff) { var id=session.Match.Owner.Id; session.RevealWord(id); session.HideWord(); session.AdvanceHandoff(id); } }
        static void Touch(VisualElement element,bool down)
        {
            var touch=new Touch { fingerId=0,position=new Vector2(120,300),phase=down ? TouchPhase.Began : TouchPhase.Ended };
            if(down) { using(var e=PointerDownEvent.GetPooled(touch)) { e.target=element; element.SendEvent(e); } }
            else { using(var e=PointerUpEvent.GetPooled(touch)) { e.target=element; element.SendEvent(e); } }
        }
        static void Submit(VisualElement element)
        {
            Assert.That(element,Is.Not.Null);
            using(var e=NavigationSubmitEvent.GetPooled()) { e.target=element; element.SendEvent(e); }
        }
        sealed class Fixture : IDisposable
        {
            public readonly string DirectoryPath=Path.Combine(Application.temporaryCachePath,"classic-ui-"+Guid.NewGuid().ToString("N"));
            public Session Session;
            public readonly GameObject Host;
            readonly PanelSettings panel;
            bool disposed;
            public VisualElement Root=>Host.GetComponent<UIDocument>().rootVisualElement;
            public Fixture(Language language,int count=5)
            {
                Session=Session.Open(DirectoryPath,language,maximum=>0);
                var names=new[]{"Alex","Bea","Chris","Dana","Eli"};
                for(int i=0;i<count;i++) Session.AddPlayer(i<names.Length ? names[i] : "Person "+i);
                Host=new GameObject("Classic interaction test"); Host.SetActive(false);
                panel=ScriptableObject.CreateInstance<PanelSettings>(); Host.AddComponent<UIDocument>().panelSettings=panel;
                Host.AddComponent<GroupScreen>().Initialize(Session); Host.SetActive(true);
                Root.style.width=390; Root.style.height=844;
            }
            public void Dispose() { if(disposed) return; disposed=true; UnityEngine.Object.Destroy(Host); UnityEngine.Object.Destroy(panel); if(Directory.Exists(DirectoryPath)) Directory.Delete(DirectoryPath,true); }
        }
    }
}
