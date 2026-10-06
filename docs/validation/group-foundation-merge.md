# Group foundation integration check

Date: 2026-10-06. The separate merger integrated approved ticket HEAD `44473f1873ddb40e6821fb9a67ab18197c58a179` into `integration/android-v1` at merge commit `318af96f17b2ce203fbf23be58ba20fcb380a06d`. Both checkouts were clean before merging, and the ticket already contained integration base `10ffa83ce7d2463121543688ec7d3ba8dfeb7fa7`. The merge had no conflicts.

The integration checkout has no Unity Library cache. A separate tooling change adds the optional `NewtonsoftJsonAssembly` MSBuild property; its default remains the local Unity package DLL. The following command compiled the merged production Session sources and passed **13/13** behavioral checks, recorded in [session-integration-green.log](evidence/group-foundation/session-integration-green.log):

```powershell
dotnet run --project tests/Session.Tests '-p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/worktrees/group-foundation/game/Library/PackageCache/com.unity.nuget.newtonsoft-json@74deb55db2a0/Runtime/Newtonsoft.Json.dll'
```

This path was available during the merge; after worktree cleanup, import the pinned packages in the current checkout or supply another existing resolved package DLL. The referenced and copied test-output DLLs both had SHA-256 `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`. No NuGet substitute, fabricated PackageCache, second Editor start, Unity test rerun or Android operation was used for this integration check. Existing **5/5** PlayMode and accepted Android results remain the evidence from [foundation validation](group-foundation.md).

`git diff --check` passes for the tooling patch. Across the complete ticket range, it reports only trailing spaces in Unity-generated `.asset`, `.meta`, `.unity` files and nine lines of the preserved original `first-runtime-logcat.txt`. Those generated values and raw evidence were retained. The remaining complete source/documentation range passes:

```powershell
git diff --check 10ffa83 -- . ':!*.asset' ':!*.meta' ':!*.unity' ':!docs/validation/android-group-runtime/first-runtime-logcat.txt'
```

The accepted APK and original build summary were copied to ignored `artifacts/foundation-baseline/` before removing any worktree. The APK is **37,513,698 bytes**, SHA-256 `181bfebbebca3c35198e1da29ebceb0d5c52aa33e2668f499a4fe4f730bd1cf2`; copied size and hash were checked. Its original summary retains the build-time worktree path as provenance. The optional test-reference path and this merge documentation do not change the game's sources or APK.

This merge accepts the saved-group foundation for issue 3. Gameplay and the remaining release checks are still pending. Push and GitHub issue updates are handled by the root agent.
