using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using WordDeduction.UI;
namespace WordDeduction.Editor
{
    public static class AppBuild
    {
        public static void Configure()
        {
            PlayerSettings.companyName = "giarrel";
            PlayerSettings.productName = "Word Deduction";
            PlayerSettings.bundleVersion = "1.2.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.giarrel.worddeduction");
            PlayerSettings.Android.bundleVersionCode = 10;
            PlayerSettings.Android.minifyRelease = true;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.predictiveBackSupport = true;
            PlayerSettings.Android.startInFullscreen = false;
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.Android, ManagedStrippingLevel.Minimal);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.defaultScreenWidth = 390; PlayerSettings.defaultScreenHeight = 844;
            PlayerSettings.runInBackground = true;
            PlayerSettings.Android.forceInternetPermission = false;
            PlayerSettings.Android.forceSDCardPermission = false;
            UnityEditor.Analytics.AnalyticsSettings.enabled = false;
            UnityEditor.Analytics.AnalyticsSettings.initializeOnStartup = false;
            var playerSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            playerSettings.FindProperty("submitAnalytics").boolValue = false;
            playerSettings.ApplyModifiedPropertiesWithoutUndo();
            UnityEditor.Advertisements.AdvertisementSettings.enabled = false;
            UnityEditor.Advertisements.AdvertisementSettings.initializeOnStartup = false;
            PlayerSettings.usePlayerLog = false;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
            PlayerSettings.colorSpace = ColorSpace.Gamma;
            QualitySettings.vSyncCount = 0;
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/WordDeduction/UI/Artwork/AppIcon.png");
            if (icon != null) PlayerSettings.SetIcons(NamedBuildTarget.Unknown,new[] { icon },IconKind.Any);
            EnsureEmojiFont();
            AssetDatabase.SaveAssets();
        }
        public static void CreateScene()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode first.");
            Configure();
            Directory.CreateDirectory("Assets/WordDeduction/Scenes");
            const string folder = "Assets/WordDeduction/UI/";
            var font = AssetDatabase.LoadAssetAtPath<Font>(folder + "Fonts/Inter-Regular.ttf");
            var fontAsset = AssetDatabase.LoadAssetAtPath<FontAsset>(folder + "Inter.asset");
            if (fontAsset == null)
            {
                fontAsset = FontAsset.CreateFontAsset(font);
                AssetDatabase.CreateAsset(fontAsset, folder + "Inter.asset");
                foreach (var texture in fontAsset.atlasTextures) AssetDatabase.AddObjectToAsset(texture,fontAsset);
                AssetDatabase.AddObjectToAsset(fontAsset.material,fontAsset);
            }
            var text = AssetDatabase.LoadAssetAtPath<PanelTextSettings>(folder + "Text.asset");
            if (text == null) { text = ScriptableObject.CreateInstance<PanelTextSettings>(); AssetDatabase.CreateAsset(text,folder + "Text.asset"); }
            text.defaultFontAsset = fontAsset;
            var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(folder + "Panel.asset");
            if (panel == null) { panel = ScriptableObject.CreateInstance<PanelSettings>(); AssetDatabase.CreateAsset(panel,folder + "Panel.asset"); }
            panel.textSettings = text;
            panel.scaleMode = PanelScaleMode.ScaleWithScreenSize; panel.referenceResolution = new Vector2Int(390,844);
            panel.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight; panel.match = 0;
            EditorUtility.SetDirty(text); EditorUtility.SetDirty(panel);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Background").AddComponent<Camera>(); camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(250,247,240,255); camera.orthographic = true;
            var host = new GameObject("Word Deduction");
            host.AddComponent<UIDocument>().panelSettings = panel; host.AddComponent<GroupScreen>();
            const string scenePath = "Assets/WordDeduction/Scenes/App.unity";
            EditorSceneManager.SaveScene(scene,scenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath,true) };
            AssetDatabase.DeleteAsset("Assets/Scenes/SampleScene.unity");
            AssetDatabase.DeleteAsset("Assets/InputSystem_Actions.inputactions");
            AssetDatabase.SaveAssets();
        }
        public static void EnsureEmojiFont()
        {
            const string folder = "Assets/WordDeduction/UI/";
            var source = AssetDatabase.LoadAssetAtPath<Font>(folder + "Fonts/NotoColorEmoji.ttf");
            if (source == null) throw new InvalidOperationException("The bundled emoji font is missing.");
            var emoji = AssetDatabase.LoadAssetAtPath<FontAsset>(folder + "Emoji.asset");
            if (emoji == null)
            {
                emoji = FontAsset.CreateFontAsset(source,109,0,UnityEngine.TextCore.LowLevel.GlyphRenderMode.COLOR,1024,1024,AtlasPopulationMode.Dynamic,true);
                emoji.name = "Noto Color Emoji";
                AssetDatabase.CreateAsset(emoji,folder + "Emoji.asset");
                foreach (var texture in emoji.atlasTextures) AssetDatabase.AddObjectToAsset(texture,emoji);
                AssetDatabase.AddObjectToAsset(emoji.material,emoji);
            }
            var text = AssetDatabase.LoadAssetAtPath<PanelTextSettings>(folder + "Text.asset");
            if (text != null)
            {
                text.emojiFallbackTextAssets = new System.Collections.Generic.List<UnityEngine.TextCore.Text.TextAsset> { emoji };
                EditorUtility.SetDirty(text);
            }
            AssetDatabase.SaveAssets();
        }
        public static void DevelopmentApk() => AndroidRelease.Build(false,true,false);
        public static void ReleaseApk() => AndroidRelease.Build(false,false,false);
        public static void ReleaseBundle() => AndroidRelease.Build(true,false,false);
        public static void ProductionApk() => AndroidRelease.Build(false,false,true);
        public static void ProductionBundle() => AndroidRelease.Build(true,false,true);
    }
}
