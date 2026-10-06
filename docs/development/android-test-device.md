# Wiederverwendbares Android-Testgerät

Stand: 6. Oktober 2026, 20:16 MESZ. Für autonome Laufzeittests wurde ein eigenes AVD eingerichtet. Der Boot ist nachgewiesen; dies ist noch kein Nachweis einer bestandenen App-Prüfung.

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

Beim tatsächlich ausgeführten Start wurden diese Argumente verwendet:

```text
-avd word_deduction_api36 -accel on -gpu software -cores 2 -memory 2048
-no-snapshot -no-window -noaudio -no-boot-anim
-camera-back none -camera-front none -port 5580
```

Windows-Prozessstart über `Start-Process -WindowStyle Hidden`, mit getrennten stdout-/stderr-Logs und aufgezeichnetem PID. Die GPU ist per Software gerendert, die CPU ist durch WHPX beschleunigt. Der Launcher kann einen eigenen QEMU-Kindprozess haben; beide gehören zum beobachteten Prüfgerät.

Alle Gerätebefehle explizit mit `adb -s emulator-5580 ...` ausführen. APK anhand des tatsächlichen Buildpfads und Hashes installieren. Screenshots per Gerätedatei und `adb pull` übertragen, nicht binär durch PowerShell-Textumleitung. Für Persistenz-/Updateprüfungen keine Daten löschen und keine Neuinstallation statt Update vortäuschen. Beenden bei Bedarf gezielt mit `adb -s emulator-5580 emu kill`.

## Aussagegrenzen

Das Image bietet ARM64-Übersetzung an; ein erfolgreicher Unity-ARM64-Appstart ist damit noch nicht bewiesen. Ein erfolgreicher übersetzter Lauf belegt wiederum keine physische ARM-Hardware, Haptik, OEM-Tastatur oder echte Gruppendynamik. Die installierte APK und geprüften Interaktionen werden deshalb jeweils separat dokumentiert. Herstellerreferenzen: [Emulator starten](https://developer.android.com/studio/run/emulator-commandline), [Beschleunigung](https://developer.android.com/studio/run/emulator-acceleration), [ADB](https://developer.android.com/tools/adb).
