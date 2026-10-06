# Schnellmodus von geheimer Wortkarte bis Folgepartie spielen

Canonical: https://github.com/giarrel/word-deduction/issues/4

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Eine Gruppe spielt einen vollständigen Quick-Match mit geheimen Karten, Gespräch, mündlicher Abstimmung, Ergebnis und direkter Folgepartie.

## Acceptance criteria

- [x] Quick-Regeln für 3–20 Personen erfüllen: genau ein Undercover, unbekannte Rollen beim Wortempfang, eine Hinweisrunde und Ergebnis bei bestätigtem Verdacht oder wiederholtem Gleichstand.
- [x] Hochziehkarte mit Haltealternative; beim Loslassen Text sofort verbergen und Karte zurückführen. Weiter nur nach Aufdecken und bei geschlossener Karte; klare namentliche Übergabe.
- [x] Wörter und Rollen vor erster Anzeige fest speichern. Wiederaufnahme würfelt nichts neu und startet verdeckt; Gruppenstatus bleibt nach Ergebnis erhalten.
- [x] Zufällige Startperson, kurze Hilfe, eindeutiger Abstimmungsweg mit korrigierbarer Vorauswahl und Bestätigung, Stichwahl/Gleichstand, vollständiges Ergebnis und direkte Folgepartie.
- [x] Durchgehend DE/EN; eine kleine eigene Starterwortdatei ist bis zum Inhaltsticket erlaubt. Öffentliche Session-Tests und gerenderter vollständiger Durchlauf mit Bildern.

## Blocked by

- [Gespeicherte Gruppe in einer ausführbaren Android-App verwalten](https://github.com/giarrel/word-deduction/issues/3)

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.


## Ergebnis

Umgesetzt in 74e7825 und separat in integration/android-v1 integriert (496d446, Merger-Prüfung013087f). 21/21 Session, 10/10 PlayMode und drei tatsächliche Android-Partien bestanden. [Androidbericht](../../validation/android-quick-runtime/report.md), [Implementierungsnachweise](../../validation/quick-mode.md). Vollständige Classic-, Recovery-, Schrift-/Touch- und Release-Prüfungen folgen in den zugehörigen Tickets.
