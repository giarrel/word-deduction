# Kings elimination integration — ticket #13

8 October 2026. The separate merger integrated the accepted elimination branch locally and independently validated the combined Session sources. Last-chance attempts, full rules/readability and Android acceptance remain with #14–#16.

## Provenance and merge

- Previous integration: `ed74667ea54b7c01c63c915860e4bddc01253cfe`.
- Implementation source: `30ac0c108f71f786840ae1aa16d7ceddd7dfe8ff`; implementation merged current integration at `40d7cf67117d99a2797dedd6de682e1891f886c6` and added validation evidence at branch tip `1019600def984b295b9fe1d23f53199165b13099`.
- No-fast-forward integration merge: `663f82f4dd1daa544f3beec0ae459923275fa0f1`, with parents `ed74667` and `1019600`. No conflicts or merger source edits.
- Implementation tip and integration merge have identical complete tree `7f1e0ce922e957b2337e4c0d88583b5d0d4857bd`; `game/` and `tests/` also match the original source commit `30ac0c1`.
- Accepted implementation history retains `Codex <codex@users.noreply.github.com>`. Merger commits use per-command `giarrel <126280217+giarrel@users.noreply.github.com>` identity; shared repository configuration was not changed.

## Preserved evidence

All **54 manifest-listed byte streams / 8,085,061 bytes** were checked against the implementation [hash manifest](../kings-elimination/hash-manifest.json), copied into ignored integration artifacts and checked again by byte length and SHA-256. The 25 raw artifact files retain their `artifacts/kings-elimination/` paths, including six PNGs, Editor/font evidence and failed-test snapshots. The other 29 source, documentation and observation streams are exact archival copies under `artifacts/kings-elimination-merge/manifest-files/<original path>`. Every destination was absent before copying; no unrelated files were replaced.

`artifacts/kings-elimination-merge/preservation.json` records the complete source-to-destination map. The original manifest SHA-256 is `462ea5c6c23d93d73f422aa5e1d51ba345a58bc8c2884841dac25ba94b98e578`. [Merger evidence.json](evidence.json) records preservation and independent-run hashes. The source worktree remains available; no cleanup was performed.

## Independent validation

**66/66 Session tests passed**, 66 individual PASS lines, zero FAIL lines, exit code 0. The full unfiltered run used the merged integration checkout, production Session sources, the preserved production Newtonsoft DLL and `WD_TEST_IO_DIAGNOSTICS=1`. Fresh process-local TEMP/TMP and DOTNET_CLI_HOME were under `artifacts/kings-elimination-merge/`.

```powershell
dotnet run --project tests/Session.Tests/Session.Tests.csproj -p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/outputs/word-deduction/artifacts/repro-runtime/Newtonsoft.Json.dll --no-launch-profile
```

The run lasted **143.216 seconds**, from `2026-10-08T17:46:33.0228310Z` to `2026-10-08T17:48:56.2388794Z`. Exact combined stdout/stderr, including the SDK first-run PATH-registration warning, is retained in `artifacts/kings-elimination-merge/session-independent.txt`; its SHA-256 is `67d5999712508a6e486b6bbf24416a9b9e0ed83e9777c6c4d2513debd2b218dd`. Command, environment, timing and exit code are in `session-independent-run.json`. The Newtonsoft DLL hash remains `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`.

Preserved completed rendered results were independently parsed: Kings **3/3 in 1.59 s**, Classic **4/4 in 4.00 s**, Quick **5/5 in 7.21 s**, each with no failures, skips or inconclusive cases. These apply to the identical merged source. The coordinator separately inspected the German ordinary/result/last-chance PNGs. The merger did not use Unity, ADB or Android builds. `git diff --check` passed; only this report and merger manifest were added after testing.

The two earlier foundation intermittent failures remain preserved and undiagnosed; neither recurred in this full run. The elimination implementation's corrected rendered-test scope mistake and its original failed observation are retained as documented in its report.

## Coordinator handoff

This integration is local only; the coordinator owns pushing and ticket closure. The earlier foundation push rejection was already resolved by the coordinator using authenticated public-repository ownership and push-permission evidence, and remote integration reached `3c6d6694ff694ec7742f0caf8642219c0fa0a07a`. It is not a continuing blocker. No push or escalation was attempted during this elimination merge.
