# Recovery and private handoffs — ticket 7

Scope: [issue 7](https://github.com/giarrel/word-deduction/issues/7), public Session actions/views with real temporary files and the actual rendered app. Worktree started from integration commit `6c12490`. No schema version was introduced by this ticket; Classic and catalog migrations remain owned by their tickets.

## Session evidence

The runner uses the production Session source and Unity's genuine Newtonsoft DLL (`com.unity.nuget.newtonsoft-json@74deb55db2a0/Runtime/Newtonsoft.Json.dll`), resolved from the Quick worktree until this worktree is imported. Evidence lives in [evidence/recovery](evidence/recovery).

| Slice | Actual failing behavior | Result |
| --- | --- | --- |
| `01-read-path` | `File.Exists` hid a generation path that was unreadable as a file; opening appeared to create an empty group. | Direct reads distinguish missing generations from read failures. `ReadFailed` blocks mutation/reset. |
| `02-interrupted-first` | An orphan pending first write looked like a clean first launch. | It requires explicit acknowledgement; fresh start archives the pending bytes before reusing the filename. |
| `03-newer-backup` | A healthy older primary hid a future-version backup that the next save would overwrite. | Either committed slot with a newer schema blocks opening and fresh start. |
| `04-quick-file-matrix` | Additional coverage of the existing transaction, green without a code change. | Handoff, next owner, clues, vote, selected suspect, runoff and result reopen unchanged. Partial and fully flushed pending writes are ignored while a committed generation exists. A real Windows sharing lock denies replace after flush; no result is acknowledged, and retry succeeds. A corrupt backup leaves a healthy primary readable; a missing primary visibly recovers backup. |
| `05-missing-fields` | A checksummed `{}` payload defaulted to an empty group. | The saved player collection is mandatory; invalid payload recovers the prior valid generation. |
| `06-denied-read-abandon` | Additional existing-behavior coverage. | A real read-sharing denial reports `ReadFailed`, without falling back to an older phase. Failed selection cancellation and abandonment retain the selected vote and group until retry succeeds. |

Current Session result before Classic/content integration: **31/31 passed** (`06-denied-read-abandon-green.log`). Existing cases also cover checksum/semantic corruption, both committed generations damaged, explicit archive-and-reset, write denial before flush, failed deal/handoff/result/rematch, stable identity, and new-schema primary rejection. Tests use only public Session actions/views; filesystem operations deliberately create storage faults or check byte preservation.

## Android policy

The existing APK manifest is saved as `evidence/recovery/android-manifest-before.xml`: predictive Back is enabled, but there was no explicit backup policy. `SessionPrivacy.androidlib` now declares `allowBackup=false` and excludes the entire `files/word-deduction` directory, including previous generations and damaged archives, from both legacy backup and modern cloud/device-transfer rules. The app keeps its existing internal storage location, preserving installed state.

Android documents that `allowBackup=false` alone does not prevent every manufacturer's device transfer, so explicit extraction rules accompany it. [Android Auto Backup](https://developer.android.com/identity/data/autobackup), [Unity Android library import](https://docs.unity3d.com/6000.3/Documentation/Manual/android-library-project-import.html).

## Runtime and remaining checks

The existing [Quick Android report](android-quick-runtime/report.md) demonstrates genuine touch/finger input, immediate concealment on a second contact, ordinary protected capture, process restart at an unconfirmed vote, and confirmed abandonment retaining the group. A native `sendevent` stylus route was rejected as unreliable; it is not acceptance evidence. The serialized `FLAG_SECURE` acknowledgement and its reveal guard remain in place.

The additional [Android interruption baseline](android-interruptions-baseline/report.md) demonstrates native Cancel, outside release, Home, sleep/wake, covered task previews and the native Back key with the same persisted state. Home-return and stable Recents screenshots were independently inspected in this ticket. It also records a **native red before the input fix**: hold finger 0, touch finger 1 elsewhere to conceal, lift only finger 1, then touch the hold area with finger 1 while finger 0 remains down. The old APK reopened the word, reproduced twice. The [reentry screenshot](android-interruptions-baseline/18-reentry-while-first-remains.png) was inspected here as well.

The fix retains the root contact set across card disposal and navigation. Additional contacts conceal immediately and cannot reopen or advance while another finger remains down. Window interruption clears the canceled stream after concealment. An ordinary Next press remains enabled until its own release. The matching rendered test was authored before implementation; the executed reentry red was the native Android sequence. Its first Editor run passed reentry but exposed a separate Next failure. Captured UITK events can bypass root callbacks, so the captured target also ends its shared contact on Up/Cancel. `07-pointer-first-editor-run.json` and `08-recovery-ui-check.json` retain these failures; `08-recovery-ui-green.json` shows all 12 tests passing after correction. `09-navigation-contact-check.json` adds a green test of held contact preservation across navigation, real touch Back/Resume, and lifecycle cancellation. The previous synthetic Quick lifecycle test now releases its second contact before beginning a separate gesture, matching a valid input stream.

`08-recovery-ui-red.json` also records a separate visible failure: a failed explicit fresh start kept showing only the old damage question. The action's localized save error now takes precedence. Both languages verify the blocked controls, preserved primary bytes and a successful explicit retry. The same green run confirms the existing Quick/group tests remain valid.

`10-recovery-screens-green.json` adds both-language rendered checks for backup restoration, unsupported versions and inaccessible paths (four Recovery tests green). Actual 360×640 game-view captures [damage question](evidence/recovery/11-damaged-de.png) and [save failure](evidence/recovery/12-save-failed-de.png) were visually inspected. The message and explicit retry remain readable. The existing empty-state hint overlaps the disabled name field on this short layout; ticket 8 should reflow the empty state when a recovery notice occupies vertical space and check the bottom start hint. This is a bounded Editor layout finding, not Android device evidence.

### Predictive Back

The native baseline's second consecutive edge swipe committed a real system Back gesture and finished the Activity to Launcher, although injected Back key events paused correctly. The integration report retains CoreBackPreview `triggerBack=true`, the animated window close and cold relaunch. Inspection of the shipped Unity player bytecode showed a Unity callback that synthesizes key events but the foreground window dumps had no registered callback. Unity's legacy `Input.backButtonLeavesApp` flag is not the input path used by this project's new Input System.

`MobileBack` registers the Android 13+ committed-Back callback at the default OS priority and queues the existing presenter Back action for the Unity thread. Canceled gestures never invoke it. This leaves IME dismissal at the normal system priority. The existing Escape/key path remains valid on older Android and in the Editor. The bridge unregisters on disposal; privacy acknowledgement is unchanged. References: [Android predictive Back](https://developer.android.com/guide/navigation/custom-back/predictive-back-gesture), [dispatcher priority contract](https://developer.android.com/reference/android/window/OnBackInvokedDispatcher), [Unity legacy Back flag](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Input-backButtonLeavesApp.html).

The Android-only code branch compiled against the installed Unity/Input System/Newtonsoft assemblies with no errors. This is a source compatibility check, not native acceptance. Actual committed/canceled gesture, Home/resume and IME interactions require the combined APK below.

## Combined Classic/content gate

Classic and bilingual content were merged from integration `20f6d3b`, retaining V1–V3 migration into V4. Both primary and backup envelope guards now reject versions above 4. Integration documentation through `7d6af6b` is included in build source commit `9ba4374`.

The integrated Session suite passed **46/46** (`13-v4-integrated-session.log`). An additional Classic matrix then passed **1/1** (`14-classic-phase-files.log`): initial/next handoff, clues, vote, selected suspect, runoff, repeated tie, elimination, continued round, pending White and both final judgments. Every checkpoint reopens without drawing or rewriting the exact primary bytes (including history/checksum), tolerates an interrupted pending write, and rejects an uncommitted abandonment. Real Windows sharing locks deny replacement after flush for round continuation and White judgment; neither advances until retry. The strengthened Quick byte-preservation check also passed (`16-quick-phase-bytes.log`). A premature second runner invocation briefly hit the first runner's executable lock; it was rerun only after the original finished. That harness failure is not counted as application red/green evidence.

The combined rendered suite passed **19/19** (`15-combined-playmode.json`): 5 group, 5 Quick, 4 Classic, 4 Recovery and the production-panel content fixture. Six long DE/EN terms were measured within the real card at 360×640 and erased on release; the latter case took 111 seconds of the 124-second run. Classic covered White judgment, covered resumption, elimination/survivors, results and rematch. No Editor test substitutes for a native OS callback.

Source inspection found no runtime logging calls in Session/UI and no secret-word tooltip/accessibility assignment. Covered card and private White explanation labels are synchronously emptied, including detached labels checked by PlayMode tests. TalkBack operation has not been proved.

## Combined Android artifact

Exactly one development APK build was dispatched from `9ba4374`. It completed successfully: **37,651,751 bytes**, SHA-256 `6284ef8a413638feb75182462c256103d3ee4a6bccede2a47d4a1abe830c0a92`, ARM64/IL2CPP, Unity 6000.3.25f1, SDK 26/36. The worktree artifact is `artifacts/android/WordDeduction-development.apk`; root received its exact path/hash for update testing.

The [build summary](evidence/recovery/17-build-summary.json) reports Succeeded with 1 error and 2 warnings. The [actual build-step diagnostics](evidence/recovery/17-build-diagnostics.json) identify the sole error as the Pipeline request's 5-second main-thread timeout; the Editor continued the same build, with no second dispatch. The warnings are disabled runtime Pipeline due to the intentionally absent runtime configuration, and the obsolete UITK `PreventDefault` API. No compiler or Gradle error is present in that build report. Repeated Editor-only SearchDatabase startup exceptions occurred outside the test runs; the complete PlayMode result remains 19/19.

Both the generated [merged manifest](evidence/recovery/17-merged-manifest.xml) and [APK's compiled manifest](evidence/recovery/17-apk-manifest.txt) were inspected. `allowBackup=false`, `enableOnBackInvokedCallback=true`, `fullBackupContent` and `dataExtractionRules` are present. `aapt` decoded the APK's actual [legacy rules](evidence/recovery/17-apk-backup-rules.txt) and [cloud/device-transfer rules](evidence/recovery/17-apk-extraction-rules.txt), each excluding the entire internal `file/word-deduction` directory. This development APK still declares INTERNET for development tooling; release packaging belongs to ticket 9.

Pending: root's native update, multi-contact, gesture, IME and lifecycle acceptance. Native predictive gesture navigation and TalkBack operation must not be claimed from source inspection or injected Back key events.
