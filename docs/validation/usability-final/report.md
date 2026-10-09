# Usability final correction and release validation

9 October 2026. Scope: group usability #17/#19 and fluid interactions #18/#20. The correction checkout starts at integration commit `2146f2d817319c3d19757caad2202e359cce0ace`. Free base-mode role counts and reordering unread live handoffs remain outside this package while the required user decisions are pending.

## Standards review

The original [Standards report](standards-review.md) is preserved separately: **zero documented-standard violations**, one optional P3 duplicated-code finding in the two new rendered suites' touch factories. This is a maintainability judgment, not a reproduced product failure.

The existing test-only `ScreenTestActions` now supplies one touch dispatcher with a target, phase, position and optional finger ID. Both suites use it. Began, Moved, Ended and Canceled dispatch their corresponding public pooled pointer event; unsupported phases fail explicitly instead of one suite treating them as release and the other ignoring them. Existing test calls all use the four supported phases. Fixtures, runtime interfaces, assertions and production gestures are unchanged. No new helper-specific tests were added; the public rendered scenarios are the validation seam.

## Spec review

The original [Spec report](spec-review.md) is preserved separately: **zero implementation findings** within #17/#18. It explicitly leaves final Android update/persistence, keyboard/accessibility, paired frame measurements, continuous motion inspection and reduced-motion/privacy acceptance open. Editor tests are not substituted for those native gates.

## Correction validation

- [GroupUsabilityScreenTests](evidence/group-test-result.json): **6/6 passed**, zero failures/skips, 2.9 seconds.
- [MotionScreenTests](evidence/motion-test-result.json): **4/4 passed**, zero failures/skips, 0.9 seconds. This includes captured reorder release/cancel followed by another finger, and preserving a different held contact as a reveal blocker.
- The coordinator's [full Session result](evidence/integration-session-full.txt) is **76/76 passed** at `3835a71035b0fe4c471e5893a54f2438b5c34505`. The Session and Session-test sources are unchanged in this correction.
- The integrated [full rendered result](../fluid-interactions/evidence/integrated-rendered-final-result.json) is **57/57 passed** at production source `33d0d99`, as described in the [motion report](../fluid-interactions/report.md). The runtime UI, Session and Android plugin sources are unchanged here; only the two affected suites were rerun after the helper consolidation.

Prior intermittent corpus runs that encountered denied `File.Replace` operations remain in the [group evidence](../group-usability/evidence/retained-corpus-failure/inspection.json) and earlier full-run reports. Their failures have not been removed or represented as a proven filesystem fix. The successful later full suite and these targeted checks are separate observed results.

[Console status](evidence/console-status.json) confirms no compilation failure. The two retained [error entries](evidence/console-errors.json) are the already documented `UnityEditor.Search.SearchDatabase` startup exceptions, with no app stack frames. They were preserved, not cleared. Build warnings and actual package results are recorded separately below when available.

The glossary now describes the current group and explicitly preserved earlier paused names, matching the compatibility-only saved-people section. No legacy data, role policy, word catalog, persistence schema or production behavior is changed in this correction.

## Release configuration and evidence boundary

Version **1.2.0 / Android code9** is consistent in the build entry point, serialized PlayerSettings, inspector defaults and reproduction guide. PlayerSettings was changed through the existing Unity `AppBuild.Configure()` entry point; the enabled scene remains `Assets/WordDeduction/Scenes/App.unity`. Existing Unity 6000.3.25f1, Android ARM64/IL2CPP, API26–36, release minification and local debug signing identity are retained.

Both independent reviews were received before any release build. The granted Editor lease belongs only to this correction checkout. Raw import/test/build logs and generated font-cache copies are preserved under ignored `artifacts/usability-final`. Test-generated font data was saved and copied before clearing Inter/Emoji through Unity; no semantic font asset change is retained. No Console clearing, ADB operation, other Editor, installation, global setting, production signing key, issue closure or Store publication is part of this correction.

Clean pinned APK/AAB builds and their actual packaged inspections follow. Native acceptance and motion comparison belong to the coordinator after this Editor and every owned build process have stopped; performance improvement is not claimed from source or Editor tests.
