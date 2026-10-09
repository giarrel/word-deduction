# Role-counts release handoff merge

Merged clean handoff `be88547f65dfa30c1abd4372b98185fcbad006e4` into `integration/role-counts-v1.3` from `9041c00bf417b86295559f7de89cce8a5abed7b0`. Merge commit: `159cd19dd29bd186004d2caebfac113496cc162d`. The integrated `game`, `tests`, `content`, `tools`, and `third-party` trees exactly match accepted build pin `b6e6a926bbcc58c7103d79a9f117cbd09e4269e3`; every change after that pin is documentation.

- [Build and rendered handoff](../role-release/report.md): Session 93/93 reused from the verified correction run; fresh rendered tests 66/66, zero failures/skips/inconclusive, 221.91 s. Merger independently parsed all 66 recorded passing results and both successful build summaries; no tests or builds rerun.
- [Preservation inventory](preservation.json): all 389 declared originals / 759,412,395 bytes verified against their size/SHA-256 manifest and copied without overwrite. Including the original manifest itself, [the raw archive](../../../artifacts/role-release-merge/be88547f65df/raw/) contains 390 files / 759,489,515 bytes. Source hashes and complete file inventory were checked again after copying. Rejected APK and unsuccessful infrastructure attempts remain intact.
- 28 [original checkout documents](../../../artifacts/role-release-merge/be88547f65df/checkout-evidence/) and 28 [canonical Git blobs](../../../artifacts/role-release-merge/be88547f65df/canonical-git/) are separately preserved with hashes and blob IDs. Checkout versus Git: 26 byte-identical, 2 CRLF/LF-only differences. Matching raw originals versus Git: 4 exact, 13 CRLF/LF-only, 1 with line-ending/trailing-whitespace normalization. The other 10 reports/exports have no byte/whitespace match and are explicitly kept distinct; canonical hashes are never presented as raw hashes.

| Accepted package | Bytes | SHA-256 |
| --- | ---: | --- |
| [APK](../../../artifacts/releases/1.3.0-code12/WordDeduction-1.3.0-code12.apk) | 37,480,110 | `cc0ab333bdb130cda07e45ec64a7eab68887d8a0384a9325506827885011aedc` |
| [AAB](../../../artifacts/releases/1.3.0-code12/WordDeduction-1.3.0-code12.aab) | 37,837,035 | `b302d3a9bc9628870ce953156c2990d6491d3e0ad1a063a942f970a84bca15a6` |

The existing APK was only verified; the AAB was newly copied from the verified raw archive. Both are version 1.3.0 / code 12 with the existing local Android debug certificate. The separate rejected candidate was not replaced or selected.

Native acceptance is owned by the coordinator and still pending at this merge. This handoff does not establish final release readiness. No native files, Unity sessions, devices, trackers, or remotes were changed; all worktrees remain in place.
