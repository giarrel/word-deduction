# Android v1: Abnahmematrix

Referenz: [kanonische Spezifikation](https://github.com/giarrel/word-deduction/issues/2), [lokale Lesekopie](../specs/android-release-v1.md). Diese Matrix trennt einen implementierten Mechanismus von seinem tatsächlich ausgeführten Nachweis. Stand nach Foundation-Integration: Implementer `44473f1`, Merge `318af96`, separate Integrationsprüfung `ac0dcca`. [Foundation-Nachweise](group-foundation.md), [Android-Bedienung](android-group-runtime/report.md) und [Merger-Prüfung](group-foundation-merge.md). Keine Gesamtanforderung gilt allein aufgrund eines Teilnachweises als bestanden.

Statuswerte: **Not run**, **Passed**, **Failed**, **Blocked**, **Not applicable**. Bei Passed sind geprüfter Commit, Plattform, konkreter Bericht und Umfang zu verlinken. Ein Teilnachweis lässt den Gesamteintrag offen. Blocked beschreibt hier ein einzelnes Prüfkriterium, nicht automatisch das aktive Codex-Ziel.

## Gruppe und Persistenz

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| G01 | Namen hinzufügen, in derselben Ansicht weitere Namen eingeben, stabile IDs nach Neustart | Foundation: echte Dateien, Unity-UI und Android API36 mit ARM64-Übersetzung; mehrere APK-Updates und Force-stop | Passed |
| G02 | Umbenennen, Pause/Aktivieren, Entfernen/Undo ohne Verlust anderer Namen oder Einstellungen | Foundation: 13 Session-Fälle und Android-Iterationen 2–5; sieben Personen, DE/Classic, ursprüngliche IDs und Undo geprüft | Passed |
| G03 | Doppelte Namen bleiben eindeutig, auch bei bereits eingegebenen Suffixen und nach Bearbeitung | Konkrete kollidierende Namensfixtures; sichtbare Anzeige | Not run |
| G04 | Unicode-Grenzen, Leer-/Steuerzeichen, 20 aktive und 40 gespeicherte Personen | Session-Grenzfälle bestanden; [Android-Schriftprobe](group-layout-probes/report.md) zeigt akzeptierte CJK-/arabische/indische Namen und Emoji leer. Darstellung vor Release korrigieren | Failed |
| G05 | Jede bestätigte Änderung übersteht Prozessende; laufende Partie wird nicht neu ausgelost | Recovery:46 Session-Szenarien plus Classic-Dateimatrix jeder Phase; [AndroidV3→V4 und Neustart](android-recovery-combined/report.md) behalten exakte bestätigte Dateien | Passed |
| G06 | Abgebrochene/fehlgeschlagene Speicherung meldet keinen Erfolg und behält gültigen Zustand | Echte Windows-Replace-Sperren und unterbrochene Dateien; [native White-Urteils-Schreibsperre](android-recovery-combined/report.md) bewahrt Datei bytegenau und bestätigt erst nach Retry | Passed |
| G07 | Hauptdatei beschädigt, Sicherung beschädigt, neues unbekanntes Schema | [Recovery](recovery.md): beide Dateigenerationen, ReadFailed, Zukunftsschema und explizite Archive/Reset-Antworten in echten Dateien und gerendertem UI | Passed |
| G08 | Folgepartie enthält alle aktiven Gruppenmitglieder einschließlich zuvor Eliminierter | Quick und Classic: direkte Folgepartien in [Quick](android-quick-runtime/report.md) und [Classic](android-classic-runtime/report.md), einschließlich vorheriger Eliminierter | Passed |

## Spielregeln

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| R01 | Quick 3–20 und Classic 4–20 mit exakter automatischer Rollenformel und White-Präferenz | Classic9187444/Integration256678d: 29 Session-Szenarien für alle 4–20-Gruppen, Quick unverändert; Android 8 sowie 4↔5 mit gespeicherter White-Präferenz | Passed |
| R02 | Zuteilung/Startperson ohne feste Sitzrotation; Undercover erfährt seine Rolle nicht | Öffentliche Session-Fixtures für alle Rollen/Startpersonen und Zufallspfad; [native Classic-Karten](android-classic-runtime/report.md) zeigen Undercover nur das Wort | Passed |
| R03 | Quick: richtige/falsche Beschuldigung, Stichwahl, wiederholter Gleichstand, Ergebnis | Implementer74e7825, Integration496d446/013087f: 21 Session-/10 PlayMode-Fälle und drei [tatsächliche Android-Partien](android-quick-runtime/report.md), DE/EN | Passed |
| R04 | Classic: fortlaufende Runden, alle Siege, keine erneute Auswahl Eliminierter | 29 Session-Szenarien, 14 gerenderte Tests, [drei tatsächliche Android-Classic-Partien](android-classic-runtime/report.md) mit fortlaufenden Runden und ausgeschlossenen Eliminierten | Passed |
| R05 | White: semantischer Tipp, richtig/falsch, Priorität des Tipps, Wiederaufnahme, kein Wortleck | White korrekt/falsch und Priorität: Session/Editor plus [Android-Neustart im Rateversuch und White als letzter Gegner](android-classic-runtime/report.md), Zielwort erst im Ergebnis | Passed |
| R06 | Back, Hilfe, korrigierbare Vorauswahl, bestätigter Abbruch und Fortsetzen | Native Randgeste korrigiert; [Kaltstart-Gegencheck](android-recovery-combined/report.md) zeigt zeitweise ignorierte Abbruchaktion, Ticket7 untersucht | Failed |
| R07 | Ergebnis nennt Sieger/Grund/Wörter; direkte Folgepartie ohne Neueingabe | Beide Modi in DE/EN nativ gespielt; [Quick](android-quick-runtime/report.md) und [Classic](android-classic-runtime/report.md) mit Wörtern/Rollen und einer Aktion zur Folgepartie | Passed |

## Wortkarte und Bedienung

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| U01 | Name vor Geheimnis; bewusstes Hochziehen; Text beim Loslassen sofort verdeckt | Quick74e7825: synchroner UITK-Test vor nächstem Frame; [native Android-Geste und betrachtete Bilder](android-quick-runtime/report.md) mit benanntem Besitzer und verborgenem Wort | Passed |
| U02 | Haltealternative, kurzer Tipp, langsamer Drag, frühes Loslassen, außerhalb der Karte | Reale Pointer-Ereignisse; kein Screenshot-Only-Nachweis | Not run |
| U03 | Pointer-Abbruch/Capture-Verlust/zweiter Finger/Doppeltipp überspringen keine Person | Recovery89b7b386: gerenderte Multi-Kontakt-/Navigationstests plus [nativer Reentry-Gegencheck und gewöhnliches Next](android-recovery-combined/report.md); keine vorzeitige Freigabe oder übersprungene Karte | Passed |
| U04 | Fokus/Pause/Resume/Prozessende verbergen Geheimnisse; Taskvorschau bleibt geschützt | Editor-Lifecycle plus installierte Android-App und Taskvorschau | Not run |
| U05 | Tastatur mit Umlauten, Einfügen, Rücktaste und Done; Fokusverlust verliert keinen Entwurf | Android-Namenseingabe; dokumentierte Tastatur/OS-Version | Not run |
| U06 | Lesbarkeit, Kontrast, Touchflächen und Systemleisten bei kleinen/hohen Displays und langen Texten | [Kleine Android-Ansicht und Schriftprobe](group-layout-probes/report.md): 48 Unity-Referenzeinheiten ergeben bei 360 dp Breite nur ca. 44 dp; mehrere Schriftsysteme bleiben leer. Anpassung und Nachtest in Ticket 8 | Failed |
| U07 | Konsistente runde Gestaltung, klare Hauptaktionen, angenehme Rückbewegung, reduzierte Bewegung | Betrachtete Bilder/Sequenzen und dokumentierte Korrekturschleife | Not run |
| U08 | Kein unnötiger Bildschirm zwischen Ergebnis und neuer Verteilung; Warmstart-Ziel geprüft | Gezählte Aktionen und Zeitmessung auf benannter Laufzeit | Not run |
| U09 | Barrierearme Alternativbedienung und keine automatisch offengelegten Geheimlabels | Gerenderte Semantik; TalkBack nur bei tatsächlich ausgeführtem Gerätetest behaupten | Not run |

## Inhalte und Sprachen

| ID | Anforderung / Abnahmeumfang | Erforderliche Evidenz | Status |
|---|---|---|---|
| C01 | Mindestens 500 eigene bilinguale Paare, 700 verschiedene Wörter je Sprache, zwölf Themen | [Content](bilingual-content.md):520 eigene bilinguale Paare,1038 normalisierte Begriffe je Sprache,20 Themen; exakter kompilierter Katalog | Passed |
| C02 | Vertraute, verschiedene, sinnvoll verwandte Begriffe; keine Übersetzungs-/Synonym-/Duplikatfüllung | [Redaktion und Katalogprüfung](bilingual-content.md): alle Paare durchgesehen, keine doppelten IDs/umgekehrten Paare/fehlenden Übersetzungen; regionale menschliche Review-Kandidaten dokumentiert | Passed |
| C03 | Vollständiger Ziehzyklus ohne Paarwiederholung, Neustart/Sprachwechsel, Zyklusübergang | 1040 reale persistierte Ziehungen mit Sprachwechsel/Neustart plus [native Migration und DE→EN-Partien](android-recovery-combined/report.md) | Passed |
| C04 | Letzte zehn Wortverwendungen bevorzugt vermeiden; Auswahl terminiert bei Restbeständen | Öffentliche Session-Tests mit tatsächlichem Katalog und absichtlich knappem Restbestand; voller Zwei-Zyklen-Test in [Content-Integration](bilingual-content-merge.md) | Passed |
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

Der [eigene Android-16-Emulator](../development/android-test-device.md) hat die installierte Gruppenbasis tatsächlich geprüft. Seine Host-GPU rendert lesbaren Text; die ARM64-App läuft über native Übersetzung auf x86_64. Ein physisches Handy mit USB-Debugging wurde über die Chat-Rückfrage angefragt; die Rückfrage selbst ist noch kein erteilter Gerätezugang. Doppelte Namen, Kapazität, Namenvalidierung und Recovery besitzen bereits Foundation-Teilnachweise; die jeweiligen breiteren Kriterien bleiben offen, bis ihre vollständige Darstellung beziehungsweise der laufende Matchzustand geprüft sind.
