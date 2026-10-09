# Code11: abschließender nativer Evidenz-Audit

9. Oktober 2026. Read-only Prüfung vorhandener Dateien; keine ADB-/Editor-/Test-/Trackeraktion. Native Ausführung und angegebene Sichtprüfungen stammen von Root. Der zusätzliche [maschinenlesbare Audit](evidence/code11-native-audit.json) enthält exakte Hashes, 20 validierte Save-Generationen aus zehn Checkpoints und die Einzelvergleiche. **Die Code11-Funktionsfolge ist belegt; daraus folgt keine allgemeine Hitchfreiheit oder 60-fps-Abnahme.** Die [32-Story-Matrix](acceptance-matrix.md) nennt insbesondere die verbleibenden empirischen Grenzen von 18.1, 18.2 und 18.6.

## Kandidat, Regression und Übernahme

Installierte Quelle: `25d3e9f9d80609c178230cf838cecfda54ae0370`, 1.2.0 / Code11, Unity 6000.3.25f1. Der [10→11-Beleg](evidence/code10-to11-update-verified.json) dokumentiert tatsächliches `install -r` mit beiden ursprünglichen Generationen unverändert. Direkt vor diesem Update restaurierte Root die Code8-Originalgenerationen ausdrücklich über [code10-before-final-update-original-replay](evidence/code10-before-final-update-original-replay/manifest.json). Der Updateversuch selbst führte kein Replay aus. Dies belegt Update-Erhaltung des restaurierten Zustands, keine ununterbrochene Nutzung ohne zwischenzeitliche Wiederherstellung. Frühere reale Schritte 8→9 und 9→10 sind in den [Code9-](code9-functional-report.md) und [Code10-Berichten](code10-functional-report.md) erhalten.

- APK: `9c08b19ab2270b236cd2eec9a8d9bd2f67b7a9db93fc0ff3d5d5f2a5d5e8c9ad`, 37.472.202 Bytes; [Paketinspektion](evidence/worktree/artifacts/usability-final/code11/apk-terminal-inspection-result.txt), 14 erfolgreiche Inspektionsbefehle.
- AAB derselben Quelle: `9148cb972b7c6b256ad0838f084ec3bfe2693e3cfbd70bcd5885e4b6623d930e`, 37.829.159 Bytes; [Paketinspektion](evidence/worktree/artifacts/usability-final/code11/aab-terminal-inspection-result.txt), vier untersuchte Pakete und 44 erfolgreiche Inspektionsbefehle. Das AAB wurde hier nicht als auf Android installiert behauptet.

Beide Paketdatei-Hashes wurden unabhängig gelesen und nachgerechnet. Das sind lokal debug-signierte Testpakete; keine Store-Veröffentlichung. Der [finale Quellvergleich](evidence/worktree/artifacts/usability-final/code11/terminal-source-equivalence.json) und ein unabhängiger read-only Git-Vergleich bestätigen gegenüber Code10 ausschließlich `GroupReorder.cs` als geänderten Runtimecode. Unveränderte Code10-Gruppenansichten, private Karten, Accessibility, Motion und Session-Beobachtungen werden in genau dieser Reichweite übernommen. Das geänderte Reorder-Verhalten hat eigene Code11-Gerätebelege.

Die finale [gerenderte Suite](evidence/worktree/artifacts/usability-final/code11/full-rendered-isolated-completed.json) enthält 61/61 bestandene Ergebnisse, 0 Fehler/Skips/Inconclusive. Die [vier tatsächlichen Input-Provider-Szenarien](evidence/worktree/artifacts/usability-final/code11/provider-after-isolation-result.json) sind 4/4 grün. [Spec-](spec-code11-terminal-verification.md) und [Standards-Review](standards-code11-terminal-verification.md) bestätigen den terminalen Stand ohne offene Befunde. Die früheren P2-Befunde und roten Zwischenläufe werden dadurch nicht nachträglich grün. Die Session-Suite bleibt 76/76 am erhaltenen Integrationsstand, mit unveränderten Session-Quellen/-Tests.

## Vier Abbrüche, dann ein atomarer Move

Der [separate Ergebnisvalidator](evidence/code11-native-reorder-reviewed-result.json) wurde durch die tatsächlichen Save-Dateien nachgeprüft:

| Folge | Nativer Input | Offline-Befund |
| --- | --- | --- |
| Cancel | [DOWN/MOVE/CANCEL](evidence/code11-native-cancel-native-input.json) | Beide Generationen exakt gleich dem [Start](evidence/code11-reorder-start-state/manifest.json). |
| Cancel erneut | [Wiederholung](evidence/code11-native-cancel-repeat-native-input.json) | Wieder beide Generationen unverändert. |
| Cancel und neuer Kontakt | [Kontakt 7 abbrechen, Kontakt 9 beginnen](evidence/code11-native-cancel-new-contact-native-input.json) | Beide Generationen unverändert; tatsächlicher Abstand CANCEL→neues DOWN 13 ms. Ein bestimmtes Unity-Update wird daraus nicht abgeleitet; exakt gemeinsam gepufferte Fälle sind separat durch Provider-Tests belegt. |
| Outside-Drop | [MOVE außerhalb und UP](evidence/code11-native-outside-native-input.json) | Beide Generationen unverändert. |
| Gültiger Drop danach | [DOWN/MOVE/UP](evidence/code11-native-valid-drop-native-input.json) | Nora wechselt vom ersten auf den **dritten Listenplatz, nullbasierter Index 2**. Ergebnis Luca, Emil, Nora, Mila, Jonas2; sämtliche vollständigen Playerrecords und alle anderen Payload-Felder unverändert. Previous ist exakt der Primary vor dem Drop. |

Alle Eingabeprotokolle gehören zu Code11, enden mit Exitcode 0 und melden erfolgreiche Injektion. Das alleine ist kein funktionaler Pass; die Byte-/Payload-Vergleiche liefern den Ergebnisnachweis. Primary nach Drop: `cdaf0acdcea738c68405275152f2aa1ffcf4c672057387b57714c16efd63c567`.

**Der ursprüngliche [FinalReorderNative.py](evidence/FinalReorderNative.py) ist fehlgeschlagen und bleibt unverändert.** Seine Assertion erwartete nur einen Tausch der ersten beiden Personen. Der tatsächliche Rand-Hold zwischen MOVE bei 61.790.154 ms und UP bei 61.790.707 ms dauerte 553 ms. `GroupReorder.AutoScroll` scrollt bei `position.y > bounds.yMax - 36` alle 16 ms um 5 weiter und berechnet die Vorschau entsprechend. Root und dieser Auditor haben [den versetzten Scrollbalken und Emil/Nora](evidence/code11-valid-drop-observed.png) gesehen. Das erklärt den anderen gültigen Zielplatz; es ist kein angeblich bestandener ursprünglicher Fixindex-Test. Die Datei `code11-native-reorder-result.json` wurde wegen des Assertions nicht erfolgreich erzeugt; der gelesene Nachvalidator trägt ausdrücklich einen anderen Namen. Der im ursprünglichen Shell-Ablauf vorgesehene Neustart fand nach dem terminierenden Assertion nicht statt.

## Direkt folgende gespielte Partie und tatsächlicher Neustart

**Ohne Neustart zwischen Drag und Start** wurde eine neue reguläre Partie begonnen, ohne Fixture-Replay. Root sah [Luca als Karte 1/5](evidence/code11-first-handoff-after-drag.png), [Schwamm und Anführer Emil bei gehaltenem Kontakt](evidence/code11-following-finger-held.png), [verdeckt nach Release](evidence/code11-following-finger-released.png) und [Emil als Karte 2/5](evidence/code11-second-handoff-after-drag.png). Kontakt 0 funktioniert nach Reorder-Kontakt 7; Root führte diese native Folge aus. Dieser Auditor hat Held/Released zusätzlich angesehen.

Der [gespielte Zustand](evidence/code11-played-match-state/manifest.json) enthält Match `a7b639fb17d2451298f0b69f3f52e801`, Kings/DE, Pair `school-office-015`, Kreide/Schwamm, Teilnehmer in der tatsächlich gespeicherten Reihenfolge. UsedPairIds steigen exakt von zwölf auf dreizehn. Der nächste Primary unterscheidet sich vom vorherigen ausschließlich durch `/Match/Handoff: 0→1`; Rollen, Wörter, IDs, Reihenfolge und gesamte History bleiben gleich.

Der **spätere tatsächlich ausgeführte Prozessneustart** ist durch [before](evidence/code11-before-restart-state/manifest.json), [after](evidence/code11-after-restart-state/manifest.json) und [sicheren Fortsetzen-Bildschirm](evidence/code11-restart-resume.png) belegt. Root und dieser Auditor haben das Bild gesehen. Beide Generationen sind bytegleich:

- Primary `f84db2424823f9f2f649b52c639f63a9aa3a3e21f078e04f1ed51a51676195c2` — gespieltes Match, Handoff 1/Emil.
- Previous `6dd45fb4d0ee0fc5b1e4e38c5d12b2a6e82269ff3b887c743476a967ec0787da` — dieselbe Partie, Handoff 0.

Der gespeicherte [Logscan](evidence/code11-final-played-log.json) für PID 29493 umfasst 89 Zeilen, fünf Namen sowie Kreide/Schwamm: 0 private Treffer und 0 Diagnose-Treffer. Der Offline-Textvergleich bestätigt 0 private Treffer. Die Metadaten zählen 12.520 Bytes, die gespeicherte Datei 12.609 Bytes; beide Angaben bleiben getrennt. Es ist ein begrenzter Logausschnitt, keine Aussage über alle künftigen Sitzungen.

## Beabsichtigter Endzustand und Grenzen

Nach dem [ausdrücklich markierten Original-Replay](evidence/code11-final-original-restore-replay/manifest.json) ist [code11-final-original](evidence/code11-final-original-state/manifest.json) in beiden Generationen exakt gleich `code8-original-state`: Primary `f146725f8d3e527fc5a71ef1ee4ac2eb80048c419a54a3dbf3822ea0f6fcf28b`, Previous `d47a4f6ae89377513a09dec09c9230f7498080e2b673be0e5596950f4f0e9467`. Fünf ursprüngliche aktive Identitäten/Namen, zwölf UsedPairIds und `Match=null` im Primary. Das Replay wird nicht als neue Partie oder zusätzlicher Update-Erhaltungsnachweis gezählt.

Root bestätigt den abschließend gestoppten Prozess, Fontscale 1.0/Motion `null` und zuvor wiederhergestelltes TalkBack; die gespeicherten [Präferenzen](evidence/code10-final-preferences-restored-preferences.json) und [TalkBack-Werte](evidence/code10-talkback-restore.txt) stützen die Wiederherstellung. Dieser Audit fragt den laufenden Gerätezustand nicht nochmals ab.

Der [Motionbericht](motion-comparison-report.md) enthält vergleichbare Messungen, Home-Fokusnachweise und gültigen Reduced-Motion-Release nach Prozessneustart. **18.1, 18.2 und 18.6 sind trotz Umsetzung nicht als uneingeschränkt empirisch erfüllt nachgewiesen:** Scrollen hat weiter längere Tails, der gemessene Median erreicht keine gleichmäßigen 16,7 ms, und First-Reveal-Hitchfreiheit ist nicht rollenweise/profiliert belegt. Dies erweitert den vereinbarten Auftrag nicht um eine physische Abnahme oder garantierte 60 fps. Kein CPU-/GPU-Ursachenbeweis, keine Aussage zu hörbarem TalkBack, haptischem Eindruck oder subframegenauer Verdeckungslatenz. Die volle Story-Zuordnung und verbleibenden Reichweiten stehen in der Matrix.

---

Lesefassung mit dauerhaften relativen Links. Das [unveränderte eingefrorene Original](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/native/code11-native-audit.md) bleibt einschließlich früherer Formulierungen erhalten. Die aktuelle Updatebeschreibung unterscheidet den vorherigen Originalrestore vom eigentlichen Installationsversuch.
