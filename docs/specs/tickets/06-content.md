# Großen deutsch-englischen Wortbestand ohne frühe Wiederholungen spielen

Canonical: https://github.com/giarrel/word-deduction/issues/6

## Parent

Part of [Android v1 specification](https://github.com/giarrel/word-deduction/issues/2).

## What to build

Jede Folgepartie schöpft aus einer großen redaktionell geprüften Sammlung; beide Sprachen funktionieren vollständig, ohne Paketwahl oder Downloads.

## Acceptance criteria

- [x] Mindestens 500 eigenständig erstellte DE/EN-Paare, 700 normalisierte unterschiedliche Wörter je Sprache und zwölf vertraute Themen liefern. Keine Wettbewerberkopien, reinen Synonyme oder Fachwörter als Füllmaterial.
- [x] Katalogvalidierung und dokumentierte redaktionelle Durchsicht: identische und umgekehrte Paare, fehlende Übersetzungen, IDs, doppelte Konzepte und problematische Begriffe prüfen.
- [x] Paar-IDs erst nach vollständigem Ziehzyklus wiederholen; Verlauf über Neustart und Sprachwechsel behalten; unmittelbare Wiederholung beim Zykluswechsel vermeiden. Wörter der letzten zehn Partien bevorzugt meiden, ohne die Auswahl zu blockieren.
- [x] Inhalt im App-Build durch die Session verwenden, lange Wörter auf echter Karte prüfen und vollständige DE/EN-Texte/Hilfen validieren. Tatsächlich gemessene Bestandszahlen berichten.

## Blocked by

- [Schnellmodus von geheimer Wortkarte bis Folgepartie spielen](https://github.com/giarrel/word-deduction/issues/4)

## Execution

Use `implement-spec` and `tdd` at the Session and rendered-app seams defined in the spec. Separate ticket worktree based on the integration branch. Read GLOSSARY and ADRs. Preserve red/green and runtime evidence; don't claim checks that didn't run. Source conventions and exact paths belong in repository docs. Dependencies are explicit here because this connector does not expose native dependency mutations.


## Outcome

Accepted at integration d5da6f0: 520 original bilingual pairs,20 themes,1038 distinct terms/language; editorial and mechanical validation,1040 persistently reopened draws, six long terms on the real production card, combined19/19 PlayMode and native V3→V4/update/restart/DE→EN history acceptance. [Combined Android evidence](../../validation/android-recovery-combined/report.md). Recovery navigation investigation remains separately open in7; full visual/accessibility polish belongs to8.

