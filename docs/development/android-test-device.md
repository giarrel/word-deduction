# Wiederverwendbares Android-Testgerät

Stand: 6. Oktober 2026, 20:44 MESZ. Für autonome Laufzeittests wurde ein eigenes AVD eingerichtet. Ein ARM64-Unity-Player ist inzwischen installiert und gestartet; die erste Gruppenoberfläche befindet sich im Playtest, noch nicht in der Gesamtabnahme.

## Installierte Werkzeuge

Separates Benutzer-SDK: `C:/Users/lucac/AppData/Local/Android/Sdk`. Installiert über Googles SDK Manager: Emulator **37.2.12**, Platform Tools, Command-line Tools **16.0** und `system-images;android-36;google_apis;x86_64`, **Revision 7**. Die Standard-Android-SDK-Lizenz wurde gezielt für diese Installation angenommen; der erste Versuch ohne Bestätigung hatte alle Pakete übersprungen. Das für Unity-Builds verwendete SDK/NDK/JDK bleibt in dessen Editorinstallation. Keine Windows-Funktion, kein zusätzlicher Hypervisortreiber und kein Rechnerneustart waren erforderlich.

AVD: `word_deduction_api36`, Profil `pixel_6`, Verzeichnis `C:/Users/lucac/AppData/Local/Android/avd/word_deduction_api36.avd`. Durch prozesslokales `ANDROID_AVD_HOME` wird dieses Verzeichnis gezielt gewählt. Der eigene Command-line-Tools-Pfad im separaten SDK vermeidet eine Besonderheit des vorhandenen Unity-`avdmanager`: Er leitet seinen SDK-Pfad aus dem Toolsverzeichnis ab; allein `ANDROID_HOME` umzusetzen reicht dort nicht zuverlässig.

## Tatsächlich beobachtete Evidenz

- `emulator -accel-check`: Exit 0, `WHPX(10.0.26200) is installed and usable.`
- Start mit `-accel on`; Emulatorlog bestätigt `Windows Hypervisor Platform accelerator is operational`.
- ADB-Status: `emulator-5580 device`.
- `sys.boot_completed`: `1`; Bootdauer laut Log: **72 828 ms**.
- `ro.build.version.sdk`: `36`.
- `ro.product.cpu.abilist64`: `x86_64,arm64-v8a`.
- `ro.dalvik.vm.native.bridge`: `libndk_translation.so`.
- Bildschirm: **1080×2400**, Dichte **420**. Ein echter Screenshot wurde per `screencap` und `adb pull` aufgenommen und betrachtet; er zeigt den vollständig gerenderten Android-Startbildschirm.
- [Maschinenlesbarer Bootnachweis](../validation/evidence/android-test-device-boot.json). Rohlogs, Launch-PID und Screenshot liegen während der Arbeit außerhalb des Repos unter `work/android-runtime-probe/` im Chat-Arbeitsverzeichnis.

Die vorausgehende WMI-Ausgabe (`HypervisorPlatform InstallState: 2`) ist gegenüber diesem direkten Funktionsnachweis nachrangig. Sie darf künftig keinen unnötigen Windows-Umbau auslösen. Ebenso ist die Magic-Leap-x86_64-Einschränkung in Unity 6.3 keine nachgewiesene allgemeine Buildsperre: Die [Release-Notes](https://unity.com/releases/editor/whats-new/6000.3.0f1) nennen verbleibende Nutzung, und die x86_64-Development-Runtime ist lokal vorhanden. Ein tatsächlicher x86_64-Projektbuild wurde bisher nicht versucht.

## Wieder starten und prüfen

Zuerst mit ADB und den Prozessen prüfen, ob dieses AVD bereits läuft. Ein Beobachtungs-Timeout ist kein abgestürzter Emulator. Nicht parallel eine zweite Instanz desselben AVD starten.

```powershell
$androidTestSdk = 'C:/Users/lucac/AppData/Local/Android/Sdk'
$env:ANDROID_HOME = $androidTestSdk
$env:ANDROID_SDK_ROOT = $androidTestSdk
$env:ANDROID_AVD_HOME = 'C:/Users/lucac/AppData/Local/Android/avd'
& "$androidTestSdk/platform-tools/adb.exe" devices -l
& "$androidTestSdk/emulator/emulator.exe" -accel-check
```

Für weitere Tests den inzwischen geprüften Host-Grafikmodus verwenden:

```text
-avd word_deduction_api36 -accel on -gpu host -cores 2 -memory 2048
-no-snapshot -no-window -noaudio -no-boot-anim
-camera-back none -camera-front none -port 5580
```

Windows-Prozessstart über `Start-Process -WindowStyle Hidden`, mit getrennten stdout-/stderr-Logs und aufgezeichnetem PID. Die CPU ist durch WHPX beschleunigt; die Grafik nutzt die vorhandene NVIDIA GeForce RTX 3080 Ti. Der Launcher kann einen eigenen QEMU-Kindprozess haben; beide gehören zum beobachteten Prüfgerät.

### Nachgewiesener Grafikvergleich

Der ursprüngliche Bootnachweis entstand mit `-gpu software` (SwiftShader, GLES 3.0). Dort waren sämtliche Unity-UI-Toolkit-Schriftzeichen gefüllte Rechtecke. Ein isolierter App-Build mit statischem statt dynamischem Fontatlas änderte das nicht. Nach einem gezielten Neustart desselben AVD mit `-gpu host` renderten **beide identischen APKs** vollständig lesbar: zuerst der statische Versuch, danach die ursprüngliche dynamische Font-Baseline. Alle vorhandenen Testdaten blieben erhalten. Unity meldet `Android Emulator OpenGL ES Translator (NVIDIA GeForce RTX 3080 Ti/PCIe/SSE2)`, GLES 3.1. Damit ist ein Einfluss des Emulator-Grafikpfads belegt; ein genereller Android-Fontdefekt ist nicht belegt. Keine dauerhafte Font-Umstellung allein aus dem SwiftShader-Bild ableiten.

APK-Baseline: SHA-256 `32079d14bec9dfd5bc121f593dcc6845ba457b0a508a020b2042d728c4f0bb35`, 37,500,646 Bytes, Unity 6000.3.25f1, IL2CPP/ARM64, Entwicklung. Font-Vergleichsbuild: `49834bd14b8b760fac4a1cefa2df8eb073d9041d94f9007b8c60f0e98d5a1582`. Paket `com.giarrel.worddeduction`, min26/target36. Rohbelege einschließlich `13-host-gpu-dynamic-baseline.png` und Launchdaten liegen während der Arbeit unter `work/android-group-playtest/` im Chat-Arbeitsverzeichnis. Die Gruppenabnahme dokumentiert die konkreten Interaktionen getrennt.

Der erste Appstart unmittelbar während des Host-AVD-Boots wurde vom Activity-Lebenszyklus beendet; ein erneuter Start nach abgeschlossenem Boot zeigte die Oberfläche. `sys.boot_completed=1` allein ist deshalb kein Beleg für einen bereits ruhigen Testzustand. Erst den tatsächlich angezeigten Bildschirm prüfen. Die von `am start -W` gemeldete Activity-Zeit ist außerdem keine Messung der Zeit bis zur bedienbaren Unity-Oberfläche.

Alle Gerätebefehle explizit mit `adb -s emulator-5580 ...` ausführen. APK anhand des tatsächlichen Buildpfads und Hashes installieren. Screenshots per Gerätedatei und `adb pull` übertragen, nicht binär durch PowerShell-Textumleitung. Für Persistenz-/Updateprüfungen keine Daten löschen und keine Neuinstallation statt Update vortäuschen. Beenden bei Bedarf gezielt mit `adb -s emulator-5580 emu kill`.

## Aussagegrenzen

Die ARM64-APK startet über die angebotene Übersetzung und lässt echte Android-Touch-/Tastaturaktionen zu. Ein erfolgreicher übersetzter Lauf belegt keine physische ARM-Hardware, Haptik, OEM-Tastatur oder echte Gruppendynamik. Die installierte APK und geprüften Interaktionen werden deshalb jeweils separat dokumentiert. Herstellerreferenzen: [Emulator starten](https://developer.android.com/studio/run/emulator-commandline), [Beschleunigung](https://developer.android.com/studio/run/emulator-acceleration), [ADB](https://developer.android.com/tools/adb).
