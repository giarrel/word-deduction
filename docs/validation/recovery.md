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

Current Session result before Classic/content integration: **29/29 passed** (`05-missing-fields-green.log`). Existing cases also cover checksum/semantic corruption, both committed generations damaged, explicit archive-and-reset, write denial before flush, failed deal/handoff/result/rematch, stable identity, and new-schema primary rejection. Tests use only public Session actions/views; filesystem operations deliberately create storage faults or check byte preservation.

## Android policy

The existing APK manifest is saved as `evidence/recovery/android-manifest-before.xml`: predictive Back is enabled, but there was no explicit backup policy. `SessionPrivacy.androidlib` now declares `allowBackup=false` and excludes the entire `files/word-deduction` directory, including previous generations and damaged archives, from both legacy backup and modern cloud/device-transfer rules. The app keeps its existing internal storage location, preserving installed state.

Android documents that `allowBackup=false` alone does not prevent every manufacturer's device transfer, so explicit extraction rules accompany it. [Android Auto Backup](https://developer.android.com/identity/data/autobackup), [Unity Android library import](https://docs.unity3d.com/6000.3/Documentation/Manual/android-library-project-import.html).

## Runtime and remaining checks

The existing [Quick Android report](android-quick-runtime/report.md) demonstrates genuine touch/finger input, immediate concealment on a second contact, ordinary protected capture, process restart at an unconfirmed vote, and confirmed abandonment retaining the group. A native `sendevent` stylus route was rejected as unreliable; it is not acceptance evidence. The serialized `FLAG_SECURE` acknowledgement and its reveal guard remain in place.

Pending in this worktree: rendered recovery/multi-contact tests under exclusive Editor ownership, actual rebuilt APK/merged-manifest inspection, and recovery coverage after Classic/White and persistent catalog-history integration. Native predictive gesture navigation and TalkBack operation must not be claimed from source inspection or injected Back key events.
