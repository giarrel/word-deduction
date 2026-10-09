using Newtonsoft.Json;
using WordDeduction;

static class RoleCountCases
{
    public static readonly (string name, Action<string> run)[] All = {
        ("free counts allow an atomic pure White replacement at minimum size", directory => {
            foreach (var example in new[] { (GameMode.Quick,3), (GameMode.Classic,4) }) {
                string path=Path.Combine(directory,example.Item1.ToString());
                var session=Group(path,example.Item1,example.Item2);
                Check(session.SetRoleCounts(0,1).Success,"one occupied slot can change role atomically");
                session=Session.Open(path,Language.German,_=>0);
                Check(session.View.UndercoverCount==0 && session.View.WhiteCount==1 && session.View.ReadyToStart,"pure White configuration persists and is playable");
                Check(session.StartMatch().Success,"pure White deal starts");
                var id=session.Match.Id; session=Session.Open(path,Language.German,_=>0);
                Check(session.Match?.Id==id && session.View.StorageNotice==null,"new distribution reopens without recovery");
                var cards=ReadCards(session);
                Check(cards.Count(c=>c.Kind==PrivateCardKind.White)==1 && cards.Where(c=>c.Kind==PrivateCardKind.Word).Select(c=>c.Word).Distinct().Count()==1,"one White, all other cards have the same word");
            }
        }),
        ("free counts preferences survive modes shrink undo growth Auto and failed writes", directory => {
            var s=Group(directory,GameMode.Quick,9);
            Check(s.SetRoleCounts(3,1).Success,"desired four adversaries saves");
            s.SetMode(GameMode.Classic); Check(s.SetRoleCounts(0,4).Success,"independent pure White preference saves");
            s.SetMode(GameMode.Kings); s.SetKingsUndercoverPreference(2); s.SetMode(GameMode.Quick);
            var removed=s.View.Players.Last(); s.RemovePlayer(removed.Id);
            Check(s.View.RoleCountsAdjusted && s.View.UndercoverCount==2 && s.View.WhiteCount==1 && s.View.DesiredUndercoverCount==3,"eight-person mix preserves one of each and desired wish");
            Check(s.UndoRemove().Success && s.View.UndercoverCount==3,"undo restores desired mix");
            for(int i=0;i<6;i++) s.RemovePlayer(s.View.Players.Last().Id);
            s=Session.Open(directory,Language.German,_=>0);
            Check(s.View.UndercoverCount==1 && s.View.WhiteCount==0 && s.View.DesiredWhiteCount==1,"one-slot shrink uses Undercover and retains White intention");
            s.SetMode(GameMode.Classic);
            Check(s.View.UndercoverCount==0 && s.View.WhiteCount==1 && s.View.DesiredWhiteCount==4,"pure White stays pure during shrink");
            for(int i=0;i<6;i++) s.AddPlayer("New "+i);
            Check(s.View.WhiteCount==4 && !s.View.RoleCountsAdjusted,"growth restores independent intention");
            Directory.CreateDirectory(Path.Combine(directory,"session.pending.json"));
            Check(s.SetRoleCounts(1,2).Error=="SaveFailed" && s.View.WhiteCount==4,"failed setting never changes memory");
            Check(Session.Open(directory,Language.English).View.WhiteCount==4,"failed setting never changes disk");
            Directory.Delete(Path.Combine(directory,"session.pending.json"));
            Check(s.SetRoleCounts(null,null).Success && !s.View.ManualRoleCounts && s.View.UndercoverCount==2 && s.View.WhiteCount==0,"Auto clears only current override");
            s.SetMode(GameMode.Quick); Check(s.View.UndercoverCount==3 && s.View.WhiteCount==1,"Quick preference intact");
            s.SetMode(GameMode.Kings); Check(s.View.KingsUndercoverPreference==2,"Kings preference intact");
        }),
        ("free counts write a new schema and reject new fields disguised as old", directory => {
            var s=Group(directory,GameMode.Quick,5); s.SetRoleCounts(0,2); s.StartMatch();
            var path=Path.Combine(directory,"session.json");
            var envelope=Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
            Check((int)envelope["Version"]==6,"new semantics use V6 so old apps block rather than misread");
            foreach(int version in new[]{1,2,3,4,5}) {
                envelope["Version"]=version; File.WriteAllText(path,envelope.ToString());
                var reopened=Session.Open(directory,Language.English);
                Check(reopened.View.StorageNotice=="RecoveredBackup" && reopened.Match==null,"lower schema rejects fields and deal rules it never supported");
            }
        }),
        ("free counts reject invalid commands and malformed saved preferences", directory => {
            var s=Group(directory,GameMode.Quick,5);
            foreach(var pair in new[]{(0,0),(-1,1),(1,-1),(2,1),(int.MaxValue,int.MaxValue)})
                Check(!s.SetRoleCounts(pair.Item1,pair.Item2).Success,"invalid complete pair rejected");
            Check(!s.SetRoleCounts(null,1).Success && !s.SetRoleCounts(1,null).Success,"partial Auto rejected");
            s.SetRoleCounts(0,2); s.StartMatch();
            string match=JsonConvert.SerializeObject(s.Match);
            Check(s.SetRoleCounts(1,0).Error=="MatchInProgress" && s.SetRoleCounts(null,null).Error=="MatchInProgress" && JsonConvert.SerializeObject(s.Match)==match,"live settings cannot alter frozen deal");
            s.AbandonMatch(s.Match.Id);
            SavedSessionFixture.RewritePrimary(directory,payload=>payload["QuickRoles"]["White"]=-1);
            s=Session.Open(directory,Language.English);
            Check(s.View.StorageNotice=="RecoveredBackup","malformed preference cannot be trusted even with a valid checksum");
        }),
        ("sequential Quick requires every adversary without another clue round", directory => {
            var s=Group(directory,GameMode.Quick,7); s.SetRoleCounts(2,1); s.StartMatch();
            var cards=ReadCards(s); var people=s.Match.Participants; s.BeginVote();
            s.SelectSuspect(people[0].Id); Check(s.ConfirmSuspect(people[0].Id).Success,"first selected Undercover confirmed");
            Check(s.Match.Phase==MatchPhase.Elimination && s.Match.Elimination.Role==Role.Undercover && s.Match.Result==null,"first catch only reveals that role");
            var oldId=s.Match.Id; s=Session.Open(directory,Language.English,_=>0);
            Check(s.Match?.Id==oldId && s.Match.Survivors.Count==6,"first elimination resumes");
            Check(s.ContinueAccusations(oldId,people[0].Id).Success && s.Match.Phase==MatchPhase.Vote && s.Match.Round==1,"continues straight to another accusation");
            Check(!s.ContinueAccusations(oldId,people[0].Id).Success && !s.SelectSuspect(people[0].Id).Success,"no duplicate continuation or recatching");
            s.SelectSuspect(people[1].Id); s.ConfirmSuspect(people[1].Id); s.ContinueAccusations(oldId,people[1].Id);
            s.SelectSuspect(people[2].Id); s.ConfirmSuspect(people[2].Id);
            Check(s.Match.Phase==MatchPhase.WhiteGuess && s.Match.Result==null,"last White guesses before the apparent good win");
            s=Session.Open(directory,Language.English,_=>0);
            Check(cards.Where(c=>c.Word!=null).All(c=>!JsonConvert.SerializeObject(s.Match).Contains(c.Word)),"no target or other word leaks in pending judgment");
            Check(s.ResolveWhiteGuess(people[2].Id,false).Success && s.Match.Result.Reason==Outcome.AllAdversariesEliminated && s.Match.Result.WinningRoles.SequenceEqual(new[]{Role.Civilian}),"all caught and final White failed gives good team victory");
            Check(Session.Open(directory,Language.English).Match.Result.Reason==Outcome.AllAdversariesEliminated,"new Quick result reopens");
            Check(s.Rematch(oldId).Success && s.Match.Survivors.Count==7,"rematch returns everyone with same settings");
        }),
        ("sequential Quick mistakes and repeated ties award all adversary roles in the deal", directory => {
            foreach(var scenario in new[]{"mixed-mistake","pure-white-mistake","mixed-tie"}) {
                var s=Group(Path.Combine(directory,scenario),GameMode.Quick,5);
                bool pure=scenario=="pure-white-mistake"; s.SetRoleCounts(pure?0:1,pure?2:1); s.StartMatch(); ReadCards(s);
                var people=s.Match.Participants; s.BeginVote(); s.SelectSuspect(people[0].Id); s.ConfirmSuspect(people[0].Id);
                if(pure) s.ResolveWhiteGuess(people[0].Id,false);
                s.ContinueAccusations(s.Match.Id,people[0].Id);
                if(scenario.EndsWith("tie")) { s.RecordTie(false); s.RecordTie(true); }
                else { s.SelectSuspect(people[2].Id); s.ConfirmSuspect(people[2].Id); }
                var expected=pure ? new[]{Role.White} : new[]{Role.Undercover,Role.White};
                Check(s.Match.Result.WinningRoles.SequenceEqual(expected),"a caught teammate shares mistake/tie victory, and an absent role never wins");
                Check(Session.Open(Path.Combine(directory,scenario),Language.English).Match.Result.WinningRoles.SequenceEqual(expected),"winner set persists");
            }
        }),
        ("multiple Whites each guess once and all Whites alone share a correct guess", directory => {
            foreach(var mode in new[]{GameMode.Quick,GameMode.Classic}) foreach(bool pure in new[]{false,true}) foreach(int correctAt in new[]{0,1,-1}) {
                string path=Path.Combine(directory,mode+"-"+pure+"-"+correctAt); var s=Group(path,mode,7);
                s.SetRoleCounts(pure?0:1,2); s.StartMatch(); var cards=ReadCards(s); var people=s.Match.Participants;
                string[] whites=cards.Where(c=>c.Kind==PrivateCardKind.White).Select(c=>c.Owner.Id).ToArray();
                for(int i=0;i<2;i++) {
                    if(s.Match.Phase==MatchPhase.Clues) s.BeginVote();
                    s.SelectSuspect(whites[i]); Check(s.ConfirmSuspect(whites[i]).Success && s.Match.Phase==MatchPhase.WhiteGuess,"each caught White immediately guesses");
                    s=Session.Open(path,Language.German,_=>0);
                    Check(s.Match.Phase==MatchPhase.WhiteGuess && s.Match.Elimination.Participant.Id==whites[i],"pending identity survives reopen");
                    Check(cards.Where(c=>c.Word!=null).All(c=>!JsonConvert.SerializeObject(s.Match).Contains(c.Word)),"solutions stay secret across multiple judgments");
                    var before=JsonConvert.SerializeObject(s.Match); Directory.CreateDirectory(Path.Combine(path,"session.pending.json"));
                    Check(s.ResolveWhiteGuess(whites[i],i==correctAt).Error=="SaveFailed" && JsonConvert.SerializeObject(s.Match)==before,"failed judgment cannot reveal or half-apply outcome");
                    Check(JsonConvert.SerializeObject(Session.Open(path,Language.English).Match)==before,"failed judgment leaves exact durable pending state");
                    Directory.Delete(Path.Combine(path,"session.pending.json"));
                    Check(s.ResolveWhiteGuess(whites[i],i==correctAt).Success && !s.ResolveWhiteGuess(whites[i],true).Success,"judgment commits once only");
                    if(i==correctAt) {
                        Check(s.Match.Result.WinningRoles.SequenceEqual(new[]{Role.White}) && s.Match.Result.Roles.Count(r=>r.Role==Role.White)==2,"all White teammates win including the previously failed White; UC excluded"); break;
                    }
                    if(s.Match.Phase==MatchPhase.Elimination) s.ContinueAccusations(s.Match.Id,whites[i]);
                }
                if(correctAt==-1) {
                    if(!pure) {
                        if(s.Match.Phase==MatchPhase.Clues) s.BeginVote();
                        s.SelectSuspect(people[0].Id); s.ConfirmSuspect(people[0].Id);
                    }
                    Check(s.Match.Result.Reason==Outcome.AllAdversariesEliminated && s.Match.Result.Winner==Role.Civilian,"failed final White evaluates end before any continuation");
                }
                var result=JsonConvert.SerializeObject(s.Match);
                Check(JsonConvert.SerializeObject(Session.Open(path,Language.English).Match)==result,"each multiple-White ending reopens exactly");
            }
        }),
        ("free counts deal and reopen every supported base-mode composition", directory => {
            // Independently enumerated capacities for sizes3..20 from the accepted majority examples.
            int[] capacities={1,1,2,2,3,3,4,4,5,5,6,6,7,7,8,8,9,9};
            foreach(var mode in new[]{GameMode.Quick,GameMode.Classic}) for(int count=mode==GameMode.Quick?3:4;count<=20;count++) {
                string path=Path.Combine(directory,mode+"-"+count); var s=Group(path,mode,count);
                int maximum=capacities[count-3];
                Check(s.View.AdversaryLimit==maximum,"shared public cap matches accepted literal size table");
                for(int total=1;total<=maximum;total++) for(int uc=0;uc<=total;uc++) {
                    int white=total-uc;
                    Check(s.SetRoleCounts(uc,white).Success && s.StartMatch().Success,"every valid pair deals");
                    var id=s.Match.Id; s=Session.Open(path,Language.English,_=>0);
                    Check(s.View.StorageNotice==null && s.Match?.Id==id,"each combination reopens same frozen deal");
                    var cards=ReadCards(s); var good=cards.Last().Word;
                    Check(cards.Count(c=>c.Kind==PrivateCardKind.White)==white && cards.Count(c=>c.Word!=null && c.Word!=good)==uc && cards.Count(c=>c.Word==good)==count-total,"public private-card distribution matches selected pair");
                    Check(Session.Open(path,Language.English).Match.Phase==MatchPhase.Clues,"completed handoff also persists");
                    s.AbandonMatch(id);
                }
            }
        }),
        ("V5 finalized Quick results keep their old frozen meaning and bytes", directory => {
            foreach(var ending in new[]{Outcome.CaughtUndercover,Outcome.AccusedCivilian,Outcome.RepeatedTie}) {
                string path=Path.Combine(directory,ending.ToString()); var s=Group(path,GameMode.Quick,3); s.StartMatch(); ReadCards(s); s.BeginVote();
                if(ending==Outcome.RepeatedTie) { s.RecordTie(false); s.RecordTie(true); }
                else { var id=s.Match.Participants[ending==Outcome.CaughtUndercover?0:1].Id; s.SelectSuspect(id); s.ConfirmSuspect(id); }
                SavedSessionFixture.RewritePrimary(path,payload=> {
                    payload.Remove("QuickRoles"); payload.Remove("ClassicRoles");
                    var match=(Newtonsoft.Json.Linq.JObject)payload["Match"]; match.Remove("RulesVersion"); match["Outcome"]=(int)ending;
                    foreach(var person in match["Participants"]) person["Eliminated"]=false;
                });
                var primary=Path.Combine(path,"session.json"); var envelope=Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(primary)); envelope["Version"]=5; File.WriteAllText(primary,envelope.ToString());
                string bytes=File.ReadAllText(primary); s=Session.Open(path,Language.German,_=>throw new Exception("legacy open must not redraw"));
                Check(s.View.StorageNotice==null && s.Match.Result.Reason==ending && s.Match.Survivors.Count==3,"V5 non-eliminating Quick result is still a valid frozen result");
                Check(s.Match.Result.Winner==(ending==Outcome.CaughtUndercover?Role.Civilian:Role.Undercover),"legacy winner retains meaning");
                Check(File.ReadAllText(primary)==bytes,"open performs no rewrite");
                var result=JsonConvert.SerializeObject(s.Match); s.SetRoleCounts(0,1);
                Check(JsonConvert.SerializeObject(Session.Open(path,Language.English).Match)==result,"first V6 settings save does not reinterpret old result");
            }
        }),
        ("sequential Quick failure and stale actions preserve confirmation and continuation", directory => {
            var s=Group(directory,GameMode.Quick,5); s.SetRoleCounts(2,0); s.StartMatch(); ReadCards(s); s.BeginVote();
            var a=s.Match.Participants[0].Id; var b=s.Match.Participants[1].Id; s.SelectSuspect(a); s.SelectSuspect(b);
            Check(!s.ConfirmSuspect(a).Success && s.Match.Elimination==null,"stale selected identity never eliminates");
            string before=JsonConvert.SerializeObject(s.Match); var pending=Path.Combine(directory,"session.pending.json"); Directory.CreateDirectory(pending);
            Check(s.ConfirmSuspect(b).Error=="SaveFailed" && JsonConvert.SerializeObject(s.Match)==before,"failed confirmation discloses nothing");
            Check(JsonConvert.SerializeObject(Session.Open(directory,Language.English).Match)==before,"selection is still durable");
            Directory.Delete(pending); s.ConfirmSuspect(b); before=JsonConvert.SerializeObject(s.Match); Directory.CreateDirectory(pending);
            Check(s.ContinueAccusations(s.Match.Id,b).Error=="SaveFailed" && JsonConvert.SerializeObject(s.Match)==before,"failed acknowledgment keeps public notice");
            Check(!s.ContinueAccusations("old-match",b).Success && !s.ContinueAccusations(s.Match.Id,a).Success && !s.ContinueRound(1).Success,"wrong match, person and Classic continuation rejected");
            Directory.Delete(pending); s.ContinueAccusations(s.Match.Id,b); s.RecordTie(false); s.RecordTie(true);
            Check(s.Match.Result.Reason==Outcome.RepeatedTie,"second tie still ends sequential Quick");
        }),
        ("sequential vote commands reject a previous accusation and another match", directory => {
            var s=Group(directory,GameMode.Quick,5); s.SetRoleCounts(2,0); s.StartMatch(); ReadCards(s); s.BeginVote();
            var before=s.Match; var a=before.Participants[0].Id; var b=before.Participants[1].Id;
            s.SelectSuspect(a); s.ConfirmSuspect(a); s.ContinueAccusations(before.Id,a);
            Check(!s.SelectSuspect(before.Id,1,0,b).Success && !s.RecordTie(before.Id,1,0,false).Success,"stale progress cannot select or tie the next accusation");
            Check(s.SelectSuspect(before.Id,1,1,b).Success,"current progress selects");
            Check(!s.ConfirmSuspect("other-match",1,1,b).Success && !s.CancelSuspect(before.Id,1,0).Success,"wrong match and old cancel cannot change current selection");
            Check(s.ConfirmSuspect(before.Id,1,1,b).Success,"current identity and progress confirms");
        })
    };
    internal static Session Group(string path,GameMode mode,int count) {
        var session=Session.Open(path,Language.English,_=>0);
        for(int i=0;i<count;i++) Check(session.AddPlayer("Person "+i).Success,"fixture person saved");
        Check(session.SetMode(mode).Success,"fixture mode saved"); return session;
    }
    internal static List<PrivateCardView> ReadCards(Session session) {
        var cards=new List<PrivateCardView>();
        while(session.Match.Phase==MatchPhase.Handoff) {
            var id=session.Match.Owner.Id; cards.Add(session.RevealCard(id)); session.HideWord();
            Check(session.AdvanceHandoff(id).Success,"handoff advances");
        }
        return cards;
    }
    internal static void Check(bool condition,string message) { if(!condition) throw new Exception(message); }
}
