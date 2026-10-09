# Code8 native motion baseline, idle Editor

9 October2026. Installed package remains1.1.0/code8, SHA256d4ba4c5ba65ca03249f59455234c1df7bdf4f85b160828ca05b945a85cd78ab9. Root captured these after both owned group Editors exited, with no build or Session stress test running. The motion agent was granted its Editor lease only after these captures and restoration finished.

## Environment and method

The exact word_deduction_api36_16k AVD, server5038/transport127.0.0.1:5583, uses AndroidAPI36,1080x1920,density480,font1.0,60Hz, ARM64 release code through the emulator's native bridge. Existing secure-window behavior was retained. Environment and installed-package bytes are recorded in code8-environment. This is no physical-phone measurement.

MotionScheduled sends the complete input batch in advance and schedules each move from Android uptime. Four1-second drags use60 requested moves each. Actual timestamps show move gaps with medians15–17ms, with scheduling tails recorded in the input JSON; these are not assumed perfectly uniform60Hz events. SurfaceFlinger actual-present fences establish displayed-frame intervals. All four gestures have presentation coverage through release+400ms in each unrecorded trace.

Group starts with the original five synthetic people in Kings. The first scroll trace follows the original settled group screenshot; the repeat follows the same four-cycle sequence. Both remain available, including the first trace's1-second stall. The card trace explicitly replays the existing synthetic card-start fixture, then uses the actual Resume button. Nora remains card1/5 with the same word/leader; no handoff advances. This replay is not represented as newly played setup.

## Unrecorded frame output

| Capture | Presented frames | Median ms | p95 ms | Maximum ms | Complete gestures |
| --- | ---: | ---: | ---: | ---: | ---: |
| code8-warm-scroll |151|48.07|93.19|1007.08|4/4|
| code8-warm-scroll-repeat |190|46.88|75.80|101.56|4/4|
| code8-warm-pull |188|45.85|76.79|157.26|4/4|
| code8-warm-pull-repeat |177|47.91|92.81|152.06|4/4|

Each collection lasts approximately9.0–9.2seconds. Final analysis uses nearest-rank p95, explicit in analysis-code8-warm-scroll--code8-warm-scroll-repeat--code8-warm-pull--code8-warm-pull-repeat.json. Earlier per-run summaries and the collector used floor((n-1)*.95); they remain unchanged, and are not silently substituted for the final percentile definition. Per-gesture drag/return windows, event gaps and all raw rows are retained.

The repeated scroll and both pull traces are the comparison set; the initial scroll stall is also reported. These observations confirm irregular frame output under this emulator workload. They do not identify CPU/GPU attribution or establish touch-to-photon latency. Candidate comparisons must retain the same fixtures, event schedule, geometry where the card/list geometry permits, and the same percentile method.

## Actual visual sequence

code8-pull-visual.webm is a separate10.098-second host-framebuffer recording (1080x1920 VP9, encoded60fps). It includes synthetic private cards through the emulator's authenticated host console without changing release privacy flags. Encoding materially worsened displayed timing (median59.17ms in the associated trace), so that run is excluded from performance comparison. Encoded60fps is not application60fps.

Root inspected the actual full-screen before/after screenshots, the first4seconds at10samples/s in code8-pull-overview.png, and the3.6–4.4second card-region sequence at30samples/s in code8-return-sequence.png. The sampled sequence shows the word/leader being replaced by the covered card and only a few distinct visible return positions. It supports the reported abrupt motion but not an exact input-to-hide latency claim. The original video remains available for matching candidate extraction.

## Restored state and other validation

After recording, root restored both originals using the guarded replay tool, relaunched, and read both again. code8-restored-state/manifest.json confirms the same five identities, Match=null,history12, with exact hashes:

- primary f146725f8d3e527fc5a71ef1ee4ac2eb80048c419a54a3dbf3822ea0f6fcf28b
- previous d47a4f6ae89377513a09dec09c9230f7498080e2b673be0e5596950f4f0e9467

No app-data clear, uninstall, settings alteration or persistent group edit occurred. The unchanged code8-source corpus stress test independently passed1/1 with diagnostics and an external temp directory; this does not explain the intermittent Windows replacement failures retained by the group implementer. No additional corpus repeats were used to erase that uncertainty.

Candidate implementation, independent reviews, full integrated suites, APK/AAB inspection, actual update and matching native gestures remain pending.

---

Lesefassung mit dauerhaften relativen Links. Das [unveränderte eingefrorene Original](../../../artifacts/usability-final-preservation/25d3e9f9d806-20261009T093925Z/native/baseline-report.md) bleibt einschließlich früherer Formulierungen erhalten. Die aktuelle Updatebeschreibung unterscheidet den vorherigen Originalrestore vom eigentlichen Installationsversuch.
