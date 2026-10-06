# Quick mode: actual Android acceptance

6 October 2026. Tester: root. Development APK **32cfb05e08d82686ae5b643ed8d6ba86e79bc641b3a074c44cdf9cc73b09cfce**, 37,552,110 bytes, ARM64 IL2CPP, Unity6000.3.25f1. Source5de9b4d; final implementer branch74e7825 contains the same application source plus integration/docs. Installed with `adb install -r`; no uninstall or data reset.

Runtime: emulator-5580, API36 Google APIs x86_64 with ARM64 native bridge, host NVIDIA GPU,1080×2400/density420. These are actual Android interactions, not physical-phone or real-group fun evidence. Private words and names in the evidence are synthetic test data.

## Upgrade and durable identity

The foundation V1 file had SHA256 `a14b4711874db72804831c25add98f1df1fef233e370b28c8d1135a1ca379200` before update and the identical on-device hash after opening the new app. Its complete payload was identical: eight active people, German, Classic, original IDs, duplicates, Zoë and NextNumber9. Opening does not rewrite/migrate the save.

Choosing Quick was the first intentional durable command; subsequent first deal is V2. All eight player records compare byte-for-byte after JSON normalization with the pre-update records. Evidence: `before-update.json`, `after-update-open.json`, `after-mode-change.json`, `first-deal.json`; [updated group](01-updated-group.png).

## Secret-card interactions and test-method correction

- Fresh match starts at Alexandra, card1of8, no visible word and disabled Next. A plain tap on the card followed by Next did not advance. [Covered card](02-covered-de.png).
- A held upward drag displayed **Birne**, with no Civilian/Undercover label; Next stayed disabled. Release concealed it. The alternative hold control also enabled valid handoffs for the remaining people. Names and duplicate suffixes remained clear; the last owner was [Zoë](07-last-owner.png).
- A second simultaneous **finger** on the disabled Next concealed the word. Releasing both contacts and moving the old pointer did not advance or reopen it. Saved Handoff remained0; a later deliberate Next advanced exactly to1. [Open](03-clean-drag.png), [second finger closes](04-clean-second-finger.png), `clean-two-finger-input.txt`, `after-clean-two-finger.json`.
- Ordinary Android `screencap` during an actually open card was completely black, hash `c35bacdb98b522206335afa5b9baffd2e4e3352a40749bb747e469cd403af514`. [Secure capture](05-android-secure-revealed.png). Separately, the emulator host framebuffer captures displayed pixels for visual inspection; this is not an Android task-preview leak.

Input-method correction matters: separate `input motionevent` calls did not establish a usable continuous drag in this test. Raw `/dev/input/event2` contacts reported FINGER at kernel level but dispatched `source=TOUCHSCREEN|STYLUS`; Unity logged unhandled pen pointer actions5/6 and ignored the second contact. This is not accepted as an app touch regression. Both raw contacts were released, temporary `pointer_location` restored to its originally unset state, and `adb unroot` restored normal shell operation.

The clean two-finger test used the external **shell-only** `WordDeductionInput.java` helper, compiled with the existing Android SDK and run from `/data/local/tmp` as normal shell. It injects MotionEvents with explicit `SOURCE_TOUCHSCREEN`, `TOOL_TYPE_FINGER` and consistent gesture downTime. `dumpsys input` confirms the actual two-contact POINTER_DOWN dispatched to Android. No test hook or helper was included in the APK. Helper source is included solely for repeatable validation.

## Complete playthroughs

1. **German / correct accusation.** All eight handoffs completed in order. The clue screen named Felix, matching the persisted random starter; no per-clue taps. [Clues](08-clues-de.png), [vote](09-vote-de.png). Proposed Bea·2 without confirming, force-stopped and relaunched: complete payload unchanged and Outcome null. After covered resume the same proposal appeared. Android Back4 canceled that proposal and persisted Suspect=null. Selected Zoë and confirmed: Civilians won, reason matched, Birne/Apfel revealed, full eight-role list reachable by scrolling. [Restored proposal](10-restored-proposal.png), [corrected selection](11-corrected-selection.png), [result](12-civilian-result.png), [all roles](13-all-roles.png). `vote-proposal-before-restart.json`, `vote-proposal-after-restart.json`, `first-result.json`.
2. **German / wrong accusation.** One Next match action opened a new deal with all eight original people. Completed all handoffs, accused known Civilian Alexandra and confirmed: Undercover won with the correct reason and Fußball/Basketball. [Result](14-undercover-result.png); `second-deal.json`, `second-result.json`.
3. **English / repeated tie.** Edit group returned directly to the retained group; EN selected without re-entering anybody. Completed all eight cards, clue round, first tie and runoff. Second tie ended with Undercover escaping; both English words Hill/Mountain and roles displayed. [English handoff](15-covered-en.png), [runoff](16-runoff-en.png), [result](17-tie-result-en.png); `third-deal-en.json`, `third-result-en.json`.
4. **Confirmed abandonment.** Started the next deal; Back4 paused. Abandon first displayed a confirmation and retained the match. Keep this match retained it. Only a subsequent explicit confirmation ended the deal. [Confirmation](18-abandon-confirmation.png), [group afterward](19-preserved-group.png); `abandon-proposed.json`, `after-confirmed-abandon.json`. Final eight player objects still compare exactly with the pre-update group; languageEN and modeQuick are the intentionally changed preferences, Match=null, NextNumber9.

Result states: first match18df50a30ad04b6ca46bfdac2c2e49e1 Outcome0; second530f8b0789e742d9b5258ed0a8b53a32 Outcome1; third193ea17868b348c3b04276d4128d1a88 Outcome2. All phase3/results and eight saved group members.

## Acceptance and remaining work

**Ticket4 Android scenarios above passed.** No new production correction was required after the implementer's UI tests. This complements21/21 Session and10/10 rendered PlayMode tests; it does not replace later full-app release validation.

Ticket7 still owns broader real Android cancel/capture-loss/outside-release, Home/task-preview, gesture-navigation and storage-fault checks across all final Classic phases. Task-preview privacy is not claimed from the screenshot test alone. TalkBack, physical ARM64/haptics and human group enjoyment remain unverified. Foundation non-Latin font failures and small-target/scrollbar polish remain recorded for Ticket8.

One cold restart was still displaying the Unity splash when an automated Resume attempt arrived three seconds after Android activity startup. That early tap is not counted as a UI failure or successful resume. A later fully loaded resume passed. Activity launch times around1.4–1.6s are not time-to-usable-screen measurements; assess final startup and remove avoidable loading friction in the later polish/release checks.
