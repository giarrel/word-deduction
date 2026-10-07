# Targeted Spec verification

Reviewed `ea69137f80754f15c3fe9b2273afc95713fe6bd9...b86f19564b1d4c2fa31f02d7872dca7e835d881a` against unchanged canonical spec #2. Correction checkout HEAD matched the pinned commit and was clean before the read-only review.

**Original P1: verified fixed.** Ran the unchanged original public-Session reproducer against the correction's production Session sources and the resolved Unity Newtonsoft DLL. The accepted 24-family-emoji name remains 24 graphemes/264 UTF-16 units. Add, deal and handoff succeed; both reopening points retain the match and all three players, with no storage notice and no blocked state. Source now validates frozen labels through the existing saved-name structural policy, preserving accepted names, duplicate suffixes and prior confirmed names. No schema rewrite is introduced. Evidence: `work/final-spec-verification-probe/Program.cs`, `Probe.csproj`, `result.txt`.

**Other scoped corrections: no new Spec finding.** Shared role-count functions preserve all existing Quick/Classic thresholds and White eligibility. The source now keeps the missing-player hint visible at enlarged text size; shortened bilingual instructions and reduced decorative space address the cited visibility requirements. Independently inspected the four new English/German empty-group and saved-White Classic-three captures: full instructions and disabled-Play explanations are visible. These are supplied rendered fixtures, not a new native-device validation by this reviewer.

Release minification and the GameTextInput keep-rule adjustment implement the spec's “No logging of personal names, words or role assignments in release logs.” JNI names/members remain retained; release builds fail if minification or the expected keep rule is absent. The added DEX inspection rejects remaining input debug/info/verbose log calls. Actual replacement APK/AAB and native logging confirmation remain the coordinator's pending checks; source configuration alone is not claimed as proof.

**Result: original P1 closed by independent reproduction; zero new substantiated findings in this scoped review.** No source edits, Unity or ADB operations.
