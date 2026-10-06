# Android-Release und eigener Playtest: belastbarer Prüfpfad

Nachtrag aus der Umsetzung: Ein separater Android-16-Testemulator wurde inzwischen erfolgreich installiert und mit nutzbarem WHPX gestartet. [Aktueller Prüfzugang](../development/android-test-device.md). Die nachfolgende Bestandsaufnahme dokumentiert den vorherigen Recherchezeitpunkt; Aussagen über damals fehlenden Emulator sind keine aktuelle Blockade.

Stand: 6. Oktober 2026. Recherche für die Unity-Version von Word Deduction: ein herumgereichtes Telefon, Englisch/Deutsch, Schnellspiel und mehrstufiges Spiel. Herstellerquellen und lesende lokale Prüfung; kein Editor, Emulator oder Build wurde für diese Recherche gestartet. `adb devices` startete seinen üblichen Hintergrunddienst und meldete keine Geräte. Empfehlungen sind als solche gekennzeichnet und noch kein Nachweis, dass die neue App diese Prüfungen bestanden hat.

## Entscheidungsvorschlag

Den vorhandenen Unity-Editor **6000.3.25f1** verwenden. Zuerst eine kleine ausführbare Strecke für Spieler hinzufügen → Wortkarte ziehen/loslassen → nächste Person → App unterbrechen/wiederaufnehmen bauen und prüfen. Erst danach sämtliche Bildschirme ausarbeiten. Für dieses fast vollständig aus zweidimensionalen Menüs bestehende Spiel ist **UI Toolkit** eine sinnvolle erste Wahl: runde Formen, Layout und kurze Bewegungen lassen sich ohne Grafikpakete ausdrücken. Ein zwingender echter räumlicher Karten-Flip wäre ein Argument für uGUI; der Nutzerwunsch kann aber als hochwertige 2D-Kartenbewegung umgesetzt werden. Diese Wahl ist eine projektbezogene Abwägung, keine Behauptung, Unity empfehle UI Toolkit pauschal für jede Runtime.

Die automatische Schleife soll drei unterschiedliche Nachweise erzeugen: Regeln/Persistenztests, tatsächlich gerenderte und kritisch betrachtete UI-Szenarien, installierter Android-Build. Ein bestandener Editor-Test oder ein schöner Screenshot allein macht die App noch nicht veröffentlichungsfähig.

## 1. Aktuelle Plattformanforderungen

| Gegenstand | Verifizierter Befund und Konsequenz |
| --- | --- |
| Target SDK | Neue Apps und Updates müssen seit **31. August 2026 API 36 / Android 16 oder höher** adressieren. Target 36 explizit setzen und im fertigen Manifest nachprüfen; eine ältere API-35-Vorlage genügt heute nicht. Das Target ist unabhängig von der unterstützten Mindestversion. [Google Play Target API](https://developer.android.com/google/play/requirements/target-sdk) |
| Minimum SDK | Unity 6.3 nennt **Android 7.1 / API 25+** als Laufzeitminimum, ARMv7 oder ARM64, OpenGL ES 3.0+ oder Vulkan. Empfehlung: zunächst minSdk 26, damit Unitys mobile Accessibility-Grundlage zum gesamten unterstützten Bereich passt; das ist eine Produktentscheidung, keine Play-Vorschrift. [Unity-Systemanforderungen](https://docs.unity3d.com/6000.3/Documentation/Manual/system-requirements.html), [Unity mobile Accessibility](https://docs.unity3d.com/6000.0/Documentation/Manual/mobile-accessibility.html) |
| 64 Bit | Die App enthält mit Unity native Bibliotheken. Für jede angebotene 32-Bit-ABI muss die entsprechende 64-Bit-ABI enthalten sein. Empfehlung: zunächst ARM64/IL2CPP; tatsächliche Bibliotheken im fertigen Paket prüfen. [Android 64 Bit](https://developer.android.com/google/play/requirements/64-bit) |
| App Bundle | Google Play verlangt für neue Apps AAB. Eine APK dient Installation und lokalen Tests; zusätzlich einen Build ohne Development-Flags als Release-AAB vorbereiten. [Unity: Google-Play-Veröffentlichung](https://docs.unity3d.com/6000.3/Documentation/Manual/android-distribution-google-play.html) |
| 16-KB-Seiten | Die aktuell abgerufene Android-Seite verlangt Unterstützung für Apps mit Target 35+ auf 64-Bit-Geräten und nennt **1. Februar 2027** für das Blockieren inkompatibler Updates. Nicht ungeprüft ältere Fristen zitieren. Unabhängig von der Frist ist Kompatibilität ein Releasekriterium für diese neue App. ZIP-Alignment des APK, ELF-Segmente aller `.so` und Bundle-Konfiguration prüfen; ein erfolgreicher Build beweist diese drei Eigenschaften nicht. NDK r27 ist nicht automatisch inkompatibel: passende Linkeroptionen können 16-KB-ELF erzeugen. Nicht allein deswegen den von Unity gebündelten NDK austauschen. [Android-Seitengrößen](https://developer.android.com/guide/practices/page-sizes) |
| Android 16 UI | Edge-to-edge lässt sich bei Target 36 auf Android 16 nicht mehr abschalten. Predictive Back ist standardmäßig aktiv; `KEYCODE_BACK`/`onBackPressed` werden dabei nicht mehr wie zuvor zugestellt. Safe Areas, Tastatur und Zurück-Geste früh auf Android prüfen. Für die veränderten Großbildschirmregeln enthält die Dokumentation eine Ausnahme für als Spiel deklarierte Apps; dennoch flexible Layouts bauen. [Android-16-Verhalten](https://developer.android.com/about/versions/16/behavior-changes-16) |

Praktische Paketprüfungen, mit konkreten Artefaktpfaden im späteren Buildskript:

```text
zipalign.exe -v -c -P 16 4 WordDeduction.apk
apksigner.bat verify --verbose --print-certs WordDeduction.apk
java -jar bundletool-all-1.17.2.jar validate --bundle=WordDeduction.aab
java -jar bundletool-all-1.17.2.jar dump config --bundle=WordDeduction.aab
llvm-objdump.exe -p <extrahierte-native-bibliothek.so>
```

Beim Bundle `PAGE_ALIGNMENT_16K`, bei ELF-Ladesegmenten mindestens `2**14` prüfen. Diese Befehle sind geplante Prüfungen, in dieser Recherche nicht an einem Word-Deduction-Build ausgeführt. Bundle-/APK-Prüfung und Lauf auf einem 16-KB-Gerät sind getrennte Nachweise. [Android-Prüfanleitung](https://developer.android.com/guide/practices/page-sizes)

## 2. Persistenz und geheime Informationen als feste Invarianten

Android darf Hintergrundprozesse beenden; ein abschließendes `onDestroy` ist nicht garantiert. Unity warnt ebenfalls ausdrücklich, mobilen Spielstand nicht erst in `OnApplicationQuit` zu sichern. [Android-Prozesslebenszyklus](https://developer.android.com/guide/components/activities/process-lifecycle), [Unity OnApplicationQuit](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/MonoBehaviour.OnApplicationQuit.html)

Folgende Festlegungen sind daraus abgeleitete Projektanforderungen:

1. **Jede bestätigte Änderung ist dauerhaft.** Hinzufügen, Umbenennen, Pausieren, Entfernen und Spielentscheidungen werden vor einer Erfolgsmeldung gespeichert. Kleine vollständige Snapshots mit Schema-Version, temporärer Datei und geprüfter Wiederherstellung verwenden. Fehler injizieren: Schreibfehler, abgeschnittene Datei, altes Schema, Neustart zwischen Schreiben und Ersetzen. Keine Erfolgsmeldung bei fehlgeschlagener Speicherung.
2. **Gruppe und Partie sind getrennt.** Stabile Spieler-ID und Gruppenteilnahme bleiben über Spiele erhalten. Der aktive Match-Snapshot enthält feste Spieler, Rollen und Wortpaar; Laden würfelt nichts neu aus. Ausscheiden verändert nicht die gespeicherte Gruppe. Bei Spielende sind die Gruppenmitglieder für das nächste Spiel wieder verfügbar.
3. **Anzeigegeheimnis wird nicht wiederhergestellt.** Nach Start, Resume, Screenwechsel, Fokusverlust, Abbruch einer Berührung und Pointer-Capture-Verlust ist die Karte geschlossen. Das gespeicherte Match bleibt gleich, aber der Spieler muss sein Wort erneut bewusst aufdecken. Beim Loslassen wird der Wortinhalt sofort verborgen; die dekorative Rückbewegung darf danach auslaufen.
4. **Lifecycle-Signale ergänzen die Speicherung, ersetzen sie nicht.** Sowohl Pause als auch Fokusverlust behandeln. Androids Bildschirmtastatur kann allein einen Fokusverlust auslösen; Home bei geöffneter Tastatur kann stattdessen nur Pause auslösen. Beim Bearbeiten eines Namens darf die Tastatur deshalb nicht die Gruppe verlassen oder die Bearbeitung verwerfen. [Unity OnApplicationPause](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/MonoBehaviour.OnApplicationPause.html)
5. **Keine Nebenkanäle in der UI.** Verdeckte Wörter nicht in Accessibility-Labels, Logs, Analyseereignisse oder öffentliche Ergebnisvorschauen schreiben. Für eine Undercover-Rolle nur das Wort zeigen, wenn die gewählte Regel ihre Identität geheim halten soll. Keine Debug-Rollenliste in der Veröffentlichung.

`Application.persistentDataPath` ist Unitys vorgesehener persistenter Speicherort; auf Android wird normalerweise `getExternalFilesDir` verwendet. Konstante Paketkennung ist für den Zugriff nach Updates wesentlich. Aktualisierung mit identischer Signatur testen, nicht nur Neuinstallation. App-Daten-Löschen oder Deinstallation ist ein anderer Vorgang als Wiederaufnahme und darf nicht als Persistenzfehler interpretiert werden. [Unity persistentDataPath](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Application-persistentDataPath.html)

**Empfehlung zur Android-Vorschau:** Während einer Partie mit geheimen Rollen `FLAG_SECURE` verwenden und den Reveal bei Pause/Fokusverlust zusätzlich schließen. Das Flag blockiert System-Screenshots und nicht sichere Displays; es ist kein Ersatz für den UI-Zustandswechsel. Damit lassen sich Spielgeheimnisse auch gegen die normale Aufgabenübersicht abschirmen. Menüs und fertige Ergebnisse können separat screenshotfähig bleiben. Den Wechsel und die tatsächlich sichtbare Aufgabenübersicht auf Android kontrollieren. [Android FLAG_SECURE](https://developer.android.com/security/fraud-prevention/activities?hl=en)

**Backup bewusst festlegen:** Androids Auto Backup kann App-Daten außerhalb der laufenden Installation sichern. Eine offline arbeitende App ist deshalb nicht automatisch identisch mit „diese Dateien verlassen niemals das Gerät“. Empfehlung: laufende Partie/Rollenzuordnung vom Backup ausschließen; gespeicherte Gruppen nur mit dokumentierter Produktentscheidung einbeziehen. Im fertigen Manifest und in Backup-Regeln prüfen. [Android Auto Backup](https://developer.android.com/identity/data/autobackup)

## 3. UI Toolkit, uGUI und Bedienbarkeit

Unity 6.3 nennt uGUI als allgemeine Runtime-Empfehlung, führt UI Toolkit aber ausdrücklich für umfangreiche, auf mehrere Auflösungen ausgelegte Menü-UIs auf. Beide bieten Text, adaptive Layouts und Input-System-Unterstützung; keyframed Animationen werden in der Vergleichstabelle uGUI zugeordnet. [Unity UI-Vergleich](https://docs.unity3d.com/6000.3/Documentation/Manual/UI-system-compare.html)

| Kriterium dieses Spiels | Projektbezogene Empfehlung |
| --- | --- |
| Runde Buttons, weiche Karten, wenige Grafiken | UI Toolkit: USS-Radien und einheitliche Designwerte statt vieler einzeln gezeichneter Assets. Border-Radius ist dokumentiert. [USS-Eigenschaften](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-USS-SupportedProperties.html) |
| Hochziehen und Zurückschnappen | Drag direkt an einer Transformation führen, danach kurze zeitbasierte Rückbewegung. Layout nicht jeden Frame durch Größenänderungen neu berechnen. Unity empfiehlt für flüssige USS-Transitions Transform-Eigenschaften. [USS-Transitions](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-Transitions.html) |
| Deutscher Text und lange Namen | Layout, Zeilenumbruch, Scrollbereich und Typografie anhand echter deutscher Inhalte testen; nicht einfach die englische Textlänge als Maß nehmen. Keine verkleinerten unlesbaren Texte als Reparatur. |
| Native Tastatur | Früher Geräte-Spike für Namen: Umlaute, ß, Leerzeichen, Einfügen, Rücktaste, Enter/Done, Tastatur schließen, Home während Bearbeitung. UI Toolkit verfügt über `keyboardType`/`hideMobileInput`; die API-Existenz ersetzt diesen Plattformtest nicht. Siehe lokale Modulprüfung unten und [TextField-API](https://docs.unity3d.com/cn/6000.0/ScriptReference/UIElements.TextField.html). |
| TalkBack | Eigene semantische Accessibility-Hierarchie mit Rollen, Labels und Aktionen. Unity stellt `AssistiveSupport` bereit; Menü-Beschriftungen werden nicht allein durch hübschen sichtbaren Text nachgewiesen. Geheime Begriffe niemals automatisch vorlesen. Geplante screenreaderfähige Alternative zur Ziehgeste separat testen. [Unity Accessibility](https://docs.unity3d.com/6000.0/Documentation/Manual/mobile-accessibility.html) |
| Körperliche Bedienbarkeit | Große Ziele, logische Reihenfolge und Unterstützung assistiver Bedienung sind Teil der Android-Empfehlungen. Projektziel: primäre Ziele mindestens 48 dp, keine ausschließlich farbcodierten Rollen oder winzigen Entfernen-Symbole. Echte Display-Skalierung messen, nicht Unity-Pixel pauschal als dp ausgeben. [Android Accessibility](https://developer.android.com/guide/topics/ui/accessibility/apps) |

Visuelles Projektziel: eine klare Hauptaktion pro Bildschirm; Gruppe auf dem Startbildschirm, zwei verständliche Modusoptionen, brauchbare Defaults, Regeln bei Bedarf. Rundungen und Bewegung dienen der Rückmeldung. Kein Intro, keine Kontopflicht, keine freizuschaltenden Wortpakete oder unnötige Bestätigungsdialoge. Das sind Nutzeranforderungen und Designableitungen, keine Herstellerpflichten.

## 4. Tatsächlich vorhandene lokale Testmittel

Die folgenden Beobachtungen erfolgten im normalen Windows-Benutzerkontext mit lesenden Befehlen. Sandbox-Sicht allein wäre für diesen Rechner unzureichend, wie der frühere Setup-Check zeigte.

| Prüfung | Ergebnis am 6. Oktober 2026 |
| --- | --- |
| Editor-/SDK-Basis | `C:/Users/lucac/AppData/Local/Unity/Editors/6000.3.25f1/Editor`; weitere Versions-, JDK- und NDK-Nachweise im [Unity-Setup](../development/unity-setup.md). |
| Unity CLI | Vorhandene CLI unter `C:/Users/lucac/Documents/Codex/2026-09-27/wenn-ich-digitalversion-von-domination-species/work/toolchain/unity-cli/bin/unity.exe`. `test --help` unterstützt EditMode/PlayMode, Filter, Timeout und NUnit/JUnit-Reports. Die lokale Skill-Referenz erklärt: Editor-Testlauf direkt, ohne Pipeline-Server. |
| Test Framework | Nischenreichs `game/Library/PackageCache/com.unity.test-framework@bd7f943e9647/package.json` meldet Version **1.6.0**. Das bedeutet Verfügbarkeit im vorhandenen Cache, noch keine Installation im neuen Projekt. |
| Android-Geräte | Gebündeltes `SDK/platform-tools/adb.exe devices -l`: leere Geräteliste. Der Befehl startete seinen Standard-ADB-Dienst auf Port 5037. Kein angeschlossenes autorisiertes Telefon oder laufender Emulator nachgewiesen. |
| Emulator | Kein `emulator/emulator.exe` im Unity-SDK oder unter `%LOCALAPPDATA%/Android/Sdk`; kein `%USERPROFILE%/.android/avd`; kein Android Studio am Standardpfad `C:/Program Files/Android/Android Studio/bin/studio64.exe`. `Get-Command emulator,adb` fand keine zusätzlichen PATH-Programme. Das ist eine gezielte Standardpfadprüfung, keine Vollsuche jeder Festplatte. |
| APK-Prüfung | `SDK/build-tools/36.0.0/zipalign.exe`, `apksigner.bat`, `aapt2.exe` vorhanden. |
| AAB-Prüfung | `AndroidPlayer/Tools/bundletool-all-1.17.2.jar` vorhanden. |
| ELF-Prüfung | `NDK/toolchains/llvm/prebuilt/windows-x86_64/bin/llvm-objdump.exe` vorhanden. |
| SDK-Paketmanager | `SDK/cmdline-tools/16.0/bin/sdkmanager.bat` vorhanden; keine Pakete installiert oder Lizenzen bestätigt. |
| Accessibility | `Data/Managed/UnityEngine/UnityEngine.AccessibilityModule.dll` und XML vorhanden. XML enthält `AccessibilityHierarchy`, `AccessibilityNode`, `AssistiveSupport`. UIElements-Modul-XML enthält mobile Textfeldoptionen und Pointer-Capture-Verlustereignis. Keine Laufzeitfunktion auf Android daraus behauptet. |

## 5. Autonome Implementierungs- und Verbesserungsschleife

### A. Fachregeln und Speicherung

EditMode-Tests über die öffentliche Schnittstelle des Regelkerns: gültige Gruppengrößen, beide Modi, alle Siegbedingungen, Gleichstände, Mr.-White-Versuch, Ausscheiden, Wiederholungsspiel, Wortauswahl ohne frühe Wiederholung. Dazu echte Datei-Roundtrips mit frischer Repository-Instanz, unterbrochene Schreibvorgänge, Migration und Wiederherstellung. Diese Tests dürfen nicht nur interne Hilfsmethoden spiegeln.

```text
unity test <Projektpfad> --mode EditMode --editor-version 6000.3.25f1 --output <Artefaktpfad>/editmode.xml --timeout 600 --format json
unity test <Projektpfad> --mode PlayMode --editor-version 6000.3.25f1 --output <Artefaktpfad>/playmode.xml --timeout 600 --format json
```

Die CLI-Syntax wurde lokal geprüft. Unitys dokumentierter Test Runner unterscheidet Tests im Editor und Tests in einem für die Zielplattform gebauten Player. **PlayMode im Editor ist kein Android-Test.** CLI-Hilfe bietet nur EditMode/PlayMode; für Android-Player-Tests den tatsächlich verfügbaren Unity-Test-Runner-Weg gezielt verwenden, nicht einen erfundenen CLI-Modus. [Unity Test-Runner-Kommandozeile](https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/reference-command-line.html) Die Online-Referenz ist 1.4; lokale Paketversion ist 1.6.0, daher API/CLI-Details beim Aufbau gegen das installierte Paket prüfen.

### B. Tatsächlich gerenderte Bedienung im Editor

Ein deterministisches Playtest-Harness verwendet dieselben Presenter und Eingabewege wie die Oberfläche, mit reproduzierbaren Beispieldaten. Es navigiert über UI-Ereignisse statt jedes Zielbild nur durch direkte Zustandszuweisung zu erzeugen. Direkte Zustandsvorgaben sind für zusätzliche Bildschirmvarianten nützlich, zählen aber nicht als durchgespielter Flow.

Pro Durchlauf: tatsächliche Screenshots und Sequenzbilder der Kartenbewegung speichern, mit dem Bildwerkzeug ansehen, Auffälligkeiten festhalten, korrigieren und betroffenen Weg erneut spielen. Unity bietet `ScreenCapture.CaptureScreenshot`; Aufnahme erst nach einem fertig gerenderten Frame. Grafiktests mit aktivem Renderer ausführen, nicht mit `-nographics`. [Unity ScreenCapture](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/ScreenCapture.CaptureScreenshot.html)

Pflichtszenarien:

- Frischer Start in Deutsch/Englisch, erste Gruppe, erster vollständiger Durchlauf beider Modi.
- Bestehende Gruppe: zwei Personen hinzufügen, eine pausieren, zwei umbenennen, eine entfernen; erneute Partie; neu gestartete App.
- Kleine schmale, normale hohe und breite/tabletähnliche Viewports; Safe-Area-Simulation; große Schrift; längste Wörter und Namen; sehr lange Gruppenliste.
- Sehr kurzer Drag, normaler Drag, mehrfaches Ziehen, zweiter Finger, Finger außerhalb des Kartenbereichs, Abbruch und sofortige Folgeaktion.
- Versehentliche Zurück-Aktion, doppelte schnelle Betätigung, Hilfe öffnen/schließen, Sprachwechsel und Wiederaufnahme während einer laufenden Partie.
- Ergebnis → weiteres Spiel ohne Umweg; geänderte Gruppengröße erhält einen gültigen verständlichen Default.

Jede Runde bewertet konkret: Ist der nächste Schritt ohne Erklärung erkennbar? Wie viele Betätigungen braucht er? Verhindert etwas das schnelle Weiterspielen? Sind Bedienelemente erreichbar, Texte lesbar und Geheimnisse zuverlässig verdeckt? Blockiert eine Animation unnötig? Festgestellte Probleme mit Vorher-/Nachher-Screenshot und Regressionstest erfassen. Eine subjektive visuelle Selbstkritik bleibt als solche bezeichnet; sie ersetzt keine Rückmeldung einer realen Spielgruppe.

### C. Android-Build und installierter Test

Reproduzierbare APK und AAB mit Buildbericht, Logs, Git-Revision, Paketkennung, Versionscode und SHA-256 erzeugen. APK installieren, App starten, Aktionen ausführen, Screenshots/Logs aufnehmen. ADB dokumentiert Installation, Shell, Screenshots und Videos. Auf PowerShell für binäre Screenshots vorzugsweise `adb shell screencap <Gerätepfad>` und `adb pull` verwenden, damit Shell-Textumleitung keine PNG-Datei beschädigt. [ADB](https://developer.android.com/tools/adb)

Die Android-Matrix muss wenigstens zeigen: Neustart bei gespeicherter Gruppe; Home und Rückkehr auf offener Karte; Prozessende und Wiederaufnahme; Tastatur mit Umlauten; Zurück-Geste; Flugmodus; Update einer bestehenden Installation; Performance ohne dauerhaftes Ruckeln. Prozessbeendigung nur gegen die eigene Paketkennung, keine pauschale Gerätebereinigung. Speichern nach bestätigter Änderung und beim Hintergrundwechsel getrennt testen: `force-stop` allein bildet nicht sämtliche Low-Memory-Lifecycle-Fälle nach.

Ein Emulator kann zusätzliche Größen, Android-Versionen und 16-KB-Konfigurationen abdecken. Er braucht zunächst SDK-Emulatorpaket, System-Image, AVD und eine zum Build passende ABI; das ist momentan nicht vorhanden. Ein ARM64-APK darf nicht ungeprüft auf einem x86_64-Image als lauffähig angenommen werden. Android dokumentiert die AVD-/Kommandozeilensteuerung, Unity nennt Android in Emulatoren allerdings nicht als unterstützte Zielhardware. Deshalb Emulatorbefunde als ergänzend ausweisen. [Android-Emulator](https://developer.android.com/studio/run/emulator-commandline), [Unity-Systemanforderungen](https://docs.unity3d.com/6000.3/Documentation/Manual/system-requirements.html)

### D. Was ein Schreibtischtest nicht beweist

Handhabung beim Herumreichen, Blickschutz, tatsächliche Einhand-Erreichbarkeit, haptische Qualität, TalkBack-Sprachausgabe, OEM-Tastaturen und ein Abend mit wechselnder Gruppe verlangen physische bzw. menschliche Nutzung. Eigene automatisierte Durchläufe können Fehlbedienung finden und Flows vereinfachen, aber Spielspaß und Gruppenreaktionen nicht messen. Diese Grenzen im Releasebericht offenlassen, bis entsprechende Nachweise vorliegen; fehlende Hardware stoppt nicht die übrige Entwicklung.

## 6. Veröffentlichung vorbereiten, externe Voraussetzungen getrennt halten

Technisch vorzubereiten: Release-AAB ohne Entwicklungsfunktionen, reproduzierbarer Build, englische/deutsche Storetexte und Screenshots, Icon, Datenschutzhinweise auf Grundlage des tatsächlichen Pakets, Supportkontakt, Inhaltsbewertung und eine ausgefüllte Freigabecheckliste. Play verlangt selbst bei Apps ohne erhobene Daten ein Data-Safety-Formular und eine verlinkte Datenschutzerklärung; ausschließlich interne Testtracks sind ausgenommen. Keine „keine Daten“-Behauptung allein aus dem Wort offline ableiten: Abhängigkeiten, Manifest und gegebenenfalls SDK-Verkehr prüfen. [Data Safety](https://support.google.com/googleplay/android-developer/answer/10787469?hl=en-GB10-1), [Vorbereitung der App-Prüfung](https://support.google.com/googleplay/android-developer/answer/9859455?hl=en-EN)

Play App Signing trennt Upload-Schlüssel und von Play verwendeten App-Signierschlüssel. Für Einrichtung sind entsprechende Kontorechte nötig. Ein Debug-Schlüssel ist kein Release-Schlüssel. Produktionsschlüssel nicht aus Nischenreich übernehmen, nicht im Repository speichern und keine Passwörter in Befehlsargumenten protokollieren. Es wurden in dieser Recherche weder private Schlüssel gesucht noch Konto-Einstellungen geändert. [Play App Signing](https://support.google.com/googleplay/android-developer/answer/9842756?hl=en)

Bei persönlichen Entwicklerkonten, die nach dem 13. November 2023 erstellt wurden, verlangt Google vor dem Antrag auf Produktionszugriff einen geschlossenen Test mit mindestens **12 durchgehend 14 Tage angemeldeten Testern**. Interne Tests ersetzen das nicht. Ob diese Voraussetzung auf das Konto des Nutzers zutrifft, wurde nicht geprüft und darf nicht unterstellt werden. [Testanforderungen](https://support.google.com/googleplay/android-developer/answer/14151465)

Aktuell konkret fehlender Prüfzugang: kein von ADB erkanntes Android-Gerät, kein Emulator in den geprüften Standardpfaden. Ein verbundenes entsperrtes Telefon mit USB-Debugging und bestätigtem Rechnerzugriff würde echte Gerätetests ermöglichen. Für den Play-Release bleiben Kontotyp/Produktionszugriff, gewünschte endgültige Paketkennung und sicher verwaltete Signierung ungeprüft. Diese Punkte erst dann als konkrete Nutzeraufgabe stellen, wenn technische Vorbereitung und benötigte Auswahl klar sind; sie sind keine Gründe, Regelkern, Persistenz, UI und Buildvorbereitung liegenzulassen.

## Abnahmestufen

| Stufe | Erforderlicher Nachweis |
| --- | --- |
| Implementiert | Beide Modi, Sprache und Wortdaten über öffentliche Flows nutzbar; kein bekannter blockierender Regel-/Persistenzfehler. |
| Im Editor geprüft | Erfolgreiche relevante Tests plus selbst betrachtete echte UI-Aufnahmen; überprüfte Bedienungsverbesserungen. |
| Android-Kandidat | APK/AAB gebaut und geprüft; installierte Android-Flows, Lifecycle und Update nachgewiesen, Geräte-/Emulatorgrenzen genannt. |
| Zur Veröffentlichung bereit | Signierter Release-Build, vollständige Store-/Datenschutzunterlagen und erforderliche Konto-/Testvoraussetzungen erfüllt; offene physische oder Accessibility-Prüfungen entweder geschlossen oder präzise im Freigabeentscheid benannt. |

Keine dieser Stufen wird allein durch das Schreiben dieses Dokuments erreicht.
