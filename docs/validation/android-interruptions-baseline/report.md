# Android-Unterbrechungen: geprüfte Quick-Basis

6. Oktober 2026. Ergänzende Laufzeitprüfung für Ticket 7, während Classic, Inhalte und Recovery umgesetzt werden. Dies ist keine Gesamtabnahme dieser späteren Änderungen.

## Prüfstand

Unveränderte, bereits abgenommene Quick-APK: SHA-256 `32cfb05e08d82686ae5b643ed8d6ba86e79bc641b3a074c44cdf9cc73b09cfce`, 37.552.110 Bytes, Unity 6000.3.25f1, ARM64/IL2CPP, Development. Android-16-Emulator `emulator-5580`, API 36, Host-GPU, ARM64-Übersetzung, 1080×2400 bei 420 dpi. Kein physisches Telefon.

Eingaben stammen aus dem externen [MotionEvent-Helfer](../android-quick-runtime/WordDeductionInput.java), mit Quelle TOUCHSCREEN, Tooltyp FINGER und gemeinsamem DownTime. Er ist nicht Teil der App. Der [Hostablauf](Invoke-CardInterruptions.ps1) hält den Helfer während einer Sequenz offen. Screenshots stammen aus dem Emulator-Framebuffer und wurden einzeln visuell geprüft. Sie beweisen Zustände nach der Aktion, keine gemessene Reaktionszeit in Millisekunden.

## Beobachtete Ergebnisse

| Aktion | Beobachtung | Evidenz |
|---|---|---|
| Wort hochziehen, natives ACTION_CANCEL senden | Wort weg, dieselbe erste Person; Weiter erst bei geschlossener Karte | [Offen](01-open-before-cancel.png), [abgebrochen](02-cancel-covered.png) |
| Noch einmal aufdecken, weit außerhalb bewegen und loslassen | Verdeckte Karte, kein Personenwechsel | [Außerhalb losgelassen](04-outside-release-covered.png) |
| Haltealternative, Home, App erneut öffnen | Geheimes Wort verschwindet; verdeckte Fortsetzen-Seite | [Vorher](05-open-before-home.png), [Rückkehr](06-home-return-covered-pause.png) |
| Haltealternative, Android-Appübersicht | Vorschau enthält sichere Pause statt Wort; Rückkehr ebenfalls Pause | [Vorher](07-open-before-recents.png), [Übergang](08-protected-recents-preview.png), [ruhende Vorschau](13-stable-recents-preview.png) |
| Haltealternative, Display schlafen legen und aufwecken | Verdeckte Fortsetzen-Seite; dargestelltes Wort vor den Unterbrechungen bleibt Bread roll | [Vorher](11-open-before-sleep.png), [aufgeweckt](12-wake-covered-pause.png) |
| Fortsetzen, native Android-Zurück-Taste | Pause, keine Beendigung der Partie; gespeicherter Envelope unverändert | [Zurück](14-native-back-key-pause.png), [vorher](after-interruptions-session.json), [nachher](after-back-session.json) |
| Bestätigungsdialog zunächst beibehalten, danach ausdrücklich abbrechen | Gruppe, IDs und Einstellungen exakt wie vor der Testpartie; Match wieder null | [Vergleich](comparison.json), [Ausgang](before-session.json), [Ende](final-session.json) |

Die Testpartie blieb bei Alexandra, Handoff 0, mit Pair-ID `bakery-sweets-001`. Nach Appübersicht, Fortsetzen und nativer Zurück-Taste ist der persistierte Envelope bytegleich. Nach bestätigtem Abbruch sind Payload und Checksum mit dem Ausgangszustand identisch. Die App-Daten wurden weder gelöscht noch durch ein Fixture ersetzt.

## Noch gesondert zu prüfen

Native **Randgeste** ist nicht mit der erfolgreichen Zurück-Taste gleichzusetzen: zwei ADB-Touch-Wischversuche vom linken Rand blendeten lediglich die Systemleisten ein. Android meldet Navigationsmodus 2, aktivierten EdgeBackGestureHandler und keine Ausschlussregion. Das ist ein offener Prüfpunkt, kein belegter Appfehler. Die vollständige Recovery-Implementierung sowie Classic- und White-Phasen folgen mit Ticket 7; Unicode, Touchgrößen und Accessibility mit Ticket 8.

Die ruhende Android-Appvorschau darf die sichere Pause zeigen. Der separate Aufnahmeschutz bei tatsächlich offenem Wort wurde bereits in der [Quick-Abnahme](../android-quick-runtime/report.md) nachgewiesen. Keine Aussage über OEM-Vorschauen, physische Haptik oder reale Gruppendynamik wird aus diesem Emulatorlauf abgeleitet.
