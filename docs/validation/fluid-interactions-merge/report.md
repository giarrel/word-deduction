# Fluid interactions merge — ticket #20

A separate merger integrated `implement/fluid-interactions` into `integration/usability-v1.2` with an explicit merge commit. Both checkouts were clean and the handoff already contained the current integration tip.

- Integration parent: `3835a71035b0fe4c471e5893a54f2438b5c34505`.
- Implementer handoff: `f07419bbbfd157c0072e61ab59f0c187d6e4ddfd`.
- Tested production source: `33d0d99a392ae1a2813edabc4194e315de677a7f`; later commits merge coordinator validation-scope documentation and add evidence.
- Merge: `8f018b15c344c687dbbb22bfaaf5601dd2b398a8`. No conflicts or production edits were necessary. Its complete tree equals the handoff tree, `8616e4c387f05813bc07bd9b4ac06031b8d39a87`.

[Verification](verification.json) records post-merge byte-count and SHA256 checks for all 33 selected files (769,137 bytes). Before merging, all 35 manifest-listed raw files (17,555,889 bytes), including generated font caches, were copied to ignored `artifacts/usability-motion-implementation/`. Source and destination hashes were checked before integration and copied hashes again afterward.

The frozen investigation's [233-file baseline](investigation-baseline-manifest.json), 3,848,293 bytes, was also verified and copied to ignored `artifacts/usability-motion-investigation/`. A [separate supplement manifest](investigation-supplement-manifest.json) preserves 31 otherwise unlisted files (1,527,895 bytes), including `measurement-sources.md`, the original baseline manifest, implementation checkpoint and probe build output. The frozen manifest was not regenerated. All supplement hashes were checked after copying and after merging. The source investigation and implementation worktree remain available; the coordinator's `work/usability-native/` was not changed.

The [implementation report](../fluid-interactions/report.md) remains the test evidence authority: the integrated rendered assembly passed **57/57**, with zero failed, skipped or inconclusive results, in 207.35 seconds. Its four new motion/contact checks include the captured reorder release regression. The retained RED results and invalid first capture harness remain distinct. The initial native investigation is not the final performance denominator; use the coordinator's subsequent idle-host, device-scheduled code8 baseline for candidate comparison.

No tests were rerun during this merge. No Unity, ADB, build, tracker, push or worktree-removal operations were performed. Native candidate performance, continuous visual inspection, Android reduced-motion/accessibility and update acceptance, independent review and inspected release packages remain coordinator integration work. Free role-count rules remain outside ticket #20 and await the pending decisions.
