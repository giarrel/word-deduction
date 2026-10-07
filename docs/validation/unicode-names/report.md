# Deterministic Unicode player names

Baseline: cd19d5fac24202f7fe6d80dd45083af77a2cf1a4. Focused contribution to #8; no UI source, Editor, Android process, or tracker changes.

## Behavior

New adds and renames count Unicode 17.0.0 default extended graphemes consistently on .NET and Unity-compatible C#. A skintone/ZWJ emoji, flag, Indic conjunct, Hangul syllable and combining-accent grapheme each consume one of the 24 available positions. NameText.FirstElement is the single public presentation helper; it returns the original first grapheme without normalization or uppercasing. It returns empty for empty text and rejects malformed UTF-16.

New inputs cannot consist only of invisible/default-ignorable formats, marks, separators or unassigned scalars. Embedded joiners and marks are preserved. U+2028 and U+2029 now follow the existing no-line-break rule. NFC normalization is unchanged; explicit UTF-16 validation occurs before it.

Previously saved names retain structural validation (nonblank, well-formed UTF-16, trimmed NFC, no control characters), without retroactively applying the new visibility or grapheme-length policy. Unicode-version changes can increase the count of a formerly valid name, so reapplying the current 24 limit on load would lose valid groups. The stricter limit still applies to new adds and renames. This compatibility rule covers saved and removed players and does not change schema V4.

## Evidence

- UI implementer independently reproduced the original failure in actual Unity: 24 astronaut ZWJ emoji rejected while 24 combining accents were accepted. Its evidence is in the #8 UI worktree, not produced by this agent.
- Invisible-name Session red: original code creates a player from U+200B. New validation passes the same add/rename/write-preservation case.
- Unicode conformance red: even .NET 9 StringInfo fails official Unicode 17 case 745 (changed pictographic classification). NameText passes all 766 unmodified official cases through its public FirstElement seam.
- Legacy red: applying the new length limit on read rejects genuine pre-change V4 group and live Classic fixtures. Separating read compatibility from new-entry policy makes both load, retain exact bytes on open, preserve IDs, support undo/rename, and continue or abandon the live match.
- The two legacy fixtures were created using the complete pre-change Session sources at cd19d5f under .NET 9. The generator source is retained here. They contain a zero-width name, an undoable mark-only player and 24 old-runtime scissors/ZWJ clusters. This is a real old Session writer fixture, not a claim of old APK execution.
- Full Session suite: 52/52 passed, including all existing Quick, Classic, content and recovery behavior plus the five initial name/conformance cases. A subsequent narrow Unicode-line-separator case went red then green (1/1) after its fix. The first focused build waited on the prior suite's Windows apphost lock; the log includes those build warnings and the actual behavioral failure after the lock cleared.
- Malformed surrogate adds and renames preserve the confirmed file; 24/25 contracts survive reopen with stable IDs and NFC names. CJK, Arabic marks and embedded joiners remain accepted.
- The generator verifies six source hashes; regenerating produces the same 2,380 property ranges and source SHA-256 789b0fe76f8777e808585ab530c72aafd99719f89b48149ac3a11117d984fb3e.

## Sources and handoff

The algorithm is [Unicode UAX #29 revision 47](https://www.unicode.org/reports/tr29/tr29-47.html#Grapheme_Cluster_Boundary_Rules), using the [official Unicode 17 property data and conformance tests](https://www.unicode.org/Public/17.0.0/ucd/auxiliary/). Source hashes, regeneration steps and licensing are recorded in third-party/unicode-17.0.0/README.md. The required Unicode License v3 is included both there and in the runtime TextAsset Resources/Unicode-LICENSE.

The UI implementer must replace its StringInfo initial extraction with NameText.FirstElement and run the already-red actual Unity regression. Runtime visual glyph availability, Arabic/Indic shaping, native IME behavior, text-element deletion and Android persistence/update are still owned by the UI/root playtest loop. This contribution does not claim those visual or native gates have passed.

Whitespace checking excludes only the verbatim upstream GraphemeBreakTest fixture, whose two original trailing spaces are retained so its published SHA-256 remains exact. All authored files pass git diff --check.
