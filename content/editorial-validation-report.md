# Proposed bilingual word catalog: editorial and mechanical review

Prepared 6 October 2026 as exploration for ticket #6. This is content preparation outside the repository, not completion of the content ticket or a release-readiness claim.

## Result

The proposed JSON contains **520 original bilingual pairs**, **20 familiar themes**, and **1,038 distinct normalized card terms in each language**. Each of the 1,040 word positions per language has a translation. The only repeated complete terms are Nashorn / Rhinoceros and Wildschwein / Wild boar, each used twice in different useful pairs. Repetition was retained deliberately where familiar words made better pairs than less familiar alternatives.

All 520 pairs were authored individually and editorially read in both languages. The parent agent additionally read the complete authoring source and supplied a second editorial review. No competitor catalog, word-list website, purchased list, or other external word collection was imported or copied. The research supplied risks and quality criteria, not source vocabulary. Similarity of ordinary pairs to naturally occurring everyday examples is incidental.

The selection emphasizes concrete objects, animals, foods, places, materials and familiar activities. Both sides should support plausible shared clues while retaining an understandable difference. Every pair was checked for this relationship, comparable everyday specificity, familiarity, DE/EN concept alignment, duplicate concepts, synonyms and inappropriate content. There are no intentionally retained known mistranslations or synonym-only pairs. Familiarity and actual difficulty remain judgments that require real groups to confirm.

## Files and schema

- `word-pairs.proposed.json`: UTF-8, NFC display strings, a JSON array of `{ "id", "theme", "de": [first, second], "en": [first, second] }` objects. Array positions align across languages. No gameplay role is attached to a side.
- `catalog-source.txt`: readable authoring source. Every row now contains its explicit stable ID; moving or removing another row does not renumber IDs.
- `validate-catalog.cjs`: Node built-in modules only; regenerates the proposed JSON and validation report from the explicit-ID source.
- `validation-report.json`: complete measured counts, duplicates, longest terms and SHA-256.
- `freeze-ids.cjs`: the one-time authoring helper used before handoff to record IDs explicitly. Normal maintenance uses the validator, not a new numbering pass.

Keep IDs attached to their concept pairs after integration. Do not renumber IDs when sorting the catalog, changing display spelling, adjusting a theme or removing another entry. A materially different replacement pair should receive a new unused ID after release so persisted history remains meaningful.

## Mechanical validation

The final run of `node validate-catalog.cjs` exits successfully and reports PASS.

| Check | Final result |
|---|---:|
| Bilingual pairs | 520 |
| Themes | 20 |
| DE distinct normalized complete terms | 1,038 |
| EN distinct normalized complete terms | 1,038 |
| Missing or blank translations | 0 |
| Duplicate IDs | 0 |
| Identical sides within a pair | 0 |
| Duplicate or reversed pairs, DE | 0 |
| Duplicate or reversed pairs, EN | 0 |
| Unexpected object fields | 0 |
| Untrimmed, non-NFC or ASCII-control display strings | 0 |
| Inconsistent translations of repeated complete terms | 0 |
| Maximum occurrence of any complete term | 2 |

Normalization applies NFKD, lowercase and ß → ss, then removes combining marks, punctuation and whitespace, retaining Unicode letters and digits. A complete displayed concept such as “Chocolate hazelnut spread” counts once, not as three words. This is stricter than case-folding and trimming alone. It does not prove semantic uniqueness; synonyms and translation meanings were examined editorially.

Final catalog SHA-256: `ee203f520d636fc6df13c8f21e0dd49f66b43cad058dcb1ea9ca8c20febf85c4`.

## Complete editorial coverage

Every row in each listed ID range was inspected, including both language arrays. The shared relation below records the intended clue space; themes are internal content metadata and need not be public hints. The current equal counts are an authoring organization choice, not an additional product requirement. Good future revisions may change theme sizes or reuse a familiar word while preserving the actual minimums.

| Theme / IDs reviewed | Count | Shared relation and meaningful distinctions considered |
|---|---:|---|
| `fruit-vegetables-001` through `026` | 26 | Familiar produce; shape, taste, seeds, preparation and growing habits. Redcurrant wording disambiguates the English dried-fruit meaning of currant. |
| `meals-001` through `026` | 26 | Meals and staples; preparation, texture, ingredients, presentation and eating occasions. No dependence on knowing a restaurant brand. |
| `bakery-sweets-001` through `026` | 26 | Baked goods, sweets and snacks; shape, texture, ingredients and serving method. Actual flavor/type differences are retained, not spelling variations. |
| `drinks-pantry-001` through `026` | 26 | Drinks, dairy and pantry ingredients; temperature, use, flavor and preparation. Near-synonymous coffee/cocoa variants were removed. |
| `kitchen-001` through `026` | 26 | Familiar utensils, appliances and containers; function, handling, heat, cleaning and storage. Ladle synonyms were not kept as separate concepts. |
| `home-001` through `026` | 26 | Furniture, rooms, surfaces and cleaning items; location, material, size and use. |
| `clothing-001` through `026` | 26 | Clothing and accessories; body location, weather, material and occasion. Plain Handschuh is preferred over the less natural Fingerhandschuh. |
| `care-health-001` through `026` | 26 | Personal care and ordinary health objects; purpose, body area, application and setting. Obscure diagnostic equipment was removed. |
| `mammals-001` through `026` | 26 | Familiar animals; habitat, size, movement, fur and behavior. Tapir and the narrow roe/red deer distinction were replaced with more familiar animals. |
| `birds-water-creatures-001` through `026` | 26 | Birds and creatures associated with water; habitat, flight/swimming, appearance and movement. Includes turtle/crocodile rather than another otter variant. |
| `garden-nature-001` through `026` | 26 | Familiar plants, plant parts and small garden animals; growth, habitat, appearance, scent and movement. |
| `landscape-weather-001` through `026` | 26 | Landscapes and visible weather; scale, shape, water, temperature and movement. Savannah replaces steppe; sleet is correctly Schneeregen. |
| `transport-travel-001` through `026` | 26 | Vehicles, travel equipment and documents; propulsion, route, carrying capacity, use and journeys. Diving bell was replaced with container ship. |
| `places-001` through `026` | 26 | Familiar public places and infrastructure; what people do there, layout, services and access. Shop translations avoid the pharmacy/drugstore ambiguity. |
| `school-office-001` through `026` | 26 | Learning, writing and office supplies; task, material, shape and use. Wax seal and rubber stamp are distinct physical concepts. |
| `tools-materials-001` through `026` | 26 | Common DIY/garden tools and materials; motion, material, protection, fixing and building. No specialist trade knowledge is required to identify them. |
| `sports-001` through `026` | 26 | Familiar sports, exercise and equipment; movement, venue, rules, implements and team/solo use. English team handball is explicit. |
| `arts-media-001` through `026` | 26 | Instruments, performers, art and media; sound, performance, medium, audience and creation. |
| `games-leisure-001` through `026` | 26 | Ordinary games, hobbies and outdoor leisure; actions, materials, setting and social participation. Literal translations of culturally narrow games were removed. |
| `technology-001` through `026` | 26 | Everyday devices and digital actions; input/output, power, communication and use. Plain Tablet and Desktop-PC replace awkward German expansions. |

Some conventional everyday pairs include a broader term and a recognized contrasting form: glove/mitten, golf/miniature golf, surfing/windsurfing, port/marina. These are intentionally retained because ordinary use provides a clear contrast and shared clues. They are not presented as strict biological or technical taxonomies. Examples such as tree species, ice-cream flavors and cake types denote genuinely different familiar concepts rather than manufactured color/number variations.

## Material changes made during the editorial pass

- Replaced ambiguous currant with Redcurrant and Rote Johannisbeere.
- Removed Gnocchi/Spätzle because “egg noodles” did not preserve the German specificity. The final pair is Nudel/Kloß and Noodle/Dumpling.
- Replaced broad cream/chocolate gateau overlap with the clear chocolate/lemon cake distinction.
- Removed the regional meaning mismatch of German/English Nougat; Marzipan/Nut brittle is now an explicit nut-confection comparison.
- Removed cocoa/hot chocolate and neighboring coffee variants whose distinction was too weak. The retained alternatives use juice, milk/cream and cream cheese/cottage cheese.
- Removed the redundant Suppenkelle/Schöpfkelle concept from separate rows; the affected pair now compares salad servers with barbecue tongs.
- Clarified Standmixer versus Handmixer, Bathrobe for Bademantel, Pot holder for Topflappen, and Pill for Tablette.
- Replaced an ear-scope term with eye patch/sleep mask, two familiar coverings with different purposes.
- Replaced Jaguar/Panther with Jaguar/Cougar because panther could describe a jaguar or leopard. Replaced blackbird/thrush and parrot/budgerigar with clearer everyday contrasts.
- Replaced Graupel/Sleet with Schneeregen/Sleet; the initial translation was inaccurate.
- Replaced the narrow Tapir, roe/red deer and Steppe options after second review. The chosen repeated Rhino/Boar terms are intentional and reported rather than hidden by alternate spellings.
- Removed culturally narrow literal translations of Topfschlagen and Gummitwist; tug of war/sack race and hopscotch/egg-and-spoon race work naturally in English.
- Replaced a near-duplicate broad bridge/viaduct pairing with suspension bridge/drawbridge.
- Clarified medical and digital vocabulary to avoid avoidable ambiguity, including Medical thermometer, Desktop PC, Disposable battery and Rechargeable battery.

## Targeted human review candidates

These are retained plausible pairs, not known defects. Familiarity can vary with age and region. They are explicit priorities for a real mixed-group vocabulary review rather than claims that the catalog has already been socially tested. No specialist rules, recipes or zoology should be required to give ordinary clues.

| ID | Pair | Remaining question |
|---|---|---|
| `meals-020` | Risotto / Paella | Are both dishes recognizable to the intended mixed-age audience? |
| `bakery-sweets-012` | Marzipan / Krokant; Marzipan / Nut brittle | The concepts are distinct, but confectionery vocabulary varies by region. |
| `mammals-009` | Pavian / Lemur; Baboon / Lemur | Familiarity with these animal names may vary more than with domestic animals. |
| `birds-water-creatures-008` | Kakadu / Wellensittich; Cockatoo / Budgerigar | English speakers may know “budgie” better; consider display wording after feedback. |
| `birds-water-creatures-026` | Schwertwal / Narwal; Killer whale / Narwhal | Narwhal recognition may vary by age and exposure. |
| `landscape-weather-012` | Vulkan / Geysir; Volcano / Geyser | Both offer clear heat/water clues; confirm geyser is familiar enough. |
| `landscape-weather-015` | Sumpf / Moor; Swamp / Bog | Distinct wetland concepts, but groups may have little everyday vocabulary to distinguish them. |
| `sports-007` | Baseball / Cricket | Familiarity is strongly regional; the game should not depend on knowing detailed rules. |
| `games-leisure-002` | Domino / Mahjong; Dominoes / Mahjong | Confirm the tile-game name is familiar without relying on a specific software product. |
| `technology-025` | Suchmaschine / Webbrowser; Search engine / Web browser | Everyday concepts that some users conflate; verify the distinction feels fair. |

There are 510 other pairs if all ten candidates are provisionally excluded. Excluding them requires rerunning the actual validator and retaining their existing IDs in any history migration; do not infer counts after edits from this report.

English predominantly follows British spelling and everyday vocabulary (colour, theatre, chips/crisps, courgette/aubergine, draughts, lorry). This is consistent and does not mean all English-speaking groups will prefer those words. A US-English variant would be an editorial localization task, not a mechanical search-and-replace. German uses standard broadly understandable wording, with familiar loanwords where idiomatic. Neither language relies on brand or celebrity knowledge.

## Integration and validation still required

The long-card candidates include `bakery-sweets-024` (“Chocolate hazelnut spread”, 25 Unicode code points), `places-008` (“Pedestrian traffic light”, 24), `care-health-021` (“Blood pressure monitor”, 22), and several 18-character German terms such as Rollkragenpullover, Blutdruckmessgerät and Schlittschuhlaufen. The mechanical report contains the complete longest-term shortlist. Line wrapping, font size, gestures and reveal concealment were not inspected in Unity or on Android during this preparation.

The ticket implementer must still adapt/load the catalog through the Session, validate the production catalog, implement and test persistent pair cycles and recent-word avoidance, check language changes and restarts, render the long words, and validate player-facing rules/UI translations. This file does not implement R05 random role assignment or establish balance, group enjoyment, child-age suitability, accessibility or release readiness. No repository files, Git state, issue state or Unity assets were changed by this content-preparation task.

References read: the local mirrors of `docs/specs/android-release-v1.md`, `docs/specs/tickets/06-content.md`, the expanded-player-feedback report (particularly R05/R06 and the editorial/repetition guidance), root `GLOSSARY.md`, applicable agent instructions and ADR 0001. The attempted live issue read could not run because `gh` was unavailable on PATH; the parent agent owns live issue coordination.
