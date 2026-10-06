# Classic: tatsächliche Android-Abnahme

6. Oktober 2026. Root spielte die installierte App über Android-Eingaben. Akzeptierte APK: SHA-256 `abc6e923f0fc7ad124d8984475923c402a9cadb22b3bb9c4c5e1bb92ff99ef8c`, 37.583.470 Bytes, gebaut aus `9c05948`, produktionsgleicher `game/`-Baum zu Implementer `9187444` und Integration `256678d`. [Separate Integrationsprüfung](../classic-mode-merge.md), [Session-/Editor-Nachweise](../classic-mode.md).

Laufzeit: eigener Android-16/API-36-Emulator `word_deduction_api36`, Seriennummer `emulator-5580`, Google-APIs-x86_64 Revision 7 mit ARM64-Übersetzung, Host-GPU, 1080×2400 bei 420 dpi. Development-APK, kein physisches Telefon. Kein Wipe, keine Deinstallation, keine Veränderung der gespeicherten Daten außerhalb der App. `run-as` las die Testzustände nur als unabhängige Prüforakel; alle Spielübergänge erfolgten durch die tatsächliche Oberfläche.

## Ausgeführte Abläufe

| Ablauf | Beobachtetes Ergebnis | Nachweis |
|---|---|---|
| Update über vorherige Quick-APK | Vollständige V2-Datei bytegleich; dieselbe laufende Partie `4bdcfa20c0e44e0da971dcb55d063d30`, acht IDs, Wörter und Kartenbesitzer. App öffnet geschützte Pause. Erst die bewusste spätere Aktion schreibt V3. | `before-classic-update.json`, `after-classic-update.json`, `01-update-preserved.png` |
| Start mit acht Personen | Classic zunächst 6 Bürger/2 Undercover/0 White; White einschalten ergibt 5/2/1. Name erscheint vor Geheimnis, Next zunächst deaktiviert. | Bilder 03–05, `match1-deal.json` |
| Englische Partie 1 | Alle acht Karten nativ gehalten, losgelassen, weitergegeben; Wortkarten nennen keine Undercover-Rolle. White sieht nur die Bluff-Anweisung. Erster Gleichstand öffnet Stichwahl; zweiter startet Hinweisrunde 2 ohne Eliminierung. | `match1-native-flow.txt`, `match1-contacts.txt`, `match1-owner-*.png`, Bilder 06–09 |
| White richtig und Neustart | Zoë ausgewählt und bestätigt; vor Rateversuch kein Zielwort. Force-stop/neuer Prozess erhält den gesamten Snapshot bytegleich, kehrt über sichere Pause zum identischen Rateversuch zurück. Richtig führt ausschließlich zu White-Sieg mit beiden Wörtern. | Bilder 10–14, `match1-white-before-restart.json`, `match1-white-after-restart.json`, `match1-result.json` |
| Direkte Folgepartie | Ein Next-match-Tipp startet neue Verteilung mit allen acht aktiven Personen, neue Partie/Zuordnung, ohne Gruppe neu einzugeben. | `15-one-action-rematch.png`, `match2-after-deal.json` |
| Englische Partie 2 über drei Abstimmungen | Alexandra und Bea · 4 als Undercover eliminiert; nur Rolle, kein Wort auf Eliminierungsansicht. Force-stop nach erster Eliminierung erhält Snapshot bytegleich. Danach fehlen eliminierte Personen in Abstimmung; nur Überlebende spielen weiter. | Bilder 16–20, `match2-elimination1.json`, `match2-elimination1-restarted.json` |
| White als letzter Gegner | Nachdem beide Undercover ausgeschieden sind, erhält Felix als letzter Gegner trotzdem zuerst seinen Rateversuch. Falsch führt erst danach zum Bürgersieg. Beide Wörter und alle acht Rollen sind durch Scrollen lesbar. | Bilder 21–23, `match2-last-white-pending.json`, `match2-result.json` |
| Flexible Gruppe und White-Grenze | Nach Sprachwechsel DE drei Personen pausiert; fünf Aktive ergeben 3/1/1. Vierte Person pausiert: vier Aktive, 3/1/0, sichtbare gespeicherte White-Präferenz an, aber deaktiviert. Reaktivieren stellt White für die neue Partie wieder bereit. Alle acht IDs bleiben gespeichert. | Bilder 24–26, `five-active.json`, `four-active.json`, `match3-after-deal.json` |
| Deutsche Partie mit fünf Personen | Alle fünf Karten nativ bedient. Nach Eliminierung von Bea · 4 und Emil bleibt ein Bürger neben Undercover und White: Ergebnis nennt beide Gegnerrollen korrekt. | `match3-native-flow.txt`, `match3-contacts.txt`, `match3-owner-*.png`, Bilder 27–30 |
| Eliminierung endet mit der Partie | Ein Tipp auf Nächste Partie nimmt wieder alle fünf aktiven Personen auf, darunter beide zuvor Eliminierte. Drei bewusst pausierte Personen bleiben pausiert. | `31-rematch-restores-eliminated.png`, `match4-rematch.json`: 5 Teilnehmer, 0 eliminiert, 5 aktiv, 8 gespeichert |

Partie 1: `93a5701ceb9747bab018253713aa47ef`, Pencil/Ballpoint pen, White-Sieg nach Runde 2. Partie 2: `1103bb65e2f24c8cb0e1adc3d05afe9f`, Quiche/Pizza, Bürgersieg nach drei Eliminierungen. Partie 3: `d393d5caac184343bf5343142a2cd921`, Teller/Schüssel, gemeinsamer Gegnerrollensieg nach zwei Eliminierungen. Die Wörter stammen hier noch aus dem kleinen bisherigen Katalog; Ticket 6 wird separat kombiniert abgenommen.

## Eingabe und Sichtprüfung

`Device.ps1` kapselt echte ADB-Touchswipes, Start/Force-stop, lesendes Snapshot-Abrufen und Emulator-Hostaufnahmen. `Deal.ps1` verwendet denselben externen Java-Shell-Helfer wie [Quick](../android-quick-runtime/WordDeductionInput.java): echte `MotionEvent`-Berührungen mit Quelle Touchscreen und Werkzeug Finger. Jede Karte wird gehalten, losgelassen und per nativer Next-Berührung weitergegeben; nach jedem Besitzerwechsel wird der tatsächlich geschriebene Fortschritt geprüft. `finally` beendet alle Kontakte. Keine direkt gesetzten Bildschirmfixtures oder Session-Kommandos ersetzen diese Abläufe.

Alle nummerierten Bilder 01–31 wurden geöffnet und visuell geprüft. Zusätzlich betrachtet: englische Bürger-, Undercover- und White-Karten sowie deutsche White-/Undercover-Karte und Hinweisrunden. Weitere Kartenbilder dokumentieren denselben Eingabelauf; ihre bloße Existenz ist keine zusätzliche visuelle Abnahme. Hostaufnahmen können die eigene Test-App trotz `FLAG_SECURE` erfassen; sie sind keine Android-App-Aufnahme und kein Nachweis eines Privatsphäredefekts.

Beim ersten Start des Deal-Skripts war ADB nach dem Unity-Editorabschluss kurz nicht verbunden. Der Versuch brach vor Spielaktionen ab; nach erneuter Geräteerkennung lief der Test erfolgreich. Der Emulator wurde nicht neu gestartet. Es wurde kein fehlgeschlagener Versuch als bestandener Spieldurchlauf gewertet.

## Ergebnis und verbleibende Arbeit

Ticket 5 ist fachlich und nativ abgenommen, zusammen mit 29 öffentlichen Session-Szenarien, 14 gerenderten Tests und separater Integration. Sämtliche Rollenanzahlen und weiteren Gewinnerkombinationen wurden am Session-Interface geprüft; dieses Android-Protokoll behauptet nur die oben tatsächlich durchgespielten Fälle.

Die bereits reproduzierten Mehrfinger-Reentry- und Android-Randgestenfehler werden weiterhin in Ticket 7 korrigiert; diese Classic-APK enthält die Korrekturen noch nicht. Font-/Touchgrößen, schwere Scrollleisten, Kartenbalance und allgemein verständlichere Regeltexte bleiben Ticket 8. Die Formulierung „Incorrect · continue“ ist bei White als letztem Gegner missverständlich, weil die Partie danach endet; im Feinschliff neutral oder ergebnisabhängig formulieren. Ebenso „lebende Gegenspielerrollen“ durch natürliche Spielersprache ersetzen. Kein neuer Regel- oder Datenverlustbefund in diesen drei Durchläufen.

Die vierte Partie bleibt bewusst auf verdeckter erster Karte stehen, damit die folgende kombinierte V4-APK ein echtes laufendes V3-Spiel migrieren muss. Alle Testkontakte sind beendet. Keine Aussage zu physischer Handhabung, Haptik oder Spaß einer realen Gruppe.
