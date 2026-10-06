# Word Deduction

An offline, pass-one-phone party game for Android, in German and English. This branch currently contains the saved-group foundation; gameplay is implemented by the dependent Quick/Classic tickets.

- Unity **6000.3.25f1**, Android module, bundled SDK/NDK/OpenJDK.
- Open `game/` once in Unity to resolve the pinned packages and import assets.
- Open `Assets/WordDeduction/Scenes/App.unity` and enter Play mode.
- Run behavior checks: `dotnet run --project tests/Session.Tests` (.NET 9).
- Run the Unity PlayMode suite `WordDeduction.Tests` through Test Runner or the existing CLI/Pipeline.
- With the Editor closed: `./tools/build-android.ps1 -Build DevelopmentApk`.
- Build output: ignored `artifacts/android/`. A development APK is not a store release.

The complete product contract is in `docs/specs/android-release-v1.md`. Module behavior and recovery are documented in `docs/development/session-interface.md`; exact evidence and current limitations are in `docs/validation/group-foundation.md`.
