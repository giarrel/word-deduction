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

## Code11 monitor candidate and further buffered-contact findings

The source `154388a64d15f8510f532b14f9d90ac7d46816a1` produced a preserved intermediate pair, never installed. [Standards verification](standards-code11-monitor-verification.md) reported zero findings at this pin. The separate [Spec verification](spec-code11-monitor-verification.md) closed the original P2 and identified two further P2 event-ordering cases before native release approval. Both reports are preserved unchanged. The already running AAB was allowed to finish safely; no source was changed while either build ran.

| Intermediate package | Bytes | SHA256 | Build | Inspection |
| --- | ---: | --- | --- | --- |
| code11 monitor APK | 37,471,766 | `640e2388f3807f1c8b61149fad5fb71ac1090fd538299f424ae7622102a8f480` | [262.24s](evidence/code11/apk-monitor-build-summary.json) | [14/14](evidence/code11/apk-monitor-inspection.json) |
| code11 monitor AAB | 37,828,711 | `af73f0ab569d5bf978babd94b32eb911752391265a36073bd00b0e59b31ae41d` | [279.55s](evidence/code11/aab-monitor-build-summary.json) | [44/44](evidence/code11/aab-monitor-inspection.json) |

Both have zero build errors and the same two documented warnings, debug signing identity and static alignment/privacy limitations. Historical raw filenames containing “final” describe this intermediate pin; their names are not a native acceptance claim and their contents remain unchanged.

Two additional public-provider tests reproduced both findings [red,2/4](evidence/code11/provider-two-gaps-red.json), while the earlier two controls stayed green. First, Begin→Move→Cancel→new Begin entirely within one update delivered its initial UI Down only after the native slot had already changed. Second, an intentional End→new Begin→that new contact's Cancel incorrectly discarded the original valid drop by reading the replacement slot's Canceled phase.

The private gesture adapter now remembers both terminal reasons for its own native contact. It accepts a native drop only if the contact observed at Down was still running and its deliberate End is observed for that captured ID. A remembered original End survives a later contact's cancellation. An ambiguous fully buffered gesture whose contact is already terminal cannot authorize a saved reorder; the preview is discarded. This conservative behavior preserves durable order when the provider has lost contact identity. All monitor removal and exact-contact cleanup paths remain unchanged. No rendering, animation, Session, schema or Android plugin changes are introduced.

All four provider cases turned [green,4/4](evidence/code11/provider-terminal-green.json). Explicit normal follow-up drags were then added to the two new scenarios; [4/4 passed again](evidence/code11/provider-terminal-controls-green.json),0.76s. This verifies both rejection of canceled previews and continued usability of intentional subsequent drags. A full61-test rendered regression and both final independent reviews precede further release builds.

The first broader run exposed a separate [fixture-isolation failure,57/61](evidence/code11/full-rendered-terminal-completed.json): three GroupUsability scenarios and one Motion scenario used directly pooled touch events while the Editor also exposed an unrelated idle native Touchscreen. A [targeted diagnostic](evidence/code11/idle-slot-probe.txt) proved `id=0, phase=None`; [the pooled reorder control](evidence/code11/idle-slot-probe-result.json) failed because it had no actual native contact whose End could be proven. All four genuine provider scenarios passed in that same full run. This is preserved separately from the two confirmed product defects.

GroupUsabilityScreenTests and MotionScreenTests now inherit Unity's already installed InputTestFixture, which saves/restores InputSystem and isolates pooled fixtures from host devices and platform input. Its documented public test API supports PlayMode UnityTest and starts with no devices. Only the test assembly adds the existing TestFramework reference; packages and production logic were not weakened for synthetic events. NativeTouchScreenTests deliberately remains on the actual provider. Targeted [Group6/6](evidence/code11/group-isolated-result.json), [Motion4/4](evidence/code11/motion-isolated-result.json), and [real provider4/4 after restoration](evidence/code11/provider-after-isolation-result.json) passed. All temporary diagnostics were removed from source; their original output remains evidence.

The final [combined rendered run](evidence/code11/full-rendered-isolated-completed.json) passed **61/61**, zero failures/skips/inconclusive, in204.37s. [Compilation status](evidence/code11/console-terminal-status.json) is clear; [Console history](evidence/code11/console-terminal-errors.json) preserves the reproduced failures and earlier Editor exceptions. No Session or Session-test source changed. Generated font caches were saved/copied and cleared through Unity with no semantic diff. This tested source is pinned before both independent reviews and the next APK/AAB. The Editor is closed during the coordinator's motion comparison; the four pending native-provider cases remain a separate packaged acceptance gate.

## Final Standards verification

The independent [terminal-contact Standards verification](standards-code11-terminal-verification.md) reports **zero findings** at `25d3e9f9d80609c178230cf838cecfda54ae0370`. It confirms the small private adapter, centralized cleanup, real-provider tests and supported fixture isolation. The original Standards reports at all earlier pins remain separate.

## Final Spec verification

The independent [terminal-contact Spec verification](spec-code11-terminal-verification.md) closes both remaining P2 findings and reports **zero new substantiated findings** at the same pin. It inspected the recorded61/61 run and follow-up drag controls; it does not claim to have run native tests. The original P2 findings, their red results and the intermediate source/artifact pairs remain preserved.

## Final packaged release

Both reviews completed before these final builds. Source `25d3e9f9d80609c178230cf838cecfda54ae0370` includes integration tip `3d0f098859277705a19c930ba520ec9fd04dda15`. A clean restarted Editor compiled without errors before packaging ([status](evidence/code11/prebuild-terminal-console.json)). No source was changed between the APK and AAB. Both use Unity6000.3.25f1, version1.2.0/code11 and package-lock SHA256 `da98f245e8c511d47699dc883e521eff5176800341117fcfe8f77fe9dcfb342d`.

| Final package | Bytes | SHA256 | Build | Inspection |
| --- | ---: | --- | --- | --- |
| APK | 37,472,202 | `9c08b19ab2270b236cd2eec9a8d9bd2f67b7a9db93fc0ff3d5d5f2a5d5e8c9ad` | [317.05s](evidence/code11/apk-terminal-build-summary.json) | [14/14](evidence/code11/apk-terminal-inspection.json) |
| AAB | 37,829,159 | `9148cb972b7c6b256ad0838f084ec3bfe2693e3cfbd70bcd5885e4b6623d930e` | [293.25s](evidence/code11/aab-terminal-build-summary.json) | [44/44, four packages](evidence/code11/aab-terminal-inspection.json) |

Both builds have zero errors and the two previously documented warnings: absent RuntimePipelineConfig disables runtime Pipeline; MatchSurface.cs:89 uses obsolete EventBase.PreventDefault. They retain the existing local Android Debug certificate SHA256 `4a0d4929acea4c086bc5534bf462eebcd9d866f31c1bced10b400412117be926`; APKv2 verification passes. Production signing and Store submission remain owner actions, not completed by these locally signed packages.

Actual DEX inspection preserves the eight native-input classes with zero debug/info/verbose log calls. Manifest has no network permission and disables backup. Boot configuration has no player connection and native-debugger wait0. [APK](evidence/code11/apk-terminal-managed-stripped.txt) and [AAB](evidence/code11/aab-terminal-managed-stripped.txt) stripped assembly lists exclude TestFramework/test, Pipeline, Analytics, Connect and Consent assemblies. The added test-fixture dependency is absent from the release player.

All six ARM64 libraries pass16K LOAD alignment and have no rounded-RELRO writable intersections. Scalar GNU_RELRO end residues remain libc++12288, game4096, il2cpp8192, main8192, swappy4096 and unity8192 bytes. These nonzero scalar residues are not hidden; static checks and translated emulator execution do not establish physical ARM64/16KB-kernel behavior or Play acceptance.

The [exact code10→11 source comparison](evidence/code11/terminal-source-equivalence.json) contains only GroupReorder.cs within runtime UI, Session, Session tests, Android plugins and packages. The coordinator's unchanged-motion comparison can therefore be associated with the final implementation, while native code11 cancellation and update are separately verified. The coordinator reports settled scroll medians32.89/33.89ms versus code8 repeat46.88ms, and pull medians31.71/32.66ms versus45.85/47.91ms. Tails remain about68–80ms p95 and136–200ms maximum; native injection timing itself has24–36ms p95. This is a measured emulator improvement, **not proof of60fps or CPU/GPU attribution**. No profiler trace identifying the remaining stalls exists; the earlier .NET projection-allocation probe is not an IL2CPP profile. Raw native traces, video, state checkpoints and final native acceptance are owned by the coordinator's separate report.

The final owned Editor PID220416 was closed after both package inspections ([process proof](evidence/code11/editor-final-confirmation.json)); the shutdown log is retained. The CLI returned an invalid response because the Editor exited, so the actual process/engine shutdown evidence establishes completion. An unrelated Nischenreich batch Editor was visible at that later instant and was not touched. It was absent from the earlier coordinator measurement window; no claim of a globally quiet host is made for this later checkpoint.

## Raw preservation and handoff

The [raw artifact manifest](raw-manifest.json) records **748 files,2,700,522,954 bytes** under this checkout's ignored `artifacts/`, excluding only that manifest itself. It includes complete code9/code10 pairs, both superseded code11 stages, final APK/AAB and generated delivery packages, both Editor logs, test failures/successes, native-red copies, installed package source references and all saved dynamic font caches. The first manifest's missing aggregate was corrected by explicitly summing the recorded file sizes; that initial file is retained too. Preserve all listed paths and verify hashes before removing the worktree. Source and tracked evidence are committed separately; no raw files or worktrees were deleted by this implementer.

The [Session source comparison](evidence/code11/session-source-equivalence.json) confirms an empty diff for both Session and Session.Tests from the successful76/76 source `3835a71035b0fe4c471e5893a54f2438b5c34505` to the final build. This establishes the input equivalence behind the reused result, separately from the actual new61/61 rendered run.

At handoff, the coordinator reports successful native code10→11 update with both save generations byte-equal, native cancellation2/2 plus Cancel/new-contact and outside-drop preservation, and an intentional autoscrolling drop that saves the expected complete order. Fresh match handoff/reveal/release/Next and process-restart safe pause passed; the release log scan contained no private payloads or diagnostics. An earlier external assertion expected a narrower destination than the actual autoscroll produced; that failed helper assertion remains preserved and is not treated as a product failure or silently reclassified as a pass. The coordinator restored the original group/settings and stopped the app. Detailed native evidence and the final acceptance matrix remain in its separate report.

Post-build changes in this handoff are evidence/documentation only. Executable game, package, tool and test sources remain equivalent to the exact built/reviewed pin `25d3e9f9d80609c178230cf838cecfda54ae0370`. The separate merger integrates the branch; the coordinator owns native acceptance records, final user-facing release notes and ticket resolution. No ADB, Store upload, production-key handling, push or issue closure was performed by this implementer.
