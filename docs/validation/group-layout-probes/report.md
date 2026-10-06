# Android group layout and script probes

Root inspection, 6 October 2026, after accepting the group foundation and before implementing Quick. Production source `44473f1`, integrated through `318af96`; installed development APK SHA256 `181bfebbebca3c35198e1da29ebceb0d5c52aa33e2668f499a4fe4f730bd1cf2`. API36 emulator `emulator-5580`, ARM64 native translation, NVIDIA host GPU. These findings belong to the full presentation acceptance in issue8; no gameplay was tested here.

## Small Android viewport

Temporarily changed this dedicated emulator from its physical1080×2400/density420 to720×1280/density320, equivalent to360×640dp. The seven-player group, mode selector and primary action fit. After tapping the name field, the input and Plus remained visible above Gboard. [Group](43-small-device.png), [keyboard](44-small-keyboard.png). No name was added. Both `wm size reset` and `wm density reset` restored the original device settings.

Touch sizing remains **Failed** against the specification: the390-unit reference panel scales48-unit controls to about44.3dp at360dp width. The44-unit language minimum is about40.6dp. The52-unit add control is approximately48dp here. Measure final actual hit bounds in Android pixels divided by display density; merely declaring48 in USS is insufficient. The scrollbar also looks substantially wider than its6-unit parent declaration; inspect child/theme minimum sizes during polish.

## Actual Unicode rendering

Preserved the complete current synthetic `session.json`, stopped the app, and temporarily installed the adjacent [valid V1 fixture](fixture-session.json) through debug `run-as`. The fixture changes only the seven display names and disambiguation flags, with a recomputed envelope checksum. This is **direct fixture injection**, not a native Unicode keyboard-entry test or an ordinary gameplay walkthrough.

The app accepted the snapshot and showed seven people. The [upper rows](01-script-names.png) and [lower rows](02-emoji-names.png) were actually rendered and inspected:

| Input name | Observed rendering |
| --- | --- |
| Zoë | Name and initial visible |
| 李雷 | Name and initial blank |
| محمد | Name and initial blank |
| नमस्ते | Name and initial blank |
| Мария | Name and initial visible |
| 👩🏽‍🚀 | Name and initial blank |
| Éléonore | Name and initial visible |

This fails accepted-name display and cannot be explained away by documented automatic OS fallback. It occurred on the working host GPU, distinct from the earlier SwiftShader failure that affected every glyph. Successful serialization, or a theoretical fallback API, does not prove rendered coverage. Proper Arabic/Indic shaping and joined emoji still require separate checks after glyph availability is fixed.

Logcat at21:38:24 for process5802 included `Deleting invalid font reference`, followed by `TextSettings.InitializeFontReferenceLookup`, `GetCachedFontAsset` and `TextUtilities.GetFontAsset`. The same warning also appeared in earlier Latin-only launches. It is an investigation lead, **not an established cause**.

After capturing both images, force-stopped the app, restored the original primary file and relaunched. Compared the restored envelope text equal to the preserved original: Alexandra, Bea·2, Chris, Bea·4, Dana, Felix, Emil; seven active people; DE/Classic; no pending removal. The previous-generation backup was not modified. No real personal data was involved.

## Base palette calculation

Using the actual `Group.uss` colors and WCAG sRGB relative luminance: ink/paper14.62:1; muted/paper5.82:1; white/action-purple6.22:1; dark-purple/lilac7.88:1; count-text/mint8.31:1; removal-text/white7.00:1; notice-text/background7.50:1. These base pairs exceed4.5:1. This is a source-color calculation, not a measurement of every rendered state; opacity, disabled controls, focus, overlays and animation remain outside this check.

Next: implement explicit working font fallback/shaping as needed and correct dp-equivalent targets in issue8, then rerun the same actual Android fixtures and native entry. Keep the original group and future running matches intact across APK updates.
