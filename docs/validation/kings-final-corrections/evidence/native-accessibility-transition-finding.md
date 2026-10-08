# Native Android finding: public hierarchy after panel transitions

Candidate: 1.1.0/code5, source `908d95fc80fc8173df02593422ff85264089b945`. Root-owned `word_deduction_api36_16k`, API36, 1080×1920/density480/font1.5. TalkBack16 is actually bound, touch exploration and service-managed double tap enabled (`work/kings-native/code5-max-group-services.txt`). External hierarchy probe explicitly preserves existing accessibility services.

After replaying the labelled English good-King maximum fixture, native accessibility actions Back → Abandon match → Yes, abandon match return to the visible twenty-player Group. Repeated native dumps contain only seven framework nodes and an empty, disabled virtual root with zero bounds. This persisted for more than30 seconds; it is not represented as a passing transient. The screenshot shows the complete functioning public Group. Native tap DE redraws that Group and restores64 nodes with usable bounds. No app exception or sensitive values occurred in the retained nonempty current-PID log.

With the repaired-by-redraw Group, pause two people: nineteen active permits8 ordinary Undercover, eighteen permits7. Mode changes Quick → Classic → Kings remain accessible and preserve the manual preference. Starting Kings again produces the same empty native hierarchy, including a later settled read. The probe stops at the missing Back label rather than pretending to navigate. Android Back redraws the now-visible Match panel and restores15 pause nodes. Accessible Partie abbrechen → Ja, Partie abbrechen reproduces the empty Group tree a second time.

Evidence under `work/kings-native`:

- `code5-max-group.json`, `code5-max-group-settled.json`, `code5-max-group-screen.png`, `code5-max-group-services.txt`.
- `code5-max-group-de.json`: successful same-panel redraw,64 native nodes.
- `code5-group-return-repro-0.json` through `-3.json`, `code5-group-return-repro-run.txt`, `code5-group-start-settled.json`.
- `code5-group-repro-pause.json`, `code5-group-repro-abandon.json`, `code5-group-return-second-repro.json`.
- `code5-max-end-log.txt`: retained current-process release log.

The implementation hypothesis is a one-shot accessibility Build visiting a newly shown panel while `resolvedStyle.display` still reports None. Subsequent Tick updates only existing nodes, so an empty hierarchy stays empty. The correction implementer owns reproducing this with rendered panel visibility transitions, fixing the cause and verifying both directions without a language-change workaround. Root owns final native verification of the corrected source-pinned APK. This is a public-navigation defect; private card audio remains outside the accepted accessibility contract.
