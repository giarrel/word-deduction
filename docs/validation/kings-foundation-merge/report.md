# Kings foundation integration — ticket #12

8 October 2026. A separate merger integrated the completed foundation into `integration/kings-v1` and independently validated the combined Session sources. This accepts the ticket #12 integration scope; Kings elimination, last chance, full rules/readability and Android acceptance remain with #13–#16.

## Source provenance

- Fixed pre-implementation review baseline: `478953f496c0a8e88f6849c32960f02c2039f671`.
- Previous integration tip: `bcdbb1389bea14b6e38e0122649380eafef04c1e`.
- Implementer commit: `7b695699b2ac29d8222fbf8580e805c6ce410f9b`, branch `implement/kings-foundation`.
- No-fast-forward merge: `eb8538390d430f0089c5fcfb9f7377e4b2186f98`; parents are the previous integration tip and the implementer commit. The merge completed without conflicts or source edits.
- Both the implementer and merge commit have the identical complete tree `c4faf8f0874c7f979cba84c3a1e8640f7c8eaa7a`.
- Author and committer on the implementer and merge commits are `giarrel <126280217+giarrel@users.noreply.github.com>`.

## Evidence preservation

All **31 files / 1,045,510 bytes** named by [the implementer manifest](../kings-foundation/evidence.json) were copied from the foundation worktree to the integration checkout's ignored `artifacts/kings-foundation/`. Every source and destination file was checked against the manifest's byte length and SHA-256. Destinations were absent before copying; no unrelated files were overwritten and the implementer worktree remains available.

The preserved files include failed observations, later passing observations, the full rendered result, Editor log and source patch. The manifest itself hashes to `6b378f3ca462d16ec70ee5b5435f7dbc7b2d23e2d2f906eca31b1ffbbdc7ce8e`. The path-by-path preservation record is `artifacts/kings-foundation-merge/preservation.json`; its hash and the independent-run hashes are recorded in [evidence.json](evidence.json). Raw evidence is retained locally in ignored artifacts; these tracked reports and manifests make its identity reviewable.

## Independent validation

From the actual merged integration checkout, the merger ran:

```powershell
dotnet run --project tests/Session.Tests/Session.Tests.csproj -p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/outputs/word-deduction/artifacts/repro-runtime/Newtonsoft.Json.dll --no-launch-profile
```

- **61/61 passed**, with 61 individual `PASS` lines, no `FAIL` lines and process exit code 0.
- Started `2026-10-08T17:00:11.6000894Z`; finished `2026-10-08T17:02:29.0751149Z`; wall duration **137.475 seconds**, including SDK startup/build.
- `WD_TEST_IO_DIAGNOSTICS=1`; process-local `DOTNET_CLI_HOME`, `TEMP` and `TMP` were under `artifacts/kings-foundation-merge/`. A failure would retain its test snapshot and emit available host IO diagnostics. No failure occurred in this run.
- The production Newtonsoft DLL SHA-256 was checked as `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`.
- The complete combined stdout/stderr is preserved in `artifacts/kings-foundation-merge/session-independent.txt`, including the SDK first-run PATH-registration warning; command, environment, timing, exit code and output hash are in `session-independent-run.json`.
- `git diff --check` passed for the merged change. Only this integration report and its manifest were added after testing; no gameplay or test sources were changed by the merger.

The preserved implementer rendered result was parsed and confirmed as **33/33 passed, 0 failed/skipped/inconclusive, 141.34 seconds**. It applies to the identical merged tree above. The merger did not rerun Unity, build an Android artifact, operate ADB, or claim native acceptance.

## Earlier failures remain unresolved

The foundation evidence includes two earlier full Session failures:

- `session-mid.txt`: **58/59**, `Quick deals are durable and private for every supported group size: read and hidden card can advance`.
- `session-final.txt`: **60/61**, `all 520 bilingual pairs are dealt once per cycle across restarts and language changes: language changes between deals`.

The original assertions did not capture action-error/host-IO details. Focused repetitions and the implementer's later full diagnostic run passed; this independent full run also passed. These observations do **not** establish a root cause or a production fix. The raw failed outputs are preserved unchanged, and future recurrence must be investigated using the retained diagnostics rather than accepted through blind repeats.

Ticket closure and the next implementation frontier belong to the coordinator. Worktree cleanup has not been performed.
