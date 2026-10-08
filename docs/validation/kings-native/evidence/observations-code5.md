# Code5 Android observations (in progress, not final acceptance)

Exact source908d95fc; APK SHA afd739cc5b8356a24ee9b2b92c7639ba128569d3f43090ac961fc0e6a3d24a01. Installed over real code4 without fixture replay. AVD word_deduction_api36_16k, 1080x1920/density480, 16KB pages, ARM64 via translation, offline. This is emulator evidence, not physical-device or human balance evidence.

## Real update and old Quick

Both real save generations remain byte-identical across install and initial launch/resume. Original Quick match5a845f1c7f8842789e784e1cc6a096b5 resumes at Luca (Nora already complete), Hosentraeger/Guertel. First actual handoff completion preserves every legacy field except Handoff1->2 and migrates V4->V5 while retaining old primary as backup. See actual-update-verified.json, code5-update-bytes.json, code5-migrated-advance.json. Emil's final handoff, Quick vote for Nora, good win and one-tap rematch performed through real native controls. Rematch pair Digitalkamera/Sofortbildkamera. Deliberate abandon retains original names.

## Group and private cards

Added Mila and Jonas consecutively with actual keyboard; Kings minimum5 visible. Pause/restore Jonas disables/re-enables start without losing names. Inline renamed Jonas->Jonas2 (stable-ID verification pending). Game1 69fc9f23a2b44963b61cc5e954f24af4: Nora good King; Luca White/evilKing; Emil+Mila ordinary good; Jonas2 ordinary evil. Ratte/Maus.

Actual held/dragged cards opened and visually checked: goodKing role+word+unmarked Luca/Jonas2; White role/no word/teammateJonas2; ordinary Emil Ratte/leaderNora and Jonas2 Maus/leaderLuca share neutral styling. Release covers all. Home returns safe pause; Recents preview covered and returns safe pause; cancel covers; second pointer covers and repeated second-pointer contact does not reopen. Force-stop unfinished Nora and resume keeps both generations byte-equal (code5-unfinished-nora-restored-comparison.json), opens covered; Nora can reveal same assignment. No fixture replay yet.

## Actual matches

Game1: ordinary Emil eliminated, only 'Kein Koenig' shown, parity continues. Pending Mila cancelled and changed to goodKingNora. Actual75ms DoubleTap.py5401535 produced outcome6 but immediately hit Rematch: FAIL, reported in ../kings-implementation/native-double-tap-finding.md. code5-game1-good-king-result.png is really new handoff, not a passing result image. Old terminal is code5-game1-result-state/session.previous.json; new primary is game2.

Game2 9f9a692f38424311b104a061b418d17a: Nora goodKing, Emil White, MilaUC, Luca/Jonas2 ordinarygood; Parkplatz/Parkhaus. Real handoff loop guards every committed step. Eliminated Luca then Jonas2: parity and lonegoodKing with3survivors both remain live. Eliminated Mila: exacttwoKings result, evil team wins. Opened result heading/rolelist images; all5 participants including eliminated remain in final disclosure. Intentional singleRematch creates game3.

Game3 5dfed8d3df3c43c4b3f0b2230ecc9b2b: Mila goodKing, LucaWhite, NoraUC; Frosch/Kroete. Whiteelimination -> choice -> committedword. Force-stop/relaunch/Back+Resume preserves branch bothgenerations byte-equal (code5-game3-branch-lock.json). Spoken-answer confirmation reveals onlyFrosch for judgment. Second force-stop preserves bothgenerations (code5-game3-answer-lock.json). Several identical-coordinate ADB taps5401535 on safePause did not resume;8001560 worked. This is an input/readiness observation, not established app defect. No logdiagnostics; nativewindowfocused; TalkBackdisabled. Intended code5-game3-judgment-resumed.png and code5-game3-word-correct-result.png still show pause and code5-game3-result-state is NOT terminal. Actual resumed judgment is code5-game3-resume-alternate-point.png. Actual Correct outcome is code5-game3-word-correct-terminal.png with code5-game3-terminal-state. Evilteamwin, notsoloWhite.

Game4 43661f7a969a44deb5807563b35cb693: Mila goodKing, EmilWhite, NoraUC; Linsensuppe/Tomatensuppe. Eliminated ordinaryevilNora -> neutralnotice, thenWhiteEmil -> word -> answercommitted -> Incorrect. Actual code5-game4-word-failed-result.png and code5-game4-result-state show goodteamwin.

Game5 3ceda517d0814ae3bad868244c417a46: Jonas2goodKing, MilaWhite, NoraUC; Heft/Notizblock. Allhandoffs, Whiteelim, committedKingbranch, pendingwrongtargetNora. code5-game5-target-pending-state saved; force-stop/relaunch current. code5-game5-target-safe-restart.png captured startupblank (notpass). Still need safeResume, pendingtargetequality, correction toJonas2, correctKingresult.

## Logs / limitations

Per-PID nonempty logs tested exact actual names/words, no diagnostics or sensitivevalues: PID6631(202lines26124B),7535(116lines20972B),8161(93lines12467B),8292(108lines18551B). Files code5-first-process-final, code5-before-word-restart, code5-before-judgment-restart, code5-before-target-restart. No global log clearing. Current newPID not scanned yet.

Device.ps1 Capture parameter is -Name, not -Label. Mistaken -Label produced capture.png; preserved and copied to intended code5-first-kings-restart.png. Subsequent native controls use correct parameter. Wait until native process command has returned PS prompt before next input; input sent while an external command runs may be lost. ADB amstart completion does not mean Unity surface interactive; inspect actual settled image before action.

## Later completed actual games and public group

Game5 safe resume retained both checkpoint generations exactly (`code5-game5-target-persistence.json`). Corrected pending Nora via Other person to Jonas2; correct King result is evil-team win (`code5-game5-king-correct-result.png`, `code5-game5-result-state`).

Game6 `83662afc639e45889146d1775cfb3d26`: Nora good King, Jonas2 White, Luca Undercover; Arbeitshandschuh/Schutzbrille. Eliminated ordinary good Emil, then White, chose King branch and incorrectly targeted Luca. Good-team win including eliminated Emil (`code5-game6-king-failed-result.png`, result-state).

Game7 `01dd1170be424ef082b9b81e4681d6bc`: Jonas2 good King, Emil White, Nora Undercover; Aubergine/Kürbis. Single confirmation of good King elimination correctly stays on evil-team result (`code5-game7-good-king-result.png`, result-state). All six distinct endings have now been observed through actual Android gameplay. Edit group retained all five active players; EN switched the full public group interface.

Final actual-games PID8810 scan (`code5-seven-games-final-log`) contains109 lines18879 bytes and no private-value/diagnostic hits. Only afterward began explicit synthetic fixture replay; the original English five-player group generations were preserved in `code5-german-good-max-replay/before`.

## Maximum synthetic fixtures and TalkBack

Explicit Session-generated replay is separate from actual gameplay. Font1.5 at360x640dp, twenty players/eight ordinary Undercover plus White. German good King: owner24W ·10 wraps without losing disambiguator; all nine known names reachable by held scrolling. Accented Latin, CJK, Greek, Cyrillic and emoji rendered on native Android. Root inspected header, middle/first names, Unicode/lower and bottom frames; released frames cover content. English White and German White headers and final teammate list likewise inspected and readable. English good King run pending visual inspection at this note.

TalkBack16 installed, enabled on the isolated AVD. Its initial notification permission dialog caused safe app pause; declined notifications using native input. `code5-talkback-bound.txt` confirms bound service, touchExplorationEnabled=true, serviceHandlesDoubleTap=true. Real native accessibility actions opened German Help and scrolled; two-pointer gesture moved rules from first section to card explanation while public focus remained Back. This establishes action/gesture behavior with service bound, not audible output or private-card gesture discoverability.

`HoldSemanticSample.py` captured actual held/released card frames and native hierarchy without suppressing TalkBack. German good King and English White kept role, words, leader/private participant lists out of Android descriptions/text; Next disabled while held. Direct injected card gestures are not attributed to a user's TalkBack pass-through gesture. `MaxFixturePass.py` repeats guarded fixture replay, waits for observed Resume node, renders held header/bottom and checks forbidden private strings against complete native hierarchy. German White pass/log clean. Root visually inspected its header and bottom.

Initial German good fixture log scan accidentally supplied Nektarine instead of actual Aprikose; retain raw log and include actual Aprikose in the later offline audit. Current code5 remains a failing candidate because of the documented rapid repeat. Code6 correction/native regression, setup clamp and explicit oldClassicV4 compatibility remain pending.

## Final code5 observations and regression baseline

English good-King header and bottom images were visually inspected: Peach, role and final duplicate-name disambiguator are readable inside the private viewport. MaxFixturePass initially reported known W... ·1 as a forbidden substring of the legitimate public owner W... ·10. This was a harness false positive, not a disclosed teammate; the raw aborted run is retained, and full semantic-node value checks plus independent offline audit replace that substring check.

Two deterministic native public accessibility defects at visibility transitions are one finding: Match→Group after confirmed abandon and Group→Match at start both produced an empty seven-node framework tree with TalkBack still bound. Subsequent reads remained empty. Same-panel redraw (language change on Group or Android Back on Match) restores the expected64/15 nodes. Captured full repro in `../kings-implementation/native-accessibility-transition-finding.md`; implementation correction pending. Group after redraw remains accessible:20→19 active keeps8 ordinary Undercover;19→18 clamps to7, and Quick/Classic/Kings mode switches preserve the preference. `code5-max-clamped-state` and `code5-group-return-repro-{0,1,2}.json` preserve these observations. Native standard scroll plus a two-pointer roster gesture reached the second long-name row.

Explicit old Classic V4 replay (`code5-old-classic-replay`) preserves the labelled original source `../android-16kb-probe/final-long-word-state`. Resume shows Bea/card2of5; held Sprachnachricht is readable at150%, release covers it, actual Next reaches Cora/card3of5. Before-resume and after-advance state pairs are captured for independent migration audit. This is fixture compatibility evidence, not another claimed real installed update. Exact played names/words scanned in currentPID11645:85lines11585bytes, no hits.

TalkBack original settings restored (services absent, accessibility0, touch exploration0); font restored1.0. Replayed the previously Android-played game1 parity checkpoint, then actual native Resume and Nora selection reconstruct the pending good-King elimination. `code5-final-update-baseline-state` and `code5-final-update-pending-nora.png` are the final in-place update baseline. No confirmation has been performed. CurrentPID11872 log:85lines11585bytes, exact five names/Ratte/Maus and diagnostics clean. Installed APK remains code5; intermediate code6 APK has never been installed and is superseded by the pending accessibility correction/final code7 package.
