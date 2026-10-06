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
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.giarrel.worddeduction");
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.predictiveBackSupport = true;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.Android, ManagedStrippingLevel.Minimal);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.defaultScreenWidth = 390; PlayerSettings.defaultScreenHeight = 844;
            PlayerSettings.runInBackground = true;
            PlayerSettings.Android.forceInternetPermission = false;
            PlayerSettings.Android.forceSDCardPermission = false;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
            PlayerSettings.colorSpace = ColorSpace.Gamma;
            QualitySettings.vSyncCount = 0;
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
        public static void DevelopmentApk() => Build(false,true);
        public static void ReleaseApk() => Build(false,false);
        public static void ReleaseBundle() => Build(true,false);
        static void Build(bool bundle, bool development)
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before building.");
            Configure();
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                throw new InvalidOperationException("Switch to Android first (or launch -buildTarget Android).");
            if (PlayerSettings.Android.useCustomKeystore) throw new InvalidOperationException("Signing must be selected explicitly; do not silently use a custom key.");
            var output = Path.GetFullPath(Path.Combine(Application.dataPath,"../../artifacts/android"));
            Directory.CreateDirectory(output);
            var path = Path.Combine(output, "WordDeduction-" + (development ? "development" : "local-release") + (bundle ? ".aab" : ".apk"));
            bool previousBundle = EditorUserBuildSettings.buildAppBundle;
            try
            {
                EditorUserBuildSettings.buildAppBundle = bundle;
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                    scenes = new[] { "Assets/WordDeduction/Scenes/App.unity" }, locationPathName = path, target = BuildTarget.Android,
                    options = BuildOptions.DetailedBuildReport | (development ? BuildOptions.Development : BuildOptions.CleanBuildCache)
                });
                File.WriteAllText(Path.Combine(output,"build-summary.json"), JsonUtility.ToJson(new BuildSummaryData {
                    result = report.summary.result.ToString(), errors = report.summary.totalErrors, warnings = report.summary.totalWarnings,
                    reportedBytes = report.summary.totalSize, fileBytes = File.Exists(path) ? (ulong)new FileInfo(path).Length : 0, path = path, unity = Application.unityVersion
                },true));
                if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Android build failed; inspect the build log.");
            }
            finally { EditorUserBuildSettings.buildAppBundle = previousBundle; }
        }
        [Serializable] sealed class BuildSummaryData { public string result; public int errors; public int warnings; public ulong reportedBytes; public ulong fileBytes; public string path; public string unity; }
    }
}
