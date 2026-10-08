# Android release reproduction

Unity 6000.3.25f1; Android ARM64/IL2CPP, min API 26, target API 36; version 1.1.0/code 8. Pin the exact source and package lock from the artifact's `build-summary.json`. A reproducible procedure is provided; byte-identical Unity output is not promised.

## Build

Use the existing Android module, JDK 17, SDK/build-tools 36, NDK 27.2.12479018. First open `game` with the required Editor and let import finish. Run `WordDeduction.Editor.AppBuild.Configure()` in that Editor once; review and commit intentional configuration changes before a release build. Never regenerate `App.unity` just to build.

With this project's Editor closed:

```powershell
./tools/build-android.ps1 -Build ReleaseApk
./tools/build-android.ps1 -Build ReleaseBundle
```

With this project's stopped, connected Editor, use its exact absolute project path:

```powershell
unity command eval 'WordDeduction.Editor.AppBuild.ReleaseApk(); return "APK complete";' --timeout 1800000 --detach --project-path '<checkout>/game' --caller plugin --skill unity-cli --format json
```

Wait for the real Editor build report and inspect the produced APK before calling `ReleaseBundle()` the same way. A Pipeline five-second callback timeout can occur while BuildPipeline continues; do not dispatch a duplicate build based on that response. Preserve errors/warnings and explain them against the actual report.

Each build has a fresh directory under `artifacts/android/<commit-prefix>/<UTC-stamp>-apk` or `-aab`, with the artifact and its own `build-summary.json`. Reports include the complete source commit, package-lock hash, Unity version, options, identity/version/code, signing category, duration, warnings/errors, size and SHA256. The builder rejects nonignored uncommitted source and explicit runtime Pipeline/profiler symbols. Development/debugging/profiler/deep-profile, player-log, bundle and signing settings are restored after each build, including a failed build.

Local candidates deliberately use the Android debug certificate. They are suitable for the existing local test installation and **must not be uploaded as production-signed artifacts**.

## Production signing

Only the owner chooses a dedicated Word Deduction keystore and alias outside the repository. Never use another app's key. `ProductionApk` and `ProductionBundle` require all four process environment variables: `WD_ANDROID_KEYSTORE`, `WD_ANDROID_KEY_ALIAS`, `WD_ANDROID_STORE_PASSWORD`, `WD_ANDROID_KEY_PASSWORD`. Missing values, missing files, keys inside the checkout or the standard debug alias fail before any build. Supply secrets through the chosen secure environment, not command-line literals, chat or committed files. Use a fresh Editor process inheriting that environment; the existing Editor cannot inherit variables set later in a shell. The script does not create, upload or publish keys.

After a signed production build, verify the certificate fingerprint against the owner's recorded upload certificate and repeat artifact inspection. A custom-key build cannot update the debug-key test installation. Use a separate device/profile or a deliberately separate validation environment without erasing the preserved test app's data. Play App Signing, upload-key backup, identity/contact, release track and submission remain owner actions.

## Inspect artifacts

```powershell
python ./tools/inspect-android-release.py '<exact.apk>' --android-player '<Unity Editor>/Data/PlaybackEngines/AndroidPlayer' --output ./artifacts/inspection-apk
python ./tools/inspect-android-release.py '<exact.aab>' --android-player '<Unity Editor>/Data/PlaybackEngines/AndroidPlayer' --output ./artifacts/inspection-aab
```

Use an unused output directory for each invocation: generated `.apks` files are not overwritten silently. The inspector records command exit codes and stdout/stderr; verifies APK v2/signature details, zipalign 16KB, ARM64 ELF headers, every LOAD and rounded RELRO writable-byte intersection; and saves boot.config, manifest and backup rules. AAB inspection uses bundletool validate/config/manifest, jarsigner, then default and universal APK generation and inspection of master/native-bearing APKs. Generated APK signing is bundletool's local debug default; verify its certificate before installing as an update.

Code3 additionally enables release R8 and removes debug/info/verbose calls from the bundled GameTextInput Java code. JNI names/members are retained. The inspector disassembles actual APK DEX and refuses input classes that still invoke those logging methods; it also checks that InputConnection still exists. This runs against direct, universal and base-master delivery packages. Historical version2 reproduction can use `--version-code 2 --version-name 1.0.0`, but its private-input log calls intentionally fail the new privacy gate. Native real keyboard entry and log inspection must follow every relevant release change.

`GNU_RELRO` end residues are reported separately from writable-data collisions. Some Unity/NDK libraries end RELRO on a 4KB boundary but leave the rest of the 16KB protection page unmapped. Do not hide these scalar failures or binary-patch them; assess the full layout and separately run the 16KB application-environment probe. Static checks and translated emulator execution do not prove physical ARM64/16KB-kernel behavior or Play acceptance.

## Tests and actual playtests

The accepted Session seam is `tests/Session.Tests`; supply `-p:NewtonsoftJsonAssembly=<resolved package DLL>` when invoking `dotnet run`. The rendered suite is `WordDeduction.Tests`, PlayMode, through the installed Pipeline `run_tests` command and its `test_status` result. It uses real temporary stores and rendered inputs. Preserve a failed result before a fix and a passed result afterward.

Before committing/building after rendered tests, preserve test-generated dynamic Inter and Emoji differences in ignored evidence, then clear only those test-populated assets through `FontAsset.ClearFontAssetData(true)` in the Editor and save assets; verify their semantic Git diff is empty. Do not commit generated atlas/glyph caches. Git may need `git add` for those exact unchanged assets to refresh CRLF normalization. Do not restore an unrelated user's asset change.

Native checks belong to the coordinator's report: actual installed source/hash, old-state update, fresh offline startup, Gboard, DE/EN, Quick/Classic, White, reveal/release, Back/Home/resume, system text scaling/TalkBack and measured warm return/rematch. Capture the actual phone framebuffer at 1080×1920 for store screenshots. Do not pass Editor fixture renders off as native gameplay.
