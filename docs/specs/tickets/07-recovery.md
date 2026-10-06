# Unterbrechungen, Speicherfehler und vertrauliche Übergaben sicher behandeln

Canonical: https://github.com/giarrel/word-deduction/issues/7

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Während des Spiels kann das Handy gesperrt, die App gewechselt oder beendet werden. Bestätigtes Spiel bleibt erhalten und Geheimnisse werden zuverlässig verdeckt; kaputte Speicherstände führen zu verständlicher Recovery.

## Acceptance criteria

- [ ] Tests mit realen Dateien für jede Phase, korrupte Haupt-/Sicherungsdateien, unterbrochene Schreibvorgänge, Schreibfehler und unbekannte neuere Schemata. Kein stiller Reset oder fälschlich angezeigter Erfolg.
- [ ] Zurück/Fortsetzen/Abbrechen mit geeigneter Bestätigung; laufende Partie erhalten. Fokusverlust durch die Tastatur darf die Namenseingabe nicht unerwartet verwerfen.
- [ ] Abgebrochene Berührung, Capture-Verlust, Fokus/Pause und Besitzerwechsel verbergen den Text sofort. Nur ein Pointer; Doppeltipps können keine Person überspringen.
- [ ] Android-Vorschau schützen, Zurück-Verhalten und Backup bewusst behandeln und Manifest prüfen. Keine Geheimnisse in Release-Logs oder verdeckten Accessibility-Labels.
- [ ] Im Editor und mit verfügbarem Android-Zugang prüfen; genaue Einschränkungen dokumentieren. Keine behaupteten Geräteprüfungen ohne Gerät.

## Blocked by

- [Schnellmodus von geheimer Wortkarte bis Folgepartie spielen](https://github.com/giarrel/word-deduction/issues/4)

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.

