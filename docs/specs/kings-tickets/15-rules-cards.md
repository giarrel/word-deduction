# Könige-Regeln und private Karten in Deutsch und Englisch verständlich machen

Canonical issue: https://github.com/giarrel/word-deduction/issues/15

## Parent

Part of [Kings specification #10](https://github.com/giarrel/word-deduction/issues/10).

## What to build

Könige-Regeln und private Karten in Deutsch und Englisch verständlich machen. Deliver a narrow usable path through the existing Session, persistence and rendered app, respecting the complete parent specification.

## Acceptance criteria

- [ ] Optional in-app rules/help accurately explains all accepted Kings rules Q1–Q19 and clearly separates table responsibilities from the app's recording of eliminations.
- [ ] Voting, runoff, tie, random starter, clue restrictions, bluffing, no advice, spoken semantic judgment and accidental leaks remain text rules; no enforcement UI or mandatory tutorial.
- [ ] Private cards remain readable at small-phone and large-text settings with long/duplicate/Unicode names and maximal permitted team lists; ordinary cards do not betray side by wording/layout/accessibility cues.
- [ ] Existing drag/hold privacy is preserved for the full private card including lists on release, interrupted gestures, focus loss and backgrounding. No later card review or viewing timer.
- [ ] DE/EN labels and mode-scoped King/White vocabulary are complete, concise and consistent in glossary and help; existing mode explanations retain their own rules.
- [ ] Use the existing rendered-app seam to demonstrate help and private-card behavior with saved screenshots and real inspection; tests must not mirror private layout helpers.
- [ ] Coordinate changes to shared copy/style files through integration; do not implement elimination or last-chance progression owned by the other tickets.

## Blocked by

- [Könige auswählen, Rollenanzahl speichern und geheime Karten verteilen](https://github.com/giarrel/word-deduction/issues/12)

## Execution

Use implement-spec and tdd with the existing agreed Session/public projection plus rendered-app seams. Work in a dedicated worktree based on integration/kings-v1; read the glossary and architectural decision first. Preserve observable red/green evidence, merge the integration tip before reporting, and let a separate merger integrate. No new product interview is needed for routine engineering decisions under the existing delegated workflow. Issue #11 remains needs-info and is not part of the Kings feature's implementation scope.
