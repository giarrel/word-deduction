using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;

namespace WordDeduction.Editor
{
    // Unity retains WebRequest-related engine modules needed by its Editor tooling.
    // The shipped game has no network feature; enforce that boundary in its manifest.
    public sealed class OfflineReleaseManifest : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 1000;
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (EditorUserBuildSettings.development) return;
            var manifest = Path.Combine(path,"src/main/AndroidManifest.xml");
            var document = XDocument.Load(manifest);
            XNamespace android = "http://schemas.android.com/apk/res/android";
            foreach (var permission in document.Root.Elements("uses-permission").Where(e =>
                (string)e.Attribute(android + "name") == "android.permission.INTERNET" ||
                (string)e.Attribute(android + "name") == "android.permission.ACCESS_NETWORK_STATE").ToArray())
                permission.Remove();
            document.Save(manifest);
        }
    }
    // GameTextInput 4.4.0 writes the full editable text through unconditional Log.d.
    // R8 removes those calls in release builds while keeping every JNI name/member.
    public sealed class PrivateReleaseInput : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 1001;
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (EditorUserBuildSettings.development) return;
            if (!PlayerSettings.Android.minifyRelease)
                throw new BuildFailedException("Release input privacy requires Android release minification.");
            var rulesPath = Path.Combine(path,"proguard-unity.txt");
            var rules = File.ReadAllText(rulesPath);
            const string keep = "-keep class com.google.androidgamesdk.gametextinput.** { *; }";
            const string optimized = "-keep,allowoptimization class com.google.androidgamesdk.gametextinput.** { *; }";
            if (!rules.Contains(keep) && !rules.Contains(optimized))
                throw new BuildFailedException("Unity's GameTextInput keep rule changed; review release input privacy before building.");
            rules = rules.Replace(keep,optimized);
            rules += "\n# Remove private text diagnostics from the packaged Java input code.\n" +
                "-assumenosideeffects class android.util.Log {\n" +
                "    public static int v(...);\n    public static int d(...);\n    public static int i(...);\n}\n";
            File.WriteAllText(rulesPath,rules);
        }
    }
}
