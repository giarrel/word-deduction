# Usability final correction and release validation

9 October 2026. Scope: group usability #17/#19 and fluid interactions #18/#20. The correction checkout starts at integration commit `2146f2d817319c3d19757caad2202e359cce0ace`. Free base-mode role counts and reordering unread live handoffs remain outside this package while the required user decisions are pending.

## Standards review

The original [Standards report](standards-review.md) is preserved separately: **zero documented-standard violations**, one optional P3 duplicated-code finding in the two new rendered suites' touch factories. This is a maintainability judgment, not a reproduced product failure.

The existing test-only `ScreenTestActions` now supplies one touch dispatcher with a target, phase, position and optional finger ID. Both suites use it. Began, Moved, Ended and Canceled dispatch their corresponding public pooled pointer event; unsupported phases fail explicitly instead of one suite treating them as release and the other ignoring them. Existing test calls all use the four supported phases. Fixtures, runtime interfaces, assertions and production gestures are unchanged. No new helper-specific tests were added; the public rendered scenarios are the validation seam.

## Spec review

The original [Spec report](spec-review.md) is preserved separately: **zero implementation findings** within #17/#18. It explicitly leaves final Android update/persistence, keyboard/accessibility, paired frame measurements, continuous motion inspection and reduced-motion/privacy acceptance open. Editor tests are not substituted for those native gates.

## Correction validation

- [GroupUsabilityScreenTests](evidence/group-test-result.json): **6/6 passed**, zero failures/skips, 2.9 seconds.
- [MotionScreenTests](evidence/motion-test-result.json): **4/4 passed**, zero failures/skips, 0.9 seconds. This includes captured reorder release/cancel followed by another finger, and preserving a different held contact as a reveal blocker.
- The coordinator's [full Session result](evidence/integration-session-full.txt) is **76/76 passed** at `3835a71035b0fe4c471e5893a54f2438b5c34505`. The Session and Session-test sources are unchanged in this correction.
- The integrated [full rendered result](../fluid-interactions/evidence/integrated-rendered-final-result.json) is **57/57 passed** at production source `33d0d99`, as described in the [motion report](../fluid-interactions/report.md). At the code9 pin, the runtime UI, Session and Android plugin sources were unchanged; only the two affected suites were rerun after the helper consolidation. The later native visual correction is separately scoped below.

Prior intermittent corpus runs that encountered denied `File.Replace` operations remain in the [group evidence](../group-usability/evidence/retained-corpus-failure/inspection.json) and earlier full-run reports. Their failures have not been removed or represented as a proven filesystem fix. The successful later full suite and these targeted checks are separate observed results.

[Console status](evidence/console-status.json) confirms no compilation failure. The two retained [error entries](evidence/console-errors.json) are the already documented `UnityEditor.Search.SearchDatabase` startup exceptions, with no app stack frames. They were preserved, not cleared. Build warnings and actual package results are recorded separately below when available.

The glossary now describes the current group and explicitly preserved earlier paused names, matching the compatibility-only saved-people section. No legacy data, role policy, word catalog or persistence schema is changed.

## Release configuration and evidence boundary

Version **1.2.0 / Android code11** is consistent in the build entry point, serialized PlayerSettings, inspector defaults and reproduction guide. PlayerSettings was changed through the existing Unity `AppBuild.Configure()` entry point; the enabled scene remains `Assets/WordDeduction/Scenes/App.unity`. Existing Unity 6000.3.25f1, Android ARM64/IL2CPP, API26–36, release minification and local debug signing identity are retained.

Both independent reviews were received before any release build. The granted Editor lease belongs only to this correction checkout. Raw import/test/build logs and generated font-cache copies are preserved under ignored `artifacts/usability-final`. Test-generated font data was saved and copied before clearing Inter/Emoji through Unity; no semantic font asset change is retained. No Console clearing, ADB operation, other Editor, installation, global setting, production signing key, issue closure or Store publication is part of this correction.

Clean pinned APK/AAB builds and their actual packaged inspections follow. Native acceptance and motion comparison belong to the coordinator after this Editor and every owned build process have stopped; performance improvement is not claimed from source or Editor tests.

## Independent correction verification

The [Standards correction verification](standards-correction-verification.md) confirms the optional P3 is resolved with zero open Standards findings. The separate [Spec correction verification](spec-correction-verification.md) reports zero new Spec findings. Both concern clean code9 source `e106ce6f388a4e2dce834e4c2fe2b7092b61d47a`; they inspected recorded tests and did not claim new native or test execution.

## Native follow-up: grip and hint presentation

The coordinator's actual code8→9 update preserved both save generations byte-for-byte and passed intentional-edit/scroll/keyboard checks. Comparing retained [code8](evidence/code9/code8-original-group.png) and [code9](evidence/code9/code9-after-update-settled.png) images exposed a visual defect: the Unicode grip rendered as a heavy multi-bar stack, while the repeated Edit explanation forced the hint onto two lines. Both APK and AAB from code9 remain preserved and successfully inspected; this is a superseded candidate, not deleted failed evidence.

The code10 correction uses a non-focusable VisualElement grip with three rounded USS bars. Decorative children ignore picking; the existing tooltip, 44×48 handle area, 60px minimum row height, reorder manipulator and explicit Edit controls remain. DE and EN hints now contain only the short reorder instruction. No new style-mirroring tests were introduced.

The same public rendered suites were rerun after the actual UI change: [GroupUsability 6/6](evidence/code10/group-test-result.json), 2.7 seconds, and [Motion 4/4](evidence/code10/motion-test-result.json), 0.89 seconds. This includes large bilingual maximum-capacity/name/keyboard layouts, valid/canceled/outside reorder, autoscroll, ordinary scrolling and captured-contact handoff privacy. Session and Session-test sources still have no changes; the existing full76/76 result remains applicable. The historical full57/57 render run is not presented as a new run of the modified presentation.

The implementer personally inspected [German Kings](evidence/code10/group-five-kings-de.png), [English Classic](evidence/code10/group-five-en.png), German Classic, 150% German Kings/English Classic and the existing long-name/keyboard captures. Normal-size hints fit on one line; the grip is light and no longer font-dependent. These are 360×640 offscreen Unity renders using a disposable public Session; actual Android system insets and font-scale acceptance belong to the coordinator. Enlarged layouts retain their existing wrapping and accessible Edit controls.

[Code10 Console evidence](evidence/code10/console-errors.json) retains five UnityEditor.Search startup exceptions, all without app stack frames; [compilation status](evidence/code10/console-status.json) is clean. Two initial fixture-tool attempts failed (no scene object after tests; missing extension-method namespace in eval); corrected fixture setup/capture outputs are retained under raw artifacts, and these are not product failures. No Console history was cleared.

The earlier source-equivalence JSON and independent correction-verification reports describe the code9 stage. Code10 production changes are limited to GroupScreen handle construction, its USS and localized hint copy; behavior, Session, Android plugins and gesture code remain unchanged. The build number advances to10 because9 was already installed. Final code10 package evidence follows after building the clean pinned source.

## Code10 package and independent review evidence

Both [Standards code10 verification](standards-code10-verification.md) and the separate [Spec code10 verification](spec-code10-verification.md) found no new findings in the presentation correction. Their scope predates the subsequently reproduced native cancellation defect below.

Clean source `bd7b618c957051d3ee61321a6ed3d275a5302a93`, package-lock SHA256 `da98f245e8c511d47699dc883e521eff5176800341117fcfe8f77fe9dcfb342d`, produced:

| Package | Bytes | SHA256 | Build | Inspection |
| --- | ---: | --- | --- | --- |
| code10 APK | 37,472,054 | `3a73e1c6816ab97aad7df07cd216083cf2b3f8141c409c7c12320ec8c3aaa180` | [270.37s](evidence/code10/apk-build-summary.json) | [14/14 commands](evidence/code10/apk-inspection.json) |
| code10 AAB | 37,828,990 | `9ed3213acc2aff0dc9c674326540799820c29fd4882f3673f6fa71ba6416b617` | [269.67s](evidence/code10/aab-build-summary.json) | [44/44 commands, four packages](evidence/code10/aab-inspection.json) |

Both have zero build errors and two retained warnings: missing RuntimePipelineConfig intentionally disables runtime Pipeline; the pre-existing MatchSurface.cs:89 `EventBase.PreventDefault` API is obsolete. The APK uses the existing Android Debug certificate (`4a0d4929acea4c086bc5534bf462eebcd9d866f31c1bced10b400412117be926`), v2 signature verified. These are locally signed release configurations, not owner-signed Store submissions.

The APK's actual DEX retains the eight input classes and contains no debug/info/verbose input-log calls. Packaged manifest has no network permissions and excludes backup; boot.config has no player connection and debugger wait is0. [APK](evidence/code10/apk-managed-stripped.txt) and [AAB](evidence/code10/aab-managed-stripped.txt) stripped assembly lists contain no test, Pipeline, Analytics, Connect or Consent assembly.

For code10, all six ARM64 libraries pass LOAD16K checks and have zero rounded-RELRO writable intersections. Scalar GNU_RELRO end residues remain nonzero: libc++12288, game4096, il2cpp8192, main8192, swappy4096, unity8192 bytes. These scalar failures remain explicit; static inspection and translated emulator execution do not prove physical ARM64/16KB-kernel behavior or Play acceptance. Both complete code9 and code10 pairs and raw inspection outputs are retained.

## Native follow-up: operating-system cancellation

After the successful code9→10 update, the coordinator isolated a real operating-system cancel defect. A combined cancel/outside trace first went red; then two separate `Down → Move → ACTION_CANCEL` sequences, with no Up, swapped Nora/Luca in saved state. The separately verified outside release preserved the original bytes. The first combined PowerShell batch had nonterminating assertions and is retained as failed evidence, never a pass. Original input logs, screenshots and both save generations are copied under ignored `artifacts/usability-final/code11/native-red`.

The accepted regression seam is the rendered app fed by public `InputSystem.QueueStateEvent`/`Touchscreen` events through the installed InputForUI provider, then public `Session.Open` observation. It uses an actual screen panel: a RenderTexture fixture with directly pooled PointerCancel cannot reproduce this engine mapping. The [first red](evidence/code11/provider-red-attempt1.json) reproduced the saved-order symptom; the [diagnostic red](evidence/code11/provider-red-phase.json) proved `PointerUp=1`, `PointerCancel=0`, and the actual Touchscreen phase `Canceled` at release. Installed primary source `InputSystemProvider.OnClickPerformed` maps an unpressed action directly to ButtonReleased without checking cancel phase; its complete reference source is retained in raw evidence.

The small production correction checks the released pointer's actual Touchscreen slot before committing a group drop. Canceled native touches use the existing cancellation and exact-contact cleanup. It does not clear every contact, modify the input package, change persistent data, add platform logs, or alter ordinary gesture paths. Pointer IDs use the provider's slot index, not the native touchId.

The same test turned [green in0.38s](evidence/code11/provider-green-completed.json). It was extended with actual outside and following valid-release controls, [green in0.41s](evidence/code11/provider-controls-green.json), so suppressing every drop cannot satisfy it. The first fix import had a `PointerType` namespace ambiguity; qualifying the existing UIElements type repaired compilation, and the blocked test then completed. The original compiler/runner diagnostics remain in the Editor log rather than being removed. Full rendered regression and clean code11 artifacts follow.

The first post-fix [full rendered run](evidence/code11/full-rendered-completed.json) completed **58/58 passed**, zero failures/skips/inconclusive, in205.04 seconds. It includes the actual-provider regression, ordinary group tests, captured release/cancel and other-held-finger privacy, content/layout, recovery and game-flow scenarios. No Session or Session-test source changed. Existing minimal test fixtures log missing-theme warnings; the production Panel asset is unchanged. [Compilation status](evidence/code11/console-status.json) reports no compilation failure; [retained error history](evidence/code11/console-errors.json) includes the prior reproduced test failures, repaired compile error and recurring UnityEditor.Search exceptions. Successful checks do not erase this history. Test-generated Inter/Emoji assets were saved/copied and cleared through Unity; no semantic font diff remains.

## Code11 Standards verification

The separate [Standards review](standards-code11-verification.md) found no documented-standard violations or actionable maintainability findings at `976c0fc49742898e41fa21d2392cd34fec6a73f8`. It explicitly left the same-update event sequence unverified. This original report is preserved unchanged.

## Code11 Spec verification and same-update correction

The separate [Spec review](spec-code11-verification.md) identified **P2: preserve cancellation before a touch slot is reused**. InputForUI buffers its PointerUp until a later dispatch; Touchscreen immediately reuses a canceled slot when the next Begin arrives. Reading only the current slot phase at UI dispatch therefore loses the terminal reason. This original finding is preserved unchanged.

The exact public-provider regression queues Cancel and a new Begin with no intervening yield. It reproduced the issue [red,0/1](evidence/code11/provider-batch-red.json). An intermediate raw-event observer also [failed](evidence/code11/provider-batch-latch-red.json): [temporary tagged diagnostics](evidence/code11/batch-latch-diagnostics.txt) show control-relative ReadValueFromEvent returned false for the TouchState event format. That attempt and its logs remain evidence, but the temporary logging and unsuccessful observer were removed from production source.

The final approach registers a state-change monitor on the active native touch's phase. The supported InputState API calls it after each state write, before the next queued Begin overwrites the slot. It latches Canceled only for the captured native touch ID and retains that fact until the buffered UI release. Every existing cancel, capture-loss, detach and release path removes the monitor. Mouse/pooled-pointer fixtures still use the existing UI event path. Exact-contact cleanup, privacy blockers, Session validation and save schema remain unchanged.

The original same-update regression then [passed,1/1](evidence/code11/provider-batch-monitor-green.json). Extending it with a following valid drag proves the latch is reset; [both actual-provider tests passed,2/2](evidence/code11/provider-all-monitor-green.json), including the original separate-frame cancellation, outside-release and valid-release controls. The [full rendered regression](evidence/code11/full-rendered-monitor-completed.json) then passed **59/59**, zero failed/skipped/inconclusive, in209.23s after this lifecycle change. [Compilation status](evidence/code11/console-monitor-status.json) is clear; [Console history](evidence/code11/console-monitor-errors.json) retains the original failed tests and prior Editor exceptions. Generated font assets were saved, copied, cleared through Unity and verified to have no semantic diff.

The [initial code11 APK](evidence/code11/apk-initial-build-summary.json), from source `976c0fc49742898e41fa21d2392cd34fec6a73f8`, built successfully in285.74s with zero errors/two known warnings and passed14/14 inspection commands. SHA256 `3c2d112cfc4d99aed516f5c7d6ee9034151a8b3bbc26e3fbc75d7c812ae7c957`,37,471,958 bytes. It was **never installed** and is superseded by this same-version correction; its artifacts remain unchanged and no AAB was built from that pin. Version1.2.0/code11 is retained with a new immutable source/artifact directory.

## Code10 to final code11 source equivalence

Compared with installed code10 source `bd7b618c957051d3ee61321a6ed3d275a5302a93`, the only runtime UI source change is `GroupReorder.cs`, which handles native cancellation as described above. Group construction/copy/USS, explicit editing, long-name layout, match/card presentation and privacy code, Session and its tests, Android plugins and package lock are unchanged. Additional changes are the actual-provider test and its InputSystem reference, release code11 configuration/inspector defaults, and evidence/documentation. This supports reuse of the coordinator's code10 native rename, legacy, large-group, bilingual150% and card-layout observations for the unchanged UI; it does **not** replace code11 native cancellation, package/update, privacy or controlled motion acceptance. The existing full Session76/76 result applies without rerunning unchanged Session sources.
