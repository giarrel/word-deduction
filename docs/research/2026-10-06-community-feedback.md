# Community-Feedback für ein Android-Wortdeduktionsspiel

Stand: 6. Oktober 2026. Recherche für `giarrel/word-deduction`; Produktrecherche, keine Implementierung und keine abschließende Regelspezifikation.

## Ergebnis

Die Quellen liefern konkrete Beispiele dafür, dass der gemeinsame Spielspaß mit wenig Bedienaufwand funktioniert und dass Unterbrechungen, wiederkehrende Wörter und missverständliche Einstellungen das Erlebnis schwächen. Besonders passend zur Projektidee ist ein Bericht über ausgeschiedene Spieler, die bei der nächsten Partie nicht wieder einsteigen konnten. Dieser Fall betrifft allerdings einen **Online-Raum**, nicht die lokale Teilnehmerliste. [E01–E18 unten]

**Empfehlung als Synthese:** Zuerst einen zuverlässigen lokalen Spielabend auf einem Android-Gerät gestalten: Gruppe behalten, nächste Partie schnell starten, Wörter sicher übergeben, brauchbaren Grundwortschatz mit Wiederholungsvermeidung mitliefern. Der Wunsch nach exakt diesem Verwaltungsablauf stammt vorrangig vom Auftraggeber; die Recherche macht ihn plausibel, beweist aber keine Marktmehrheit.

## Nutzerprioritäten, unabhängig von externen Stimmen

Im Gespräch vom 6. Oktober 2026 festgelegt beziehungsweise bestätigt:

- Android zuerst; Unity ist installiert, die technische Wahl ist noch offen.
- Zwischen Partien Personen einfach hinzufügen, entfernen, umbenennen und vorübergehend aussetzen lassen, ohne die ganze Gruppe und Einstellungen neu aufzubauen.
- Einfache, schnörkellose Bedienung. **Spielername sichtbar; geheimes Wort durch Hochziehen der Karte aufdecken.** Diese Geste wurde nach Rückfrage bestätigt.
- Zwei Abläufe sind interessant: ein Beschreibungsdurchgang mit anschließender Entscheidung für wechselnde Gruppen; mehrere Durchgänge mit Eliminierung für längere Partien.
- Mit und ohne Mr. White wurde bereits gespielt; die genaue Kombination der Rollen pro Modus bleibt zu spezifizieren.
- Wortpakete, Shop und Importoberfläche haben zunächst geringe Priorität; ein sinnvoller Startbestand reicht.

## Methode und Grenzen

Relevanzbasierte, nicht repräsentative Stichprobe aus **18 einzelnen Spielerbeiträgen**: neun Android-Rezensionen auf Google Play, sieben iOS-Rezensionen im App Store und zwei Reddit-Kommentare. Sie verteilen sich auf vier eindeutig identifizierte Apps. Die beiden Reddit-Kommentare gehören zu einem vom Entwickler eröffneten Thread; die zitierten Erfahrungen stammen von anderen Kommentierenden. Entwicklerantworten und Produktbeschreibungen werden separat gekennzeichnet.

Gesucht wurde unter anderem nach App-Namen zusammen mit `reviews`, `names`, `rename`, `add players`, `reset`, `swipe`, `offline` und `crash`. Ausgewählt wurden zugängliche Originalbeiträge mit Bezug zu den Projektfragen. Suchmaschinen und Store-Seiten zeigen nur einen Ausschnitt und sortieren nicht zufällig. Doppelt ausgegebene Rezensionstexte wurden nur einmal gezählt. Manche konkreten Suchanfragen lieferten ausschließlich unpassende Spiele oder Aggregatoren.

Keine App wurde selbst installiert oder praktisch getestet. Rezensionen sind Belege für **berichtete Erfahrungen**, nicht für reproduzierte Fehler oder den aktuellen Zustand jeder Version. iOS-Erfahrungen sind Hinweise auf das Spielkonzept, kein Nachweis gleichen Android-Verhaltens. Sterne, Downloadzahlen und „hilfreich“-Zähler wurden nicht als Häufigkeitsmessung verwendet. Historische Beschwerden können längst behoben sein. Die Store-Links führen zur jeweiligen Rezensionenliste; Autor und Datum ermöglichen das Wiederfinden, solange der Store den Beitrag ausliefert.

Evidenzkennzeichnung:

- **Direkt:** Originalbeitrag mit erkennbarem Autor und Datum; solide für die einzelne geschilderte Erfahrung, schwach für Verallgemeinerungen.
- **Direkt, eingeschränkt:** Originalbeitrag vorhanden, aber Jahr oder Geräteplattform nicht sichtbar.
- **Hersteller:** Regel, Feature oder Antwort des Anbieters; kein unabhängiges Nutzerurteil.

## Welche Apps gemeint sind

| Kürzel | Identität | Plattform und Zuordnung |
|---|---|---|
| A | **Undercover® / Undercover™: Word Party Game**, Yanstar Studio OU | Android `com.yanstarstudio.joss.undercover`; iOS `946882449`. [Google Play][s1], [App Store][s2] |
| B | **Imposter Game – Party Edition**, Sven Vucak | Android `at.vucak.impostor`; iOS `6745120053`. [Google Play][s4], [App Store][s5] |
| C | **Secret Spy – Word Party Game**, Game of Shots – Partyverse | Android `com.DefaultCompany.Impostor`. Der beim Abruf sichtbare Originaltitel lautet Secret Spy; manche Suchtreffer benutzen noch „Find the Imposter“. Entscheidend ist hier die Paket-ID. [Google Play][s6] |
| D | **Imposter Game – Party Edition**, Annalyst / ANNA LYST GmbH | Android `com.bluffcircle.party`. Trotz gleichem Store-Titel **eine andere App als B**. [Google Play][s7] |

Zusätzlich geprüft: **Mr White**, Lucky Kadam, Android `io.kyara.undercover`. Der Store nennt eigene Wörter und eine Variante nur mit Mr. White ohne Undercover, lieferte aber keine auswertbaren Nutzerrezensionen. Deshalb kein Community-Beleg und nicht mit Yanstar gleichgesetzt. [Herstellerangaben im Google Play Store][s8]

## Konkrete Spielerbeiträge

### Bedienung, Gruppen und Spielfluss

| ID | App / Quelle / Datum | Beobachtung, sinngemäß | Evidenz und Reichweite |
|---|---|---|---|
| E01 | A, David Andres Bernal Navarrete, Android, 30.08.2026, [Play Store][s1] | Lob für gute Wörter, leichte Bedienung und Abwechslung durch Rollen. | Direkt; positives Gesamterlebnis. |
| E02 | A, Jayjay Noble, Android, 11.09.2026, [Play Store][s1] | 50 kostenlose Wörter seien zu wenig; beim Online-Versuch lande die App wieder offline. | Direkt; Inhalts- und Funktionsbeschwerde, nicht reproduziert. |
| E03 | A, mohamad ali, Android, 13.11.2024, [Play Store][s1] | Hosts würden Personen auch während des Spiels entfernen; neue öffentliche Gruppen seien schwer zu füllen. | Direkt; öffentliche Online-Lobbys, keine lokale Teilnehmerbearbeitung. |
| E04 | A, Dragon_Lord14, iOS, 11.01.2024, [App Store][s2] | Offline-Spiel mit Freunden im Bus gefällt; der Wortvorrat wirke erschöpft. | Direkt; Anbieter stellt in seiner Antwort vom 20.01.2024 klar, dass Wörter wiederholt werden können. Kein belegter vollständiger Spielstopp. |
| E05 | A, Panda.001, iOS, 19.04.2025, [App Store][s2] | Positives Spielurteil, aber Werbung zwischen Online-Partien fällt auf. | Direkt; betrifft diesen Online-Ablauf. |
| E06 | A, Sarah Wiersema, iOS, „Jan 1“, [App Store][s2] | Kritik an Namensänderung nur alle 30 Tage. | Direkt, eingeschränkt: Jahr fehlt; Entwicklerantwort verweist auf 2026 und Schutz vor Identitätsmissbrauch. Daher Online-Profilproblem, kein Nachweis einer lokalen Namenssperre. |
| E07 | A, Metawolf7460, iOS, 22.06.2019, [App Store][s2] | Gruppenspaß, aber Anfänger verstehen teils nicht, dass sie selbst Undercover sein könnten. | Direkt; historischer Hinweis auf verständliche Rollenerklärung. |
| E08 | A, schraderbrau, Reddit, 12.08.2024, [Originalthread][s3] | Die Freundesgruppe spielt das Spiel regelmäßig im Sommerurlaub. | Direkt, eingeschränkt: Android oder iOS nicht genannt. |
| E09 | A, dental-misorder, Reddit, Datum im Abruf nur „8mo ago“, [Originalthread][s3] | Wenn Mr. White nicht beginnen darf, verrät der erste Sprecher indirekt seine Unschuld; als erster hätte Mr. White wiederum keine Wortinformation. | Direkt, eingeschränkt; konkretes Regeldilemma. Der Entwickler bestätigt den Zielkonflikt. |
| E10 | B, Nick, Android, 01.06.2026, [Play Store][s4] | Lob für Bedienbarkeit, Gratis-Kategorien sowie eigene Kategorien und Hinweise. | Direkt; Nutzer hat einen Lifetime-Zugang gekauft. |
| E11 | B, Shae Spreafico, Android, 20.12.2025, [Play Store][s4] | Einmalkauf gefällt; zufällige Imposter-Anzahl sollte begrenzbar sein, damit kleine Gruppen keine unbrauchbare Besetzung bekommen. | Direkt; zusätzlich berichtet die Person eine defekte Feedbackfunktion. |
| E12 | B, Nathan Collins, Android, 08.07.2026, [Play Store][s4] | Werbefreie Gratisversion gefällt; dadurch erwägt die Person einen Kauf. | Direkt; Zahlungsbereitschaft einer Person, keine Konversionsmessung. |
| E13 | B, Should play this, iOS, „Jul 29“, [App Store][s5] | Viel Spaß auf einer Reise zu viert, aber Wörter wiederholen sich beim längeren Spielen. | Direkt, eingeschränkt: Jahr fehlt. |
| E14 | B, #PuppyHayden, iOS, „Jul 8“, [App Store][s5] | Zwei Spielvarianten gefallen; zusätzliche Kategorien hinter einem Kauf werden zugleich kritisiert. | Direkt, eingeschränkt: Jahr fehlt. Keine Aussage zu unseren zwei geplanten Rundenmodi. |
| E15 | B, LeonardoDihcaprio, iOS, „Jul 21“, [App Store][s5] | Lob für Alltagstauglichkeit und Werbefreiheit; Wunsch nach weiteren Modi gegen Wiederholung. | Direkt, eingeschränkt: Jahr fehlt. |
| E16 | C, Sahil Saiyed, Android, 23.03.2026, [Play Store][s6] | Ausgeschiedene Personen könnten der nächsten Online-Partie nicht beitreten; deshalb müsse ein neuer Raum erstellt werden. | Direkt; sehr passender Übergangsfehler. **Entwickler meldet am 27.03.2026 einen Fix**; nicht als aktuell offener Fehler behandeln. |
| E17 | C, Rhea Pearl, Android, 01.04.2026, [Play Store][s6] | Viel Spaß mit Freunden, aber englische Wörter erschweren das Spiel für die eigene Gruppe. | Direkt; Sprachverständlichkeit beeinflusst die Wortqualität. |
| E18 | D, Van, Dean & Grant, Android, 04.08.2026, [Play Store][s7] | Schon das Öffnen der Einstellungen werde durch Werbung unterbrochen. | Direkt; heftiger Frust über Bedienunterbrechung, Häufigkeit nicht selbst überprüft. |

## Herstellerangaben getrennt von Spielerfeedback

Yanstar beschreibt ein lokales Spiel auf einem herumgereichten Telefon sowie einen separaten Online-Modus. Bürger erhalten dasselbe Wort, Undercover ein anderes ähnliches Wort, Mr. White keines. Die offizielle Anleitung umfasst wiederholte Beschreibung, Diskussion und Eliminierung. Das erklärt die klassische Variante; ihre Existenz beweist keine allgemeine Präferenz dafür. [Play-Beschreibung][s1], [offizielle Spielanleitung][s9]

Vucaks Produktbeschreibung nennt Offline-Spiel und neben dem Wortmodus „Find the Liar“ mit unterschiedlichen Fragen. Das ist eine andere Unterscheidung als „eine Abstimmung versus mehrere Eliminationsrunden“. E14 darf deshalb nicht als direkte Bestätigung unserer beiden Modi gelesen werden. [Play-Beschreibung][s4]

## Was daraus für unser Spiel folgt

Die folgenden Punkte sind **Produktsynthese und zu prüfende Gestaltungsvorschläge**, keine aus Rezensionen übernommenen Anforderungen.

1. **Die Gruppe soll den Spielabend überleben.** Teilnehmerliste und Einstellungen über mehrere Partien behalten. „Ausgeschieden“ gilt für die laufende Partie; „setzt aus“ für die Auswahl der nächsten Partie. Änderungen an der Gruppe dürfen keinen stillen Reset auslösen. E16 zeigt die praktische Bedeutung eines sauberen Wiedereinstiegs; die konkreten lokalen Aktionen kommen aus der Nutzeranforderung.
2. **Nächste Partie als Hauptaktion.** Nach dem Ergebnis direkt wieder starten oder die Gruppe bearbeiten. Umbenennen an Ort und Stelle; neue Personen ohne erneute Eingabe der bestehenden Namen ergänzen. Vor dem Start eine ungültige Rollenanzahl nachvollziehbar korrigieren lassen. Keine heimliche Änderung laufender Rollenverteilungen.
3. **Wortübergabe als kurze, sichere Interaktion.** Sichtbarer Name und Hochziehen gemäß Nutzerwunsch; vor der Weitergabe muss das Wort wieder verdeckt sein. Im Prototyp überprüfen, ob die Geste sofort verstanden wird, ein versehentliches Antippen etwas enthüllt und Android-Unterbrechungen ein Geheimnis zeigen. Die recherchierten Rezensionen beweisen keine Überlegenheit von Swipe gegenüber Tippen.
4. **Zwei Modi zunächst ausprobieren.** „Schnell“ mit einer abschließenden Entscheidung und „Klassisch“ mit mehreren Eliminierungen sind sinnvolle Testhypothesen aus dem Gespräch. E14/E15 stützen Interesse an Variation allgemein. Sie bestimmen weder die Siegerregeln noch die richtige Rundenlänge.
5. **Kleiner Inhaltsumfang darf sich nicht klein anfühlen.** Wortpaare redaktionell prüfen, bereits gezogene Wörter in einer Sitzung zurückstellen und einen erschöpften Vorrat verständlich behandeln. Ein schlichter Datenbestand reicht für den Start; ein Shop oder Importdialog ist dafür nicht nötig. Die konkrete Größe muss anhand echter Spieldauer getestet werden.
6. **Tempo erhalten.** Für den ersten Prototyp keine Werbeunterbrechungen in Namensbearbeitung, Wortübergabe oder Neustart. Falls später monetarisiert wird, ist ein klarer Einmalkauf eine zu prüfende Option; E11/E12 sind positive Einzelstimmen dafür, kein Beleg für die wirtschaftlich beste Lösung.
7. **Offline-Funktion ernst nehmen.** Lokaler Start, Rückkehr aus dem Hintergrund und Fortsetzen ohne Netz gehören in den praktischen Test. Belegte Netz- und Übergangsprobleme sind bisher überwiegend online; die gewünschte lokale Robustheit muss eigenständig geprüft werden.

## Noch nicht extern belegt oder noch offen

- **Keine direkte lokale Rezension gefunden**, die genau Hinzufügen, Entfernen, Umbenennen und Pausieren ohne Reset fordert. E03, E06 und E16 sind angrenzende Online-Probleme. Daraus keinen angeblich etablierten Wettbewerbsfehler konstruieren.
- **Keine belastbare direkte Präferenz** für eine Runde gegenüber mehreren Eliminierungsrunden. Der Nutzer hat beide selbst erlebt; eigene Gruppentests müssen klären, welche Variante in welcher Situation trägt.
- **Keine belegte Mehrheit für oder gegen Mr. White.** E09 zeigt ein konkretes Startspielerproblem, keine Ablehnung der Rolle insgesamt.
- **Kein externer Nachweis für die gewünschte Aufdeckgeste.** Sie ist trotzdem eine legitime, bestätigte Produktpräferenz.
- **Keine belastbare Android-Absturzserie** in den ausgewählten Originalbeiträgen. E02 und E11 betreffen einzelne Funktionen, E16 einen historisch gemeldeten Übergang. Ein technischer Ursachenbefund wäre spekulativ.
- Wortqualität wurde gelobt und kritisiert, aber die Stichprobe erlaubt keine Rangliste einzelner Wortpakete und keine Aussage über die Qualität eines deutschen Startbestands.
- Vor einer Regelspezifikation offen: Rollen im Schnellmodus, Abstimmungsverfahren, Gleichstand, exakter Siegzeitpunkt, Mr.-White-Raten, Startreihenfolge, Mindest-/Zielgruppengröße und Umgang mit einer Person, die mitten in der Partie gehen muss.

## Nächster sinnvoller Lernschritt

Ein kleiner Spieltest mit wechselnder Besetzung kann die heute schwach belegten Punkte direkt klären: mehrere Partien nacheinander, eine Person kommt dazu, eine setzt aus, eine wird umbenannt, anschließend wechselt der Modus. Messen: Zeit bis zur nächsten Wortverteilung, notwendige Neueingaben, Bedienfehler, versehentliche Enthüllungen und Verwirrung über Rollen. Die Zielwerte erst nach einem ersten Durchlauf festlegen. Dafür ist zunächst ein Papier- oder sehr einfacher Geräteprototyp ausreichend.

[s1]: https://play.google.com/store/apps/details?hl=en&id=com.yanstarstudio.joss.undercover
[s2]: https://apps.apple.com/us/app/undercover-word-party-game/id946882449?see-all=reviews
[s3]: https://www.reddit.com/r/digitaltabletop/comments/8ntayr/iosandroid_undercover_a_werewolflike_social/
[s4]: https://play.google.com/store/apps/details?id=at.vucak.impostor
[s5]: https://apps.apple.com/us/app/imposter-game-party-edition/id6745120053?platform=iphone&see-all=reviews
[s6]: https://play.google.com/store/apps/details?id=com.DefaultCompany.Impostor
[s7]: https://play.google.com/store/apps/details?hl=en_US&id=com.bluffcircle.party
[s8]: https://play.google.com/store/apps/details?hl=en&id=io.kyara.undercover
[s9]: https://www.yanstarstudio.com/undercover-how-to-play
