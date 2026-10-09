# Freie Rollenanzahlen und sequenzielle Quick-/White-Ergebnisse

Canonical: https://github.com/giarrel/word-deduction/issues/21

## Parent

Spec #11: https://github.com/giarrel/word-deduction/issues/11

## What to build

Implement the complete Quick/Classic configurable-role path, from remembered Group settings through safe deal and sequential accusation/White judgment to result/rematch, in DE/EN. Use the existing Session and rendered-app seams. Read the full Spec including deterministic shrink behavior, atomic one-slot role replacement and V1–V5 compatibility.

## Acceptance criteria

- [ ] Both base modes expose usable Undercover and White count controls, Auto and shared-limit explanations, including pure White and one-slot replacement.
- [ ] Per-mode desired counts persist; shrinking uses valid effective counts and growing restores wishes without changing the group or frozen deal.
- [ ] Quick confirms/reveals each selected person, continues after non-final evil catches without a required new clue phase, requires all adversaries and loses immediately on a Civilian.
- [ ] Each White gets one persisted immediate spoken guess before terminal evaluation; correct awards the whole White side alone, incorrect continues/checks the appropriate end; no target word before Result.
- [ ] Classic survivors, ties, winner role sets and new multi-White cases work; Quick mistake/tie awards every adversary role present in the deal.
- [ ] Older group/match saves and finalized Quick results reopen unchanged; malformed new data, failed writes, recovery, newer-version blocking and stale commands have behavior coverage.
- [ ] DE/EN rules, outcomes, public accessibility, small-screen/large-text UI and the existing group/privacy protections are coherent; Session and relevant rendered TDD evidence is recorded.

## Blocked by

None (can start immediately).

