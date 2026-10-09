# Usability 1.2.0/code11 integration and preservation

9 October 2026. A separate merger integrated handoff `d4b1d2cbaf01e11cfb627ec2b2b2e6a8610850bb` into `integration/usability-v1.2` at `3d0f098859277705a19c930ba520ec9fd04dda15`, without conflicts. Merge commit: **`1d9fcece9bd1752d25db56e57d28b8e16a8f6082`**. No implementation was changed during merging.

The handoff differs from exact built/reviewed source **`25d3e9f9d80609c178230cf838cecfda54ae0370`** only in documentation and evidence. The merged `game`, `content`, `tools`, `tests` and `third-party` tree identities match that pin exactly; [verification](verification.json) records the identities and independently checked package hashes. The [implementer report](../usability-final/report.md) retains all previous candidates, failures, fixes, package inspections and review findings.

## Preserved worktree evidence

The complete ignored `artifacts/` and small `game/Logs/`, `game/Temp/`, `game/UserSettings/` and `game/.utmp/` content was copied into a new archive, preserving paths relative to the worktree. All **808 files / 2,703,119,562 bytes** were checked against source and destination using SHA256 and byte counts; the source inventory was rechecked after copying. This includes all **748 files / 2,700,522,954 bytes** listed by the implementer's raw manifest, its own additional manifest file, and the small ancillary files. The original raw manifest SHA256 is `3afd862f2f7c7ec803398c6ecb812974f9bd397755bdb6d2f378279ae53b1c34`.

The [preservation manifest](worktree-preservation.json) maps every file under [the durable local worktree archive](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/worktree/). Its raw paths retain code9/code10, superseded code11 stages, final packages, generated delivery packages, original failures, font caches and logs. Existing earlier archives were not overwritten. These large binary/raw directories remain local and Git-ignored; the manifest and selected reports are tracked.

## Stable local release files

| Package | Bytes | SHA256 |
|---|---:|---|
| [APK](../../../artifacts/releases/1.2.0-code11/WordDeduction-1.2.0-code11.apk) | 37,472,202 | `9c08b19ab2270b236cd2eec9a8d9bd2f67b7a9db93fc0ff3d5d5f2a5d5e8c9ad` |
| [AAB](../../../artifacts/releases/1.2.0-code11/WordDeduction-1.2.0-code11.aab) | 37,829,159 | `9148cb972b7c6b256ad0838f084ec3bfe2693e3cfbd70bcd5885e4b6623d930e` |

Both stable copies and their archived originals match the exact-source build summaries. [Local provenance](../../../artifacts/releases/1.2.0-code11/provenance.json) and [checksums](../../../artifacts/releases/1.2.0-code11/SHA256SUMS.txt) accompany them. These are Unity 6000.3.25f1, ARM64/IL2CPP, API26–36, version1.2.0/code11 release configurations signed with the existing local Android Debug certificate; production signing and Play submission remain owner actions.

The recorded final APK inspection passed 14/14 commands and AAB inspection 44/44 across four generated packages. Both independent terminal reviews reported zero findings. The actual full rendered run passed **61/61**. The successful **76/76 Session** run is reused with an explicit empty source diff for Session and Session.Tests, rather than described as a new run here. No test, Editor, ADB or build command was performed by this merger. Native acceptance and motion limitations belong to the coordinator's separately frozen records.

## Native acceptance and final cleanup

The complete frozen native directory is now preserved: **1,127 files / 117,521,959 bytes**, including original failures, scripts, traces, videos, save generations and the auditor's original reports. [Native preservation manifest](native-preservation.json); [durable raw archive](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/native/). The [32-story matrix](../usability-native/acceptance-matrix.md), [final native audit](../usability-native/code11-native-audit.md) and [motion comparison](../usability-native/motion-comparison-report.md) have permanent relative links and selected small evidence. Their raw originals remain unchanged. The current reading copies clarify that Root explicitly restored the original save generations immediately before 10→11; the actual `install -r` attempt itself performed no replay and preserved both files. The real update chain is not described as uninterrupted use without restoration.

Native code11 evidence confirms cancellation/replacement-contact and outside-drop preservation, a deliberate atomic autoscrolling reorder, direct new match/reveal/release/Next, safe recovery after process restart, and final original-state restoration. The earlier incorrect fixed-index harness assertion remains a recorded failure, followed by a separate audited result. **Stories 18.1, 18.2 and 18.6 remain only partially demonstrated empirically.** Controlled code8→10 comparisons improved medians to approximately 32–34ms, but tails remained roughly 68–80ms p95 and 136–200ms maximum. This is neither guaranteed hitch freedom/60fps nor a physical-phone or CPU/GPU-cause claim. The implemented #17/#18 scope includes this transparent measurement acceptance, not a new guaranteed physical-60fps requirement.

Existing reproducible measurement sources are retained without execution: [MotionScheduled](../usability-native/evidence/MotionScheduled.py), [MeasureFrames](../usability-native/evidence/MeasureFrames.py), [AnalyzeScheduled](../usability-native/evidence/AnalyzeScheduled.py), [ConsoleRecord](../usability-native/evidence/ConsoleRecord.py), [harness preparation](../usability-native/evidence/prepare-motion-tools.py) and [native input source](../usability-native/evidence/input-timing/WordDeductionScheduledInput.java). They retain their original explicit local SDK/ADB paths; data labels resolve beside the scripts. The full raw archive preserves the original layout, built helper and SDK reference, while recorded video remains excluded from performance measurements.

After explicit final cleanup approval, all worktree and native source/copy inventories, byte counts and SHA256 values were reverified. The exact correction worktree had its merged clean head, no reparse points and no Editor on its project. Git removed only that worktree using per-command long-path support and force solely for inventoried ignored/generated files. The branch `implement/usability-final-corrections` remains at the handoff; the native source directory and unrelated Editors were untouched. [Cleanup record](cleanup.json).

No push, issue closure or Store publication was performed by this merger. Free base-mode role counts in [#11](https://github.com/giarrel/word-deduction/issues/11) remain open and unimplemented, outside the delivered group/motion scope.
