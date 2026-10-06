# Quick mode — issue 4 validation

Date: 2026-10-06. Worktree: `ticket/4-quick-mode`, created from integration commit `ac0dcca`. The existing group and native keyboard behavior remain part of the regression suite. This ticket adds the complete Quick match; Classic deliberately remains unavailable until issue 5.

## Delivered behavior

The public Session actions own Quick deals for 3–20 active people, one Undercover, independent random starting person, the handoff, one clue round, spoken-vote selection and confirmation, one runoff, results, rematch and explicit abandonment. The UI never computes the winning side. Result exposes both words, the winning side/reason and every role; private cards expose only a word. The small original starter file contains 20 bilingual pairs with IDs matching the prepared editorial catalog. The 500+ pair catalog and persistent draw cycle remain issue 6.

The snapshot envelope is version 2. Version 1 group state imports without changing names, IDs, participation, preferences or undo; opening does not rewrite it. Earlier version-1 builds reject version 2. A committed match stores concrete words, the pair ID, frozen display names/language, assignments and progress. Opening never redraws. Validation rejects invalid phases, roles, counts, owner indices, suspect references and inconsistent outcomes, even with a valid checksum. Existing primary/previous-generation recovery is retained.

Reveal state is volatile. A handoff needs a successful reveal and concealment; an expected owner ID prevents duplicated Next from skipping a participant. Start, handoff, vote, confirmation and rematch publish their changed views only after the atomic file write succeeds. A failed result write exposes no result. Rematch applies the same start preconditions even if a caller changed the group or mode while on the result.

The secret card keeps a fixed input surface and translates/tilts only its visual child. Drag needs a deliberate upward threshold; the alternative area reveals while held. Release, cancel, capture loss, a second finger anywhere in the safe-area root, focus loss, application pause, Back/help, disable/detach and owner changes clear the actual secret label synchronously. Decorative settling happens after erasure. Next additionally waits for a closed card and no active pointer. Restart and re-enable show a covered pause surface requiring Resume or confirmed Abandon.

Android uses `FLAG_SECURE` throughout a live match. The UI does not permit reveal until the latest UI-thread flag request acknowledges completion. Every live render reapplies protection, including resume; stale flag acknowledgements cannot authorize a newer request. The flag is cleared for a completed result/group. System-disabled Android animations skip settling/tilt. Predictive Back is enabled through Unity's supported PlayerSettings setting. These implementations require the separate Android runtime checks below; Editor tests cannot prove platform privacy.

## Executed tests

`dotnet run --project tests/Session.Tests`: **21/21 passed**, compiling production Session sources against the worktree's genuinely imported Unity Newtonsoft DLL. Early iterations used the already imported Foundation DLL through the documented `NewtonsoftJsonAssembly` override while this worktree imported normally. No fabricated package cache was used.

The public action/view seam uses real temporary directories and reopens saved data. Added coverage includes every Quick group size, assignment to each seat, independent starting selection, safe views, persisted deal/reveal after reopen, correct/wrong suspect, editable unconfirmed selection, stale confirmation/Next/rematch/abandon, both tie stages, frozen live participants/preferences, failed start/handoff/confirmation/rematch with retry, legacy migration, semantic corruption recovery and shared rematch preconditions.

Unity 6000.3.25f1 PlayMode, through Pipeline 0.8.0: **10/10 passed** (five foundation plus five Quick scenarios). Tests dispatch actual UI Toolkit touch/submit events into the rendered document. They verify immediate text erasure before another frame, not just an eventual blank screen. They cover tap vs. drag, hold and outside release, cancel, capture loss, second pointer, stale pointer events, Next while open, duplicate Next, focus/pause/disable, explicit resume, full DE/EN play and abandonment, real write obstruction/retry, and a 20-person vote with the last row scrolled into view on a short surface. Native Android input is a separate test seam and is not claimed from synthetic events.

Representative red/green evidence in [evidence/quick](evidence/quick/):

| Slice | Observed red | Passing outcome |
|---|---|---|
| 01 deal | Public deal/reveal/advance actions absent | Durable private deal at all 3–20 counts |
| 02 vote | Public vote/confirmation/rematch actions absent | Correction, confirmation and direct rematch |
| 03 runoff | Tie/abandon actions absent | One runoff; second tie wins for Undercover; live settings frozen |
| 04 validation | Checksum-valid role 99 accepted | Previous validated clue-phase snapshot recovered |
| 05 rendered start | Play did not create a named card | Real Play button reaches the covered card |
| 06 rematch | Paused group below minimum still created a new deal | Shared start preconditions preserve prior result |
| 08 second pointer | Touch on safe-area background left `Violin` visible | Same synchronous concealment across the full app area |

An early test harness command used an incorrect external DLL hash and failed before compiling the product; it is not a TDD red. Touch factory overloads were corrected to the installed public signature. The close-animation test originally waited a fixed frame count, which was insufficient at the Editor's high frame rate; it now waits 0.25 seconds of real elapsed time. No product change is attributed to that timing correction.

## Visual walkthrough

The real App scene, its production panel/font and actual rendered controls were used. `QuickEvidence` is an Editor-only reproducible fixture helper; it changes only a new temporary Session, not Android user data. `PlayModeWindow.SetCustomRenderingResolution` sets the actual Game View, and `capture_game_view --source screen` captures its composed output. A first `screenshot` command captured only the background camera, so that blank capture was rejected and replaced; it was not an application layout defect.

All images were opened and visually inspected. [German covered card](images/quick/01-covered-de.png), [revealed card](images/quick/02-revealed-de.png), [clues](images/quick/03-clues-de.png), [vote](images/quick/04-vote-de.png), [unconfirmed suspect](images/quick/05-confirm-de.png), [result](images/quick/06-result-de.png) and [covered pause](images/quick/07-pause-de.png) use 390×844. The suspect was corrected before confirmation and the result led directly to another match.

The English 360×640 walkthrough includes [24-character owner](images/quick/08-covered-en-small.png), [held reveal](images/quick/09-revealed-en-small.png), [clues](images/quick/10-clues-en-small.png), [vote](images/quick/11-vote-en-small.png), [confirmation](images/quick/12-confirm-en-small.png), [runoff](images/quick/13-runoff-en-small.png), [repeated-tie result](images/quick/14-result-en-small.png), [pause](images/quick/15-pause-en-small.png) and [help](images/quick/16-help-en-small.png). Content fits; result roles remain scrollable with primary actions fixed below. The native-looking result scrollbar seen in the first small result capture was restyled to match the app and rechecked.

## Platform handoff and limits

The Android development APK is built using the existing AppBuild entry, ARM64/IL2CPP, min API 26, target API 36, CleanBuildCache and the original application ID. No signing, Unity installation, global CLI configuration or other project was changed. Build summary, hash and actual size are recorded after completion in the adjacent evidence.

Root owns ADB/emulator-5580 and will test an update over the existing version-1 German/Classic group (eight active people, including Zoë), actual drag/hold/release/multitouch, Home/task preview/restart, system Back and secure screenshots. This implementation task does not claim those checks before they happen. Editor screenshots intentionally show fixture secrets; Android screen-capture protection is assessed separately. Physical-device usability, full TalkBack operation, large-catalog freshness, full Unicode font coverage and overall release readiness remain later release work. The known platform font gaps are tracked under issue 8, not concealed by Latin-only fixture success.
