# Aktuelle Projektentscheidungen

Stand: 6. Oktober 2026. Diese Entscheidungen aus dem Projektgespräch haben Vorrang vor den noch offenen Empfehlungen der ersten Research-Runde.

## Technik

- Der Nutzer hat **Unity** für Word Deduction gewählt. Android bleibt die erste Zielplattform.
- Ausgangsbasis soll das für Nischenreich bereits verwendete Setup sein. Der [Setup-Check](development/unity-setup.md) bestätigt Unity **6000.3.25f1** mit Android-Werkzeugen in einer benutzerweiten Installation.
- Gruppenverwaltung, Regelkern, dauerhafte Speicherung und Darstellung bleiben fachlich getrennt. Die Wahl von Unity ändert diese Anforderungen nicht.
- Der aktuelle Auftrag ist die autonome Umsetzung bis zu einem geprüften Android-Release-Kandidaten. Research und Spezifikation kommen vor der Implementierung; anschließend folgen eigene Spieltests, visuelle Kritik und Korrekturschleifen.
- Lokales Pass-and-Play auf einem gemeinsam herumgereichten Handy ist bestätigt. Kein Online-Modus.
- Beide Spielmodi gehören zum Umfang: eine kurze Partie mit einer Hinweisrunde und Entscheidung sowie klassische Partien mit wiederholten Eliminierungen und optionalem Mr. White.
- Oberfläche, Regeln und Wörter sind von Anfang an auf Deutsch und Englisch verfügbar. Eine große, redaktionell geprüfte Wörterauswahl gehört zur ersten Version.

## Gestaltung und Wortkarte

Vom Nutzer bestätigt:

- Modern, weich und rund; wenig kantige Formen.
- Abgerundete Buttons und eine abgerundete Wortkarte.
- Leicht spielerische Menüs mit stimmigen Übergängen.
- Zuerst ist der Name sichtbar. Hochziehen der Karte zeigt das geheime Wort.
- **Beim Loslassen klappt die Karte wieder nach unten und verdeckt das Wort.** Dieser Punkt war in der Research-Runde noch offen und ist jetzt entschieden.
- Eine eigenständige, stimmige Oberfläche genügt; eine gesonderte Illustrationsproduktion ist keine Voraussetzung. Visuelle Prüfung echter App-Bilder und Verbesserung der Bedienung gehören zur Umsetzung.

Die bereits festgehaltenen Prioritäten für einfache Gruppenänderungen zwischen Partien und den Erhalt von Namen und Einstellungen bleiben maßgeblich. Der Nutzer delegiert normale Produkt-, Architektur- und Testentscheidungen ausdrücklich: Grundlage sind seine Eingaben, belegtes Spielerfeedback und eigene Tests. Entscheidungen und verbleibende Unsicherheiten werden dokumentiert, ohne jede Implementierungsentscheidung zur Rückfrage zu machen.

Der [Engineering-Ablauf](development/autonomous-workflow.md) beschreibt Planung, Umsetzung und Fertigkriterien. Veröffentlichung in einem Store ist ein eigener externer Schritt; ein erfolgreicher Build allein gilt nicht als veröffentlichungsfähig.

## Offenes Brainstorming für die nächste Version

Feedback vom 7.–8. Oktober 2026. Die Regeln des dritten Modus wurden im Grilling bis einschließlich Q19 gemeinsam geklärt. Auf ausdrücklichen Auftrag mit `to-spec` ist daraus die kanonische [Spezifikation #10](https://github.com/giarrel/word-deduction/issues/10) mit Label `ready-for-agent` entstanden; eine [lokale Kopie](specs/kings-mode.md) ist ebenfalls vorhanden. Die folgenden Notizen bewahren die Herkunft der Entscheidungen. Veröffentlichung der Spec beauftragt noch keine Implementierung. Weitere Änderungswünsche für Schnell und Klassisch bleiben separat offen. Der vorhandene Release-Kandidat bleibt unverändert.

### Wünsche und aktueller Rollenentwurf des Nutzers

- Die Anzahl von Undercover und Mr. White soll freier einstellbar sein, auch im Schnellmodus. Mr. White soll dort ebenfalls möglich sein. Zulässige Kombinationen und passende Siegesbedingungen müssen noch ausgearbeitet werden.
- Nach Veröffentlichung von Spec #10 ausdrücklich ergänzt: Die freie Anzahlwahl gehört auch in die Grundmodi; gemeinsame Obergrenzen sollen über alle drei Modi gelten. [Anforderung #11](https://github.com/giarrel/word-deduction/issues/11) hält dies samt noch offenen Siegesbedingungen fest. Spec #10 ist entsprechend verknüpft und verlangt eine gemeinsame Grenzprüfung. Als konkrete Planungsregel gilt die anfängliche Mehrheit der Guten über Undercover plus Mr. White zusammen; Könige behält zusätzlich genau einen Mr. White. Die Obergrenze gilt beim Start, nicht als neue Siegbedingung während der Partie.
- Der Nutzer hat den Anführer-/Königsentwurf als **dritten, eigenständigen Modus** bestätigt. Schnell und Klassisch bleiben eigene Modi. Rolleninformationen werden beim privaten Aufdecken der Karte sichtbar. Die offenen Regeln werden vor einer Implementierung gemeinsam im Grilling geklärt.
- Neuester Entwurf vom 8. Oktober: Die guten Teilnehmer und die Undercover erfahren jeweils, wer ihr eigener **Group Leader / Anführer** ist. Sie erfahren dabei weiterhin nicht, ob sie gut oder böse sind. Die Anzeige muss diese Ungewissheit erhalten, etwa mit „Dein Anführer: [Name]“ statt einer Parteibezeichnung.
- Beide Seiten haben einen Anführer, vom Nutzer auch Boss beziehungsweise König genannt. Wird ein Boss herausgewählt, verliert dessen Team grundsätzlich. Die letzte Chance des bösen Anführers ist dabei als mögliche Ausnahme zu berücksichtigen.
- Beide Anführer kennen ausdrücklich ihre eigene Seite. Der gute König kennt sein Wort und alle bösen Personen, aber nicht, welche davon der böse König ist. Der böse König kennt sein eigenes Team, jedoch weder den guten König noch das gute Wort. Er erhält kein eigenes Wort.
- **Mr. White ist im dritten Modus der böse König**, keine zusätzliche dritte Seite. Diese Festlegung ersetzt den Vorschlag, Mr. White aus diesem Modus herauszulassen. Q6 klärt den vorübergehenden Widerspruch zu Q2: Mr. White hat kein Wort; gewöhnliche Undercover behalten ihr abweichendes Wort.
- **Kein separater Assassine:** Nach seiner Abwahl übernimmt Mr. White die letzte Chance. Er entscheidet allein und verbindlich zwischen dem Raten des guten Wortes und dem gezielten Ausschalten des guten Königs durch dessen Identifikation. Er hat genau einen Versuch, keine zweite Option nach einem Fehlschlag und keine Beratung währenddessen. Erfolg bedeutet Sieg des gesamten bösen Teams, Fehlschlag Sieg des gesamten guten Teams. Bis zur Auflösung bleiben die übrigen Geheimnisse verdeckt.

### Einordnung der bisherigen Ideen

Die zuvor erwogene separate Assassinenrolle ist durch die letzte Chance des bösen Anführers ersetzt. Die gefährdete gute Sonderrolle und die gute Person mit Kenntnis der Bösen sind im guten König zusammengeführt. Nun hängt grundsätzlich jede Seite an ihrem eigenen Boss. Quest wurde vom Nutzer als Inspiration für die letzte Jagd genannt; dies ist unser eigener Entwurf, keine geprüfte Wiedergabe seiner Regeln.

### Grilling: bestätigte Antworten aus Runde 1

- **Q1 angenommen:** Guter König kennt seine Seite, sein Wort und die Namen aller Bösen, aber nicht deren König.
- **Q2 angenommen, Wortbesitz später durch Q6 korrigiert:** Böser König kennt seine Seite und sein Team, nicht den guten König oder dessen Wort. Q2 enthielt ein eigenes Undercover-Wort; Q5 macht ihn zu Mr. White und Q6 legt ausdrücklich fest, dass er kein Wort hat.
- **Q3 angenommen:** Behauptungen und Lügen über Identitäten und Anführer sind erlaubt; Karten zeigen ist verboten. Der Nutzer akzeptiert das strategische Risiko einer Offenlegung: Der gute König wird dadurch zum Ziel der letzten Jagd, der böse König zum Abstimmungsziel der anfänglichen guten Mehrheit.
- **Q4 vorläufig angenommen:** Ein herausgewählter gewöhnlicher Teilnehmer scheidet aus, gibt keine Hinweise mehr und stimmt nicht mehr ab. Die App zeigt nur „Kein König“; Seite, Wort und Anführer bleiben bis zum endgültigen Ergebnis geheim. Ausgeschiedene gewinnen oder verlieren weiterhin mit ihrem Team.
- **Q5 angepasst:** Mr. White übernimmt die Rolle des bösen Königs. Keine separate Assassinenrolle.

Die Antworten gelten für den dritten Modus. Regeln und Rollenanzahlen von Schnell und Klassisch werden dadurch nicht automatisch geändert.

### Grilling: bestätigte Antworten aus Runde 2

- **Q6 angenommen:** Mr. White hat kein Wort. Das ersetzt die Wortannahme aus Q2; seine übrigen Informationen bleiben bestehen.
- **Q7 angenommen, später um Q11 ergänzt:** Die Partie läuft grundsätzlich bis zur Abwahl eines Königs. Keine automatische Niederlage bei gleicher Teamgröße oder nur einem verbliebenen Bürger. Die ausdrücklich bestätigte Ausnahme für zwei verbliebene Könige steht in Q11.
- **Q8 angenommen:** Letzte Chance allein, verbindlich entweder Wort oder König, genau ein Versuch, keine Beratung. Treffer gewinnt für das böse Team, Fehlschlag für das gute Team. Keine Enthüllung der übrigen Geheimnisse vor der Entscheidung.
- **Q9 angenommen:** Genau ein König je Seite, mindestens ein gewöhnlicher Mitspieler pro König und anfänglich mehr Gute als Böse. Minimum fünf Personen: drei Gute einschließlich ihres Königs gegen Mr. White und einen Undercover. Undercover-Anzahl innerhalb dieser Grenzen einstellbar. Die Spielbalance ist noch nicht praktisch belegt.
- **Q10 angenommen:** Jede Runde besteht aus einem Hinweis je verbliebenem Teilnehmer, gemeinsamer Diskussion und einer Abstimmung. Nach der Eliminierung eines gewöhnlichen Teilnehmers beginnt eine neue Hinweisrunde. Keine Nachtphasen oder zusätzlichen privaten Sonderaktionen; die besonderen Informationen gibt es beim anfänglichen Aufdecken, die letzte Chance erst nach Mr. Whites Abwahl.

### Grilling: bestätigte Antworten aus Runde 3

- **Q11 angenommen:** Bleiben ausschließlich die beiden Könige übrig, gewinnen die Bösen automatisch. Das ist die ausdrücklich gewählte Ausnahme von Q7: Der gute König ist dann als Jagdziel eindeutig, und eine gegenseitige Abstimmung würde festhängen.
- **Q12 als Tischregel angenommen, ausdrücklich ohne App-Ablauf:** Alle verbliebenen Teilnehmer zeigen gleichzeitig auf eine andere Person, eine Stimme pro Person. Die meisten Stimmen reichen. Bei Gleichstand einmal Stichwahl zwischen den Führenden; bei erneutem Gleichstand folgt eine neue Hinweisrunde ohne Eliminierung. Diese Regeln gehören ausschließlich in die Regelbeschreibung, nicht in Abstimmungs-, Stimmenzähl- oder Stichwahlfunktionen der App.
- **Q13 angenommen:** Jede Hinweisrunde beginnt bei einer zufällig gewählten verbliebenen Person, einschließlich Mr. White; danach geht es reihum. Mr. White muss gegebenenfalls ohne vorangehenden Hinweis improvisieren. Keine appseitige Kontrolle der Sprecherfolge.
- **Q14 angenommen:** Ein Wort als Hinweis. Das eigene Geheimwort, offensichtliche Wortformen und direkte Übersetzungen sowie Hinweise auf Schreibweise oder Anfangsbuchstaben sind verboten. Anschließend freie Diskussion und Rollenbluffs; das eigene Geheimwort darf weiterhin nicht direkt genannt werden. Dies sind Tischregeln, keine automatische Sprachkontrolle.
- **Q15 angenommen:** Genau eine ausgesprochene Wortantwort wird von der Gruppe nach Bedeutung bewertet. Artikel, Einzahl/Mehrzahl und echte Synonyme sind zulässig; bloß verwandte Begriffe reichen nicht. Das Zielwort wird erst nach der verbindlichen Antwort gezeigt. Keine automatische Wort- oder Spracherkennung.
- **Q16 angenommen:** Könige und Teams werden jede Partie innerhalb der eingestellten Größen vollständig neu zufällig verteilt. Wiederholungen sind möglich; keine garantierte Rotation oder erzwungener Rollenwechsel.

### Grilling: bestätigte Antworten aus Runde 4

- **Q17 abweichend entschieden, Betrachtungsdauer durch Q19 geklärt:** Die private Karte wird nur bei der anfänglichen Vergabe angeschaut; danach ist kein erneutes Anschauen erlaubt. Die Gedächtnisbelastung des Königs ist vom Nutzer bewusst gewünscht. Längeres Betrachten kann seine Rolle verraten, ist aber nicht verboten.
- **Q18 angenommen:** Keine zusätzlichen Strafregeln oder App-Funktionen für versehentlich verratene Wörter beziehungsweise ungültige Hinweise. Grundsätzlich wird weitergespielt; wenn ein Versehen die Partie verdorben hat, kann die Gruppe gemeinsam eine neue starten. Absichtliche Behauptungen oder Enthüllungen von Anführeridentitäten bleiben wie vereinbart Teil des Bluffspiels.

### Grilling: bestätigte Antwort aus Runde 5

- **Q19 angenommen:** Keine feste Betrachtungsdauer und kein App-Timer. Jeder entscheidet selbst, wann er das Handy weitergibt. Langes Schauen ist ein strategisches Risiko, kein Regelverstoß. Gewöhnliche Teilnehmer dürfen absichtlich länger schauen, um einen König zu decken; ein König kann kurz schauen und Erinnerungslücken riskieren. Nach abgeschlossener eigener Übergabe gibt es keine spätere Wiederansicht der Karte.

Damit sind die im Grilling behandelten Regelfragen des dritten Modus entschieden. Der Nutzer hat anschließend ausdrücklich die Synthese und Veröffentlichung mit `to-spec` beauftragt; die konsolidierte Grundlage steht in Spezifikation #10. Dieses Dokument beauftragt keine Implementierung. Praktische Balance und Verständlichkeit sind anschließend durch echte Partien zu prüfen.

### Produktvorgabe: schlanke App

Der Nutzer hat ausdrücklich präzisiert: Was als zwischenmenschliche Regel am Tisch funktioniert, soll nicht als zusätzlicher App-Ablauf implementiert werden. Insbesondere Q12 wird nur in den Regeln beschrieben. Abstimmung, Stichwahl, Diskussion, Hinweisregeln und das Einhalten von Rede-/Beratungsverboten liegen bei der Gruppe. Daraus folgen keine zusätzlichen Abstimmungsbildschirme, Stimmenzählung, Stichwahlzustände, Diskussionsphasen, Pflicht-Timer oder Bestätigungen für jeden Hinweis.

Die App soll sich auf geheime Zuteilung und Anzeige, notwendigen Spielstand und Auflösung konzentrieren. Als minimaler Entwurf genügt nach einer erfolgten Tischabstimmung das Eintragen der ausgeschiedenen Person; Gleichstände erfordern keinen App-Schritt. Bei einer Wortratechance beurteilt die Gruppe die Antwort. Die genaue Oberfläche wird erst nach Abschluss der Regelfindung spezifiziert. Bestehende Funktionen des gelieferten Release-Kandidaten werden durch diese Notizen nicht verändert.

### Erinnerung an die aktuelle Version

Nur ein herausgewählter Mr. White darf in Klassisch einmal das Mehrheitswort laut raten. Die Gruppe bewertet die Antwort. Eine richtige Antwort beendet die Partie sofort mit einem alleinigen Sieg von Mr. White; bei einer falschen Antwort bleibt er ausgeschieden und die übrigen Siegesbedingungen werden geprüft. Undercover haben aktuell keine zusätzliche Wortratechance. Im Schnellmodus gibt es aktuell weder Mr. White noch eine Wortratephase.

### Offene Regelfragen für später

- Das Anführer-System gehört zum dritten Modus. Für den davon getrennten Schnellmodus bleibt offen, wie mehrere böse Teilnehmer und Mr. White zu einer Partie mit nur einer Hinweisrunde und einer Entscheidung passen.
- Die grundsätzlichen Teamgrenzen des dritten Modus stehen durch Q9 fest. Spezifikation #10 ergänzt als ausdrücklich gekennzeichnete Integrationsentscheidung einen gespeicherten Wunschwert und eine sichtbar begrenzte wirksame Anzahl bei kleineren Gruppen. Für die anderen Modi bleiben die frei wählbaren Rollenanzahlen gesondert auszuarbeiten.
- Die gemeinsame böse Seite ist für den dritten Modus festgelegt. Für Schnell und Klassisch bleiben Ziele und Zusammenspiel bei mehreren Mr. Whites offen.
- Für den dritten Modus sind die besprochenen Regeln durch Q1–Q19 geklärt, einschließlich freier Betrachtungsdauer ohne spätere Wiederansicht. Spezifikation #10 beschreibt die schlanke Oberfläche und verwendet „Könige / Kings“ als Arbeitsnamen. Endgültiges Branding ist keine offene Frage des hier abgestimmten Regelkerns.

Die verbleibenden Fragen zu Schnell und Klassisch werden gesondert behandelt. Für den dritten Modus ersetzt Spezifikation #10 widersprechende frühere Rollenideen und dient als Grundlage für einen späteren Implementierungsauftrag.
