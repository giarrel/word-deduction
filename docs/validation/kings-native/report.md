# Kings Android validation — accepted local candidate

Accepted local candidate: **1.1.0/code8**, source `5c56e63d755df24156e6f5e88f4170ae4857a0ab`. The seven original UI-played Kings games were on code5; the final APK received focused native correction regressions. [Final corrections and package inspection](../kings-final-corrections/report.md), [independent merge verification](../kings-final-corrections-merge/report.md), [56-story matrix](../kings/acceptance-matrix.md), and [delivery hashes](delivery-manifest.json) provide the full chain. Session **74/74** and rendered **47/47** pass; final independent Standards/Spec follow-ups have zero findings.

## Environment and evidence boundaries

Root exclusively operated the isolated `word_deduction_api36_16k` AVD through the Unity-bundled ADB server5038/transport127.0.0.1:5583. Android16/API36, 1080×1920 at density480 (360×640dp), actual16KB kernel pages and both16KB compatibility shims disabled. The app is ARM64 IL2CPP executing through Android's native bridge on an x86_64 emulator. Airplane mode was enabled. Normal text and system font1.5 were inspected. Secure-window screenshots came from the authenticated emulator framebuffer; ordinary app screenshots/Recents must conceal private data.

This is actual emulator execution, not physical ARM64 hardware, physical haptics/performance, audible TalkBack evaluation or human group balance. Local packages use the existing local debug signing certificate while the application is a non-debuggable release build. Production signing and Store publication remain outside Spec#10.

## Actual update and preserved players

Installed code5 over the real code4 installation without first replacing save data. Both primary and previous generations remained byte-identical after install and initial resume. The existing Quick game resumed with Nora's card completed and Luca next. The next actual handoff migrated V4→V5, preserving every legacy field except the intended Handoff1→2 advance; the old primary became the exact backup. Completed Quick, a normal rematch and deliberate abandon retained the saved group.

Added Mila and Jonas using the native keyboard, paused/restored a player at Kings' minimum and renamed Jonas→Jonas2. An independent offline audit verifies that only `/Players/4/Name` changed and the stable ID remained `ac5d584c805b49889e54e0db0a07e654`. The same five-player roster survived all seven actual Kings matches and returned active to Group. Persistent history appended exactly seven new pairs without losing the earlier four; recent history intentionally retains its latest ten.

A later explicitly labelled replay of a previously saved V4 Classic checkpoint independently verified compatibility. Bea/card2of5 resumed with Sprachnachricht; actual hold/release/Next reached Cora/card3of5 and migrated only that durable advance. This is fixture compatibility, not a second claimed real installation update.

## Six ending paths and recovery

All six endings were reached through actual Android UI actions before any synthetic max-layout replay:

| Actual game | Terminal checkpoint | Expected result |
| --- | --- | --- |
| 2 | `code5-game2-result-state` | Only both Kings remain: evil team wins |
| 3 | `code5-game3-terminal-state` | Correct word: evil team wins |
| 4 | `code5-game4-result-state` | Incorrect word: good team wins |
| 5 | `code5-game5-result-state` | Correct King target: evil team wins |
| 6 | `code5-game6-result-state` | Incorrect King target: good team wins |
| 7 | `code5-game7-result-state` | Good King eliminated: evil team wins |

Ordinary eliminations disclosed only Not a king; parity and a lone good King against two evil players continued. The two-King end was reached only after the remaining ordinary evil player was eliminated. Result lists included eliminated teammates and both Kings. White's win is an evil-team win.

Force-stop/relaunch preserved both save generations exactly for an unfinished handoff, committed word branch, committed spoken-answer judgment and pending King target. The pending wrong target remained pending after restart and could be corrected before final confirmation. Startup/return showed a safe pause; private cards resumed covered. Home, Recents, pointer cancellation and second-finger interruption covered the actual card. Completed cards could not be revisited through the normal handoff flow.

## Layout, privacy and public navigation

Four separately labelled public-Session-generated fixtures exercised both leaders in German and English with20 participants/eight ordinary Undercover plus White. At150% font on360×640dp, role headings, words, all maximum private-list entries, long duplicate-name suffixes, accented Latin, CJK, Greek, Cyrillic and emoji were reachable and readable inside the rounded private viewport. Root inspected the actual held header/list/end and released frames. Native system font fallback rendered characters absent in earlier Editor captures.

Actual TalkBack16 was bound with touch exploration and service-managed double tap. Native standard actions opened/scrolled Help; a two-pointer gesture moved the public rules and another moved the long public roster. Public hierarchy dumps while cards were genuinely held/released contained only the complete allowed public values. They contained no private word, leader assignment, role/name association or private participant list. Next was disabled while held. These injected visual card gestures do not establish TalkBack pass-through discoverability or blind independent play; private audio is intentionally absent.

After public Group redraw,20→19 players kept8 ordinary Undercover;19→18 showed7. The stored preference stayed8 and Quick/Classic/Kings switching preserved it. The offline audit verifies both stored values and selected-mode semantics.

## Defects found and correction gates

1. A repeat requesting75ms host spacing (actual original Android interval was not measured) on good-King elimination confirmed the result and then accidentally activated the newly rendered Rematch button. Original game1's terminal is in its backup; its primary is the unintended game2. The misleadingly named result screenshot is retained as failed evidence. Source correction protects newly rendered controls after a successful pointer commit while preserving normal navigation and later intentional single taps. Rendered tests also cover answer→judgment, target→result and judgment→Group boundaries.
2. With TalkBack bound, both newly shown Group→Match and Match→Group panels could have an empty native hierarchy because visibility styles had not resolved at the one-shot hierarchy build. Same-panel redraw restored nodes. This was reproduced natively twice and then in a5/20-player rendered regression. The final code8 native public transitions pass in both5- and20-player groups; details below preserve the failed earlier candidate.

`native-state-audit.md/json` records467 defined offline checks across74 original snapshot files: six distinct terminal outcomes, stable IDs/history, checksums/manifests, recovery, migration, max-card semantic allowlists and corrected retained-log scans. This complements actual UI observation and does not replace it. Retained nonempty per-PID logs found no played private values or targeted failure diagnostics; no global logs were cleared.

Known harness distinctions remain explicit: early startup blank captures are not passing screenshots; game3's misleading result checkpoint is still judgment and the correct terminal has a separate name; one max-card substring check mistook public `·10` for private `·1` and was replaced by an independent complete-value audit; two log argument omissions were corrected against the retained original logs. No failures were erased or relabelled as successes.


## Final package regression results

Coordinator-owned actual emulator observations, 2026-10-08. Candidate source `5c56e63d755df24156e6f5e88f4170ae4857a0ab`, version 1.1.0/code8. APK SHA256 `d4ba4c5ba65ca03249f59455234c1df7bdf4f85b160828ca05b945a85cd78ab9`. AAB SHA256 `41d5a652323b3a263bd7d28d5847bced7e9a2c3ab5a1359500e3026bb0ca8e50` (AAB inspected, APK actually installed).

- APK hash independently matched before install-r. Both generations remained byte-identical (`code8-update-byte-equality.json`). This final update used an explicitly replayed original pending-Nora checkpoint; the earlier real code4→5 migration remains separately documented.
- First cold process after installation: measured repeat had 76ms first-UP→second-DOWN. `code8-first-cold-doubletap-state` remains match69fc9f, Phase3/Outcome6; visually inspected result stays visible. Earlier code7 measured answer/judgment boundaries passed with the identical input-guard source. First unmeasured code7 rematch observation remains retained, with actual timing unknown.
- TalkBack activation once triggered Android LOW_MEMORY process exit (pid13388, reason3/status0, RSS543MB), not a recorded Java/native crash. First `code8-five-public` probe stopped before any action. Both save generations survived exactly (`code8-low-memory-preservation.json`). Empty crash buffer, platform exit record and retained PID log are preserved. Relaunch with the service already bound resumed the terminal result.
- `code8-five-bound-sequence.json`: eight actual native public actions pass. Result→Group35 nodes; Quick31, Classic32, Kings35; Start→Match16; Back15; abandon15; confirmed abandon→Group35. No redraw workaround.
- German maximum fixture:20 participants/eight ordinary Undercover+White, font1.5. `code8-max-good-pull-held.png` shows good-King/Pfirsich header; `code8-max-good-bottom-held.png` reaches final entries. Paired pull-held/released native trees expose public values only. First stationary injected TalkBack holds stayed covered; those images are NOT private-visible proof.
- First20-player sequence returned the previous card tree immediately after Back. A later read showed the expected Pause15 without repeating the action. The revised external sequence waits boundedly for the destination label with read-only probes and preserves every intermediate tree; it never repeats an action or forces redraw. `code8-twenty-settled-sequence.json` passes: abandon→Group64→Start/Match14→Pause15→confirmed abandon/Group64. Actual Group and new handoff frames inspected.
- English maximum evil-King fixture: `code8-max-evil-pull-held.png` shows Mr.White/no word; bottom-held reaches eight teammates, including readable CJK/Greek/Cyrillic. Paired pull-held/released public trees expose no secret. After restoring original TalkBack settings, ordinary stationary hold opens the card and release covers it (`code8-max-evil-ordinary-hold-*`). No spoken output, real TalkBack pass-through gesture or blind independent play is claimed.
- Normal font restored. Labelled replay of actual game7's terminal followed by one intentional native tap starts a fresh match `b8a9b91c8fa14f2db7605b8ba32c809c`, Phase0/Handoff0, all five players active. Actual Back/abandon returns to Group. Final checkpoint has Matchnull, original five stable player IDs/names active,12 used pairs (eleven earlier plus the intentional rematch).
- Final system font1.0, TalkBack bound services empty and touch exploration false. Final actual Group screenshot inspected. Nonempty current-PID logs and the retained killed-PID log were scanned for actual names/words and bounded failure patterns; evidence retains platform warnings. No global log buffers were cleared.

Validation environment remains the isolated Android16/API36 16KB-page emulator, ARM64 IL2CPP through the emulator's native bridge, airplane mode. This does not prove physical-phone performance/haptics, audible accessibility or human group enjoyment/balance. Production signing and Store publication are outside this local release acceptance.

## Preserved evidence

All native originals are archived locally under `artifacts/kings-native/raw/`, with byte verification in [raw-preservation.json](raw-preservation.json): **1,092 files / 76,455,269 bytes**. [Selected evidence](selected-evidence-manifest.json) includes original terminal generations, update manifests, final public trees, inspected screenshots and four independent offline audits. Raw observation labels in the matrix identify these files or the complete local archive. Failed and intermediate observations were retained, never overwritten with passing results.

Offline audit reports cover [original played games and maximum cards](audits/native-state-audit.md), [code7 timing and failed accessibility](audits/code7-native-audit.md), [code8 first-cold-process and five-player observations](audits/code8-native-audit.md), and [final maximum-card, group and package checks](audits/code8-final-native-audit.md). They verify consistency of retained bytes; actual image inspection and device operation belong to the coordinator. The APK and AAB are copied with independently verified hashes to the local `outputs/Word-Deduction-1.1.0` handoff.

Quick/Classic free role controls and White in Quick remain in issue#11. Their undecided victory rules are not inferred from Kings. Physical-phone testing, human group balance, private spoken accessibility, production signing and Store publication are not claimed.

## Worktree cleanup

After the separate merge and hash-verified preservation, all six clean, merged Kings implementer worktrees were removed. Their branches and the primary checkout remain. [The cleanup record](worktree-cleanup.json) identifies each exact path and source commit. The first Git removal left an unregistered acceptance-directory remainder when Windows denied access to a Unity build report; a guarded native PowerShell cleanup under the owning user's permissions completed that path. No source changes or validation evidence were discarded.
