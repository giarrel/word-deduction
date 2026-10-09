# Fluid interactions — ticket #20

Implemented from [Spec #18](https://github.com/giarrel/word-deduction/issues/18) and [ticket #20](https://github.com/giarrel/word-deduction/issues/20). The fixed product baseline is `53951d9f1d6cc0f9014db7615cfa7a159bb804d9`; implementation starts from planning commit `af0cba1f605877dfd3ee672f869704a74a9d9174`. Group integration `01bffc23b7d40dfdc4a39f6155511b563635035b` was merged before final interaction testing.

## Changes and boundaries

- Android requests the rounded current display refresh rate (60 fallback), initially and on focus/resume. Android's otherwise default 30 fps behavior is documented by [Unity](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Application-targetFrameRate.html). The existing Android optimized frame pacing setting remains enabled. This request alone is not a measured frame-rate result.
- A released, already concealed card returns from its actual visible lift over a bounded 140 ms unscaled-time curve. Partial pulls retain a visible return; a large logical reveal distance no longer creates a delay before visible movement. Reduced-motion and immediate interruption still settle immediately.
- Regrabbing a returning card retains its current visible position and follows fresh finger motion directly. Only fresh deliberate travel counts toward reveal, so residual decoration cannot expose a word. No drag smoothing or input lag was introduced.
- Card text/private names are populated once per reveal transition. The card's local ready state is updated on concealment instead of repeatedly allocating complete detached match projections every frame. Session still authoritatively validates every handoff; no private-card payload is retained outside the existing presentation lifetime.
- An integrated regression found that captured reorder Up/Cancel bypassed the shared root contact tracker. The handle now ends only its actual released/cancelled pointer ID before a redraw. Capture loss alone does not clear a held finger; other held contacts continue to block private reveal.

No role rules, save schema, corpus, accessibility refresh algorithm or renderer was changed. All secret fields are synchronously cleared before decorative return, capture release or redraw.

## Rendered RED/GREEN evidence

Tests use real Session persistence, GroupScreen, UI Toolkit panels and pooled public pointer/navigation events. The new tests observe visible card bounds, text concealment, capture ownership and whether the next gesture works.

| Scenario | Actual RED | GREEN |
| --- | --- | --- |
| Partial pull returns visibly instead of snapping | `partial-return-red-result.json`: card already at rest (274 px) after 70 ms | `partial-return-green-result.json`: 1/1 |
| Regrab catches the card without jumping away | `regrab-red-result.json`: stationary finger caused 229.36 → 274 px jump | `regrab-green-result.json`: 2/2 including partial return |
| Captured reorder release permits another finger to reveal | `reorder-reveal-red-captured-result.json`: order committed, subsequent private word stayed empty | `reorder-reveal-green-result.json`: 4/4 including cancellation and a separate still-held-finger guard |

Existing QuickScreenTests were 5/5 both before and after the projection refactor (`refactor-before-result.json`, `refactor-after-result.json`), including full bilingual flow, hold, cancellation, capture loss, second finger, background/focus, repeated Next and denied writes.

The first reorder harness attempt explicitly targeted a different element despite capture. That bypassed capture and failed its setup/order assertion, not the product behavior; it is retained as `reorder-reveal-red-result.json`. The corrected test uses the confirmed captured target. Unity's [6000.3 dispatch source](https://github.com/Unity-Technologies/UnityCsReference/blob/6000.3/Modules/UIElements/Core/Events/EventDispatchUtilities.cs) explains both the preassigned-target exception and skipped ancestor callbacks during capture. No engine internals are accessed by the test.

The final integrated rendered assembly is **57/57 passed**, 0 failed/skipped/inconclusive, 207.35 seconds: [completed result](evidence/integrated-rendered-final-result.json). It includes the full bilingual word-fitting corpus, all three modes, maximal private lists, large text, group editing/reordering, recovery, accessibility transitions and the four new motion/contact checks. This result covers production commit `33d0d99`; the later merge `e5a99857fb530294972ba854fd5fa01f62c34154` adds only the coordinator's updated validation scope from integration `3835a71035b0fe4c471e5893a54f2438b5c34505`.

## Native baseline and limits

The separate read-only investigation and all raw traces are preserved outside the repository in `work/ux-motion-investigation/`, including `report.md`, `baseline-manifest.json`, `gesture-analysis.json` and `session-projection-allocations.json`. Installed code8 APK SHA256 was `d4ba4c5ba65ca03249f59455234c1df7bdf4f85b160828ca05b945a85cd78ab9`. Original primary and previous saves were preserved and restored byte-for-byte; final state remained five original players, no active match and history length 12.

The initial emulator measurements showed slow actual presented intervals but included host-driven input and later Editor import contention. The coordinator subsequently repeated code8 with idle Editor and device-scheduled input in `work/usability-native/`; this is the appropriate paired baseline for final candidate comparison. Initial raw failures and incomplete capture windows were retained, not counted as complete cycles.

SurfaceFlinger actual-present timestamps are measured, excluding pending/invalid fence values. [AOSP FrameTracker](https://android.googlesource.com/platform/frameworks/native/+/cdb6b16dec3a541b455be99d075004cb2f0a0cd7/services/surfaceflinger/FrameTracker.cpp) defines the latency columns. Translated ARM64 emulator performance is not a physical-device result, CPU/GPU attribution or touch-to-photon latency. A separate .NET allocation probe confirms the detached Session projections allocate; its byte counts are not Unity/IL2CPP measurements or proof they explain the whole stall.

Candidate Android build, identical native performance comparison, continuous motion inspection, native reduced-motion/accessibility and final update acceptance remain the coordinator's integration validation. This implementer does not claim those results from Editor tests.

## Tooling and handoff

Unity 6000.3.25f1, one authorized Editor for the exact `motion/game` project, PID 217316. Initial test dispatch reported Editor busy while importing and was retained separately. Tests were collected using actual completed `test_status` responses. No console clearing, APK installation, build, push, issue closure or Nischenreich modification was performed during implementation. Root retained ADB after the restored baseline handoff.

[Console status](evidence/console-status.json) confirms compilation succeeded. The ten retained [console errors](evidence/console-errors.json) are the existing `UnityEditor.Search.SearchDatabase` startup exceptions, with no app stack frames. Generated Inter/Emoji data was saved and copied to ignored raw evidence, then cleared through the Editor; Git confirmed no semantic font asset change. A scheduled Editor exit did not terminate the process, so the exact owned PID was verified and stopped after assets and results were saved. [Exit confirmation](evidence/editor-exited.txt) releases the sole Editor lease.

One sandboxed process-identity query returned Access denied; its subsequent null result was not accepted as shutdown evidence. The authorized exact-PID query and shutdown above provide the actual confirmation.

The selected `evidence/` files are byte-for-byte copies from `artifacts/fluid-interactions/`; [evidence-manifest.json](evidence-manifest.json) freezes hashes and sizes. [raw-manifest.json](raw-manifest.json) also covers the two preserved generated font caches. Preserve all manifest-listed raw files before removing the implementation worktree. Raw evidence uses Git `-text` attributes. Native candidate comparison and final release validation remain outstanding at this handoff; role-count changes are outside this ticket.
