# Role-count update: build and rendered acceptance

9 October 2026. Ticket [#23](https://github.com/giarrel/word-deduction/issues/23), [specification](../../specs/role-counts.md). Build and rendered criteria pass. **Root's actual Android update, native gameplay/accessibility checks and final 34-story matrix remain separate acceptance requirements.** This handoff does not close the ticket or declare Store readiness.

## Source and tests

Both accepted artifacts use clean combined source **`b6e6a926bbcc58c7103d79a9f117cbd09e4269e3`**. It combines corrected gameplay `d3ad9d0ecd42f4bfb46e2e09f75f914e07a02e59` with version preparation `bcffd96114896f32606f66067c4060edfccc4d1d`. The only production/configuration changes from that corrected source are the three reviewed version values in AppBuild, PlayerSettings and inspector defaults. Later commits in this handoff contain documentation/evidence only.

- Public Session **93/93** is reused from the [post-correction run](../role-review-corrections/report.md), exit 0, 299.56 seconds. [Exact source proof](evidence/source-proof.json) compares the complete Session/UI/tests/plugins/packages/content trees and the reviewed version files. This is reused execution evidence, not another test run.
- The final complete rendered PlayMode suite ran once on the combined source: **66/66**, zero failures/skips/inconclusive, **221.91 seconds**. [Result](evidence/rendered-full.json); [compile status](evidence/compile.json). It exercises the production screens/input, role controls, sequential Quick, multiple Whites, Kings starter and existing group/privacy behavior.
- Independent Standards and Spec reviews have **zero open findings** after the single shared-starter correction. [Clearance and both rechecks](evidence/review-prebuild-clearance.md). Source review does not replace native acceptance.
- Four fresh original-resolution views were actually inspected after the final suite: [DE Quick 3 replacement](evidence/screenshots/role-counts-German-Quick-3-swap.png), [EN Classic 20 bounds](evidence/screenshots/role-counts-English-Classic-20-large.png), [DE Kings starter](evidence/screenshots/starter-small-German-True-top.png), [EN Kings last participant](evidence/screenshots/starter-small-English-True-last.png). These use the existing small-screen/150% fixtures. Actions and full limits/names remain reachable by scrolling; the images do not imply simultaneous visibility. The English Help preview discrepancy is byte-identical to the previously pixel-verified PNG (`84d93a6e...`); no font change was introduced to compensate for the preview. [All 175 rendered outputs and inspected-image list](evidence/rendered-preservation.json).

## Accepted artifacts

Unity **6000.3.25f1**, existing Pipeline **0.8.0-exp.1**, CLI **1.0.0-beta.11**; **1.3.0 / code 12**, `com.giarrel.worddeduction`, ARM64 IL2CPP, Android minimum 26 / target 36, OpenGLES3, portrait, nondevelopment. Package-lock SHA256: `da98f245e8c511d47699dc883e521eff5176800341117fcfe8f77fe9dcfb342d`.

| Artifact | Bytes | SHA256 | Build time |
| --- | ---: | --- | ---: |
| APK | 37,480,110 | `cc0ab333bdb130cda07e45ec64a7eab68887d8a0384a9325506827885011aedc` | 341.04 s |
| AAB | 37,837,035 | `b302d3a9bc9628870ce953156c2990d6491d3e0ad1a063a942f970a84bca15a6` | 332.67 s |

The accepted original directories are `artifacts/android/b6e6a926bbcc/20261009T194843422Z-apk/` and `artifacts/android/b6e6a926bbcc/20261009T195632345Z-aab/` in the implementer checkout, each containing `WordDeduction-local-release` with its appropriate extension and a build summary. Independent byte-verified copies live outside the removable checkout at `C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/role-counts/release-build/packages-raw/`, retaining the timestamped directory names. [Preserved files/hashes](evidence/packages-preservation.json), [APK summary](evidence/apk-build-summary.json), [AAB summary](evidence/aab-build-summary.json). Unity's reported build-input total is not the download size.

Each accepted build succeeded with **0 errors, 2 warnings**: intentional disabled runtime Pipeline and the existing obsolete `PreventDefault` call. [APK warnings](evidence/apk-warnings.json), [AAB warnings](evidence/aab-warnings.json). No build was duplicated because a callback had not returned; each dispatch/result is retained in the raw evidence.

Both use the existing local Android Debug certificate, SHA256 **`4a0d4929acea4c086bc5534bf462eebcd9d866f31c1bced10b400412117be926`**. [APK verification](evidence/apk-signature.txt), [AAB public certificate](evidence/aab-certificate.txt). This preserves the local update identity and is not production signing.

## Inspection and provenance

The exact APK passes **14 commands / 1 package**; the AAB passes **44 commands / 4 packages**, including generated universal, base-master and ARM64 split packages. Every inspection command exits 0. [APK inspection](evidence/apk-inspection.json), [AAB inspection](evidence/aab-inspection.json), [native-layout summary](evidence/package-layout-summary.json).

Checks cover package/version/SDK/ABI, nondebuggability, absence of network/sensitive permissions, private-save backup exclusions, no player-connection/debugger boot settings, retained GameTextInput with no debug/info/verbose logging, APK signatures and zip alignment. Bundletool validates the AAB and its `PAGE_ALIGNMENT_16K` configuration. Every native LOAD has compatible 16-KiB alignment/congruence; rounded RELRO protection overlaps no writable payload outside RELRO. Nonzero scalar RELRO end remainders and compressed native entries remain explicitly recorded; neither zipalign nor this static check proves physical ARM64/16-KiB-kernel execution.

AAB jarsigner reports a verified archive while retaining its local self-signed/untrusted-chain, timestamp and manifest-order warnings. [Full signature output](evidence/aab-signature.txt) is preserved. This is not Play acceptance.

The APK passed a post-build source/content check before release to Root. Unity rewrote only the original Inter/Emoji YAML from CRLF to LF; [exact proof](evidence/font-line-ending-proof.json) establishes the previous raw bytes were the Git content with CRLF and the new raw bytes equal the original Git LF bytes. Complete canonical Git blobs are unchanged. Raw before/after hashes are deliberately not called identical. Refreshing only these unchanged index entries leaves [Git clean](evidence/apk-source-check.json). The AAB then preserves both canonical content **and** these LF bytes exactly, with [clean post-build state](evidence/aab-source-check.json).

## Rejected first candidate and corrected build procedure

The first APK (`0220c9eefcc523754ded30423e3906d7a9f88730c340b6d237bbe5560cfaf684`) passed static inspection but was **rejected before installation**. After the rendered run, its Editor still held font feature data in memory. AppBuild's initial source guard saw a clean tree, then Configure/SaveAssets serialized 224,971 additional Inter glyph-pair adjustment lines during the build. The next AAB preflight detected this real source drift and prevented dispatch. This first APK is historical evidence, not an accepted release artifact.

The full diff, generated assets, first APK and all logs remain outside the worktree. [Incident and reusable procedure](evidence/rejected-candidate.md). The test Editor was closed, only its generated assets restored, and builds repeated in a fresh session without PlayMode. A failed restore due to Windows safe.directory path separators and an owned test-scene recovery prompt are retained as infrastructure mistakes. The second restore used the exact normalized path; the test recovery directory was inventoried and moved outside the checkout, never applied to a tracked scene. No gameplay, pipeline implementation, dependency or test change was made to resolve this.

The [reproduction guide](../../release/reproduction.md) now requires a fresh build session after PlayMode, configuration before the source check, and post-build source verification before releasing any package to a device. A source commit alone does not prove asset equality when live generated state remains in the Editor.

## Handoff and remaining criteria

| Criterion | Status in this handoff |
| --- | --- |
| Integrated Session and rendered behavior | Passed: exact-source 93/93 plus fresh 66/66 |
| Independent Standards and Spec review | Passed: correction reviewed; zero open findings |
| Editor visuals and DE/EN large-text controls | Passed within the recorded fixture/rendered scope |
| APK/AAB build, identity, inspection and provenance | Passed for the two accepted hashes above |
| Actual code 11 update, native state/restart/play/accessibility | Not run by this implementer; Root owns these required checks |
| Full 34-story final matrix and ticket closure | Pending Root's combined native acceptance |

Owned build Editor **244496** was verified stopped through the actual Unity exit API and process check: [termination](evidence/editor-exit.json). An earlier delayed exit callback did not terminate it and is not counted as a successful shutdown. Original logs, command outputs, inspected package extracts, scene-backup inventory, screenshots and failed attempts remain at `work/role-counts/release-build`; none requires the implementer worktree to remain. No ADB action, new installation, signing change, unrelated Unity process, push or tracker mutation was performed by this implementer.

No physical-phone smoothness, audible TalkBack, haptics, real-group enjoyment or Store readiness is claimed. No broad test rerun is required for these subsequent documentation/evidence commits; their runtime inputs must remain equal to the build pin during the separate merge.
