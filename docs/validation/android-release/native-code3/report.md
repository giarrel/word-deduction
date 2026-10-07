# Corrected Android release: coordinator native acceptance

Status: code3 candidate tested; an additional word-typography correction requires code4. This report records only the exact checks entered, not planned tests as passes. Despite the historical `final-*` capture prefix, these observations identify code3 and are not substituted for code4 evidence.

## Scope and device

This targeted round follows complete fresh offline Quick and Classic/White playthroughs on code2, recorded in `docs/validation/android-release/native-code2/release-report.md`. The independent final reviews and the single correction implementation retain separate evidence. Changes requiring native rechecks are accepted Unicode label restoration, role-count policy consolidation, small-screen large-text guidance, and removal of native input debug logging from the release package.

Own AVD `word_deduction_api36_16k`, API36 Google APIs ps16k x86_64 revision7; Emulator37.2.12, WHPX and host GPU. ARM64 runs through native translation. PAGE_SIZE16384 with application compatibility fallback disabled is an application-alignment probe, not a physical ARM64 or true16KB-kernel result. Actual1080×1920/density480 gives360×640dp. No physical-phone performance, haptics, spoken TalkBack or human-group enjoyment is inferred.

## Update gate

Before installation, the code2 app has five stable people, Classic/DE with White enabled, covered Alex/Card1of5. Match `b7203215ad014899bfda2147685f8ddd`, pairtechnology-023, four used pairs. Write-once `release-pre-correction-state` preserves both generations: primarySHA256 `bcd07d9c71791a7d7119b625486ddee28fbac955a4feb5a58f51b69923a5ca94`, previous `7be2f81ff7a376e0e16485af16fc66aa350468989af8c00c9e7a55a5f469de38`.

Actual code2→code3 `install -r` succeeded. Installed1.0.0/code3,min26/target36 has no DEBUGGABLE flag and matches APK SHA256 `50e18dadc4764a807f2f4109d6ddda193dd471c7f91ff3cd4f810c1bbff532e3`,37,422,994bytes, clean source `b86f19564b1d4c2fa31f02d7872dca7e835d881a`. Offline/PAGE_SIZE16384/compatibility-disabled settings were re-read. Both primary and previous generations remain byte-equal before installation, after installation before first launch, after startup and after explicit Resume. Root viewed the safe Pause and covered Alex/Card1of5; no new deal or history consumption.

During a concurrent AAB build, the initial cold samples8.581/8.961/10.257/15.313seconds were loading. The2s image and identical-hash5s/10s/15s image were personally viewed; later `final-update-settled` was still loading and `final-update-ready` showed safe Pause. No exact cold-readiness time or physical-phone speed claim is made from this loaded-host run.

Actual Alex hold revealed the correct White instruction; release concealed it and enabled Next. Bea's card appeared covered after one Next; a slow native drag correctly revealed Sprachnachricht and release concealed it. However, the word wrapped as `Sprachnachri`/`cht`, a confirmed visual defect despite fitting inside the card. The same correction implementer is improving word fitting and will produce code4. The exact naturally dealt match at Bea/Handoff1 was preserved as `final-long-word-state`: primary `f1589d992239c3c9d34f24866d122417f18b0f26985fe6d23a6240f8e8f7bc0a`, previous matching the original primary. It can reproduce the typography without inventing a production test endpoint.

## Corrected behavior

After preserving the naturally played match, this own test app alone was force-stopped and cleared for a fresh-data UI probe; no uninstall/reinstall is claimed for code3. Font scale was set and read back as1.5. Offline startup reached the empty English group without prompts. During the concurrent build, samples10.218/10.341/10.459/15.094seconds were loading; later actual ready captures establish the UI but no exact readiness time. Root viewed the unique sample images (the first two have identical hashes).

Actual360×640dp/150% English empty group shows the complete first instruction and `3 more players needed`; German shows `Ersten Namen eingeben.` and `Noch 3 Personen.`. The immediate `code3-empty-large-de` image still has English content during the transition; the subsequent `code3-name-keyboard` captures the settled German empty screen before the IME arrives. Do not mislabel the former as a German pass.

Native focused fields/Gboard and explicit Plus saved Nora, Luca and Emil. Android Back dismisses the keyboard. Root personally viewed typed input and saved rows. Quick with2 now shows `Noch eine Person.` / `Add one more player.` beneath disabled Play; Classic with3 shows the same correct reason and the disabled White availability explanation. All four actual native images were viewed. The first `code3-classic3-large-en` attempt still showed the keyboard after a premature automation transition; `code3-classic3-large-en-confirmed` and `code3-classic3-large-de` are the actual checked states.

The actual new app process9588 log contains254lines/30,715bytes. `ReleaseLogScan.py` captured this PID without clearing device logs and searched the actually entered Nora/Luca/Emil strings, GameTextInput text diagnostics and fatal/managed-exception markers: zero matches. Ordinary engine/platform/IME/SELinux-save-fallback warnings remain visible and are not called an empty log. This closes the demonstrated code2 personal-name logging defect for code3; the exact later code4 package retains its own DEX and native smoke gates.

## Visual review and store captures

Pending personally viewed unedited native captures from the installed code3 APK. Selected captures will be copied to `docs/release/store/screenshots/` with source/hash provenance.

## Outcome and boundaries

Pending. Local signing is the Android debug certificate, so the APK can be installed for testing and update its preceding local candidate. Dedicated production signing and Play publication are separate owner steps described in the prepared handoff.
