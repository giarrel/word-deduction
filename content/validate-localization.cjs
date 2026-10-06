// Supplementary data validation; rendered app tests remain the layout/input seam.
const fs=require('node:fs');
const path=require('node:path');
const root=path.join(__dirname,'../game/Assets/WordDeduction');
const source=fs.readFileSync(path.join(root,'UI/Copy.cs'),'utf8');
const entry=/\{\s*"([^"]+)"\s*,\s*new\[\]\s*\{\s*("(?:[^"\\]|\\.)*")\s*,\s*("(?:[^"\\]|\\.)*")\s*\}\s*\}/g;
const copy=new Map(); const errors=[];
for(const match of source.matchAll(entry)) {
    const [en,de]=[JSON.parse(match[2]),JSON.parse(match[3])];
    if(copy.has(match[1])) errors.push('Duplicate key: '+match[1]);
    if(!en.trim() || !de.trim()) errors.push('Missing translation: '+match[1]);
    const placeholders=text=>[...text.matchAll(/\{(\d+)(?:[^}]*)\}/g)].map(m=>m[1]).sort().join(',');
    if(placeholders(en)!==placeholders(de)) errors.push('Placeholder mismatch: '+match[1]);
    copy.set(match[1],[en,de]);
}
const declared=(source.match(/new\[\]\s*\{/g)||[]).length;
if(copy.size!==declared) errors.push('Not every copy entry parsed: '+copy.size+'/'+declared);
const references=new Set();
for(const area of ['UI','Session']) for(const file of fs.readdirSync(path.join(root,area)).filter(f=>f.endsWith('.cs'))) {
    const code=fs.readFileSync(path.join(root,area,file),'utf8');
    for(const pattern of [/\bT\(\s*"([^"]+)"/g,/\bAction\(\s*"[^"]+"\s*,\s*"([^"]+)"/g,/(?:Error|Notice)\s*=\s*"([^"]+)"/g])
        for(const match of code.matchAll(pattern)) references.add(match[1]);
    for(const match of code.matchAll(/enum\s+(?:Role|Outcome)\s*\{([^}]+)\}/g))
        for(const value of match[1].split(',')) references.add(value.trim().split(/[\s=]/)[0]);
}
for(const key of references) if(!copy.has(key)) errors.push('Untranslated referenced key: '+key);
const report={status:errors.length?'FAIL':'PASS',bilingualKeys:copy.size,checkedLiteralAndDomainReferences:references.size,errors,
    scope:'Nonblank EN/DE copy, placeholder parity, direct UI references, storage/action errors and role/outcome labels. Ternary/dynamic contextual keys, grammar and layout require rendered walkthroughs.'};
console.log(JSON.stringify(report,null,2));
if(errors.length) process.exitCode=1;
