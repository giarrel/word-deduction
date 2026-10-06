# Word Deduction: Android release v1

Status: implementation specification, 6 October 2026. Parent: [Autonomous Android release effort](https://github.com/giarrel/word-deduction/issues/1). The GitHub spec issue is canonical; this file mirrors its substantive requirements.

## Problem Statement

A changing group wants to start a word-deduction party game and keep playing. Re-entering names, intrusive setup, unreliable secret reveals, lost progress, repeated or unfamiliar words and unclear results interrupt the conversation. The app should handle the administration so the people can play together.

## Solution

An offline Android app for one shared phone, with Quick and Classic modes, complete German and English support, a substantial included word collection, a persistent editable group, and a playful, rounded interface. Names appear on face-down cards; pulling up reveals the word, releasing immediately hides its text while the card settles down. The group talks and votes aloud; the app records the decision and guides the next step.

No account, network, ads, purchases, forced timer, content downloads or mandatory tutorial are part of this version. The default home is the saved group with a clear Play action and a compact two-mode selector. Instructions are short, contextual and available on demand.

## User Stories

1. As a host, I want to open the app directly into my group, so that I can start without navigating a dashboard.
2. As a new host, I want a single obvious field for names, so that setup is understandable without a tour.
3. As a host, I want to add several players without repeatedly opening dialogs, so that a large group is quick to prepare.
4. As a host, I want to rename someone in place, so that correcting a name preserves their identity.
5. As a host, I want to pause someone without deleting them, so that they can rejoin next time.
6. As a host, I want to remove a player and undo an accidental removal, so that editing is forgiving.
7. As a host, I want duplicate names distinguished visibly, so that the correct person receives each card.
8. As a returning host, I want names, participation and mode retained after closing the app, so that I never recreate the same group.
9. As a host, I want a clear explanation if too few people are active, so that starting never creates an invalid match.
10. As a host, I want automatic sensible role counts, so that I do not configure a rules matrix.
11. As a participant, I want to see who should hold the phone before any secret appears, so that handoffs are clear.
12. As a participant, I want to pull the card up to see my word and release it to hide it, so that disclosure is intentional.
13. As a participant who cannot comfortably drag, I want a hold-to-reveal alternative, so that I can play without a precise gesture.
14. As a participant, I want an interrupted or cancelled gesture to hide the word immediately, so that phone interruptions do not disclose it.
15. As a participant, I want the next-person action available only after a successful reveal and with the card closed, so that accidental taps cannot skip my word.
16. As a Civilian or Undercover, I want to receive only my word, so that the app does not reveal my initially unknown role.
17. As Mr. White, I want a concise private explanation that I have no word, so that I know to bluff.
18. As a group, we want roles randomly assigned without a predictable rotation, so that the seat order cannot identify an adversary.
19. As a group, we want to put the phone down during clues and discussion, so that the app does not require a tap per spoken clue.
20. As a group, we want a visible starting player and brief instructions, so that the conversation begins smoothly.
21. As a Quick group, we want one clue round, one decision and a clear result, so that nobody sits out successive rounds.
22. As a Classic group, we want to eliminate players and continue with survivors, so that information develops across rounds.
23. As a group, we want a clear tie rule, so that tied votes do not cause arbitrary eliminations or a dead end.
24. As a host, I want to confirm the selected suspect before revealing their role, so that an accidental tap is correctable.
25. As Mr. White, I want the group to judge the meaning of my spoken guess, so that spelling or synonyms do not cause an unfair loss.
26. As a remaining Undercover, I want White's guess resolution to avoid displaying the majority word, so that a continuing match is not spoiled.
27. As a group, we want results to explain who won and why and reveal the words, so that the match has a satisfying conclusion.
28. As a group, we want one action from result to the next deal, so that we keep our momentum.
29. As a host, I want to change the group between matches, so that arrivals and departures do not reset everything.
30. As an eliminated participant, I want to be included in the next match automatically, so that elimination does not remove me from the group.
31. As a host, I want an interrupted match restored without reshuffling, so that closing the app does not change the game.
32. As a host, I want an explicit resume or abandon choice, so that going back cannot silently destroy a match.
33. As a host, I want understandable recovery when saving fails, so that unpersisted actions are not presented as safe.
34. As a player, I want everyday, fair word pairs across many themes, so that obscure knowledge does not reveal my role.
35. As a returning group, we want used pairs remembered, so that the word collection stays fresh across sessions.
36. As a German-speaking group, we want German UI, rules and words, so that no translation is necessary while playing.
37. As an English-speaking group, we want the same complete experience in English.
38. As a bilingual host, I want a simple persistent language choice, so that changing language does not discard my group.
39. As a phone user, I want readable text, generous touch targets and content clear of system bars and the keyboard, so that handling the app feels easy.
40. As a player, I want a polished rounded design with restrained responsive motion, so that the app feels pleasant in use.
41. As a player sensitive to motion, I want reduced motion, so that visual polish does not impede me.
42. As an offline group, we want all core content available from first install without permissions or internet, so that a trip or poor connection is irrelevant.
43. As an owner, I want a versioned reproducible build and honest validation evidence, so that release readiness can be assessed.

## Implementation Decisions

### Product and rules contract

These are delegated product choices, not claims of universal Undercover rules. They may change only with an updated spec and regression tests.

- Group capacity: 3–20 active players for Quick; 4–20 for Classic. Up to 40 saved people can be paused for later. Trim names; require 1–24 Unicode text elements; reject blank/control-only input. Duplicate visible names get stable, visible disambiguators without changing the entered name or ID.
- Quick is the first-run default. Exactly one Undercover, everyone else Civilian, no White. Each person receives a word, one clue round and discussion occur, then the group votes aloud. Correctly accusing the Undercover wins for the Civilians; accusing a Civilian wins for the Undercover. Result is immediate after confirmation.
- Classic chooses 1 Undercover for 4–7 active players, 2 for 8–12, and 3 for 13–20. White is a single optional additional role, off initially, requiring at least 5 active players. The rest are Civilians. The group screen describes the automatic mix. At four players a saved White preference is shown as unavailable; the effective mix omits White without deleting the preference. Quick never uses the saved Classic White preference.
- Classic ends when all adversaries are eliminated (Civilian win), or only one Civilian remains and at least one adversary survives (surviving adversary side wins), or an eliminated White guesses correctly (White wins immediately). Check White's pending guess before any other terminal condition resulting from that elimination. Wrong White guess removes White and evaluates the remaining match. Survivors continue to the next clue round otherwise.
- Every round's starting player is selected at random from all survivors, including White. There is no role-based exclusion, deterministic rotation or fairness guarantee that leaks roles. White may have to improvise without prior clues; this is explained in help. No public hint category is added for White.
- Voting is simultaneous pointing or spoken group voting outside the app. The host selects the most-voted survivor, then confirms their name. The app never claims to have counted unentered individual ballots. A tie offers one brief runoff among tied people. If still tied: Quick ends with the Undercover escaping; Classic starts another clue round without eliminating anyone. No random elimination.
- A confirmed Classic elimination shows the eliminated role but never their secret word. White speaks their guess aloud, then the group selects Correct or Incorrect based on meaning. The majority word remains hidden until final result. Confirming a suspect is the point after which that vote cannot be undone because information has been disclosed.
- Results explain the winning side and reason, reveal both words and role assignments, and offer prominent Next match plus secondary Edit group. Eliminations do not persist as group participation changes.
- Back first cancels the current edit/selection/help. In a live match, Back leads to a covered pause surface with Resume and explicit Abandon match; abandonment has confirmation and never deletes the group. Mid-match arrivals/renames are managed after abandoning or finishing; the running match's participants and language remain fixed. This keeps the first release understandable without a second concurrently editable group flow.

### Modules and storage

- Unity 6000.3.25f1, Android first, portrait-first responsive UI. UI Toolkit is the chosen presentation system for responsive rounded menus and a translating/tilting card; a physical 3D card is unnecessary. The design will be verified in actual Unity rendering.
- A deep Session module owns the group, match lifecycle, rule transitions, word selection/history and persistence transaction. Its small public interface expresses user actions and returns safe views. Presentation does not mutate DTOs or implement win conditions.
- Group members have stable IDs. Starting a match snapshots active participants, names, mode, effective roles, word language and chosen words. Progress is separate from that immutable match setup. Loading never reassigns roles, consumes another word pair or advances a turn.
- Persistent commands succeed only once their new complete state is durably written. Versioned JSON with checksum and primary/previous-generation recovery is stored in app-private persistent storage through atomic replace or a recoverable equivalent. Write failures retain the previous in-memory and on-disk state and offer retry. Unknown newer versions and invalid snapshots must not be silently overwritten.
- A corrupt primary can restore a validated previous generation with a visible recovery notice. If neither can be used, preserve damaged files and require explicit acknowledgement before starting fresh. No logging of personal names, words or role assignments in release logs.
- Reveal visibility is volatile. On pointer release/cancel/capture loss, focus loss, pause, navigation and before the next owner, erase the secret text immediately; the return animation is decoration after concealment. Never restore an open card from disk. Background/task previews must not show secret words.
- A narrow randomness seam supports deterministic behavioral tests; production uses unbiased shuffling with a fresh random source, without fixed-seat patterns. Save the outcome, not instructions to rerun the draw.
- One included bilingual catalog, at least 500 original pairs with DE and EN variants, at least 700 distinct normalized words in each language, spanning at least 12 broad familiar themes. No competitor-list copying, celebrity/brand dependency, explicit adult material, trivial synonyms or pairs that translate to the same concept. Editorial review is required in addition to validation. Do not pad with meaningless pairs to hit counts.
- A persistent shuffled draw cycle prevents pair-ID repeats until all current eligible pairs are used, across language changes and restarts. On cycle rollover, avoid an immediate repeat. Prefer avoiding words from the last ten matches when compatible with the remaining deck; pair-cycle completion takes precedence and selection must terminate. Randomly swap pair sides; persist the resulting words.
- UI language initially follows German system locale or defaults to English. An always-discoverable DE/EN choice on the group screen changes both UI and future word language. No language change inside a running match. Every player-facing string, error, rule and accessibility label has both translations; no concatenated grammar fragments.
- The palette is warm ivory with dark ink, lavender as the primary action color and restrained coral/mint accents. Broad corner radii, strong typography, generous spacing, a distinctive secret card and small responsive transitions form the style. Normal text contrast aims at 4.5:1; role meaning must not depend on color alone. This is a product design choice, not a research finding.
- Core targets are at least 48 dp equivalent; long names, German strings and keyboard input must fit. The system back path and safe area/insets are handled explicitly. Reduced motion skips bounce/tilt while retaining clear state changes. Accessibility is validated at the rendered app; no claim of TalkBack support until exercised on Android.
- No remote backend or monetization SDK. All words ship inside the app. Only necessary Unity modules and development/test tooling are included. Runtime permissions and manifest/network behavior are inspected on the actual build.

## Testing Decisions

There is no pre-existing game/test implementation. The user's explicit autonomous-testing mandate delegates the following two seams: Session's public action/view interface with a real temporary store, and the rendered application's actual inputs and lifecycle. Tests exercise observable behavior, not private methods or mocks of owned modules. Follow red → green in vertical slices and preserve failing evidence.

| Contract | Required scenarios / expected outcome |
|---|---|
| Group identity and durability | Add, rename, pause/resume, remove/undo; duplicates; Unicode; 40 saved people; close/reopen after each confirmed action; settings preserved. |
| Match integrity | Seeded fixtures at every supported player count and mode; exact roles; no public secret role labels; no mutation of the setup when advancing. |
| Quick | Correct suspect, wrong suspect, runoff and repeated tie; one clue round; final reveal; next match includes everyone active. |
| Classic | All win paths, ongoing rounds, eliminated participant cannot be selected again; White correct/incorrect; White guessed before survivor win evaluation; tie has no elimination. |
| Secret card | Tap without drag, slow drag, early release, pointer outside, cancellation, second finger, Next while open, focus/pause, scene return and process restart. Secret disappears immediately on each hide trigger. |
| Persistence failures | Restart in every phase including unconfirmed suspect/White guess; primary corruption, backup corruption, interrupted write, denied write, unsupported schema; no silent reset or successful UI acknowledgement of failed save. |
| Content | Counts, unique IDs, nonblank bilingual strings, normalized duplicate/reversed pairs, identical sides, familiar editorial quality, at least 12 themes; full draw cycle across restarts and language switches. |
| Localization and layout | All screens in DE/EN, smallest supported phone and tall phone, 20 active players, long names, safe area, keyboard, visible focus and tap targets. |
| Offline and platform | First launch without network, suspend/resume, force-stop/relaunch, Back gesture/button and task preview privacy in Android runtime; actual manifest and native libraries checked. |
| Build and packaging | Compile, EditMode/PlayMode or equivalent behavioral suite, development APK install/run where possible, non-development ARM64 IL2CPP APK/AAB, target API current requirement, signing/version/16KB inspection. |

Playtest loops record initial condition → actual interactions → screenshot/evidence → observed friction/defect → fix → rerun. Inspect real screenshots; code review is not visual testing. Gather screenshots of home, group editing, concealed/revealed card, clue round, vote, White guess, result and resume in both languages. Check startup and rematch latency on available runtime and state the hardware; targets are <=3 seconds warm start and no avoidable navigation between result and next deal.

The baseline for final two-axis code review is the pre-implementation research/spec commit. Release gates include no known save loss, secret leak, dead-end state or mode-breaking defect. Automated simulations do not prove fun for real people; actual group feedback and physical-device/haptic coverage remain explicit limitations if unavailable.

## Out of Scope

Online/multiple phones, accounts/cloud sync, ads/IAP, word marketplace/import UI, more roles, scoring ladder, AI-generated live words, forced timers, iOS release and changes to Nischenreich. Publishing to a store or creating a paid developer account requires the owner at the actual publication step. Normal implementation, local tool setup, testing and repository work are authorized.

## Further Notes

Evidence references refer to `2026-10-06-expanded-player-feedback.md`, preserving dated individual reports rather than asserting widespread current bugs.

| Decisions | Basis |
|---|---|
| Immediate start, contextual help, complete result path | R01/R02: P06, P08, P12–P14, P26; explicit user emphasis. |
| Persistent group and safe recovery | Explicit user priority; R08, with P15/P24/P26 providing adjacent historic failure cases. |
| Card interaction and concealment | Explicit user gesture; R04: P07/P21 support privacy/input risks, not that exact gesture preference. |
| Random assignment and familiar word collection | R05/R06: P01/P03/P04/P17–P20/P23/P25; large DE/EN catalog explicitly requested. Exact counts are our acceptance targets. |
| Spoken semantic White judgment | R07/P28; no target-word display until game end is our privacy decision. |
| No forced timer or setup matrix | R03/R09 and the user's fast social flow. Exact player/role thresholds and tie policies are our rules decisions. |
| Classic role behavior | Reference: [official Undercover rules](https://www.yanstarstudio.com/undercover-how-to-play) and [FAQ](https://www.yanstarstudio.com/undercover-faq). Our counts, Quick mode and tie fallback are explicitly our own variant. |
| Technical release gates | Separate release/playtest research based on current official Android/Unity documentation and local tooling checks. |

The app's working name is Word Deduction. No competitor branding, artwork, text or word list is reused. Store identity and publisher contact must be settled before submission; they do not block building and testing the app.
