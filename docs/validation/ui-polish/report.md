# UI polish / issue 8

## Scope and result

This change keeps the accepted Session, saved group/match and private-card input contracts, with a softer rounded identity and clearer bilingual menus. Native validation uses an Android 16 emulator with an ARM64 build through translation. It is not a physical-device, audible TalkBack speech or release-performance claim. The coordinator accepted source 7404d17826e7dd2e447e6dacbe1ea2d5770ce358 after the final DE/EN native long-owner and capacity checks. Both original V4 generations were then restored byte-for-byte and all temporary accessibility/display settings were reset. Issue 8 is ready for the separate integration merge; release gates remain with issue 9.

## Product changes

- Android density defines UI units, so a 48-unit control is 48dp. Language buttons now meet that minimum. Shared thin scrollbars leave more room for names and choices.
- Rounded mint/lilac cards and an original two-card launcher mark give the app a consistent identity. Release clears secret text synchronously before decorative settling. Android's animator setting controls the tilt/return motion at startup and resume.
- Public text follows the OS preference from 100% to 150%. Larger OS scales deliberately use that upper bound. Enlarged German edit actions occupy two rows; the redundant group heading hides while editing with the keyboard. Public prose, help, votes and results scroll when needed.
- Compact ready-group spacing removes redundant ready/saved copy. Capacity now explains why Add is unavailable. Blocked recovery keeps the message and applicable recovery action; previous-generation recovery still shows the recovered group normally.
- The lifted card hides its redundant pass-phone hint while retaining the owner. Decorative geometry no longer collides with an enlarged caption, and a concealed secret label consumes no layout space. At large text, the 24-wide-character owner plus generated discriminator remains fully wrapped above the card; the decorative lift stays inside its available slot. Reveal thresholds and privacy are unchanged.
- An explicit accessibility hierarchy exposes public labels, text entry and actual menu actions. Private word/assignment and decorative glyphs never enter Handoff semantics. Clipped frames and visibility follow layout without replacing native node identity on keyboard movement or scrolling.
- ATG and ICU shape complete strings. Installed script fonts load lazily for both saved names and unsaved drafts, with a cached on-demand route for other installed scripts. This is device-dependent installed-font coverage, not every Unicode glyph on every OEM.
- A pinned Google Noto Color Emoji CBDT/CBLC font and OFL notice ship with the app because the actual Android 16 COLRv1 font loaded but rendered blank. The primary font remains Inter. No Burst package or OS font installation was added.
- Unicode name handling comes from the reviewed `f65ec717` contribution, cherry-picked as `f1e0fd8`: pinned Unicode 17 extended graphemes, new invisible-only rejection and compatibility with existing V4 names. Badges use its public first-element helper. See [Unicode evidence](../unicode-names/report.md).
- The generated launcher icon source is `tools/generate-app-icon.ps1`. Nonfullscreen startup exposes the Android navigation bar and avoids the immersive first-use coaching overlay; the status bar remains hidden on the tested player.

## Test evidence

| Evidence | Result |
| --- | --- |
| `touch-red.json` | Actual rendered German language target was 44 units; fixed to 48. |
| `grapheme-red.json` | Unity rejected 24 joined astronaut graphemes before the deterministic Unicode implementation. |
| `large-card-red.json` | Enlarged caption collided with the decorative mark. |
| `large-edit-red.json` | German Save extended to x449 beyond its x328 viewport. |
| `final-full-suite-green.json` | 25/25 PlayMode tests passed in 93s after card/privacy/recovery changes, including six selected long DE/EN words obtained through actual Session deals and rendered on the small card. |
| `final-polish-green.json` | After the subsequent responsive edit correction, all 6 polish tests passed: touch bounds, Unity 24/25 graphemes/invisible names, private-safe semantics, stable/clipped frames, enlarged card and actual persisted German rename. |
| `final-group-green.json` | All 5 existing group tests passed after that correction, including last-row keyboard resizing and reopen persistence. |
| `large-owner-red.json` | A 24-wide-character owner with generated discriminator at 150% text pushed the progress label into the toolbar. |
| `large-owner-green.json` | All 7 polish tests passed after the final bounded owner/lift and capacity correction, including a revealed card that keeps the whole owner visible. |
| `final-privacy-green.json` / `final-recovery-green.json` | All 5 Quick and all 5 Recovery rendered tests passed again after the final card changes. |
| [Bilingual key audit](bilingual-key-audit.json) | All 126 authored UI keys have nonempty DE/EN translations. This is a copy inventory, not a substitute for interaction testing. |
| [Visual review](visual-review.md) | 130 actual Game view captures opened and inspected across small/tall DE/EN, every match phase, 20 active/40 saved, recovery and public-Session-generated discriminator collisions. Source stages are identified; earlier images are not presented as the final revision. |

The new tests use rendered UI input and the public Session boundary. Layout stress tests reproduce enlarged font metrics and a keyboard-sized viewport; native OS preference delivery and actual Gboard are separate Android checks.

## Native Android checks

The coordinator controls all ADB operations and restores both original committed V4 generations between temporary fixtures. Updates so far preserve their hashes byte-for-byte. Display stress uses 1080×1920 at density 480 (360×640dp), with native cutout/insets and a visible navigation bar. Console PNGs retain 1080×2400 letterboxing and are QA evidence, not store screenshots.

- CJK, joined Arabic, Devanagari and mixed Latin/RTL names render on the actual Android player. [Joined skin-tone astronaut](native/polish1-small-emoji-astronaut.png) is visible in both name and badge; simple emoji and a keycap also render. These samples do not imply exhaustive font coverage.
- Real TalkBack with touch exploration can activate public actions and focus the native keyboard field. Six native scroll actions and a [real two-finger touchscreen gesture](native/polish1-after-two-finger.png) preserve field focus and update clipped bounds. The raw ScrollView container's visibility flag alone is not treated as a defect because its native actions and actual gesture work.
- Newly created Handoff controls have correct nonzero native frames after Resume. Its public tree excludes private text. No blind private-word audio mode is promised.
- Actual Gboard entry, Backspace, Done and explicit Add work. Done/Back preserve drafts without committing another player. A test-only foreground clipboard seed produced a Gboard clipboard chip; tapping that chip pasted the exact sample and Done retained it without saving. Android Ctrl+V itself remains unverified.
- Actual touchscreen sequences cover short tap, early drag, slow threshold crossing, release outside, cancellation and multiple contacts. Secrets conceal correctly; reentry waits until all fingers lift. Home while held returns to a safe Pause and covered Resume.
- Animator scale 0, refreshed on resume, shows a [horizontal held card](native/polish1-reduced-motion-confirmed-held.png) and a [covered release](native/polish1-reduced-motion-confirmed-released.png). Original global settings are restored afterward.
- Native large-text review found the [card decoration overlap](native/polish1-large-live-handoff.png) and [clipped German edit actions](native/polish1-capacity-large-edit.png). Both have red/green rendered regressions. Polish2 passed actual German Save with Gboard open: the renamed player retained its ID, all 40 saved/20 active people and history. Its actual held/released card has clear caption spacing and conceals on release. See [native edit](native/polish2-capacity-edit-keyboard-settled.png), [saved result](native/polish2-capacity-save-current.png) and [held card](native/polish2-reveal-confirmed-held.png).
- Discriminator collisions are generated through public Session commands, including literal suffix names, and exported for native replay. Actual native rows show Alex · 1, Alex · 2, Alex · 1 · 3, Alex · 2 · 4 and Alex · 1 · 1 distinctly.


The polish3 long-owner fixtures were created by public Session commands and rendered Start, then replayed with checksum validation. On the actual Android player at 360×640dp and OS font scale 2.0 (app 1.5×), all 24 wide M characters and the generated discriminator remain visible above the card. The coordinator personally opened the covered, held and released frames in both languages. The card reveals only the assigned word and conceals on release, while Hold and Next remain separate:

| Final long owner | German | English |
| --- | --- | --- |
| Covered | [View](native/polish3-wide-de-covered.png) | [View](native/polish3-wide-en-covered.png) |
| Actual native hold | [View](native/polish3-wide-de-held.png) | [View](native/polish3-wide-en-held.png) |
| Released | [View](native/polish3-wide-de-released.png) | [View](native/polish3-wide-en-released.png) |

The final [German capacity view](native/polish3-capacity-de.png) and [English capacity view](native/polish3-capacity-en.png) show 40 saved names, a clear full-group explanation and reachable main actions at the same enlarged setting. The coordinator opened both and accepted the final candidate. Afterward, `CheckOriginals` passed for both restored committed generations, the app was stopped, font scale returned to 1.0, display overrides reset, TalkBack disabled/services absent and the original absent animator setting restored.

The [coordinator's native record](native/coordinator-report.md) preserves the chronological failures, excluded early/stale helper attempts and subsequent closures. Main results above refer to the accepted later stages. No private snapshot files or test-only helper package are included in the application.

## APK provenance

| APK | Source | SHA256 | Finding |
| --- | --- | --- | --- |
| Font probe1 | d83704d | fe652e671af0a71ed8ab4db9638b88d37221e969b15c46fed7401331f0b7f433 | Android font discovery includes ` - Regular`; no fallback initially loaded. |
| Font probe2 | 7152e7b | f18aa04633cae54d23418922b70021a2da9d609f01084a3abb37f98d115515a4 | Shaped scripts visible; 28 eager faces, emoji blank. First visible frame about 23s on translated emulator. |
| Font probe3 | 740361a | c249d1aadd23f1a27733b5b01ef2b30cb8bb202218cf35ed36d43ef51b86bd03 | Lazy fonts and native semantics; stale frames/focus reset discovered. |
| Polish1 | b4551b2 | ea4176a6e0556de471c634071023e76268c616f59e3ef7552c6d8d9a8d9f50f7 | Stable native geometry/focus, emoji and public actions pass; large-text visual issues discovered. |
| Polish2 | febf5438f93b3db335849c952d90979077ab7319 | eb1fc180bd87566e1126bb34516f02b6c3bc5d7bac82419467d5bca2e0709cda | 53,761,160 bytes; enlarged native German keyboard Save and short-owner reveal passed. |
| Polish3 | 7404d17826e7dd2e447e6dacbe1ea2d5770ce358 | d4f75a4f60601c959fad9ae6ee9a1ece2b7c100cb74d6fb97c7f4f4aee9ec334 | 53,759,826 bytes; final bounded owner/capacity correction. |

All builds reported Succeeded. The Pipeline request times out after five seconds while Unity continues building; this is recorded as one build error rather than hidden. Probe2 also recorded a forced-stopped asset worker. Polish2 completed in 42.1s and polish3 in 39.848s with no compile/Gradle failure; their two warnings are the intentionally absent runtime Pipeline configuration and the existing obsolete PreventDefault call in privacy input handling. The twelve icon-compression warnings from polish1 are fixed. Missing Burst-library probes were nonfatal in observed native runs; ATG uses the engine's native text path.

Current integration `cd19d5f` is merged into the candidate. Artifacts use the current package/key for update continuity. They are development validation builds; final release-only gates belong to issue 9.