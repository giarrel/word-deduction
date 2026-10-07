# Scoped Spec verification: secret-word fitting

Reviewed only the runtime/asset delta `b86f19564b1d4c2fa31f02d7872dca7e835d881a...71e6f39e599727cd176e2545ee65d1ab692d12de`; checkout HEAD matched the latter. No new substantiated Spec finding.

The change addresses the spec's “As a phone user, I want readable text, generous touch targets and content clear of system bars and the keyboard, so that handling the app feels easy.” `SecretCard.cs:70–86` measures complete space-separated tokens using the rendered label font and available card width, choosing 22–32dp. `Match.uss:23–25` gives the label explicit usable width, removes inherited insets, and prevents wrapping single terms. Phrases retain natural wrapping between words; catalog text is unchanged. Mr. White's separate instruction keeps its existing size.

The spec requires: “On pointer release/cancel/capture loss, focus loss, pause, navigation and before the next owner, erase the secret text immediately; the return animation is decoration after concealment.” `Conceal()` still clears `word.text` synchronously before capture release or animation. Fitting introduces no deferred callback, persisted reveal state or alternate reveal path. Existing pointer/cancel/navigation handling is unchanged.

Inspected the new nine-term normal/150% regression, supplied 30/30 rendered result, and complete-token measurement report: 2,127 catalog tokens measured at 22dp, widest 244px versus 248px card content. These are supplied run evidence, not a rerun by this reviewer. Independently viewed four captures: normal Sprachnachricht, enlarged Nuss-Nougat-Creme, enlarged Chocolate hazelnut spread, and enlarged released Sprachnachricht. Card terms remain whole/readable and the released card shows no secret.

Evidence caveat: the English phrase capture has an incomplete Help label and no visible owner; neither unchanged presenter path establishes a defect caused by this scoped delta. Reported to the coordinator for the pending native check rather than asserting a source regression.

**Result: scoped source/evidence verification passes; actual code4 APK/AAB and Android word-fit/privacy acceptance remain the coordinator's gates.** No source edits, Unity, ADB, repeated Session run, or broader repository scan.
