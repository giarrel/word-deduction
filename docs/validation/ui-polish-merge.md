# UI polish integration check

Date: 7 October 2026. The separate merger integrated accepted ticket #8 tip `8028ef65932f456c39c918dd5a71cdf7e0f02b06` into `integration/android-v1` through no-ff merge `24233437ffb4af9f1052b4288aa74b5af5ce5fc1`. Both checkouts were clean before the merge. The candidate already contained integration base `cd19d5fac24202f7fe6d80dd45083af77a2cf1a4`; there were no conflicts.

## Independent checks on the merged source

The full Session suite passed **53/53** on the integration checkout. Its [complete output](ui-polish/merger-session.log) includes durable group edits and capacity, Quick and Classic rule paths, recovery and failed writes, migration, the full 520-pair draw cycle across restarts/languages, recent-word avoidance and the Unicode scenarios. The Unicode conformance scenario explicitly executes all **766** unmodified official Unicode 17 cases. Legacy V4 names, 24/25 extended graphemes, malformed UTF-16, invisible-only inputs and line separators also pass through the public Session/name boundary.

Command, run from the integration checkout:

```powershell
dotnet run --project tests/Session.Tests '-p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/work/worktrees/ui-polish/game/Library/PackageCache/com.unity.nuget.newtonsoft-json@74deb55db2a0/Runtime/Newtonsoft.Json.dll'
```

The Unity package DLL and the test-output copy both have SHA-256 `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`. The Unicode test fixture and its output copy both match the published/pinned SHA-256 `e2d134d2c52919bace503ebb6a551c1855fe1a1faec18478c78fff254a1793ec`. With Windows CRLF normalized to LF, the generated property source matches its recorded SHA-256 `789b0fe76f8777e808585ab530c72aafd99719f89b48149ac3a11117d984fb3e`.

## Source and artifact provenance

`git diff --exit-code 8028ef6 HEAD` returned zero immediately after the merge: the entire merged tree matched the accepted candidate. Comparing APK source `7404d17826e7dd2e447e6dacbe1ea2d5770ce358` against the merge for `game/`, `tests/`, `content/`, `tools/` and `third-party/` also returned zero. The final candidate commit adds only documentation/evidence. This merger adds no production change and does not rebuild the accepted APK.

The preserved development APK in ignored `artifacts/ui-polish/WordDeduction-polish-3.apk` was independently rehashed: **53,759,826 bytes**, SHA-256 `d4f75a4f60601c959fad9ae6ee9a1ece2b7c100cb74d6fb97c7f4f4aee9ec334`. Its summary reports Succeeded, one recorded Pipeline timeout error and two warnings; those diagnostics remain preserved and are not presented as a zero-error build. This is a development validation artifact, not the ticket #9 release candidate.

Full whitespace checking reports only Unity-generated metadata/serialized empty-value lines. Authored content passes after excluding `*.meta`, `Emoji.asset`, `ProjectSettings.asset` and `UIToolkitProjectSettings.asset`. The upstream Unicode fixture's original bytes retain their existing `.gitattributes` exemption. No accepted asset was rewritten for whitespace.

## Previously executed rendered and native evidence

The merger read the [implementer's report](ui-polish/report.md), [visual review](ui-polish/visual-review.md) and [coordinator's native record](ui-polish/native/coordinator-report.md). Structured evidence inspection confirmed the prior complete rendered suite's 25/25 pass and subsequent targeted passes of 6/6 polish, 5/5 group, final 7/7 enlarged-owner polish, 5/5 Quick privacy and 5/5 Recovery. These occurred at their documented source stages; they are not described as a single newly rerun final suite.

The coordinator accepted the final source after actual DE/EN enlarged long-owner hold/release and 40-saved-player capacity checks. Native keyboard Save, script/emoji rendering, TalkBack public controls, clipboard paste, lifecycle/privacy and original-data restoration are that coordinator's evidence. The merger performed no Editor or ADB operations and makes no additional native, physical-device, audible screen-reader, human-group or release-performance claim.

Ticket #9 retains the final two-axis review and release-only package, offline, performance, 16KB, signing and store gates. Issue closure and worktree cleanup remain with the coordinator.
