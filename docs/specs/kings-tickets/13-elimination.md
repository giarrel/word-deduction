# Könige am Tisch spielen und Eliminierungen ohne Abstimmungsdialoge erfassen

Canonical issue: https://github.com/giarrel/word-deduction/issues/13

## Parent

Part of [Kings specification #10](https://github.com/giarrel/word-deduction/issues/10).

## What to build

Könige am Tisch spielen und Eliminierungen ohne Abstimmungsdialoge erfassen. Deliver a narrow usable path through the existing Session, persistence and rendered app, respecting the complete parent specification.

## Acceptance criteria

- [ ] After handoffs, a compact survivor surface lets the host record the participant the group eliminated, with a correctable pending selection and deliberate confirmation before irreversible disclosure.
- [ ] Ordinary elimination persists and exposes only Not a king/Kein König through both Session and rendered views; no side, word, leader, side-survivor counts or other secret leaks.
- [ ] Ordinary eliminated participants cannot be selected again and remain active in the saved Group for the next match. No clue/vote/runoff/countdown states are required for human table play.
- [ ] Eliminating the good King resolves an evil-team victory. Eliminating White enters a durable last-chance entry surface without premature result or word/King disclosure, ready for the dependent ticket.
- [ ] Only two surviving Kings resolves the explicit evil victory; numerical parity and a lone good King with other survivors do not import Classic victory conditions.
- [ ] Results describe team victory including eliminated teammates, expose final assignments only when appropriate, and support direct rematch and group editing with settings/history retained.
- [ ] Restart, failed writes, duplicate confirmations and stale actions preserve legal committed progression. DE/EN controls and behavior have Session and rendered red/green evidence.

## Blocked by

- [Könige auswählen, Rollenanzahl speichern und geheime Karten verteilen](https://github.com/giarrel/word-deduction/issues/12)

## Execution

Use implement-spec and tdd with the existing agreed Session/public projection plus rendered-app seams. Work in a dedicated worktree based on integration/kings-v1; read the glossary and architectural decision first. Preserve observable red/green evidence, merge the integration tip before reporting, and let a separate merger integrate. No new product interview is needed for routine engineering decisions under the existing delegated workflow. Issue #11 remains needs-info and is not part of the Kings feature's implementation scope.
