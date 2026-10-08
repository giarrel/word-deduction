using System.Security.Cryptography;
using Newtonsoft.Json;
using WordDeduction;

// Explicit test utility, never compiled into the Unity application. These are
// synthetic visual fixtures, not evidence of matches played on Android.
static class NativeVisualFixtures
{
    public static int Export(string output, string sourceCommit)
    {
        if (sourceCommit.Length != 40 || sourceCommit.Any(c => !Uri.IsHexDigit(c))) throw new ArgumentException("Provide the exact source commit.");
        output = Path.GetFullPath(output);
        if (Directory.Exists(output)) throw new IOException("Fixture output must be a fresh directory.");
        Directory.CreateDirectory(output);
        var fixtures = new List<object>();
        foreach (var language in new[] { Language.English, Language.German })
        foreach (var kind in new[] { PrivateCardKind.GoodKing, PrivateCardKind.EvilKing })
        {
            string name = language + "-" + kind, folder = Path.Combine(output, name);
            var session = Session.Open(folder, language, _ => 0);
            string[] names = {
                "WWWWWWWWWWWWWWWWWWWWWWWW", "WWWWWWWWWWWWWWWWWWWWWWWW",
                "ÄÖÜéèñøçÅæœß Königin 👑", "Zoë 👨‍👩‍👧‍👦", "王小明東京花子", "Ελένη Παπαδοπούλου",
                "Александра", "Wunderbare Wortkönigin", "WWWWWWWWWWWWWWWWWWWWWWWW",
                "WWWWWWWWWWWWWWWWWWWWWWWW", "Ana María Fernández", "Christopher-Maximilian",
                "Amélie", "Søren", "Renée", "Łukasz", "François", "İpek", "Guðrún", "Léa 👩🏽‍🚀"
            };
            foreach (string person in names) Require(session.AddPlayer(person));
            Require(session.SetMode(GameMode.Kings)); Require(session.SetKingsUndercoverPreference(8)); Require(session.StartMatch());
            PrivateCardView chosen = null;
            while (session.Match.Phase == MatchPhase.Handoff)
            {
                string owner = session.Match.Owner.Id; var card = session.RevealCard(owner); session.HideWord();
                if (card.Kind == kind) { chosen = card; break; }
                Require(session.AdvanceHandoff(owner));
            }
            if (chosen == null) throw new Exception("Requested card was not dealt.");
            session = Session.Open(folder, language, _ => throw new Exception("Fixture reopen must not draw."));
            if (session.View.StorageNotice != null || session.Match.Owner.Id != chosen.Owner.Id || session.Match.CanAdvance)
                throw new Exception("The fixture did not reopen covered at its unfinished handoff.");
            var files = new[] { "session.json", "session.previous.json" }.Select(file => {
                string path = Path.Combine(folder, file); byte[] bytes = File.ReadAllBytes(path);
                return new { path = name + "/" + file, bytes = bytes.Length, sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant() };
            }).ToArray();
            fixtures.Add(new { name, language = language.ToString(), kind = kind.ToString(), session.Match.Id,
                session.Match.HandoffNumber, owner = chosen.Owner, knownParticipants = chosen.KnownParticipants,
                participants = session.Match.Participants, ordinaryUndercover = 8, totalParticipants = 20, files });
        }
        File.WriteAllText(Path.Combine(output, "manifest.json"), JsonConvert.SerializeObject(new {
            purpose = "Synthetic visual fixtures generated solely by public Session actions; not Android gameplay evidence.",
            sourceCommit, generatedUtc = DateTime.UtcNow, fixtures
        }, Formatting.Indented));
        Console.WriteLine("Exported four covered, unfinished Kings handoffs with both committed generations: " + output);
        return 0;
    }
    static void Require(CommandResult result) { if (!result.Success) throw new Exception("Fixture action failed: " + result.Error); }
}
