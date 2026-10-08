# Code8 native addendum — first cold repeat and five-player public navigation

Frozen offline evidence audit generated 2026-10-08T21:31:11.848177+00:00. **256/256 consistency checks**, 24 saved generations. Original467 and the separate Code7 audit remain byte-identical. Checks include preserving non-passing observations; this is not a final release score. No ADB, Editor, build, push or production-source operation was performed.

## Actual update and first cold repeated confirmation

`code8-update-byte-equality.json` agrees with independently compared Code7 pending baseline → Code8 before-first-launch files. Both are V5: primary **`1b7a964802c292756d77195ceefa622f0d6e0b274113975218254d9c20c3abe2`**, previous **`687e3835c5b2930d952c10445fb0f59f1eb21af008f7606fa14ff68438096239`**. The package record says version1.1.0/code8, min26/target36. The replay-derived pending baseline's provenance remains explicit; the install/update itself is real.

`code8-first-cold-doubletap-timing.json` contains four successful device events, DOWN/UP/DOWN/UP, injectionMode2, returnCode0. The measured first-UP→second-DOWN interval is **76ms** (DOWN→DOWN **154ms**). The result remains match **`69fc9f23a2b44963b61cc5e954f24af4`**, phase3/Result, outcome6/GoodKingEliminated, primary SHA256 **`9153d5cdab17d839809a6c75e64b2a4e6fcb7ea3701e1b8a92386b2b65c92155`**. Roster/history are unchanged; previous equals the exact pending primary. This is a bounded measured repeat pass and does not erase the earlier unmeasured Code7 unintended rematch.

## Initial accessibility attempt: zero actions, low-memory termination

`code8-five-public-sequence.json` is **pass=false with actions=[]**. Its initial capture has 9 nodes and only the retained initial semantics; no completed public-navigation sequence is claimed for this attempt. Android's saved `ApplicationExitInfo #0` says **2026-10-08 23:19:53.427, PID13388, reason3 LOW_MEMORY, status0, RSS543MB**. The saved crash buffer and fatal-log files contain no non-whitespace content. The reason identifies the OS termination category; it does not diagnose an allocation leak or prove memory robustness.

`code8-low-memory-preservation.json` independently checks out: both generations after termination are byte-identical to the cold-repeat result. Primary **`9153d5cdab17d839809a6c75e64b2a4e6fcb7ea3701e1b8a92386b2b65c92155`**; previous **`1b7a964802c292756d77195ceefa622f0d6e0b274113975218254d9c20c3abe2`**. This establishes retained committed state, while the interrupted run remains not passed.

## Eight actual public actions after restart

Every action's raw stderr reports `NAMED_APP_ACTION click success=true`; the audit additionally checks the saved resulting tree, exact visible semantic values, nonzero visible semantic bounds, snapshot checksums/hashes, roster and selected mode. All dumps use `dontSuppressAccessibilityServices=true`; the retained service checkpoint records TalkBack bound with touch exploration enabled. This is native hierarchy/action evidence with the service retained, not an audible or blind-user evaluation.

| Step | Actual public action | Resulting nodes | Primary state SHA256 |
| --- | --- | --- | --- |
| 1 | Gruppe bearbeiten | 35 | `ba887accc6fc702118b7890ba41094748f1bf0aeb948494b05c803e159d1ac2a` |
| 2 | Schnell | 31 | `e8f50cda4b5b25db4322025611a8c01c97654e12134e54ee8daa4f0696cfd96e` |
| 3 | Klassisch | 32 | `a73575d5b4948afde5b61daa9c90bb18b177a18d615424f4ccbea0df7d4e46fa` |
| 4 | Könige | 35 | `ba887accc6fc702118b7890ba41094748f1bf0aeb948494b05c803e159d1ac2a` |
| 5 | Los geht's | 16 | `02af7bea8b2efd254805bfcb47ed6aef312aa62bb3d730a701ac13e475fc0268` |
| 6 | Zurück | 15 | `02af7bea8b2efd254805bfcb47ed6aef312aa62bb3d730a701ac13e475fc0268` |
| 7 | Partie abbrechen | 15 | `02af7bea8b2efd254805bfcb47ed6aef312aa62bb3d730a701ac13e475fc0268` |
| 8 | Ja, Partie abbrechen | 35 | `5d39919e4d0955f0267d1c9c787c506f49de64f8dd9bb3f016b72e2100065450` |

The direct result→Group return now contains35 nodes and expected names/public controls; no extra language redraw is needed in this recorded sequence. Switching Quick/Classic/Kings yields31/32/35 nodes. Start yields16 nodes for the covered first card. Back and abandon confirmation each yield15; confirmed abandon restores35 Group nodes. All five saved player records are unchanged, including Jonas2's stable ID. Back and the confirmation dialog preserve both generations exactly; confirmed abandon clears Match and retains the Group. These five-player observations address the previously failed transition on Code7. Twenty-player and private-card retests are separate.

## Retained log rescans

| PID | Nonempty lines | Saved bytes | Distinct literal values tested | Literal hits | Saved SHA256 |
| --- | --- | --- | --- | --- | --- |
| 13388 | 87 | 11735 | 13 | 0 | `9150160251830f71033be8f4b45633450e7ce78a5c74fb5068a82aa566f441d9` |
| 14363 | 90 | 13195 | 16 | 0 | `0d4eb978cb72a995eec096eb76de24fa89e6160afc386529619d85b7efb336b0` |

The killed PID13388 scan covers the actual five player names/IDs, match ID and Ratte/Maus. The later PID14363 scan also includes the actual new handoff's Parkhaus/Parkplatz and new match ID. Both retained logs contain only their expected PID among parsed log records. Neither contains a tested private value or the explicitly listed crash/exception patterns. The JSON retains every W/E/F severity line, including emulator EGL/platform warnings and the `session.json` link-denial diagnostic; **this is not a claim that logs contain no warnings/errors or that LOW_MEMORY was harmless**. The exact scanned values/patterns and raw-file hashes are recorded.

## Evidence limits and remaining gates

The first stationary max-card image `code8-max-good-header-held.png` remains **covered according to root's visual observation** and is not private-visible proof, even if the helper's text checks pass. Its exact SHA256 is `bb0bc83e38d66c86f503a646e0ba720b9681ec5b21cd202f63d20802f552ceb5`. Root is separately collecting actual pull-held/released and list-bottom evidence plus twenty-player public transitions; none is inferred from this initial frame or from the five-player pass.

Code8 maximal private-card/20-player native checks, final source-pinned APK/AAB hashes/inspection and final release acceptance remain pending within this snapshot. No physical-device, spoken TalkBack, blind-user usability, haptic or human group-balance claim is made. The full input hash inventory, all snapshot states, raw trees, both comparison reports, timing events, exit record and bounded scans are in `code8-native-audit.json`.
