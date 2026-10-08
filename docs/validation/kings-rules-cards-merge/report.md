# Kings rules and cards integration — ticket #15

8 October 2026. A separate merger integrated accepted branch tip `c63cf67cc218a71f91c8f023fe72c7e6c9e592be` into `integration/kings-v1` locally. Previous integration `4217440823ce78c4c25ef356d8e3e4f9f542887e` was already an ancestor. No-fast-forward merge `ca8e2f10b7529d5950d650f8f33c6d58ab0d07ed` completed without conflicts and has the identical complete source tree `75412735fb3d20eea13f2aad165ce34a411355a2`.

## Evidence and rendered validation

The merger checked all **80 selected files / 1,980,675 bytes** against [manifest.csv](../kings-rules-cards/manifest.csv) in both the source worktree and merged primary checkout. It also independently verified all **87 raw files / 22,437,537 bytes** in the source and existing ignored primary backup `artifacts/kings-rules-cards-ticket15/` against [raw-manifest.csv](../kings-rules-cards/raw-manifest.csv). Both destination directory inventories match their manifests exactly. Existing verified raw streams were retained without overwriting; Editor logs and generated-font evidence remain in that local backup.

The completed [full rendered result](../kings-rules-cards/evidence/rendered-full.json) records **41/41 passed in 209.87 seconds**, zero failed, skipped or inconclusive cases. Its SHA-256 is `757deefa049c5311bc4c4c8febea7bc6878012af911c8be59d10eb1bc7575392`. Runtime UI tree `08e849f1bf9c5f5c4ceef637edaccb1c18ad9691` is identical in the accepted implementation and integration. The coordinator inspected actual bilingual help/card/word/setup captures and the header pixel comparison. The apparent missing H remains a resolved inspection discrepancy, with no inferred production-font fix.

## Session validation reused with unchanged inputs

The independent **66/66**, exit-0 diagnostic Session run on merge `663f82f4dd1daa544f3beec0ae459923275fa0f1` remains applicable. These Git input trees are identical in that tested merge, pre-merge integration, accepted ticket #15 tip and this integration merge:

| Input | Git tree |
| --- | --- |
| Production Session | `2aa54ec2ec2d92bbc729b04255c8d5ddac41b116` |
| Session project, tests and fixtures | `5746906388c3bffa3284f6d2fb022cfa4f2e88f8` |
| Content | `fae305a03886bbde4c514b9ab4606b56be478fdd` |
| Unicode conformance data | `60dfecd06cdc211dcbca878ec227edd563803b32` |

The preserved log was rechecked against SHA-256 `67d5999712508a6e486b6bbf24416a9b9e0ed83e9777c6c4d2513debd2b218dd`, its successful command metadata and nonzero 66/66 summary. The production Newtonsoft DLL still hashes to `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`. See [the independent #13 report](../kings-elimination-merge/report.md). No Session rerun was needed for this UI-only integration; the implementer's targeted 6/6 result is also retained. Earlier foundation intermittent failures remain undiagnosed as recorded there.

## Integration correction and remaining gates

The initial diff check found one extra blank line at EOF in `KingsRulesScreenTests.cs`. The merger removed only that final blank line; the final combined diff check passes. This formatting correction and merger documentation are the only additions after the tested source. Runtime behavior and test assertions are unchanged, so no rendered rerun was warranted.

Native CJK system-font fallback remains a coordinator/#16 gate; Windows Editor evidence does not establish Android acceptance. No Unity, ADB, build, push, escalation, issue closure or cleanup occurred during this merge. The coordinator owns push and closure. Merger/report commits use per-command giarrel noreply identity without changing shared Git configuration. [Merger evidence.json](evidence.json) pins the verification record, hashes and reused-test provenance.
