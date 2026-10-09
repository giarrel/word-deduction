# Flüssiges Scrollen und Kartenaufdecken

## Problem Statement

Der Nutzer erlebt Scrollen und Aufdeckanimationen als ruckelig. Die bisherige Abnahme prüfte Funktion und Einzelbilder, belegt aber keine gleichmäßige Bewegung auf seinem Handy. Die App soll direkt auf Berührungen reagieren und beim Durchgeben angenehm zu bedienen sein.

## Solution

Die App verwendet eine zum Gerät passende, gleichmäßige Bildausgabe. Listen folgen Wischbewegungen flüssig; die Karte folgt dem Finger ohne unnötige Sprünge oder Nachlaufen. Beim Loslassen kehrt ihre leere Rückseite weich zurück, während geheime Inhalte sofort verborgen bleiben. Änderungen werden mit vergleichbaren Vorher-/Nachher-Messungen geprüft.

## User Stories

1. As a player, I want scrolling to follow my finger smoothly, so that browsing does not feel jerky.
2. As a player, I want consistent frame pacing during interaction, so that motion is predictable.
3. As a player, I want the private card to track my pull directly, so that revealing feels under my control.
4. As a player, I want a smooth return on release, so that the card feels polished.
5. As a player, I want all secrets hidden immediately when I release or interrupt the gesture, so that animation cannot leak information.
6. As a player, I want no hitch when my card first reveals its word or private names, so that role-specific content does not disrupt handling.
7. As a participant with a long private list, I want reveal and private scrolling to cooperate, so that every permitted name remains readable.
8. As a player using reduced motion, I want the existing reduced-motion behavior respected, so that decorative motion remains optional.
9. As a player, I want hold-to-reveal, multitouch interruption and safe resume preserved, so that polish does not weaken control or privacy.
10. As a host, I want editing and reordering to coexist with smooth group scrolling, so that one improvement does not break another.
11. As a user, I want smoothness improvements without new settings to configure, so that I can start playing immediately.
12. As a returning user, I want language, group, word history and saved matches untouched by a rendering change, so that the update is safe.

## Implementation Decisions

- Establish the installed code8 baseline first. Separate frame pacing, input/gesture mapping and layout/text work rather than attributing all stutter to one cause without evidence.
- Reuse the current Unity UI Toolkit and existing card module; no new animation framework, third-party scrolling replacement or alternate renderer.
- Target smooth display-compatible interactive frame pacing, with 60 fps as the baseline target on a 60 Hz display and explicit compatible behavior on other refresh rates. Confirm Unity's Android behavior against primary documentation. Do not claim measured performance merely from setting a requested frame rate.
- Keep direct finger movement independent of decorative easing. Decorative return should be time-based, bounded and stable across varying frame intervals. Reduced-motion mode suppresses decorative movement. Reuse existing timing/input seams; avoid a new global animation architecture.
- Conceal the word, role, leader names and known-player list synchronously at release, pointer cancellation, interruption or app backgrounding. Only non-secret decoration may animate afterward. No privacy fade that leaves secrets readable.
- Remove recurring layout, allocation or accessibility refresh work only when evidence identifies it as material. Preserve semantic refreshes after actual content/layout changes and all recently fixed native accessibility transitions.
- Preserve existing release signing/privacy flags and all three game modes. Diagnostic profiling is opt-in in validation tooling and must not remain active in the distributable release.

## Testing Decisions

- Public rendered interaction is the relevant seam. Test observable card movement, return completion, release concealment, reduced motion and existing gesture behavior; do not assert private helper names or exact easing implementation.
- Preserve comparable baseline and candidate measurements: same device/AVD, resolution, font size, group/list size, APK mode and action sequence. Report sample length, median/tail frame interval, stalls and tooling limitations. Distinguish app-requested frame rate from actual presented frames.
- Aim for the 60 Hz test environment to sustain a measured interactive median near 16.7 ms instead of 33.3 ms when an old 30 fps cap is confirmed. Record the real result; investigate recurring larger stalls rather than hiding them in an average. If host contention limits translated-emulator measurements, state this and retain the data; do not assert a physical-device result.
- Verify actual motion visually over time, not only still screenshots. Use native frame traces or a capture method that does not defeat private-card protection. Recheck immediate hiding through release/cancel/background and long private lists, large text, DE/EN and public accessibility transitions.
- Run the relevant existing suites once per meaningful code change. Preserve a focused failing observation and corrected result for each discovered defect; avoid repeating full suites without new concerns.

## Out of Scope

- Physical-device claims without that device, a guarantee across all Android hardware, battery benchmarks, new sound/haptic systems or a visual redesign.
- Weakening secure-window or secret-concealment behavior to record a video.
- New gameplay settings, online services, analytics or production publication.

## Further Notes

Source: explicit user feedback on 9 October 2026. No technical cause was confirmed by the earlier screenshot investigation. The existing isolated Android emulator is suitable for comparative observations but does not establish performance or tactile quality on the user's physical handset. Initial code review baseline: `53951d9f1d6cc0f9014db7615cfa7a159bb804d9`.
