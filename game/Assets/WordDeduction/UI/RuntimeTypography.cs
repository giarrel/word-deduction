using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    // Font discovery belongs to this panel and runs only for scripts in its names.
    internal sealed class RuntimeTypography : IDisposable
    {
        readonly UIDocument document;
        readonly PanelSettings original, panel;
        readonly PanelTextSettings text;
        readonly List<FontAsset> fonts = new List<FontAsset>();
        readonly HashSet<string> attempted = new HashSet<string>();
        string[] families;
        public RuntimeTypography(UIDocument document)
        {
            this.document = document; original = document.panelSettings;
            if (original == null || original.textSettings == null) return;
            panel = UnityEngine.Object.Instantiate(original); MobileViewport.Configure(panel);
            text = UnityEngine.Object.Instantiate(original.textSettings); panel.textSettings = text;
            text.fallbackFontAssets = new List<FontAsset>(text.fallbackFontAssets ?? new List<FontAsset>());
            text.emojiFallbackTextAssets = new List<UnityEngine.TextCore.Text.TextAsset>();
            document.panelSettings = panel;
            document.rootVisualElement.style.unityFontDefinition = FontDefinition.FromSDFFont(text.defaultFontAsset);
            document.rootVisualElement.style.unityTextGenerator = TextGeneratorType.Advanced;
        }
        public void IncludeNames(IEnumerable<string> names)
        {
            if (text == null) return;
            foreach (var name in names)
                for (int i = 0; i < name.Length; i++)
                {
                    int scalar = char.IsHighSurrogate(name[i]) && i + 1 < name.Length && char.IsLowSurrogate(name[i + 1]) ? char.ConvertToUtf32(name,i) : name[i]; if (scalar > 0xffff) i++;
                    if (scalar >= 0x0600 && scalar <= 0x08ff || scalar >= 0xfb50 && scalar <= 0xfeff) Load("Noto Naskh Arabic", "Arial");
                    else if (scalar >= 0x0900 && scalar <= 0x097f) Load("Noto Sans Devanagari", "Nirmala UI");
                    else if (scalar >= 0x0980 && scalar <= 0x09ff) Load("Noto Sans Bengali", "Nirmala UI");
                    else if (scalar >= 0x0b80 && scalar <= 0x0bff) Load("Noto Sans Tamil", "Nirmala UI");
                    else if (scalar >= 0x0e00 && scalar <= 0x0e7f) Load("Noto Sans Thai", "Leelawadee UI");
                    else if (scalar >= 0x0590 && scalar <= 0x05ff) Load("Noto Sans Hebrew", "Arial");
                    else if (scalar >= 0x2e80 && scalar <= 0xd7ff || scalar >= 0x20000 && scalar <= 0x323af) Load("Noto Sans CJK SC", "Microsoft YaHei");
                    else if (scalar >= 0x1f000 && scalar <= 0x1faff || scalar >= 0x2600 && scalar <= 0x27bf) Load("Noto Color Emoji", "Segoe UI Emoji",true);
                }
        }
        void Load(string family,string desktopAlternative,bool color = false)
        {
            if (!attempted.Add(family)) return;
            if (families == null) families = FontEngine.GetSystemFontNames();
            string entry = families.FirstOrDefault(f => f == family + " - Regular" || f == family) ??
                families.FirstOrDefault(f => f == desktopAlternative + " - Regular" || f == desktopAlternative);
            if (entry == null) return;
            int separator = entry.LastIndexOf(" - ",StringComparison.Ordinal);
            string selected = separator < 0 ? entry : entry.Substring(0,separator);
            string style = separator < 0 ? "Regular" : entry.Substring(separator + 3);
            var font = FontAsset.CreateFontAsset(selected,style,90,color ? 0 : 9,color ? GlyphRenderMode.COLOR : GlyphRenderMode.SDFAA);
            if (font == null) return;
            fonts.Add(font);
            if (color) text.emojiFallbackTextAssets = new List<UnityEngine.TextCore.Text.TextAsset>(text.emojiFallbackTextAssets) { font };
            else text.fallbackFontAssets = new List<FontAsset>(text.fallbackFontAssets) { font };
#if DEVELOPMENT_BUILD
            Debug.Log("Typography face: " + font.faceInfo.familyName + " / " + font.faceInfo.styleName);
#endif
        }
        public void Dispose()
        {
            if (panel == null) return;
            if (document != null) document.panelSettings = original;
            foreach (var font in fonts)
            {
                foreach (var atlas in font.atlasTextures) if (atlas != null) UnityEngine.Object.Destroy(atlas);
                if (font.material != null) UnityEngine.Object.Destroy(font.material);
                UnityEngine.Object.Destroy(font);
            }
            UnityEngine.Object.Destroy(text); UnityEngine.Object.Destroy(panel);
        }
    }
}
