# Group foundation: actual Android playtest

6 October 2026, root agent. Scope: ticket #3 only. This first pass is **Failed** for usable Android presentation; fixes requested from the implementer. No gameplay exists in this foundation.

## Build and environment

- APK: `group-foundation/artifacts/android/WordDeduction-development.apk` from the still-uncommitted ticket branch.
- SHA-256: `32079d14bec9dfd5bc121f593dcc6845ba457b0a508a020b2042d728c4f0bb35`; 37,500,646 bytes.
- Unity 6000.3.25f1, IL2CPP, ARM64, development; build report Succeeded, zero errors, one warning (implementer identifies absent RuntimePipelineConfig, disabling Pipeline in player).
- Package `com.giarrel.worddeduction`, activity `com.unity3d.player.UnityPlayerGameActivity`, version 0.1.0 / code 1, minSdk 26, targetSdk 36, primary ABI arm64-v8a.
- Emulator `emulator-5580`, Android 16 / API 36, 1080×2400 at 420 dpi. WHPX CPU acceleration, SwiftShader OpenGL ES 3.0, ARM64 native bridge on x86_64. This does not establish physical ARM64 compatibility.
- Installed with `adb -s emulator-5580 install -r <apk>`: Success. Development manifest reports INTERNET and ALLOW_BACKUP. Recheck release permission/backup policy in platform tickets.

## Observed interactions

1. `am start -W -n <activity>`: Status ok, Android activity cold-start time 1878 ms. This is **not** a measured time to a usable Unity screen. First-run Android fullscreen teaching overlay appeared; dismissed with its Got it button.
2. Actual app rendered, but every text glyph became a filled rectangle. Layout/background/button geometry rendered. [02-home.png](02-home.png) is the failing evidence. Unity/AndroidRuntime error-level logcat returned no lines; full startup log saved in [first-runtime-logcat.txt](first-runtime-logcat.txt).
3. Tap name field: Android keyboard opens after asynchronous IME startup. It covers the name field and + button completely. [04-after-language-tap.png](04-after-language-tap.png) shows the obstruction. App remained responsive: tapping DE saved language 1 to app-private session.json.
4. Controlled input after keyboard focus: tap (410,1590), wait 2 seconds, `input text Alex`, then in a separate command `input keyevent 66`. Keyboard closes and Alex remains in the input; Players is still empty. [06-enter-focused.png](06-enter-focused.png) records the resulting screen. Enter/Done does not currently add the player. Earlier commands before focus or while language button had focus are not evidence of input loss.
5. Tap + at (946,1595), wait 2 seconds. `run-as ... cat files/word-deduction/session.json` confirms one Alex, active, stable ID `8b863f1307a943d9ba1d11a325d91f9f`, language German, Quick. Saved as [after-add-envelope.json](after-add-envelope.json).
6. Back then `am force-stop`, followed by `am start -W`: Status ok, activity cold-start 1595 ms. Envelope read after restart is identical ([after-force-stop-envelope.json](after-force-stop-envelope.json)). The first screenshot caught the loading surface; later [08-restored-home.png](08-restored-home.png) shows the restored one-player row. Glyph failure remains, so names cannot be visually read.

## Result and next verification

- Passed in this limited scenario: ARM64 build installs/starts under the native bridge; real touch changes language; + adds a synthetic player; app-private write and force-stop durability work; one-player group is restored.
- Failed: readable Android text; unobstructed keyboard interaction. Enter confirmation is a friction point to improve.
- Not run yet: readable rename/pause/remove/undo flow, multiple players, long names, small viewport, APK update without data loss, language/mode after every action. Do these on the corrected APK, installed with `-r`; preserve current synthetic save.
- Screenshots and all data here are test-only. No real personal player data is involved.

## Iteration 2: isolate rendering, preserve updates

- Static-font-only build `49834bd14b8b760fac4a1cefa2df8eb073d9041d94f9007b8c60f0e98d5a1582` installed with `-r`: same rectangles on SwiftShader ([09-static-font.png](09-static-font.png)). Alex, ID and German language survived the update.
- Closed only this AVD using `emu kill`, waited for its launcher to exit, and restarted the same AVD/data with `-gpu host`; NVIDIA RTX 3080 Ti, translated GLES 3.1 confirmed in Unity log. Launcher PID180604 is recorded in [emulator-host-launch.json](emulator-host-launch.json) (ephemeral, inspect before reuse).
- First app start during remaining Android boot activity was destroyed; a retry after boot settled rendered readable text ([12-host-gpu-retry.png](12-host-gpu-retry.png)). No cause beyond observed lifecycle is asserted for the first destruction.
- Reinstalled the original dynamic-font baseline `32079…bb35` with `-r`, same hardware-GPU environment: equally readable ([13-host-gpu-dynamic-baseline.png](13-host-gpu-dynamic-baseline.png)), same durable save. Thus the static font experiment was unnecessary and the original dynamic Inter font can remain. This isolates the emulator graphics path as an influence; it does not establish all physical GPU compatibility.
- On the original baseline, paused Alex, tapped its name, saw native selection of the complete name ([14-edit-paused.png](14-edit-paused.png)), typed Alexandra and tapped Save. Real stored name changed, ID and paused state did not. The keyboard incorrectly remained visible after the edit ended ([15-renamed-paused.png](15-renamed-paused.png)); correction requested.

## Iteration 3: keyboard visibility and confirmation semantics

- Build SHA-256 `327e253d31befe8116901cc2fa3c5ea66c2113f327e5793157c9c3a02b6f26bf` installed with `-r` on host-GPU AVD. Original dynamic font restored; developer added Android IME insets and a compact entry layout. Saved Alexandra/paused/ID survived.
- Field and + remain clearly visible above keyboard: **Passed**, [16-keyboard-insets.png](16-keyboard-insets.png).
- Bea + native Enter66 added one person: [17-ime-done-added.png](17-ime-done-added.png). Correct German singular count rendered. This initially looked successful, but the cancel case below means the native completion behavior is **not accepted**.
- Consecutive Chris and second Bea using the visible +: both added, no duplicate commits. Bea · 2 and Bea · 4 visibly distinguish the names; stable IDs and stored names remain separate ([18-multiple-and-duplicate.png](18-multiple-and-duplicate.png)).
- Added Dana using + while keyboard remained open. No error before closing ([19-plus-before-back.png](19-plus-before-back.png)). New fifth row was partly clipped rather than automatically scrolled into view; requested post-layout scroll correction.
- Closing the now-empty keyboard using only Back4 produced InvalidName ([20-empty-back-error.png](20-empty-back-error.png)). Isolated a second case: typed Unconfirmed and used only Back4; the app actually saved Unconfirmed ([after-back-with-draft.json](after-back-with-draft.json)). Thus this runtime's observed native Done status cannot be used alone as explicit confirmation. Requested removal of ambiguous auto-commit if explicit IME Done cannot be robustly distinguished; the visible + already provides fast repeat input without closing the keyboard. Back must not silently add or rename.
- Current synthetic group: Alexandra paused; Bea, Chris, second Bea, Dana, Unconfirmed active. Language EN, Quick. Further update/restart checks must preserve this state; the deliberately erroneous Unconfirmed can be removed through the UI and undone as the next test.

## Removal, undo and settings before final correction

Still on build327e: switched to German and Classic, opened Bea · 2's edit row, then removed it ([21-remove-edit.png](21-remove-edit.png), [22-removed.png](22-removed.png), [after-remove.json](after-remove.json)). This was Bea · 2, not the initially intended Alexandra, because the list had a retained scroll offset; the screenshot identifies the actual target. Remaining Bea retained its displayed suffix4. The first Bea's ID, participation and original index1 were durably stored in Removed.

Force-stopped and relaunched. German, Classic, five remaining people and the explicit Undo action all survived ([23-undo-after-restart.png](23-undo-after-restart.png)). Tapped Undo, then reactivated Alexandra. Saved JSON in [after-undo-and-rejoin.json](after-undo-and-rejoin.json) verifies six active people, original order and IDs, same original Bea ID at index1, both duplicate-name disambiguators, and Removed=null. Core remove/undo/rejoin/settings persistence passes this scenario; final keyboard-cancel correction remains pending.

## Iteration 4: explicit confirmation passes, native keyboard close still fails

- Clean build `74eca5d6272ac62cf82cf7744fe936b6d3c039e28e5dcb3cbca7bcbdc4e4235c`, 37,517,238 bytes, installed with `-r`; prior six people, IDs, German and Classic survived ([24-corrected-home.png](24-corrected-home.png)). Clean packaging removed the previous incremental ZIP padding.
- Controlled focus then typed Draft and pressed Back only: draft stayed visible, count stayed six, no InvalidName and no automatic addition ([27-back-draft-settled.png](27-back-draft-settled.png)). An earlier immediate scripted input caught focus transition and is not treated as input correctness evidence.
- Refocused the selected draft, replaced with Emil, tapped visible +: seventh person added and fully scrolled into view while keyboard stayed open for more entries ([29-added-visible.png](29-added-visible.png)). Empty Back then closed keyboard without validation error ([30-empty-back.png](30-empty-back.png)). These corrections pass.
- Edited Unconfirmed (stable ID7b3b955adc2f42069f2b4e4a66ea2584) to Felix. With the edit row near the bottom, its action buttons were clipped by the scroll viewport ([31-rename-focused.png](31-rename-focused.png)); swiping the list exposed them and closed IME ([32-edit-actions.png](32-edit-actions.png)). Save was then tapped. Requested automatic edit-row reveal after keyboard/layout changes.
- Opened Alexandra's top row with keyboard visibly active ([33-top-edit-keyboard.png](33-top-edit-keyboard.png)). An attempted first Save tap failed because the ADB daemon temporarily restarted and reported device not found; screenshot34 does not prove Save behavior and is discarded as a test result.
- Typed CancelMe into the edit and tapped Abbrechen. Name returned to Alexandra, but keyboard stayed visible in both immediate and later settled screenshots35/36. Cancel discarded the draft correctly; native keyboard close failed.
- Reproduced Save independently: Back closed the keyboard, tapped Alexandra, waited2s for keyboard, tapped Speichern at actual screen coordinate830,730, waited2s. Edit row closed, but IME remained visible ([37-save-retest.png](37-save-retest.png)). Source-level Blur/keyboard.active=false is insufficient in this runtime. Requested correction without restoring ambiguous Done auto-commit.
- Current group is seven active people, DE/Classic: Alexandra, Bea·2, Chris, Bea·4, Dana, Felix, Emil. No gameplay claims. Next APK retest only needs the native edit-closing/visibility issues plus state preservation.

## Iteration 5: focused Android retest passed

- Clean development APK SHA256 `181bfebbebca3c35198e1da29ebceb0d5c52aa33e2668f499a4fe4f730bd1cf2`, 37,513,698 bytes. Build summary Succeeded, zero errors, one expected Pipeline warning. Updated with install-r; all seven people, DE/Classic and stored identities retained.
- Opened Alexandra's edit with visible Gboard ([38-focused-final.png](38-focused-final.png)); Save closed both edit and native keyboard ([39-save-final.png](39-save-final.png)).
- Reopened, typed Discard, tapped Abbrechen; original Alexandra retained and keyboard closed ([40-cancel-final.png](40-cancel-final.png)).
- Reopened, tapped Entfernen; count became six and keyboard closed, Undo visible ([41-remove-final.png](41-remove-final.png)). Tapped Undo to restore Alexandra.
- Scrolled to the last player Emil and opened editing; entire field plus Remove/Cancel/Save automatically stayed above keyboard, while the separate add-new-name field was hidden ([42-bottom-edit-final.png](42-bottom-edit-final.png)). No manual corrective scroll needed after opening edit. This resolves the remaining actual obstruction.
- Canceled edit, force-stopped and relaunched. [final-foundation-state.json](final-foundation-state.json) records final durable seven-player group, DE/Classic, original IDs and no pending removal. This is foundation acceptance, not full-app or gameplay acceptance. Physical ARM64, Unicode visual coverage, small-device dp measurements, gesture navigation, TalkBack, match lifecycle and release packaging still belong to later tickets.
