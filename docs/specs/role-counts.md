# Free role counts and Kings round starters

Canonical: [Issue #11](https://github.com/giarrel/word-deduction/issues/11). Accepted rule answers: 9 October 2026. This specification supersedes the earlier unanswered requirements note and the original Quick single-adversary rule. For Kings it supersedes only the previous rule-only starter selection.

## Problem Statement

Hosts cannot choose Undercover and Mr. White counts in Quick or Classic. Kings count controls appear broken with five or six participants because both bounds coincide without a useful explanation. Groups also lack an app-provided starting person for each Kings clue round. Setup must remain quick, remembered and comprehensible as people join or leave.

## Solution

Offer compact role-count controls in the existing Group screen with useful automatic defaults, remembered manual choices and one shared initial-majority limit. Quick resolves individually confirmed accusations in sequence: every adversary must be caught, while the first accused Civilian immediately ends the match for the adversaries. Each caught White has a spoken guess; a correct guess wins for the entire White side alone. The group controls discussion and how it reaches its accusations.

Kings shows a random surviving starting person on its existing table view. A normal elimination automatically prepares the next starter; an optional Next round action handles rounds without an elimination. No new required discussion or voting screens.

## User Stories

1. As a Quick host, I want to change both Undercover and Mr. White counts, so that short matches support my group.
2. As a Classic host, I want both counts editable, so that I can choose the desired mix.
3. As a host, I want to use Whites without Undercover, so that a pure bluffing composition is possible.
4. As a host, I want at least one adversary and an initial good majority, so that every offered setup is valid.
5. As a host, I want the same total-adversary limit across all three modes, so that the limits are understandable.
6. As a new host, I want useful defaults without setup work, so that I can immediately start.
7. As a returning host, I want manual preferences remembered separately per mode, so that changing modes does not overwrite them.
8. As a host, I want shrinking groups to use a valid effective mix while retaining my desired counts, so that adding players restores my preference.
9. As a host, I want the effective counts and any temporary adjustment explained, so that the actual deal never surprises me.
10. As a host, I want disabled controls explained, especially Kings with five or six people, so that a rule limit is not mistaken for a broken button.
11. As a host, I want to restore automatic counts with one small action, so that experimentation is reversible.
12. As a participant, I want changing future settings to leave a live frozen match intact, so that roles and words never change mid-match.
13. As a Quick group, we want to confirm one accused person at a time and see their role, so that we control how we organize our discussion.
14. As a Quick group, we want another accusation after catching a non-final adversary, so that all adversaries must be found.
15. As a Quick group, we want an accused Civilian to immediately end the match for the adversaries, so that mistakes have a clear consequence.
16. As a Quick group, we want catching the last adversary to award the good side unless a White succeeds first, so that the finish is unambiguous.
17. As a White in Quick or Classic, I want one immediate spoken guess when selected, so that every White has the agreed last chance.
18. As a White player, I want a teammate's correct guess to win for all Whites but not Undercover, so that the winning side is clear.
19. As a remaining White, I want the target word concealed until the match ends, so that an earlier unsuccessful guess does not spoil it.
20. As a group, we want to judge spoken guesses with Correct / Incorrect, so that no text entry or automatic word matching interrupts play.
21. As a group, we want pending selections, eliminations and White guesses to survive restart, so that progress is not lost or repeated.
22. As a Classic group, we want existing survivor rounds, ties and terminal checks preserved with larger role counts, so that the established mode remains familiar.
23. As a Kings group, we want a visible random starter immediately after handoff, so that we know who gives the first clue.
24. As a Kings group, we want a new surviving starter after a normal elimination, so that the next clue round begins smoothly.
25. As a Kings group, we want an optional Next round action without an elimination, so that a tie can lead directly to another clue round.
26. As a Kings participant, I want every survivor, including White, eligible to start, so that the suggestion reveals no role.
27. As a returning Kings group, we want the current starter to survive restart, so that reopening is not a reroll.
28. As a Kings group, we want existing hidden teams, ordinary non-king announcements and last-chance rules preserved, so that starter assistance does not change the game.
29. As a German or English player, I want concise localized rules, controls and outcomes, so that the changed rules are understandable.
30. As a player with large text or accessibility needs, I want legible reachable controls and public starter information, so that setup and rounds remain usable.
31. As an existing user, I want my saved group, order, identities, language, word history and frozen match preserved on update, so that installing the new version requires no reconfiguration.
32. As a host, I want clear save-failure behavior without half-applied choices, so that retries are safe.
33. As a group, we want rematches to keep the group and effective settings, so that everyone can immediately play again.
34. As a tester, I want an installable Android build with exact validation evidence, so that the implemented behavior can be checked on a phone.

## Implementation Decisions

### Accepted rules

- Quick minimum is 3 participants, Classic 4, Kings 5; all retain the 20-active-participant ceiling.
- In all modes Undercover plus White must be at least one and at most floor((participant count - 1) / 2) at the deal. Counts are nonnegative integers. This is an initial configuration rule, not a live parity victory rule.
- Quick and Classic allow zero Undercover with one or more Whites, and zero Whites with one or more Undercover.
- Kings retains exactly one White (evil King), at least one ordinary Undercover and exactly one good King among the Civilians. Its ordinary-Undercover maximum is the shared total limit minus one. Five/six players therefore permit exactly one ordinary Undercover plus White.
- Quick confirms and reveals one selected person at a time. The group may decide all accusations before entering them or continue discussion between them; no bundled selection, enforced extra clue rounds, ballots or speech monitoring.
- A confirmed Quick Civilian accusation ends immediately with all adversary roles present in the frozen match winning, including previously eliminated teammates. A caught ordinary Undercover continues if adversaries remain; catching the last adversary awards all Civilians.
- A caught White in either base mode enters a persisted pending guess before any terminal evaluation. Exactly one spoken guess per caught White; correct ends immediately for all Whites (including eliminated teammates), never Undercover. Incorrect removes that White and checks the mode's terminal conditions.
- Quick continuation returns to choosing the next survivor after the public role/failed-guess notice without a forced Clues screen or starter reroll. A minimal acknowledgment is acceptable so the role notice can be read. All selections are correctable before irreversible confirmation.
- Neither base-mode elimination nor guess judgment exposes the Civilian word or Undercover word; only final Result exposes the full deal. The group judges semantic correctness. Kings retains its existing binding branch choice and post-spoken-answer word judgment because its last chance immediately ends the match.
- Classic still awards Civilians when all adversaries are eliminated; with only one Civilian remaining, surviving adversary roles win. Check a pending White guess first. Classic normal eliminations and repeated ties lead to another survivor clue round.
- Preserve the existing spoken runoff guidance. Quick's repeated unresolved tie ends for the adversary roles present; Classic's repeated tie starts another round without elimination. Kings ties stay table-run.
- Ordinary roles remain privately word-based as before; this work does not add explicit side disclosure to ordinary cards.

### Settings and persistence

- Keep rule ownership, validation, safe projections and atomic commands inside the existing deep Session module; UI consumes its public interface. No second independent count table or winner calculation in presentation.
- Store an optional desired Undercover/White pair separately for Quick and Classic. Auto uses the existing mode defaults and the existing Classic White preference on migration. Keep Kings' existing desired count.
- Manual changes validate the complete effective pair against current group capacity. Offer controls that allow every valid pair, including a switch from one Undercover to one White at the minimum size (for example a single atomic replacement interaction when the only slot is occupied). Do not require a transient zero-adversary or over-capacity saved state.
- When the group shrinks, retain the desired pair while showing a deterministic valid effective pair. If both roles were desired and capacity is at least two, keep at least one of each; distribute remaining slots up to the desired Undercover count first, then White. With capacity one and both desired, use one Undercover. A pure-White preference remains pure White. Growing restores the desired pair. Document the adjustment in concise public copy.
- Auto restoration clears only the current mode's manual override; mode changes do not overwrite other preferences. Live matches remain frozen and reject group/settings mutations according to the existing contract.
- Advance the save envelope schema as needed and retain support for V1–V5. Open old saves without rewriting, losing history or rerolling an existing deal. Old finalized Quick results retain their meaning. Reject malformed new configurations and new-only fields/states falsely tagged as old schema; retain newer-version blocking, two-generation recovery and save-failure atomicity.

### Kings starter

- Choose uniformly from all living participants, regardless of role; repeated starters are allowed. Show only the public name, without side information.
- Reuse the current frozen initial starter for a new match. After each confirmed ordinary nonterminal elimination, persist the next survivor starter in the same transaction. No reroll when merely selecting/canceling an elimination or reopening.
- Offer one optional Next round action in the existing Kings table/ordinary-elimination surface when no elimination selection is pending. It advances the suggestion without eliminating anyone. Guard against stale/duplicate UI commands using match identity and current round/progress. It is not an obligatory gate to entering the next elimination.
- Keep round tracking internal or unobtrusive; a new full-screen round flow is out of scope. No Next round action during pending selection, private handoff, last chance or Result.
- Existing V5 Kings saves may contain a starter who has since been eliminated because the old UI never used it. Migrate these deterministically to a surviving public suggestion without changing roles, words, IDs, outcome or disk on open. Reopening the same legacy save gives the same suggestion.

### Presentation

- Extend the established rounded Group and Match surfaces; no new settings page, tutorial gate or animation redesign.
- Show the current total limit and explain a disabled lower/upper bound in DE/EN. Do not show a dead Kings White counter as though multiple evil Kings were supported.
- Count controls must be reachable at supported small-screen/large-text sizes and represented in the public accessibility hierarchy. Private cards remain excluded and synchronously concealed.
- Keep the recent explicit Edit, reorder, remove/undo and motion fixes intact. Do not claim new physical-device performance improvements from these rule changes.

## Testing Decisions

- The primary agreed seam is public Session behavior using real saved-session directories and reopen/failed-write scenarios. Test literal outcomes and known valid/invalid examples, never private RoleCounts methods or a copied implementation formula.
- The second existing seam is the rendered app using the production UI and input path. Verify role controls, atomic one-slot replacement, disabled-bound explanations, DE/EN, large text, actual next-person flow and Kings starter actions. These seams are retained under the user's existing autonomous planning authorization.
- Use the current Session behavior runner, legacy fixtures and Unity PlayMode tests as prior art. Apply TDD in vertical red/green slices, preserving failed evidence.
- Check all valid count combinations for sizes 3–20 through deal/reopen with mode-specific minima; include pure White, maximum total, invalid zero/negative/over-limit, mode isolation, shrinking/growing, Auto, removal/undo, restart, frozen match and rematch.
- Check sequential Quick multiple hits, first mistake after prior hits, each White correct/incorrect, all-White outcomes, repeated tie, duplicate/stale commands and save failure. Cover Classic all endings with several Whites, including final White pending judgment. Preserve legacy valid results and pending states.
- Check Kings initial/ordinary-elimination/no-elimination starters, eliminated-person exclusion, inclusion of White in the candidate set, same-person repeats, restart stability, stale commands, failed persistence and legacy dead-starter migration.
- Inspect actual Unity rendering, then build APK/AAB with the established Unity Android toolchain. Validate an actual update from code11, retained save generations and representative native flows. Reuse unrelated baseline evidence only when source equivalence is explicit.
- Report exact tested commits, packages, checksums, passing/failing checks and emulator/physical-device limits. User group enjoyment and phone smoothness are not proven by model tests.

## Out of Scope

Online play; new special roles; more Kings per side; changing Kings last-chance rules; individual ballots or app-enforced discussion; a new word-pack system; new Unity/package installations; unrelated visual/performance redesign; production signing and Store publication.

## Further Notes

Human Q1/Q4/Q6 explicitly rejected a single caught adversary and a mandatory simultaneous multi-selection. Human Q2/Q7 accepted team-wide White victory and one spoken chance per caught White with no public solution until the end. Q3 accepted pure White compositions and the shared initial-majority cap. Q5 accepted Kings' visible starter plus optional Next round. These answers remove the previous needs-info blocker. Routine seam/ticket details remain covered by the existing autonomous-workflow authorization.

The implementation uses one integration branch, separate ticket implementer worktrees, separate merges, independent Standards/Spec reviews and a correction loop. Review baseline is a0e929f956348563336899743258d8d4c1c22b04 (delivered 1.2.0/code11). Preserve all relevant evidence before removing worktrees.

