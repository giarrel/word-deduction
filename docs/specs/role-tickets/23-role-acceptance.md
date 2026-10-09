# Rollen-Update: integrierte Android-Abnahme und Release-Artefakte

Canonical: https://github.com/giarrel/word-deduction/issues/23

## Parent

Spec #11: https://github.com/giarrel/word-deduction/issues/11

## What to build

Validate the integrated role-count and Kings starter behavior on the actual rendered app and Android build. Produce a versioned APK/AAB, exact source/package provenance and a story-by-story acceptance report. Resolve actual issues through the implement-spec correction and review loop, preserving failures. Production signing and Store publication remain out of scope.

## Acceptance criteria

- [ ] Entire Session and rendered suites pass against the integrated source; changed rule/storage/UI behavior has meaningful coverage.
- [ ] Independent Standards and Spec reviews from the delivered code11 baseline are complete, with any findings fixed and rechecked.
- [ ] DE/EN setup, tiny/large group boundaries, 150% text, count adjustment and accessibility are visually inspected; Quick multi-enemy/White and Kings round flows are exercised.
- [ ] Actual Android code11-to-new-version update retains saved group, preferences, frozen state and history without data clearing; new settings and pending match progress survive restart.
- [ ] APK/AAB are built and inspected with the established Unity6000.3.25f1 toolchain; package/version/signing/hash and exact commit are documented honestly.
- [ ] Installable artifacts, report and traceable 34-story acceptance matrix are preserved at durable paths, with native/emulator limits stated.

## Blocked by

#21
#22

