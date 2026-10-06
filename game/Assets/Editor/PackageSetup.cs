using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
namespace WordDeduction.Editor
{
    public static class PackageSetup
    {
        static AddAndRemoveRequest request;
        public static void Install()
        {
            request = Client.AddAndRemove(new[] { "com.unity.nuget.newtonsoft-json@3.2.1", "com.unity.pipeline@0.8.0-exp.1" },
                new[] { "com.unity.collab-proxy", "com.unity.feature.2d", "com.unity.ide.rider", "com.unity.ide.visualstudio", "com.unity.multiplayer.center", "com.unity.timeline", "com.unity.visualscripting" });
            EditorApplication.update += Poll;
        }
        static void Poll()
        {
            if (!request.IsCompleted) return;
            EditorApplication.update -= Poll;
            if (request.Status == StatusCode.Success) { Debug.Log("Word Deduction packages installed."); EditorApplication.Exit(0); }
            else { Debug.LogError(request.Error.message); EditorApplication.Exit(1); }
        }
    }
}
