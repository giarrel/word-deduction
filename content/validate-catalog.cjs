const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');

const directory = __dirname;
const source = fs.readFileSync(path.join(directory, 'catalog-source.txt'), 'utf8');
const pairs = [];
const counters = new Map();
let theme;
for (const [lineNumber, raw] of source.split(/\r?\n/u).entries()) {
  const line = raw.trim();
  if (!line) continue;
  if (line.startsWith('@')) {
    theme = line.slice(1);
    if (!/^[a-z]+(?:-[a-z]+)*$/u.test(theme)) throw new Error(`Invalid theme on line ${lineNumber + 1}`);
    continue;
  }
  const fields = line.split('|');
  if (!theme || fields.length !== 5) throw new Error(`Invalid source line ${lineNumber + 1}: an explicit stable ID and four terms are required`);
  const [id, ...words] = fields;
  if (!new RegExp(`^${theme}-[0-9]{3}$`, 'u').test(id)) throw new Error(`Invalid ID on source line ${lineNumber + 1}`);
  const index = (counters.get(theme) || 0) + 1;
  counters.set(theme, index);
  pairs.push({ id, theme, de: words.slice(0, 2), en: words.slice(2, 4) });
}

// Strict normalization retains meaningful letters but ignores punctuation,
// accents, spacing, casing and German ss/eszett spelling differences.
const normalize = value => value.normalize('NFKD').toLowerCase().replace(/ß/gu, 'ss').replace(/\p{M}/gu, '').replace(/[^\p{L}\p{N}]/gu, '');
const errors = [];
const warnings = [];
const ids = new Set();
const counts = {};
for (const pair of pairs) {
  if (ids.has(pair.id)) errors.push(`Duplicate ID: ${pair.id}`);
  ids.add(pair.id);
  if (Object.keys(pair).join(',') !== 'id,theme,de,en') errors.push(`Unexpected schema: ${pair.id}`);
}
for (const language of ['de', 'en']) {
  const words = new Map();
  const unorderedPairs = new Map();
  const translations = new Map();
  for (const pair of pairs) {
    const sides = pair[language];
    if (!Array.isArray(sides) || sides.length !== 2 || sides.some(word => typeof word !== 'string' || !word.trim())) {
      errors.push(`Missing/invalid translations: ${pair.id}.${language}`);
      continue;
    }
    if (sides.some(word => word !== word.trim() || /[\u0000-\u001f\u007f]/u.test(word) || word !== word.normalize('NFC'))) errors.push(`Whitespace/control/NFC issue: ${pair.id}.${language}`);
    const normalized = sides.map(normalize);
    if (normalized.some(word => !word)) errors.push(`Empty normalized word: ${pair.id}.${language}`);
    if (normalized[0] === normalized[1]) errors.push(`Identical sides: ${pair.id}.${language}`);
    const key = normalized.slice().sort().join('|');
    if (unorderedPairs.has(key)) errors.push(`Duplicate/reversed ${language} pair: ${unorderedPairs.get(key)} / ${pair.id}`);
    unorderedPairs.set(key, pair.id);
    for (let index = 0; index < 2; index++) {
      const entry = words.get(normalized[index]) || { word: sides[index], count: 0, ids: [] };
      entry.count++;
      entry.ids.push(pair.id);
      words.set(normalized[index], entry);
      const translated = pair[language === 'de' ? 'en' : 'de'][index];
      const equivalents = translations.get(normalized[index]) || new Set();
      equivalents.add(translated);
      translations.set(normalized[index], equivalents);
    }
  }
  const inconsistent = [...translations].filter(([, alternatives]) => alternatives.size > 1).map(([word, alternatives]) => ({ word, alternatives: [...alternatives] }));
  if (inconsistent.length) warnings.push({ language, inconsistentTranslations: inconsistent });
  const repeated = [...words.values()].filter(entry => entry.count > 1).sort((a, b) => b.count - a.count || a.word.localeCompare(b.word));
  const longest = pairs.flatMap(pair => pair[language].map(word => ({ id: pair.id, word, characters: Array.from(word).length }))).sort((a, b) => b.characters - a.characters).slice(0, 12);
  counts[language] = { distinctNormalizedWords: words.size, totalWordSlots: pairs.length * 2, repeatedWords: repeated, longestTerms: longest };
  if (words.size < 700) errors.push(`Insufficient ${language} word coverage: ${words.size}`);
}
if (pairs.length < 500) errors.push(`Insufficient pair count: ${pairs.length}`);
if (counters.size < 12) errors.push(`Insufficient theme count: ${counters.size}`);

const output = JSON.stringify(pairs, null, 2) + '\n';
if (fs.readFileSync(path.join(directory, 'word-pairs.json'), 'utf8') !== output) errors.push('Frozen catalog differs from the explicit-ID authoring source.');
const report = {
  status: errors.length ? 'FAIL' : 'PASS',
  pairCount: pairs.length,
  themeCount: counters.size,
  themeCounts: Object.fromEntries(counters),
  languages: counts,
  errors,
  warnings,
  normalization: 'NFKD; lowercase; ß→ss; remove combining marks, whitespace and punctuation; retain Unicode letters and digits',
  catalogSha256: crypto.createHash('sha256').update(output, 'utf8').digest('hex'),
};
fs.writeFileSync(path.join(directory, 'validation-report.json'), JSON.stringify(report, null, 2) + '\n', 'utf8');
console.log(JSON.stringify(report, null, 2));
if (errors.length) process.exitCode = 1;
