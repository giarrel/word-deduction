# Unicode 17.0.0 name data

NameText implements the default extended grapheme rules in [UAX #29 revision 47](https://www.unicode.org/reports/tr29/tr29-47.html#Grapheme_Cluster_Boundary_Rules), including Indic conjuncts and emoji joins. It uses generated property ranges, not the host runtime's StringInfo tables.

The property data and the unmodified conformance fixture are from the [Unicode 17.0.0 UCD](https://www.unicode.org/Public/17.0.0/ucd/). They are covered by [Unicode License v3](LICENSE.txt). A runtime copy is included as game/Assets/Resources/Unicode-LICENSE.txt for distribution and credits.

| Official input | SHA-256 |
| --- | --- |
| auxiliary/GraphemeBreakProperty.txt | d6b51d1d2ae5c33b451b7ed994b48f1f4dc62b2272a5831e7fd418514a6bae89 |
| emoji/emoji-data.txt | 2cb2bb9455cda83e8481541ecf5b6dfda66a3bb89efa3fa7c5297eccf607b72b |
| DerivedCoreProperties.txt | 24c7fed1195c482faaefd5c1e7eb821c5ee1fb6de07ecdbaa64b56a99da22c08 |
| UnicodeData.txt | 2e1efc1dcb59c575eedf5ccae60f95229f706ee6d031835247d843c11d96470c |
| auxiliary/GraphemeBreakTest.txt | e2d134d2c52919bace503ebb6a551c1855fe1a1faec18478c78fff254a1793ec |
| https://www.unicode.org/license.txt (retrieved 2026-10-07) | e7a93b009565cfce55919a381437ac4db883e9da2126fa28b91d12732bc53d96 |

Regenerate using Python 3:

    python tools/generate-name-unicode.py --download

The generator verifies every downloaded SHA-256 before generating the data. To reuse the verified files offline, omit --download, optionally passing --cache <directory>. The generated file is approximately 79 KB of source / 28 KB of integer data.

New player names require a visible base: not a Unicode mark, separator, control, format, surrogate, unassigned scalar, or default-ignorable character. This is an application input policy, not a claim that the font can render every remaining character. Embedded marks, joiners and variation selectors remain intact. Existing stored names retain structural validation without applying new visibility or grapheme-limit policy retroactively.

Normalization still uses the existing platform NFC implementation. Grapheme segmentation and name-base classification use the pinned Unicode 17 data. Glyph availability, shaping, keyboard editing and rendering are separate UI/platform concerns.
