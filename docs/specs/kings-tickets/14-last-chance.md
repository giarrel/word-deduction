# Mr. Whites verbindliche letzte Chance auf Wort oder König spielen

Canonical issue: https://github.com/giarrel/word-deduction/issues/14

## Parent

Part of [Kings specification #10](https://github.com/giarrel/word-deduction/issues/10).

## What to build

Mr. Whites verbindliche letzte Chance auf Wort oder König spielen. Deliver a narrow usable path through the existing Session, persistence and rendered app, respecting the complete parent specification.

## Acceptance criteria

- [ ] After White's elimination, exactly one irrevocable choice between word and King is available; no general result or private target is exposed beforehand.
- [ ] Word path accepts one spoken answer socially; a committed answer-given action persists before showing the good word for group Correct/Incorrect judgment. No speech/text matching or answer storage.
- [ ] King path shows unmarked surviving participants and permits correction of a pending selection, followed by one committed target and final comparison by stable ID.
- [ ] Correct attempt wins for the entire evil team; incorrect attempt wins for the entire good team. White has no solo victory in Kings.
- [ ] Restart each commitment/pending-judgment stage, back, duplicate taps, stale match actions and failed writes cannot switch branches, retry, falsely acknowledge a save or reveal an uncommitted secret.
- [ ] DE/EN last-chance and final-result screens remain short and usable; no advice monitoring or unnecessary ceremony. Session and rendered evidence covers every result path.
- [ ] Merge integration tip before reporting; preserve artifacts and document the final contract.

## Blocked by

- [Könige am Tisch spielen und Eliminierungen ohne Abstimmungsdialoge erfassen](https://github.com/giarrel/word-deduction/issues/13)

## Execution

Use implement-spec and tdd with the existing agreed Session/public projection plus rendered-app seams. Work in a dedicated worktree based on integration/kings-v1; read the glossary and architectural decision first. Preserve observable red/green evidence, merge the integration tip before reporting, and let a separate merger integrate. No new product interview is needed for routine engineering decisions under the existing delegated workflow. Issue #11 remains needs-info and is not part of the Kings feature's implementation scope.
