# Rollen-Update: integrierte Android-Abnahme und Release-Artefakte

Canonical: https://github.com/giarrel/word-deduction/issues/23

## Parent

Spec #11: https://github.com/giarrel/word-deduction/issues/11

## What to build

Validate the integrated role-count and Kings starter behavior on the actual rendered app and Android build. Produce a versioned APK/AAB, exact source/package provenance and a story-by-story acceptance report. Resolve actual issues through the implement-spec correction and review loop, preserving failures. Production signing and Store publication remain out of scope.

## Acceptance criteria

- [x] Entire Session and rendered suites pass against the integrated source; changed rule/storage/UI behavior has meaningful coverage.
- [x] Independent Standards and Spec reviews from the delivered code11 baseline are complete, with any findings fixed and rechecked.
- [x] DE/EN setup, tiny/large group boundaries, 150% text, count adjustment and accessibility are visually inspected; Quick multi-enemy/White and Kings round flows are exercised.
- [x] Actual Android code11-to-new-version update retains saved group, preferences, frozen state and history without data clearing; new settings and pending match progress survive restart.
- [x] APK/AAB are built and inspected with the established Unity6000.3.25f1 toolchain; package/version/signing/hash and exact commit are documented honestly.
- [x] Installable artifacts, report and traceable 34-story acceptance matrix are preserved at durable paths, with native/emulator limits stated.

## Blocked by

#21
#22


## Outcome

Accepted at the specified automated/rendered/emulator scope. [Native report](../../validation/role-native/report.md), [34-story matrix](../../validation/role-native/acceptance-matrix.md), [independent reviews](../../validation/role-final-review/report.md), and [delivery hashes](../../validation/role-native/delivery-manifest.json) identify exact source, evidence and limitations. Production signing/Store publication were not performed.
