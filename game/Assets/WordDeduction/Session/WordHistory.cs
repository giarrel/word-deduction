using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace WordDeduction
{
    public sealed partial class Session
    {
        WordCatalog.Pair DrawPair(WordHistoryState history)
        {
            var eligible = WordCatalog.Pairs.ToDictionary(p => p.Id);
            // Retain the pending shuffle across restarts. Removed IDs may remain in
            // used history; new catalog IDs join the current cycle without repeats.
            history.RemainingPairIds.RemoveAll(id => !eligible.ContainsKey(id));
            var known = new HashSet<string>(history.UsedPairIds.Concat(history.RemainingPairIds));
            var added = eligible.Keys.Where(id => !known.Contains(id)).ToList();
            Shuffle(added);
            history.RemainingPairIds.AddRange(added);
            if (history.RemainingPairIds.Count == 0)
            {
                history.UsedPairIds.Clear();
                history.RemainingPairIds.AddRange(eligible.Keys);
                Shuffle(history.RemainingPairIds);
            }
            string last = history.RecentPairIds.LastOrDefault();
            var recentWords = new HashSet<string>(history.RecentPairIds.Where(eligible.ContainsKey).SelectMany(id => WordKeys(eligible[id])));
            int index = history.RemainingPairIds.FindIndex(id => id != last && !WordKeys(eligible[id]).Any(recentWords.Contains));
            // One finite scan, then relax word avoidance. Completing the pair cycle
            // outranks recent-word preference, even when only conflicting words remain.
            if (index < 0) index = history.RemainingPairIds.FindIndex(id => id != last);
            if (index < 0) index = 0;
            string selected = history.RemainingPairIds[index];
            history.RemainingPairIds.RemoveAt(index);
            history.UsedPairIds.Add(selected);
            history.RecentPairIds.Add(selected);
            if (history.RecentPairIds.Count > 10) history.RecentPairIds.RemoveAt(0);
            return eligible[selected];
        }
        static IEnumerable<string> WordKeys(WordCatalog.Pair pair) =>
            pair.German.Select(word => "de:" + NormalizeWord(word)).Concat(pair.English.Select(word => "en:" + NormalizeWord(word)));
        static string NormalizeWord(string word)
        {
            var result = new StringBuilder();
            foreach (char value in word.Normalize(NormalizationForm.FormKD).ToLowerInvariant().Replace("ß","ss"))
                if (char.IsLetterOrDigit(value) && CharUnicodeInfo.GetUnicodeCategory(value) != UnicodeCategory.NonSpacingMark) result.Append(value);
            return result.ToString();
        }
        void Shuffle(List<string> values)
        {
            for (int i=values.Count-1;i>0;i--) { int j=random(i+1); string item=values[i]; values[i]=values[j]; values[j]=item; }
        }
        internal static void MigrateWordHistory(SessionState value)
        {
            value.History = new WordHistoryState();
            if (value.Match == null) return;
            value.History.UsedPairIds.Add(value.Match.PairId);
            value.History.RecentPairIds.Add(value.Match.PairId);
        }
        static bool ValidWordHistory(WordHistoryState history)
        {
            if (history == null || history.UsedPairIds == null || history.RemainingPairIds == null || history.RecentPairIds == null ||
                history.RecentPairIds.Count > 10 || history.UsedPairIds.Count > 10000 || history.RemainingPairIds.Count > 10000) return false;
            var cycle = history.UsedPairIds.Concat(history.RemainingPairIds).ToArray();
            if (cycle.Distinct(StringComparer.Ordinal).Count() != cycle.Length) return false;
            return cycle.Concat(history.RecentPairIds).All(id => !string.IsNullOrWhiteSpace(id) && id.Length <= 100 && !id.Any(char.IsControl));
        }
    }
    internal sealed class WordHistoryState
    {
        [JsonProperty(Required = Required.Always)] public List<string> UsedPairIds = new List<string>();
        [JsonProperty(Required = Required.Always)] public List<string> RemainingPairIds = new List<string>();
        [JsonProperty(Required = Required.Always)] public List<string> RecentPairIds = new List<string>();
    }
}
