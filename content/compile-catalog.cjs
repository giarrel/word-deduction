// Compile the frozen authored catalog into the Unity-independent Session assembly.
// --check fails when the shipped C# differs; ordinary invocation updates that file.
const fs = require('node:fs');
const path = require('node:path');
const pairs = JSON.parse(fs.readFileSync(path.join(__dirname,'word-pairs.json'),'utf8'));
const target = path.join(__dirname,'../game/Assets/WordDeduction/Session/WordCatalog.cs');
const quote = value => JSON.stringify(value);
const source = `// Generated from content/word-pairs.json by content/compile-catalog.cjs. Preserve stable IDs.\nnamespace WordDeduction\n{\n    internal static class WordCatalog\n    {\n        internal sealed class Pair\n        {\n            public readonly string Id;\n            public readonly string[] German, English;\n            public Pair(string id, string deA, string deB, string enA, string enB) { Id=id; German=new[]{deA,deB}; English=new[]{enA,enB}; }\n        }\n        internal static readonly Pair[] Pairs = {\n${pairs.map(p => `            new Pair(${[p.id,...p.de,...p.en].map(quote).join(', ')}),`).join('\n')}\n        };\n    }\n}\n`;
if (process.argv.includes('--check')) {
    if (fs.readFileSync(target,'utf8') !== source) throw new Error('Shipped catalog differs from the authored catalog. Run node content/compile-catalog.cjs.');
    console.log(`PASS: all ${pairs.length} authored bilingual pairs match the shipped Session catalog.`);
} else fs.writeFileSync(target,source);
