# Nutzerfeedback: Gruppe, Scrollen und Rollenanzahlen

Stand: 9. Oktober 2026, Europe/Zurich. Auftrag: zunächst untersuchen, Feedback dauerhaft festhalten und berichten. Keine Umsetzung und kein neuer Build in diesem Durchgang.

## Bestätigte Wünsche

- Den **Pause-/Mitmachen-Button pro Spieler entfernen**. Für die gewünschte einfache Gruppenverwaltung reicht Entfernen; versehentliches Entfernen soll weiterhin rückgängig gemacht werden können.
- An dieser Stelle einen ausdrücklichen **Bearbeiten-Button** anbieten. Umbenennen und Entfernen bleiben darüber erreichbar. Die große Namensfläche soll beim Scrollen keine Bearbeitung öffnen.
- Die **Spielerreihenfolge frei ändern** können. Das Wort „immer“ nicht stillschweigend auf eine engere Situation reduzieren: Für eine spätere Umsetzung ist insbesondere die Abgrenzung zwischen Gruppenreihenfolge und bereits laufender/vergebener Partie zu berücksichtigen. Keine stillen Neuvergaben oder verlorenen Übergaben.
- Rollenanzahlen in Schnell und Klassisch frei innerhalb sinnvoller gemeinsamer Grenzen wählen können; die reine Mr.-White-An/Aus-Option reicht nicht.
- Verständlich anzeigen, weshalb eine Rollenanzahl bei kleiner Gruppe nicht veränderbar ist.
- Nach Rückfrage bestätigt: Mit „Spielmodus nicht ändern“ waren **die Rollenanzahlen** gemeint. Kein zusätzlicher Fehler der drei Modus-Schaltflächen gemeldet.

Die Pause-Aussage bezieht sich auf die Teilnahmebuttons in der Gruppe. Eine Abschaffung des Schutzes beim Hintergrundwechsel oder der sicheren Wiederaufnahme einer Partie wurde hier nicht verlangt.

## Untersuchte Version

- Repository: `integration/kings-v1`, sauberer Ausgangsstand `53951d9f1d6cc0f9014db7615cfa7a159bb804d9`.
- Installierte APK: **1.1.0 / code 8**, im vorhandenen isolierten AVD `word_deduction_api36_16k`, Android 16/API 36, 1080×1920, normale Schrift. Keine Installation, Datenrücksetzung oder neue Partie für diese Untersuchung.
- Vorhandene fünf Testpersonen; Könige ausgewählt. Der Bericht des Nutzers betrifft fünf bis sechs Personen. Das konkrete physische Gerät des Nutzers wurde nicht untersucht.
- [Ausgangsansicht](assets/2026-10-09/group-before.png).

## Rollenregler: im aktuellen Code erklärbar

| Modus | Aktuelles Verhalten | Einordnung |
| --- | --- | --- |
| Schnell | Genau ein Undercover, kein Mr. White, keine Anzahlregler. | Freie Verteilung ist noch nicht implementiert. |
| Klassisch | Undercover automatisch: 1 bei 4–7, 2 bei 8–12, 3 bei 13–20 aktiven Personen. Mr. White ab 5 Personen optional genau einmal. | Deshalb ist nur White an/aus sichtbar. |
| Könige | Genau ein Mr. White als böser König; gewöhnliche Undercover manuell einstellbar, mindestens einer. | Die gemeinsame Anfangsmehrheit begrenzt die Auswahl. |

Die Grenze für gewöhnliche Undercover in Könige ist `floor((aktive Personen - 1) / 2) - 1`. Der zusätzliche Mr. White zählt bereits zu den Bösen; der gute König zählt zu den Guten.

| Aktive Personen | Gewöhnliche Undercover in Könige | Mr. White | Konsequenz |
| ---: | --- | ---: | --- |
| 5–6 | genau 1 | 1 | Plus und Minus beide deaktiviert. |
| 7–8 | 1–2 | 1 | Erste tatsächlich veränderbare Spanne. |
| 9–10 | 1–3 | 1 | Größere Spanne. |

Bei sechs Personen wären zwei Undercover plus White bereits drei Böse gegen drei Gute. Die vereinbarte strikte Anfangsmehrheit wäre verletzt. Die Sperre entspricht somit der Regel, wird aber derzeit nicht ausreichend erklärt. „Auto“ ist ebenfalls deaktiviert, wenn bereits die automatische Verteilung aktiv ist; es setzt eine vorherige manuelle Wahl zurück.

Quellen: [RoleCounts in Match.cs](../../game/Assets/WordDeduction/Session/Match.cs), [SessionView und Einstellungen](../../game/Assets/WordDeduction/Session/Session.cs), [Regler und Spielerzeilen in GroupScreen.cs](../../game/Assets/WordDeduction/UI/GroupScreen.cs).

Die freien Grundmodus-Anzahlen sind weiterhin als bestätigte Anforderung in [Issue #11](https://github.com/giarrel/word-deduction/issues/11) erfasst. Das Ticket ist offen (`needs-info`); Quick mit mehreren Gegenspielern, White im Quickmodus, mehrere Whites in Klassisch und Kombinationen ohne Undercover benötigen noch festgelegte Ergebnisregeln. Die abgeschlossene Könige-Umsetzung hat die gemeinsame Startgrenze umgesetzt, diese Grundmodus-Funktion aber ausdrücklich ausgeklammert. Das neue Feedback bestätigt deren praktische Bedeutung; es beantwortet die offenen Ergebnisregeln noch nicht.

## Scrollen öffnet Bearbeiten: begrenzt reproduziert

Die Namensfläche ist ein vollwertiger `Button` (`PlayerRow`, `GroupScreen.cs`, Zeile 264), dessen Klick sofort `editingId` setzt und neu rendert. Die Bearbeitung fokussiert anschließend das Textfeld und öffnet die Android-Tastatur. Daneben steht ein zweiter Button für Pause/Mitmachen. Eine Änderung der Spielerreihenfolge ist weder als Gruppenaktion noch als Bedienung vorhanden; die Liste folgt der gespeicherten Reihenfolge.

Tatsächlich ausgeführte native Touchaktionen auf der vorhandenen APK:

1. Über Lucas Namensfläche von `(500,790)` nach `(500,730)`, 300 ms: Liste scrollt, Namensbutton erhält Fokus; noch keine Bearbeitung im beobachteten Folgezustand.
2. Über derselben Namensfläche von `(500,740)` nach `(500,728)`, 250 ms: Nach Abwarten der Oberfläche öffnet sich **Bearbeiten mit Tastatur**.
3. Bearbeitung ohne Namensänderung abgebrochen.
4. Erneut über Lucas nun anders positionierter Namensfläche von `(500,680)` nach `(500,668)`, 250 ms: **Bearbeiten mit Tastatur** erneut geöffnet, [beobachteter Zustand](assets/2026-10-09/short-swipe-opens-edit.png).
5. Wieder abgebrochen. Es wurden keine Spieler oder Einstellungen verändert und keine Partie gestartet.

Die 12-Pixel-Bewegungen entsprechen bei der Emulatordichte ungefähr 4 dp. Damit ist ein kleiner Scrollversuch, der noch als Button-Klick behandelt wird, zweimal gezeigt; nicht behauptet wird, dass jeder größere Wischzug fehlschlägt oder dieselbe Gestentoleranz auf dem physischen Gerät nachgewiesen wäre. Ein separater Bearbeiten-Button und eine nicht klickbare Namensfläche adressieren genau diesen Konflikt. Der Fix wurde noch nicht implementiert oder auf Wirksamkeit geprüft.

Die bestehenden Gruppentests prüfen explizites Bearbeiten sowie automatisches Sichtbarscrollen hinzugefügter/bearbeiteter Zeilen. Sie bilden diesen kleinen Touch-Scrollversuch über einem Namensbutton bislang nicht ab. Vor einer Umsetzung gehört eine Prüfung an der tatsächlichen Gestenoberfläche dazu; bloßes Auslösen des Button-Callbacks reicht dafür nicht.

## Ergänztes Nutzerfeedback: ruckelige Bewegungen

Der Nutzer meldet zusätzlich am 9. Oktober 2026: **Scrollen und Aufdeckanimationen sind ruckelig.** Das ist ein eigener Befund zur wahrgenommenen Flüssigkeit, zusätzlich zum versehentlichen Öffnen der Bearbeitung beim Scrollen.

- Gewünscht sind flüssiges Scrollen und eine Karte, die beim Hochziehen gleichmäßig auf den Finger reagiert. Auch der bereits gewünschte Rücklauf beim Loslassen gehört zur späteren Bewegungsprüfung.
- Status: Nutzerbeobachtung festgehalten; noch keine gezielte Reproduktion, Framezeitmessung oder bestätigte technische Ursache. Die bisherigen Screenshots und funktionalen Tests belegen keine flüssigen Animationen auf dem benutzten Handy.
- Bei der späteren Untersuchung Scrollen, fingergeführtes Aufdecken und Rücklauf getrennt messen, auf wiederkehrende Framezeitspitzen und verzögerte Eingabereaktion achten und den Ausgangszustand vor Änderungen festhalten. Gerät und Bildwiederholrate sind noch nicht bekannt. Ursachen wie Layoutarbeit oder Bildratenbegrenzung erst anhand von Messungen bewerten.
- Die Bewegung darf weicher werden, während geheime Inhalte beim Loslassen oder Unterbrechen weiterhin sofort verborgen werden. Eine visuelle Rücklaufanimation darf das Wort nicht länger lesbar lassen.

## Folgerungen für die spätere Umsetzung

- Scrollen, Bearbeiten und Reihenfolge ändern müssen eindeutig getrennte Gesten/Ziele erhalten. Ein Verschiebegriff ist eine mögliche Gestaltung, noch keine endgültig gewählte Lösung.
- Vorhandene Namen, stabile IDs, Undo und Reihenfolge müssen erhalten bleiben. Vor dem Entfernen des Teilnahme-Konzepts alte pausierte Personen und Gruppen mit mehr als 20 gespeicherten Personen berücksichtigen; nicht automatisch heimlich aktivieren oder löschen.
- Bei Könige mit 5–6 Personen eine kurze Erklärung der festen Verteilung zeigen, statt nur ausgegraute Regler anzubieten. Die Mehrheitsregel nicht stillschweigend lockern.
- Rollenregler der Grundmodi zusammen mit den offenen Regeln aus #11 bearbeiten. Kein scheinbar funktionierender Regler vor konsistenten Siegbedingungen.
- Diese Notiz dokumentiert Feedback und Untersuchung. Produktionsquellcode, Szenen, Einstellungen und APK bleiben unverändert.

Die vollständigen unveränderten Screenshots und der Capture-Helfer dieser Untersuchung liegen zusätzlich im Chat-Arbeitsverzeichnis `work/feedback-2026-10-09/`. Die Untersuchung verwendete den Skill `unity-workbench:unity-bug-investigation`; Fix- und Regressionstestphasen wurden entsprechend dem ausdrücklich auf Untersuchung begrenzten Auftrag nicht gestartet.
