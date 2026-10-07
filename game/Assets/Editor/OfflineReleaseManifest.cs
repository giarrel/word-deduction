using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Android;

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
}
