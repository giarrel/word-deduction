# Kings acceptance implementation — ticket #16

Status: local candidate prepared for coordinator acceptance. Native Android execution and the final independent Standards/Spec reviews remain required before closing #16 or #10. Issue #11 remains separate.

## Source and scope

Candidate source: `908d95fc80fc8173df02593422ff85264089b945`, including integration tracking commit `8150e85`. Unity `6000.3.25f1`; package-lock SHA256 `da98f245e8c511d47699dc883e521eff5176800341117fcfe8f77fe9dcfb342d`.

This ticket adds recovery acceptance coverage at the established public Session boundary, a test-only generator of native visual fixtures, and the intentional version `1.1.0` / Android code `5`. The release inspector now accepts an explicit version-name expectation and defaults to this candidate. No gameplay/persistence production fix was necessary: the tested behavior already passed. The first suspected missing ordinary-teammate validation was already enforced by the shared initial-composition policy. Its passing corruption test is retained as acceptance coverage; it is not represented as a red/green production defect.

## Session and rendered evidence

- [Full Session run](evidence/session-full.log): **74/74 passed**, with task-local TEMP/TMP/DOTNET_CLI_HOME and `WD_TEST_IO_DIAGNOSTICS=1`. Existing Quick/Classic, legacy V2/V3/V4, Unicode names, corpus/history and all Kings scenarios remain included.
- [Focused recovery run](evidence/recovery-focused.log): **3/3 passed**. Every durable Kings phase, every pending elimination/King selection, all six endings and rematch reopen with the same public state. The real primary file is held against replacement after the pending generation flushes; each failed action leaves both live disclosure and committed bytes unchanged, then succeeds once storage permits it. Incomplete pending writes never become confirmed progress.
- Invalid valid-checksum composition, damaged/missing primary, both generations damaged, inaccessible primary and newer schema in either committed slot are covered. Backup recovery explicitly warns and may restore an older unfinished handoff; no zero-rollback or tamperproof guarantee is made. First repair preserves the valid backup, and deliberate reset archives both damaged files. Future/unreadable data cannot be reset or overwritten.
- [Full rendered suite](evidence/rendered-full.json): **44/44 passed**, zero skipped/inconclusive, **210.73 seconds**. This is the complete `WordDeduction.PlayTests` assembly, including Quick/Classic, Kings cards/rules/table play/last chance, save failure, lifecycle concealment, public accessibility projections, small viewport and 150% text cases.

Rendered tests ran before the release version bump; the tested Session/UI/PlayMode source is unchanged in the candidate. The information screenshot consequently shows historical `1.0.0`; production reads `Application.version` and the candidate setting is `1.1.0`. Test-created Inter/Emoji assets and their diffs were preserved in ignored raw evidence, then only those known caches were cleared through the Editor API. Final semantic asset diff is empty.

I visually inspected the actual [maximal German good-King list end](evidence/max-list-German-GoodKing-hold-last.png), [German committed-answer judgment](evidence/judgment-German-True.png) and [English rules at 150%](evidence/help-English-start.png). The held list's final entries remain inside its rounded viewport, fixed actions remain separated, judgment uses neutral Correct/Incorrect labels, and long help remains readable in its scroll area. These observations found no additional layout defect. Other scenarios retain their prior ticket inspections and automated assertions; this report does not claim every saved image was manually inspected. Windows CJK fallback limitations require the native follow-up.

## Local APK and AAB

Both artifacts are intended for the existing local installation, using its Android debug certificate. The application itself is non-development/non-debuggable ARM64 IL2CPP, min API26 / target API36, R8 release minification, no player log, no runtime Pipeline/profiler or debugger boot configuration. Production signing and Store publication remain owner actions.

APK: `artifacts/android/908d95fc80fc/20261008T191608690Z-apk/WordDeduction-local-release.apk`

- Size: **37,458,410 bytes**.
- SHA256: `afd739cc5b8356a24ee9b2b92c7639ba128569d3f43090ac961fc0e6a3d24a01`.
- Build: succeeded in321.989s, zero errors, two warnings retained in raw Editor/build evidence.
- Inspection:14 installed-tool commands passed; APK v2 signature, correct identity/SDK/ABI, no forbidden network/sensitive permissions or debuggable flag, backup exclusion rules, actual DEX private-input logging checks,16KB ZIP/ELF LOAD layout and writable RELRO collision checks.
- Certificate SHA256: `4a0d4929acea4c086bc5534bf462eebcd9d866f31c1bced10b400412117be926`, matching the retained code4 signature evidence.

AAB: `artifacts/android/908d95fc80fc/20261008T192436613Z-aab/WordDeduction-local-release.aab`; **37,815,362 bytes**, SHA256 `4b8aee0303ad77f5e4b39da1e6da9dcf8624a99005fe2bdda266cc58cc066d64`. Build succeeded in279.224s with zero errors/two warnings. Inspection passed44 installed-tool commands and four packages: bundletool validation/configuration, JAR signature, default split and universal APK delivery, signatures/manifest/backup/DEX logging/native alignment checks.

The actual latest BuildReport identifies both warnings: runtime Pipeline deliberately disabled because no runtime configuration exists, and the inherited `MatchSurface.cs:80` use of obsolete `EventBase.PreventDefault()`. That input code was not changed by this ticket and the complete interaction suites passed. Warnings are retained, not suppressed. Both builds restored their configured state; final semantic Git asset diff is empty.

The exact owned Editor PID108764 exited; the OS confirmed it absent at19:32:26Z. The exit RPC lost its response as Pipeline shut down; this is separated from the successful process-exit evidence. No other Editor was closed. Scalar RELRO end remainders are retained in the inspection; static alignment is not physical ARM64/16KB-kernel proof.

## Native visual fixtures and handoff

`artifacts/kings-acceptance/native-visual-fixtures/manifest.json` records four synthetic checkpoints generated by **public Session actions from the candidate source**, each with20 participants, eight ordinary Undercover, one evil King, long/duplicate/Unicode names, and both saved generations/hashes. DE/EN each provide an unfinished good-King or evil-King handoff, re-opened covered with no random draw. These are visual fixture inputs, not Android gameplay evidence.

Reproduce to a fresh directory using the Session runner: `--export-native-visual-fixtures <output> <full-source-commit>`. The generator is compiled only into the test runner, never the application. Root preserves the real code4 live save before the real in-place update and any guarded replay. This agent performed no ADB/emulator operations.

The initial fixture-export shell omitted writable TEMP/TMP and failed in MSBuild before generation; its original log is retained, and the corrected environment exported successfully. One new test initially referenced a nonexistent public property; that compiler error and the corrected focused run are retained. The first AAB dispatch was rejected before build because draft report evidence made the checkout untracked; moving only that draft into ignored artifacts restored the exact clean source before the confirmed retry. None of these tool/harness setup errors is described as an app defect or a successful build.

## Remaining acceptance gates

The coordinator owns the actual code4 in-place update, live Quick/Classic preservation, all six Kings endings on the installed package, restart/finality/privacy, offline/log checks, DE/EN, large text/maximal lists/Unicode and native TalkBack public navigation. The final independent reviews and any resulting single correction loop remain outstanding. Real table balance, spoken screen-reader evaluation, physical haptics/performance and physical ARM64 hardware are not inferred from Editor tests or a silent translated emulator. See the [acceptance matrix](../kings/acceptance-matrix.md).

## Evidence preservation

`raw-manifest.csv` lists216 selected raw files (165,095,193 bytes), relative to the checkout, for verified merger preservation before worktree cleanup. It includes exact APK/AAB/build reports, command outputs, inspection reports, rendered screenshots, both-generation visual fixtures and preserved font changes. Regenerable extracted native/DEX/APKS intermediates and task-local runtime/temp caches are excluded. `evidence-manifest.csv` identifies the concise tracked evidence. Native evidence belongs to the coordinator and is not asserted here.
