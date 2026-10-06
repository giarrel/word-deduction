# Android v1: Abnahmematrix

Referenz: [kanonische Spezifikation](https://github.com/giarrel/word-deduction/issues/2), [lokale Lesekopie](../specs/android-release-v1.md). Diese Matrix trennt einen implementierten Mechanismus von seinem tatsächlich ausgeführten Nachweis. Stand bei Anlage: Der erste Implementierungszweig ist noch nicht integriert; keine Gesamtanforderung gilt allein aufgrund von Zwischenmeldungen als bestanden.

Statuswerte: **Not run**, **Passed**, **Failed**, **Blocked**, **Not applicable**. Bei Passed sind geprüfter Commit, Plattform, konkreter Bericht und Umfang zu verlinken. Ein Teilnachweis lässt den Gesamteintrag offen. Blocked beschreibt hier ein einzelnes Prüfkriterium, nicht automatisch das aktive Codex-Ziel.

## Gruppe und Persistenz

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| G01 | Namen hinzufügen, in derselben Ansicht weitere Namen eingeben, stabile IDs nach Neustart | Session-Test mit realer Ablage; tatsächlich bediente Gruppenansicht | Not run |
| G02 | Umbenennen, Pause/Aktivieren, Entfernen/Undo ohne Verlust anderer Namen oder Einstellungen | Öffentliche Tests mit Schließen/Öffnen; gerenderter Bedienweg | Not run |
| G03 | Doppelte Namen bleiben eindeutig, auch bei bereits eingegebenen Suffixen und nach Bearbeitung | Konkrete kollidierende Namensfixtures; sichtbare Anzeige | Not run |
| G04 | Unicode-Grenzen, Leer-/Steuerzeichen, 20 aktive und 40 gespeicherte Personen | Grenzfalltests; verständliche DE/EN-Rückmeldungen | Not run |
| G05 | Jede bestätigte Änderung übersteht Prozessende; laufende Partie wird nicht neu ausgelost | Dateitests jeder Spielphase und Android-Neustart | Not run |
| G06 | Abgebrochene/fehlgeschlagene Speicherung meldet keinen Erfolg und behält gültigen Zustand | Reproduzierbarer Schreibfehler und unterbrochener Schreibvorgang | Not run |
| G07 | Hauptdatei beschädigt, Sicherung beschädigt, neues unbekanntes Schema | Öffentliche Recovery-Szenarien; keine stillen Überschreibungen; UI-Nachweis | Not run |
| G08 | Folgepartie enthält alle aktiven Gruppenmitglieder einschließlich zuvor Eliminierter | Beide Modi vom Ergebnis bis zur nächsten Verteilung | Not run |

## Spielregeln

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| R01 | Quick 3–20 und Classic 4–20 mit exakter automatischer Rollenformel und White-Präferenz | Deterministische Fixtures aller Gruppengrößen und Regeln | Not run |
| R02 | Zuteilung/Startperson ohne feste Sitzrotation; Undercover erfährt seine Rolle nicht | Prüfung des Zufallswegs und öffentlicher Projektionen | Not run |
| R03 | Quick: richtige/falsche Beschuldigung, Stichwahl, wiederholter Gleichstand, Ergebnis | Öffentliche Tests plus vollständiger gerenderter Ablauf | Not run |
| R04 | Classic: fortlaufende Runden, alle Siege, keine erneute Auswahl Eliminierter | Öffentliche Tests und gerenderter Mehr-Runden-Ablauf | Not run |
| R05 | White: semantischer Tipp, richtig/falsch, Priorität des Tipps, Wiederaufnahme, kein Wortleck | Grenzfalltests und tatsächlich bediente White-Ansicht | Not run |
| R06 | Back, Hilfe, korrigierbare Vorauswahl, bestätigter Abbruch und Fortsetzen | Gerenderte Eingaben und Android-Zurück-Geste/-Taste | Not run |
| R07 | Ergebnis nennt Sieger/Grund/Wörter; direkte Folgepartie ohne Neueingabe | Beide Sprachen und Modi über UI gespielt | Not run |

## Wortkarte und Bedienung

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| U01 | Name vor Geheimnis; bewusstes Hochziehen; Text beim Loslassen sofort verdeckt | Eingabesequenz und betrachtete Frames der echten Karte | Not run |
| U02 | Haltealternative, kurzer Tipp, langsamer Drag, frühes Loslassen, außerhalb der Karte | Reale Pointer-Ereignisse; kein Screenshot-Only-Nachweis | Not run |
| U03 | Pointer-Abbruch/Capture-Verlust/zweiter Finger/Doppeltipp überspringen keine Person | Reproduzierbare UI-Szenarien; Besitzerwechsel bleibt verdeckt | Not run |
| U04 | Fokus/Pause/Resume/Prozessende verbergen Geheimnisse; Taskvorschau bleibt geschützt | Editor-Lifecycle plus installierte Android-App und Taskvorschau | Not run |
| U05 | Tastatur mit Umlauten, Einfügen, Rücktaste und Done; Fokusverlust verliert keinen Entwurf | Android-Namenseingabe; dokumentierte Tastatur/OS-Version | Not run |
| U06 | Lesbarkeit, Kontrast, Touchflächen und Systemleisten bei kleinen/hohen Displays und langen Texten | Tatsächliche Bilder, Messwerte und kritische Sichtprüfung | Not run |
| U07 | Konsistente runde Gestaltung, klare Hauptaktionen, angenehme Rückbewegung, reduzierte Bewegung | Betrachtete Bilder/Sequenzen und dokumentierte Korrekturschleife | Not run |
| U08 | Kein unnötiger Bildschirm zwischen Ergebnis und neuer Verteilung; Warmstart-Ziel geprüft | Gezählte Aktionen und Zeitmessung auf benannter Laufzeit | Not run |
| U09 | Barrierearme Alternativbedienung und keine automatisch offengelegten Geheimlabels | Gerenderte Semantik; TalkBack nur bei tatsächlich ausgeführtem Gerätetest behaupten | Not run |

## Inhalte und Sprachen

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| C01 | Mindestens 500 eigene bilinguale Paare, 700 verschiedene Wörter je Sprache, zwölf Themen | Gemessene Katalogstatistik und eigene Herkunft | Not run |
| C02 | Vertraute, verschiedene, sinnvoll verwandte Begriffe; keine Übersetzungs-/Synonym-/Duplikatfüllung | Datenvalidierung und dokumentierte redaktionelle Durchsicht | Not run |
| C03 | Vollständiger Ziehzyklus ohne Paarwiederholung, Neustart/Sprachwechsel, Zyklusübergang | Tests mit tatsächlichem Katalog und persistiertem Verlauf | Not run |
| C04 | Letzte zehn Wortverwendungen bevorzugt vermeiden; Auswahl terminiert bei Restbeständen | Passende knappe Restdeck-Fixtures und vollständiger Zyklus | Not run |
| C05 | UI, Fehler, Regeln und Zugänglichkeitslabels vollständig in DE/EN; Sprache gespeichert | Schlüsselprüfung plus jede reale Ansicht in beiden Sprachen | Not run |

## Android und Release

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| A01 | Erster Start und vollständige Spiele ohne Internet/Account/Download/Monetarisierung | Installierter Offline-Lauf; tatsächliches Manifest und Abhängigkeiten | Not run |
| A02 | APK/AAB ohne Development-Flags, ARM64/IL2CPP, min26/target36, reproduzierbar | Buildreport, Commit, genaue Befehle, Versions- und Hashdaten | Not run |
| A03 | APK-ZIP-Alignment, alle nativen ELF-Segmente und Bundle-Seitenausrichtung | Ergebnisse von zipalign, ELF-Prüfung und bundletool | Not run |
| A04 | Produktionssignierung und Update behalten vorhandene Gruppe/Partie | Signaturprüfung und Update einer bestehenden Installation | Not run |
| A05 | Keine Geheimnisse in Release-Logs/Backups/Taskvorschau; korrekte Berechtigungen | Paketprüfung und beobachtete Android-Laufzeit | Not run |
| A06 | Standards- und Spec-Review, erhebliche Befunde behoben | Zwei getrennte Reviewberichte gegen festgehaltene Basis und Retests | Not run |
| A07 | Icon, echte App-Bilder, DE/EN-Storetexte, Datenschutz-/Data-Safety-Grundlage | Sichtprüfung finaler Dateien und Abgleich mit tatsächlichem Releasepaket | Not run |
| A08 | Signierung, Publisherkontakt, Storekonto/Testvoraussetzungen abschließend geklärt | Konkreter Freigabeentscheid bzw. explizit benannte externe Voraussetzung | Not run |

## Grenzen der eigenen Prüfung

Automatisierte Abläufe, eigene UI-Bedienung und visuelle Kritik werden getrennt von Rückmeldungen einer realen Spielgruppe ausgewiesen. Physische Handhabung, Haptik und soziale Balance sind ohne echte Nutzung nicht bewiesen. Dies ist keine Erlaubnis, nicht getestete Kernfunktionen als fertig zu melden.

Zum Zeitpunkt der Anlage ist kein Android-Gerät von ADB erkannt. Ein Handy mit USB-Debugging wurde über die Chat-Rückfrage angefragt; die Implementierung bleibt davon unabhängig aktiv. Die Rückfrage selbst ist noch kein erteilter Gerätezugang.
