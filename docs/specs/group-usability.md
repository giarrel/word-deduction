# Gruppenbedienung: bewusst bearbeiten, einfach entfernen, Reihenfolge ändern

## Problem Statement

Beim Scrollen öffnet die große Namensfläche unerwünscht die Bearbeitung. Die Teilnahme-Buttons „Pause/Mitmachen“ belegen Platz, obwohl der Nutzer Personen lieber entfernt. Die gespeicherte Reihenfolge lässt sich nicht an die Sitzordnung anpassen. Die App soll die Vorbereitung unterstützen und möglichst wenig Aufmerksamkeit verlangen.

## Solution

Eine ruhige, scrollbare Spielerliste zeigt Namen als normalen Inhalt und rechts einen klaren Bearbeiten-Button. Darüber sind Umbenennen und Entfernen erreichbar; versehentliches Entfernen bleibt rückgängig machbar. In der Gruppe lässt sich die Reihenfolge direkt ändern und dauerhaft behalten. Normale Spielerverwaltung benötigt keine Teilnahme-Schalter.

## User Stories

1. As a host, I want to scroll over player names without opening editing, so that browsing the group is reliable.
2. As a host, I want an explicit Edit button on each player row, so that editing is intentional.
3. As a host, I want no Pause or Join action on ordinary rows, so that the group stays simple.
4. As a host, I want to rename a player without changing their identity or position, so that names remain trustworthy.
5. As a host, I want to remove a player from the group, so that absent people do not require a participation setting.
6. As a host, I want to undo the last removal with the original identity and position, so that mistakes are easy to correct.
7. As a host, I want to reorder the group before any new match, so that the phone follows our seating order.
8. As a host, I want a distinct reorder gesture or control, so that ordinary scrolling does not rearrange people.
9. As an alternative-input user, I want a non-drag way to move a player, so that reordering does not require a precision gesture.
10. As a returning host, I want the reordered group to survive process restart and mode changes, so that setup is not repeated.
11. As a player, I want the next match's handoff to use that order, so that names follow the intended sequence.
12. As a host, I want an interrupted or cancelled reorder to keep the last committed order, so that partial gestures do not corrupt the group.
13. As a host, I want storage failures to preserve the committed group and report the failure, so that a move is never falsely acknowledged.
14. As a host, I want duplicate and long Unicode names to remain distinguishable, so that editing and moving affect the intended person.
15. As a returning user, I want previously paused saved names preserved without silently joining the next game, so that this simplification does not lose data or add absent players.
16. As a host, I want a clear group-capacity message, so that adding a person never silently creates a paused row.
17. As a host, I want editing cancellation and Android Back to close the keyboard safely, so that a scroll or cancelled edit does not change a name.
18. As a German- or English-speaking user, I want all new controls and explanations translated, so that group setup is understandable.
19. As a player using a small screen or large text, I want reachable controls and readable names, so that list management remains usable.
20. As a returning player, I want existing live matches, word history and role assignments preserved, so that updating the group UI cannot reroll a game.

## Implementation Decisions

- Keep the existing Session as the deep module for group mutations, stable identities, saved order and durable transactions. Rendered UI consumes its detached projections and commands; no second group store.
- Remove the normal Pause/Join affordance. Names and avatars are not edit buttons. A clearly labelled localized Edit button replaces the former participation action; editing contains Remove, Cancel and Save.
- Normal groups contain at most 20 current players. A new Add at capacity reports the limit; it must not silently add an inactive person. Preserve the existing 40-record compatibility envelope for older saves until old inactive records are explicitly dealt with.
- Preserve older inactive people in a small collapsed, clearly labelled legacy saved-people section shown only when such records exist. They can be explicitly added back when capacity permits, renamed or removed. This is migration compatibility, not a new normal pause/archive workflow. Opening an old save does not rewrite it, activate people, discard records or alter a frozen match.
- Give reorder a distinct handle; ordinary row/name drags remain scrolling. Provide accessible move-up/move-down actions in editing as a non-drag alternative. Avoid persistent arrow clutter on every normal row. Use stable IDs and commit one complete order atomically at drop/action; invalid, stale or cancelled requests leave the prior state unchanged.
- Preserve removal Undo and disambiguated names. Removing/undoing and adding keep a predictable position. A reorder must not change roles, names, IDs, participation or word history.
- Baseline scope is free order editing in the group before every match; an already frozen live match retains its participants, assignments and consumed handoffs. The coordinator has separately asked whether reordering still-unread cards during handoff is also wanted; that additional live-match behavior is not silently inferred or implemented while the answer is pending.
- Preserve rounded styling, the existing font/accessibility behavior, platform Back and reduced-motion preference. The privacy pause on backgrounding is unaffected by removing the group participation buttons.
- No schema bump is needed solely to change an existing ordered player list. If additional durable information is genuinely necessary, migrate explicitly and preserve all supported old saves.

## Testing Decisions

- Existing agreed seams: public Session behavior with real temporary persistence, and the rendered app's real input/control surface. The user's delegated workflow covers these unchanged seams and routine ticket decisions.
- Work in vertical red/green slices. Demonstrate rename/remove/undo/reorder through commands and reopened projections, including stable identities, Unicode duplicates, capacity, old inactive data, live-match preservation and a denied save. Do not test private helper call order.
- Use actual pointer movement over names and over the reorder handle, not only direct button callback invocation. The known short movement over a name must no longer open editing. Scrolling must not reorder, commit a rename, or open the keyboard.
- Verify drag cancellation, release outside the list, restart, first/last positions, keyboard/alternative controls, DE/EN, small display and large text. Preserve only relevant failing/passing evidence.
- Run the relevant regression suites and install the final Android candidate as an update over the existing code8 save. Inspect actual screenshots; identify emulator versus physical-phone evidence accurately.

## Out of Scope

- New game rules, online play, replacing persistence, a general contacts manager, automatic seat detection or forced seating setup.
- Disabling the match's privacy pause or reopening completed private cards.
- Silently deleting or activating older saved people; silently rearranging an already frozen match.
- Production signing and Store publication.

## Further Notes

Source: the user's 9 October feedback and native code8 investigation. Initial review baseline is `53951d9f1d6cc0f9014db7615cfa7a159bb804d9`. The scroll/edit symptom was reproduced twice with small pointer movements; larger observed movement scrolled normally. A reorder handle is a reversible implementation choice, not a new game rule. Current instruction explicitly authorizes writing Specs and immediate implementation using the Engineering skills.
