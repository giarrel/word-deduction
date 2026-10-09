# Usability candidate acceptance

Status: planned, no candidate acceptance claimed. Baseline installed 1.1.0/code8; source review baseline 53951d9f1d6cc0f9014db7615cfa7a159bb804d9. Specs #17/#18; free-role rules #11 await the user's three explicit decisions. Required rule answers are not inferred from elapsed time.

## Evidence contract

- Session scenarios verify durable identity/order, limits, undo, rejected writes and old saves. Rendered scenarios exercise pointer dispatch, cancellation, keyboard/alternative input, localization and card privacy. Independent Standards and Spec reviews precede final acceptance.
- Update the existing isolated Android installation without uninstalling or clearing its saves. Preserve both save generations before installation; compare their exact bytes before the first durable candidate action. Label fixture replay separately from the real update.
- Root alone owns ADB server5038 / 127.0.0.1:5583, verified AVD word_deduction_api36_16k. No Nischenreich operations. Unity Editor/import/build activity must be idle for comparable frame traces.
- Retain failed traces and screenshots. Inspect images before calling them visually checked. A translated ARM64 emulator cannot establish physical-phone frame pacing, haptics or human group enjoyment.

## Native scenarios

| Area | Scenario | Expected evidence | Status |
| --- | --- | --- | --- |
| Update | Install candidate over code8 without altering the five-person original group | Package/hash/version and byte-preserved saves; visible group | Not run |
| Intentional editing | Repeat small and larger swipes over player names | List scrolls; no editor/keyboard/reorder opens | Not run |
| Edit | Explicit Edit, rename, cancel, Android Back, real keyboard | Intended stable ID only; no stray committed draft | Not run |
| Remove | Remove then Undo | Same ID/name/position restored | Not run |
| Order | Drag distinct handle; move via Edit alternatives; cancel/outside drop | Complete committed order or original order; no accidental edit | Not run |
| Durable order | Change mode, restart process, start next match | Group order retained; named handoffs follow it | Not run |
| Layout | DE/EN, five/twenty players, normal/large text | Reachable controls and names, no ordinary Pause/Join | Not run |
| Compatibility | Explicit labelled old inactive-name fixture | Collapsed saved-people section; no automatic joining/deletion | Not run |
| Motion baseline | Warm code8 scroll/pull sequence with idle Editor | Actual presentation fences and actual device event timing | Complete; baseline-report.md, all four gestures covered in each run; original saves restored byte-exactly |
| Motion candidate | Same fixture, geometry, timings and environment | Median/p95/stalls plus coverage and bounded visual sequence | Not run |
| Card privacy | Pull/hold/release, cancel, second touch, Home/resume | Secrets disappear immediately; decorative return only | Not run |
| Card breadth | Maximum Kings lists, DE/EN, large text, reduced motion | All permitted names reachable; no private accessibility exposure | Not run |
| Public transitions | Group/edit/match/back/result navigation with native accessibility | Fresh public destination tree; secrets excluded | Not run |
| Final state | Restore original synthetic baseline after fixtures, preserving new durable intentions separately | Save hashes, identities/order/history and settings accounted for | Not run |

Free-role scenarios will be added after #11 decisions; do not claim this plan covers unagreed game rules. APK and AAB inspection must match the final clean source pin, unchanged signing identity, release settings and monotonically increased version code. Retain exact test counts, package hashes and unresolved constraints in the final report.

## Integration checks already completed

- Session suite: 76/76 passed at integration3835a710, preserved in integration-session-full.txt. Same Session and test source remains in motion merge8f018b1. Earlier intermittent corpus replacement failures remain in the group report and are not erased by this passed run.
- Full rendered suite: 57/57 passed on final motion/group production33d0d99. Evidence is tracked under docs/validation/fluid-interactions/evidence; later merge/report commits contain no further runtime changes.
- Source review: fixed53951d9..8f018b1. Standards review has no documented-standard violations and one optional P3 duplicated test-helper finding; routed to the single final correction implementer. Spec report is pending at this entry.
- Native helpers were copied from prior accepted test tooling into this folder. Legacy capacity data is explicitly synthetic V4 (20 active,20 saved); both copies are labelled capacity-legacy-fixture. Existing Kings fixtures retain their original public-Session generation provenance. Neither is counted as a played flow.
- VerifyCandidateUpdate.py will require the original code8 saves and install the exact inspected artifact with adb install -r. For version codes above8, ReplayFixture.py now requires this new actual8-to-candidate record; the historical4-to5 record is insufficient.

---

Lesefassung mit dauerhaften relativen Links. Das [unveränderte eingefrorene Original](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/native/acceptance-plan.md) bleibt einschließlich früherer Formulierungen erhalten. Die aktuelle Updatebeschreibung unterscheidet den vorherigen Originalrestore vom eigentlichen Installationsversuch.
