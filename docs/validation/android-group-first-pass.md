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
2. Actual app rendered, but every text glyph became a filled rectangle. Layout/background/button geometry rendered. `02-home.png` is the failing evidence. Unity/AndroidRuntime error-level logcat returned no lines; full startup log saved in `first-runtime-logcat.txt`.
3. Tap name field: Android keyboard opens after asynchronous IME startup. It covers the name field and + button completely. `04-after-language-tap.png` shows the obstruction. App remained responsive: tapping DE saved language 1 to app-private session.json.
4. Controlled input after keyboard focus: tap (410,1590), wait 2 seconds, `input text Alex`, then in a separate command `input keyevent 66`. Keyboard closes and Alex remains in the input; Players is still empty. `06-enter-focused.png` records the resulting screen. Enter/Done does not currently add the player. Earlier commands before focus or while language button had focus are not evidence of input loss.
5. Tap + at (946,1595), wait 2 seconds. `run-as ... cat files/word-deduction/session.json` confirms one Alex, active, stable ID `8b863f1307a943d9ba1d11a325d91f9f`, language German, Quick. Saved as `after-add-envelope.json`.
6. Back then `am force-stop`, followed by `am start -W`: Status ok, activity cold-start 1595 ms. Envelope read after restart is identical (`after-force-stop-envelope.json`). The first screenshot caught the loading surface; later `08-restored-home.png` shows the restored one-player row. Glyph failure remains, so names cannot be visually read.

## Result and next verification

- Passed in this limited scenario: ARM64 build installs/starts under the native bridge; real touch changes language; + adds a synthetic player; app-private write and force-stop durability work; one-player group is restored.
- Failed: readable Android text; unobstructed keyboard interaction. Enter confirmation is a friction point to improve.
- Not run yet: readable rename/pause/remove/undo flow, multiple players, long names, small viewport, APK update without data loss, language/mode after every action. Do these on the corrected APK, installed with `-r`; preserve current synthetic save.
- Screenshots and all data here are test-only. No real personal player data is involved.
