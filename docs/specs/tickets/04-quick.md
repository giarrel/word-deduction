# Schnellmodus von geheimer Wortkarte bis Folgepartie spielen

Canonical: https://github.com/giarrel/word-deduction/issues/4

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Eine Gruppe spielt einen vollständigen Quick-Match mit geheimen Karten, Gespräch, mündlicher Abstimmung, Ergebnis und direkter Folgepartie.

## Acceptance criteria

- [ ] Quick-Regeln für 3–20 Personen erfüllen: genau ein Undercover, unbekannte Rollen beim Wortempfang, eine Hinweisrunde und Ergebnis bei bestätigtem Verdacht oder wiederholtem Gleichstand.
- [ ] Hochziehkarte mit Haltealternative; beim Loslassen Text sofort verbergen und Karte zurückführen. Weiter nur nach Aufdecken und bei geschlossener Karte; klare namentliche Übergabe.
- [ ] Wörter und Rollen vor erster Anzeige fest speichern. Wiederaufnahme würfelt nichts neu und startet verdeckt; Gruppenstatus bleibt nach Ergebnis erhalten.
- [ ] Zufällige Startperson, kurze Hilfe, eindeutiger Abstimmungsweg mit korrigierbarer Vorauswahl und Bestätigung, Stichwahl/Gleichstand, vollständiges Ergebnis und direkte Folgepartie.
- [ ] Durchgehend DE/EN; eine kleine eigene Starterwortdatei ist bis zum Inhaltsticket erlaubt. Öffentliche Session-Tests und gerenderter vollständiger Durchlauf mit Bildern.

## Blocked by

- [Gespeicherte Gruppe in einer ausführbaren Android-App verwalten](https://github.com/giarrel/word-deduction/issues/3)

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.

