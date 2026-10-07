# Data Safety and publication handoff

Prepared 7 October 2026 for the exact nondevelopment package identified in the release report. Recheck these answers if runtime packages, permission, network behavior or features change. This is an evidence-based form preparation, not a submitted Play Console form.

| Topic | Prepared answer / evidence boundary |
| --- | --- |
| Data collected or shared by this app | No developer collection or sharing. Entered names, random player IDs, preferences, match state and pair history are processed only on this device. Check final package manifest and dependency inspection in the release report. |
| Account creation | No account and no login. No account-deletion flow is applicable. |
| Advertising / in-app purchases | Neither is implemented or included as a runtime SDK. |
| Permissions | No camera, microphone, contacts, location, external-storage or Internet permission in the accepted release artifact. The AndroidX signature-scoped receiver permission is a platform library mechanism, not a permission prompt or collection feature. |
| Encryption in transit | No off-device app transmission is implemented. Do not claim an encrypted service or tick a question that is inapplicable to the no-collection path. |
| Deleting local data | Android Clear storage or uninstall removes app-local data. Removing a visible name can leave it in the previous save generation or a retained damaged copy. |
| SDKs | Unity's core runtime, UI/Input/accessibility, AndroidX appcompat/core/emoji/lifecycle, GameActivity/frame pacing and Newtonsoft.Json support local execution. Pipeline runtime/server/test assemblies are excluded. Analytics startup and device-stat submission are disabled; networking permission is absent. Editor-only tooling telemetry is not app telemetry. |
| App access for review | Unrestricted; no credentials. Add 3 names and choose Quick, or add 5 and choose Classic with Mr. White. All content ships locally. |
| Intended category | Game → Word is the proposed category. Store tags and age-target selection require the owner's final audience decision. |
| Content rating basis | Authored everyday word pairs and social bluffing; no ads, real-money gambling, purchases, chat, online user content or external web browsing. The owner completes the actual IARC questionnaire; no age rating is invented here. |

Google defines collection for this form as sending data off the device and excludes purely local processing. It still requires a Data Safety form and public privacy-policy link for closed/open/production distribution even when an app collects nothing; internal-only testing has a separate exemption. [Official Data Safety guidance](https://support.google.com/googleplay/android-developer/answer/10787469?hl=en).

The [prepared privacy policy](privacy-policy.md) names Word Deduction and describes the actual local handling. Publisher identity/contact remain visibly pending. Before submission, the owner must provide a stable public HTML policy URL and ensure the contact/public identity match the final listing. The app's optional group Info entry already exposes local privacy and license text. [Official user-data policy](https://support.google.com/googleplay/android-developer/answer/10144311?hl=en).

## Prepared store materials

- `store/AppIcon.png`: 512×512, original two-card geometry, square PNG; no borrowed competitor imagery.
- `store/feature-de.png` and `store/feature-en.png`: 1024×500, RGB PNG, with the app's existing palette and card motif. Both were opened and visually inspected independently by implementer and coordinator.
- `store/listing-de.md` and `store/listing-en.md`: name, short/full descriptions and version notes. Actual final native screenshots are supplied by the coordinator and identified separately; no Editor fixture is relabelled as played Android content.
- Art regenerates with `tools/generate-store-art.ps1`, reusing the existing icon generator and Inter font. The generated feature art is promotional artwork; gameplay screenshots remain actual screen captures.
- The app includes readable Inter, Noto Color Emoji, Unicode and Newtonsoft/Json.Net.Unity3D notices in its Info screen. Original licenses remain intact; Android-supplied system fonts are not redistributed by this project.

Google's current preview requirements specify a 512×512 icon and 1024×500 feature graphic. Screenshots must represent the app and keep their longer side within twice the shorter side; this release uses an actual 1080×1920 viewport. [Official preview requirements](https://support.google.com/googleplay/android-developer/answer/9866151?hl=en).

## Owner prerequisites after local acceptance

1. Confirm the working app name Word Deduction and package `com.giarrel.worddeduction`, intended audience, countries and distribution track. Changing the package creates a different Android app and does not transparently migrate the existing installation.
2. Supply the public publisher identity and support/privacy contact; host the completed prepared policy at the chosen public URL.
3. Use an eligible Play Console account, satisfy its required testing/verification steps, and complete the content-rating and Data Safety forms against this package. Account eligibility cannot be inferred from a local successful build.
4. Choose and securely back up a dedicated Word Deduction upload key outside this repository, configure Play App Signing if using Play, and build using the explicit production entrypoint. Do not reuse Nischenreich's key and do not send passwords in chat.
5. Verify the production certificate, rerun artifact inspection and signed-device smoke, then give the explicit instruction for the chosen release/upload. Nothing in this work publishes to a store.

The already-created local APK/AAB make that decision concrete. Their debug certificate is intentionally for local validation; it is not misrepresented as an owner-authorized production identity.
