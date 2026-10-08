# Kings last chance — ticket #14

Implementation source: `e918bd7`; rendered test refinement: `7dab37b`. The full combined rendered run uses `dd9fab75313da403425c6e82fd024ec9aef2a5e6`, including the accepted #15 merge/report. The subsequent merge of root progress/matrix docs at `cac843d64a6772d157a330ede49e8b8c217174cc` changes no game, test, content or Unicode input. Canonical requirements: [ticket #14](https://github.com/giarrel/word-deduction/issues/14) and [Spec #10](https://github.com/giarrel/word-deduction/issues/10).

## Delivered contract

White's elimination now offers a binding word-or-King choice. The word branch saves the spoken-answer acknowledgment before exposing a phase-scoped `Match.Judgment.Word`; the group judges meaning with neutral Correct/Incorrect actions. No answer text/audio or automated matching is added. The King branch shows unmarked survivors, saves a correctable pending identity, then resolves one confirmed target. Back can cancel that pending target or pause; it cannot change a committed branch. Success wins for Undercover and White together, failure for every Civilian, including eliminated teammates. Complete words and King assignments remain terminal-only except for the explicitly permitted judgment word.

The Session appendix documents legal actions, saved phases/outcomes and disclosure. Every new mutation uses the existing atomic save transaction and match/phase/target guards. Invalid saved combinations are rejected even with a valid checksum; Quick/Classic cannot accept Kings phases. No package, scene, production font, build setting or old-mode rule change is required. #11 remains separate.

## Behavioral evidence

The agreed public Session and actual rendered-app seams were used. Raw observations are preserved under `artifacts/kings-last-chance`; `manifest.csv` records the exact files and hashes after Editor shutdown.

- Word choice/answer-stage red → green: restart keeps the branch and pending judgment, no target before the acknowledgment commit, old/duplicate commands rejected (`01-*`).
- Word judgment red → green: both outcomes award whole teams, full terminal disclosure, reopen/rematch and old-match rejection (`02-*`).
- King target red → green: surviving stable IDs only, pending correction/reopen, finality, both team outcomes (`03-*`).
- All new write failures retain the exact safe live and reopened view. A valid-checksum malformed Classic phase test first failed, then passed after the validator excluded Kings phases outside Kings (`04-*`).
- The first full run was **70/71**: the old public-projection test caught a new top-level nullable word field. Replaced it with the nullable phase-scoped Judgment projection; the old test is unchanged. Its focused run and the corrected full Session run passed **71/71** (`05-*`, `06-session-full-green.txt`). Failed snapshots are retained. This was a deterministic interface regression, not a host IO failure. `session-input-comparison.json` proves later UI/docs merges did not change those Session test inputs.
- Rendered word flow red → green (`07-*`); actual screenshot inspection caught a reused Classic solo-White-win label. A focused test reproduced it, then neutral localized labels passed (`08-*`).
- Rendered King flow red → green (`09-*`), including pending-name correction via Back, reopening, failed writes, both results and rematch in DE/EN.
- Three focused final rendered scenarios passed **3/3** (`11-scrolling-accessibility-completed.json`). They include 360×640, 150% text, 40/24 safe-area insets, twenty duplicate 24-character wide names, reachable final targets/instructions/result rows and controls within the phone. Recursive public accessibility includes the good word only at the judgment stage, with no evil word or good-King identification.

## Visual inspection and limits

Actual render-texture PNGs were opened and inspected for German choice, spoken-answer, judgment, King targets/confirmation, both team results, and English judgment. Large-text first/last screens were inspected for every new German last-chance stage and final result. Long-name suffixes remain distinguishable, actions stay visible, and the last explanatory line is reachable through public scrolling. Required progression contains no social voting, advice, clue or speech enforcement. Final captures wait for settled layout.

These are Unity Editor UI Toolkit tests and public Session behavior, not Android, audible TalkBack, physical touch/haptics, blind private-card access or human-group balance evidence. Root owns Android validation and release packaging under #16.

## Final validation and handoff

- Complete combined rendered assembly: **44/44 passed in 207.16s**, with zero skipped/inconclusive cases (`evidence/12-rendered-full-result.json`). This includes all new paths and existing Quick/Classic, recovery, privacy, group editing, Kings cards/rules and the full bilingual card-word fit regression.
- Session: **71/71**, production sources and tests byte-identical after the later UI/docs merges. Compilation completed without errors (`evidence/12-final-compilation.json`). No authored changes to Unity scenes, fonts, packages or build settings.
- Final generated judgment PNG was reopened after the complete run; Richtig/Falsch remains correct. English large-text choice and King-confirmation end views were also inspected: complete binding/no-second-attempt copy remains readable above the fixed actions.
- Owned Editor **PID218656** was started only for this exact worktree. Its executable/project command line was verified before requesting exit. The closing CLI connection lost its response as the process ended; the OS subsequently confirmed **StillRunning=false at 2026-10-08T18:42:55Z**. The coordinator received the released lease immediately. Other Editors were untouched.
- Rendered tests generated Inter atlas pixels/glyph tables. Those exact diffs were preserved in the raw `generated-font-cache.patch`; after Editor exit, only the previously clean Inter/Emoji assets were restored from this branch's HEAD. Their final diffs are empty. No authored asset change was discarded.
- All raw output, screenshots, failed snapshots and generated-cache diagnostic data are inventoried in `manifest.csv`; focused results are also copied into `evidence/`. The separate merger must preserve the raw `artifacts/kings-last-chance` tree before worktree cleanup. No ADB, emulator mutation, APK build, push or issue closure was performed by this implementer.
