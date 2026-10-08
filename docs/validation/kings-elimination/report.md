# Kings elimination — ticket #13

Implemented on `implement/kings-elimination` from clean foundation `d7cf9b895a62942c91e3ad67a523bf43918b57bc`, for [ticket #13](https://github.com/giarrel/word-deduction/issues/13). Source commit: `30ac0c1`. Current integration tip `ed74667ea54b7c01c63c915860e4bddc01253cfe` was merged into this implementation branch before handoff; it adds the acceptance ledger only, with no change to the tested source or test files. The public Session and rendered app are the agreed test seams. The implementation extends the existing atomic store and UI; no alternate rules engine, ballots, vote counts, runoffs or clue-round acknowledgments were added.

## Delivered behavior

- Table participants are selectable, with a saved, correctable pending choice and deliberate confirmation. Every new command guards the match ID, elimination count, phase and target. Back cancels a pending choice.
- Ordinary elimination persists the survivor change and exposes only the public participant plus `KingsElimination`, meaning “Not a king / Kein König.” The Classic role projection, words, leader and result stay absent. The same screen shows remaining selectable people immediately; no Continue action is required.
- The good King's elimination ends in victory for the complete evil team, including eliminated Undercover. Exactly two surviving Kings is also an evil victory. Numerical parity and a lone good King with other survivors remain live.
- White elimination has priority and enters durable `KingsLastChance` with a neutral ID/name projection and no result, words or good-King disclosure. Classic White judgment and ordinary table actions reject that stage.
- Terminal results name the winning team, explain that eliminated teammates share its result, reveal both words and all assignments, mark both Kings, and retain direct rematch and editable-group actions. Group participation is never changed by elimination.
- Existing phase/outcome values are preserved; new values are appended. The unreleased V5 Kings schema from #12 remains in use. The Session interface appendix documents the #14 continuation.

## Red → green observations

All observations are retained, including one corrected test-scope mistake. Session failures below were behavioral failures after compilable public-contract scaffolding, not missing-symbol compilation errors.

| Slice | Red | Green |
| --- | --- | --- |
| Pending correction, ordinary disclosure and direct next selection | `session-selection-red.txt`: table selection rejected | `session-selection-green.txt`: 1/1 |
| Good King, complete evil team, rematch | `session-good-king-red.txt`: elimination did not end the match | `session-good-king-green.txt`: 1/1 |
| Parity/lone good King and exactly two Kings | `session-two-kings-red.txt`: two Kings did not end the match | `session-two-kings-green.txt`: 1/1 |
| Durable White last-chance entry | `session-last-chance-red.txt`: White entered ordinary elimination | `session-last-chance-green.txt`: 1/1 |
| Rendered pending correction and ordinary screen, DE/EN | `ui-selection-red.json`: survivor action absent | `ui-selection-green.json`: 1/1 |
| Rendered complete team result, DE/EN | `ui-result-red.json`: Kings result copy absent | `ui-result-green.json`: 1/1 |
| Rendered safe last-chance entry/restart, DE/EN | `ui-last-chance-red.json`: entry surface absent | `ui-elimination-green.json`: 3/3, all new screens |

`ui-selection-test-scope-failure.json` retained a test mistake: it searched every label beneath the application root, including the hidden group setup with its public initial role count. The assertion was narrowed to the displayed match screen. No production privacy change was needed for that observation.

## Validation

- Full Session suite: **66/66 passed**, `session-full-green.txt`, with the production Session sources, preserved production Newtonsoft DLL, real fresh task-local TEMP/TMP and DOTNET_CLI_HOME, and `WD_TEST_IO_DIAGNOSTICS=1`. No recurrence of the earlier unrelated full-suite failures.
- Focused Session progression: **5/5**, `session-progression-green.txt`. Includes real exclusive pending-file locks during selection, cancellation and confirmation for ordinary, White and good-King targets. Failed actions preserve identical live and reopened safe views; only a successful retry exposes the consequence. This uses the existing store guarantee and passed without a new persistence implementation.
- Unity 6000.3.25f1 compilation completed without errors. New rendered suite: **3/3**, 1.59 s; Classic regression: **4/4**, 4.00 s; Quick regression: **5/5**, 7.21 s. Each has a completed `test_status` JSON, not merely a dispatch acknowledgment.
- Six actual 390×844 RenderTexture PNGs were opened and visually inspected: ordinary elimination, terminal result and last-chance entry, each in English and German. Text, neutral ordinary disclosure, directly available survivor choices, result scrolling and fixed rematch/group actions were readable. Root independently inspected the three German captures. These are Editor renders; no native or physical-device acceptance is claimed.
- `git diff --check` passed. No scene, package, project-settings, font-atlas or other unrelated semantic changes remain.

## Evidence and Editor ownership

Tracked observations and `hash-manifest.json` are in this folder. Raw evidence is in ignored `artifacts/kings-elimination/` in this worktree. The manifest records SHA-256, byte length and repository-relative paths for source files, observations, PNGs, Editor log, real failed-test snapshots and lifecycle evidence; .NET build/runtime caches are excluded.

Only this worktree's Editor was started, PID **100940**, hidden and targeted by the exact project path on every command. The render tests populated the shared Inter atlas from its initial 1×1 texture. The actual generated asset and its diff/hash evidence were preserved as `Inter-populated.asset` and `font-before-cleanup.txt`; only that exact Inter asset was cleared through `ClearFontAssetData(true)` and `SaveAssetIfDirty`. Its normalized Git blob returned exactly to `faec5f30ce35bbc711768f193cba42e7f6ecfea7`, matching HEAD. No other font or unrelated assets were cleared or saved.

The delayed Editor exit acknowledged but did not terminate the process. Its PID, executable and exact worktree argument were revalidated before stopping only PID 100940. `editor-exit-verified.json` records `StillRunning: false`; the lease was then released to root. No ADB commands, APK builds, package installation, push, merge into integration, ticket closure or sibling-worktree mutation occurred.

## Scope handoff

Ticket #14 must implement the binding word/King choice and single attempt, extend `ValidKingsMatch` and `Winners` for its terminal outcomes, and add actions to `KingsLastChance`. This ticket intentionally leaves that safe entry with no judgment or result shortcut. #15 owns rules and private-card readability; #16/root owns combined acceptance, native update/lifecycle/accessibility and the final Android candidate. Human group balance/fun and physical-phone testing remain outside this evidence.
