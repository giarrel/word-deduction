# Included bilingual words

`word-pairs.json` is the frozen, individually authored catalog: 520 stable pair IDs, 20 themes and 1,038 distinct normalized complete terms per language. Its SHA-256 is `ee203f520d636fc6df13c8f21e0dd49f66b43cad058dcb1ea9ca8c20febf85c4`. `catalog-source.txt` is the matching readable explicit-ID authoring source. No competitor list is used.

Run `node content/validate-catalog.cjs` from the repository root. It checks the source against the frozen JSON and writes `validation-report.json`; it does not replace the catalog. Validation checks IDs, bilingual alignment, blank strings, control characters, normalization, identical sides, reversed/duplicate pairs, repeated-word translation consistency, counts and themes. Editorial judgments are recorded in [the preparation review](editorial-validation-report.md); mechanical checks do not establish familiarity or play balance.

`node content/compile-catalog.cjs` compiles the JSON into the Unity-independent Session assembly's `WordCatalog.cs`. `node content/compile-catalog.cjs --check` verifies that every shipped ID and display term exactly matches the JSON. The app contains these strings in its assembly and needs no content download or runtime file lookup. The .NET Session behavior runner uses the JSON as its independent expected-word fixture.

Maintain IDs when correcting spelling, translations, theme metadata or ordering. A materially different replacement concept pair needs a new unused ID. To edit deliberately, update both authored sources, run validation, regenerate C#, and repeat Session and rendered-card checks. The original preparation report used the temporary filename `word-pairs.proposed.json`; integration renamed that same byte-identical file to `word-pairs.json` and made its validator verification-only.

All ten documented editorial review candidates are retained. Their pairs are familiar plausible contrasts, but age and region can affect difficulty. A real mixed-group vocabulary review remains outstanding. Long-word card and cycle evidence is maintained in [bilingual content validation](../docs/validation/bilingual-content.md).
