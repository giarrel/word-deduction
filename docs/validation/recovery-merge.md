# Recovery integration check

Date: 2026-10-06. The separate merger integrated approved Recovery HEAD `89b7b3866801a931663ce2509f048f75958dc132` into `integration/android-v1` through no-ff merge `63381f762e23b2a97ec5ac19ecb7e018a4b09af2`. Both checkouts were clean. The ticket already contained integration base `7d6af6be61ffa57826cdaee2dd44ad8f714a7dbd`; the merge had no conflicts.

The integrated production Session sources passed **six selected scenarios**, recorded in [session-integration-targeted.log](evidence/recovery/session-integration-targeted.log):

- Every Classic phase, both White judgments, unchanged saved bytes/history across reopening and interrupted writes, rejected uncommitted abandonment, and actual Windows sharing-lock failures followed by successful retry.
- Every Quick phase reopening without a redraw or visible card.
- V2/V3 deal migration and the V3 Classic pending White guess continuing into V4 without rewriting on open or changing the saved deal.
- Unknown newer schemas in both the primary and backup slots remaining blocked and preserved.

Commands, executed sequentially to avoid overlapping test executables:

```powershell
dotnet run --project tests/Session.Tests '-p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/worktrees/recovery/game/Library/PackageCache/com.unity.nuget.newtonsoft-json@74deb55db2a0/Runtime/Newtonsoft.Json.dll' -- 'every Classic phase'
dotnet run --no-build --no-restore --project tests/Session.Tests -- 'every Quick phase'
dotnet run --no-build --no-restore --project tests/Session.Tests -- 'V3'
dotnet run --no-build --no-restore --project tests/Session.Tests -- 'newer'
```

The genuine Unity package DLL and its test-output copy both have SHA-256 `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`. The earlier complete Session and **19/19** combined PlayMode results remain the implementer's evidence in [recovery.md](recovery.md); the merger did not repeat the long content cycles or launch an Editor.

The entire `game/` and `tests/` trees match approved ticket HEAD `89b7b386`. The entire `game/` tree also matches APK build source `9ba4374c38c77969cfdcf3b208ab1724afe775f2`. Both comparisons used `git diff --exit-code <commit> HEAD -- <paths>` and returned zero. No production change was introduced by the merger.

Full `git diff --check 7d6af6b HEAD` reports only six empty-value trailing spaces in Unity-generated `game/Assets/Plugins.meta` and `game/Assets/Plugins/Android.meta`. All other files pass. Those metadata bytes and the accepted source tree were preserved.

The exact combined development APK, build summary and identity are preserved in ignored `artifacts/recovery-baseline/`. The copied APK is **37,651,751 bytes**, SHA-256 `6284ef8a413638feb75182462c256103d3ee4a6bccede2a47d4a1abe830c0a92`; size and hash were verified. The original summary and documented diagnostics retain the Pipeline timeout/error count; this report does not relabel it as a zero-error build.

Root's combined Android gate was still in progress during this integration check. This report makes no additional native acceptance claim. No ADB/Unity operation, issue closure or worktree cleanup was performed by the merger. The checked integration branch is authorized for push; root records the separate Android evidence and final issue decisions.
