# Code9 native update and initial group checks

Intermediate candidate1.2.0/code9, sourcee106ce6f388a4e2dce834e4c2fe2b7092b61d47a, APK SHA256cae1f76c5811aee2519ab137ffbdb47defb2b6dee9bc13f200a9d9e45c302b4d. This is the owned translated ARM64 Android36 emulator, not a physical phone. AAB build ran on the host during these checks; no performance measurement is attributed to them.

- Actual install-r updated code8 directly to code9 without uninstall/data clear. code8-candidate-update-verified.json records the exact artifact and byte-preserved primary/previous saves. The subsequent code9-opened-update and code9-settled-update checkpoints also equal both originals. Five identities, names/order, Kings preference and12-word history entries remain unchanged.
- Initial capture shortly after Activity startup was still the blank startup surface; retained as code9-after-update.png. A later settled capture shows the group. This is not a measured startup-time comparison.
- Root viewed code9-after-update-settled.png against code8-original-group.png. Explicit Edit appears correctly, but the native reorder symbol is visually heavy and the expanded hint consumes an extra line, reducing the visible list. This late visual finding was sent to the single correction implementer: font-independent light grip, one short localized hint, newcode10 packages. Code9 is therefore an intermediate package, not the final accepted visual result.
- An8px name swipe followed by a name tap (code9-small-name-swipe-native-input.json) leaves the ordinary group visible, without editing or keyboard. Its viewed screenshot and both byte-equal save checkpoints support this result.
- Explicit Nora Edit opens the real Gboard keyboard. code9-open-edit.png was viewed; code9-open-edit-ime.txt records the native IME. All edit actions are visible at normal text size.
- Android input entered synthetic draft NoraEntwurf9, then KEYCODE_BACK. code9-draft-back.png was viewed: keyboard/editing closed, Nora unchanged. Both committed save files remain byte-equal to the originals; Back did not submit the draft.
- A longer name drag, y800→570 atx500 (code9-name-scroll-native-input.json), scrolls from Nora/Luca to Emil. Root viewed code9-name-scroll.png: no editor/keyboard opened. Both save generations remain original, proving no durable reorder/rename.

All reported screenshots were actually viewed. Finalcode10 update, actual handle reorder/removal/undo/rename persistence, motion measurements, continuous return/regrab review, large text/legacy compatibility and native accessibility remain outstanding. The code8→9 update record is retained when the subsequent9→10 update is checked; fixture replay is prohibited until the chain reaches the installed candidate. No free-role rules are implemented or accepted here.

---

Lesefassung mit dauerhaften relativen Links. Das [unveränderte eingefrorene Original](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/native/code9-functional-report.md) bleibt einschließlich früherer Formulierungen erhalten. Die aktuelle Updatebeschreibung unterscheidet den vorherigen Originalrestore vom eigentlichen Installationsversuch.
