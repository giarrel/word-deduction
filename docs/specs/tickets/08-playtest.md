# Bedienung und Gestaltung durch echte Playtests verbessern

Canonical: https://github.com/giarrel/word-deduction/issues/8

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Die gesamte App wird mehrfach über tatsächliche Eingaben gespielt und anhand gerenderter Bilder kritisch überarbeitet, bis die festgelegten Bedien- und Sichtkriterien erfüllt sind.

## Acceptance criteria

- [ ] Reproduzierbares Playtest-Harness verwendet reale UI-Eingaben/Presenter. Direkt gesetzte Bildschirmfixtures gelten separat und nicht als durchgespielter Ablauf.
- [ ] Alle Screens in DE/EN, kleines/hohes Display, 20 aktive Personen, lange Namen, Safe Area, Tastatur, Kartenbewegung und Folgepartie prüfen. Korrekturen mit Vorher/Nachher oder Regression dokumentieren.
- [ ] Konsistente runde Gestaltung, lesbare Kontraste, großzügige Touchziele, ruhige Übergänge und reduzierte Bewegung. Störende Extras und unnötige Betätigungen entfernen.
- [ ] APK mit verfügbarem Zugang installieren; Lifecycle, Zurück, Offline-Nutzung und Update prüfen. Emulator und physisches Gerät unterscheiden; fehlende Hardware konkret ausweisen.
- [ ] Keine offenen Befunde zu Datenverlust, Geheimnislecks, fehlerhaften Regeln oder Sackgassen. Grenzen für echte Gruppendynamik und physische Handhabung ehrlich festhalten.

## Blocked by

- [Klassische Partien einschließlich Mr. White vollständig spielen](https://github.com/giarrel/word-deduction/issues/5)
- [Großen deutsch-englischen Wortbestand ohne frühe Wiederholungen spielen](https://github.com/giarrel/word-deduction/issues/6)
- [Unterbrechungen, Speicherfehler und vertrauliche Übergaben sicher behandeln](https://github.com/giarrel/word-deduction/issues/7)

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.

