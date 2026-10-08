# Kings accessibility correction — independent Spec review

Reviewed commit: `52be9527b8a2d1b9e4040bba4e2c6ade6d970e37`.
Baseline: `c98ab1f06680857caa6b28f2bc0348b9f4ed49b6`.
Exact comparison: `git diff c98ab1f06680857caa6b28f2bc0348b9f4ed49b6...52be9527b8a2d1b9e4040bba4e2c6ade6d970e37` in `work/kings-worktrees/final-corrections`.

**No new Spec findings. The public-navigation defect is addressed at the source/rendered-test boundary.**

Spec story 55 (`docs/specs/kings-mode.md:75`) requires “public navigation without additional secret disclosures.” `AccessibleMenu.Refresh` now records the latest refresh frame; `Tick` rebuilds only in a subsequent frame (`:25–27`, `:122–138`). `GroupScreen.Update` continues calling Tick while either Group or Match is visible. Repeated requests coalesce around the latest view, and no ordinary per-frame path continually postpones rebuilding. The new hierarchy therefore visits the newly visible panel after style resolution instead of permanently retaining an empty traversal.

Both directions are covered: the added rendered test returns from deliberate abandonment and from a result to Group, then starts another Match, with 5 and 20 participants. It checks public controls, nested participant names, active nodes and nonzero frames without a language-change workaround. I inspected `artifacts/kings-final-corrections/13-a11y-red-result.json`: the test genuinely failed because the newly visible Group hierarchy was empty. `14-a11y-green-result.json` reports that same test passing.

Private protection is retained: traversal still excludes hidden elements and the entire `cardDrag` subtree, including Kings role/leader/name lists (`AccessibleMenu.cs:46–48`). Existing button invocation, text-field focus and scroll handlers remain present; post-build Tick continues updating bounds and activity. Deferral introduces a short refresh interval, not permanent removal of public actions or a private disclosure channel. This review preserves the previously documented visual-only private-access limitation.

The version-code increase to 6 is consistent across build configuration and inspection defaults and satisfies ticket #16's monotonic candidate requirement. No game-rule or persistence behavior changes are introduced.

No Editor, ADB or build execution by this reviewer. Full rendered-suite completion and corrected native TalkBack acceptance remain coordinator-owned evidence gates; the focused green result does not substitute for them. Earlier reports are retained.

**Spec: 0 findings in this delta.**
