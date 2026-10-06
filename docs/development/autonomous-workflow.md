# Autonomer Engineering-Ablauf

Stand: 6. Oktober 2026. Auftrag: ein fertiges, leicht bedienbares Android-Partyspiel mit zwei Modi, Deutsch und Englisch, großem Wortbestand und verlässlicher Gruppen-Persistenz. Das aktive Codex-Ziel führt die Arbeit über einzelne Kontextfenster hinaus fort.

## Skill-Auswahl und Reihenfolge

Die benutzerweiten Engineering-Skills wurden vor Implementierungsbeginn gelesen. Der passende Weg ist `research` → `domain-modeling` und `codebase-design` → `to-spec` → `to-tickets` → `implement-spec` mit `tdd` → `code-review` → Unity-Buildvalidierung und Spieltest → Korrekturtickets.

`wayfinder` wurde geprüft. Sein standardmäßiger Gesprächs- und Stoppablauf passt nicht zur ausdrücklich delegierten, durchgehenden Umsetzung. Statt einer zusätzlichen Entscheidungskarte führen eine Produktspezifikation, abhängige GitHub-Tickets und dieses Ausführungsprotokoll durch die Arbeit. `prototype` wird nur bei einer echten offenen Gestaltungsfrage eingesetzt; es ersetzt nicht das Produkt. `diagnosing-bugs` dient später konkreten schwierigen Fehlern, nicht als obligatorischer Zusatzschritt.

Der Nutzer hat autonome Planung, Entscheidungen und Tests ausdrücklich beauftragt. Deshalb werden die üblichen separaten Abnahmen für Test-Seams, Ticketzuschnitt und normale Implementierungsdetails aus `to-spec`, `to-tickets` und `tdd` in diesem Auftrag durch dokumentierte Entscheidungen ersetzt. Dies gilt nicht als Freigabe für Käufe, Store-Veröffentlichung, destruktive Änderungen anderer Projekte oder neue Konten. Bestehende Unity-Installation und Android-Werkzeuge werden wiederverwendet; Nischenreich bleibt ein lesendes Referenzprojekt.

## Phase 1: Evidenz und konkrete Regeln

- Vorhandene Research-Dateien behalten ihren historischen Kontext; neue Nutzerentscheidungen haben Vorrang.
- Erweiterte Originalrezensionen über weitere vergleichbare und angrenzende Spiele untersuchen. Jede Beobachtung hat App, Plattform, Datum soweit vorhanden, Quellenlink und Einordnung.
- Positive und negative Befunde in nachvollziehbare Anforderungen übersetzen. Eine kleine qualitative Stichprobe liefert keine Mehrheits- oder Marktbehauptung.
- Regeln vollständig festlegen: Spielerzahlen, Rollen, Siegbedingungen, Gleichstand, Mr.-White-Rateversuch, Startperson, Abbruch, Wechsel der Gruppe und Wiederaufnahme.
- Release- und Teststrategie anhand offizieller Quellen und tatsächlich vorhandener lokaler Werkzeuge festlegen.

## Phase 2: Spezifikation und Aufgabengraph

GitHub Issues in `giarrel/word-deduction` sind die kanonische Quelle für Spec und Tickets. Eine lokale Kopie dient dem schnellen Lesen. Jeder Anforderung sind Herkunft und prüfbares Verhalten zugeordnet. Offen gebliebene Fragen werden durch begründete, reversible Produktentscheidungen aufgelöst.

Tickets liefern jeweils einen schmalen, benutzbaren Weg durch Darstellung, Spiellogik und Speicherung. Abhängigkeiten stehen ausdrücklich im Ticket; unabhängige Arbeit kann parallel beginnen. Bekannte Änderungen werden nicht vorsorglich in viele hypothetische Module aufgeteilt.

## Phase 3: Implement-Spec

- Eine gemeinsame Integrationsbranch sammelt die gesamte Spec.
- Implementierungsagenten arbeiten in eigenen Git-Worktrees auf Ticketbranches, jeweils vom aktuellen Integrationsstand aus.
- Sie lesen Spec, Ticket, Glossar und relevante Entscheidungen über Dateipfade/Links und setzen Verhalten mit `tdd` um: erst beobachtetes Rot, dann Grün, jeweils ein Verhalten.
- Vor Abschluss holen sie den Integrationsstand. Ein separater Merger-Agent integriert und prüft den jeweiligen Beitrag.
- Nur Tickets ohne offene Blocker werden begonnen. Gemeinsame Unity-Editorzustände werden serialisiert; parallele Agenten dürfen nicht denselben Editor umbauen.
- Nach vollständiger Umsetzung prüft `code-review` Standards und Spezifikation getrennt gegen den festgehaltenen Ausgangscommit. Befunde werden behoben und erneut geprüft.

## Prüf-Seams

Gemäß `codebase-design` liegen Tests möglichst hoch an kleinen Interfaces tiefer Module. Die Festlegung erfolgt hier im Rahmen der delegierten Testentscheidungen:

1. **Spielabend-Interface:** Aktionen der Gruppe und Partie sowie die danach sichtbare Projektion. Eine echte temporäre Ablage erlaubt Schließen und erneutes Öffnen. Regeln, Identitäten, Wörterverlauf, Änderungen und Wiederaufnahme werden hier als Nutzungsszenarien geprüft; keine Tests privater Methoden oder interner Aufrufreihenfolgen.
2. **Gerenderte App:** tatsächliche Eingaben, Aufdeckgeste, Loslassen, Fokusverlust, Navigation, Texte und Bildschirmbilder. Dieses zweite Seam ist nötig, weil das fachliche Interface weder Berührung noch Layout noch Android-Lebenszyklus beweist.

Dateisystemfehler und Zufall werden nur an ihren tatsächlichen äußeren Seams kontrolliert. Validierung des ausgelieferten Wortbestands ist ein Datenvertrag, kein Duplikat der Laufzeitimplementierung.

## Phase 4: Verbesserungsloop

Jeder Durchlauf enthält Szenario, erwartetes und beobachtetes Verhalten, Screenshot oder Testnachweis, Schwere, Änderung und Nachprüfung. Reihenfolge: Datenverlust/Geheimnisleck/Regelfehler → Bedienhindernisse → Lesbarkeit und visuelle Konsistenz → dezente Bewegung und Feinschliff.

Wiederholt prüfen: frischer Start; nächste Partie; Hinzufügen/Pausieren/Umbenennen/Entfernen; beide Modi; Mr. White; Gleichstand; Neustart während jeder Spielphase; Loslassen und App-Wechsel während der Wortanzeige; kleine und große Gruppen; lange Namen; Deutsch und Englisch; kleine und hohe Displays; Offline-Nutzung. Eine erzeugte Screenshot-Datei gilt erst nach tatsächlicher Sichtprüfung als visuell geprüft.

Zusätzliche Features entstehen nicht aus Langeweile: Nur bestätigte Anforderungen oder konkrete Befunde rechtfertigen Erweiterungen. Der Loop endet bei erfüllten Kriterien, nicht nach einer beliebigen Anzahl kosmetischer Änderungen.

## Fertigkriterien

- Alle Spec-Anforderungen sind mit bestandenem Nachweis oder ausdrücklich benannter externer Voraussetzung erfasst; offene Produktfehler werden nicht als fertig bezeichnet.
- Beide Modi funktionieren vom Start bis Ergebnis und Folgepartie. Spielergruppe und bestätigter Partiestand überstehen Prozessende. Geheimwörter bleiben bei Übergabe/Wiederaufnahme verdeckt.
- Beide Sprachen sind vollständig, Wortdaten validiert und ausreichend vielfältig; Wiederholungsvermeidung funktioniert über Neustarts hinweg.
- Echte App-Ansichten wurden kritisch geprüft und Bedienhindernisse korrigiert. Android-spezifische Tests werden getrennt von Editor-Prüfungen ausgewiesen.
- Reproduzierbarer Android-Build, APK zum Testen, AAB-Erzeugung, Versions-/Hashnachweise und Release-Checkliste liegen vor. Release-Signierung benötigt einen dauerhaft verwahrten Schlüssel; ein Entwicklungskey wird nicht als Store-Identität ausgegeben.
- Storetexte, Datenschutzangaben und erforderliche Bildmaterialien sind vorbereitet. Konto, konkrete Produktionssignierung und Storefreigabe werden erst am tatsächlichen Übergabepunkt angefordert, sofern sie nicht schon verfügbar und autorisiert sind.
- Review ist abgeschlossen; der Integrationsstand und verbleibende Einschränkungen sind dokumentiert. Kein Anspruch auf reale Gruppendynamik, physische Haptik oder Gerätekombinationen, die nicht tatsächlich getestet wurden.
