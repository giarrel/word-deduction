using WordDeduction;
using Newtonsoft.Json.Linq;

static class ContentCases
{
    static readonly JArray Catalog = JArray.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"word-pairs.json")));
    public static (string name, Action<string> run)[] All = {
        ("all 520 bilingual pairs are dealt once per cycle across restarts and language changes", directory => {
            var session = Group(directory);
            var used = new HashSet<string>(); string last = null;
            for (int deal=0;deal<1040;deal++) {
                session = Session.Open(directory,Language.English, maximum => maximum-1);
                Check(session.SetLanguage(deal%2==0 ? Language.German : Language.English).Success,"language changes between deals");
                string id = Draw(session);
                if (deal%520==0) { Check(last != id,"rollover avoids the last pair"); used.Clear(); }
                Check(used.Add(id),"pair repeats before all 520 pairs were used at deal " + deal);
                last=id;
            }
            Check(used.Count==520,"complete second cycle");
        }),
        ("words from the last ten deals are avoided across languages while other pairs remain", directory => {
            var session=Group(directory); var recent=new Queue<JToken>();
            for(int deal=0;deal<250;deal++) {
                session=Session.Open(directory,Language.English,maximum => maximum-1);
                session.SetLanguage(deal%2==0 ? Language.German : Language.English);
                string id=Draw(session);
                var pair=Catalog.Single(p => (string)p["id"]==id);
                foreach(string language in new[]{"de","en"}) {
                    var prior=recent.SelectMany(p=>p[language].Values<string>()).ToHashSet();
                    Check(!pair[language].Values<string>().Any(prior.Contains),"recent word repeated although hundreds of untouched alternatives remain");
                }
                recent.Enqueue(pair); if(recent.Count>10) recent.Dequeue();
            }
        }),
        ("a V4 deal without its consumed pair is rejected before revealing a word", directory => {
            var session=Group(directory); session.StartMatch();
            string owner=session.Match.Owner.Id;
            string path=Path.Combine(directory,"session.json");
            Rewrite(path,4,payload => payload["History"]["UsedPairIds"] = new JArray());
            session=Session.Open(directory,Language.German);
            Check(session.View.StorageNotice=="RecoveredBackup" && session.Match==null,"inconsistent deal/history recovers the last committed group");
            Check(session.RevealWord(owner)==null,"unvalidated deal cannot reveal");
        }),
        ("legacy V2 and V3 deals retain their words and seed the shared cycle without rewriting on open", directory => {
            foreach(int version in new[]{2,3}) {
                string folder=Path.Combine(directory,version.ToString());
                var session=Group(folder); session.StartMatch();
                string id=session.Match.Id, owner=session.Match.Owner.Id, word=session.RevealWord(owner); session.HideWord();
                string path=Path.Combine(folder,"session.json");
                Rewrite(path,version,payload => payload.Remove("History"));
                string old=File.ReadAllText(path);
                session=Session.Open(folder,Language.German,maximum=>maximum-1);
                Check(session.View.StorageNotice==null && session.Match.Id==id && !session.Match.CanAdvance,"legacy match resumes covered without recovery or a new deal");
                Check(session.Match.Language==Language.English && session.RevealWord(owner)==word,"legacy words and word language retained");
                Check(File.ReadAllText(path)==old,"opening legacy data never rewrites it");
                session.AbandonMatch(id);
                Check(Draw(session)!="fruit-vegetables-001","legacy current pair is already consumed in the new cycle");
            }
        }),
        ("failed deals consume no pair in memory or after restart", directory => {
            string actual=Path.Combine(directory,"actual"), control=Path.Combine(directory,"control");
            var session=Group(actual); var comparison=Group(control);
            string pending=Path.Combine(actual,"session.pending.json"); Directory.CreateDirectory(pending);
            Check(session.StartMatch().Error=="SaveFailed" && session.Match==null,"failed deal remains outside a match");
            Directory.Delete(pending);
            session=Session.Open(actual,Language.German,maximum=>maximum-1);
            Check(Draw(session)==Draw(comparison),"failed draw did not consume first pair");
            Directory.CreateDirectory(pending);
            Check(session.StartMatch().Error=="SaveFailed","later draw can fail too"); Directory.Delete(pending);
            Check(Draw(session)==Draw(comparison),"retry on same Session keeps the same pending cycle position");
        }),
        ("a cycle completes even when the final eligible pair repeats a recent word", directory => {
            // Deterministic randomness puts the two independently authored rhino
            // pairs at the end of this shuffle. Only the public deals are observed.
            var session=Group(directory,maximum => maximum==520 ? 217 : maximum==519 ? 218 : maximum-1);
            var ids=new HashSet<string>(); string beforeLast=null, last=null;
            for(int deal=0;deal<520;deal++) { beforeLast=last; last=Draw(session); Check(ids.Add(last),"pair cycle never repeats"); }
            var a=Catalog.Single(p=>(string)p["id"]==beforeLast);
            var b=Catalog.Single(p=>(string)p["id"]==last);
            Check(a["en"].Values<string>().Intersect(b["en"].Values<string>()).Contains("Rhinoceros"),"fixture exercises an unavoidable final recent word");
            Check(Draw(session)!=last,"rollover still avoids the immediate pair");
        })
    };
    static Session Group(string directory, Func<int,int> random = null)
    {
        var session=Session.Open(directory,Language.English,random ?? (maximum => maximum-1));
        foreach(var name in new[]{"A","B","C"}) Check(session.AddPlayer(name).Success,"group saved");
        return session;
    }
    static string Draw(Session session)
    {
        Check(session.StartMatch().Success,"deal saved");
        var words=new HashSet<string>();
        while(session.Match.Phase==MatchPhase.Handoff) {
            string owner=session.Match.Owner.Id;
            words.Add(session.RevealWord(owner)); session.HideWord();
            Check(session.AdvanceHandoff(owner).Success,"handoff saved");
        }
        string language=session.Match.Language==Language.German ? "de" : "en";
        var pair=Catalog.SingleOrDefault(p => words.SetEquals(p[language].Values<string>()));
        Check(pair!=null,"every displayed pair belongs to the authored bilingual catalog");
        Check(session.AbandonMatch(session.Match.Id).Success,"abandon still consumes its deal");
        return (string)pair["id"];
    }
    static void Check(bool actual,string expected) { if(!actual) throw new Exception(expected); }
    static void Rewrite(string path,int version,Action<JObject> edit)
    {
        var envelope=JObject.Parse(File.ReadAllText(path));
        var payload=JObject.Parse((string)envelope["Payload"]); edit(payload);
        string text=payload.ToString(Newtonsoft.Json.Formatting.None);
        envelope["Version"]=version; envelope["Payload"]=text;
        envelope["Checksum"]=Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(text)));
        File.WriteAllText(path,envelope.ToString());
    }
}
