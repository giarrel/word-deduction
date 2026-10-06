using WordDeduction;

static class ClassicCases
{
    public static (string name, Action<string> run)[] All = {
        ("Classic deals the automatic mix and retains the optional White preference", directory => {
            foreach (var scenario in new[] { (4,1,0), (5,1,1), (6,1,1), (7,1,1), (8,2,1), (9,2,1), (10,2,1), (11,2,1), (12,2,1), (13,3,1), (14,3,1), (15,3,1), (16,3,1), (17,3,1), (18,3,1), (19,3,1), (20,3,1) }) {
                var folder = Path.Combine(directory, scenario.Item1.ToString());
                var session = Group(folder,scenario.Item1);
                Check(!session.View.WhitePreferred,"White begins off");
                session.StartMatch(); var withoutWhite=ReadCards(session);
                Check(!withoutWhite.Contains("Mr. White") && withoutWhite[0]!=withoutWhite[^1] && withoutWhite.Count(w=>w==withoutWhite[0])==scenario.Item2,"default Classic mix has no White at every supported size");
                session.AbandonMatch(session.Match.Id);
                Check(session.SetWhitePreference(true).Success,"White preference saves");
                session = Session.Open(folder,Language.German, maximum => 0);
                Check(session.View.WhitePreferred && session.View.WhiteCount == scenario.Item3 && session.View.UndercoverCount == scenario.Item2,"effective mix and preference survive reopening");
                Check(session.StartMatch().Success,"Classic starts");
                var cards = ReadCards(session);
                Check(cards.Count(w => w == "Mr. White") == scenario.Item3,"White has no word");
                Check(cards[0]!=cards[^1] && cards.Count(w => w == cards[0]) == scenario.Item2,"Undercover count matches mix");
                Check(cards.Count(w => w == cards[^1]) == scenario.Item1-scenario.Item2-scenario.Item3,"remaining people are Civilians");
                Check(!session.SetWhitePreference(false).Success,"live deal preferences frozen");
                session.AbandonMatch(session.Match.Id); session.SetMode(GameMode.Quick); session.StartMatch();
                var quick = ReadCards(session);
                Check(quick.Count(w => w == quick[0]) == 1 && !quick.Contains("Mr. White"),"Quick ignores retained White preference");
                Check(session.View.WhitePreferred,"Quick never deletes Classic preference");
            }
        }),
        ("Classic eliminates survivors only and continues until all adversaries are found", directory => {
            var session=Group(directory,8); session.StartMatch(); ReadCards(session);
            var people=session.Match.Participants;
            Eliminate(session,people[0].Id);
            Check(session.Match.Phase==MatchPhase.Elimination && session.Match.Elimination.Role==Role.Undercover,"eliminated role displayed");
            Check(session.Match.Result==null && session.Match.Survivors.Count==7,"words hidden while seven survive");
            session=Session.Open(directory,Language.German, maximum=>0);
            Check(session.Match.Elimination.Participant.Id==people[0].Id,"elimination persists");
            Check(session.ContinueRound(1).Success && session.Match.Round==2,"next clue round starts");
            Check(!session.ContinueRound(1).Success,"duplicate next cannot skip a round");
            Check(session.Match.StartingPlayer.Id==people[1].Id,"new starter comes from survivors");
            session.BeginVote(); Check(!session.SelectSuspect(people[0].Id).Success,"eliminated participant cannot be selected again");
            session.SelectSuspect(people[1].Id); session.ConfirmSuspect(people[1].Id);
            Check(session.Match.Result.WinningRoles.SequenceEqual(new[]{Role.Civilian}) && session.Match.Result.Reason==Outcome.AllAdversariesEliminated,"finding all Undercover ends the match");
            Check(session.View.ActiveCount==8,"elimination never pauses saved group members");
            var oldId=session.Match.Id; session.Rematch(oldId);
            Check(session.Match.Survivors.Count==8 && session.Match.Round==1,"rematch restores all active group members");
        }),
        ("Classic names exactly the surviving adversary roles at one Civilian left", directory => {
            foreach(var remove in new[]{"none","undercover","white"}) {
                var session=Group(Path.Combine(directory,remove),5); session.SetWhitePreference(true); session.StartMatch(); ReadCards(session);
                var people=session.Match.Participants;
                if(remove=="undercover") { Eliminate(session,people[0].Id); session.ContinueRound(session.Match.Round); }
                if(remove=="white") { Eliminate(session,people[1].Id); Check(session.Match.Phase==MatchPhase.WhiteGuess,"White must guess before resolution"); session.ResolveWhiteGuess(people[1].Id,false); }
                Eliminate(session,people[2].Id); session.ContinueRound(session.Match.Round); Eliminate(session,people[3].Id);
                var expected=remove=="none" ? new[]{Role.Undercover,Role.White} : remove=="undercover" ? new[]{Role.White} : new[]{Role.Undercover};
                Check(session.Match.Result.Reason==Outcome.OneCivilianRemains && session.Match.Result.WinningRoles.SequenceEqual(expected),"only living adversary sides win");
            }
        }),
        ("White gets one durable spoken guess before any terminal evaluation", directory => {
            foreach(bool correct in new[]{true,false}) {
                var folder=Path.Combine(directory,correct.ToString()); var session=Group(folder,5); session.SetWhitePreference(true); session.StartMatch();
                var privateWords=ReadCards(session).Where(word=>word!="Mr. White").Distinct().ToArray();
                var people=session.Match.Participants;
                Eliminate(session,people[0].Id); session.ContinueRound(1);
                Eliminate(session,people[1].Id);
                Check(session.Match.Phase==MatchPhase.WhiteGuess && session.Match.Result==null,"last adversary still gets the guess before Civilian win");
                session=Session.Open(folder,Language.German,maximum=>0);
                Check(session.Match.Phase==MatchPhase.WhiteGuess && session.Match.Language==Language.English && session.Match.Elimination.Role==Role.White,"pending guess resumes unchanged");
                string visible=Newtonsoft.Json.JsonConvert.SerializeObject(session.Match);
                Check(privateWords.All(word=>!visible.Contains(word)),"target remains private");
                Check(!session.ResolveWhiteGuess(people[0].Id,true).Success,"wrong identity cannot resolve the guess");
                Check(session.ResolveWhiteGuess(people[1].Id,correct).Success,"spoken judgment saves");
                Check(session.Match.Result.WinningRoles.SequenceEqual(new[]{correct ? Role.White : Role.Civilian}),"correct White wins alone; wrong final White loses");
                Check(!session.ResolveWhiteGuess(people[1].Id,!correct).Success,"guess may not be reversed after disclosure");
            }
        }),
        ("Classic has one runoff then a new clue round with no elimination and any survivor may start", directory => {
            for(int starter=0;starter<5;starter++) {
                int draw=0; var folder=Path.Combine(directory,starter.ToString());
                var session=Group(folder,5); session.SetWhitePreference(true);
                session=Session.Open(folder,Language.English,maximum => draw++ < 2 ? 0 : Math.Min(starter,maximum-1));
                session.StartMatch(); ReadCards(session); var ids=session.Match.Survivors.Select(p=>p.Id).ToArray();
                Check(session.Match.StartingPlayer.Id==ids[starter],"initial random starter includes White");
                session.BeginVote(); session.SelectSuspect(ids[2]); session.RecordTie(false);
                Check(session.Match.Runoff && session.Match.SelectedSuspect==null,"runoff clears unconfirmed suspect");
                Check(!session.RecordTie(false).Success,"duplicate first tie rejected");
                session=Session.Open(folder,Language.German,maximum=>starter);
                Check(session.RecordTie(true).Success && session.Match.Phase==MatchPhase.Clues && session.Match.Round==2,"second tie continues Classic");
                Check(session.Match.Survivors.Select(p=>p.Id).SequenceEqual(ids) && session.Match.StartingPlayer.Id==ids[starter],"no random elimination or role-based starter exclusion");
                Check(!session.Match.Runoff && session.Match.SelectedSuspect==null && session.Match.Result==null,"new round has no stale vote or exposed result");
            }
        }),
        ("invalid Classic progress is rejected even with a valid checksum", directory => {
            foreach(var corruption in new[]{"eliminated suspect","eliminated starter","terminal alive state","invalid round","Quick elimination"}) {
                var folder=Path.Combine(directory,corruption); var session=Group(folder,5); session.StartMatch(); ReadCards(session); session.BeginVote();
                var path=Path.Combine(folder,"session.json");
                var envelope=Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
                var payload=Newtonsoft.Json.Linq.JObject.Parse((string)envelope["Payload"]); var match=payload["Match"];
                if(corruption=="eliminated suspect") { match["Suspect"]=match["Participants"][2]["Id"].DeepClone(); match["Participants"][2]["Eliminated"]=true; }
                if(corruption=="eliminated starter") match["Participants"][0]["Eliminated"]=true;
                if(corruption=="terminal alive state") { match["Participants"][0]["Eliminated"]=true; match["StartingIndex"]=1; }
                if(corruption=="invalid round") match["Round"]=0;
                if(corruption=="Quick elimination") { match["Mode"]=0; match["Participants"][2]["Eliminated"]=true; }
                var json=payload.ToString(Newtonsoft.Json.Formatting.None); envelope["Payload"]=json; envelope["Checksum"]=Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json)));
                File.WriteAllText(path,envelope.ToString()); session=Session.Open(folder,Language.English);
                Check(session.View.StorageNotice=="RecoveredBackup" && session.Match.Phase==MatchPhase.Clues,"invalid "+corruption+" recovers prior valid state");
            }
        }),
        ("Classic failed elimination and White judgment preserve both memory and saved progress", directory => {
            var session=Group(directory,5); session.SetWhitePreference(true); session.StartMatch(); ReadCards(session);
            var white=session.Match.Participants[1].Id; session.BeginVote(); session.SelectSuspect(white);
            var pending=Path.Combine(directory,"session.pending.json"); Directory.CreateDirectory(pending);
            Check(session.ConfirmSuspect(white).Error=="SaveFailed" && session.Match.Elimination==null && session.Match.Survivors.Count==5,"failed confirmation discloses no role");
            Check(Session.Open(directory,Language.German).Match.SelectedSuspect.Id==white,"saved suspect is still unconfirmed");
            Directory.Delete(pending); session.ConfirmSuspect(white); Directory.CreateDirectory(pending);
            Check(session.ResolveWhiteGuess(white,true).Error=="SaveFailed" && session.Match.Phase==MatchPhase.WhiteGuess && session.Match.Result==null,"failed White judgment exposes no result");
            Check(Session.Open(directory,Language.German).Match.Phase==MatchPhase.WhiteGuess,"pending guess retained on disk");
            Directory.Delete(pending); Check(session.ResolveWhiteGuess(white,false).Success && session.Match.Phase==MatchPhase.Clues,"wrong judgment retries and survivors continue");
            Check(session.Match.Survivors.Count==4 && session.Match.Round==2,"White stays eliminated");
        }),
        ("V2 live Quick import preserves the frozen deal and remains covered", directory => {
            var session=Group(directory,4); session.SetMode(GameMode.Quick); session.StartMatch();
            var id=session.Match.Id; var owner=session.Match.Owner.Id; var word=session.RevealWord(owner);
            var path=Path.Combine(directory,"session.json"); var envelope=Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
            var payload=Newtonsoft.Json.Linq.JObject.Parse((string)envelope["Payload"]); payload.Remove("WhitePreferred");
            ((Newtonsoft.Json.Linq.JObject)payload["Match"]).Remove("Round"); foreach(var participant in payload["Match"]["Participants"]) ((Newtonsoft.Json.Linq.JObject)participant).Remove("Eliminated");
            var json=payload.ToString(Newtonsoft.Json.Formatting.None); envelope["Version"]=2; envelope["Payload"]=json; envelope["Checksum"]=Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json)));
            File.WriteAllText(path,envelope.ToString()); var old=File.ReadAllText(path);
            session=Session.Open(directory,Language.German,maximum=>maximum-1);
            Check(session.Match.Id==id && session.Match.Owner.Id==owner && !session.Match.CanAdvance && session.Match.Round==1 && !session.View.WhitePreferred,"legacy progress imports covered with new defaults");
            Check(session.RevealWord(owner)==word && File.ReadAllText(path)==old,"migration neither redraws nor writes on open");
            session.HideWord(); session.AdvanceHandoff(owner); session=Session.Open(directory,Language.German);
            Check(session.Match.Id==id && session.Match.HandoffNumber==2,"first current-schema write retains legacy deal");
        })
    };
    static Session Group(string directory,int count)
    {
        var session=Session.Open(directory,Language.English, maximum => 0);
        for(int i=0;i<count;i++) Check(session.AddPlayer("Person " + i).Success,"person added");
        session.SetMode(GameMode.Classic); return session;
    }
    static List<string> ReadCards(Session session)
    {
        var words=new List<string>();
        while(session.Match.Phase==MatchPhase.Handoff) { var id=session.Match.Owner.Id; words.Add(session.RevealWord(id)); session.HideWord(); Check(session.AdvanceHandoff(id).Success,"handoff advances"); }
        return words;
    }
    static void Eliminate(Session session,string id) { Check(session.BeginVote().Success,"vote opens"); Check(session.SelectSuspect(id).Success,"suspect selected"); Check(session.ConfirmSuspect(id).Success,"suspect confirmed"); }
    static void Check(bool actual,string expected) { if(!actual) throw new Exception(expected); }
}
