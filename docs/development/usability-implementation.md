# Usability implementation, October 2026

- User explicitly requested Specs and their immediate implementation for the captured group, role-count and motion feedback.
- Integration: `integration/usability-v1.2`; fixed review baseline: `53951d9f1d6cc0f9014db7615cfa7a159bb804d9`.
- Workflow: to-spec, to-tickets, implement-spec, codebase-design, TDD, independent Standards/Spec reviews, one correction implementer, separate merger, native acceptance, inspected APK/AAB and safe worktree cleanup.
- Existing autonomous-workflow authorization covers routine seam, ticket and implementation choices. Unknown base-mode victory rules require the pending human answers; unrelated group/motion work progresses meanwhile.
- Current Specs: `../specs/group-usability.md`, `../specs/fluid-interactions.md`. A complete free-role-count Spec will follow the answers for issue #11. All user feedback remains in `../feedback/2026-10-09-group-and-role-controls.md`.
- Group mutation and persistence use the existing public Session seam; gesture, layout and frame pacing use the rendered-app seam. Keep source provenance and preserve original failed evidence.
- Each implementer works in a separate checkout from this integration branch. Confirm baseline ancestry and merge the current integration tip before final handoff. A separate merger integrates; no implementation agent pushes or closes issues.
- Only one Unity Editor lease at a time. Root grants the lease before any launch/rendered test/build. Explicit project paths; never touch Nischenreich. The motion investigator temporarily owns the isolated AVD's ADB state for baseline measurements; root will reclaim it explicitly before native acceptance.
- Retain existing tools and signing identity. Final version planned as 1.2.0 with a monotonically higher code than 8; exact code and source are pinned at build. No Store publication.
- Preserve required raw logs/screenshots/packages outside disposable worktrees before removing them. Per-command Git safe.directory and established noreply identity; no global Git changes.

## Published graph

| Spec | Ticket | Delivers | Blocked by |
| --- | --- | --- | --- |
| [#17](https://github.com/giarrel/word-deduction/issues/17) | [#19](https://github.com/giarrel/word-deduction/issues/19) | Intentional editing, no ordinary participation controls, persistent ordering and compatibility | None |
| [#18](https://github.com/giarrel/word-deduction/issues/18) | [#20](https://github.com/giarrel/word-deduction/issues/20) | Measured frame pacing and card/scroll motion | None |
| #11, updated after answers | To be created | Free base-mode role counts and consistent outcomes | Human rule answers; group ticket for shared setup/persistence integration |

Final package acceptance waits for the complete agreed graph and independent reviews. No unresolved base-mode rules are inferred from Kings.
