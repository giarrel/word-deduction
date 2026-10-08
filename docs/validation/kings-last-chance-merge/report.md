# Kings last-chance integration — ticket #14

8 October 2026. The separate merger integrated accepted branch tip `d03b31eab35e057add1b40d23929d4961b6d9f76` locally into `integration/kings-v1`, whose prior tip was `7d20ae1ea72bf622adb5b3cc98b8783abb1e73f8`. No-fast-forward merge **`9f0980d1a32b3ec76bfcea17f041c00ddc4054f1`** completed without conflicts or source edits. Its complete tree `0c3e5f88671af7045348303a3b924a9a2b6a47a4` is identical to the accepted branch tip.

## Independent Session validation

**71/71 passed**, 71 individual PASS lines, zero FAIL lines, process exit code 0. The full unfiltered run used the actual merged integration sources, the preserved production Newtonsoft DLL and `WD_TEST_IO_DIAGNOSTICS=1`, with fresh process-local DOTNET_CLI_HOME/TEMP/TMP under `artifacts/kings-last-chance-merge/`.

```powershell
dotnet run --project tests/Session.Tests/Session.Tests.csproj -p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/outputs/word-deduction/artifacts/repro-runtime/Newtonsoft.Json.dll --no-launch-profile
```

The run lasted **155.189 seconds**, from `2026-10-08T18:53:36.1153675Z` to `2026-10-08T18:56:11.3041640Z`. Exact combined stdout/stderr, including the SDK first-run PATH-registration warning, is preserved in `artifacts/kings-last-chance-merge/session-independent.txt`, SHA-256 `23e4fc6a916235fde081bf0ce629e5d0dc7280bbc2fd1c9e6ed3f2cf828bcda6`. `session-independent-run.json` records command, environment, timing and exit code. The production runtime DLL was verified as `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`.

## Rendered provenance and preserved evidence

The completed [rendered result](../kings-last-chance/evidence/12-rendered-full-result.json) was independently parsed: **44/44 passed in 207.16 seconds**, zero failed/skipped/inconclusive cases. Its hash is `adb28fa47a92f31ecca61d618f6be079130162fab69fd14848aaba71bda8d217`. The `game`, `tests`, `content` and `third-party` Git trees are identical between rendered source `dd9fab75313da403425c6e82fd024ec9aef2a5e6` and the integration merge; exact tree hashes are in [evidence.json](evidence.json). Root inspected actual standard and large-text frames. The merger did not rerun Unity or claim Android acceptance.

All **109 raw files / 10,142,383 bytes** in the [implementation manifest](../kings-last-chance/manifest.csv) were copied into the primary checkout's ignored `artifacts/kings-last-chance/`. Source and destination byte lengths and SHA-256 values were checked independently against the manifest. The destination inventory contains exactly those 109 files. All 27 tracked observation copies were also verified against their raw manifest entries. No destination files existed before copying; no unrelated evidence or source worktree was removed.

The original manifest hashes to `2fb77fb2cefea6834e84b827b54d0f0d0863d6e0013419e2de63ce86a8740404`. `artifacts/kings-last-chance-merge/preservation.json` records all paths and hashes; the merger manifest pins that record and independent output. Editor logs, screenshots, failed snapshots and generated-cache diagnostics remain preserved locally; no cache or build artifacts were added to Git.

The original **70/71** full Session observation remains unchanged: the existing public-projection test rejected a top-level private word field. The implementer replaced it with phase-scoped `Judgment` disclosure, retained the old assertion, and obtained 71/71 before this independent passing run. This diagnosed interface regression is distinct from the earlier foundation intermittent failures, whose causes remain unconfirmed.

## Checks and handoff

`git diff --check` passed in the source worktree and merged checkout using the committed evidence-specific attributes, which preserve captured CRLF/console bytes. A pre-merge check from the older checkout lacked those incoming attributes and flagged the captured bytes; no raw observation was normalized or rewritten. Only this merger report and manifest were added after testing.

Merger/report commits use per-command giarrel noreply identity without changing shared configuration. No Unity, ADB, Android build, push, escalation, issue closure or cleanup occurred. Root owns GitHub, #16 native acceptance and final release validation; native CJK/accessibility/device evidence is not inferred from these Editor and Session results.
