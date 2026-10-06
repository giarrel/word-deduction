using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    // One panel owns its runtime OS fonts; no device-specific references are saved.
    internal sealed class RuntimeTypography : IDisposable
    {
        readonly UIDocument document;
        readonly PanelSettings original, panel;
        readonly PanelTextSettings text;
        readonly List<FontAsset> fonts = new List<FontAsset>();

        public RuntimeTypography(UIDocument document)
        {
            this.document = document;
            original = document.panelSettings;
            if (original == null || original.textSettings == null) return;
            panel = UnityEngine.Object.Instantiate(original);
            MobileViewport.Configure(panel);
            text = UnityEngine.Object.Instantiate(original.textSettings);
            panel.textSettings = text;
            var fallback = new List<FontAsset>(text.fallbackFontAssets ?? new List<FontAsset>());
            var emoji = new List<UnityEngine.TextCore.Text.TextAsset>();
            var families = FontEngine.GetSystemFontNames();
            foreach (var entry in families.Where(IsScriptFamily).Distinct())
            {
                // FontEngine exposes "family - style" on Android and Windows.
                int separator = entry.LastIndexOf(" - ",StringComparison.Ordinal);
                string family = separator < 0 ? entry : entry.Substring(0,separator);
                string style = separator < 0 ? "Regular" : entry.Substring(separator + 3);
                if (style != "Regular") continue;
                bool color = family.IndexOf("Emoji", StringComparison.OrdinalIgnoreCase) >= 0;
                var font = FontAsset.CreateFontAsset(family, style, 90, color ? 0 : 9,
                    color ? GlyphRenderMode.COLOR : GlyphRenderMode.SDFAA);
                if (font == null) continue;
                fonts.Add(font);
                if (color) emoji.Add(font); else fallback.Add(font);
#if DEVELOPMENT_BUILD
                Debug.Log("Typography face: " + font.faceInfo.familyName + " / " + font.faceInfo.styleName + " / " + font.atlasPopulationMode);
#endif
            }
            text.fallbackFontAssets = fallback;
            text.emojiFallbackTextAssets = emoji;
            document.panelSettings = panel;
            document.rootVisualElement.style.unityFontDefinition = FontDefinition.FromSDFFont(text.defaultFontAsset);
#if DEVELOPMENT_BUILD
            Debug.Log("Typography system families: " + families.Length + "; candidate families: " + string.Join(", ",families.Where(IsScriptFamily)));
#endif
        }
        static bool IsScriptFamily(string family)
        {
            return family.IndexOf("Noto",StringComparison.OrdinalIgnoreCase) >= 0 &&
                new[] { "Arabic", "Devanagari", "CJK", "Emoji", "Hebrew", "Thai", "Bengali", "Tamil" }.Any(s => family.IndexOf(s,StringComparison.OrdinalIgnoreCase) >= 0)
                || family == "Segoe UI Emoji" || family == "Microsoft YaHei" || family == "Nirmala UI";
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
