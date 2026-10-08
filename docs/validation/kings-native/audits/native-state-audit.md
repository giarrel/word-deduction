# Code5 native evidence: offline consistency audit

8 October 2026. **467/467 defined offline consistency checks pass across 74 preserved snapshot files. This is not overall native acceptance.** The known code5 double-tap and max-group accessibility return-transition defects remain open for correction and code6 native verification.

Reproducible audit: `work/kings-implementation/audit-native-state.py`. Full results, complete input-file SHA-256 hashes, payload checksums, comparison results and semantic strings: `work/kings-implementation/native-state-audit.json`.

Audit JSON SHA-256: **`f207fae79deb64cf21891558a43036d1463eba5dc9b2509af3a4fcc261612d19`**.

All paths below are relative to `C:/Users/lucac/Documents/Codex/2026-10-06/sie`; native evidence is under `work/kings-native/`. No source repository, native evidence file, Editor, build, ADB or emulator state was changed. Only this audit's script/JSON/report were written.

## Integrity and provenance

For every audited `session.json` and `session.previous.json`, recomputed SHA-256 of the **original UTF-8 Payload string**, encoded as Base64, matches its envelope Checksum. Recomputed whole-file SHA-256 matches the corresponding manifest; manifest Match, Players and UsedPairIds fields match the decoded primary. This verifies the stored evidence's internal consistency, not cryptographic authenticity against a malicious producer.

Enum meanings, checksum algorithm and history limit were read from immutable source commit **`908d95fc80fc8173df02593422ff85264089b945`** through read-only `git show`; exact source-blob hashes are in the JSON. `actual-update-verified.json` identifies code4 → code5, version 1.1.0 and APK SHA-256 **`afd739cc5b8356a24ee9b2b92c7639ba128569d3f43090ac961fc0e6a3d24a01`**. The preserved installation output says Success. This audit did not independently rehash the APK or inspect a running installation.

## Six actual terminal cases

| Game | Correct checkpoint | Previous → current phase | Outcome | Winner | Used / remaining pairs |
| --- | --- | --- | --- | --- | --- |
| 2 | `code5-game2-result-state` | 6 → 3 | 7 — OnlyKingsRemain | Evil team | 6 / 514 |
| 3 | **`code5-game3-terminal-state`** | 10 → 3 | 8 — KingsWordCorrect | Evil team | 7 / 513 |
| 4 | `code5-game4-result-state` | 10 → 3 | 9 — KingsWordIncorrect | Good team | 8 / 512 |
| 5 | `code5-game5-result-state` | 11 → 3 | 10 — KingsKingCorrect | Evil team | 9 / 511 |
| 6 | `code5-game6-result-state` | 11 → 3 | 11 — KingsKingIncorrect | Good team | 10 / 510 |
| 7 | `code5-game7-result-state` | 6 → 3 | 6 — GoodKingEliminated | Evil team | 11 / 509 |

These are six distinct expected outcomes, all Kings mode with phase3. Each backup is the same match immediately before terminal resolution. Fixed deal fields, assigned participant IDs/names/roles and history agree with their original deal checkpoints. Game2 has exactly the good King and White surviving; game5's chosen target equals its good King; game6's target differs; game7's eliminated suspect is its good King. The word-correct result awards the evil team, not White alone.

Game1 is retained separately as a **failed interaction test**: its terminal phase3/outcome6 is in `code5-game1-result-state/session.previous.json`; primary is the unintended new game2 at phase0/handoff0. The second tap started Rematch. `code5-game1-good-king-result.png` is not a passing result-screen image.

## Roster and seven-game history

`code5-kings-five-group-state/session.json` has Nora, Luca, Emil, Mila and **Jonas**. Comparing to `code5-renamed-group-state/session.json`, the only changed JSON path is `/Players/4/Name`: **Jonas → Jonas2**, retaining ID **`ac5d584c805b49889e54e0db0a07e654`**. The rename backup is byte-identical to the original five-player primary. All seven completed Kings matches retain the same ordered player and participant identities; each of games2–7 retains that roster in both generations.

Starting history contains four earlier pairs: `tools-materials-023`, `kitchen-011`, `clothing-021`, `technology-014`. Games1–7 append exactly:

`mammals-023` → `places-004` → `garden-nature-025` → `meals-023` → `school-office-007` → `tools-materials-018` → `fruit-vegetables-025`.

Every step preserves all prior UsedPairIds, removes only the new pair from the remaining queue without reordering it, and keeps used/remaining disjoint with **520 total entries**. Final UsedPairIds contains all eleven. RecentPairIds intentionally retains only the latest ten, as defined in source; dropping `tools-materials-023` from Recent is not lost played history. `code5-group-after-seven-games-state` retains the final roster/history.

## Actual update, recovery and separately labelled Classic replay

Independently recomputed both-generation byte equality supports the existing comparison reports for: real install before first launch; initial old-Quick resume; emulator recovery; unfinished Nora restart; game3 committed word branch; game3 committed answer/judgment; and game5 pending King target. Game5's restored pending target is Nora, still wrong at that checkpoint; the later terminal's corrected target is Jonas2. Byte equality proves durable state, not screenshot readiness.

The real Quick first durable advance changes V4 → V5, retains the original V4 primary as the exact backup, and changes **only `/Match/Handoff`, 1 → 2, among legacy fields**. Original match, roster, deal, words, preferences and history remain intact.

The later **explicit V4 Classic fixture replay is compatibility evidence, not a second real install update**. Source `work/android-16kb-probe/final-long-word-state` matches both files in `code5-old-classic-before-resume-state` byte-for-byte. After the reported real Bea hold/Next action, `code5-old-classic-after-advance-state` contains V5 primary with only legacy Handoff1 → 2 changed, and exact old V4 primary as backup. Match **`b7203215ad014899bfda2147685f8ddd`**, Classic phase0, Sprachnachricht/Videoanruf, roster, roles, preferences and history are preserved.

## Synthetic max-card trees and corrected retained-log scans

The four synthetic fixtures are deliberately excluded from the seven actual matches. Their replay-manifest expected primary hashes agree with the source fixtures. Audited held/released tree pairs:

- German good King: `code5-de-good-talkback-private-{held,released}-tree.json`.
- English evil King: `code5-en-evil-max-header-{held,released}-tree.json`.
- German evil King: `code5-de-evil-max-header-{held,released}-tree.json`.
- English good King: `code5-en-good-max-header-{held,released}-tree.json`.

Each tree has **16 nodes and nine non-null text/description values**. Held and released semantic values are identical. The entire ordered value list matches a public-only allowlist: Back, mode, Help, card counter, public owner, handoff instruction, gesture instruction, private-reading explanation and pass-on button. Action labels are null. No word, non-owner participant name or role/name association is added. Complete node fields and action strings are retained in the JSON, so this is stronger than checking a few substrings.

Good-King trees expose the public owner ending **`· 10`**; evil-King trees expose owner **`· 9`**. The EnglishGood helper's original stop on known secret name ending `· 1` was a substring false positive inside the complete public `· 10` value. The saved original run remains failed/aborted; this separate complete-value audit passes the two captured trees. It does not retrospectively claim the helper completed its remaining actions.

German fixtures contain **Pfirsich/Aprikose**, not Nektarine. The original `code5-de-good-max-log.json` omitted Aprikose; this audit rescans the retained PID9394 log against both actual words, all raw/display names and all match/player IDs: **44 distinct values, zero matches**. Exact saved log hash: `1716b0f7a26c8fa4f4ddfa81a1600e779aa173c13b74817573286db2f006b8cf`. It has 90 nonempty lines/90 LF bytes, 11,875 saved bytes; normalizing doubled CR yields the original metadata's 11,785 bytes. The original files are unchanged.

The retained PID10823 `code5-max-end-log.txt` is also rescanned against **all20 players' actual raw/display names and IDs**, plus Kirsche, Pflaume, Peach and Apricot: zero matches. This closes the original scanner's wrong-Name-property omission, which tested only four words. Exact saved log hash: `0594305fc63f72b0a8907a03438a112f1c918b4ff8a3f690c49e2119f9f51445`; 116 nonempty lines, 17,124 bytes. These are bounded retained-log scans, not assertions about uncaptured future processes or every diagnostic category.

`code5-max-clamped-state` and `code5-max-end-state` retain all20 players, 18 active and manual KingsUndercoverPreference **8**. The settled Quick/Classic/Kings trees `code5-group-return-repro-{0,1,2}.json` have the correct selected mode. Kings reports **10 good including King, 7 Undercover + White**: effective7 matches the source limit at18 active while the saved preference remains8. These settled-state checks do not negate the separately observed accessibility return-transition defect.

## Evidence boundaries retained

`code5-game3-result-state` is phase10/outcome null and is not terminal. Parent notes identify `code5-game3-judgment-resumed.png` and `code5-game3-word-correct-result.png` as still paused; actual resumed judgment and result use the alternate-point/terminal evidence. Repeated taps at one coordinate while not ready are an input/readiness observation, not a diagnosed app defect. `code5-game5-target-safe-restart.png` was startup blank and remains non-passing visual evidence.

`observations-code5.md` is an earlier in-progress note; its pending game5/6/7 state checks are superseded here by actual later files, while its failed/mislabelled distinctions remain. Root reports visual inspection of four max-card headers and bottoms/lists; this offline audit does not independently establish visual fit, spoken output, gesture usability or general accessibility success. **Known double-tap and max-group return-transition defects, final code6 source/build verification and code6 native regression remain pending.** No physical-device or human group/balance claim is made.
