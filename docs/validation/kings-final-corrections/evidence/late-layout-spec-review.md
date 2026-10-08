# Kings late-layout correction — independent Spec review

Baseline: `52be9527b8a2d1b9e4040bba4e2c6ade6d970e37`.
Reviewed commit: `85fa17555b44bca61640bcb13a738c3e37ffc34f`.
Exact comparison: `git diff 52be9527b8a2d1b9e4040bba4e2c6ade6d970e37...85fa17555b44bca61640bcb13a738c3e37ffc34f`, inspected in `work/kings-worktrees/final-corrections`.

**No Spec findings in this delta.**

Spec story 55 requires “public navigation without additional secret disclosures.” The earlier next-frame-only change did not account for actions arriving after the panel's layout pass; its rendered success was insufficient to establish Android acceptance. This correction first lets the panel scheduler execute, records that frame, then rebuilds on a later Tick. Refresh clears an obsolete pending frame and coalesces scheduled work, so a later view change cannot reuse an earlier layout checkpoint. Disposal still prevents rebuilding.

The updated test moves both Group return and subsequent Match start to `WaitForEndOfFrame`, covering the newly identified ordering. It retains abandonment/result return paths, groups of 5/20, public/nested labels, active nodes and nonzero bounds. I inspected the preserved evidence: `20-a11y-late-red-result.json` fails because the visible Group's hierarchy is empty; `21-a11y-late-green-result.json` passes the same test. This is behavioral red/green evidence, not a compilation-only failure.

Private exclusions remain unchanged: hidden elements and the complete `cardDrag` subtree, including Kings words, roles and leader/name lists, are omitted. Public button invocation, field focus, scrolling, and ongoing bounds/activity updates remain intact. The patch introduces no new secret channel, game rule, or persistence transition. Version-code metadata is internally consistent at 7; no corrected native artifact is accepted by this review.

Scope: source and retained rendered evidence only; no source edits, Unity, ADB or build execution. Code7's actual Android failure remains valid evidence. Corrected source-pinned Android transition checks are still required and must not be inferred from the green test or this report. Earlier review reports are retained.

**Spec: 0 findings; native pass not established.**
