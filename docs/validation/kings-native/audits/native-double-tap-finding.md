# Native Android finding — 2026-10-08

Candidate 1.1.0/code5, build source `908d95fc80fc8173df02593422ff85264089b945`, integrated/reviewed HEAD `7e0c60a0d8c7d92dab6d471c49363f72aa55b960`.

On root-owned AVD `word_deduction_api36_16k`, actual touch input with `work/kings-native/DoubleTap.py 540 1535` (75 ms between taps) on the confirmed good-King elimination performs the elimination and immediately starts a rematch. The second tap hits the new result screen's Rematch action at the same location. A player trying to confirm a consequence sees the next private handoff without a useful result screen.

Evidence under `work/kings-native`:
- `code5-game1-corrected-nora.png`: pending elimination of Nora.
- `code5-game1-good-king-result.png`: despite its intended label, the actual image is the new match's first handoff; this is failing evidence, not a result pass.
- `code5-game1-result-state/session.previous.json`: completed original match `69fc9f23a2b44963b61cc5e954f24af4`, phase3/outcome6, Nora good King and Emil eliminated, Ratte/Maus.
- `code5-game1-result-state/session.json`: new match `9f9a692f38424311b104a061b418d17a`, phase0/handoff0, Parkplatz/Parkhaus.
- `code5-game2-deal-state`: same new match; later test actions did not create it.

Fix the cross-screen activation at the rendered input boundary while preserving deliberate one-action rematch. Reproduce with actual rendered pointer events first, test native later with root. Include last-chance answer/target/result transitions in reasoning; a quick repeated tap must not silently make a different new-screen decision. Do not merely assert that stale Session commands are rejected: these are two distinct valid commands on different screens.

Root retains exclusive ADB ownership. Do not modify/replay these native files. Final code5 native acceptance remains incomplete and code5 is not the final deliverable.
