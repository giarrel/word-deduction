# Word Deduction

An offline party game for one shared Android phone, in German and English. Quick plays one clue round and a vote; Classic continues through eliminations, with optional Mr. White. The app includes 520 bilingual word pairs across 20 themes, persistent groups (40 saved people, up to 20 playing), pull/hold-to-reveal cards, covered match recovery, and optional privacy/license information.

- Unity **6000.3.25f1**, Android module, bundled SDK/NDK/OpenJDK.
- Open `game/` once in Unity to resolve the pinned packages and import assets.
- Open `Assets/WordDeduction/Scenes/App.unity` and enter Play mode.
- Run behavior checks: `dotnet run --project tests/Session.Tests` (.NET 9).
- If this checkout has no Unity package cache, add `-p:NewtonsoftJsonAssembly="<absolute path to an already resolved Unity Runtime/Newtonsoft.Json.dll>"`; the runner still compiles this checkout's production Session sources.
- Run the Unity PlayMode suite `WordDeduction.Tests` through Test Runner or the existing CLI/Pipeline.
- With the Editor closed: `./tools/build-android.ps1 -Build DevelopmentApk`.
- Local nondevelopment candidates: `./tools/build-android.ps1 -Build ReleaseApk`, then `-Build ReleaseBundle`, with the Editor closed. For an open Editor, follow the exact-project live commands in the release guide.
- Build output: ignored `artifacts/android/<source-prefix>/<UTC-stamp>-apk` or `-aab`. Every artifact has its own source/version/options/hash report. Local candidates use the Android debug certificate and are not production-signed submissions.

Read the [product contract](docs/specs/android-release-v1.md), [Session interface](docs/development/session-interface.md), [acceptance matrix](docs/validation/acceptance-matrix.md) and [actual UI/native playtest evidence](docs/validation/ui-polish/report.md). The [release reproduction guide](docs/release/reproduction.md) covers pinned builds, package inspection, signing and real-device validation. [German](docs/release/store/listing-de.md) and [English](docs/release/store/listing-en.md) store text and a [privacy-policy draft](docs/release/privacy-policy.md) are prepared.

The [release candidate report](docs/validation/android-release/report.md) identifies the exact APK/AAB source, hashes, package checks, tests and remaining native/review gates. The [Data Safety and owner handoff](docs/release/data-safety.md) separates prepared materials from actual store submission.

Publishing remains an explicit owner decision. The owner must settle the public identity/support contact, public policy URL, Play account/test-track eligibility and dedicated upload key. The production entrypoint fails before building when its external signing inputs are absent. Never reuse Nischenreich's signing identity. Automated tests and translated-emulator playtests do not establish physical-phone handling or fun/balance for a human group.
