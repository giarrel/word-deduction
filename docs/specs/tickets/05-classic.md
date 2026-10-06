# Klassische Partien einschließlich Mr. White vollständig spielen

Canonical: https://github.com/giarrel/word-deduction/issues/5

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Dieselbe gespeicherte Gruppe wechselt in Classic und spielt wiederholte Hinweisrunden mit Eliminierungen, optionalem Mr. White und sämtlichen Siegwegen.

## Acceptance criteria

- [ ] Automatische Rollenformel und optionale White-Präferenz gemäß Spec für 4–20 Personen umsetzen; Quick bleibt unverändert.
- [ ] Runden mit Überlebenden, zufällige Startperson einschließlich White, mündliche Abstimmung, Rollenanzeige ohne Geheimwort; erneuter Gleichstand führt ohne Eliminierung weiter.
- [ ] White rät laut; Gruppe bewertet die Bedeutung; Zielwort bleibt bis Ergebnis verborgen. Ausstehender Rateversuch hat Vorrang vor anderen Endbedingungen und ist wiederaufnehmbar.
- [ ] Alle Siegpfade, Grenzfälle und erneute Auswahl ausgeschiedener Personen über das öffentliche Interface testen. DE/EN-Spieloberfläche; Folgepartie schließt alle aktiven Gruppenmitglieder wieder ein.

## Blocked by

- [Schnellmodus von geheimer Wortkarte bis Folgepartie spielen](https://github.com/giarrel/word-deduction/issues/4)

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.

