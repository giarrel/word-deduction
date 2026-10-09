# Usability-Abnahmematrix — 32 Stories, native Code10-Abnahme offen

Vorbereitung vom 9. Oktober 2026 für [Spec #17: Gruppenbedienung][S17] (20 Stories) und [Spec #18: flüssige Interaktionen][S18] (12 Stories). Die Kurzfassungen unten ersetzen die Spezifikationen nicht. Kandidat: **1.2.0 / Code10**, Quelle `bd7b618c957051d3ee61321a6ed3d275a5302a93`. Dies ist eine Zuordnung vorhandener Evidenz, keine abgeschlossene Abnahme. Für diese Vorbereitung wurden keine Tests, Editor-, ADB-/Geräte- oder Trackeraktionen ausgeführt.

Vorhandener Stand:

- **Session 76/76** bei Integration `3835a71035b0fe4c471e5893a54f2438b5c34505` [S]. Die gespeicherte [Code10-Quellvergleichsdatei][EQ10] meldet keine Änderungen an Session und Session-Tests seit diesem Stand.
- **Vollständige gerenderte Suite 57/57**, ohne Fehler/Skips/Inconclusive, bei Produktionsstand `33d0d99` [R]. Nach der Testhelfer-Korrektur wurden **Gruppe 6/6 + Motion 4/4** erneut geprüft [G9][M9]; nach der späteren Code10-Darstellungskorrektur nochmals **6/6 + 4/4** [G10][M10]. **57/57 ist kein neuer vollständiger Lauf der geänderten Code10-Darstellung.**
- Die unabhängigen [Standards-][ST] und [Spec-Reviews][SP] sowie ihre [Korrektur-][ST9] [Verifikationen][SP9] sind erhalten. Die optionale P3-Testhelfer-Duplizierung ist behoben. Beide [Code10-][ST10] [Verifikationen][SP10] melden keine neuen/offenen Befunde; sie belegen keine native Abnahme.
- [Code9][N9] belegt die reale Update-Kette **8→9**, erste Name-Scroll-/Edit-/Gboard-/Back-Fälle und unveränderte Speicherdateien. Code9 bleibt wegen des danach korrigierten Griffs/Hinweises ein Zwischenstand. Die [Code8-Motion-Baseline][B8] ist vorhanden; der vergleichbare Kandidatenlauf fehlt.

**Native Gates für Code10 bleiben offen:** reale Update-Kette **9→10** vor jedem Fixture-Replay; vollständige Gruppenbedienung und Persistenz; Legacy-/Maximalgruppe, DE/EN und große Schrift; öffentliche Accessibility und private Geheimhaltung; vergleichbare tatsächliche Motion-Messung plus visuelle Zeitfolge; Reduced Motion und Unterbrechungen. Finale Paketprüfung, Evidenzsicherung und Wiederherstellung des beabsichtigten Gerätezustands werden separat nachgeführt [PLAN].

## #17 — Gruppenbedienung

| Story | Kurzanforderung | Vorhandene Evidenz | Offene native Code10-Abnahme |
| --- | --- | --- | --- |
| 17.1 | Über Namen scrollen, ohne Bearbeitung zu öffnen | Name-Drag-Szenario [G10]; Code9: 8px-Wisch, Tap und längerer Scroll [N9] | Auf finalem Kandidaten bestätigen; kein Editor, Keyboard oder Reorder. |
| 17.2 | Expliziter Edit-Button je Zeile | Bewusstes Bearbeiten [G10]; echte Gboard-Öffnung auf Code9 [N9] | Korrigierte Darstellung und gezielte Bedienung auf Android. |
| 17.3 | Keine normalen Pause-/Mitmachen-Aktionen | Produkt-/Layoutbeschreibung [GR], Spec-Verifikation [SP10] | Normale Zeilen und öffentliche Semantik prüfen; Legacy getrennt halten. |
| 17.4 | Umbenennen erhält Identität und Position | Session-Rename [S], gerenderte Gruppenaktionen [R] | Tatsächlich speichern, stabile ID/Position und Wiederöffnung nachweisen. |
| 17.5 | Person entfernen | Session-Remove [S], Gruppenaktionen [R] | Reale Entfernung und sichtbares Ergebnis. |
| 17.6 | Letzte Entfernung mit ID und Position rückgängig machen | Session-Undo/Wiederöffnung [S], Gruppenaktionen [R] | Remove→Undo samt beider Speicherstände prüfen. |
| 17.7 | Gruppe vor neuer Partie umordnen | Atomare ID-Reihenfolge [S]; gültiger Handle-Drop [G10] | Echter Griff-Drag, neue Reihenfolge und dauerhafte Speicherung. |
| 17.8 | Reorder von normalem Scrollen unterscheiden | Name-Drag, Handle, Autoscroll [G10]; Code10-Griff geprüft [SP10] | Reale Griff-/Namengesten und korrigierten Griff vergleichen. |
| 17.9 | Nicht ziehende Alternative zum Verschieben | Move-Buttons bis erstes/letztes Element [G10] | Move up/down mit nativen Accessibility-Aktionen und erreichbaren Controls. |
| 17.10 | Reihenfolge über Neustart und Moduswechsel erhalten | Dauerhafte atomare Reihenfolge [S] | Prozessneustart und Quick/Classic/Kings-Wechsel mit gespeicherter Reihenfolge. |
| 17.11 | Nächste Kartenübergabe folgt Gruppenreihenfolge | Session-Reorder/Next-Handoff [S][GR] | Neue Partie starten und tatsächliche Übergabefolge belegen. |
| 17.12 | Abgebrochener Reorder behält letzte bestätigte Reihenfolge | Cancel, Outside-Drop, Fokusverlust [G10] | Unterbrechung/Abbruch und Outside-Drop; keine Teilreihenfolge speichern. |
| 17.13 | Speicherfehler bewahrt Gruppe und meldet Fehlschlag | Reale verweigerte Writes [S]; Failed-Drop-Feedback [G10] | Finaler Android-Bedienlauf offen; bisheriger Fehlernachweis ist Session/gerendert, kein behaupteter nativer Fault-Test. |
| 17.14 | Lange/doppelte Unicode-Namen bleiben unterscheidbar | Stabile Unicode-IDs [S], große zweisprachige Gruppe [G10] | Maximalgruppe mit nativen Fonts; richtige Person bearbeiten/verschieben. |
| 17.15 | Alte pausierte Namen bewahren, nicht automatisch aktivieren | Legacy-Persistenz [S], eingeklappte explizite Wiederherstellung [G10] | Markiertes Legacy-Fixture: unverändert öffnen, getrennten Bereich und Kapazitätsgrenzen prüfen. |
| 17.16 | Kapazität erklären; keine still pausierte neue Person | Capacity-RED/GREEN [GR], Session [S], große Gruppe [G10] | 20 aktive / Legacy-Kompatibilität; sichtbare Erklärung und kein stiller Overflow. |
| 17.17 | Cancel und Android Back schließen Keyboard ohne Rename | Gerenderte Edit-/Keyboard-Layouts [R][G10]; echter Draft+Back auf Code9 [N9] | Code10: echte IME, Cancel/Back, unveränderte bestätigte Namen. |
| 17.18 | Neue Controls/Hinweise vollständig DE/EN | Zweisprachige Gruppe [G10], gekürzter Hinweis [SP10] | DE/EN auf Android inklusive Edit, Move, Remove/Undo und Legacy. |
| 17.19 | Kleine Anzeige/große Schrift: Namen und Aktionen erreichbar | Große zweisprachige Gruppen/Keyboard-Viewport [G10], bestehende Layout-Regressionen [R] | Native Insets, 150% Schrift, lange letzte Zeile, Griff und Gboard. |
| 17.20 | Live-Match, Historie und Rollenzuteilung erhalten | Session-/Migrationsfälle [S]; Code8→9-Dateien unverändert [N9] | Code9→10 bytegleich vor Mutation; bestehende eingefrorene Partie und Historie erhalten. |

## #18 — Flüssige Interaktionen

| Story | Kurzanforderung | Vorhandene Evidenz | Offene native Code10-Abnahme |
| --- | --- | --- | --- |
| 18.1 | Scrollen folgt dem Finger flüssig | Idle-Code8-Scrolltraces [B8]; Implementierungsbericht [MR] | Vergleichbare unaufgezeichnete Kandidaten-Traces und Bewegung über Zeit; kein FPS-Versprechen aus Settings. |
| 18.2 | Gleichmäßige tatsächliche Bildausgabe | Actual-present-Baseline mit Median/p95/Stalls [B8]; Display-Anforderung dokumentiert [MR] | Gleiche Umgebung/Inputfolge; tatsächliche Median-/Tail-Intervalle, Stalls und vollständige Gestenabdeckung. |
| 18.3 | Private Karte folgt Zug direkt | Regrab/kein Positionssprung [M10], Kartenfluss [R] | Tatsächlicher Zug und erneutes Greifen in visueller Zeitfolge; Grenzen des Emulators nennen. |
| 18.4 | Weiche Rückkehr beim Loslassen | Partial-Return- und Regrab-RED/GREEN [MR], erneute Motion-Tests [M10] | Kontinuierliche Rückkehr mit vergleichbarer Aufnahme inspizieren; keine Ableitung aus Einzelbildern. |
| 18.5 | Geheimnisse bei Release/Unterbrechung sofort verbergen | Synchrones Concealment-/Lifecycle-Szenario [R]; Return kann nicht erneut aufdecken [M10] | Release, Cancel, zweiter Kontakt, Home/Resume samt privaten Wörtern/Rollen/Listen; nur leere Dekoration darf nachlaufen. |
| 18.6 | Erstes Aufdecken ohne rollenbedingten Hitch | Einmaliges Textbefüllen/lokaler Kartenstatus [MR]; funktionale Kartenfälle [R] | Erstes und warmes Reveal samt langen privaten Namen messen; Cache allein beweist keine Hitch-Freiheit. |
| 18.7 | Aufdecken und lange private Liste zusammen bedienen | Maximale Kings-Listen bei großer Schrift [R] | Tatsächliche lange private Liste halten/scrollen und bis zum Ende prüfen. |
| 18.8 | Bestehende Reduced-Motion-Präferenz respektieren | Sofortiges Settle laut Implementierungsbericht [MR]; Spec-Verifikation [SP10] | Native Systemeinstellung, sichtbare Rückkehr ohne Dekoration und unveränderte Geheimhaltung. |
| 18.9 | Hold, Multitouch-Abbruch und sicheres Resume erhalten | Hold/Cancel/Lifecycle [R]; getrennt gehaltener Kontakt und Reorder-Ende [M10] | Reale Kontakt-/Unterbrechungsfolge und verdecktes Resume auf finalem APK. |
| 18.10 | Gruppen-Scroll, Edit und Reorder koexistieren | Gruppe [G10] plus erfasster Reorder→Reveal-Kontakt [M10] | Echte Gruppenaktionen vor Kartenfluss; kein hängen gebliebener Kontakt oder unbeabsichtigtes Edit. |
| 18.11 | Verbesserung ohne neue Konfiguration | Änderungen und Scope [MR], keine neuen Spec-Befunde [SP10] | Normaler Start/Resume mit bestehenden Einstellungen; keine neue Einrichtung voraussetzen. |
| 18.12 | Rendering ändert Sprache, Gruppe, Historie und Save nicht | Unveränderte Session-Quellen [EQ10], Session [S], reale 8→9-Kette [N9] | 9→10 beide Generationen prüfen; Motion-Fixtures erst danach separat markieren und Zustand wiederherstellen. |

## Grenzen und Nachführung

- Der nachgewiesene Code8-Baseline-Median liegt in den vier gespeicherten Läufen bei etwa **45.85–48.07 ms**, nicht bei behaupteten 33.3 ms. Der erste Scrolllauf enthält einen **1007.08-ms-Stall**; er bleibt erhalten. Baseline und Kandidat brauchen dieselbe Perzentildefinition, passende Geometrie/Fixtures und tatsächliche Geräteeingabezeitstempel [B8]. Die separate 60-fps-Videokodierung verschlechterte das Timing und ist keine Performance-Messung der App.
- Frühere fehlgeschlagene vollständige Gruppen-/Corpus-Läufe, Fixture-Probleme, der Code9-Darstellungsbefund und der frühe leere Startup-Screenshot bleiben erhalten [GR][N9]. 76/76 und fokussierte Korrekturtests löschen diese Beobachtungen nicht und diagnostizieren insbesondere keinen behobenen Windows-Dateisystemfehler.
- Accessibility benötigt native Zielbäume und tatsächliche öffentliche Aktionen; gerenderte Semantik und private Ausschlüsse [R] allein schließen dieses Gate nicht. Keine Behauptung zu hörbarem TalkBack, blinder Bedienbarkeit, physischem Handy oder Haptik aus dem übersetzten Emulator.
- **#11 (freie Rollenzahlen) gehört nicht zu #17/#18 und wartet auf drei menschliche Regelantworten.** Es werden keine Regeln ergänzt oder Antworten unterstellt. Auch das Umsortieren bereits eingefrorener/unabgelesener Live-Übergaben wird nicht stillschweigend Teil dieser Abnahme.
- Nach den finalen nativen Checks folgt der eigentliche Evidenz-Audit: genaue Kandidaten-/Save-Hashes, tatsächlich ausgeführte Aktionen, Messfenster und Grenzen werden je Story ergänzt. Bis dahin bleiben die oben benannten nativen Gates offen.

[S17]: ../../specs/group-usability.md
[S18]: ../../specs/fluid-interactions.md
[S]: evidence/integration-session-full.txt
[R]: ../fluid-interactions/evidence/integrated-rendered-final-result.json
[GR]: ../group-usability/report.md
[MR]: ../fluid-interactions/report.md
[G9]: evidence/worktree/artifacts/usability-final/group-test-result.json
[M9]: evidence/worktree/artifacts/usability-final/motion-test-result.json
[G10]: evidence/worktree/artifacts/usability-final/code10/group-test-result.json
[M10]: evidence/worktree/artifacts/usability-final/code10/motion-test-result.json
[EQ10]: evidence/worktree/artifacts/usability-final/code10/source-equivalence.json
[N9]: code9-functional-report.md
[B8]: baseline-report.md
[PLAN]: acceptance-plan.md
[ST]: standards-review.md
[SP]: spec-review.md
[ST9]: standards-correction-verification.md
[SP9]: spec-correction-verification.md
[ST10]: standards-code10-verification.md
[SP10]: spec-code10-verification.md

---

Lesefassung mit dauerhaften relativen Links. Das [unveränderte eingefrorene Original](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/native/acceptance-matrix-pre-final.md) bleibt einschließlich früherer Formulierungen erhalten. Die aktuelle Updatebeschreibung unterscheidet den vorherigen Originalrestore vom eigentlichen Installationsversuch.
