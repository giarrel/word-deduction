# Freie Rollenanzahlen: Anforderung für die nächste Version

Kanonisch: [GitHub-Issue #11](https://github.com/giarrel/word-deduction/issues/11). Status: `needs-info`; die offenen Siegbedingungen stehen im Ticket.

## Ziel

Zur nächsten Version gehört mehr Freiheit bei der Anzahl von Undercover und Mr. White in **Schnell und Klassisch**, einschließlich Mr. White im Schnellmodus. Die Auswahl soll dieselben gemeinsamen Obergrenzen verwenden wie der dritte Modus. Diese Anforderung ergänzt die [Könige-Spezifikation #10](https://github.com/giarrel/word-deduction/issues/10); sie ist kein Auftrag, die App bereits zu implementieren.

## Bestätigte Anforderungen

- Undercover- und Mr.-White-Anzahlen in den Grundmodi veränderbar machen; keine dauerhafte Beschränkung des Schnellmodus auf genau einen Undercover und keinen Mr. White.
- Sinnvolle Maximalgrenzen sind erwünscht. Eine gemeinsame Grenzregel über alle drei Modi verwenden, statt voneinander abweichender unabhängiger Grenztabellen.
- Bestehende automatische Verteilungen als bequeme Startwerte erhalten. Manuelle Wünsche und Spielergruppe dauerhaft speichern; Gruppenwechsel dürfen keine erneute Namenseingabe oder vollständige Neueinrichtung erzwingen.
- Regeln, die die Gruppe am Tisch umsetzen kann, bleiben Regeln. Keine digitalen Einzelabstimmungen oder zusätzlichen Pflichtphasen zur Verwaltung größerer Rollenanzahlen.
- Der dritte Modus behält die bereits abgestimmte Struktur: genau ein guter König und genau ein Mr. White als böser König; mindestens ein gewöhnlicher Undercover. Freie Rollenanzahlen in den Grundmodi führen dort nicht zu mehreren bösen Königen.

## Gemeinsame Obergrenze

Als konkrete, einheitliche Planungsregel wird die bereits für Könige beschlossene anfängliche Mehrheit der Guten verwendet:

- Undercover und Mr. White zählen **zusammen** gegen dieselbe Grenze.
- Beim Start müssen mehr gute Teilnehmer als Undercover plus Mr. White vorhanden sein.
- Damit sind bei 3–4 Personen insgesamt höchstens 1 Gegenspieler, bei 5–6 höchstens 2, bei 7–8 höchstens 3 und bei 20 höchstens 9 möglich.
- Könige zählen zu ihrer jeweiligen Seite; der gute König ist kein zusätzlicher Teilnehmer außerhalb der guten Anzahl.
- Es gibt keine getrennten Maximalwerte, die einzeln gültig wären, zusammen aber eine ungültige Verteilung erlauben.
- Die gemeinsame Grenze gilt für die Startaufstellung. Laufende Partien werden nach Eliminierungen nicht neu verteilt oder wegen dieser Grenze beendet.
- Modusspezifische Struktur bleibt zusätzlich gültig: Könige startet erst ab fünf Personen und enthält genau einen Mr. White; Schnell und Klassisch behalten zunächst ihre vorhandenen Mindestgrößen.
- Eine zentrale fachliche Gültigkeitsprüfung muss Anzeige, Startfreigabe, Zuteilung und gespeicherten Partiestand konsistent behandeln.

Die Übertragung der guten Anfangsmehrheit auf die Grundmodi ist eine nachvollziehbare Planungsentscheidung aus der vorhandenen Regel, keine Behauptung bereits nachgewiesener Spielbalance. Ob zusätzliche feste Obergrenzen pro Rolle gewünscht sind, ist damit nicht vorgegeben.

## Noch vor einer umsetzungsreifen Spec zu entscheiden

1. **Schnell mit mehreren Gegenspielern:** Reicht bei einer einzigen Abstimmung ein beliebiger gefundener Gegenspieler für einen Sieg der Guten, oder muss die Gruppe mehrere Personen benennen? Die Eigenschaft einer kurzen Partie soll erhalten bleiben.
2. **Mr. White im Schnellmodus:** Welche Wortratechance folgt seiner Wahl, und wie geht das bei weiteren noch nicht gewählten Gegenspielern aus?
3. **Mehrere Mr. Whites in Klassisch:** Gewinnt ein richtig ratender Mr. White allein oder die gesamte White-Seite? Wie wirkt sich das auf verbleibende Undercover aus?
4. **Zulässige Kombinationen am unteren Rand:** Können die Grundmodi auch mit null Undercover und einem oder mehreren Mr. Whites starten? Eine Partie ganz ohne Gegenspieler ist bisher nicht vorgesehen.

Diese Punkte sind keine bereits beantworteten Fragen aus dem Könige-Grilling. Deshalb bleibt dieses Ticket ausdrücklich **needs-info**. Es darf nicht durch erfundene Siegesbedingungen als umsetzungsreif behandelt werden. Die fertige Könige-Spezifikation bleibt eigenständig umsetzungsreif.

## Geplante Prüfung

Die vorhandenen zwei Testebenen weiterverwenden: öffentliches Session-Verhalten mit echter Speicherung sowie gerenderte App für Auswahl, sichtbare Grenzen und Bedienung. Nach Klärung der Regeln insbesondere alle gültigen Rollenpaare an Mindest- und Maximalgrößen, gemeinsame Gesamtgrenze, Gruppenverkleinerung/-vergrößerung, Moduswechsel, Wiederaufnahme, Folgepartien und sämtliche neuen Ergebniswege prüfen. Keine Tests oder App-Funktionen zum Überwachen menschlicher Abstimmungen.

## Herkunft

Der Nutzer wünschte bereits beim Sammeln des Versionsfeedbacks freie Rollenanzahlen und Mr. White im Schnellmodus. Nach Veröffentlichung von #10 stellte er ausdrücklich klar, dass diese Freiheit auch in die Planung der Grundmodi gehört und gemeinsame Maximallimits über alle Modi sinnvoll sind.
