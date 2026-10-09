# Spec review — group usability and fluid interactions

Compared fixed baseline `53951d9f1d6cc0f9014db7615cfa7a159bb804d9` with `8f018b15c344c687dbbb22bfaaf5601dd2b398a8` using the three-dot diff. Sources: Specs #17/#18 (`docs/specs/group-usability.md`, `docs/specs/fluid-interactions.md`), recorded user feedback, implementation scope, AGENTS/domain guidance and relevant source/tests. Read-only review; no Editor, ADB or test execution by this reviewer.

**Implementation findings: 0.** No confirmed missing behavior, incorrect implementation or unrequested scope was found within these two specifications. Explicit editing replaces name clicks and ordinary participation controls; reordering uses stable IDs and an atomic expected-order check, with cancellation, non-drag controls and legacy inactive preservation. Secret concealment precedes return decoration; the new reveal/readiness cache stays within the card lifetime, while Session continues to validate advancement. Captured reorder endings remove only their own contact.

**Acceptance remains incomplete at this review point; this is not a runtime-defect finding:**

- Group testing requires: “install the final Android candidate as an update over the existing code8 save. Inspect actual screenshots” (`group-usability.md`, Testing Decisions). Candidate update/persistence and native keyboard/accessibility acceptance must be recorded before closure.
- Motion testing requires: “Preserve comparable baseline and candidate measurements” and “Verify actual motion visually over time” (`fluid-interactions.md`, Testing Decisions). The reported rendered 57/57 and Session 76/76 results cannot establish actual Android smoothness or native reduced-motion behavior. Complete the planned paired frame measurements, continuous visual inspection and native reduced-motion/privacy checks; retain any stalls and emulator limitations.

Free base-mode role counts (#11) and reordering unread live handoffs are explicitly outside this reviewed package while their human decisions remain pending. Their absence is not counted as a deviation from #17/#18, and this review does not establish completion of those requests.

Spec axis: **0 implementation findings; native acceptance gates remain open.**
