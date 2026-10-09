# Usability implementation, October 2026

- User explicitly requested Specs and their immediate implementation for the captured group, role-count and motion feedback.
- Integration: `integration/usability-v1.2`; fixed review baseline: `53951d9f1d6cc0f9014db7615cfa7a159bb804d9`.
- Workflow: to-spec, to-tickets, implement-spec, codebase-design, TDD, independent Standards/Spec reviews, one correction implementer, separate merger, native acceptance, inspected APK/AAB and safe worktree cleanup.
- Existing autonomous-workflow authorization covers routine seam, ticket and implementation choices. Unknown base-mode victory rules require the pending human answers; unrelated group/motion work progresses meanwhile.
- Current Specs: `../specs/group-usability.md`, `../specs/fluid-interactions.md`. A complete free-role-count Spec will follow the answers for issue #11. All user feedback remains in `../feedback/2026-10-09-group-and-role-controls.md`.
- Group mutation and persistence use the existing public Session seam; gesture, layout and frame pacing use the rendered-app seam. Keep source provenance and preserve original failed evidence.
- Each implementer works in a separate checkout from this integration branch. Confirm baseline ancestry and merge the current integration tip before final handoff. A separate merger integrates; no implementation agent pushes or closes issues.
- Only one Unity Editor lease at a time. Root grants the lease before any launch/rendered test/build. Explicit project paths; never touch Nischenreich. The motion investigator restored both original saves and released ADB. Root owns the isolated AVD for acceptance and has repeated the code8 motion baseline with all owned Editors closed.
- Retain existing tools and signing identity. Final version planned as 1.2.0 with a monotonically higher code than 8; exact code and source are pinned at build. No Store publication.
- Preserve required raw logs/screenshots/packages outside disposable worktrees before removing them. Per-command Git safe.directory and established noreply identity; no global Git changes.

## Published graph

| Spec | Ticket | Delivers | Blocked by |
| --- | --- | --- | --- |
| [#17](https://github.com/giarrel/word-deduction/issues/17) | [#19](https://github.com/giarrel/word-deduction/issues/19) | Intentional editing, no ordinary participation controls, persistent ordering and compatibility | None |
| [#18](https://github.com/giarrel/word-deduction/issues/18) | [#20](https://github.com/giarrel/word-deduction/issues/20) | Measured frame pacing and card/scroll motion | None |
| #11, updated after answers | To be created | Free base-mode role counts and consistent outcomes | Human rule answers; group ticket for shared setup/persistence integration |

The independent group/motion specifications may be validated and delivered while the role specification awaits its required answers. Such a package must clearly state that it does not yet implement issue #11; that issue stays open. Completion of the full requested role-count work still requires the decisions and its own implementation/acceptance. No unresolved base-mode rules are inferred from Kings.

## Integration progress

- Group ticket #19 is merged at `3d9687c6a9fd043148d22d6c7d1f19c692a2d45e`; preservation/report commit `01bffc23b7d40dfdc4a39f6155511b563635035b`. Native acceptance, integrated full regression and final independent review remain outstanding. Exact full versus targeted test results are retained in the group reports.
- Motion ticket #20 includes an integrated reorder-to-reveal regression. A captured reorder release could leave an old pointer contact and block a later finger; the fix must clear only the actually ended contact, preserving other held fingers and privacy guards.
- The code8 comparison uses actual SurfaceFlinger presentation timestamps and device-scheduled input with actual event timestamps. A separate recorded sequence is excluded from frame-performance comparison because encoding adds measurable load. Both original save generations were verified byte-identical after restoration.
- Pending role decisions: Quick's accusation criterion with multiple adversaries; winners after a correct White guess; whether pure-White compositions are allowed. Optional unread-handoff reordering remains outside the current pre-match order baseline until answered.
