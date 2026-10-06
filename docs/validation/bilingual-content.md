# Bilingual content — issue 6 validation

Date: 2026-10-06. Worktree `ticket/6-bilingual-content` started at integration commit `6c12490`. Canonical scope: `docs/specs/tickets/06-content.md`. Public Session action/view and rendered app inputs are the authorized test seams; the work uses implement-spec, tdd and codebase-design.

## Content and storage

The app ships 520 original bilingual pairs, 20 themes and 1,038 distinct normalized complete terms in each language. Each complete displayed phrase counts as one term. Mechanical validation has zero duplicate IDs, missing translations, identical sides or normalized duplicate/reversed pairs. See `content/validation-report.json`, the complete editorial review and `content/README.md` for provenance, maintenance and the frozen SHA-256. The compiled Session catalog exactly matches the authored JSON; there is no download or language pack choice.

Session persists a shuffled remaining deck, consumed pair IDs and the last ten deal IDs in envelope V4. Both languages use one shared pair cycle. The last ten bilingual concept pairs supply normalized recent words; selection scans the remaining deck once for an unused pair without those terms. If none fits, it draws a remaining pair anyway. The pair cycle always takes precedence and rolls over only when every eligible ID has been consumed; the previous pair cannot immediately repeat at rollover. Side swapping remains random, and concrete words and roles remain part of the immutable deal.

Deal and history changes share Session's atomic snapshot transaction. Failed saving publishes neither. Abandoning consumes a deal that was already successfully saved; loading, revealing, handoff and language changes consume none. Missing/inconsistent V4 history is rejected before any word can be revealed. V1–V3 state receives fresh history in memory; an existing match's pair is seeded as consumed/recent. Opening an older state does not rewrite its files or alter words, names, roles or language. Before the final integration merge, Classic's V3 migration must be retained.

## Session evidence

The production sources compile against the genuine Unity Newtonsoft assembly using the documented `NewtonsoftJsonAssembly` override. `session-green.log` reports 26/26 passing scenarios before the additional termination fixture; `04-termination-green.log` passes that fixture separately. A final suite is required after merging Classic.

Recorded red/green slices under `evidence/bilingual`:

- `01-cycle-red.log`: the second deal reused a pair. The implementation then passed 1,040 deals across two full cycles, with a fresh Session and alternating language for every deal. Its first full run also exposed a brittle old test fixture that replaced a literal V2 envelope string; the fixture now edits the envelope's version structurally. That earlier run's 21/22 summary is not claimed as suite success.
- `02-recent-red.log`: a shared rhino term repeated while hundreds of alternatives remained. The passing run avoids recent words across alternating languages and restarts.
- `03-history-red.log`: a checksum-valid deal without its consumed pair was accepted. The passing run recovers the validated previous group and exposes no word from that invalid deal.
- Supplementary scenarios verify failed-save retries consume no pair; V2/V3 current deals migrate without rewriting on open; and a deliberately ordered full cycle ends with both rhino pairs. The final pair is still dealt despite the unavoidable recent term, and the next cycle avoids its immediate repetition.

## Rendered evidence and limits

Unity 6000.3.25f1 imported and compiled this worktree normally. Its own Editor used PID 159968 / Pipeline port 7801; all commands explicitly targeted `bilingual-content/game`. The six-term PlayMode scenario passed in 78.36 seconds with the production panel and font: Chocolate hazelnut spread, Pedestrian traffic light, Blood pressure monitor, Rollkragenpullover, Blutdruckmessgerät and Schlittschuhlaufen. Every fixture searches ordinary Session deals, dispatches actual UI Toolkit hold/release events, measures nonzero glyphs and card bounds, checks the visible Next action, and verifies synchronous word erasure. See `05-production-card-run.json`. An earlier blank-PanelSettings fixture also passed, but its missing-theme warnings prompted replacement with a clone of the real production panel; that preliminary pass alone is not the font/layout evidence.

The actual App scene was captured at 360×640. All four screenshots were opened and inspected: [long English phrase](images/bilingual/01-long-en-before.png), [covered German card](images/bilingual/02-covered-de.png), [long German compound](images/bilingual/03-long-de.png), and [German help](images/bilingual/04-help-de.png). The phrase wraps across three lines, the compound across two, with complete readable text, ample card padding, a long owner name and visible actions. Help remains scrollable. The fixture locates the target through deliberate public reveals, so its later covered card has already been read. No presentation fix was needed for these terms. The Editor was closed normally afterward.

The supplementary copy validator found 92 complete DE/EN entries with matching placeholders before Classic integration, plus translations for all 72 directly referenced UI/error/role/outcome keys. The full copy was read editorially; Quick's prior complete DE/EN rendered flow and this long-content pass supply complementary evidence. Classic adds its own copy and rendered paths; rerun this data validator after that merge.

Root explicitly scheduled one combined Android build after accepted Classic, content and recovery integration. That actual app acceptance remains outstanding and issue 6 stays open until root completes it. This task did not build or install a content-only APK. No font repair, native accessibility or group playtest result is claimed here; root owns ADB and ticket 8 owns the known font/touch repairs. The catalog's ten age/region review candidates remain documented as human vocabulary-review priorities.
