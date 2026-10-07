# Android local release candidate: native coordinator validation

Code2 candidate test round completed with corrections required. Only the observations entered below count as completed evidence. The separate `polish3-offline-report.md` covers the earlier accepted development APK, not this release artifact. A subsequent pinned code3 candidate will receive targeted regression tests before release acceptance; this report does not claim those results.

## Environment and boundaries

Own AVD `word_deduction_api36_16k`, Android API36 Google APIs ps16k x86_64 image revision7; Emulator37.2.12, WHPX, host GPU. ARM64 APK executes through `libndk_translation.so`. The environment reports PAGE_SIZE16384; `bionic.linker.16kb.app_compat.enabled=false` and `pm.16kb.app_compat.disabled=true` were verified before installation. This is 16KB application alignment emulation over an x86_64 host/kernel arrangement, not a physical ARM64/16KB-kernel test.

Actual display1080×1920/density480, 360×640dp. ADB server5038, shell transport127.0.0.1:5583, emulator console5582. Each helper verifies the AVD name first; screenshot helpers reject emulator KO responses and stale image timestamps. Touches are actual native events; held cards use the separate test-OS input helper, never a bundled production test endpoint.

The test OS permits per-command `su 0` for read-only inspection of private saved files after installing a nondebuggable app. This capability belongs to this userdebug emulator, not to the app. The stopped older4KB AVD and its original eight-person match remain untouched.

First-boot Android System UI, phone-service and GMS ANRs predated the app's earlier development playthrough; those are OS-process events, not inferred game failures. Any new runtime failure is assessed by timestamp/process. Timings are emulator observations, not phone-performance or human-fun claims.

## Artifact identity

Candidate source `ef261b030a0396e7deca55a0511b5d31c2081874`; local-release APK39,431,579bytes, SHA256 `4d7f9b2d7eeceb5df3669025976276cb71bb2ba6e8eb4591019a796578d49ec0`. Root recomputed that hash before installation. Build succeeded264.294s, zero errors/two warnings per its build summary. The candidate is nondevelopment but deliberately signed with the local Android debug certificate; it is not a production-signed Play upload. Installed facts follow from the native probe rather than this supplied build identity.

## Update from an actual saved match

Baseline was created through actual name entry and Quick gameplay in the accepted development APK, then a one-action next match. It contains Alex, Bea and Cora with stable IDs and match `e9de3e06edd647a4852114d7bb1f4ec9`, Quick/English/Handoff0; used pairs mammals-013 and games-leisure-022. Both generations were preserved before restart and verified byte-equal afterward. They were again checked through read-only test-OS `su` before the release install:

- session.json: `58ae0a899dc2f1563e50dcea470e0e3a237949db59130670050f1158560c163d`
- session.previous.json: `a6a1a20662e5c8cb055d842aebb86307aa3426f6c2d91084736d8349b7fe73a1`

Actual development version0.1.0/code1 → local release1.0.0/code2 update succeeded. Installed SHA256 matches; package flags are HAS_CODE ALLOW_CLEAR_USER_DATA, without DEBUGGABLE, and run-as is explicitly refused. PAGE_SIZE16384/backcompat-disabled/offline values remain verified (`release-installed-environment.json`). Both saved generations match exactly before first launch, after launch, and after explicit Resume. Root opened `release-update-settled.png` (safe Pause) and `release-update-resumed-card.png` (Alex, Card1of3, covered); no new draw or progression was persisted.

## Fresh offline games and native UI

Actual uninstall and fresh install of the exact release APK succeeded while airplane mode remained on and active default network was none. Installed flags have no DEBUGGABLE; run-as refuses. First-install cold samples at4.536/5.092/10.096seconds still show loading;15.106seconds shows the empty group, no account/onboarding/permission prompt. Root personally opened all four. This is distinct from the earlier data-clear probe.

Info/privacy and licenses were opened and scrolled in EN/DE, with Android Back returning to the group. Actual font_scale1.5 on360×640dp exposed the two findings recorded in release-native-findings.md: missing disabled-Play reason and clipped German empty-group instruction. The longer privacy/license views remained readable and scrollable. Some initial immediate Home→Start captures had not applied font scaling; only the later confirmed images establish the large-font observation.

Alex, Bea and Cora were entered through actual focused Android name fields, Gboard/input text and Plus, then keyboard Back. Fresh English Quick dealt Mouthwash to Alex/Bea and Toothpaste to Cora. Root viewed each covered card, actual held or slowly dragged reveal and release. Next remained unavailable while held and became available after concealment. No initially unknown Civilian/Undercover label appeared. Match643f2cd3d4854cf89c2aeb142175ea34,care-health-002,Handoff2 survived a force-stop/relaunch with both files byte-equal: primary1fdc01bb8ccdf1c56f98f03f3973bd9e80b5e7227dd33057c631e1bca3be265a, previous999dc3a0133695152f4a9ea69b10ceae0b814671f45d2d8d2b43ab1225e0d772. Root viewed safe Pause and then Cora covered with Next disabled until a fresh reveal. Only one used pair remained. Cold samples7.176/7.305/10.140/15.110seconds were all still loading (2s/5s filenames are requested sample times, not actual completion); the later settled frame is ready without an exact readiness bound. These are ARM64-translated emulator observations, not physical-phone performance.

English Quick selected/confirmed Cora, showing Civilian win, reason, Mouthwash/Toothpaste and all three matching roles via actual result scroll. One Next match tap went directly to covered Alex/Card1of3: same stable participant IDs, new match7d8575f825274208ad15cac2dcb89539 and pairgarden-nature-013 (Parsley/Basil), two used pair IDs total. No intermediate setup.

After explicit abandonment confirmation the group remained intact. DE was selected; Dani and Emil were added through the focused name field/Plus without dialogs. Classic showed4Civilian/1Undercover/0White, then3/1/1 after enabling White. All five real cards were personally viewed held/dragged and released: Alex/Bea/Emil Steckdose, Dani Ladekabel, Cora Mr. White with concise bluff instruction. Round1 legitimately selected White/Cora as starter. Dani was selected then confirmed; elimination showed Undercover with no secret word and4remaining. Round2 selected Emil and offered only Alex/Bea/Cora/Emil for voting. Cora was confirmed; the pending White guess showed no target word and semantic group judgment.

White guess matchd015858b2a374f62be3e25f81952d321, technology-008,Round2/Phase5 survived force-stop/relaunch byte-equal in both generations: primaryac4551eaf760927ca8ca15f0a803519d440119097b559ab8c86308876c4348bf, previousb587984fa2ac1fe8d0bb5e26ee891ab6139d94e8f0359ef21433f108b615b732. Cold samples4.591/5.101/10.111seconds were loading;15.111showed safe Pause, all personally viewed. Resume returned the same unanswered guess. Incorrect produced Civilian win because all adversaries were out, with correct reason and role assignments. One Next match immediately included all5 again, including eliminated Cora/Dani, with the same IDs and fresh pairtechnology-023. Root viewed covered Alex/Card1of5. Preserved update baseline `release-pre-correction-state`: matchb7203215ad014899bfda2147685f8ddd,ClassicDE/Handoff0,4usedpairs,primarybcd07d9c71791a7d7119b625486ddee28fbac955a4feb5a58f51b69923a5ca94,previous7be2f81ff7a376e0e16485af16fc66aa350468989af8c00c9e7a55a5f469de38.

## Runtime, privacy and latency

Initial post-update COLD launch returned from am in3.783s; captures completed4.078/5.409s still show loading, not a ready app. The later settled image is ready but does not establish an exact cold-readiness time. A separately timed empty-data cold start is still loading at5.094s and visibly ready at10.094s (also15.104s); all four images personally opened. That empty-data probe is explicitly separate from the following full uninstall/reinstall first-install gate.

Three release same-process Home→foreground returns (PID6595) are personally viewed and show the safe Pause screen by2.101/2.099/2.100s. These are capture-completion upper bounds, not first-ready times or phone benchmarks. First image includes a transient top Android status-bar trace; app text and controls are present and clear. A prior probe failed before any gameplay command when Unity's deployment scanner disconnected the other-SDK ADB server; its copied stale development outputs were replaced and are excluded. Release evidence is the successful PID6595 JSON/images only.

Unity's own Editor log identified that scanner behavior. Root verified private port5038 listed only this AVD and its TCP alias, restarted only that server using the already installed Unity-bundled ADB36.0.0 executable, reconnected and confirmed the same appPID alive. Helper paths now use that binary; no default5037 server, Nischenreich setting, or global SDK/PATH configuration changed.

Release launch log identifies Build type Release/IL2CPP/ARM64 and shows GLES3.2→supported-context fallback, native-library name lookup retries, and ordinary engine startup messages. Player-log=false does not mean Android logcat is completely empty. Captured UID10214-only log104757bytes covers app processes7376/7982/8611. Search found no dealt word/role values, FATAL EXCEPTION, managed NullReference/Argument/OutOfMemory or SIGSEGV/ANR markers. Warnings include translatedCPU/GLESfallback, keyboard timeout/inactive connections, surface transitions, failed hard-link attempts preceding successful save fallback, and Swappy native lookup. They are retained rather than called an empty/error-free log.

The privacy scan FAILED for personal names: native `gti.InputConnection` debug lines log Alex/Bea/Cora/Emil during actual input. This new P2 is recorded in release-native-findings.md and assigned to the same final correction implementer. No acceptance waiver or device-global log suppression is used. A fresh installed-build input/log test is required after the fix.

The actual held Basil card → Android Home → Recents → app foreground probe passed with personally viewed `release-privacy-settled-*` images: held word, launcher, completely blank task preview, safe Pause on return. The preceding short-wait probe's Home/Recents-labelled images captured unfinished OS transitions; these do not establish those surfaces and are excluded. The corrected sequence used120ms hardware-key presses and3seconds settling before captures, with the touch still held until after the task preview.

## Store images and final status

Final store images will be captured from the corrected installed code3 artifact. The candidate images above are genuine unedited app captures, personally viewed during this round. They are retained as evidence of both successful behavior and the findings; they do not imply code2 final acceptance. Store images must have no development watermark, fabricated gameplay, or edited phone-frame proportions.
