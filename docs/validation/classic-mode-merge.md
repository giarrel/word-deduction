# Classic mode integration check

Date: 2026-10-06. The separate merger integrated final ticket commit `9187444bd0e6c39eca5bffad8a6ed70c3353b329` into `integration/android-v1` with no-ff merge `3af5b7d17dcaaadad4b2f87bc7ad2b06c52c840e`. Both checkouts were clean before merging. The ticket already contained integration base `79e4a7c1251951268becba60f02756b4c4e2f606`; the merge had no conflicts.

The integrated production Session sources passed **29/29** behavioral scenarios in a single .NET run, including the existing group/Quick checks and the new Classic cases. The existing reference override used the genuine Unity Newtonsoft DLL from the Classic worktree:

```powershell
dotnet run --project tests/Session.Tests '-p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/worktrees/classic-mode/game/Library/PackageCache/com.unity.nuget.newtonsoft-json@74deb55db2a0/Runtime/Newtonsoft.Json.dll'
```

The complete result is [session-integration-green.log](evidence/classic/session-integration-green.log). The referenced assembly and its copy in the test output both have SHA-256 `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`. No alternate serializer, fabricated package cache or second Unity process was used. After worktree cleanup, resolve the pinned package locally or pass another existing resolved Unity DLL.

The complete ticket range passed `git diff --check 79e4a7c HEAD` without exclusions. The entire `game/` tree is byte-identical to both final ticket commit `9187444` and recorded APK build source `9c05948299b04ed93b236b03430600c765f44465`, verified with `git diff --exit-code <commit> HEAD -- game`. The merger made no changes to production sources, settings or tests.

The accepted APK was copied specifically from `WordDeduction-classic-accepted.apk`, avoiding the default output overwritten by the redundant unchanged-source build described in [classic-mode.md](classic-mode.md). The preserved copy in ignored `artifacts/classic-baseline/` is **37,583,470 bytes**, SHA-256 `abc6e923f0fc7ad124d8984475923c402a9cadb22b3bb9c4c5e1bb92ff99ef8c`; both values were checked. The corresponding committed build summary and APK identity were copied alongside it, retaining their original build-time paths and source revision as provenance.

The existing **14/14** PlayMode result and focused preference rerun remain the implementer's evidence. No Unity or Android operation was run by the merger. Root owns the separate native Android acceptance; this report makes no claim that those parallel runtime checks have completed. No push, GitHub mutation, worktree cleanup or global configuration change was performed.
