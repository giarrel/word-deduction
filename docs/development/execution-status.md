# Umsetzung: Android v1

Ziel und Arbeitsweise: [Engineering-Ablauf](autonomous-workflow.md). Kanonische [Spec](https://github.com/giarrel/word-deduction/issues/2), [lokale Lesekopie](../specs/android-release-v1.md), [Gesamtziel](https://github.com/giarrel/word-deduction/issues/1).

## Vor Implementierungsbeginn abgeschlossen

- Engineering-Skills einschließlich `implement-spec` und `codebase-design` gelesen; autonome Entscheidungen und zwei Test-Seams festgehalten.
- Erste Recherche plus [29 zusätzliche Originalrezensionen](../research/2026-10-06-expanded-player-feedback.md) aus sechs weiteren Apps ausgewertet. Die Stichprobe bleibt qualitativ und überwiegend historisch.
- [Release-/Playtest-Strategie](../research/2026-10-06-release-and-playtest-strategy.md) mit aktuellen offiziellen Quellen und lokaler Werkzeugprüfung erstellt.
- Regeln, Glossar, Spec und sieben abhängige Umsetzungstickets veröffentlicht. Keine offenen Grundregelfragen auf Implementierer abgewälzt.

## Aufgabengraph

| Ticket | Ergebnis | Voraussetzung | Stand |
|---|---|---|---|
| [Gespeicherte Gruppe](https://github.com/giarrel/word-deduction/issues/3) | Ausführbare Unity-App, dauerhafte Gruppe, Test-/Buildbasis | Keine | Abgenommen und integriert: `318af96`, Testpfad/Doku `ac0dcca` |
| [Schnellmodus](https://github.com/giarrel/word-deduction/issues/4) | Karte → Gespräch → Vote → Ergebnis → Folgepartie | Gruppe | Abgenommen und integriert: `496d446`, Integrationsprüfung `013087f` |
| [Klassischer Modus](https://github.com/giarrel/word-deduction/issues/5) | Mehrere Runden und Mr. White | Schnellmodus | Abgenommen: `256678d` plus drei native Android-Partien |
| [Großer DE/EN-Wortbestand](https://github.com/giarrel/word-deduction/issues/6) | Redaktionelle Inhalte, Wiederholungsvermeidung, Übersetzungen | Schnellmodus | Abgenommen: `029ed64`/`d5da6f0`; kombinierte Karten-, Update- und Sprachwechselprüfung bestanden |
| [Unterbrechung und Recovery](https://github.com/giarrel/word-deduction/issues/7) | Speicherschäden, Lebenszyklus, Geheimnisschutz | Schnellmodus | Abgenommen: `d5da6f0`; native Recovery-, Mehrfinger-, Systemgesten- und IME-Prüfung bestanden |
| [Bedienung und visuelle Playtests](https://github.com/giarrel/word-deduction/issues/8) | Belegte Verbesserung der vollständigen App | Klassisch, Inhalte, Recovery | Abgenommen: `7404d17`; separat integriert mit `2423343`, 53/53 Session-Tests |
| [Release-Kandidat](https://github.com/giarrel/word-deduction/issues/9) | Review, APK/AAB, Verpackungsnachweise, Storeunterlagen | Playtests | Bereit |

```mermaid
flowchart LR
  A[Gespeicherte Gruppe] --> B[Schnellmodus]
  B --> C[Klassisch]
  B --> D[Wörter DE/EN]
  B --> E[Recovery]
  C --> F[Playtests und visuelle Korrekturen]
  D --> F
  E --> F
  F --> G[Review und Release-Kandidat]
```

## Prüfzugang und verbleibende externe Voraussetzungen

Unity, Android-Compiler und Paketprüfwerkzeuge sind vorhanden. Der eigene Android-16-Emulator `emulator-5580` läuft mit API 36, 1080×2400 bei 420 dpi, Google-APIs-x86_64-Image Revision 7 und ARM64-Übersetzung. Die ARM64-Gruppenbasis wurde erfolgreich gebaut, installiert und bedient. Der anfängliche Schriftfehler wurde per A/B-Vergleich auf den Emulator-Grafikpfad eingegrenzt: vorhandene NVIDIA-Hostgrafik rendert beide geprüften APKs korrekt, SwiftShader nicht. Siehe [wiederverwendbare Prüfumgebung](android-test-device.md).

Die frühere Ableitung aus `HypervisorPlatform InstallState: 2` war zu stark: Der direkte Emulatorcheck bestätigt nutzbares WHPX, und der tatsächliche Boot gelang. Keine Windows-Funktion wurde geändert und kein Rechnerneustart veranlasst. Ebenso ist die Unity-Dokumentation zum eingeschränkten Magic-Leap-x86_64-Ziel kein Beweis einer allgemeinen technischen x86_64-Buildsperre; die [6.3-Release-Notes](https://unity.com/releases/editor/whats-new/6000.3.0f1) und lokal vorhandenen Playerdateien stützen einen späteren Vergleichsbuild, falls erforderlich.

Die Gruppenbasis ist inzwischen abgenommen: 13/13 Session-Tests, 5/5 Unity-PlayMode-Tests und tatsächliche Android-Bedienung mit erhaltenen IDs, Einstellungen und Undo nach APK-Updates und Prozessneustarts. Android-spezifische Fehler bei Tastaturüberdeckung, ungewolltem Speichern durch Zurück, Tastaturabschluss und Sichtbarkeit neuer/bearbeiteter Zeilen wurden reproduziert und korrigiert. [Foundation-Nachweise](../validation/group-foundation.md), [Android-Korrekturschleifen](../validation/android-group-runtime/report.md) und [separate Integration](../validation/group-foundation-merge.md). Der anschließend abgenommene Schnellmodus ist unten dokumentiert.

Ein physisches Telefon wurde über die Chat-Rückfrage angefragt; noch keines ist bestätigt. Emulator-, übersetzte ARM64- und physische ARM64-Nachweise bleiben getrennt. Produktionssignierung, Play-Kontostatus und Publisherkontakt sind nicht geprüft; sie werden am konkreten Release-Artefakt geklärt. Tatsächliches TalkBack mit Touch Exploration bedient inzwischen öffentliche App-Menüs und Gboard; echte Zweifingergesten scrollen mit erhaltenem Fokus. Geheimlabels bleiben ausgeschlossen. Das 48-dp-Ziel ist nativ nachgemessen, CJK/Arabisch/Devanagari und Emoji sind sichtbar. Große Systemschrift wird bewusst auf 150% begrenzt. Der letzte native Gegencheck für lange Besitzernamen und den Kapazitätshinweis ist in DE/EN auf `7404d17` bestanden. Siehe [UI-Abnahme](../validation/ui-polish/report.md).

## Integrationskonvention

Die Implementation läuft auf `integration/android-v1`. Je Ticket eine eigene Branch und ein eigener Worktree; ein Merger-Agent übernimmt Integration. GitHub-Tickets werden nach tatsächlicher Abnahme mit Ergebnisnachweis geschlossen. Feste Basis des abschließenden Reviews: Planungscommit `ab25c325e02040d30755ae448c07789357730f38`.

## Schnellmodus abgenommen

21/21 Session-Tests, 10/10 Unity-PlayMode-Tests und drei vollständig bediente Android-Partien (richtiger Verdacht, falscher Verdacht, wiederholter Gleichstand). V1-Gruppe unverändert übernommen, V2 bei bewusster Aktion geschrieben; nach Folgepartien und bestätigtem Abbruch sind alle acht Spieleridentitäten erhalten. Native Zwei-Finger-Eingabe und Android-Aufnahmeschutz geprüft. [Android-Nachweise](../validation/android-quick-runtime/report.md), [separate Integration](../validation/quick-mode-merge.md). Der nachfolgend abgenommene Classic-Modus ist unten dokumentiert.

## Classic abgenommen, großer Wortbestand integriert

Classic: 29/29 Session-Szenarien, 14/14 gerenderte Tests und drei vollständig bediente Android-Partien in EN/DE. White-Sieg, Bürgersieg nach falschem Tipp des letzten Gegners, beide überlebenden Gegnerrollen, Gleichstand ohne Eliminierung, 4↔5-White-Präferenz und Wiederaufnahme nach Prozessende geprüft. [Android-Abnahme](../validation/android-classic-runtime/report.md), [separate Integration](../validation/classic-mode-merge.md). Kein offener Classic-Regelfehler in diesem Umfang.

Der große Wortbestand ist separat integriert: 520 Paare, 20 Themen, 1.038 Begriffe pro Sprache; V4 übernimmt bestehende Classic-/Quick-Partien und verbrauchte Wortpaare. 36 Session-Szenarien beim Implementer sowie separate Migration und 1.040 persistierte Ziehungen beim Merger bestanden. [Content-Integration](../validation/bilingual-content-merge.md). Ticket 6 ist nach 19 kombinierten gerenderten Tests und der [Android-Abnahme von Migration, Neustart und Sprachwechsel](../validation/android-recovery-combined/report.md) abgenommen. Die ursprünglichen Mehrfinger- und Systemgestenfälle funktionieren im Gegencheck. Der vermeintliche Kaltstart-/Abbruchfehler wurde durch sichtbare Bereitschaft vor Eingaben und gehaltene statt zeitloser Back-Tasten aufgeklärt; der korrigierte Ablauf funktioniert ohne Produktionsänderung. Ticket7 ist damit abgenommen. Der UI-/Playtest-Feinschliff in Ticket 8 ist auf `7404d17` abgenommen und mit `2423343` separat integriert. Die [Integrationsprüfung](../validation/ui-polish-merge.md) bestätigt 53/53 Session-Fälle einschließlich aller 766 Unicode-Konformitätsfälle und unveränderte APK-Quellen.



## UI- und Bedienungsprüfung

Ticket 8 verbessert echte Android-Dichte, Schrift-/Emojiabdeckung, sichere öffentliche Accessibility-Menüs, große Schrift, kompakte Gruppenverwaltung und Kartenidentität. [Prüfbericht](../validation/ui-polish/report.md), [betrachtete Ansichten](../validation/ui-polish/visual-review.md), [aktualisierte Abnahmematrix](../validation/acceptance-matrix.md). Der komplette PlayMode-Lauf bestand mit 25/25 Fällen; nach den folgenden gezielten Änderungen wurden sieben UI-, fünf Gruppen- und die fünf Quick-/fünf Recoveryfälle erneut geprüft. Die Berichte nennen die jeweiligen Stände, statt einen nicht ausgeführten identischen Gesamtlauf zu behaupten.

Native Updates behalten die beiden bestätigten V4-Generationen bytegenau. Gespeicherte Gruppen werden vor temporären Fixtures gesichert und danach exakt wiederhergestellt. APKs sind weiterhin Development-Prüfstände; Releasebau, 16-KiB-Verpackung, Signierung, finale Latenz, Storeunterlagen und Reviews gehören zu Ticket 9. Physische Bedienung und echte Gruppendynamik bleiben von Emulatornachweisen getrennt.
