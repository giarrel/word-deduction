# Corrected pre-build review

Reviewed gameplay baseline a0e929f956348563336899743258d8d4c1c22b04 through d7cc6a79516294f8d2c69d9364b134b1fa16f603; subsequent production correction d3ad9d0ecd42f4bfb46e2e09f75f914e07a02e59 and independent version preparation bcffd96114896f32606f66067c4060edfccc4d1d were checked on both axes. Original reports are retained in review-prebuild.md and its two source reports. The extraction's full Session before/after runs both passed 93/93 with exit0; final handoff ec3c3d4ff2066b99ed3d97e6ae0c4ac474fe3218 has only subsequent evidence documentation. Integration and exact source equality are still a separate merger check.

## Standards
# Standards recheck — correction and version preparation

Reviewed immutable deltas:

- `git diff d7cc6a79516294f8d2c69d9364b134b1fa16f603...d3ad9d0ecd42f4bfb46e2e09f75f914e07a02e59`; sole commit `d3ad9d0` (shared starter advancement).
- `git diff d7cc6a79516294f8d2c69d9364b134b1fa16f603...bcffd96114896f32606f66067c4060edfccc4d1d`; sole commit `bcffd96` (Android release configuration).

**Previous heuristic finding closed.** `Match.cs` now performs survivor filtering, the random starter draw and round advancement once in private `AdvanceRoundStarter`. Both Kings and Classic use it. Kings retains its V6 marker; Classic retains its phase, suspect and runoff resets. The extraction preserves the existing state transition order and public interface, without adding a speculative abstraction.

**No new documented violations or substantiated heuristic findings.** All changed runtime lines and all version-preparation lines were inspected against the previously identified repository standards. `AppBuild.cs`, `ProjectSettings.asset` and the release inspector defaults consistently specify `1.3.0` / `12`, matching `docs/development/role-counts-implementation.md`. Other build settings and inspection logic are unchanged.

This was read-only source review. No tests, Unity, ADB, tracker operations or source edits were performed. The correction's post-change Session run, final integrated provenance, produced-package inspection and native Android acceptance remain separate evidence requirements. This report does not mark ticket #23 or the release ready.

**Totals: previous finding resolved; 0 open documented violations; 0 open heuristic findings in the reviewed deltas.**

## Spec

# Spec review — role-counts correction and release-version deltas

Independent Spec-axis review against `docs/specs/role-counts.md` (34 stories), AGENTS/domain guidance, glossary and the independent-Session ADR. Read the prior zero-findings gameplay review `review-spec-prebuild.md`; this report extends it only for the two immutable deltas below.

All three revisions resolve as commits and both three-dot diffs are non-empty. In each named checkout, executed `git -c safe.directory=<that checkout> log --oneline <base>..<tip>` and `git -c safe.directory=<that checkout> diff <base>...<tip>`:

- `implement-review-corrections`: `d7cc6a79516294f8d2c69d9364b134b1fa16f603...d3ad9d0ecd42f4bfb46e2e09f75f914e07a02e59`. Sole commit: `d3ad9d0 Share survivor starter advancement across clue rounds`.
- `implement-acceptance`: `d7cc6a79516294f8d2c69d9364b134b1fa16f603...bcffd96114896f32606f66067c4060edfccc4d1d`. Sole commit: `bcffd96 Prepare Android 1.3.0 code12 release configuration (#23)`.

**Spec findings: 0.** No missing/partial implemented behavior, scope expansion or incorrectly implemented requirement was found in these deltas. All hunks and the affected callers were inspected.

The shared starter helper retains survivor filtering, exactly one `random(survivors.Length)` call, index assignment and then `Round++`, in the same order. It matches “Choose uniformly from all living participants, regardless of role; repeated starters are allowed.” Kings alone still sets `RulesVersion = 6` afterward. Classic alone still clears `Suspect`/`Runoff` and enters `Clues`; its tie, failed-White-guess and ordinary-continuation callers are unchanged. Quick continuation gains neither a starter draw nor a clue round. No transaction boundary, validation guard or outcome calculation changed.

AppBuild, serialized PlayerSettings and inspector defaults consistently use **1.3.0/code12**, preserving application identity, Android targets and release options. The higher code supports the specified code11 update; it is not proof that a package was built or accepted.

Baseline Session **93/93** is previously recorded evidence, not a new result from this reviewer. The post-correction run, final integrated-source/evidence verification, Android update/native flows and #23 APK/AAB acceptance remain pending. No release-readiness claim is made. No source, Unity, tests, ADB, tracker or push operations were performed; only this external report was written.

Standards: 0 open findings (one maintenance heuristic resolved). Spec: 0 findings. Full integrated rendered test, package inspection and actual Android acceptance remain pending under ticket23; this is build-source clearance, not release acceptance.
