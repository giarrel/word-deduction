# Unity-/Android-Setup: Prüfung und Wiederverwendung

Geprüft am 6. Oktober 2026 anhand des Chats **Nischenreich**, seiner Projektdateien, einer vorhandenen APK, der lokalen Werkzeuge und Herstellerdokumentation. Die Prüfung änderte weder Nischenreich noch die Unity-Installation.

## Ergebnis

Das für Android geeignete Setup ist bereits vorhanden und kann für Word Deduction wiederverwendet werden. Es ist keine erneute Installation von Unity, SDK, NDK oder JDK pro Projekt erforderlich. Als gemeinsame Ausgangsversion bietet sich die bereits für Nischenreich verwendete **6000.3.25f1** an.

Die frühere Recherche hatte ausschließlich die Installation **6000.6.3f1** unter `Program Files` betrachtet. Dort fehlt Android-Unterstützung tatsächlich. Die zusätzliche Installation unter dem Benutzerprofil wurde erst bei diesem projektübergreifenden Check gefunden. Damit war die frühere Aussage zum insgesamt verfügbaren Android-Setup unvollständig.

## Tatsächlich gefundener Stand

| Komponente | Beobachtung |
| --- | --- |
| Unity für Nischenreich | `6000.3.25f1`, stimmt mit `game/ProjectSettings/ProjectVersion.txt` überein. |
| Editorpfad | `%LOCALAPPDATA%\Unity\Editors\6000.3.25f1\Editor\Unity.exe` im normalen Benutzerkonto. |
| Android-Unterstützung | AndroidPlayer, SDK, NDK und OpenJDK sind vorhanden; die CLI führt alle drei Android-Module auf. |
| NDK | `27.2.12479018` / r27c laut `NDK/source.properties`. |
| Java | OpenJDK `17.0.18`; `java -version` erfolgreich. |
| Android-Werkzeuge | `adb version` erfolgreich, Platform Tools `36.0.0`; mehrere SDK-Plattformen einschließlich API 36 vorhanden. |
| Zweiter Editor | `6000.6.3f1` unter `%ProgramFiles%\Unity\Hub\Editor`, von der CLI mit Web-Modul aufgeführt. Für diesen Auftrag kein Anlass, ihn umzurüsten. |
| Unity CLI | `1.0.0-beta.11`; `unity` ist über den Benutzer-PATH auch außerhalb von Nischenreich aufrufbar. |
| CLI-Speicherort | Noch im Nischenreich-Unterordner `work/toolchain/unity-cli/bin/unity.exe`. |
| Editor-Anbindung in Nischenreich | `com.unity.pipeline` Version `0.8.0-exp.1` in `game/Packages/manifest.json`; vorhandene Werkzeuge nutzen `unity command ... --project-path ...`. |
| Laufender Editor | Im normalen Benutzerkontext kein Unity-/Unity-Hub-Prozess und keine aktive Pipeline-Instanz gefunden. Eine aktuelle Live-Verbindung ist daher nicht nachgewiesen. |
| Anmeldung/Lizenz | CLI meldet `loggedIn: true`, aber `sessionState: stale`. Die Lizenzabfrage listet `Unity Personal` als `Assigned`; das Ablauf-Feld ist leer. Dies ist kein Nachweis einer frisch validierten Online-Sitzung oder eines aktuellen Lizenzstarts im Editor. |

Die erforderlichen Android-Komponenten entsprechen dem [Unity-Setup](https://docs.unity3d.com/6000.3/Documentation/Manual/android-sdksetup.html). Die [versionsabhängigen Vorgaben](https://docs.unity3d.com/6000.3/Documentation/Manual/android-supported-dependency-versions.html) sind bei einem späteren Editorwechsel erneut zu prüfen. Für neue Projekte sollten die zum Editor gehörenden Werkzeuge über die [Android External Tools](https://docs.unity3d.com/6000.3/Documentation/Manual/android-external-tools-reference.html) verwendet werden.

## Wie Nischenreich seine APK baut

Im Nischenreich-Repo liegt ein eigener Editor-Buildeinstieg unter `game/Assets/Editor/ProjectBootstrap/AndroidBuild.cs`:

- `Nischenreich.Editor.AndroidBuild.Development` erzeugt eine Entwicklungs-APK.
- Separate Methoden erzeugen Entwicklungs- und Release-App-Bundles.
- Der Build verlangt das Android-Ziel und einen gestoppten Spielmodus, verwendet aktivierte Szenen und einen frischen Build-Cache und schreibt einen Buildbericht.
- Entwicklungssignierung und extern bereitgestellte Release-Signierung sind getrennt. Vorübergehend veränderte Player-/Qualitätseinstellungen werden wiederhergestellt.

Für Word Deduction ist dieses Muster wiederverwendbar. Spielname, Paketkennung, Szenen und projektspezifische Prüfungen brauchen eigene Werte; der komplette Nischenreich-Buildhelfer sollte nicht ungeprüft kopiert werden.

Konkreter vorhandener Nachweis im Nischenreich-Repo:

```text
artifacts/android/20261004-144430/Nischenreich-development.apk
55 146 642 Byte
SHA-256: ff86d54b49fccdd6bb08133d2dffc483cfaffda18f4a57fca3233f0b1e5bef9b
```

Datei, Größe und Hash wurden in dieser Runde erneut geprüft und stimmen mit `docs/validation/resource-tiles.md` überein. Dort sind ARM64/IL2CPP und ein erfolgreicher Build dokumentiert. Weitere Build-/Verpackungsnachweise stehen in `docs/validation/android-packaging.md` und dessen Evidence-Datei. Heute wurde kein neuer Build und kein Android-Gerätetest durchgeführt.

## Einmalig sinnvoll ordnen

**Editor und Android-Werkzeuge können an ihrem jetzigen zentralen Benutzerpfad bleiben.** Die konkrete Wartungsstelle ist die CLI: Ein globaler PATH sollte dauerhaft auf einen Benutzer-Werkzeugordner zeigen, nicht auf ein einzelnes Projekt unter `work/`.

Vorschlag für eine spätere einmalige Bereinigung: die bestehende CLI in einem festen Ort wie `%LOCALAPPDATA%\Programs\UnityCLI\bin` bereitstellen, den Benutzer-PATH darauf umstellen und aus einer frischen Shell sowie einem anderen Projekt `unity --version` und die Editorerkennung prüfen. Dabei den bisherigen Pfad zunächst behalten: Nischenreich enthält Skripte mit relativen Verweisen auf `work/toolchain/unity-cli/bin/unity.exe`. Erst nach Anpassung und Prüfung dieser Aufrufer wäre die alte Kopie entbehrlich. Im vorliegenden Prüfauftrag wurde nichts verschoben, installiert oder am PATH geändert.

Für Word Deduction fällt später einmalige **Projekteinrichtung** an: Editorversion festhalten, Projekt anlegen, vorhandenen Pipeline-/CLI-Weg gezielt übernehmen, einen eigenen APK-Buildeinstieg einrichten und einen ersten Build prüfen. Die Codex-seitigen Unity-Plugins müssen dafür nicht nochmals installiert werden. Die Pipeline-Anbindung gehört zum jeweiligen Unity-Projekt; eine Plugin-Installation allein erzeugt sie nicht automatisch in einem neuen Repo.

Eine zusätzliche MCP-Server-Anbindung ist für den vorhandenen CLI-Arbeitsweg nicht Voraussetzung. In diesem Chat sind aktuell keine eigenen Unity-MCP-Tools verfügbar. Nischenreich wird nachweislich über CLI und Pipeline gesteuert; ein zweiter paralleler Bridge-Anbieter ist dafür nicht nötig.

## Mögliche Handgriffe des Nutzers

- **Keine Neuinstallation erforderlich.** Editor und Android-Werkzeuge sind vorhanden.
- Falls beim nächsten Editorstart oder einem CLI-Cloud-Vorgang eine Anmeldung verlangt wird, die bestehende Unity-Anmeldung erneuern. Der CLI-Status `stale` allein beweist nicht, dass ein lokaler Build daran scheitert; lokale Lizenzierung und Cloud-Sitzung wurden getrennt betrachtet.
- Eine eventuell verlangte interaktive Lizenz-/Clientfreigabe würde beim ersten tatsächlichen Verbindungsversuch konkret benannt. In dieser Runde erschien keine solche Aufforderung.

## Kurzer wiederverwendbarer Prüfpfad

```powershell
Get-Command unity
unity --version
unity editors --installed --format json
unity status --format json
```

Die Editorliste und der Benutzer-PATH müssen im tatsächlichen Benutzerkontext geprüft werden: Die Codex-Sandbox zeigte nur den Editor unter `Program Files` und warnte bei der Prozessprüfung. Dieselben lesenden Befehle im normalen Benutzerkontext fanden zusätzlich die vollständige Android-Installation. Eine leere Sandbox-Liste darf deshalb keine Neuinstallation auslösen.

Für spätere Editorbefehle den Projektpfad explizit übergeben, damit Nischenreich und Word Deduction nicht verwechselt werden. Editorversion und Paketversionen werden im jeweiligen Repo festgehalten; zusätzliche SDK-/Java-Installationen pro Projekt sind im geprüften Aufbau nicht vorgesehen.
