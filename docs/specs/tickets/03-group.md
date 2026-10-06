# Gespeicherte Gruppe in einer ausführbaren Android-App verwalten

Canonical: https://github.com/giarrel/word-deduction/issues/3

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Eine echte startbare Unity-App zeigt sofort die Gruppe, erlaubt Namenseingabe und Bearbeitung und stellt dieselbe Gruppe nach Neustart wieder her. Dies ist die erste vertikale Strecke samt wiederverwendbarer Build- und Testbasis.

## Acceptance criteria

- [ ] Unity 6000.3.25f1 mit vorhandener Werkzeugkette verwenden; UI Toolkit und kein Cloudlink. Eigenen reproduzierbaren Android-Buildeinstieg mit ARM64/IL2CPP, minSdk 26 und Target API 36 einrichten; vorhandene CLI/Pipeline gezielt nutzen.
- [ ] Session-Interface und echte dauerhafte Ablage: stabile IDs, Hinzufügen, Umbenennen, Pausieren/Aktivieren, Entfernen und Undo; gleiche Namen sichtbar unterscheiden und Unicode-/Kapazitätsgrenzen der Spec einhalten.
- [ ] Eine tatsächlich gerenderte Gruppenansicht mit runder Gestaltung, Scrollbereich, Namensfeld, DE/EN und Quick/Classic-Vorauswahl bereitstellen. Der noch folgende Spielstart wird als nächstes Ticket dokumentiert.
- [ ] Jede bestätigte Änderung vor der Erfolgsmeldung sichern; versioniertes Format, Backup und Fehlerbehandlung vorbereiten; Schließen und erneutes Öffnen über das öffentliche Session-Interface prüfen.
- [ ] TDD-Nachweise für Rot und Grün, erfolgreiche Unity-Kompilation, echter Screenshot und ausführbarer Build-Einstieg liegen vor. Dokumentation nennt genaue Befehle und Grenzen.

## Blocked by

None (can start immediately).

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.

