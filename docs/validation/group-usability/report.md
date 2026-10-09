# Group usability — ticket #19

Implemented from [Spec #17](https://github.com/giarrel/word-deduction/issues/17) and [ticket #19](https://github.com/giarrel/word-deduction/issues/19), based on integration planning commit `af0cba1f605877dfd3ee672f869704a74a9d9174`. Final implementation source: `99b0bf04c236fc8f6c1467cf699d760de29c6acc` on `implement/group-usability`. The integration tip was merged before handoff (already up to date).

## Delivered behavior

Names and avatars are ordinary scrollable content. Each row has a localized Edit button; normal Pause/Join actions are gone. Editing retains rename, explicit Cancel/Save, Remove and durable Undo. At20 current players the unavailable Add field is hidden and an explanation remains visible. It is impossible for a new Add to silently create an inactive player.

A distinct handle previews an order while dragging and scrolls at the list edge. Only a valid drop commits. Cancel, capture loss, outside drop, focus interruption and a redraw cancel the preview. Editing also provides Move up/down; draft names are retained while moving. Both routes send a complete stable-ID order to the existing Session transaction. Stale/invalid orders and denied writes preserve the committed order; the next match follows that order, and an existing live match remains frozen.

Earlier inactive records are kept in a collapsed compatibility section with an explanation. They may be explicitly restored (subject to20-player capacity), renamed or removed. Opening old saves does not activate or discard them; the old40-record envelope stays valid. No schema or role-rule change was made.

At150% text, each ordinary row gives Edit its own line and long names sufficient width. The native accessibility hierarchy exposes names and Edit/move actions; the decorative drag mark is omitted. The existing privacy pause remains intact.

## Evidence and TDD

The agreed seams remain Session with real temporary persistence and actual rendered UI input. No private helper test seam was added.

| Scenario | Original failure | Passing check |
| --- | --- | --- |
| Normal Add must not create a paused overflow person | [02-capacity-red](evidence/02-capacity-red.txt),0/1 | [03-capacity-green](evidence/03-capacity-green.txt),1/1 |
| Durable atomic ID-based order | [06-reorder-red](evidence/06-reorder-red.txt),0/1 | [07-reorder-green](evidence/07-reorder-green.txt),1/1 |
| Short8px name drag must not edit | [10-name-drag-red](evidence/10-name-drag-red-result.json),0/1 | [12-name-drag-green](evidence/12-name-drag-green-result.json),1/1 |
| Old inactive people start collapsed | [15-legacy-red](evidence/15-legacy-red-result.json),0/1 | [17-legacy-green](evidence/17-legacy-green-result.json),1/1 |
| Handle drop plus non-drag move actions | [19-handle-red](evidence/19-handle-red-result.json),0/1 | [21-handle-green](evidence/21-handle-green-result.json),1/1 |

The legacy40-record persistence case passed against the existing compatibility behavior without a product change: [09-legacy-existing-green](evidence/09-legacy-existing-green.txt). Capacity and reorder tests cover stable Unicode duplicates, reopen/mode change, invalid/stale requests, real denied writes, next handoff and live-deal preservation. Existing remove/Undo and rename regressions remain.

[32-group-final-result](evidence/32-group-final-result.json) records6/6 focused rendered scenarios, including actual name scrolling, handle drop/cancel/outside release, focus loss, failed-save feedback, first/last moves and edge autoscroll. The full rendered run [28](evidence/28-rendered-regression-result.json) was51/53. It exposed maximum-capacity large-text overflow, which was corrected. The later full run [37](evidence/37-rendered-final-result.json) was50/53: a zero-padding flex-wrap correction caused two large-edit failures, and a previous accessibility assertion still expected Add at full capacity.

All three remaining rendered failures were resolved with focused retests: [39-large-final](evidence/39-large-final-result.json)9/9, [41-stencil-capture](evidence/41-stencil-capture-result.json)1/1, and [43-accessibility](evidence/43-accessibility-result.json)1/1. These include the old large-text regressions and late native-accessibility invocation on5/20-player groups. **There was no final fresh53/53 full run at the final source pin.** Integration should retain the exact full/targeted distinction.

## Visual inspection

The implementer inspected the six final DE/EN360×640 group and360×380 simulated keyboard-view captures. The coordinator independently inspected both final edit captures. Names, explicit Edit, capacity text, start actions and move/remove/cancel/save labels are readable in these views.

- [German group](evidence/screenshots/group-German.png), [English group](evidence/screenshots/group-English.png)
- [German large last row](evidence/screenshots/group-large-last-German.png), [English large last row](evidence/screenshots/group-large-last-English.png)
- [German enlarged editing](evidence/screenshots/editing-large-German.png), [English enlarged editing](evidence/screenshots/editing-large-English.png)

Original cramped long-name captures are retained under `evidence/screenshots-before-layout/`. The visual fixture itself needed three corrections: avoid applying150% twice to retained elements; apply simulated keyboard state after viewport geometry settles; resize the capture target to380px and give it24-bit depth/stencil. A640px target retained old pixels below the shorter root; a zero-depth target incorrectly rendered nested rounded TextField clipping as a white overlay. Changing only fixture depth restored selected text. These were not Android UI fixes or proof of native keyboard behavior. An interim zero-padding Cancel rule caused a real flex-wrap regression; positive4px padding fixes it and keeps Abbrechen on one line.

## Session regression limitation

Both full Session attempts were75/76. The unchanged1,040-deal corpus stress scenario failed intermittently in Windows file replacement. [14](evidence/14-session-regression.txt) failed at deal950 with SaveFailed; its ordinary runner cleaned the temp directory. A focused retry [22](evidence/22-corpus-regression-retry.txt) passed1/1. A diagnostic full run [34](evidence/34-session-final.txt) reproduced `IOException: Unable to remove the file to be replaced` from `FileSystem.ReplaceFile` and retained all generations. Free disk was about130GB ([disk metadata](evidence/disk-availability.json)).

[Retained inspection](evidence/retained-corpus-failure/inspection.json) shows all three checksums valid: committed primary has handoff0/history458; pending has handoff1 in the same match; previous has no match/history457. Exact bytes, SHA256, timestamps and directory ACL are retained. No SnapshotStore implementation or retry behavior was changed. Host interference is plausible but unproven; this is not a clean76/76 result. The coordinator separately reported one unchanged baseline corpus pass with external TEMP; final integration will compare using that same temporary-directory arrangement.

## Tooling and handoff

Unity6000.3.25f1, exact group/game worktree, sole Editor lease. Sandboxed launch235376 could not use the licensing channel; licensed normal-user Editor230024 performed tests. No ADB, release build, push, issue closure or Nischenreich changes were made.

[Final console state](evidence/45-console-status.json) confirms no compilation failure. [Retained errors](evidence/47-console-errors.json) contain14 UnityEditor.Search.SearchDatabase startup exceptions and the Pipeline generic-quit failure in Edit Mode; they do not have app frames. No console clearing was used. Dynamic Inter/Emoji data was copied into ignored raw evidence, cleared through the Editor and verified to have no semantic Git diff. Pipeline quit and a delayed Editor exit did not terminate the process, so the exact owned PID was stopped after saving results/assets; [both owned PIDs exited](evidence/49-editor-exited.txt).

[evidence-manifest.json](evidence-manifest.json) covers70 selected files/1,768,010 bytes copied byte-for-byte. [raw-manifest.json](raw-manifest.json) covers72 required raw files/16,487,391 bytes, including both generated font caches. Temporary .NET bootstrap/cache files are excluded; the failed test generations are copied into retained-corpus-failure. The merger must preserve the manifest-listed raw artifacts before removing the worktree. Evidence files use Git -text attributes to prevent newline normalization.

Remaining integration work: independent review, motion integration/resume hooks, final Android update installation, real Gboard/Back, native accessibility and save/update acceptance, and final package validation. Group ordering deliberately does not alter unread cards in an already frozen match; that optional extension has not been authorized by a new answer. No physical phone or human-group playtest is claimed.
