# Classic mode — issue 5 validation

Date: 2026-10-06. Worktree `ticket/5-classic-mode` began at integration commit `6c12490`. The canonical issue, its empty comment list, the specification, glossary, ADR and previous Quick acceptance were read before implementation. Session and rendered-app seams were already authorized in `docs/development/autonomous-workflow.md`.

## Delivered behavior

Classic supports every group size 4–20, with 1/2/3 Undercover for 4–7/8–12/13–20 people. White is off initially, separately remembered, effective only for Classic with at least 5 people. Dropping to 4 keeps the preference visible but unavailable. Quick ignores it. Random assignment draws without replacement; each clue round's starter is drawn from every survivor, including White. The deal freezes participant identities, labels, roles, words and language.

Confirmed eliminations show only the eliminated role. The same person cannot be selected again. Continuing starts a new clue round with survivors; a first tie allows a spoken runoff, and another tie starts a clue round without elimination. White has a private bluffing explanation and one spoken, semantically judged guess after elimination. The pending guess is durable and takes precedence over the last-adversary Civilian win. Correct White wins alone; incorrect removes White and evaluates the remaining match. At one remaining Civilian, results name Undercover, White, or both according to surviving roles. All active saved group members return in a rematch.

The snapshot schema is V3, importing V1 groups and V2 Quick deals without rewriting on open. New fields default to round 1 and nobody eliminated. Numeric phase values 0–3 are unchanged, with Elimination 4 and WhiteGuess 5 appended. Validation rejects impossible phases, eliminated suspects/starters, invalid rounds, terminal states presented as live, and Classic elimination fields in Quick. Newer primary versions still block fallback/overwrite. Failed elimination or White-judgment saves expose no uncommitted role/result and retain the retryable prior phase. Existing atomic file handling is unchanged.

## Executed tests and red/green sequence

The production Session source compiled against Unity's genuine Newtonsoft assembly: **29/29 scenarios passed**. Initially the DLL came from the already imported Quick checkout using the documented `NewtonsoftJsonAssembly` override; no package cache was fabricated. [Full result](evidence/classic/session-tests.txt).

Public Session coverage includes every supported Classic count with White off/on, unaffected Quick roles, frozen preferences, survivor-only eliminations and starters, every winning-role set, White correct/incorrect including last adversary, pending-guess restart and target-word privacy, stale confirmation/guess/round actions, repeated ties, save obstruction and retry, valid-checksum semantic corruption, legacy V1 group and V2 deal import, and rematch inclusion. Tests use real temporary directories and deliberate reopen operations.

Observed vertical slices:

| Slice | Observed red | Passing behavior |
|---|---|---|
| Saved Classic mix | `WhitePreferred`, effective counts and preference command absent | Classic all 4–20 mixes and retained preference, Quick unchanged |
| Elimination rounds | Survivor/round/elimination interface absent | Survivor-only next round and Civilian result after final Undercover |
| White and winning sides | White guess interface and outcomes absent | Pending White precedes terminal evaluation; exact winner set |
| Runoff | `second tie continues Classic` failed | New clue round, same survivors, random starter including White |
| Snapshot validation | `invalid eliminated suspect recovers prior valid state` failed | Invalid Classic progress recovers validated previous generation |
| Rendered start | Real Play button remained disabled after selecting Classic | DE/EN start and saved White preference; [red](evidence/classic/01-ui-start-red.json), [green](evidence/classic/01-ui-start-green.json) |
| Private White card | Card showed only `Mr. White`, without the bluffing explanation | Localized private explanation erased synchronously; [red](evidence/classic/02-white-card-red.json) |
| Rendered guess | Eliminated White had no judgment screen | Actual DE/EN flow, covered resume, wrong judgment, Civilian result, direct rematch; [red](evidence/classic/03-white-judgment-red.json), [green](evidence/classic/03-white-judgment-green.json) |

Unity 6000.3.25f1 / Pipeline 0.8.0 PlayMode full regression: **14/14 passed** (five group, five Quick, four Classic). [Results](evidence/classic/playmode-tests.json). Classic uses actual UI Toolkit submit and pointer events, real persistence, and a 20-person short-screen vote. It checks private text erasure, White guess resume, survivor-only buttons, role-only elimination, Back after confirmed elimination and pending White, two-stage tie, Civilian/White/both-adversary results, and rematch. The additional 4↔5 preference assertions were rerun after the full suite.

## Visual walkthrough

`ClassicEvidence` uses new temporary sessions, the real App scene and its production panel/font, with actual rendered controls. Game View resolution is set through `PlayModeWindow.SetCustomRenderingResolution`; `capture_game_view --source screen` captures composed UI. A first capture path containing `..` was rejected by the CLI; subsequent captures used its allowed temporary asset path and were copied into this evidence folder. No failed capture is represented as a screenshot.

All 16 screenshots were opened and visually inspected. German 390×844 covers [group and role mix](images/classic/01-group-de.png), [covered handoff](images/classic/02-covered-de.png), [private White card](images/classic/03-white-private-de.png), [clues](images/classic/04-clues-de.png), [spoken guess](images/classic/05-white-guess-de.png), [covered pause](images/classic/06-pause-de.png), and [White-alone result](images/classic/07-white-result-de.png).

English 360×640 covers [eight-person group](images/classic/08-group-en-small.png), [clues](images/classic/09-clues-en-small.png), [role-only elimination](images/classic/10-elimination-en-small.png), [runoff](images/classic/11-runoff-en-small.png), [White judgment](images/classic/12-white-guess-en-small.png), [Civilian result](images/classic/13-civilian-result-en-small.png), [scrolling help](images/classic/14-help-en-small.png), [private White card](images/classic/15-white-private-en-small.png) and [both surviving adversary roles](images/classic/16-adversary-result-en-small.png). Text and actions fit. The second tie continued round 3 without elimination. A wrong White guess continued play; eliminating the remaining Undercover ended with Civilians. A separate 5-person case ended with both adversary roles, then one Next match action restored all 5 participants.

Known wide scrollbars and general font/touch polish remain issue 8. These screenshots establish rendered readability in the stated configurations, not physical Android touch behavior or human group enjoyment.

## Android handoff

The development APK uses the existing AppBuild entry and original application identity. Android runtime acceptance belongs to root, which owns ADB/emulator operations. This ticket does not claim native lifecycle, predictive Back or multi-contact regression fixes assigned to issue 7. Build identity and result will be appended after the completed build.

The initial sandbox Editor launch had no usable license channel. Only the verified Classic-owned stalled Editor processes were terminated; relaunching in normal user context recovered the editor and Pipeline. No Unity installation or other project was modified.
