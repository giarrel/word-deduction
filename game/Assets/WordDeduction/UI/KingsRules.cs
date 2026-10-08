using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    internal static class KingsRules
    {
        public static void AddTo(VisualElement parent, Language language)
        {
            var scroll = new ScrollView { name = "kingsRulesScroll", horizontalScrollerVisibility = ScrollerVisibility.Hidden };
            scroll.AddToClassList("info-scroll"); parent.Add(scroll);
            var title = new Label(Copy.Get(language, "kingsHelpTitle")) { name = "helpTitle" };
            title.AddToClassList("info-title"); scroll.Add(title);
            foreach (var section in new[] { "Teams", "Cards", "Privacy", "Round", "Clues", "Vote", "Elimination", "LastChance", "Judgment", "Accidents" })
            {
                var heading = new Label(Copy.Get(language, "kingsRules" + section + "Title"));
                heading.AddToClassList("kings-rule-title"); scroll.Add(heading);
                var text = new Label(Copy.Get(language, "kingsRules" + section)) { name = "kingsRules" + section };
                text.AddToClassList("kings-rule-copy"); scroll.Add(text);
            }
        }
    }
}
