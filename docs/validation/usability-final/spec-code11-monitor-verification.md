# Spec verification — code11 pin 154388a

Read-only review of `976c0fc49742898e41fa21d2392cd34fec6a73f8..154388a64d15f8510f532b14f9d90ac7d46816a1` against Specs #17/#18. This report freezes the findings at that pin; later corrections require a separate verification.

**Original P2 closed:** cancellation of an already captured native contact is now latched before same-update slot reuse. The original failing provider sequence and its following valid drag pass. The monitor is removed by the shared release/cancel/capture-loss/detach cleanup, and other contacts remain protected. Recorded initial verification: provider 2/2 and full rendered 59/59. The original P2 report remains unchanged and hash-identical.

**Two additional P2 findings are dynamically confirmed at this pin:**

1. **Register cancellation observation before a fully buffered drag starts.** In `GroupReorder.cs:55–60`, observation starts at the buffered UI Down and reads the slot's current native ID. A complete Begin→Move→Cancel→replacement Begin queued before UI dispatch therefore arrives too late for this monitor: the canceled preview commits. This violates #17: “invalid, stale or cancelled requests leave the prior state unchanged” (`group-usability.md:40`). Preserve the terminal reason for the original contact even when its whole gesture precedes UI dispatch.

2. **Do not apply a replacement contact's cancellation to an intentional release.** `GroupReorder.cs:99` falls back to the current slot phase without checking the original contact identity. A valid End followed by another Begin→Cancel in the same input update consequently discards the intentional reorder. This violates #17's “commit one complete order atomically at drop/action” (`group-usability.md:40`) and its pre-match reordering story. Classify the completed contact using its own terminal event rather than a newer occupant's phase.

I inspected `artifacts/usability-final/code11/provider-two-gaps-red.json`: **2 passed, 2 failed**, 0.85 s. `FullyBufferedCanceledDragKeepsTheOrder` and `LaterCanceledContactDoesNotCancelAnEndedDrag` fail on reopened durable order; both earlier provider cases pass. These are observed defects, not hypothetical gates.

No additional #18 mismatch identified. Final native/package/performance acceptance remains with the coordinator. I ran no tests, Editor or ADB and made no repository changes; only this external report was written.
