# Usability-Abnahmematrix — finaler Code11-Evidenzstand, 32 Stories

9. Oktober 2026; [#17 Gruppenbedienung][S17] (20 Stories), [#18 Interaktionen][S18] (12 Stories). Kandidat **1.2.0 / Code11**, gebaute/installierte Quelle `25d3e9f9d80609c178230cf838cecfda54ae0370`. Der vorherige offene Code10-Stand ist unverändert als [pre-final](acceptance-matrix-pre-final.md) gesichert (SHA256 `86d0bd6d69264f06a08dc263ceec400b9d2188e98d5e4e661cae1f5e4f70dad0`). Keine neuen Tests, ADB-/Editor-/Trackeraktionen durch diesen Audit.

**Funktionale Korrekturen, reale Update-Kette, Cancel/Drop, große/alte Gruppen, Privatsphäre, Reduced Motion nach Prozessneustart und Endwiederherstellung sind durch die unten bezeichneten Belege gedeckt. Eine pauschale Behauptung „32/32 vollständig empirisch erfüllt“ wäre falsch:** 18.1, 18.2 und 18.6 bleiben hinsichtlich durchgehend flüssiger Ausgabe beziehungsweise Hitchfreiheit nur teilweise empirisch belegt. Die geforderte vergleichbare Messung wurde durchgeführt; daraus wird keine zusätzliche Pflicht zu garantierten 60 fps oder einer physischen Geräteabnahme erfunden.

„Funktional belegt“ bedeutet die Kombination der konkret genannten vorhandenen Nachweise. **S/R** kennzeichnet Session-/gerenderte Tests; **nativ** bezeichnet tatsächliche Bedienung auf dem übersetzten ARM64/API36-Emulator. Kein Status behauptet eine Ausführung jeder Variante auf physischer Hardware.

## Nachweisbasis

- **Session 76/76** am Integrationsstand `3835a71035b0fe4c471e5893a54f2438b5c34505` [S]; [Code10-][EQ10] und [Code11-Quellgleichheit][EQ11] erhalten Session und deren Tests. Die 76 Fälle wurden für diesen Audit nicht erneut ausgeführt.
- **Final gerendert 61/61**, 0 Fehler/Skips/Inconclusive, einschließlich Gruppe 6, Motion 4 und tatsächlichem Provider 4 [R11]. Die zusätzlichen Providerfälle sind separat **4/4** nach Fixture-Isolation belegt [P11]. Frühere 57/58/59- und rote Zwischenstände bleiben erhalten.
- [Terminale Spec-][SP11] und [Standards-Reviews][ST11]: 0 offene Befunde bei `25d3e9f…`. Frühere P2-Berichte werden nicht umgedeutet.
- Reale Installation **8→9→10→11**, pro Kandidat vor dessen Testfixtures; vorher konnte eine ausdrückliche Originalwiederherstellung stattfinden. Insbesondere erfolgte unmittelbar vor 10→11 das [Original-Replay](evidence/code10-before-final-update-original-replay/manifest.json). Der anschließende Updateversuch selbst führte kein Replay aus und erhielt beide Originalgenerationen [N9][N10][U11]. [Finaler nativer Audit][N11] und [Hash-/Payload-Audit][J11] prüfen 20 Code11-Save-Generationen, vier Abbrüche, atomaren Drop, direkte neue Partie, späteren Neustart und finale Originalwiederherstellung.
- APK SHA256 `9c08b19ab2270b236cd2eec9a8d9bd2f67b7a9db93fc0ff3d5d5f2a5d5e8c9ad`; AAB `9148cb972b7c6b256ad0838f084ec3bfe2693e3cfbd70bcd5885e4b6623d930e`. Beider Dateihashes nachgerechnet; APK-Inspektion 14, AAB-Inspektion 44 Befehle erfolgreich, gleiche Quelle [APK][AAB]. Lokal debug-signierte Testpakete, keine Produktionsveröffentlichung.
- Von Code10 zu Code11 ändert sich im Runtimecode ausschließlich `GroupReorder.cs` [EQ11]. Unveränderte Code10-UI-/Accessibility-/Karten-/Motion-Befunde bleiben in dieser Reichweite gültig; Reorder hat eigene Code11-Nachweise. Der Implementer meldet Handoff `d4b1d2cbaf01e11cfb627ec2b2b2e6a8610850bb` mit unverändertem game/tools/tests gegenüber der gebauten Quelle; Repository-Integration/Evidenzarchivierung liegen beim Root.

## #17 — Gruppenbedienung

| Story | Kurzanforderung | Belege und Bewertung |
| --- | --- | --- |
| 17.1 | Namen scrollen ohne Edit | **Funktional belegt.** Code9: 8-px-Wisch, Tap und langer Name-Drag ohne Editor/IME oder Save-Änderung [N9]; finaler gerenderter NameDrag-/OrdinaryScroll-Fall [R11]. Kein behaupteter neuer Code11-Kurzswipe. |
| 17.2 | Expliziter Edit-Button | **Funktional/nativ belegt.** Bewusstes Öffnen, echter Gboard-Editor und Code10-Griff/Einzeilenhinweis mit zwei vollständigen Zeilen [N10]; finale UI-Tests [R11]. |
| 17.3 | Keine Pause-/Join-Aktionen auf normalen Zeilen | **Funktional/nativ belegt.** Normale Code10/11-Gruppe mit Edit, öffentlich gelesene Bäume; nur Legacy hat explizites Hinzufügen [N10][N11]. |
| 17.4 | Rename erhält Identität/Position | **Funktional/nativ belegt.** Nora→NoraTest10 ändert ausschließlich Name; ID, Position, History unverändert [N10]. Session und wiedereröffnete Controls [S][R11]. |
| 17.5 | Entfernen | **Funktional/nativ belegt.** Entfernung NoraTest10, vier verbleibende Personen, Removed mit gleicher ID und Index [N10]; S/R [S][R11]. |
| 17.6 | Undo erhält ID/Position | **Funktional/nativ belegt.** Undo-Primary einschließlich voller Players-Liste bytegleich dem Rename-Stand; Backup korrekt als Zwischenstand erhalten [N10]. |
| 17.7 | Vor neuer Partie umordnen | **Funktional/nativ belegt.** Code11-Drop schreibt genau eine neue Reihenfolge, identische vollständige Playerrecords und History [N11][J11]. Der ursprüngliche Fixindex-Harness bleibt fehlgeschlagen; tatsächlicher dritter Zielplatz separat geprüft. |
| 17.8 | Reorder vom Scroll unterscheiden | **Funktional belegt.** Name-Scroll [N9], sichtbarer leichter Griff [N10], gültiger Griff-Drag/Autoscroll [N11], finales getrenntes Gestenverhalten [R11]. |
| 17.9 | Nicht ziehende Move-Alternative | **Funktional/nativ belegt.** Tatsächliches Move down und native Accessibility-Aktion bei 150%; Editorcontrols erreichbar [N10]. Beide Listenenden/Move up-down S/R [R11], keine behauptete vollständige native Pfeil-Permutation. |
| 17.10 | Reihenfolge über Neustart/Moduswechsel | **Funktional belegt.** Code10 Kings→EN/Quick und Neustart bytegleich [N10]; Code11 neu geordnete Kings-Partie über echten Neustart bytegleich [N11]. Modus-/Persistenzkombinationen zusätzlich S/R [S][R11]; kein separater nativer Rundlauf jeder Kombination behauptet. |
| 17.11 | Neue Kartenfolge nutzt Reihenfolge | **Funktional/nativ belegt.** Direkt nach Code11-Drag ohne Neustart neue Partie: Luca 1/5, Emil 2/5; gespeicherte Participants stimmen exakt mit Gruppe überein [N11]. |
| 17.12 | Abbruch erhält bestätigte Reihenfolge | **Funktional/nativ belegt.** Code10 zweimal rot; Code11 Cancel zweimal, Cancel+neuer Kontakt sowie Outside jeweils beide Generationen unverändert [N10][N11]. Gepufferte Provider-Sonderfälle zusätzlich 4/4 [P11]; nativer 13-ms-Kontaktabstand wird nicht als garantierter gemeinsamer Unity-Frame bezeichnet. |
| 17.13 | Speicherfehler bewahrt Gruppe/meldet Fehler | **S/R belegt.** Reale verweigerte Writes und Wiederöffnung [S]; finaler FailedDrop-/Feedback-Fall [R11]. Kein nativer Android-Dateisystemfehler injiziert; dieser Randfall ist empirisch an den vereinbarten Session-/Rendered-Seams geprüft. |
| 17.14 | Doppelte/lange Unicode-Namen unterscheiden | **S/R und begrenzte native Darstellung belegt.** IDs/Duplikate und Auswahl S/R [S][R11]; native lange Legacy-Namen, vollständige Speech-Labels und mehrschriftige private Kings-Listen [N10]. Kein nativer Edit/Move jeder Schrift-/Duplikatkombination behauptet. |
| 17.15 | Alte inaktive Namen bewahren | **Funktional/nativ belegt.** Markiertes V4-Fixture mit 20 aktiven +20 alten Namen; Fold erreichbar, Edit21, drei UI-only-Checkpoints beide Generationen bytegleich [N10]. Explizite Wiederaufnahme bei freier Kapazität zusätzlich S/R [R11]. |
| 17.16 | Kapazität erklären, kein stiller Overflow | **Funktional/nativ belegt.** Tatsächliche DE-Kapazitätsansicht; Legacy-Hinzufügen bei 20 aktiv deaktiviert, Erklärung sichtbar [N10]. Add-Kapazität über S/R [S][R11]; erstes falsch DE-benanntes Bild bleibt EN-Beleg. |
| 17.17 | Cancel/Android Back ohne Rename | **Funktional belegt.** Tatsächlicher Draft+Back mit echter Gboard-IME und unveränderten Saves auf Code9 [N9]; native Code10-Cancel/Back-/Legacy-Editorfolge [N10]; finale unveränderte Editlogik/S/R [R11]. |
| 17.18 | Controls/Hinweise DE/EN | **Funktional/nativ belegt.** DE/EN-Gruppe/Editor/Move, native Kapazität/Legacy und beide privaten Karten bei 150% [N10]; finale zweisprachige Layout-/Copy-Fälle [R11]. |
| 17.19 | Kleine Anzeige/große Schrift erreichbar | **Funktional/nativ belegt.** 150%-Gboard-Editor und Native Scroll zum Legacy-Fold/ersten Control; DE/EN-private Liste bis letztem Namen [N10]. Kleine Rendered-Viewports [R11]. Nicht alle Controls gleichzeitig sichtbar und keine physische Geräteabnahme behauptet. |
| 17.20 | Live-Match, History und Rollen erhalten | **Funktional belegt.** Update-Kette beide Generationen bytegleich [N9][N10][U11]; Code11 gespielte Partie einschließlich Handoff/Rollen/History über Neustart identisch [N11]. Eingefrorene Live-Matches zusätzlich S/R [S][R11]; primärer Zustand bei der Updateinstallation war Gruppe, kein erfundener aktiver-Primary-Updateversuch. |

## #18 — Flüssige Interaktionen

| Story | Kurzanforderung | Belege und Bewertung |
| --- | --- | --- |
| 18.1 | Scrollen folgt flüssig | **Nur teilweise empirisch belegt.** Vergleichbare echte Inputs und Present-Traces: Code10-Scrollmediane 32,89/33,89 ms gegenüber 46,88 ms Code8-Repeat; p95/Maxima bleiben teils schlechter [M]. Verbesserter typischer Takt ist keine durchgehende Ruckelfreiheit oder physische Wahrnehmungsabnahme. |
| 18.2 | Gleichmäßiges Frame-Pacing | **Nicht uneingeschränkt empirisch erfüllt.** Alle vier Kandidatenläufe 4/4 Gesten abgedeckt; Mediane 31,71–33,89 ms, nicht konstant nahe 16,7 ms; Tails bis 200,33 ms [M]. Umsetzung und Vergleich sind erledigt, eine stabile 60-fps-Ausgabe ist nicht nachgewiesen. |
| 18.3 | Karte folgt Zug direkt | **Funktional/visuell belegt.** Direkte Pullfolge und gehaltene/verdeckt-Ansichten [M][N11]; gerendertes Regrab ohne Positionssprung [R11]. Kein gemessener Touch-to-photon-Wert und kein nativer Regrab innerhalb einer Rückkehr behauptet. |
| 18.4 | Weiche Rückkehr | **Funktionale Rückkehr visuell belegt.** Aus Quellvideo extrahierte Framefolge zeigt mehrere verdeckte Rückkehrpositionen [M]; PartialReturn-/Regrab-Fälle [R11]. Gesamte Bildausgabe bleibt durch 18.1/.2 begrenzt; Root prüfte Frames, spielte das WebM nicht in Echtzeit ab. |
| 18.5 | Geheimnisse bei Release/Unterbrechung verdecken | **Funktional/nativ belegt.** Release, Cancel, zweiter Kontakt, korrelierter Home-Fokus und sichere Rückkehr [M]; native Max-Kartenbäume ohne private Werte [N10]; Code11-Folge [N11]. Synchrone Logik S/R; keine subframegenaue native Verdeckungslatenz behauptet. |
| 18.6 | Erstes Reveal ohne rollenbedingten Hitch | **Nur teilweise empirisch belegt.** Karteninhalt/Privatlisten funktional und visuell geprüft [N10][N11][R11], warme Traces verbessert [M]. Kein kontrollierter rollenweiser First-Reveal-/CPU-/GPU-Nachweis; erster Code10-Pulllauf enthält 200,33 ms, deren Ursache nicht zugeordnet ist. Keine allgemeine Hitchfreiheit. |
| 18.7 | Lange Privatliste mit Reveal scrollen | **Funktional/nativ belegt.** DE/EN-GoodKing-20er-Fixtures bei 150% bis zum letzten Namen gehalten/gescrollt und danach verdeckt; öffentliche Held/Released-Bäume sekretfrei [N10]; max Listen S/R [R11]. |
| 18.8 | Bestehendes Reduced Motion | **Begrenzter nativer Nachweis bestanden.** Nach echtem Prozessneustart bei Wert 0 zeigt gültige READY→Hold→Recorder→Release-Folge direkten Wechsel zur verdeckten Endposition [M]. Live-Übernahme nur durch Resume bleibt unbelegt; beide ungeeigneten Vorversuche bleiben erhalten. |
| 18.9 | Hold, Multitouch, sicheres Resume | **Funktional/nativ belegt.** Code10 zweiter/verbleibender Kontakt verdeckt, neuer Kontakt öffnet, Cancel verdeckt, Home/Resume sicher [M]; Code11 echter späterer Neustart sicher [N11]; finale Lifecycle-/Providerregression [R11][P11]. |
| 18.10 | Gruppe und Kartenkontakte koexistieren | **Funktional/nativ belegt.** Edit/Move/Scroll [N9][N10]; Code11 Abbrüche→Drag7→direkter neuer Matchstart→Reveal0→nächste Karte ohne Zwischenneustart [N11]. Genaues Kontaktcleanup zusätzlich S/R [R11]. |
| 18.11 | Keine neue Konfiguration nötig | **Funktional/nativ belegt.** Reale Update-/Start-/Fortsetzenfolge mit vorhandenen Einstellungen [N10][N11]; keine neue Gameplay-Einstellung in Scope/Quellreview [SP11]. Temporäre Systemwerte sind ausdrücklich Validierungseingriffe und wiederhergestellt. |
| 18.12 | Rendering erhält Sprache/Gruppe/History/Save | **Funktional/nativ belegt.** Session unverändert [EQ10][EQ11], reale Updates erhalten beide Generationen [U11]; tatsächlicher Code11-Neustart und abschließende exakte Originalwiederherstellung [N11][J11]. Fixtures getrennt, keine Rerandomisierung als Vergleichsbeleg. |

## Beibehaltene Grenzen und Endzustand

- Die drei Performance-Stories 18.1/.2/.6 sind **nicht voll empirisch erfüllt**. Medianverbesserung, gemischte Tails und Messgrenzen stehen in [M]. Kein CPU-/GPU-Ursachenbeweis, keine zugesicherten 60 fps, keine Behauptung über das Handy des Nutzers. Dies ist die verlangte transparente Messabnahme; zusätzliche hypothetische Tests werden daraus nicht abgeleitet.
- S/R-only-Randfälle sind in 17.13, Teilen von 17.14 und den übrigen Zeilen ausdrücklich bezeichnet. Native Accessibility belegt öffentliche Nodes und tatsächlich ausgelöste Aktionen; hörbares TalkBack, blinde Bedienbarkeit, Haptik und tatsächliche TalkBack-Durchreichgeste wurden nicht behauptet.
- Original-Harness bei Code11 erwartete falschen Fixindex und blieb rot; tatsächlicher 553-ms-Rand-Hold/AutoScroll und atomarer dritter Zielplatz wurden separat auditiert [N11]. Vier Abortchecks davor waren grün. Code10-Cancel bleibt zweimal rot [N10]. Fehlgeschlagene Provider-/breitere Testzwischenstände, Host-Dateisystemprobleme, blanke Startframes, falsch benanntes EN-Kapazitätsbild, CP1252/UTF-8-Toolausgaben, frühe 0-/7-Node-Bäume, unkorreliertes Home-Bild und ungeeignete Reduced-Vorläufe bleiben erhalten [N9][N10][M][SP11][ST11].
- Final beide Savegenerationen exakt wiederhergestellt: Primary `f146725f8d3e527fc5a71ef1ee4ac2eb80048c419a54a3dbf3822ea0f6fcf28b`, Previous `d47a4f6ae89377513a09dec09c9230f7498080e2b673be0e5596950f4f0e9467`; fünf ursprüngliche aktive Personen, History12, Primary-Match null [N11]. Root bestätigte App gestoppt, Font1.0/Motion null und restauriertes TalkBack. Gespielte Code11-Partie bleibt in der Evidenz erhalten.
- **#11 freie Rollenzahlen gehört nicht zu #17/#18 und wartet auf drei menschliche Regelantworten.** Keine Antworten, Regeln oder freie Umordnung bereits eingefrorener Live-Handoffs wurden ergänzt. Repository-Integration und vollständige Evidenzsicherung bleiben separat koordinierte Arbeit; diese Matrix schreibt keinen Trackerstatus.

[S17]: ../../specs/group-usability.md
[S18]: ../../specs/fluid-interactions.md
[S]: evidence/integration-session-full.txt
[R11]: evidence/worktree/artifacts/usability-final/code11/full-rendered-isolated-completed.json
[P11]: evidence/worktree/artifacts/usability-final/code11/provider-after-isolation-result.json
[EQ10]: evidence/worktree/artifacts/usability-final/code10/source-equivalence.json
[EQ11]: evidence/worktree/artifacts/usability-final/code11/terminal-source-equivalence.json
[ST11]: standards-code11-terminal-verification.md
[SP11]: spec-code11-terminal-verification.md
[N9]: code9-functional-report.md
[N10]: code10-functional-report.md
[N11]: code11-native-audit.md
[J11]: evidence/code11-native-audit.json
[U11]: evidence/code10-to11-update-verified.json
[M]: motion-comparison-report.md
[APK]: evidence/worktree/artifacts/usability-final/code11/apk-terminal-inspection-result.txt
[AAB]: evidence/worktree/artifacts/usability-final/code11/aab-terminal-inspection-result.txt

---

Lesefassung mit dauerhaften relativen Links. Das [unveränderte eingefrorene Original](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/native/acceptance-matrix.md) bleibt einschließlich früherer Formulierungen erhalten. Die aktuelle Updatebeschreibung unterscheidet den vorherigen Originalrestore vom eigentlichen Installationsversuch.
