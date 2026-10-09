# Role review correction

9 October 2026. The independent pre-build Standards review found one duplicated survivor draw and round increment; its documented-standards axis and the separate Spec review had no findings. This is the complete correction set supplied by Root. Source baseline: `d7cc6a79516294f8d2c69d9364b134b1fa16f603`; corrected source: `d3ad9d0ecd42f4bfb46e2e09f75f914e07a02e59`, branch `implement/role-review-corrections`.

`Match.cs` now owns the shared operation in private `AdvanceRoundStarter`. Both existing transitions still draw from living participants, set the starter and increment the clue round in the same order. `AdvanceKingsStarter` retains its subsequent V6 rules marker; Classic `NextRound` retains its subsequent suspect/runoff reset and Clues phase. No public interface, rule, rendering or test changed.

## Validation

This review-stage extraction uses the already agreed public Session seam and real saved directories. Existing correct behavior was verified before and after; no artificial failing test or private-helper test was added.

- Before, at the baseline above: **93/93 passed, exit 0**. [Full output](evidence/session-before.log), [run provenance](evidence/session-before-run.json).
- After, at the corrected source above: **93/93 passed, exit 0**, 299.56 seconds. [Full output](evidence/session-after.log), [run provenance](evidence/session-after-run.json).
- Existing cases cover Kings ordinary elimination, optional rounds, all living starter candidates including White, repeated choices, stale actions, failed saves and authentic V5 continuation. Classic elimination, failed White guess and repeated-tie continuation retain their public round/starter/reset behavior.
- Latest integration was merged before handoff: already up to date at `d7cc6a7`. Production/tests/content/tools still equal the tested correction source. `git diff --check` passed.

Run JSON records the exact command, source and dependency hash. Raw outputs remain in `artifacts/role-review-corrections`; the linked files are committed text copies. The recorded log SHA256 identifies the raw capture, not Git's possible newline-normalized copy. Preserve raw files before deleting this worktree.

No Unity, Android, build, tracker or push operation was performed. Root owns independent correction recheck, separate integration, final rendered regression and native/release acceptance under ticket #23. These model results do not establish Android readiness or physical-device behavior.
