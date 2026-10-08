# Kings rules and private cards — ticket #15

8 October 2026. Dedicated branch `implement/kings-rules-cards`, initial integration base `d7cf9b895a62942c91e3ad67a523bf43918b57bc`. Feature checkpoint `235e78f` was merged with the completed elimination work and latest coordinator documentation at integration `4217440823ce78c4c25ef356d8e3e4f9f542887e` before final validation. This ticket changes optional rules, private-card presentation and bounded reveal scrolling, not Session progression. Tickets #13/#14 own eliminations and the last chance.

## Bilingual table-rule audit

Both language strings were read against the accepted rules in `docs/specs/kings-mode.md`. This is a copy audit, not a test claiming to enforce human behavior. Optional Kings help is shared by the general Info entry and in-match Help. Existing Quick/Classic help retains its mode-specific rules.

| Rule | Actual DE/EN section reviewed |
| --- | --- |
| 5–20 participants, strict initial good majority, one King per side with teammates, ordinary Undercover count plus mandatory White, random repeatable roles, existing word-history continuity | Two teams / Zwei Teams |
| Neutral ordinary word/leader knowledge; different words; good King knows every evil name but not its King; White knows teammates but neither word nor good King | What your card tells you / Was deine Karte verrät |
| Own initial handoff only, unfinished reopens allowed, no later completed review, no timer, longer viewing as bluff, no showing cards | Read privately / Privat lesen |
| Random surviving starter every clue round including White, one word each in table order, discussion, eliminated people stop clues/votes, app only records eliminations | One clue each / Je ein Hinweis |
| No own word, obvious forms, translations, spelling/initial-letter hints; own word also forbidden during discussion; true/false identity claims allowed; naming real leader permitted at strategic risk | Clues and bluffs / Hinweise und Bluffs |
| Simultaneous pointing at another survivor, one vote, highest count without absolute majority, one spoken runoff among top-tied people, second tie yields a clue round with no elimination/app action, correct pending selection | Point / Zeigen |
| Ordinary elimination says only Not a king; side/word/leader stay hidden; parity and one good survivor do not end play; good King loss and exactly two Kings yield evil win; White gets last chance before final reveal | Find a King / Einen König finden |
| White chooses word OR surviving King alone with no advice; one binding attempt, no switching/retry; speak before confirming/revealing target; pending King target correctable; restart preserves commitment | White's last chance / Whites Letzte Chance |
| Group judges meaning: articles, number, true synonyms may count, related words do not; no speech/text matching; whole-team victory including eliminated people; no White solo win; full reveal only at result | Judge meaning / Bedeutung werten |
| No automatic penalty for accidental invalid clues or leaks; continue or collectively abandon/restart; retained group, rematch restores active people with new assignments | Accidents / Missgeschicke |

The glossary scopes King/Anführer and the Civilian's knowledge correctly. Public cards remain neutral; there are no new ballots, timers, tutorials, speech processing, rule penalties, completed-card review or enforced social decisions.

## Red → green observations

Raw evidence remains in ignored `artifacts/kings-rules-cards/` and has a hash-verified backup in the primary repository at `artifacts/kings-rules-cards-ticket15/`, so worktree cleanup will not remove it. The retained `evidence/` files include red/green results and final screenshots; `manifest.csv` records their SHA256 hashes. `raw-manifest.csv` includes the Editor logs and generated-font patches.

| Slice | Failed observation | Passing observation |
| --- | --- | --- |
| Optional help from Info and match, both languages, large text | `01-help-red.json`: Info has no Kings rules entry | `01-help-green.json`: 1/1; actual help screenshots inspected |
| Maximum private list reached by one in-bounds pointer | `02-private-list-red.json`: last name bottom 721.20 versus visible bottom 486.20 after dragging to y48 | `02-private-list-green.json`: 1/1, eight DE/EN × King × hold/drag scenarios, forward and back |
| Three full mode labels and Kings count controls at 150% | `03-selector-red.json` and PNG: Schnell/Klassisch overlapped; 89px label in 79px available width | `06-mode-row-green.json`: 1/1 in both languages; intrinsic label widths and smaller side padding keep all three modes in one row |
| Long words inside the Kings private viewport | `04-word-fit-first.json`: Nuss-Nougat-Creme measured 255px in a 254px content width | `04-word-fit-green.json`: 1/1, three actual catalog words, unbroken tokens at ≥22px with a long leader |
| Preservation of whole-card privacy and ordinary neutrality | Existing foundation behavior, strengthened regression rather than a new rule | `05-privacy-regression.json`: 1/1, four kinds × DE/EN, recursive hierarchy, every interruption, completed handoff rejection |

The first list fixture accidentally supplied a 25-element accented name. Its rejected rename is separately retained as `02-private-list-fixture-error.json`; it is not a product failure. The corrected fixture uses valid 24-element names. The red screenshot also exposed Hold/Next overlap with a very long owner. Later fixture scaling waits for the large-text class to settle before multiplying text, matching production scheduling; the first screenshot overscaled that owner.

The first list refinement passed hold, but the drag path remained 61px short because its travel calculation ignored the safe area's top padding. `02-private-list-diagnostics.json` and observed layout measurements identify offset 660.7 of 724 versus hold 724 of 724. Including the actual top padding produced the final passing bounded path.

The selector's first two-row refinement exposed a 4dp lower-safe-area overrun; reduced Kings-setting spacing fixed that focused scenario (`03-selector-refinement.json`). The first complete merged run then found two existing large-text regressions: empty-state copy overlapped the name field by 4dp and a disabled Classic start reason exceeded the safe bottom by 13dp (`06-group-spacing-red.json`, 39/41 passed). A spacing-only refinement exposed further language-specific overflow. The final selector gives labels their intrinsic widths with smaller side padding in one row. This keeps the complete German labels and removes the additional row's height: existing Polish tests **9/9 passed** and the Kings selector **1/1 passed**. The spacing-only experiment is not in the final source.

## Final validation

- Existing Kings Session behavior: **6/6 passed**, with IO diagnostics enabled, using the preserved production Newtonsoft DLL. This ticket adds no Session rules.
- Final complete merged rendered suite: **41/41 passed in 209.87s**, including all five ticket tests, #13 elimination tests, the full existing bilingual long-word corpus and Quick/Classic/recovery/privacy/accessibility regressions (`evidence/rendered-full.json`). All focused slices also passed.
- Actual screenshots were inspected, not merely generated. Long duplicate owner suffixes and full last-name suffixes are visible, Hold and Next do not overlap, words remain whole and readable, covered faces have no private text, optional help scrolls to its end, and all three mode labels fit.
- A default-detail image inspection appeared to omit the initial H in Help/Hilfe. The coordinator independently read the complete label in the identical 39,349-byte file (SHA256 `026f7a32bacadffb4c8397c7b65c3dabbacf898aa27bfb39f05ba2ec89f46f2f`), and reopening it at original detail confirmed the complete H. `07-help-glyph-observation.png` pins that file. This was an inspection discrepancy, not evidence for a product font change; public Help width is also checked after release. Captures wait 150ms after layout.
- A similar apparent omission in the Chocolate and Rollkragenpullover frames was resolved with direct, read-only PNG pixel comparison. The H rectangle (x269–290, y66–91) has **zero differing pixels** against the complete-H English White frame in both images; recorded rows show both vertical strokes and the crossbar. The coordinator confirmed identical file hashes. See `evidence/07-header-pixel-comparison.json`. The actual German Help text measures 56px inside 56px of content space; the public header needs no production change.

## Environment and limitations

- Actual Unity 6000.3.25f1 UI Toolkit rendering at 360×640 logical pixels, cloned production panel/fonts, 150% text, simulated 40/24 safe-area insets. Captures come from the panel's render texture, not reconstructed pictures.
- Maximal lists use 20 participants and eight ordinary Undercover: nine evil names for the good King and eight teammates for White. Names include duplicates, 24 wide letters, accents, CJK and 24 joined emoji sequences. Pointer coordinates stay inside the visible phone; private scroll offsets are never assigned by the test.
- The first inspected Windows screenshot had missing CJK glyphs. Android uses system font fallbacks and root will test native CJK; no new fonts were installed or an Android regression inferred from the Windows result.
- First owned Editor PID 218668 was closed after the initial observations. Its exact path/project were checked; graceful exit remained pending, so only that verified PID was stopped. The OS confirmed `StillRunning=false` before releasing the lease.
- Refinement Editor PID 217056 was closed after the final suite. Exact executable and project identity were verified, and OS confirmation recorded `StillRunning=false` at 18:18:34Z. Root received the released lease before evidence packaging. Other Unity processes were untouched.
- Tests populated `Inter.asset` and `Emoji.asset` dynamic atlas pixels and glyph tables; the full word regression also populated Inter character/feature tables. These assets were clean before the Editor tests and have no authored changes in this ticket. Their exact generated diffs are preserved. Both exact files were restored from HEAD after Editor exit and their diffs are empty. No unrelated font assets changed.
- Root owns Android/ADB and final native acceptance. No APK, physical-phone, speech, blind private-card access or human-group enjoyment/balance claim is made by this ticket.
