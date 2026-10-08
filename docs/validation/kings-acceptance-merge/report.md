# Kings candidate contribution integration — ticket #16

> Historical code5 candidate evidence. Final local acceptance is documented in the [code8 native report](../kings-native/report.md) and [final correction merge](../kings-final-corrections-merge/report.md). Original observations below are retained unchanged.

8 October 2026. The separate merger integrated the recovery coverage and inspected local Android candidate. **#16 and #10 remain open** for coordinator native acceptance and final independent reviews; this report does not accept the complete release.

## Source and merge

Prior integration: `8150e8518b52b0212836437069e3ab841133debe`. Accepted implementation tip: `23d54c5abae564bd5333071ba8367def7fd935c6`. No-fast-forward merge **`42633ae4c0e3d384234377cc3aabf969b2560c0c`** completed without conflicts or source edits. Its complete tree `4e5992acbe3463317179ac0aeb4d19d88181bb57` matches the implementation tip.

The `game`, `tests`, `tools`, `content` and `third-party` trees exactly match APK/AAB source **`908d95fc80fc8173df02593422ff85264089b945`**; their hashes are pinned in [evidence.json](evidence.json). The package lock still hashes to `da98f245e8c511d47699dc883e521eff5176800341117fcfe8f77fe9dcfb342d`. The candidate is version **1.1.0 / code 5**. Only merger documentation was added after validation.

## Independent Session and retained rendered evidence

The unfiltered integration Session run passed **74/74**, with 74 individual PASS lines, zero FAIL lines and exit code 0. It used the production sources, preserved Newtonsoft DLL and `WD_TEST_IO_DIAGNOSTICS=1`; fresh process-local DOTNET_CLI_HOME/TEMP/TMP were under `artifacts/kings-acceptance-merge/`.

```powershell
dotnet run --project tests/Session.Tests/Session.Tests.csproj -p:NewtonsoftJsonAssembly=C:/Users/lucac/Documents/Codex/2026-10-06/sie/outputs/word-deduction/artifacts/repro-runtime/Newtonsoft.Json.dll --no-launch-profile
```

Timing: `2026-10-08T19:40:49.6445560Z` to `2026-10-08T19:43:29.5282660Z`, **159.884 seconds**. Exact combined output, including the SDK first-run PATH-registration warning, is `artifacts/kings-acceptance-merge/session-independent.txt`, SHA-256 `b2d2d6f5baa67bd4158ccc8e3658cb34d519169c2e78f3435c42dd460dc9f9da`. The adjacent run JSON records command, environment and exit code. DLL SHA-256 remains `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`. No failed run or blind retry occurred.

The retained complete [rendered result](../kings-acceptance/evidence/rendered-full.json) was parsed as **44/44 passed in 210.73 seconds**, zero failed/skipped/inconclusive cases. Its SHA-256 is `12a020bab9142c63312b7c86d322e7bc695d49a86423d978135345283089ee17`. Rendering preceded the intentional release-version setting change; production Session/UI/PlayMode and content/Unicode trees remained identical. The historical Info screenshot therefore shows 1.0.0, while the built candidate is 1.1.0. The merger did not open Unity or repeat rendered tests.

## Candidate artifacts and inspection

| Artifact | Preserved primary path | Bytes | SHA-256 |
| --- | --- | ---: | --- |
| APK | `artifacts/android/908d95fc80fc/20261008T191608690Z-apk/WordDeduction-local-release.apk` | 37,458,410 | `afd739cc5b8356a24ee9b2b92c7639ba128569d3f43090ac961fc0e6a3d24a01` |
| AAB | `artifacts/android/908d95fc80fc/20261008T192436613Z-aab/WordDeduction-local-release.aab` | 37,815,362 | `4b8aee0303ad77f5e4b39da1e6da9dcf8624a99005fe2bdda266cc58cc066d64` |

The original build summaries identify source 908d95fc, successful builds, zero errors and two warnings each. Retained final inspector summaries report **14 successful commands / 1 inspected package** for APK and **44 successful commands / 4 packages** for AAB; every recorded command exit code is zero. Artifact identities agree with the independently hashed preserved files. Inspection was verified from retained output, not rerun by the merger.

The runtime-Pipeline-disabled warning and inherited obsolete `PreventDefault()` warning remain documented. The local Android debug certificate is retained; this is not production signing or Store publication. Package/alignment inspection is not physical-device or physical ARM64/16KB-kernel acceptance.

## Preservation and remaining work

All **216 raw files / 165,095,193 bytes** and **15 selected evidence streams / 272,760 bytes** were source-checked, preserved and destination-checked against their original SHA-256 manifests before any checkout newline conversion. Candidate-owned raw paths and exact Android build paths are retained in place. **101 streams originating under earlier ticket paths** are archived instead under `artifacts/kings-acceptance-merge/raw/<original path>` to avoid replacing historical captures. No existing evidence was overwritten.

Exact selected streams are additionally archived in `artifacts/kings-acceptance-merge/tracked-evidence/evidence/`, with both original manifests retained beside `preservation.json`. The merged tracked copies are 13 exact byte matches; APK/AAB build-summary JSONs differ only by Git's CRLF checkout conversion. Their normalized text is identical, and `tracked-evidence-comparison.json` pins both original and checkout hashes. The raw manifest hashes to `e4c27573d47a559393e0dfbda281c5eddffe162961f4def9cd97eb238753f61b`; the selected manifest hashes to `b6b41afc41821720d641a9f6c70b780d83cda7ef629133c1e29447a35a6b6eb5`.

Combined diff checks pass. Merger commits use per-command giarrel noreply identity. No runtime/cache/build binary was added to Git, and no Unity, ADB, build, push, escalation, issue closure or cleanup occurred. Root's ongoing native update/gameplay/privacy/accessibility work and final Standards/Spec reviews remain separate acceptance gates. Earlier failures and setup observations remain in the preserved evidence; this report makes no human-group or physical-device claim.
