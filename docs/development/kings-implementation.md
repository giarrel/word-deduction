# Kings implementation

## Scope and source

- User invoked implement-spec after accepting rules Q1–Q19 and publishing Spec #10.
- Canonical: https://github.com/giarrel/word-deduction/issues/10; local: ../specs/kings-mode.md.
- Integration branch: integration/kings-v1.
- Fixed pre-implementation review baseline: 478953f496c0a8e88f6849c32960f02c2039f671 (completed Android v1).
- Goal: implement and validate Kings, integrate through separate merger, run independent final reviews/corrections, supply inspected Android candidate and close genuinely completed tickets.
- #11 stays needs-info; its unresolved base-mode outcomes are not invented. Shared initial-count validity is in scope; new base-mode count controls/outcomes are not.
- Current local debug signing is retained; production signing/store publishing are separate owner actions.

## Task graph

| Ticket | Deliverable | Blocked by | Initial state |
| --- | --- | --- | --- |
| #12 | Select mode, valid saved count, deal and inspect private cards | None | Ready |
| #13 | Record actual eliminations and King outcomes | #12 | Waiting |
| #14 | Irrevocable word/King last chance | #13 | Waiting |
| #15 | DE/EN rules and readable private cards | #12 | Waiting |
| #16 | Recovery, final validation and Android candidate | #14, #15 | Waiting |

Native dependency/sub-issue mutation capabilities are unavailable in the connected tracker; ticket bodies contain the blocking edges. The ready frontier is computed from actual completion, not merely the triage label.

## Execution contract

The existing user delegation and autonomous workflow apply to routine ticket granularity, test-seam selection and review baseline selection. No extra approval interview is introduced. New rules from #11 require actual decisions rather than assumptions.

- Implementers use tdd at the existing public Session and rendered-app seams. Preserve a failed observation before a passing observation for each substantive behavior.
- Each implementer owns a separate Git worktree and branch created from the integration tip. Never reset or discard unrelated changes. Confirm ancestry and merge the current integration tip before reporting done.
- A separate merger integrates completed commits and checks the combined tree.
- Unity Editor mutation and rendering/builds are serialized. An implementer needs an explicit coordinator lease before starting Unity; others may work on independent source and Session tests. Root owns ADB/native application state.
- Use existing Unity 6000.3.25f1 and installed SDK/NDK/JDK. Nischenreich stays untouched.
- Evidence and disposable build caches remain ignored; preserve required evidence and hashes before removing worktrees.
- Keep artifacts from earlier releases intact. New version/code must be monotonic and final source/artefact provenance exact.
- After implementation, Standards and Spec reviews run independently against the pinned baseline. One correction implementer handles their findings. New evidence or changes determine focused reruns.
- Root performs native validation, visually inspects actual screenshots, and distinguishes Android emulator, Editor and physical-device evidence.
- No claim of human group enjoyment/balance without human group play.

## Test seams

1. Public Session actions and permitted projections, with real temporary storage/reopening and controlled random inputs.
2. Actual rendered app behavior for touch/hold, hiding private content, readability, navigation, localization, and mobile lifecycle.

No tests of private method order, no duplicate game engine, no machine enforcement of social voting/clue rules.

## Runtime context pointers

- Existing setup: unity-setup.md; previous release reproduction: ../release/reproduction.md.
- Current read-only exploration will be saved outside the repo at C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/kings-implementation/runtime-setup.md.
- Preserved v1 test runtime DLL: artifacts/repro-runtime/Newtonsoft.Json.dll.
- Baseline delivery: sibling outputs/Word-Deduction-1.0.0 (installable code4).

## Progress

Ticket #12 is accepted and closed. Foundation commit `7b695699b2ac29d8222fbf8580e805c6ce410f9b` was integrated by a separate merger at `eb8538390d430f0089c5fcfb9f7377e4b2186f98`; report commit `3c6d6694ff694ec7742f0caf8642219c0fa0a07a` is pushed. Implementer Session61/61, rendered33/33 and independent integration Session61/61 passed. Both earlier isolated full-suite failures are preserved with their cause unconfirmed; future failures now retain diagnostic details and snapshots. See ../validation/kings-foundation/report.md and ../validation/kings-foundation-merge/report.md.

Ticket #13 is accepted and closed. Source `30ac0c108f71f786840ae1aa16d7ceddd7dfe8ff` and completed implementer tip `1019600def984b295b9fe1d23f53199165b13099` were integrated by the separate merger at `663f82f4dd1daa544f3beec0ae459923275fa0f1`. Pushed report tip: `82124083890c1d2c429d3f1da687fd610bb00b5d`. Full Session66/66, new rendered3/3 (both languages), Classic4/4 and Quick5/5 passed; the independent integration Session run passed66/66. All54 manifest streams /8,085,061bytes were preserved and SHA-verified. See ../validation/kings-elimination/report.md and ../validation/kings-elimination-merge/report.md.

Ticket #15 is accepted and closed. Completed source `c63cf67cc218a71f91c8f023fe72c7e6c9e592be` was separately merged at `ca8e2f10b7529d5950d650f8f33c6d58ab0d07ed`; pushed report tip `b402fb375323758f398055a3f8257c37667a4a11`. Full combined rendered41/41 passed. The independent Session66/66 evidence is reused with identical source/test/fixture/content/Unicode tree hashes. All80 selected evidence files and87 ignored raw backup files were independently verified. See ../validation/kings-rules-cards/report.md and ../validation/kings-rules-cards-merge/report.md. Native CJK fallback remains part of #16 acceptance.

#14 (binding last chance) is the active frontier, in its own worktree from8212408 with #15 integrated. It currently holds the sole coordinator-granted Unity lease. #16 remains blocked by #14, and Spec#10 is not complete.

The coordinator recovered the existing Android test guest after an OS service failure, preserved both old saves byte-for-byte, and prepared an authentic code4 Quick game with one completed handoff for update acceptance. Fresh native preparation and raw evidence are outside the repo in `C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/kings-native/`; no Kings APK has been built or accepted. Native accessibility preparation uses the already installed TalkBack service and the existing external test helper.
