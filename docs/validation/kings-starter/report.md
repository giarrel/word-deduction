# Kings round starters — ticket #22

9 October 2026. Branch `implement/kings-starter`, based on integration `3b22b2947e4380def13fd017834205c53313f358`. The starting runtime/test tree matched the validated #21 source `30ec5b14fe1dafea7adb4ca81b349e047633d194`. [Spec](../../specs/role-counts.md), [ticket](../../specs/role-tickets/22-kings-starter.md). Root retains tracker updates, native device acceptance, release builds/versioning and final integration.

## Implemented

Kings exposes its frozen starter after handoff. Each ordinary nonterminal elimination atomically chooses a living starter and advances the clue round. Optional Next clue round handles a round without elimination; selecting the next eliminated person stays directly available. Every survivor, including White, is eligible and repeats remain possible. Opening, selecting and cancelling do not reroll. New rendered callbacks carry match identity, round and elimination count; stale/duplicate actions and unsafe phases reject without progress.

The existing table owns the complete interface: a small round label, public starter card, neutral elimination notice and participant choices in one scroll area, plus the optional action below. No new mandatory page, ballot, discussion tracking or private-role disclosure. German/English help and interface/Kings documentation reflect app-assisted starters.

Validated old Kings data with an eliminated unused starter receives the first surviving participant as an in-memory fallback. No random draw or disk write on Open. New Kings transitions carry the V6 rules marker, allowing continued old matches to reopen after later rounds. Malformed ongoing new states with eliminated starters and invalid round shapes recover/block through the existing storage contract.

## Behavior evidence

Agreed seams: public Session with actual saved-session directories, and production rendered UI/input. Production and full-suite test pin: `e7fa596b2ecead1a5f7cdb2c7d1f962e238aa952`.

- Initial public starter test first failed because Kings hid its saved suggestion. An early test revision incorrectly assumed six RNG calls despite the word-history shuffle; the final test observes no draws after the deal instead of coupling to a call count. Both outputs remain retained.
- Guarded elimination overloads and NextKingsRound first failed to compile; after implementation the tests cover atomic next starter, unchanged selection/cancel, retries, all seven candidate seats including White, repeated starter and stale identity/round/progress.
- Authentic V5 fixture failed on the dead Ada starter. After migration it reopens twice with Bea (White), with both generations byte-identical. The test then writes V6 through Next, reopens, selects/confirms another ordinary participant, reopens again and finishes by eliminating the good King. Original match ID, group/order/language, Peach/Apricot words and White identity remain correct. Fixture provenance/hashes are retained under tests/Session.Tests/Fixtures.
- Unsafe handoff/last-chance/result/base-mode actions and malformed checksum-valid state recovery pass through the same public seam. Existing six Kings end paths are retained.
- Rendered DE/EN starter/next test first failed because the table lacked the starter label; it passes after UI integration, including selection/correction, detached old action, ordinary elimination, public accessibility, restart and absence during last chance.
- Small 360×616 / 150% test exposed an actual overflow: the participant viewport extended to y783 underneath the fixed action at y522. There is no screenshot of this first failure because the assertion stopped before capture; its exact result is preserved. Moving the starter/notice/instruction into the existing scroll area fixed it. Visual review then made the round label unobtrusive and added checks that the entire long duplicate name can be read. Text is enlarged only once; final metrics assert actual 19.5 px caption and record 39 px starter, 22.5 px action text.

## Validation status

Full Session: **93/93 passed**. Full rendered PlayMode suite: **66/66 passed**, zero skipped/inconclusive, 216.91 seconds. Both ran at the pinned production/test source. Subsequent work only adds an End-of-frame barrier to the test fixture screenshot capture; no runtime code changed. Its targeted rerun and immutable final screenshots are recorded at handoff.

## Environment and limits

Pinned Unity 6000.3.25f1 Editor 252884 was launched hidden in normal user context for this exact worktree. The explicit project-targeted CLI reached Pipeline 7800; a generic status call reported no instance even though explicit commands succeeded. No installation or account change was required. No ADB, package install, version/signing change, release build or unrelated Unity process was touched.

Generated font-asset changes are runtime artifacts and excluded from the implementation. Editor logs and all intermediate outputs stay in artifacts/kings-starter for Root preservation. The work proves model and Editor behavior, not Android update acceptance, physical-phone smoothness or real group enjoyment. Native acceptance belongs to #23.

## Final acceptance and handoff

- Session **93/93**: [full output](evidence/09-session-full.txt). Production source equals `e7fa596b2ecead1a5f7cdb2c7d1f962e238aa952`.
- Complete rendered suite **66/66**, no failures/skips/inconclusive, **216.91 s**: [full result](evidence/10-ui-full-result.json).
- The only subsequent test change waits for End-of-frame before reading screenshot pixels. Targeted complete KingsEliminationScreenTests **7/7**, **9.33 s**: [result](evidence/11-capture-result.json). No production source changed after the full suite.
- Final Editor compile status completed with no errors and Editor ready/stopped: [compile](evidence/12-compile-final-status.json), [state](evidence/12-editor-final-status.json).
- Final captured pixels and SHA256 manifest: [screenshots](evidence/screenshots/manifest.json). Implementer viewed DE/EN small top and last-participant captures plus the ordinary English table, including original-resolution reads. Starter names/suffixes, public list and Next are reachable. Root independently inspected the same small DE/EN views before handoff.

Some inline image previews appeared to omit the H in Help/Hilfe. This was not a changed file or confirmed product defect: the English top file has identical SHA256 `1f8af0aec04a02941f0991868f7ed1a8067d04074230d5ca69beb68fea4fbaad` before/after the capture barrier, and an original-resolution read displayed the full label. Direct PNG pixel inspection confirms both H stems and crossbar at x274–286 in English top/last and German top, retained in [pixel evidence](evidence/12-header-pixels.jsonl). No production layout or font change was made for that preview discrepancy. Earlier captures and their manifests remain separately under artifacts/kings-starter/pre-visual-correction and after-full-before-frame-fence; the original genuine overflow is retained as a failing geometry test, not a fabricated screenshot.

Only owned Editor252884 was stopped after checking its exact executable/project/owner; [exit](evidence/12-editor-stopped.json) confirms termination. Its generated Inter and Emoji font diffs were preserved in ignored artifacts/kings-starter/12-generated-font-diff.txt and 12-generated-emoji-diff.txt, then both assets were restored to tracked source. No runtime font changes are included. Root may now reuse the Unity lease. Full Editor logs/intermediate output remain under artifacts/kings-starter and should be archived before removing the implementer worktree.

No scene, prefab, package, PlayerSettings, signing, Android device or production setup change is required. Native Android installation/update, final independent reviews and release delivery remain Root-owned ticket #23 work.
