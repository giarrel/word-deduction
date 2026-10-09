# Spec verification — code11 terminal-contact correction

Read-only review of `154388a64d15f8510f532b14f9d90ac7d46816a1..25d3e9f9d80609c178230cf838cecfda54ae0370` in final-corrections, limited to the two confirmed P2 findings and their #17/#18 regression implications.

**Both additional P2 findings are closed; 0 new substantiated Spec findings.**

- **Fully buffered canceled drag:** a native drop now requires a contact observed as live at Down and a deliberate End belonging to that contact. A replacement slot's arbitrary UI Up no longer authorizes the canceled preview. `FullyBufferedCanceledDragKeepsTheOrder` now preserves reopened order and subsequently completes a normal drag. This meets #17's “invalid, stale or cancelled requests leave the prior state unchanged” (`group-usability.md:40`).
- **Later cancellation of another contact:** the monitor records `Ended` for its captured native ID as well as `Canceled`. The original deliberate End therefore survives the replacement contact's later cancellation. `LaterCanceledContactDoesNotCancelAnEndedDrag` now commits the intended order and permits a following normal drag, meeting #17's atomic drop/action requirement on the same specification line.

Exact-contact cleanup and monitor removal remain unchanged. The prior same-update cancellation regression stays green. No new secret-card, privacy, persistence, role-rule or rendering behavior was introduced by this correction.

I inspected the retained results: all four actual-provider scenarios **4/4 passed** with normal follow-up controls (0.76 s); the real-provider scenarios passed **4/4 again** after fixture isolation (0.77 s); combined rendered regression **61/61 passed**, zero failures/skips/inconclusive (204.37 s). These are inspected results, not new executions by this reviewer.

The pooled GroupUsability/Motion fixtures now use the installed public InputTestFixture to isolate synthetic events from unrelated host touch hardware. The real-provider tests remain on their actual provider path; production logic was not weakened for the fixtures. The earlier failed broader run and original P2 reports remain retained. The preceding two-P2 report was verified SHA256-identical to its committed copy.

Final packaged native cancellation, update, privacy and motion acceptance remain with the coordinator. No new hypothetical acceptance gates are added. No Editor, build, ADB, tests or repository changes were performed; only this external verification report was written.
