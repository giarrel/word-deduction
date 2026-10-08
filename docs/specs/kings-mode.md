## Problem Statement

Groups want a third offline word-deduction mode built around protecting their own hidden leader and identifying the opposing leader. Ordinary participants should know their own word and leader without being told whether they are good or evil. The two leaders need different private information, and catching the evil leader should trigger one last strategic choice rather than immediately ending the game.

The existing Quick and Classic modes do not express these rules. Reusing Classic unchanged would disclose eliminated participants' sides, apply the wrong victory conditions, and add voting/runoff screens that the group does not want. The app must remain a discreet assistant: the people at the table give clues, discuss, vote, resolve ties, and judge spoken answers.

## Solution

Add a separate third mode, using **Kings / Könige** as its working label. Preserve Quick / Schnell and Classic / Klassisch as separate modes.

Each side has exactly one King / König (the Group Leader / Anführer). The good side starts with a strict numerical majority. Ordinary Civilians / Bürger receive the good word and their good King's name; ordinary Undercover receive the related alternative word and Mr. White's name. Both ordinary card types use neutral wording and do not disclose their side.

The good King knows the good word and all evil participants, but cannot distinguish which of them is the evil King. **Mr. White is the evil King**, receives no word, and knows his own side and team but neither the good word nor the good King.

After the initial private Handoff / Übergabe, play happens around the table. The app records only confirmed eliminations and resolves the resulting game state. Ordinary eliminations reveal only “Not a king / Kein König”. A good King's elimination wins for the evil team. An eliminated Mr. White must choose one last chance: guess the good word, or identify the good King. One successful attempt wins for the entire evil team; failure wins for the entire good team. If only the two Kings remain, the evil team wins automatically.

Private cards are available during their own initial handoff only. There is no later review and no viewing timer. Memory and the risk of looking conspicuously long are intentional parts of play.

## User Stories

1. As a host, I want to select a distinct third mode, so that our group can choose the leader game without changing Quick or Classic.
2. As a returning player, I want the existing two modes to retain their rules and settings, so that familiar games continue to work.
3. As a host, I want to reuse my saved Group, so that trying this mode requires no repeated name entry.
4. As a host, I want to add, rename, pause, restore, or remove players between matches, so that the app accommodates a changing group.
5. As a host, I want to start with at least five active participants, so that both leaders have teammates and the good side has a majority.
6. As a host, I want exactly one King on each side, so that each team's protection objective is clear.
7. As a host, I want to choose the number of ordinary Undercover within valid limits, so that I can adapt the challenge to the group.
8. As a host, I want the role summary to distinguish ordinary Undercover from Mr. White, so that I understand the actual evil-team size.
9. As a host, I want a valid default distribution, so that I can begin without configuring a rules matrix.
10. As a host, I want my count preference to survive reopening and mode changes, so that setup does not need repeating.
11. As a host, I want group-size changes to preserve my group and clearly show any effective count adjustment, so that invalid combinations do not derail the next match.
12. As a participant, I want teams and Kings dealt afresh at random, so that previous matches do not reveal my next role.
13. As a participant, I want repeated assignments to remain possible, so that nobody can infer a role from a rotation guarantee.
14. As a participant, I want a familiar bilingual word pair drawn from the existing collection, so that the new mode works with the included content.
15. As a returning group, I want the existing persistent word history to apply, so that this mode does not restart early word repetitions.
16. As an ordinary Civilian, I want my private word and leader's name, so that I can give clues and protect my leader without learning my side.
17. As an ordinary Undercover, I want my alternative word and leader's name in the same neutral presentation, so that the card does not identify me as evil.
18. As an ordinary participant, I want no role label, color, icon, speech label, or layout cue that distinguishes Civilian from Undercover, so that my uncertainty is genuine.
19. As the good King, I want to know that I am good and see the good word, so that I understand my objective.
20. As the good King, I want the names of all evil participants without a marker for their King, so that I can guide my team while still having to find the critical target.
21. As Mr. White, I want to know that I am the evil King and receive no word, so that I know I must bluff.
22. As Mr. White, I want the names of my ordinary Undercover teammates, so that I know my team without learning the good King or either secret word.
23. As a participant, I want long or duplicate names to remain distinguishable on private information cards, so that I remember the correct people.
24. As a participant, I want my name shown before revealing, so that the phone is handed to the correct person.
25. As a participant, I want the established pull-up or hold-to-reveal interaction, so that the new mode feels familiar.
26. As a participant, I want all private information hidden immediately on release, interruption, or app backgrounding, so that others cannot glimpse my card.
27. As a participant, I want freedom to choose how long I examine my initial card, so that looking time can become part of the bluff.
28. As a participant, I want no later access to a completed private handoff, so that remembering information remains part of the challenge.
29. As a participant whose initial handoff is interrupted, I want to resume that same unfinished handoff safely, so that an interruption does not lose my original assignment.
30. As a group, I want to give clues and discuss without app confirmations, so that the phone stays out of the conversation.
31. As a group, I want voting and runoffs explained as table rules, so that we can resolve them without digital ballots or vote-count entry.
32. As a group, I want a tied vote to require no app action, so that another clue round can begin immediately.
33. As a host, I want to record the one person actually eliminated by the group, so that the survivor list and outcome stay correct.
34. As a host, I want to correct an unconfirmed name selection before revealing its consequence, so that a mistaken tap does not spoil the game.
35. As a surviving participant, I want ordinary eliminations to show only “Not a king”, so that neither the app nor its public state reveals the eliminated side, word, or leader.
36. As an eliminated participant, I want to remain part of my original team's final result, so that elimination does not exclude me from a team victory.
37. As a group, I want play to continue through ordinary eliminations and numerical parity, so that finding a King remains the objective.
38. As a group, I want the good King's elimination to end the match with an evil-team victory, so that the consequence is immediate and clear.
39. As a group, I want the evil King's elimination to offer his last chance before any general reveal, so that he cannot use the result screen to choose correctly.
40. As Mr. White, I want to choose irrevocably between a word attempt and a King attempt, so that I get one strategic chance rather than both.
41. As Mr. White, I want to make that choice and attempt without team advice, so that the final decision belongs to me.
42. As Mr. White choosing the word, I want to speak one answer before the target word is shown, so that the attempt is fair.
43. As a group, I want to judge the meaning of that spoken answer, so that true synonyms and harmless grammatical differences do not cause an unfair loss.
44. As Mr. White choosing the King, I want to identify one surviving participant, so that my decision has one definite target.
45. As a group, I want a successful last chance to win for the whole evil team and a failed one for the whole good team, so that Mr. White has no separate solo victory in this mode.
46. As a group, I want a match with only the two Kings left to end with an evil-team victory, so that we cannot become stuck in an unavoidable voting tie.
47. As a returning group, I want eliminations, completed handoffs, a committed last-chance choice, and pending answer judgment to survive app restarts, so that reopening cannot undo decisions or provide an extra attempt.
48. As a host, I want an unsuccessful save to leave the committed game unchanged and report the failure, so that the app never silently loses or pretends to record progress.
49. As an existing user, I want an app update to preserve my group, settings, word history, and any live Quick or Classic match, so that the third mode does not cost me existing data.
50. As a group, I want an explicit final result with both words, teams, and Kings, so that we can discuss what happened after the outcome is settled.
51. As a group, I want one-action Rematch / Folgepartie with all active group members restored, so that we can start playing again immediately.
52. As a host, I want an explicit way to abandon a spoiled match while retaining the Group, so that accidents do not require rebuilding setup.
53. As a German- or English-speaking player, I want complete cards, help, actions, errors, and results in my selected language, so that rules are understandable throughout the game.
54. As a player using large text or a small phone, I want the names and leader information to remain readable and privately inspectable, so that memory is tested rather than eyesight.
55. As a screen-reader or alternative-input user, I want equivalent private reveal and public navigation without additional secret disclosures, so that accessibility does not change what other participants can learn.
56. As a group, I want offline play without accounts, network setup, or new services, so that passing one phone remains sufficient.

## Implementation Decisions

### Confirmed game rules

- This is a separate third GameMode. Quick and Classic retain their existing behavior. Kings/Könige is a working product label, not a new fourth mode or a renaming of Classic.
- Preserve the existing distinction between Group, Player, Match, Participant, Clue round, Handoff, Reveal, Vote, and Rematch. Elimination affects a Participant in one Match, never the Player's active Group membership.
- Add mode-scoped terminology for King/Anführer, good team, evil team, and last chance. The good King is a Civilian with a leader responsibility; Mr. White is the evil King. Ordinary Undercover retain the alternative word. The existing glossary description of White's Classic ability must remain mode-scoped rather than becoming a universal rule.
- Exactly one good King and one Mr. White exist in this mode. At least one ordinary teammate must accompany each King, and the initial good team must strictly outnumber the entire evil team. Count both Kings in their respective teams. Five participants therefore means three good participants including the good King versus one ordinary Undercover plus Mr. White.
- The configurable Undercover count counts only ordinary Undercover; Mr. White is additional and mandatory. This mode does not offer an independent White count or the Classic White toggle.
- Shared count-limit requirement for the next version: use one initial-composition policy across Quick, Classic, and Kings. Ordinary Undercover plus all Mr. Whites together must be fewer than the good participants at the start; count each King within its own side. Do not maintain separate independent maxima that permit an invalid combined total. Kings adds its already-agreed structural constraints (one White, at least one ordinary Undercover, minimum five). This shared initial limit never becomes a live-match victory condition. Free controls and the remaining base-mode rule decisions are tracked in [issue #11](https://github.com/giarrel/word-deduction/issues/11).
- Assign participants randomly without role-rotation guarantees or dependence on earlier assignments. Use the existing word catalog, language selection, randomized word-pair orientation, and durable draw history. Neither King status nor team membership changes the existing word meanings.
- Every ordinary card reveals only its owner's word and own leader's display name. It must not reveal the owner's side or role. The good King sees his side, the good word, and the set of all evil participants, including Mr. White, without identifying which is the King. Mr. White sees his evil-King identity and his ordinary teammates, with no word and no identification of the good King.
- Private name lists use a neutral stable order independent of hidden role, such as the existing participant order. They must not sort, group, label, or decorate Mr. White separately for the good King. Preserve stable IDs and visible duplicate-name disambiguation.
- Each participant has one initial handoff opportunity. Once that handoff is completed and committed, no previous-card navigation or later review is available. Covering and reopening the current, not-yet-completed handoff is allowed, including safe recovery after an interruption; it must not reopen anyone's completed handoff. This implements the agreed boundary of no later review after one's own handoff rather than penalizing an interrupted touch gesture.
- There is no fixed viewing duration, timer, forced countdown, or role-dependent viewing period. Longer viewing can be suspicious; ordinary participants can deliberately look longer and leaders can choose to risk incomplete recall.
- Ordinary eliminations remove the person from future clues and votes but disclose only “Not a king / Kein König”. Their side, word, and leader remain undisclosed until the final result. They still share their team's win or loss.
- Eliminating the good King immediately wins for the entire evil team. Eliminating Mr. White enters the last chance and takes priority over final disclosure. Ordinary eliminations otherwise keep the match live.
- If a confirmed ordinary elimination leaves exactly the two Kings alive, the evil team wins automatically. This is the agreed explicit exception to the King-elimination rule. Do not import Classic's one-Civilian-left or parity-based victory checks into this mode.
- Mr. White chooses either word or King, alone and without advice. The mode choice is binding and persistent. He receives one attempt, cannot switch branches after commitment, and cannot retry after failure.
- A correct word attempt or correct King identification wins for the whole evil team, including eliminated participants. An incorrect attempt wins for the whole good team. Mr. White never wins alone in this mode.
- Only the terminal result exposes the full assignments and both words. The one earlier permitted exception is revealing the good word for group judgment after the word branch and spoken answer have been irrevocably committed; the good King's identity still stays concealed until the terminal result.

### Rules implemented by the people at the table

- Each Clue round: every survivor gives one single-word clue, followed by discussion and simultaneous pointing at another survivor. One vote per survivor; the highest count wins without an absolute-majority requirement.
- An initial tie produces one spoken runoff among participants tied for the highest vote count. A repeated tie starts another clue round without eliminating anyone. These are help/rule text only. Do not create ballots, vote totals, a runoff state, tie buttons, or a required transition for a tied vote in this mode.
- Choose a random surviving starter for each clue round, including Mr. White, then continue around the table. Document this as a group-run rule. The app need not track clue-round numbers or force a new starter selection after a tie; a starting-person suggestion must never become a required action or exclude Mr. White.
- Clues may not use the participant's own secret word, obvious forms or direct translations of it, or spelling/initial-letter hints. Discussion and identity bluffs are allowed; directly saying one's own secret word remains forbidden. Do not inspect speech, validate clues, count turns, or police lies.
- Showing a private card to others is forbidden as a social rule. Assertions about leaders may be true or false; revealing one's real leader is a strategic risk, not a forbidden identity statement.
- One spoken word answer is judged by the group for meaning. Articles, singular/plural changes, and true synonyms can count; merely related words do not. There is no speech recognition, typed-answer matching, synonym database, or automated semantic judge.
- Eliminated participants stop contributing clues and votes. No advice during the last chance. Accidental invalid clues or word leaks do not create automatic penalties: normally play continues, or the group agrees to abandon and restart a spoiled match.
- The app records the outcome of social decisions, not the interpersonal process that produced them. This boundary is a feature requirement, not an optional simplification.

### Integration with the existing app

- Keep the Unity-independent Session boundary established by the existing architecture decision. Session owns legal actions, dealing, role-specific disclosure, match progression, validation, outcomes, and durable state. The rendered UI remains a consumer of public/private projections and commands.
- Extend the existing Session and match contracts rather than creating a parallel game manager or a second persistence system. Prefer one owner-scoped private-card projection capable of carrying an optional word, permitted role information, leader identity, and permitted known participants. Public match projections must contain only currently allowed information.
- The existing ordinary-elimination projection exposes a Role for Classic. Reusing it unchanged for this mode would leak the side. Introduce mode-appropriate disclosure at the Session boundary; hiding a label in the UI alone is insufficient. Do not expose surviving per-side totals, leader associations, hidden role flags, or secret word fields in public projections before their permitted disclosure.
- Keep card backs, handoff navigation, and ordinary-card presentation neutral across roles. On release, focus loss, app backgrounding, or interrupted gestures, immediately conceal the entire private projection, including names and leader lists. Apply existing Android privacy, logging, focus, multi-touch, and accessibility protections to every new private field.
- Use the existing persistent Group and its supported limits: 5–20 active participants for this mode and up to 40 saved players. Keep normal name editing, participation changes, removal undo, and stable IDs. A running deal freezes participants, roles, words, language, and settings.
- Routine configuration integration: use the existing adaptive Classic ordinary-Undercover defaults until the host makes a manual choice. Preserve an explicit preference independently for this mode. If a smaller group makes that preference too large, use the largest valid effective count, show that adjustment clearly, and retain the preference for a later larger group. Never silently delete names or reset the group. Below five active participants, clearly show how many are missing and do not deal. Explicit attempts to configure an invalid count must not create an invalid match.
- The above configuration behavior is a reversible integration choice made while synthesizing this spec, not an additional rule approved in the grilling. It implements the existing request for persistent settings, valid combinations, and low-friction group changes.
- After handoffs, offer a compact survivor view and direct entry of the person the group actually eliminated. Allow changing an unconfirmed selection; confirm before disclosing an irreversible consequence. Do not require “begin discussion”, “begin vote”, “runoff”, or “next clue round” screens. An ordinary “Not a king” acknowledgment may be integrated into that same view.
- The last-chance choice is persisted before its branch begins. In the word branch, the group confirms that the spoken answer has been given before the app durably locks that answer stage and reveals the good word for Correct/Incorrect judgment. Do not store audio or the spoken text. Do not reveal a target or acknowledge progress when its required write failed.
- In the King branch, show surviving participants without side markings. Mr. White's single target is committed by stable participant ID, then compared with the actual good King and resolved. A pending unconfirmed target can be corrected; a confirmed target or committed branch cannot be changed.
- Persist enough phase and decision information to reject duplicate or stale actions, including actions from an older Match or earlier stage. Back, restart, or double taps must not turn one attempt into multiple attempts or expose another branch.
- Extend the saved-session envelope from its currently supported version 4 with a new version for the new mode and state. Preserve existing serialized identities and the meaning of existing enum values. Migrate supported old saves without rerolling any live Quick/Classic deal, changing accepted names, resetting word history, or altering old-mode preferences.
- Persist the selected mode, this mode's count preference, frozen deal, both King identities, completed handoff position, eliminated participants, last-chance commitment/answer stage, pending target or judgment, and final outcome. Revealed-on-screen status remains transient: reopening starts covered or on the existing safe resume surface.
- Retain the current durable-save, validation, backup, blocked-write, unsupported-newer-save, and explicit damaged-data recovery policies. Do not introduce a silent fresh start or claim success on a failed save. Backup recovery may restore an earlier valid checkpoint and must keep its explicit warning; this spec does not promise tamper-proof enforcement or zero rollback after damaged storage.
- Add complete German and English copy, including short role-specific private explanations and optional rules/help. Use the existing rounded visual language and accessible reveal alternative. Secret name lists and large text must remain readable without clipping or disclosure outside the private surface.
- Rematch preserves active Group membership, mode, valid effective settings, and word history, while generating a new Match and fresh random assignments. Eliminated participants return. Abandoning a match requires the existing deliberate action and retains the Group.
- Do not refactor the old modes into the new rules as part of this feature. Existing Quick/Classic behavior and its tests remain regression requirements.

## Testing Decisions

Use the already-established testing strategy from the project: the public Session contract is the primary behavioral seam, with the rendered app as the necessary second seam for gesture, layout, disclosure, and Android lifecycle behavior. Reuse these seams rather than adding direct tests of private helpers or multiple parallel engines. This preserves the earlier delegated test-design decision; no new test seam or product interview is required.

A good test drives observable user actions and checks the information, outcome, and durable state the caller is entitled to see. It does not mirror an implementation algorithm, inspect private call order, or assert file/class layout. Use real temporary persistence and controlled randomness at the existing external boundary. Keep new tests focused on changed behavior and shared regressions.

1. **Session: configuration and dealing.** Exercise every supported group size and valid count, including five-player minimum, largest groups, maximum valid Undercover counts, invalid counts, group shrink/restore, and mode switching. Assert exactly one King per side, no overlap, strict initial good majority, correct words, no word for White, ordinary participants' correct leader, and durable preferences. Controlled examples must demonstrate that either side of a word pair may be the good word and that roles may repeat across matches.
2. **Session: information contract.** Compare ordinary Civilian and Undercover projections, allowing only their own words and leader IDs to differ. Verify the good King's unmarked evil-name set and White's permitted team knowledge. Public snapshots, ordinary-elimination output, action errors, and in-progress results must not expose hidden sides, team survival counts, words, or King identities beyond the phase's rules.
3. **Session: handoff lifecycle.** Reveal, cover, complete, and reopen at every participant position. Completed handoffs cannot be revisited through old IDs, restart, back navigation, or stale commands. Interrupted unfinished handoffs retain their original private information and reopen covered. No timeout changes the deal or advances a handoff.
4. **Session: ordinary play.** Record confirmed ordinary eliminations directly without entering a vote or runoff workflow. Validate survivors and “Not a king” disclosure; reject eliminated or unknown targets and duplicate/stale confirmations. Test an even team split and a lone remaining good King with more than two total survivors: neither should import a Classic automatic win.
5. **Session: outcomes.** Cover direct good-King elimination, White elimination into last chance, both success/failure paths for word and King choices, and the exact two-Kings-only automatic evil win. Verify the winning team includes eliminated teammates, White never wins alone, and complete disclosure occurs only after the allowed result boundary.
6. **Session: irreversible last chance.** Test branch locking, answer-stage locking before word disclosure, pending target correction, committed target finality, prohibition on switching after a failed attempt, and stale actions from other matches. Reopen at choice, committed word branch, answer locked and pending judgment, committed King branch, pending target, and terminal result.
7. **Session: durability and compatibility.** At new durable actions, reproduce denied writes, interrupted saves, valid backup recovery, unsupported newer schemas, and invalid-but-checksummed snapshots. Verify no successful acknowledgment or newly disclosed secret accompanies a failed required commit. Upgrade saved groups and representative live Quick/Classic phases from the current release and preserve IDs, names, assignments, preferences, and word history.
8. **Rendered app.** Exercise the third-mode selection, effective count display, all private-card variants, small and maximal name lists, both languages, long/duplicate/Unicode names, large text, concealment on release and focus loss, normal elimination, both last-chance branches, result, and direct rematch. Check that ordinary sides remain indistinguishable and that public hierarchy/accessibility labels do not retain private information after concealment.
9. **Rendered flow simplicity.** Demonstrate a full match whose required app interactions are setup, initial handoffs, recording actual eliminations, resolving a last chance when applicable, and result/rematch. Assert there are no third-mode ballots, vote counters, tie/runoff controls, clue confirmations, required discussion screens, card-review action, or countdown.
10. **Android validation.** Run representative offline matches through all distinct endings on the built Android app. Cover Home/Recents/backgrounding while private cards are open, force-stop/relaunch around completed handoffs and pending last chance, update from the current release, and large-font/small-screen name-list readability. Review real screenshots and check logs for names, words, teams, or leader associations. Identify emulator versus physical-device evidence accurately.
11. **Human-rule verification.** Review German/English rules against the accepted clue, voting, runoff, random-starter, bluffing, silence, answer-judgment, and accidental-leak rules. Do not create app state or speech-analysis tests pretending to enforce human behavior. Balance, memory burden, and table enjoyment need actual group play and cannot be proven by automated tests.

Prior art already exists in the Session test suite for automatic role mixes, frozen deals, persistent White judgment, elimination/restart progression, invalid-action rejection, storage failures, legacy imports, Unicode-name restoration, and durable word history. Existing rendered Classic, recovery, content, and polish tests provide the appropriate fixtures for actual controls, privacy, private-card readability, and mobile lifecycle checks. Extend those styles without making their current test counts a feature requirement.

These are acceptance and future validation requirements, not claims that the new mode has already been implemented or tested.

## Out of Scope

- Implementing free Undercover/Mr.-White controls in Quick/Classic and White in Quick is covered by the linked next-version requirement [#11](https://github.com/giarrel/word-deduction/issues/11), not discarded. Its remaining victory-rule decisions must be settled before that feature is implemented. The shared count-limit policy is included here so that all three modes use consistent bounds.
- A separate Assassin, an additional all-knowing good role, multiple Kings per side, multiple Whites in this mode, or a third independent side.
- Reopening completed private cards, enforced viewing time, countdowns, or a memory aid during play.
- Digital ballots, per-player vote collection, vote counting, runoff/tie state, forced clue/discussion screens, night phases, speech processing, automatic rule policing, penalties, or consultation monitoring.
- A standalone White victory or the old Classic end conditions in this mode.
- Online play, accounts, matchmaking, cloud synchronization, purchases, analytics, or backend services.
- A new word-pack system or new content production; use the existing bilingual catalog.
- Production signing, store publication, store-account work, or an assertion that the unimplemented feature is release-ready.
- Implementation, task decomposition, or creation of an autonomous development goal merely because this specification is published.

## Further Notes

- Scope clarification after publication: the user explicitly requires freer role counts in the base modes and common upper limits across all three modes. [Issue #11](https://github.com/giarrel/word-deduction/issues/11) preserves that requirement and its unresolved base-mode outcomes. This Kings specification remains ready for implementation; #11 must not silently inherit Kings victory rules.
- Source of truth: the user's third-mode decisions, grilling answers Q1–Q19, and explicit requirement that interpersonal rules remain at the table. Newer answers supersede the earlier separate-Assassin idea and the provisional assumption that the evil King has an Undercover word.
- The existing two-mode release specification is [issue #2](https://github.com/giarrel/word-deduction/issues/2). This is a new feature specification, not a reopening of the completed Android release.
- The codebase inspected for this synthesis is the current integration baseline at commit 478953f496c0a8e88f6849c32960f02c2039f671. The agreed Unity-independent Session architecture remains applicable; no conflicting architectural decision is introduced.
- User-approved rule decisions are distinguished above from routine integration choices, notably working naming and safe effective-count behavior. No further interview is needed to begin implementation from this spec when that is separately requested.
- The initial team-majority rule and the two-Kings exception are deliberate decisions. They are not claims of established balance or an exact reproduction of another game's rules.
- In this trusted local pass-and-play game, the app protects ordinary use and safe resumption. It cannot prevent another person from looking over a shoulder, intentionally handing the phone to the wrong person, or manipulating device storage.
- Publication with the ready-for-agent label means the feature is specified for future work. It does not claim implementation, validation completion, or authorization to publish a new app release.
