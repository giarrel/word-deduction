# Late-layout Standards review

Prior reviewed pin: `52be9527b8a2d1b9e4040bba4e2c6ade6d970e37`.
New exact reviewed pin: `85fa17555b44bca61640bcb13a738c3e37ffc34f`.

Reviewed the complete committed delta between these pins: two commits (`760efb7`, code7 metadata; `85fa175`, panel-layout ordering), six files, +19/-8. Earlier reports remain intact.

**Findings: 0 new documented-standard violations; 0 new reportable baseline smells.**

- `game/Assets/WordDeduction/UI/AccessibleMenu.cs:25–34,129–138` now coalesces refresh requests through the panel scheduler, then waits for a later frame before rebuilding. `refreshFrame = -1` invalidates any earlier pending rebuild; disposal guards protect both the scheduled callback and Tick. This remains presentation lifecycle work, consistent with session-interface.md:3,17 and ADR 0001. Session behavior, private-card exclusions, hierarchy traversal and ordinary node-bound updates are unchanged.
- `game/Assets/WordDeduction/Tests/PlayMode/KingsEliminationScreenTests.cs:38,55` adds `WaitForEndOfFrame` before public transitions, covering the reported late-arriving input without calling private methods or asserting internal order. The existing observable hierarchy, bounds and hidden-panel checks remain, consistent with autonomous-workflow.md:40–43.
- Code7 metadata is consistent across build settings, project settings, release reproduction and inspector expectation.

The three prior P3 resolutions and repeated-input correction are unchanged by this delta; no regression of their Standards conclusions was found. Repository overrides and tooling exclusions remain those of the original review.

Read `20-a11y-late-red-result.json` and `21-a11y-late-green-result.json`: the late-frame regression first failed with an empty Group hierarchy, then passed 1/1. These are supplied implementer observations.

This is a source-only Standards review. No source edits, tests, Editor, builds or ADB operations were performed. The reported Android failure of the earlier code7 package remains material; passing this rendered regression does **not** establish corrected native behavior or release acceptance.

