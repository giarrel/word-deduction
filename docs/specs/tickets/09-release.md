# Geprüfte Android-Artefakte und vollständige Release-Unterlagen liefern

Canonical: https://github.com/giarrel/word-deduction/issues/9

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Ein reproduzierbarer Release-Kandidat mit APK/AAB, Messnachweisen und deutsch-englischen Storeunterlagen kann konkret zur Veröffentlichung beurteilt werden.

## Acceptance criteria

- [ ] Standards-/Spec-Review gegen den festen Ausgangscommit durchführen; erhebliche Befunde beheben und erneut prüfen.
- [ ] APK/AAB ohne Development-Flags mit ARM64/IL2CPP erzeugen; Manifest, SDK, Berechtigungen, 16-KB-ZIP-/ELF-Ausrichtung, Bundlekonfiguration, Signatur, Versionscode und Hash prüfen.
- [ ] Produktionssignierung von Debug trennen; keine Nischenreich-Schlüssel oder Geheimnisse im Repository. Fehlende Eigentümer-Voraussetzungen anhand konkret vorbereiteter Artefakte benennen.
- [ ] Eigenes Icon, Bilder der echten App, DE/EN-Storetexte, tatsächliche Datenschutz-/Data-Safety-Grundlage und Abnahmebericht mit Status je Kriterium vorbereiten.
- [ ] Build, Tests und Playtest-Bilder reproduzierbar dokumentieren; offene Anforderungen nicht als abgeschlossen markieren; Storeveröffentlichung benötigt ausdrücklichen Auftrag.

## Blocked by

- [Bedienung und Gestaltung durch echte Playtests verbessern](https://github.com/giarrel/word-deduction/issues/8)

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.

