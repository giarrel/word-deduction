# Role counts and Kings starter implementation

9 October 2026. The user resolved the final human rule questions and has already authorized writing Specs and implementing the feedback directly. His explicit autonomous-workflow authorization covers ordinary test seams, ticket sizing, worktrees and implementation choices; no further rule assumptions replace human answers.

- Spec: [#11](https://github.com/giarrel/word-deduction/issues/11), local [spec](../specs/role-counts.md).
- Integration: `integration/role-counts-v1.3`, based on delivered `a0e929f956348563336899743258d8d4c1c22b04` (1.2.0/code11). This is also the fixed independent review baseline.
- Graph: [#21](https://github.com/giarrel/word-deduction/issues/21) free counts and Quick/White outcomes → [#22](https://github.com/giarrel/word-deduction/issues/22) Kings starter → [#23](https://github.com/giarrel/word-deduction/issues/23) integrated acceptance. Shared match/schema changes make the second genuinely dependent on the first.
- Skills: to-spec, to-tickets, codebase-design, implement-spec with TDD, Unity feature/build validation, independent Standards/Spec code-review and one correction implementer.
- Agreed existing test seams: public Session using real persistence, plus the production rendered-app interface. Native Android acceptance exercises the actual APK.
- Each implementer owns an isolated branch/worktree based on this integration tip, follows vertical red/green behavior tests, and merges integration before handoff. Separate merger agents integrate. Root owns issue updates/pushes and the final native device acceptance.
- Exclusive Unity lease: no agent starts Editor/tests/builds without Root granting its exact project path. Never act on Nischenreich. Root similarly owns ADB on server5038/transport127.0.0.1:5583; no agent may change or clear it without explicit delegation.
- Target release is 1.3.0 with versionCode12 or a later code if actual installed test iterations require it. Local existing signing identity; no Store publication.
- Preserve evidence and packages to durable primary artifacts before cleaning isolated worktrees. Do not repeat unrelated motion measurements or claim physical-device smoothness.

## Status

Ticket #21 is integrated at `a425f16e9981e328741968c74224bf458739f9c7`, with production/test trees identical to validated `30ec5b14fe1dafea7adb4ca81b349e047633d194` (87/87 Session; 64/64 rendered, zero skipped). [Merge and raw/canonical evidence preservation](../validation/role-counts-merge/report.md). Ticket #22 Kings starters and #23 final independent/native/release acceptance remain outstanding. The installed code11 release still does not contain these features.
