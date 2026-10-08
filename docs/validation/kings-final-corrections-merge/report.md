# Kings final corrections: independent integration

The separate merger integrated `cc0361d29ef267c90d1896db46c6051b37f11b35` into `integration/kings-v1` from `bdd8e2f291bd0d08b511acf940bb1ca99ab8f738`, after the coordinator's native code 8 gate. The no-fast-forward merge is `b7d7e86aceef9785bcac45b65efda9abbda18c49`. Source and merge have the identical complete Git tree `c436e108c3a7c3e2da616ebdd6c3933fdd307cba`; no conflict resolution or production edit was needed. The author and committer use `giarrel <126280217+giarrel@users.noreply.github.com>` through per-command options.

The final handoff includes the committed-byte correction after `252e98b`. All **168 selected files / 979,879 bytes** independently match their manifest in the implementer's checkout, a Git archive of the final commit, and the merged checkout. The manifest SHA256 is `68fec9adeeb8258a87519c08102285a98e1be25f5f96979ccb5246258f691036`. Original CRLF bytes are retained. The [implementation report](../kings-final-corrections/report.md) explains the input guard, native accessibility correction, intermediate failures and native boundaries; [integration evidence](evidence.json) records the exact trees, packages and preservation hashes.

## Validation provenance

- **Session 74/74** is reused from the correction implementer's full run at `c98ab1f06680857caa6b28f2bc0348b9f4ed49b6`. The merger independently counted 74 PASS records, zero FAIL records and the final `74/74 passed` line. The log SHA256 is `1807995ad8583e712c8665e0e7818cde7b947c479cf5168355afcf8695f29ac2`. The Session tree (`6e80e583e3b9418293837f7aea6e8430d793080f`), Session.Tests tree (`b3e692c843cc9f65bc1d33fa6afef7eb4b78f4db`), content and third-party trees match that run exactly. This is explicit reuse of that correction run, not a claim of a new independent execution or reuse of the earlier, different acceptance source.
- **Rendered 47/47**, zero failed/skipped/inconclusive, completed in **208.73 seconds** at `85fa17555b44bca61640bcb13a738c3e37ffc34f`. The merged runtime, rendered tests, Session tests, content and third-party inputs match that source. Subsequent release configuration pins code 8; the suites were not repeated during integration. Both retained late-layout source reviews report no new findings.
- The entire `game`, `tests`, `tools`, `content` and `third-party` trees match build pin `5c56e63d755df24156e6f5e88f4170ae4857a0ab`. The package-lock SHA256 is `da98f245e8c511d47699dc883e521eff5176800341117fcfe8f77fe9dcfb342d`.
- The authored source diff passes `git diff --check`. Authored report/configuration metadata also passes with CR treated as a line ending. The default full check flags intentionally preserved CRLF and captured command whitespace in raw evidence; its exact output is retained under `artifacts/kings-final-corrections-merge/full-diff-check.txt`. Evidence bytes were not normalized to hide these findings.

## Packages and preserved streams

Both final packages are version **1.1.0 / code 8**, built from `5c56e63`, with zero build errors and two reported warnings each. The merger verified actual package bytes against the build summaries and inspection manifests. These remain locally debug-signed test packages.

| Package | Bytes | SHA256 | Existing inspection verified |
| --- | ---: | --- | --- |
| APK | 37,454,738 | `d4ba4c5ba65ca03249f59455234c1df7bdf4f85b160828ca05b945a85cd78ab9` | 14 commands, all exit 0 |
| AAB | 37,811,704 | `41d5a652323b3a263bd7d28d5847bced7e9a2c3ab5a1359500e3026bb0ca8e50` | 44 commands, four packages, all exit 0 |

All **609 raw source files / 1,401,946,541 bytes** were copied and independently checked by size and SHA256. This includes the complete correction directory, editor logs, extracted inspection data, font backups, all code 6/7/8 Android outputs and the screenshot dependencies. The code 6 APK and code 7 APK/AAB are preserved as historical intermediate packages, with their separate sources and hashes in `evidence.json`. Original red tests, the overbroad guard's failed full run, the failed code 6 inspection and the code 7 accessibility failure remain available. The earlier unmeasured code 7 repeat observation remains unclassified.

Candidate-owned correction files and build-pin directories retain their canonical `artifacts/` paths. The 119 files produced under earlier Kings ticket folders are preserved separately under `artifacts/kings-final-corrections-merge/raw/artifacts/`; no historical stream was overwritten. The complete map is `artifacts/kings-final-corrections-merge/preservation.json`, SHA256 `f7492da2299c50bee701f0788975c27884fef61d8a81fa7152763c4a8a18c1c3`. A Git archive preserves the selected committed evidence separately. No raw package, cache or extracted binary was added to tracked files by this merger.

The coordinator's already copied native set was independently rechecked against its existing manifest: **1,092 files / 76,455,269 bytes**, SHA256 `bb0c8ae66bfe8fd6c6ce675386786d52d6f3e70730fe94b796b9775121a25030`, at `artifacts/kings-native/preservation.json`. Every destination file matches. These are the actual manifest totals, correcting transcription differences in the handoff message.

## Native scope and handoff

The coordinator's `work/kings-native/observations-code8.md` records the installed APK's measured 76 ms repeat, five/twenty-player public navigation, large-font German good-King and English evil-King cards, ordinary hold/release, intentional later rematch and final preserved Group. It also retains the low-memory process exit with byte-preserved saves, bounded settled-tree probes and covered stationary injected TalkBack holds. The merger did not operate the device or broaden these claims to audible accessibility, independent blind play, physical-phone performance or human group enjoyment. Final native reporting and acceptance remain coordinator-owned.

This integration ran no Unity, ADB or new builds, performed no push or issue closure, and removed no worktree or evidence. The earlier push rejection had already been resolved by the coordinator using authenticated public-owner evidence; it is not a current blocker. The coordinator can now add the final native documentation and perform publication and tracker closure.
