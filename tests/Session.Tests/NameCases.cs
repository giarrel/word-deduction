using WordDeduction;

internal static class NameCases
{
    internal static readonly (string name, Action<string> run)[] All = {
        ("accepted long grapheme names and duplicate labels survive dealt and progressed matches", directory => {
            var family = "👨‍👩‍👧‍👦";
            var names = new[] { "a" + new string('\u0301', 201), string.Concat(Enumerable.Repeat(family, 19)), string.Concat(Enumerable.Repeat(family, 24)), string.Concat(Enumerable.Repeat(family, 18)) };
            for (int i = 0; i < names.Length; i++)
            {
                var folder = Path.Combine(directory, i.ToString());
                var session = Session.Open(folder, Language.English);
                Check(session.AddPlayer(names[i]).Success, "accepted name is within 24 extended graphemes");
                Check(session.AddPlayer(i == 3 ? names[i] : "Bea").Success, "second player accepted, including duplicate long name");
                Check(session.AddPlayer("Chris").Success, "Quick group ready");
                var labels = session.View.Players.Select(p => p.DisplayName).ToArray();
                Check(labels[0].Length > 200, "fixture crosses former frozen-name code-unit limit");
                session = Session.Open(folder, Language.German);
                Check(session.View.StorageNotice == null && session.View.Players.Select(p => p.DisplayName).SequenceEqual(labels), "group opens unchanged");
                Check(session.StartMatch().Success, "deal confirmed");
                var matchId = session.Match.Id;
                var reopened = Session.Open(folder, Language.German);
                Check(reopened.View.StorageNotice == null && reopened.Match?.Id == matchId, "confirmed deal restores without recovery or reshuffle");
                Check(reopened.Match.Participants.Select(p => p.DisplayName).SequenceEqual(labels), "complete frozen labels and duplicate suffixes retained");
                var owner = session.Match.Owner.Id;
                var word = session.RevealWord(owner); session.HideWord();
                Check(session.AdvanceHandoff(owner).Success, "handoff confirmed after reading");
                reopened = Session.Open(folder, Language.German);
                Check(reopened.View.StorageNotice == null && !reopened.View.StorageBlocked && reopened.Match?.Id == matchId, "both generations remain valid after progression");
                Check(reopened.Match.HandoffNumber == 2 && reopened.Match.Participants.Select(p => p.DisplayName).SequenceEqual(labels), "progress and all frozen labels survive restart");
            }
        }),
        ("malformed frozen labels remain blocked despite a valid checksum", directory => {
            foreach (var invalid in new[] { "", " ", "Alex\nBea", "A\u0000B", " padded ", "e\u0301" })
            {
                var folder = Path.Combine(directory, Guid.NewGuid().ToString("N"));
                var session = Session.Open(folder, Language.English);
                foreach (var name in new[] { "Alex", "Bea", "Chris" }) session.AddPlayer(name);
                Check(session.StartMatch().Success, "valid fixture deal");
                var path = Path.Combine(folder, "session.json");
                var envelope = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
                var payload = Newtonsoft.Json.Linq.JObject.Parse((string)envelope["Payload"]);
                payload["Match"]["Participants"][0]["DisplayName"] = invalid;
                var json = payload.ToString(Newtonsoft.Json.Formatting.None);
                envelope["Payload"] = json;
                envelope["Checksum"] = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json)));
                File.WriteAllText(path, envelope.ToString());
                File.Copy(path, Path.Combine(folder, "session.previous.json"), true);
                var reopened = Session.Open(folder, Language.German);
                Check(reopened.View.StorageBlocked && reopened.View.StorageNotice == "DamagedData", "invalid confirmed labels cannot bypass structural validation");
                Check(!reopened.AddPlayer("Overwrite").Success, "invalid generations stay protected");
            }
        }),
        ("Unicode 17 conformance keeps every expected extended grapheme intact", directory => {
            int cases = 0;
            foreach (var line in File.ReadLines(Path.Combine(AppContext.BaseDirectory, "GraphemeBreakTest.txt")))
            {
                var data = line.Split('#')[0].Trim();
                if (data.Length == 0) continue;
                var expected = new List<string>();
                var element = new System.Text.StringBuilder();
                foreach (var token in data.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (token == "÷")
                    {
                        if (element.Length > 0) { expected.Add(element.ToString()); element.Clear(); }
                    }
                    else if (token != "×")
                        element.Append(char.ConvertFromUtf32(Convert.ToInt32(token, 16)));
                }
                var remaining = string.Concat(expected);
                var actual = new List<string>();
                while (remaining.Length > 0)
                {
                    var first = NameText.FirstElement(remaining);
                    Check(first.Length > 0 && remaining.StartsWith(first, StringComparison.Ordinal), "segmentation advances");
                    actual.Add(first);
                    remaining = remaining.Substring(first.Length);
                }
                Check(expected.SequenceEqual(actual), $"conformance case {cases + 1}: {data}");
                cases++;
            }
            Check(cases == 766, "all 766 official Unicode 17 test cases executed");
        }),
        ("legacy V4 names survive stricter new-entry policy in groups and live matches", directory => {
            foreach (var suffix in new[] { "group", "match" })
            {
                var savedDirectory = Path.Combine(directory, suffix);
                Directory.CreateDirectory(savedDirectory);
                var path = Path.Combine(savedDirectory, "session.json");
                File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures", "legacy-v4-names-" + suffix + ".json"), path);
                var before = File.ReadAllBytes(path);
                var session = Session.Open(savedDirectory, Language.English);
                Check(!session.View.StorageBlocked && session.View.StorageNotice == null, "legacy snapshot opens normally");
                Check(session.View.Players.Count == 4 && session.View.CanUndo, "legacy group and removed-name undo are retained");
                Check(before.SequenceEqual(File.ReadAllBytes(path)), "opening preserves exact legacy file");
                var invisible = session.View.Players[0];
                var formerlyShort = session.View.Players[1];
                Check(invisible.Name == "\u200B" && formerlyShort.Name == string.Concat(Enumerable.Repeat("\u2701\u200D\u2701", 24)), "legacy names retained without rewriting");
                if (suffix == "match")
                {
                    Check(session.Match != null && session.Match.Mode == GameMode.Classic && session.Match.Participants.Count == 4, "live Classic keeps all participants");
                    var matchId = session.Match.Id;
                    Check(Session.Open(savedDirectory, Language.English).Match.Id == matchId, "same live match restored");
                    Check(session.AbandonMatch(matchId).Success, "legacy match remains operable");
                }
                Check(session.AddPlayer(invisible.Name).Error == "InvalidName", "new invisible name remains rejected");
                Check(session.AddPlayer(formerlyShort.Name).Error == "InvalidName", "new Unicode-17-overlong name remains rejected");
                Check(session.AddPlayer("Chris").Success && session.UndoRemove().Success, "unrelated edits and legacy undo work");
                Check(session.RenamePlayer(invisible.Id, "Renamed").Success, "old invisible player can be renamed normally");
                session = Session.Open(savedDirectory, Language.English);
                Check(session.View.Players.Single(player => player.Id == invisible.Id).Name == "Renamed", "rename preserves legacy identity");
                Check(session.View.Players.Single(player => player.Id == formerlyShort.Id).Name == formerlyShort.Name, "untouched legacy name survives subsequent writes");
                Check(session.View.Players.Any(player => player.Name == "\u0301"), "legacy mark-only removed player restored");
            }
        }),
        ("new names use 24 extended graphemes for emoji flags Indic Hangul and accents", directory => {
            var session = Session.Open(directory, Language.English);
            foreach (var element in new[] { "👩🏽‍🚀", "🇨🇭", "\u0915\u094D\u0937", "\u1100\u1161\u11A8", "e\u0301" })
            {
                var twentyFour = string.Concat(Enumerable.Repeat(element, 24));
                var twentyFive = twentyFour + element;
                Check(NameText.FirstElement(element + "X") == element, "first element remains complete and unnormalized");
                Check(session.AddPlayer(twentyFour).Success, "24 extended graphemes accepted");
                var player = session.View.Players.Last();
                Check(session.AddPlayer(twentyFive).Error == "InvalidName", "25 extended graphemes rejected");
                Check(session.RenamePlayer(player.Id, twentyFive).Error == "InvalidName", "overlong rename rejected");
                session = Session.Open(directory, Language.German);
                Check(session.View.Players.Last().Id == player.Id && session.View.Players.Last().Name == twentyFour.Normalize(), "NFC name and identity survive reopening");
            }
            Check(session.AddPlayer("مُحمّد").Success && session.AddPlayer("李雷").Success, "Arabic marks and CJK remain valid");
            Check(session.AddPlayer("A\u200DB").Success, "embedded joiner is preserved");
        }),
        ("malformed UTF16 edits fail without damaging the confirmed name", directory => {
            var session = Session.Open(directory, Language.English);
            Check(session.AddPlayer("Zoë").Success, "valid name accepted");
            var player = session.View.Players.Single();
            var before = File.ReadAllBytes(Path.Combine(directory, "session.json"));
            foreach (var malformed in new[] { "\uD800", "\uDC00", "A\uD800B", "\uD83D\uD83D", "\uDC00\uD800" })
            {
                Check(session.AddPlayer(malformed).Error == "InvalidName", "malformed add rejected");
                Check(session.RenamePlayer(player.Id, malformed).Error == "InvalidName", "malformed rename rejected");
                bool rejected = false;
                try { NameText.FirstElement(malformed); } catch (ArgumentException) { rejected = true; }
                Check(rejected, "presentation helper rejects malformed text");
            }
            Check(NameText.FirstElement("") == "", "empty presentation text has no initial");
            Check(before.SequenceEqual(File.ReadAllBytes(Path.Combine(directory, "session.json"))), "invalid UTF16 does not write");
            Check(Session.Open(directory, Language.English).View.Players.Single().Id == player.Id, "confirmed identity retained");
        }),
        ("Unicode line separators cannot enter a new name", directory => {
            var session = Session.Open(directory, Language.English);
            Check(session.AddPlayer("Alex").Success, "visible name accepted");
            var id = session.View.Players.Single().Id;
            foreach (var name in new[] { "A\u2028B", "A\u2029B" })
            {
                Check(session.AddPlayer(name).Error == "InvalidName", "line separator add rejected");
                Check(session.RenamePlayer(id, name).Error == "InvalidName", "line separator rename rejected");
            }
            Check(Session.Open(directory, Language.English).View.Players.Single().Name == "Alex", "confirmed name retained");
        }),
        ("invisible-only names cannot create or replace a player", directory => {
            var session = Session.Open(directory, Language.English);
            Check(session.AddPlayer("Alex").Success, "visible name accepted");
            var id = session.View.Players.Single().Id;
            var before = File.ReadAllBytes(Path.Combine(directory, "session.json"));
            foreach (var name in new[] { "\u200B", "\u200E", "\u200D\uFE0F", "\u0301\u0903", " \u2060\u200F ", "\u3164" })
            {
                Check(session.AddPlayer(name).Error == "InvalidName", "invisible add rejected");
                Check(session.RenamePlayer(id, name).Error == "InvalidName", "invisible rename rejected");
            }
            Check(before.SequenceEqual(File.ReadAllBytes(Path.Combine(directory, "session.json"))), "invalid input does not write");
            var reopened = Session.Open(directory, Language.German);
            Check(reopened.View.Players.Single().Id == id && reopened.View.Players.Single().Name == "Alex", "valid name and identity retained");
        })
    };

    private static void Check(bool actual, string expected) { if (!actual) throw new Exception(expected); }
}
