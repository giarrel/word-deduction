# Könige-Persistenz absichern und geprüftes Android-Paket bereitstellen

Canonical issue: https://github.com/giarrel/word-deduction/issues/16

## Parent

Part of [Kings specification #10](https://github.com/giarrel/word-deduction/issues/10).

## What to build

Könige-Persistenz absichern und geprüftes Android-Paket bereitstellen. Deliver a narrow usable path through the existing Session, persistence and rendered app, respecting the complete parent specification.

## Acceptance criteria

- [ ] Verify and correct complete Kings recovery behavior at every durable phase, storage failures and damaged/newer/invalid snapshots using the established Session seam.
- [ ] Upgrade from the current release preserving old Group, Unicode names, history and live Quick/Classic states; existing behavior tests remain green.
- [ ] Run the complete relevant Session and rendered suites, perform focused layout/privacy refinements found by actual inspection, and preserve red/green evidence for defects fixed.
- [ ] Produce a clean-source-pinned Android local release APK and AAB with monotonic version/code, inspected package/signature/privacy/16KB metadata and hashes using the installed toolchain.
- [ ] Provide installable handoff documentation, a Spec #10 acceptance matrix, and actual Android evidence for all Kings ending paths, restart, update, offline operation, background privacy, DE/EN and large text. Coordinator owns native ADB execution.
- [ ] Independent final Standards and Spec review, correction loop and final accepted native package remain required before this ticket and parent are closed; do not claim human group balance from emulator tests.
- [ ] Keep existing local signing limitations explicit and #11 open. Do not publish to a store or touch Nischenreich.

## Blocked by

- [Mr. Whites verbindliche letzte Chance auf Wort oder König spielen](https://github.com/giarrel/word-deduction/issues/14)
- [Könige-Regeln und private Karten in Deutsch und Englisch verständlich machen](https://github.com/giarrel/word-deduction/issues/15)

## Execution

Use implement-spec and tdd with the existing agreed Session/public projection plus rendered-app seams. Work in a dedicated worktree based on integration/kings-v1; read the glossary and architectural decision first. Preserve observable red/green evidence, merge the integration tip before reporting, and let a separate merger integrate. No new product interview is needed for routine engineering decisions under the existing delegated workflow. Issue #11 remains needs-info and is not part of the Kings feature's implementation scope.
