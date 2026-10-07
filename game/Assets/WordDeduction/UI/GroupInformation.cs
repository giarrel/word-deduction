using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    // Optional public information. It never reads or changes a Session or an unsaved name.
    internal sealed class GroupInformation
    {
        readonly VisualElement group, host;
        readonly Func<Language> language;
        readonly Action changed;
        VisualElement screen;
        public bool IsOpen => screen != null;
        public GroupInformation(VisualElement root, Func<Language> language, Action changed)
        {
            group = root.Q<VisualElement>("screen"); host = root.Q<VisualElement>("safeRoot");
            this.language = language; this.changed = changed;
        }
        public void Open() => Show(false);
        void Show(bool licenses)
        {
            screen?.RemoveFromHierarchy();
            group.AddToClassList("hidden");
            screen = new VisualElement { name = "appInformation" }; screen.AddToClassList("screen"); host.Add(screen);
            AddLabel(screen,"infoTitle",T(licenses ? "licenses" : "privacyTitle"),"info-title");
            AddLabel(screen,"infoVersion","Word Deduction · " + Application.version,"info-version");
            var scroll = new ScrollView { name = "infoScroll", horizontalScrollerVisibility = ScrollerVisibility.Hidden };
            scroll.AddToClassList("info-scroll"); screen.Add(scroll);
            AddLabel(scroll,licenses ? "licenseIntro" : "privacyText",T(licenses ? "licenseIntro" : "privacyText"),"info-copy");
            if (licenses)
            {
                string notices = "";
                foreach (var name in new[] { "Inter-LICENSE", "NotoColorEmoji-LICENSE", "Unicode-LICENSE", "Newtonsoft-LICENSE" })
                    notices += name + "\n\n" + Resources.Load<TextAsset>(name).text + "\n\n";
                AddLabel(scroll,"licenseText",notices,"license-copy");
            }
            else
            {
                var button = new Button(() => Show(true)) { name = "showLicenses", text = T("licenses") };
                button.AddToClassList("white-preference"); scroll.Add(button);
            }
            var close = new Button(Close) { name = "closeInfo", text = T("back") }; close.AddToClassList("play-button"); screen.Add(close);
            changed();
        }
        public void Close()
        {
            if (screen == null) return;
            screen.RemoveFromHierarchy(); screen = null; group.RemoveFromClassList("hidden"); changed();
        }
        string T(string key) => Copy.Get(language(),key);
        static void AddLabel(VisualElement parent,string name,string value,string style)
        {
            var label = new Label(value) { name = name }; label.AddToClassList(style); parent.Add(label);
        }
    }
}
