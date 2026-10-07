# Release integration verification

7 October 2026. Independent implement-spec merger check for ticket 9; native acceptance and final review remain with the coordinator.

- Clean release branch `implement/release` at `f41dece7e57f1c0d7ea660704ddee07361399b7a` was a descendant of clean integration `922e4b7b387ff0c0219630725ccacf6d7be5848b`.
- Merge commit: `a6b210191aa0bc6d8a1e4ec322c026ea3b88b6f3` on `integration/android-v1`. No conflict or source edit was required.
- The merged `game`, `content` and `tests` trees exactly match build source `ef261b030a0396e7deca55a0511b5d31c2081874`. Their tree IDs are respectively `8cfa10d8aed4a35f9dfa1a812ed4f2d357c217cc`, `fae305a03886bbde4c514b9ab4606b56be478fdd` and `0b531cb31dce3211877427b5de398df4b4f379fc`.
- Independent Session runner on the integration checkout: **53/53 passed**, exit code 0. This includes both modes, durable commands/recovery, bilingual word-cycle scenarios and Unicode 17 conformance. Command: `dotnet run --project tests/Session.Tests -p:NewtonsoftJsonAssembly=<release-worktree>/game/Library/PackageCache/com.unity.nuget.newtonsoft-json@74deb55db2a0/Runtime/Newtonsoft.Json.dll`. The DLL SHA256 was `7292d3eb508652d14726749dd27094f2d481aeccf2db6427b62f68a71460897e`.
- The pre-implementation review baseline `ab25c325e02040d30755ae448c07789357730f38` resolves and produces a nonempty diff: 905 changed files and 71 commits through the merge commit, before this verification document. Review is still pending; this check does not substitute for it.

## Preserved local artifacts

Before any worktree cleanup, 98 files totaling **570,777,613 bytes** were copied into the output repository's ignored `artifacts/android-release/` directory. Every copied file's length and SHA256 were compared with its source. The original release worktree and all source artifacts remain intact.

| Preserved item | Path relative to `artifacts/android-release/` |
| --- | --- |
| Final APK and original build summary | `android/ef261b030a03/20261007T005044376Z-apk/` |
| Final AAB and original build summary | `android/ef261b030a03/20261007T005648918Z-aab/` |
| Raw final APK inspection, native libraries and command output | `release/apk-complete-final/` |
| Raw final AAB inspection, generated `.apks` archives, extracted packages, libraries and command output | `release/aab-complete-final/` |
| Original build/editor logs | `release/editor-live.log`, `release/editor-resident.log`, `release/editor.log` |
| Per-file byte/hash manifest and source identity | `preservation-manifest.json` |

The preservation manifest's SHA256 is `6c55ef7c1f7f67fdaf653dc4c2daa4631a9e4ae8166f0c0fafdf4dba7ebcac11`. The APK, AAB, universal APK, base ARM64 APK and base master APK hashes independently match all five values in the [build report](report.md). The copied raw inspection reports contain 13 APK and 42 AAB command records, all with exit code 0. Copy validation does not rerun those package commands or expand their acceptance scope. These ignored files are local deliverables, not uploaded Git contents.

## Check boundaries

Full-baseline `git diff --check` reports whitespace in preserved raw diagnostics and Unity-generated serialization. The release-only diff reports six Unity `.meta` empty-value lines and the privacy draft's intentional Markdown hard break. The source/style-only full-baseline check reports a historical extra final blank line in `QuickScreenTests.cs:204`. These are recorded rather than silently represented as a passing blanket whitespace check; no runtime/source formatting change was made during the merge.

No Editor, ADB, device, issue closure, production signing, worktree removal or other-project operation was performed. Ticket 9 remains open. The two confirmed small-phone/large-text findings and the coordinator's remaining native acceptance, screenshots and independent Standards/Spec review must still be completed.
