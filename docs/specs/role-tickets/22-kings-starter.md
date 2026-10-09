# Könige: Startperson und schlanker Wechsel der Hinweisrunde

Canonical: https://github.com/giarrel/word-deduction/issues/22

## Parent

Spec #11: https://github.com/giarrel/word-deduction/issues/11

## What to build

Show a persisted random living starter on the existing Kings table view. Automatically select the next starter after an ordinary nonterminal elimination and offer optional Next round for no-elimination rounds. Keep the app's direct elimination flow and the complete existing Kings rules. Extend the count branch's migration rather than creating a competing save schema.

## Acceptance criteria

- [ ] Initial table shows an eligible starter, including White; ordinary elimination atomically changes to a surviving suggestion without a required extra screen.
- [ ] Optional Next round is available only at safe table states without a pending selection; it never blocks direct elimination or records votes.
- [ ] Match/round/progress guards reject stale duplicate actions; failed writes and reopen preserve the suggestion.
- [ ] Legacy Kings saves with eliminated unused starter migrate deterministically without rerolling the deal or writing on open.
- [ ] Public display/accessibility contains only the starter's name; private sides and ordinary-elimination roles remain concealed.
- [ ] DE/EN rules and existing Kings spec/interface documentation are updated for the newly accepted app-assisted starter rule.
- [ ] Public Session and rendered-app red/green tests cover ordinary/tie rounds, restart, eligible starter set, safe controls and compatibility; existing Kings end paths pass.

## Blocked by

#21 (shared match/save evolution from free counts must be integrated first).

