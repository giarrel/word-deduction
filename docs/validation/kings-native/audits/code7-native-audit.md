# Code7 native evidence addendum

Frozen offline audit; generated 2026-10-08T21:22:44.321744+00:00. This does not replace or rerun the original **467-check** audit. **163/163 evidence-consistency checks agree with the expected records**; those records include the still-failing accessibility transition. This count is not an app acceptance score. No ADB, Unity, build, push or production-source edits were performed.

The original Markdown, JSON and audit script are byte-identical before/after this audit:

| Original file | SHA256 |
| --- | --- |
| `native-state-audit.md` | `b57b872ef09df20097fe128082d8f1a9ee05c35526f244bd403f1325469a1ef9` |
| `native-state-audit.json` | `f207fae79deb64cf21891558a43036d1463eba5dc9b2509af3a4fcc261612d19` |
| `audit-native-state.py` | `8a2a5227fdd89a668a1f6df79275be09811ada92cb38674bad449be24428cff4` |

## Actual Code5 → Code7 update

Independently recomputed file equality agrees with both `code7-update-byte-equality.json` and `code7-resumed-byte-equality.json`. Both V5 generations of `code5-final-update-baseline-state` are unchanged at `code7-before-first-launch-state` and `code7-resumed-before-confirm-state`:

- Primary: `1b7a964802c292756d77195ceefa622f0d6e0b274113975218254d9c20c3abe2`.
- Previous: `687e3835c5b2930d952c10445fb0f59f1eb21af008f7606fa14ff68438096239`.

Match `69fc9f23a2b44963b61cc5e954f24af4` remains Kings, phase6/TablePlay, outcome null, handoff5, pending suspect Nora, Ratte/Maus. The five-player roster (including stable-ID Jonas2), roles, settings and history are identical because the complete files are identical. The retained installed-package record says version1.1.0/code7, min26/target36. This is the actual in-place update observation, distinct from the later fixture replays. The replayed starting baseline itself was explicitly reconstructed from earlier Android gameplay, as recorded in the Code5 notes; no claim that it was never replayed is made.

## Measured repeated input

| Capture | Same match ID | First UP → second DOWN | Phase | Outcome | Primary SHA256 |
| --- | --- | --- | --- | --- | --- |
| `code7-sync-doubletap` | `69fc9f23a2b44963b61cc5e954f24af4` | 75 ms | 3 | 6 | `9153d5cdab17d839809a6c75e64b2a4e6fcb7ea3701e1b8a92386b2b65c92155` |
| `code7-word-answer-repeat` | `5dfed8d3df3c43c4b3f0b2230ecc9b2b` | 76 ms | 10 | null | `8f6d44dceb80aba2e2f83a554c6f5c2f292e726b276f4a3ae345ce34443bc80a` |
| `code7-word-judgment-repeat` | `5dfed8d3df3c43c4b3f0b2230ecc9b2b` | 75 ms | 3 | 9 | `7e5f104c27cd39c8c71a5827d79b11a2c3d1a796db36918156556cd6ca650076` |

Each timing file records action sequence `[0,1,0,1]`, four `injected=true` device events, injectionMode2 and returnCode0. Recomputing device event timestamps gives **75/76/75 ms**. DOWN→DOWN is **151/147/144 ms**, a different measurement. The host-requested spacing was75 ms; it is not substituted for the recorded device gap.

The synchronized good-King confirmation stays on the same game's phase3/GoodKingEliminated(6), rather than creating a new deal. The word-answer repeat stops at phase10/KingsWordJudgment with null outcome; the repeated judgment ends phase3/KingsWordIncorrect(9), awarding the good team. IDs, assigned roles/words/King, roster, handoff and complete history remain unchanged through each measured transition. Each previous file equals the exact preceding primary. Both generations of `code7-before-next-update-state` equal the word-judgment result pair.

These measured cases use explicit replay manifests `code7-timing-sync-replay` and `code7-word-repeat-replay-replay`. Their expected fixture hashes match the actual earlier saved files, and their preserved-before hashes match the saved backup copies. They are bounded native regression observations, not three new fully played matches or a proof for every timing window.

## Initial unmeasured attempt remains unresolved

`code7-doubletap-good-king-result-state` actually contains a new match **`b1c304f0de6b442f92984a984a92c939`**, phase0/handoff0/outcome null, Parkplatz/Parkhaus. Its primary SHA256 is `f0fdbcf77e3f64ad9291ea99fd017cf9b6a5e043268b0503d8a1c76c7eb52332`; backup is the old match's terminal result `9153d5cdab17d839809a6c75e64b2a4e6fcb7ea3701e1b8a92386b2b65c92155`. Offline image inspection confirms `code7-doubletap-good-king-result.png` is covered Nora/card1of5, not the intended result screen.

No actual event-timing record exists for that initial attempt in the supplied evidence. The unintended rematch observation is retained as unresolved timing evidence. The subsequent measured cases do not turn the original attempt into a pass or establish its cause.

## Public Result → Group still fails on Code7

`code7-result-public.json` has **31 nodes / 24 nonempty semantic values**. Both immediate and settled Group captures contain **seven framework nodes, zero nonempty text/description values**, and a final disabled/invisible node with zero bounds and no actions. Both dumps explicitly preserve accessibility services. The saved service record has TalkBack bound and touch exploration enabled.

The offline-inspected screenshot displays the real German Group with **5 playing**, Nora and Luca visible, selected Kings mode, entry field and start controls. This is not merely a screenshot of a blank screen or the synthetic twenty-player fixture. Root associates these retained records with the actual result→Group action; this audit did not acquire a new post-return save or query the device.

| Retained transition evidence | SHA256 |
| --- | --- |
| `code7-result-public.json` | `84cbf3fa5041483e4ac71917abfa13c34ec9fdc5dd686224ad1b6ffc1cf39352` |
| `code7-result-to-group.json` | `8a27e014f8b9dd875adb7dcf61d737036c1c735d72b9817dfab61b73022e1928` |
| `code7-result-to-group-settled.json` | `8a27e014f8b9dd875adb7dcf61d737036c1c735d72b9817dfab61b73022e1928` |
| `code7-result-to-group.png` | `6deac13b933b28b0a0228b01c204fe49e8e4ec55b63643892cc67544551e97ff` |
| `code7-result-to-group-services.txt` | `5b062d2f18bddec5c76ca9f6241e3a0e2801347babf0001022f97f77a8a1f32b` |

This remains an actual **Code7 accessibility defect**. Empty private-card nodes alone are not a success criterion for public navigation. Neither a later redraw nor a green rendered test retroactively makes this transition pass.

## Later source correction and pending gates

The later correction **`85fa17555b44bca61640bcb13a738c3e37ffc34f`** lets the panel scheduler run before waiting until a later frame to rebuild public accessibility. Exact source blob hashes/line excerpts are in the JSON. The retained late-frame test has a behavioral RED: completed1, passed0, failed1, message “A newly visible Group must restore its public controls and nested player list without another redraw action” with an empty actual string. The GREEN of the same named test is completed1, passed1, failed0. The test source uses end-of-frame transitions; the reviews cover Group returns and subsequent start at5/20 participants.

`late-layout-standards-review.md`: **0 new violations, 0 new reportable baseline smells**. `late-layout-spec-review.md`: **0 findings**. Both reviews identify the exact later source pin and explicitly withhold corrected native acceptance. Their hashes and complete retained text, together with RED/GREEN result JSON, are frozen in this addendum's machine-readable evidence.

**Code8 corrected-native accessibility transitions, final source-pinned package verification and final release acceptance remain pending within this addendum.** No Code8 captures are read or inferred here. The separate `acceptance-matrix-draft.md` maps all56 stories to prior source/rendered and bounded native evidence; it is a coverage draft, not a release assertion. Earlier mislabelled images, failed interaction attempts, Classic-fixture provenance, silent-emulator/physical-device limitations and the original467 audit remain intact.
