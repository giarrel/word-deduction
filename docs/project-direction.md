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
