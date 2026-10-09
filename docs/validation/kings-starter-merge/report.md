# Kings starter: separate merge and preserved evidence

9 October 2026. Handoff **`e76c702648d4c7324041487e655e3e6e50f7bab5`** merged without conflict into `integration/role-counts-v1.3` from `3b22b2947e4380def13fd017834205c53313f358`. Merge commit: **`465ebe9712a15a2014d3eedaf736663a630b1b51`**. Both checkouts were clean; handoff already contained the integration base. No source changes or conflict resolutions were performed during merging.

## Exact source and test coverage

The merge's complete `game`, `tools`, `tests`, `content` and `third-party` Git trees equal the handoff. All production inputs match full-suite pin **`e7fa596b2ecead1a5f7cdb2c7d1f962e238aa952`**. The [verification](verification.json) records tree IDs and the complete production pathspec comparison; the [exact post-suite delta](post-full-suite-delta.patch) preserves the two exceptions:

- One `yield return new WaitForEndOfFrame();` in `KingsEliminationScreenTests.Fixture.Capture` before reading screenshot pixels.
- Spacing/readability edits in the authentic V5 fixture's Markdown provenance. No fixture JSON or Session-test code changed after the full run.

The [implementer report](../kings-starter/report.md) records **93/93 Session** and **66/66 full rendered PlayMode**, zero skipped/inconclusive, **216.91seconds**, at the full-suite pin. After the screenshot-only test change, the complete affected KingsEliminationScreenTests passed **7/7 in9.33seconds**. This is a full run plus a targeted post-change run, not a claimed new66-test run on the final test file. The merger ran no tests or Editor/device operations.

## Preservation and exact hashes

[Durable archive](../../../artifacts/kings-starter-preservation/e76c702648d4-20261009T185347Z/): **143 original files /26,161,978bytes**, including both complete raw directories `artifacts/kings-starter` and `artifacts/kings-elimination`, plus the implementer's working validation documents. All original failure output, prior capture sets, capture manifests, Editor log and generated-font diffs are preserved. Source inventory was stable and every raw source/destination byte count and SHA256 was verified.

Canonical committed Git blobs are separately preserved/inventoried for55 validation files. **28** match working bytes exactly; the remaining **27** are text files equivalent after CRLF-to-LF normalization. Original byte hashes and canonical hashes are recorded separately in [preservation.json](preservation.json); normalization is not described as an exact-byte pass. All **10 final catalogued PNGs** independently match the captured raw bytes, committed Git bytes and final screenshot manifest SHA256 exactly.

## Visual evidence boundaries

Root and the implementer inspected final DE/EN small top/last-participant views and reported readable/reachable starter names, public choices and Next. The merger does not claim additional image inspection. The initial genuine overflow failed a geometry assertion before capture; its failed output exists, and no screenshot was invented for it.

Root additionally re-opened the immutable DE/EN top PNGs at original resolution. Because inline previews were inconsistent, Root independently read the English True-top PNG region x270–289/y66–89 and confirmed both complete H stems and its crossbar,17pixels high. English True-top SHA256 is `1f8af0aec04a02941f0991868f7ed1a8067d04074230d5ca69beb68fea4fbaad`, unchanged across the capture barrier. This is **no confirmed product clipping defect**. The test's end-of-frame barrier is not asserted to be a demonstrated cause/fix of a product rendering bug; no production UI/font change followed that preview discrepancy.

## Remaining work

Tickets #21 and #22 are integrated. Independent Standards/Spec reviews, final native/update acceptance and release APK/AAB verification remain ticket #23. No package, signing, installed app, remote branch, issue or worktree cleanup was changed by this merger. Existing emulator/physical-device and performance limitations remain.
