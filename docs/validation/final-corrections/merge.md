# Final correction merge

7 October 2026. The separate merger integrated `implement/final-corrections` at `1d4a758e144dc6206927b0d75f30770d3ec8ef22` into `integration/android-v1` at `b22d4e0`, without conflicts. Merge commit: `2d0bc5dc16729e857ae54cf159c0756354355a2b`. No implementation was changed by the merger.

The merged `game`, `content`, `tools`, `tests`, and `third-party` trees exactly match clean build source `71e6f39e599727cd176e2545ee65d1ab692d12de`. The complete tracked diff against that pin contains only documentation and evidence. [Verification](verification.json) records the tree identities and independently checked package hashes.

The merger independently ran all **55 public Session scenarios: 55 passed, zero failed**. The run used the preserved `artifacts/repro-runtime/Newtonsoft.Json.dll`, SHA256 `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`, rather than a worktree package cache. [Actual output](session-green.txt) includes persisted names and Unicode conformance, both game modes, interrupted saves, migrations, and the full bilingual word cycle. The command was `dotnet run --project tests/Session.Tests/Session.Tests.csproj -p:NewtonsoftJsonAssembly=<absolute preserved DLL path> --no-launch-profile` from the integration checkout. No Unity build, rendered test rerun, or device operation was performed by this merger; those results remain in the [correction report](../final-review/report.md) and coordinator native reports.

Before cleanup, the complete correction `artifacts` directory was copied to the integration checkout's ignored `artifacts/final-release-corrections`. All **303 files, 1,476,113,904 bytes**, were SHA256 checked against both source and copy. This includes 302 handoff artifacts plus their inventory JSON. The [preservation manifest](artifact-preservation.json) records every relative path and hash; original logs, generated packages and intermediate failed tests remain available.

| Code4 package | Preserved path under `artifacts/final-release-corrections/` | Bytes | SHA256 |
|---|---|---:|---|
| APK | `android/71e6f39e5997/20261007T022033606Z-apk/WordDeduction-local-release.apk` | 37,422,486 | `2f4523b0cbfe87379997bbaecfd4a78e21e9ae4afc09c79fc9baf3b20b1a9a97` |
| AAB | `android/71e6f39e5997/20261007T022558040Z-aab/WordDeduction-local-release.aab` | 37,779,435 | `ae6ba0061f53fb7e0ba2da538569105ad32e991cbeaaf344c629012fb0865ae7` |

Both version 1.0.0/code4 packages match the build summaries and use the local debug certificate. Production signing and Play acceptance are not established by these checks.

Only `work/worktrees/final-corrections` was removed after verifying its clean tracked state, merged retained branch, closed target Editor and artifact preservation. Git unregistered it but encountered Windows long paths; native PowerShell removed the exact checked residual directory after another hash verification. [Cleanup record](cleanup.json). The `implement/final-corrections` branch is retained; other worktrees belong to the coordinator's cleanup.
