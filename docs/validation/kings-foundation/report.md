# Kings foundation — ticket #12

8 October 2026. Scope: select/configure/deal the third mode, privately inspect its initial cards, and reach a neutral table-play landing. Integration base `bcdbb1389bea14b6e38e0122649380eafef04c1e`; dedicated branch `implement/kings-foundation`. The unpublished planning predecessor was replaced by the coordinator for author-email correctness; `reset --soft` moved this uncommitted work onto the identical corrected tree without deleting edits.

## Behavior delivered

- Persistent Kings/Könige selection, 5–20 participants, one good King, one White evil King and valid ordinary-Undercover controls. All modes share the initial majority bound; Quick/Classic free controls from #11 remain untouched.
- Adaptive defaults until manual choice, retained requested count across group shrink/grow and mode changes, visible effective-count adjustment, durable Auto reset and invalid-setting rejection.
- Fresh randomized assignments with existing DE/EN catalog/history. Ordinary cards are neutral word/leader cards. The good King sees the good word and unmarked evil names in participant order; White sees only his identity and teammates, with neither word.
- Owner-scoped `RevealCard`; no private fields on public match/participant views. Synchronous concealment covers every private label. All private information stays inside the excluded accessibility subtree; a recursive hierarchy assertion checks descendants too.
- Completed handoffs cannot be revisited, unfinished interrupted handoffs reopen covered, no timer. Handoff completion leads directly to `TablePlay`, with no Quick/Classic vote or tie action accepted.
- V5 saved format; old groups, V2/V3/V4 live deals, old enum values, accepted Unicode names, IDs, undo and durable word history stay compatible.

## Red → green evidence

Raw observations are retained under ignored `artifacts/kings-foundation/`; the final evidence manifest records their hashes. Tests run the production Session sources and actual rendered UI, with controlled randomness and real temporary persistence.

| Slice | Failed observation | Passing observation |
| --- | --- | --- |
| Third mode / minimum / adaptive default | `01-selection-red.txt`: Kings rejected as a setting | `01-selection-green.txt`: 1/1 |
| Retained explicit count | `02-preference-red.txt`: new public preference contract absent at compilation | `02-preference-green.txt`: 2/2 |
| Owner-scoped knowledge and completed handoffs | `03-private-card-red.txt`: new card projection and TablePlay contract absent at compilation | `03-private-card-green.txt`: 3/3 |
| V5 compatibility | `04-schema-red.txt`: V5 rejected, existing deal inaccessible | `04-schema-green.txt`: 1/1 |
| Rendered mode/count setup, both languages | `05-selection-ui-red.json`: Kings selector absent | `05-selection-ui-green.json`: 1/1, 0.72 s |
| Rendered private variants/concealment/table landing | `06-private-ui-red.json`: leader/private fields absent | `06-private-ui-green.json`: 2/2, 2.93 s |
| Recursive semantic privacy, interrupted card, mode-specific help | `07-privacy-help-red.json`: card privacy/resume checks passed, but Kings incorrectly displayed Quick help | Final rendered suite, including dedicated Kings help |

Further Session validation covers every supported size and every valid selected ordinary-Undercover count, reopening all frozen deals, neutral ordered knowledge, multiple possible King seats, repeated assignments, failed preference/deal/handoff writes and unchanged group participation. `kings-session-expanded.txt`: 6/6 passed.

## Final validation

- **Session: 61/61 passed**, `session-final-diagnostic.txt`, with `WD_TEST_IO_DIAGNOSTICS=1`, the original workspace-local temp path and the Editor confirmed closed. Includes all 55 existing cases and six Kings cases (72 valid size/count combinations).
- **Rendered Unity PlayMode: 33/33 passed in 141.34 seconds**, `rendered-full.json`. Includes the three Kings cases and all existing content, Classic, Quick, recovery, large-type, accessibility and release-information cases. No skipped/inconclusive tests.
- `git diff --check` passed. No generated font, scene, package or project-settings changes remained. No native APK or release acceptance is claimed by this ticket.

The Session command is the established `dotnet run --project tests/Session.Tests/Session.Tests.csproj -p:NewtonsoftJsonAssembly=<preserved-production-Newtonsoft-DLL> --no-launch-profile`, using a writable process-local TEMP/TMP and DOTNET_CLI_HOME. Optional `WD_TEST_IO_DIAGNOSTICS=1` adds failure-only IO stacks and retains the failed temporary snapshot. Tests still observe the public Session contract; diagnostics are not new production behavior or assertions against private helpers.

## Execution observations and scope limits

- Reused Unity 6000.3.25f1 and the existing Pipeline package/CLI; no toolchain or package installation. Only this worktree's Editor was opened (owned process 214156). Root retains ADB ownership.
- The Session runner used the preserved production Newtonsoft DLL and process-local temporary/.NET paths under ignored artifacts because the sandbox blocked the system temporary directory. No production storage paths changed.
- A first full Session run recorded 58/59: one existing Quick handoff test failed once. The identical focused test immediately passed without a source change (`quick-repeat.txt`). A later full run recorded 60/61 with a different existing test failing at a language change during the 1040-deal cycle. Neither generic assertion captured the original action error, so the cause remains **unconfirmed**, not asserted to be an app bug or host IO issue. Public action-error and optional host IO diagnostics were then added to the runner. The 1040-deal case passed unchanged on both the shorter and original temp paths (`session-io-diagnostic.txt`, `session-io-original-path.txt`), followed by the separately retained 61/61 full diagnostic result. Earlier failed runs are preserved, not rewritten as passing or described as a diagnosed fix. A recurrence will retain its snapshot and underlying IO evidence for investigation.
- One AssetDatabase refresh command exceeded the Pipeline five-second callback limit; the Editor completed its import and the following rendered tests executed. It was not treated as a test failure or dispatched twice.
- Automatic approval review rejected an unnecessary font-data clearing/saving command as destructive tracked-asset mutation. That command did not execute and was not retried indirectly. Tracked font/scene diffs were verified empty; the Editor was instead closed without asset saving. Its delayed Exit acknowledgment did not terminate the process, so the exact owned PID/executable/project was revalidated and stopped; an OS check returned `StillRunning=false`. No unrelated Editor was touched.
- TablePlay elimination/outcomes and final last-chance rules await #13/#14. #15 owns complete rule text, maximal/long private-name layout and small-phone/large-font polish. #16/root owns Android update/lifecycle/native accessibility/release validation. No assertion of physical-phone or human-group balance/fun is made here.
