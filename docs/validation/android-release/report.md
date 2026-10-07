# Android release candidate: build and static acceptance

7 October 2026. Ticket [#9](https://github.com/giarrel/word-deduction/issues/9), implementation base `922e4b7b387ff0c0219630725ccacf6d7be5848b`. The final APK and AAB were both built from committed, clean **`ef261b030a0396e7deca55a0511b5d31c2081874`**. Subsequent changes in this handoff are inspection tooling, store wording and documentation; no runtime changes are hidden behind these artifacts.

Build/static checks pass at the scope described below. **Coordinator native acceptance, final screenshots and the independent Standards/Spec reviews are pending in this handoff.** This report does not close ticket 9 or declare the app published. Production signing, public publisher/contact/policy URL and store eligibility are explicit owner prerequisites.

## Exact artifacts

| Artifact | Actual bytes | SHA256 | Build duration |
| --- | ---: | --- | ---: |
| Local nondevelopment APK | 39,431,579 | `4d7f9b2d7eeceb5df3669025976276cb71bb2ba6e8eb4591019a796578d49ec0` | 264.29425 s |
| Local nondevelopment AAB | 39,342,141 | `f5b5cc327a70fc2349d4ce86a8d280cd78f2397f32b1cb19122d0f381a3863f0` | 264.6668275 s |

Paths relative to the build checkout are `artifacts/android/ef261b030a03/20261007T005044376Z-apk/WordDeduction-local-release.apk` and `artifacts/android/ef261b030a03/20261007T005648918Z-aab/WordDeduction-local-release.aab`. The [APK report](apk-build-summary.json) and [AAB report](aab-build-summary.json) preserve independent source/options/version/signing/size/hash records. Unity's much larger `reportedBytes` counts build inputs and is not the file download size. Preserve the artifacts before removing the ignored build checkout.

The independent [merge verification](merge-verification.md) records the integration test rerun and the verified copies now preserved in the output repository under `artifacts/android-release/`.

Unity **6000.3.25f1**, ARM64/IL2CPP, min SDK26, target/compile SDK36, OpenGLES3, portrait, package `com.giarrel.worddeduction`, version **1.0.0 / code2**. Package-lock SHA256 `da98f245e8c511d47699dc883e521eff5176800341117fcfe8f77fe9dcfb342d`. Installed Android tooling: OpenJDK17.0.18, NDK27.2.12479018, build-tools36.0.0, AGP9/Gradle9.3.1. This is a repeatable pinned build procedure, not a claim of byte-identical Unity builds.

## Reproduction and provenance

Use the [release reproduction guide](../../release/reproduction.md). A single resident, stopped Editor was opened on the exact release worktree with Android active. Each build was dispatched once; the APK completed before AAB dispatch. Exact CLI executable was `C:/Users/lucac/Documents/Codex/2026-09-27/wenn-ich-digitalversion-von-domination-species/work/toolchain/unity-cli/bin/unity.exe`; project path was `C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/worktrees/release/game`. CLI1.0.0-beta.11 used the existing Pipeline0.8.0-exp.1.

```powershell
$env:UNITY_NO_UPDATE_CHECK='1'
$env:UNITY_NON_INTERACTIVE='1'
unity command eval 'WordDeduction.Editor.AppBuild.ReleaseApk(); return "APK complete";' --timeout 1800000 --detach --project-path '<exact-release-checkout>/game' --caller plugin --skill unity-cli --format json
unity job status 4cc66bb079b547fbbe44918282f8db3a --project-path '<exact-release-checkout>/game' --format json
unity command eval 'WordDeduction.Editor.AppBuild.ReleaseBundle(); return "AAB complete";' --timeout 1800000 --detach --project-path '<exact-release-checkout>/game' --caller plugin --skill unity-cli --format json
unity job status a740977d223443cc85220b6819b3a9ef --project-path '<exact-release-checkout>/game' --format json
```

Both actual Unity reports say **Succeeded, 0 errors, 2 warnings**. No build was redispatched because of a callback timeout. The two warnings are: Pipeline has no RuntimePipelineConfig and will be disabled in players (intentional); existing `MatchSurface.cs:74` uses obsolete `EventBase.PreventDefault()` (secret-input behavior retained and regression-tested). Logs remain under `artifacts/release/editor-live.log`. Dedicated Editor PID184784 subsequently exited gracefully, with normal subsystem shutdown confirmed; other Editors were not touched.

Release entrypoints force and restore development/debug/profiler/deep-profiling/player-log/bundle/signing settings. Git rejects all nonignored tracked **and untracked** changes before build. A real temporary untracked file caused the [expected pre-build refusal](untracked-source-rejected.json); the file was then removed. ProductionBundle without credentials caused the [expected refusal before any artifact](production-missing-key.json). A first pre-build attempt exposed Windows Git safe-directory slash handling; the helper now normalizes that exact path. The signing-path restore also preserves its exact original serialized value, keeping successful builds clean.

## Package inspection

The final inspector completed **13 APK commands** and **42 AAB/delivery commands**, each with exit code0. Full argument vectors, file hashes and all ELF program headers are recorded in [APK inspection](inspection.json) and [AAB inspection](aab-inspection.json); adjacent text files retain stdout/stderr. The inspector uses the installed JDK explicitly. `keytool -printcert -jarfile <exact.aab>` additionally records the [public AAB certificate](aab-certificate.txt).

Committed text exports normalize line endings and trim trailing whitespace from tool output; original unmodified captures remain in the ignored artifact inspection directories.

- APK and AAB base/universal manifests have no INTERNET/ACCESS_NETWORK_STATE or camera/microphone/contacts/location/storage runtime permissions. The sole receiver permission is the AndroidX package-specific signature permission. Release `debuggable` is absent. The coordinator independently checks installed nondebuggability.
- `allowBackup=false`, predictive Back enabled, `extractNativeLibs=true`. Actual renamed compiled backup resources were resolved from the resource table, then read: file-domain `word-deduction` is excluded from legacy backup, modern cloud backup and device transfer. The same backup checks pass in bundle-derived universal/base packages.
- APK and AAB boot.config contain no PlayerConnection or managed-debugger setting; native debugger wait is0. Build manifests/version/SDK/ABI agree with the stated identity.
- Runtime dependency evidence: [stripped managed assemblies](managed-stripped.txt), [generated Gradle dependencies](generated-unity-library.gradle.txt), [304 generated C++ files searched](il2cpp-runtime-exclusions.json). No runtime Pipeline driver, test assembly, visual-fixture helper or typography debug message survives. The upstream ScriptingAssemblies inventory lists unused modules and is not confused with the stripped runtime. Unity analytics startup/device-stat submission and ads settings are disabled; player logging is off. There is no ad, billing or analytics service integration.

The original release candidate on `e6c676be0e0e9053bfdc3711bd9352af3349ffef` retained INTERNET even with Unity's force flag disabled. Its [actual manifest-derived badging](permission-red-badging.txt) and [failed inspection](permission-red-inspection.json) are preserved. `OfflineReleaseManifest` now removes network permissions from nondevelopment generated manifests. The final APK/AAB and all delivered manifests pass the same boundary.

Inspection harness corrections are distinct from app defects: default sandbox execution denied llvm-readelf access; normal-user execution succeeded. An early wrapper lacked JAVA_HOME; the final inspector sets the bundled JDK. aapt renames backup XML files, so lookup now follows resource names. Isolated ABI-split `aapt badging` invents legacy implied permissions because uses-sdk is inherited from base; the final verifier reads actual XML declarations and separately verifies SDK26/36 in base/universal packages. The ABI split XML declares none of those legacy permissions.

## Signatures and native delivery

All artifacts deliberately use the local **Android Debug** certificate, SHA256 **`4a0d4929acea4c086bc5534bf462eebcd9d866f31c1bced10b400412117be926`**, RSA2048. APKv2 passes; generated APKv2/v3 pass. This matches the earlier local development installation and enables the coordinator's update test. It is not a production signing identity.

AAB `jarsigner` exits0 and says `jar verified`, while reporting a self-signed/untrusted chain, no timestamp, POSIX attributes outside signature coverage, and JarInputStream versus JarFile manifest-order warnings. These warnings are preserved verbatim in [AAB signature output](aab-bundle-signature.txt). Bundletool validates the archive and successfully generates all packages. Production owner signing and Play's validation remain separate gates.

| Bundle-derived package | Bytes | SHA256 |
| --- | ---: | --- |
| universal.apk | 39,469,587 | `5e59fa8a019f05492b664d61a69f3b4ef06a8aa8068e01c40df7062a779ff4dd` |
| base-arm64_v8a.apk | 22,721,236 | `9079ecfa71791deb4524e424e517a361e6442e8e61b4fc5847f0b5d23769ec05` |
| base-master.apk | 16,756,829 | `dd8cbc87ae88d6e4d24533ce8b925f689e8f7e0af6f10be86368c40401837254` |

Bundletool1.17.2 validates and dumps the AAB; its build metadata reports bundletool1.18.3 from the build chain. Config explicitly specifies **PAGE_ALIGNMENT_16K**. The default delivery set contains base-master and base-arm64_v8a; the universal package is inspected separately. Each package's signature/zipalign passes. Every native library in the direct APK, AAB, universal APK and native split has LOAD alignment at least16384 with matching virtual-address/file-offset congruence. Rounded16KB GNU_RELRO protection overlaps **zero writable payload bytes** outside RELRO.

Scalar GNU_RELRO end alignment is **not** a universal pass: libc++_shared12288, libgame4096, libil2cpp8192, libmain8192, libswappywrapper4096, libunity4096 are the nonzero remainders modulo16384. Full segment intersections are retained, not hidden or patched. All native entries in these actual packages are compressed (ZIP method8), with extraction enabled. `zipalign -c -P16 4` passes but does not demonstrate uncompressed native-entry alignment for these compressed entries. No engine upgrade or binary patch was inferred from a scalar remainder.

The coordinator separately probes the actual installed ARM64 app in an Android16KB application environment with compatibility fallback disabled. That translated x86_64 emulator is not a physical ARM64/16KB-kernel device or a Play acceptance result. Its native report must retain that boundary.

## Behavioral, visual and store work

Optional group Info exposes DE/EN local privacy and original Inter, Noto Color Emoji, Unicode and Newtonsoft/Json.Net.Unity3D notices. It occupies the existing brand position and keeps group edits/drafts intact. [Rendered red](info-red.json) failed because the entry did not exist; [rendered green](info-green.json) passes the DE/EN flow, privacy, readable license text, minimum target and return without group/draft mutation. [Full PlayMode suite](full-rendered-green.json): **28/28 passed**,104.64s, no failures/skips. The preceding integration had53 Session scenarios including766 Unicode conformance cases; this release adds no Session rule/storage behavior. Merger reruns remain independent evidence.

The final [source bilingual audit](bilingual-source-audit.json) finds131 unique keys with two nonempty translations; the five Info keys extend the previous compiled126-key audit. This source check is not relabelled as a compiled probe. Dynamic Inter/Emoji caches populated during rendered tests were cleared with the font API, preserving the accepted font/emoji assets and scene.

[Store preparation](../../release/data-safety.md) links DE/EN copy, release notes, the actual privacy-policy draft and owner prerequisites. The original512×512 icon and two1024×500 RGB feature graphics were opened and visually accepted independently by implementer and coordinator. Store copy accounts for White having no word and repeated Classic ties causing no elimination. Final store screenshots must come from the coordinator's real1080×1920 final-release framebuffer; none are fabricated or substituted with Editor fixtures here.

## Remaining acceptance

The coordinator is completing fresh offline native games, update/restart byte checks, privacy/lifecycle checks, Info/large-text operation, measured warm return/rematch and final store screenshots. Preliminary reports are deliberately not marked complete in this handoff. Independent Standards and Spec reviews use pre-implementation baseline `ab25c325e02040d30755ae448c07789357730f38` after integration. No physical-phone performance, physical haptics, TalkBack audio or real-group fun/balance is claimed by static checks and self-play.

At handoff, the coordinator has two confirmed small-phone/150%-text findings for the single final correction pass: **P2** the disabled Play button has no visible too-few-players reason because the large-type layout hides it; **P3** the German empty-group instruction clips its final word. The actual Info/privacy/licenses route passed DE/EN150% scrolling and Android Back. These are known outstanding UI findings, not waived by the28 passing tests. The coordinator retains their actual screenshots and will integrate that evidence with the correction and native report. The closed release Editor was not reopened for an uncoordinated competing fix.

The [owner handoff](../../release/data-safety.md#owner-prerequisites-after-local-acceptance) names the exact remaining public identity, contact/HTML policy URL, Play-account/testing, dedicated signing and publication decisions. No key was copied from another project, no passwords requested, no store content published.
