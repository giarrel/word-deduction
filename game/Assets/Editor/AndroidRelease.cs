using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace WordDeduction.Editor
{
    // Local candidates intentionally keep Android's debug certificate. Only the explicit
    // production entrypoints read owner-supplied credentials, and never write them to reports.
    static class AndroidRelease
    {
        public static void Build(bool bundle, bool development, bool production)
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before building.");
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                throw new InvalidOperationException("Switch to Android first (or launch -buildTarget Android).");
            var root = Path.GetFullPath(Path.Combine(Application.dataPath,"../.."));
            var defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android).Split(';');
            if (!development && defines.Any(d => d.Trim() == "ENABLE_RUNTIME_PIPELINE" || d.Trim() == "ENABLE_PROFILER"))
                throw new InvalidOperationException("Release builds forbid runtime Pipeline/profiler defines.");
            if (!production && PlayerSettings.Android.useCustomKeystore)
                throw new InvalidOperationException("Local builds refuse an existing custom signing key.");
            string key = null, alias = null, storePassword = null, keyPassword = null;
            if (production)
            {
                key = Required("WD_ANDROID_KEYSTORE"); alias = Required("WD_ANDROID_KEY_ALIAS");
                storePassword = Required("WD_ANDROID_STORE_PASSWORD"); keyPassword = Required("WD_ANDROID_KEY_PASSWORD");
                key = Path.GetFullPath(key);
                if (!File.Exists(key) || key.StartsWith(root + Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)
                    || alias.Equals("androiddebugkey",StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Production requires an existing dedicated key outside the checkout, with a non-debug alias.");
            }
            var source = Git(root,"rev-parse HEAD").Trim();
            if (!development && Git(root,"status --porcelain --untracked-files=no").Length != 0)
                throw new InvalidOperationException("Commit tracked source and configured settings before building a release candidate.");
            var stamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
            var kind = development ? "development" : production ? "production" : "local-release";
            var output = Path.Combine(root,"artifacts/android",source.Substring(0,12),stamp + (bundle ? "-aab" : "-apk"));
            Directory.CreateDirectory(output);
            var path = Path.Combine(output,"WordDeduction-" + kind + (bundle ? ".aab" : ".apk"));
            var options = BuildOptions.DetailedBuildReport | (development ? BuildOptions.Development : BuildOptions.CleanBuildCache);
            using (new BuildState())
            {
                AppBuild.Configure();
                EditorUserBuildSettings.buildAppBundle = bundle;
                EditorUserBuildSettings.development = development;
                EditorUserBuildSettings.allowDebugging = false;
                EditorUserBuildSettings.connectProfiler = false;
                EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                PlayerSettings.usePlayerLog = development;
                PlayerSettings.Android.useCustomKeystore = production;
                if (production)
                {
                    PlayerSettings.Android.keystoreName = key; PlayerSettings.Android.keyaliasName = alias;
                    PlayerSettings.Android.keystorePass = storePassword; PlayerSettings.Android.keyaliasPass = keyPassword;
                }
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                    scenes = new[] { "Assets/WordDeduction/Scenes/App.unity" }, locationPathName = path,
                    target = BuildTarget.Android, options = options
                });
                File.WriteAllText(Path.Combine(output,"build-summary.json"),JsonUtility.ToJson(new Summary {
                    result = report.summary.result.ToString(), errors = report.summary.totalErrors, warnings = report.summary.totalWarnings,
                    durationSeconds = report.summary.totalTime.TotalSeconds, reportedBytes = report.summary.totalSize,
                    fileBytes = File.Exists(path) ? (ulong)new FileInfo(path).Length : 0, sha256 = Hash(path), path = path,
                    unity = Application.unityVersion, sourceCommit = source, packageLockSha256 = Hash(Path.Combine(root,"game/Packages/packages-lock.json")),
                    options = options.ToString(), version = PlayerSettings.bundleVersion, versionCode = PlayerSettings.Android.bundleVersionCode,
                    applicationId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android),
                    signing = production ? "owner-selected external key; verify certificate before distribution" : "Android debug certificate; local testing only"
                },true));
                if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Android build failed; inspect its build-summary.json and Editor log.");
            }
        }
        static string Required(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException("Production signing requires " + name + ". No artifact was built.");
            return value;
        }
        static string Git(string root,string arguments)
        {
            var gitPath = root.Replace('\\','/');
            var start = new System.Diagnostics.ProcessStartInfo("git","-c safe.directory=\"" + gitPath + "\" -C \"" + gitPath + "\" " + arguments) {
                UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true
            };
            using (var process = System.Diagnostics.Process.Start(start))
            {
                var result = process.StandardOutput.ReadToEnd(); var error = process.StandardError.ReadToEnd(); process.WaitForExit();
                if (process.ExitCode != 0) throw new InvalidOperationException("Cannot establish release source revision: " + error);
                return result;
            }
        }
        static string Hash(string path)
        {
            if (!File.Exists(path)) return "";
            using (var algorithm = SHA256.Create()) using (var stream = File.OpenRead(path))
                return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-","").ToLowerInvariant();
        }
        sealed class BuildState : IDisposable
        {
            readonly bool bundle = EditorUserBuildSettings.buildAppBundle, development = EditorUserBuildSettings.development,
                debugging = EditorUserBuildSettings.allowDebugging, profiler = EditorUserBuildSettings.connectProfiler,
                deepProfiling = EditorUserBuildSettings.buildWithDeepProfilingSupport, log = PlayerSettings.usePlayerLog,
                custom = PlayerSettings.Android.useCustomKeystore;
            readonly string key = PlayerSettings.Android.keystoreName, alias = PlayerSettings.Android.keyaliasName,
                storePassword = PlayerSettings.Android.keystorePass, keyPassword = PlayerSettings.Android.keyaliasPass;
            public void Dispose()
            {
                EditorUserBuildSettings.buildAppBundle = bundle; EditorUserBuildSettings.development = development;
                EditorUserBuildSettings.allowDebugging = debugging; EditorUserBuildSettings.connectProfiler = profiler;
                EditorUserBuildSettings.buildWithDeepProfilingSupport = deepProfiling; PlayerSettings.usePlayerLog = log;
                PlayerSettings.Android.useCustomKeystore = custom; PlayerSettings.Android.keystoreName = key;
                PlayerSettings.Android.keyaliasName = alias; PlayerSettings.Android.keystorePass = storePassword;
                PlayerSettings.Android.keyaliasPass = keyPassword;
                AssetDatabase.SaveAssets();
            }
        }
        [Serializable] sealed class Summary
        {
            public string result,sha256,path,unity,sourceCommit,packageLockSha256,options,version,applicationId,signing;
            public int errors,warnings,versionCode; public ulong reportedBytes,fileBytes; public double durationSeconds;
        }
    }
}
