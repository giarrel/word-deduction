# Könige auswählen, Rollenanzahl speichern und geheime Karten verteilen

Canonical issue: https://github.com/giarrel/word-deduction/issues/12

## Parent

Part of [Kings specification #10](https://github.com/giarrel/word-deduction/issues/10).

## What to build

Könige auswählen, Rollenanzahl speichern und geheime Karten verteilen. Deliver a narrow usable path through the existing Session, persistence and rendered app, respecting the complete parent specification.

## Acceptance criteria

- [ ] A third Kings/Könige mode can be selected in DE/EN using the existing persistent Group; five to twenty active participants, one King per side and valid ordinary-Undercover selection work end to end.
- [ ] Use one initial good-majority count policy for all modes without adding the unresolved Quick/Classic count features from #11. Existing mode behavior stays intact.
- [ ] Persist the Kings count preference; use existing adaptive defaults before manual selection, visibly clamp the effective count for a smaller group and retain the requested value. Invalid explicit counts cannot start invalid games.
- [ ] Deal fresh random sides/Kings with existing bilingual word/history behavior. Ordinary participants see only their word and own leader; good King sees good word and unmarked evil names; White is evil King with no word and only own teammates.
- [ ] Private/public Session projections expose exactly the allowed knowledge. Neutral ordinary-card visuals, safe release/hold concealment and completed-handoff non-revisit work. An interrupted unfinished handoff resumes covered.
- [ ] Introduce the new saved format and preserve valid old groups and live Quick/Classic deals; stable IDs, Unicode/duplicate names and word history survive reopening.
- [ ] Rendered tests demonstrate selection, valid counts, each private-card variant and handoff completion into a simple table-play landing; no voting/runoff workflow is added for Kings.
- [ ] Preserve red-before-green evidence at the established Session and rendered-app seams; update the relevant interface documentation.

## Blocked by

None (can start immediately).

## Execution

Use implement-spec and tdd with the existing agreed Session/public projection plus rendered-app seams. Work in a dedicated worktree based on integration/kings-v1; read the glossary and architectural decision first. Preserve observable red/green evidence, merge the integration tip before reporting, and let a separate merger integrate. No new product interview is needed for routine engineering decisions under the existing delegated workflow. Issue #11 remains needs-info and is not part of the Kings feature's implementation scope.
