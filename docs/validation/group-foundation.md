# Group foundation — issue 3 validation

Date: 2026-10-06. Base: `ab25c325e02040d30755ae448c07789357730f38` on `ticket/3-group-foundation`. Integration target: `integration/android-v1`.

## Delivered slice

Unity 6000.3.25f1 application using UI Toolkit, a real file-backed Session module, German/English group UI, Quick/Classic selection and an Android ARM64/IL2CPP build entry. No Unity installation, Nischenreich file or global CLI configuration was changed. The template was `com.unity.template.2d` 11.0.0, created with no cloud link and no additional Git repository. Editor package additions used `Client.AddAndRemove`: Pipeline 0.8.0-exp.1 and Newtonsoft 3.2.1; unnecessary template packages were removed. Input System 1.20.0 and Test Framework 1.6.0 remain pinned.

Gameplay start is deliberately disabled in this slice. Issue 4 supplies the complete Quick path, later tickets Classic and release work. This document makes no claim that this foundation is a finished game.

## Behavioral evidence

The pre-agreed public Session seam is exercised with genuine temporary files, reopening after confirmed actions. The runner compiles the exact production C# sources; its final run references Unity's actual Newtonsoft 13.0.2 DLL from the resolved package. Early red/green iterations used compatible Newtonsoft 13.0.1 before the Unity package resolved; all cases were rerun against the production DLL afterward. No own-module mocks are used.

Command: `dotnet run --project tests/Session.Tests` (installed .NET 9 SDK). Final result: **13/13 passed**. Selected red/green evidence is preserved in [evidence/group-foundation](evidence/group-foundation/); full working logs are in ignored `artifacts/validation/`.

| Slice | Observed red | Green |
|---|---|---|
| 01 Add/reopen | `add succeeds` assertion failed | Same trimmed Unicode name and stable ID after reopen |
| 02 Edit lifecycle | New public actions initially absent (compile red) | Rename, pause/resume, remove and persisted undo retain identity/order |
| 03 Duplicates | `two Alex players can be distinguished` failed | Durable disambiguators |
| 04 Name contract | `blank rejected` failed | Blank/control/overlong rejected; 24 combining characters normalize and persist |
| 05 Capacity | `only 20 people active` failed | 40 saved, overflow paused, activation and undo respect limits |
| 06 Preferences | New public settings/readiness absent (compile red) | DE/EN and Quick/Classic survive restart; minimum group explained |
| 07 Recovery | Corrupt JSON threw on open | Valid previous generation recovered, visible notice and usable next save |
| 08 Failed write | Pending-file obstruction threw | Failure result; memory/disk unchanged; retry succeeds |
| 09 Both damaged | Backup parse threw | Mutation blocked; explicit fresh start archives damaged files |
| 10 Newer schema | `newer version gets explicit block` failed | No rollback, reset or overwrite |
| 11 Label collision | `all three display labels distinct` failed | A literal name matching a generated label is disambiguated too |
| 13 Invalid payload | `Players:null` threw in View despite valid checksum | Semantic validation recovers a valid prior group |
| 14 Stale callback | Double-remove threw index error | Missing IDs return `PlayerNotFound`, preserving undo |

Initial sandbox NuGet restore failure was an infrastructure failure and is not counted as a TDD red. The final test project no longer needs a NuGet JSON download; Unity's package must have been imported first.

## Rendered UI

Unity PlayMode tests ran through the live Editor/Pipeline, using rendered UI Toolkit controls and `NavigationSubmitEvent` rather than directly invoking the Session actions. The first screen test was red because the name input did not exist, then green. The extended scenario enters names, pauses, renames, removes/undoes, switches DE/Classic, and reopens the real store. Two further cases check retaining a failed rename draft and revealing the last row after layout. The final suite is **4/4 passed**, recorded in [ui-final-green.json](evidence/group-foundation/ui-final-green.json). These are synthetic UI events; native Android evidence is recorded separately below.

Example command (substitute the absolute `game` path):

```powershell
unity command run_tests --mode playmode --filter WordDeduction.Tests --async_tests true --project-path <game> --caller plugin --skill ui-uitk --format json
unity command test_status --project-path <game> --caller plugin --skill ui-uitk --format json
```

[English initial group screen](images/group-empty-en.png) was captured from the real Game View and visually inspected. Warm ivory, dark Inter text, lavender actions and large rounded input/button shapes are legible at 390×844. An initial capture stretched a landscape Game View into portrait; that image was rejected, the actual Game View was set to 390×844, and the corrected capture replaced it. Merely requesting portrait output dimensions does not change the underlying Game View.

The [final German group view](images/group-final-de.png) was likewise captured and inspected at a real 390×844 Game View. The six synthetic names include duplicates and a longer paused name; disambiguators are legible, the long name wraps inside its card and the action reads Mitspielen. This injected temporary Session fixture is visual evidence, not an additional touch walkthrough.

Font source: the Unity installation's Inter-Regular.ttf, redistributed with its bundled SIL Open Font License in `Assets/WordDeduction/UI/Fonts/Inter-LICENSE.txt`.

## Build commands

With the Editor closed:

```powershell
./tools/build-android.ps1 -Build DevelopmentApk
```

For an existing connected Editor, stop Play mode, switch its active target to Android once, then:

```powershell
unity command eval 'WordDeduction.Editor.AppBuild.DevelopmentApk(); return "Android APK built";' --timeout 1800000 --detach --project-path <game> --caller plugin --skill unity-cli --format json
unity job status <returned-job-id> --project-path <game> --format json
```

Target switching normally reloads assemblies and can discard the transient job ID. Verify `EditorUserBuildSettings.activeBuildTarget` afterward. Scene/assets are created through `AppBuild.CreateScene()` using actual Editor APIs, never hand-written Unity YAML. `AppBuild.Configure()` pins minSdk26, target36, ARM64 and IL2CPP. Builds write to ignored `artifacts/android/` and produce a JSON summary. `ReleaseApk`/`ReleaseBundle` are local non-development packaging entries, not a signed store release. A custom keystore is rejected until signing is explicitly wired in the release ticket.

## Limits

Android runtime testing used a real running API36 emulator, host GPU and ARM64 native translation. Physical ARM64 devices, API26–29 keyboard fallback, 16KB compatibility, task preview privacy, production signing, store metadata, screen-reader behavior and real group fun are not proven by these checks. Future tickets must provide their own evidence. This slice has no networking/account/ads UI and no online game mode.

## Isolated font experiment and emulator comparison

The first Android screenshots showed glyph rectangles despite working Editor text. The dynamic atlas was initially a hypothesis (empty atlas serialized for runtime generation). An isolated static-atlas build, hash `49834bd14b8b760fac4a1cefa2df8eb073d9041d94f9007b8c60f0e98d5a1582`, pre-baked 323 glyphs while keeping the same font, shader and OpenGLES3 renderer; it still failed under SwiftShader. The parent then ran that identical APK under host GPU (NVIDIA RTX3080Ti/OpenGLES3.1): text rendered correctly. The original dynamic-font APK also rendered correctly under host GPU, with the same saved group retained. This contradicts changing the app's font to fix the observed environment. The static font experiment was removed; the original dynamic font remains. The emulator renderer is a demonstrated influence; this is not a physical-device compatibility claim.

[Original dynamic font on host GPU](images/android-host-gpu-original-font.png). The preserved [first Android report](android-group-first-pass.md) records the failed starting point; its screenshots are also retained as [glyph baseline](images/android-glyphs-before.png) and [keyboard baseline](images/android-keyboard-before.png).

The single build warning is expected: `Pipeline: No RuntimePipelineConfig asset found ... Pipeline will be disabled in Player builds.` Development control is intentionally Editor-only; no player bridge was enabled.

## First polish pass

A 360×640 view showed that retaining the decorative heading consumed too much room for names. The heading now appears only for an empty group, and compact/typing layouts prioritize the list and entry. A subtle scroll indicator replaces the default arrow scroller. Singular German labels now read `1 spielt mit` and `eine Person`.

The Android keyboard baseline led to a platform-specific inset adapter: API30+ reads `WindowInsets.Type.ime`; API26–29 falls back to the visible window frame. The typing view removes the decorative heading and mode/start controls while reserving the actual keyboard height. The first correction attempted to treat native `TouchScreenKeyboard.Status.Done` as confirmation. Real Android testing disproved this: Back also reported Done, unexpectedly saving an unconfirmed name (or showing InvalidName after Plus cleared the field). That automatic confirmation was removed. The visible Plus adds and retains the keyboard for rapid entry; IME Done/Back only close it. Actual hardware Return events remain supported. Save/Cancel/Remove close the rename keyboard when ending the edit.

A separate UI regression was reproduced before correction: on an obstructed file write, renaming Alex to Alexandra reset the typed field to Alex. The UI now retains its editing draft for retry while Session and disk retain the old confirmed name. [15-rename-draft-red.json](evidence/group-foundation/15-rename-draft-red.json) records red; the final passing test retries the same draft after removing the file obstruction and verifies persistence.

The Android list exposed another timing issue: the fifth person was saved but below the list's visible edge. The new row now waits for its GeometryChangedEvent before scheduling ScrollTo. The added Editor test passed both before and after the change, so it is regression coverage rather than a claimed red reproduction; the actual Android screenshot is the failing evidence.

The keyboard-iteration APK grew from 37,500,646 to 53,369,291 bytes. ZIP inspection found the same 407 entries and only about 17KB more compressed payload; the increase was unused ZIP gaps from incremental packaging, not a bundled font experiment or additional package. The reproducible builder now requests CleanBuildCache and records actual file size separately from Unity's aggregate build-report size.

Final clean development build: **Succeeded**, **0 errors**, **1 expected warning**, 249 seconds; actual APK **37,517,238 bytes**. The clean package eliminated the earlier 16MB overhead. SHA-256: `74eca5d6272ac62cf82cf7744fe936b6d3c039e28e5dcb3cbca7bcbdc4e4235c`. Package: `com.giarrel.worddeduction`, version 0.1.0 / code 1. This is an installable development foundation APK, not the finished game's store artifact.
