"""Final offline max/private/rematch addendum; preserves earlier audit snapshots."""
from pathlib import Path
import hashlib, base64, json, re
from datetime import datetime, timezone
BASE=Path(__file__).resolve().parents[2]
O=BASE/'work/kings-implementation'; N=BASE/'work/kings-native'
F=BASE/'work/kings-worktrees/final-corrections/artifacts/kings-final-corrections'
inputs={}; checks=[]; snapshots={}
def sha(b): return hashlib.sha256(b).hexdigest()
def read(p):
    b=p.read_bytes(); key=p.relative_to(BASE).as_posix(); meta={'sha256':sha(b),'bytes':len(b)}
    if key in inputs: assert inputs[key]==meta, 'Input changed: '+key
    inputs[key]=meta; return b
def js(p): return json.loads(read(p).decode('utf-8-sig'))
def check(n,a,e): checks.append({'name':n,'actual':a,'expected':e,'pass':a==e})
core=js(O/'code8-native-audit.json')
frozen=core['earlierAuditsPreserved']|{
    'code8-native-audit.json':'877a1ab97a332a69e28b822be0d4357fe17741ac3a9f7fb0d725e102691b838f',
    'code8-native-audit.md':'aea91b9548f48175fcd5f9d2f09d6d2caaba59f88eb8cad1289e3ff763fe91c8'}
def frozen_check(stage):
    for k,v in frozen.items(): check(stage+'/'+k,sha(read(O/k)),v)
frozen_check('before')
def state(p):
    b=read(p); e=json.loads(b.decode('utf-8-sig'))
    c=base64.b64encode(hashlib.sha256(e['Payload'].encode('utf-8')).digest()).decode()
    check(p.relative_to(N).as_posix()+'/checksum',c,e['Checksum'])
    check(p.relative_to(N).as_posix()+'/schema',e['Version'],5)
    s=json.loads(e['Payload']); snapshots[p.relative_to(BASE).as_posix()]={'sha256':sha(b),'version':e['Version'],'checksum':c,'state':s}
    return s
def pair(label):
    folder=N/label; m=js(folder/'manifest.json'); out={}
    for filename in ['session.json','session.previous.json']:
        p=folder/filename; out[filename]=state(p)
        check(label+'/'+filename+'/manifestHash',sha(read(p)),m['files'][filename])
    for key,field in [('match','Match'),('players','Players')]: check(label+'/manifest/'+key,m[key],out['session.json'][field])
    check(label+'/manifest/history',m['used_pairs'],out['session.json']['History']['UsedPairIds'])
    return out
def values(t):
    return [n[k] for n in t['nodes'] for k in ['text','description'] if n.get(k) not in [None,'','null']]
def visible_values(t):
    out=[]
    for n in t['nodes']:
        if not n['visible']: continue
        v=n.get('description')
        if v in [None,'','null']: v=n.get('text')
        if v not in [None,'','null']: out.append(v)
    return out

fixtures={}; replay_manifests={}
for kind,fixture,replay in [('good','German-GoodKing','code8-max-good-replay'),('evil','English-EvilKing','code8-max-evil-replay-replay')]:
    rm=js(N/replay/'manifest.json'); replay_manifests[kind]=rm
    ss={name:state(N/'session-fixtures'/fixture/name) for name in ['session.json','session.previous.json']}
    for name in ss: check(kind+'/'+name+'/explicitFixtureHash',sha(read(N/'session-fixtures'/fixture/name)),rm['expected'][name])
    check(kind+'/replayProvenance',rm['provenance'],'Explicit fixture replay, not Android-played match')
    fixtures[kind]=ss['session.json']

seq=js(N/'code8-twenty-settled-sequence.json')
check('twenty/pass',seq['pass'],True); check('twenty/actionCount',len(seq['actions']),6)
counts=[15,64,14,15,15,64]; actions=[]; pairs={}
for i,a in enumerate(seq['actions']):
    label=f'code8-twenty-settled-{i:02}'; p=pair(label+'-state'); pairs[label]=p
    t=js(N/(label+'.json')); err=read(N/(label+'-stderr.txt')).decode('utf-8-sig')
    check(label+'/labelReference',a['evidence'],label)
    check(label+'/counts',[len(t['nodes']),a['nodeCount']],[counts[i],counts[i]])
    check(label+'/visibleLabels',visible_values(t),a['visibleLabels'])
    check(label+'/servicesRetained',t['dontSuppressAccessibilityServices'],True)
    check(label+'/actionSucceeded','NAMED_APP_ACTION click success=true' in err,True)
    s=p['session.json']; check(label+'/rosterPreserved',s['Players'],fixtures['good']['Players'])
    check(label+'/active20',sum(x['Active'] for x in s['Players']),20)
    check(label+'/manual8',s['KingsUndercoverPreference'],8)
    check(label+'/groupOrMatch',s['Match'] is None,i in [1,5])
    zero=[n for n in t['nodes'] if n['visible'] and n['bounds']=='0 0 0 0' and any(n.get(k) not in [None,'','null'] for k in ['text','description'])]
    check(label+'/semanticBounds',zero,[])
    actions.append({'label':a['label'],'tree':t,'statePrimarySha256':sha(read(N/(label+'-state')/'session.json')),'stderr':err})
    read(N/(label+'.png'))
for i in [3,4]:
    for f in ['session.json','session.previous.json']:
        check(f'twenty/{i}/{f}/pauseAndModalPreserve',sha(read(N/f'code8-twenty-settled-{i:02}-state'/f)),sha(read(N/'code8-twenty-settled-02-state'/f)))
early=js(N/'code8-twenty-bound-sequence.json'); late=js(N/'code8-twenty-after-back-settled.json')
check('earlierTwenty/notPassed',early['pass'],False)
check('earlierTwenty/oneRecordedBack',len(early['actions']),1)
check('earlierTwenty/oldCardNodes',early['actions'][0]['nodeCount'],14)
check('pureReadLater/pauseNodes',len(late['nodes']),15)
check('pureReadLater/hasResume','Partie fortsetzen' in values(late),True)

private={}
for kind in ['good','evil']:
    s=fixtures[kind]; m=s['Match']; owner=m['Participants'][m['Handoff']]['DisplayName']
    if kind=='good':
        expected=['Zurück','Könige','Hilfe','KARTE 10 VON 20',owner,
          'Private Karte. Wörter werden nie vorgelesen. Zum visuellen Lesen TalkBack kurz durchreichen lassen und dieses Feld halten. Loslassen verdeckt die Karte.','Handy weitergeben']
        check('good/ownerIsGoodKing',m['Participants'][m['Handoff']]['Id'],m['GoodKingId'])
    else:
        expected=['Back','Kings','Help','CARD 9 OF 20',owner,
          "Private card. Words are never spoken. To read it visually, use TalkBack's pass-through gesture, then hold here. Release to hide.",'Hide & pass on']
        check('evil/ownerIsWhite',m['Participants'][m['Handoff']]['Role'],2)
    captured={}
    for phase in ['held','released']:
        stem=f'code8-max-{kind}-pull-{phase}'; t=js(N/(stem+'-tree.json'))
        check(stem+'/nodeCount',len(t['nodes']),14)
        check(stem+'/exactPublicOnlyValues',values(t),expected)
        check(stem+'/servicesRetained',t['dontSuppressAccessibilityServices'],True)
        next_node=next(n for n in t['nodes'] if n.get('description')==expected[-1])
        check(stem+'/nextEnabled',next_node['enabled'],phase=='released')
        private_action_labels=[x for n in t['nodes'] for x in re.findall(r' - ([^,\]]+)',n['actions']) if x!='null']
        check(stem+'/noCustomActionLabels',private_action_labels,[])
        raw=json.dumps(t,ensure_ascii=False)
        forbidden=[m['CivilianWord'],m['UndercoverWord']]+[p['Id'] for p in m['Participants']]
        check(stem+'/noWordsOrIDs',[v for v in forbidden if v in raw],[])
        captured[phase]={'tree':t,'imageSha256':sha(read(N/(stem+'.png'))),'treeSha256':sha(read(N/(stem+'-tree.json')))}
    private[kind]={'publicAllowlist':expected,'fixtureMatchId':m['Id'],'actualWords':[m['CivilianWord'],m['UndercoverWord']],
                   'heldReleased':captured,'visualEvidence':'Root explicitly inspected actual pull-held header and held list bottom; initial stationary good-card attempts remain covered/non-passing.'}
    for phase in ['held','released']: read(N/f'code8-max-{kind}-bottom-{phase}.png')
for file in ['code8-max-good-header-held.png','code8-max-good-stationary-settled-held.png',
             'code8-max-good-stationary-settled-released.png','code8-max-evil-ordinary-hold-held.png','code8-max-evil-ordinary-hold-released.png']:
    read(N/file)

game7=pair('code5-game7-result-state')
rm=js(N/'code8-final-rematch-replay-replay'/'manifest.json')
for f in game7: check('finalRematch/replayExpected/'+f,rm['expected'][f],sha(read(N/'code5-game7-result-state'/f)))
rematch=pair('code8-intentional-rematch-state'); final=pair('code8-final-group-state')
g=game7['session.json']; r=rematch['session.json']; end=final['session.json']
check('rematch/newExactIDPhase',[r['Match'][k] for k in ['Id','Phase','Handoff','Outcome']],['b8a9b91c8fa14f2db7605b8ba32c809c',0,0,None])
check('rematch/rosterSame',r['Players'],g['Players']); check('rematch/allFiveActive',sum(p['Active'] for p in r['Players']),5)
check('rematch/noEliminated',any(p['Eliminated'] for p in r['Match']['Participants']),False)
check('rematch/historyAppend',r['History']['UsedPairIds'],g['History']['UsedPairIds']+[r['Match']['PairId']])
check('rematch/history12',len(r['History']['UsedPairIds']),12)
check('finalGroup/noMatch',end['Match'],None); check('finalGroup/sameRoster',end['Players'],g['Players'])
check('finalGroup/sameHistory',end['History'],r['History'])
check('finalGroup/exactPreviousRematch',sha(read(N/'code8-final-group-state/session.previous.json')),sha(read(N/'code8-intentional-rematch-state/session.json')))
font=read(N/'code8-final-font.txt').decode('utf-8-sig').strip()
services=read(N/'code8-final-accessibility-services.txt').decode('utf-8-sig')
check('restored/font1',font,'1.0'); check('restored/noBoundServices','Bound services:{}' in services,True)
check('restored/touchExplorationOff','touchExplorationEnabled=false' in services,True)
for file in ['code8-rematch-before.png','code8-rematch-after.png','code8-final-pause.png','code8-final-abandon-confirm.png','code8-final-group.png']: read(N/file)

def secrets(states):
    v=set()
    for s in states:
        for p in s['Players']: v.update([p['Name'],p['Id']])
        if s['Match']:
            m=s['Match']; v.update([m['Id'],m['CivilianWord'],m['UndercoverWord']])
            for p in m['Participants']: v.update([p['Id'],p['DisplayName']])
    return sorted(x for x in v if x)
logs=[]
pattern=re.compile(r'FATAL EXCEPTION|Fatal signal|\bANR\b|AndroidJavaException|NullReferenceException|InvalidOperationException|\bException\b|StackOverflow|OutOfMemoryError',re.I)
for stem,pid,ss in [('code8-twenty-final-log','14856',[fixtures['good'],pairs['code8-twenty-settled-02']['session.json']]),
                    ('code8-max-evil-final-log','16094',[fixtures['evil']]),('code8-final-log','16465',[g,r,end])]:
    raw=read(N/(stem+'.txt')); txt=raw.decode('utf-8-sig'); lines=[x for x in txt.splitlines() if x.strip()]; tested=secrets(ss)
    hit=[{'line':i,'value':v} for i,line in enumerate(lines,1) for v in tested if v in line]
    diag=[{'line':i,'text':line} for i,line in enumerate(lines,1) if pattern.search(line)]
    pids=sorted(set(m.group(1) for line in lines if (m:=re.match(r'^\d\d-\d\d\s+\S+\s+(\d+)\s+',line))))
    check(stem+'/pid',pids,[pid]); check(stem+'/privateHits',hit,[]); check(stem+'/crashExceptionPatternHits',diag,[])
    original=js(N/(stem+'.json'))
    logs.append({'file':stem+'.txt','pid':pid,'savedBytes':len(raw),'nonemptyLines':len(lines),'sha256':sha(raw),
                 'testedValues':tested,'hits':hit,'diagnosticPattern':pattern.pattern,'diagnosticHits':diag,'originalMetadata':original})

rendered=js(F/'22-rendered-native-layout-result.json'); check('currentRendered/summary',rendered['data']['result']['summary'],{'total':47,'passed':47,'failed':0,'skipped':0,'inconclusive':0})
session=read(F/'session-full.log').decode('utf-8-sig'); check('currentSession/74',session.strip().endswith('74/74 passed'),True)
packages=js(F/'handoff/artifacts.json'); check('packages/source',packages['sourceCommit'],'5c56e63d755df24156e6f5e88f4170ae4857a0ab')
for a in packages['artifacts']:
    p=Path(a['path']); b=read(p); check(p.suffix+'/fileHash',sha(b),a['sha256']); check(p.suffix+'/fileBytes',len(b),a['fileBytes'])
read(F/'handoff/report.md')
frozen_check('after')
failed=[c for c in checks if not c['pass']]
audit={'generatedUtc':datetime.now(timezone.utc).isoformat(),'scope':'Offline completion of Code8 native max/public/private/rematch evidence; no new device execution.',
       'checksSummary':{'total':len(checks),'passed':len(checks)-len(failed),'failed':failed},'earlierAuditsPreserved':frozen,
       'twentyPublic':{'sequence':seq,'actions':actions,'earlierNotPassed':early,'pureReadSettled':late,
                       'interpretation':'Earlier helper sampled old card after Back; later pure read has Pause15, no redraw. Completed harness waits boundedly for destination by reading, not retrying action.'},
       'maxPrivate':private,'replayManifests':replay_manifests,'finalRematchReplay':rm,'finalRoster':end['Players'],
       'finalUsedPairs':end['History']['UsedPairIds'],'finalSettings':{'font':font,'services':services},'logs':logs,
       'renderedSummary':rendered['data']['result']['summary'],'packageManifest':packages,
       'remaining':'Native public/private visual gates met within recorded scope; final integration, evidence/artifact preservation and coordinator release acceptance pending.',
       'limits':['Original467, Code7, and first Code8 core snapshots remain frozen including historical pending statuses.',
                 'Initial stationary German good-King captures remain covered; only actual pull-held/released trees support current private-visibility audit.',
                 'Visual inspections are root observations; no actual TalkBack pass-through gesture, audible output, blind-user, physical-device or group-balance claim.',
                 'Intentional final rematch is a native action from an explicitly replayed actual-game7 terminal, not a new fully played game.'],
       'snapshots':snapshots,'checks':checks,'inputs':inputs}
(O/'code8-final-native-audit.json').write_text(json.dumps(audit,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
logrows='\n'.join(f"| {l['pid']} | {l['nonemptyLines']} | {l['savedBytes']} | {len(l['testedValues'])} | `{l['sha256']}` |" for l in logs)
treerows='\n'.join(f"| {k} {p} | `{v['heldReleased'][p]['treeSha256']}` |" for k,v in private.items() for p in ['held','released'])
doc=f'''# Final Code8 native evidence addendum

Generated {audit['generatedUtc']}. **{len(checks)-len(failed)}/{len(checks)} offline consistency checks**, {len(snapshots)} snapshot generations. This completes the bounded native evidence mapping together with `code8-native-audit.md/json` (five-player/update/cold-repeat/low-memory); it leaves that earlier256-check snapshot, the Code7 addendum and original467 audit unchanged. No ADB, Unity, build, push or production-source edits were performed.

## Public navigation at twenty players

`code8-twenty-settled-sequence.json` passes six actual actions: abandon→confirmation15 nodes; confirmedGroup64; Start→card14; Back→Pause15; abandon→confirmation15; confirmedGroup64. Raw action stderr, exact visible values, tree counts, nonzero visible semantic bounds, retained services and each saved state agree. All20 player records and manual preference8 survive; pause/dialog leave both generations unchanged. Together with the eight five-player actions (35/31/32/35/16/15/15/35 nodes) in the first Code8 audit, the recorded native public-navigation gate is met.

The earlier `code8-twenty-bound-sequence.json` remains pass=false after one Back action: its14-node snapshot still shows the old card. A later **read-only** capture `code8-twenty-after-back-settled.json` shows Pause15 and Resume without another redraw action. The subsequent helper waits boundedly for destination observations and does not retry the action. This sampling/readiness distinction is retained; no false pass is assigned to the earlier run.

## Actually revealed maximal private cards

The semantic audit uses only `code8-max-{{good,evil}}-pull-{{held,released}}-tree.json`. German good King is owner`·10`, fixture words **Pfirsich/Aprikose**. English White is owner`·9`, fixture words Peach/Apricot, and receives no word on his private face. All four trees have14 nodes and exactly seven allowed public values: Back, mode, Help, card counter, public owner, private-reading instruction and pass-on action. Their complete semantic lists equal those allowlists; no words, participant IDs, custom action labels or additional private associations appear. The pass-on button is disabled while held and enabled after release. Raw trees and fixture associations are in the JSON.

| Actual private capture | Tree SHA256 |
| --- | --- |
{treerows}

Root explicitly inspected actual good pull-held header/Pfirsich and held list bottom; English White pull-held header/no-word and bottom with all8 teammates including CJK/Greek/Cyrillic. Root also inspected ordinary stationary hold/release after restoring TalkBack settings: it opens/covers correctly. The first stationary German good-King header and the later stationary-with-TalkBack attempt stayed **covered**; they are retained as non-passing private-visible observations despite helper textual success. Neither is substituted for the actual pull-visible evidence. The private visual/semantic gate is met in this bounded scope; no real TalkBack pass-through gesture, audible/private audio or blind-user evaluation is claimed.

## Intentional final rematch and restoration

An explicitly labelled replay restores the previously Android-played game7 terminal. Its expected hashes equal the original terminal generations. A deliberate single native Rematch creates **`b8a9b91c8fa14f2db7605b8ba32c809c`**, phase0/handoff0/outcome null, all five active and nobody eliminated. Primary SHA256 **`{sha(read(N/'code8-intentional-rematch-state/session.json'))}`**. History is the original11 used pairs plus `home-003`, exactly12, preserving every previous pair.

Actual Back/abandon then leaves `code8-final-group-state`: Match null, Nora/Luca/Emil/Mila/Jonas2 with the same stable IDs and all active; complete history unchanged from the rematch. Primary SHA256 **`{sha(read(N/'code8-final-group-state/session.json'))}`**; its backup equals the exact rematch primary. Root inspected result/rematch/confirmation/final Group frames. Font is restored to1.0, bound accessibility services empty, touch exploration false in the final retained records. This is a native Rematch action from a replayed terminal, not another fully played match.

## Complete retained-log rescans

| PID | Nonempty lines | Exact saved bytes | Distinct values tested | SHA256 |
| --- | --- | --- | --- | --- |
{logrows}

Each saved log is independently rescanned for all actual roster raw/display names, player/match IDs and the appropriate actual secret words, including Aprikose. There are zero literal private-value matches and zero matches to the explicit bounded crash/exception pattern. The JSON preserves the original scanner metadata; Windows doubled-CR storage can make saved byte counts exceed original process-output counts by one byte per line. These scans do not assert absence of every platform warning or prove future process behavior. The earlier PID13388 LOW_MEMORY/no-action abort, its preserved bytes and platform warnings remain in the first Code8 audit.

## Current validation and remaining coordination gates

The actual retained full rendered result is **47/47 passed,0failed/skipped/inconclusive**; current Session log ends **74/74 passed**. Both late-layout reviews remain0findings at reviewed runtime `85fa17555b44bca61640bcb13a738c3e37ffc34f`. Package source is **`5c56e63d755df24156e6f5e88f4170ae4857a0ab`**, version1.1.0/code8. This audit independently rehashed the actual supplied artifacts:

- APK37,454,738 bytes: `d4ba4c5ba65ca03249f59455234c1df7bdf4f85b160828ca05b945a85cd78ab9`.
- AAB37,811,704 bytes: `41d5a652323b3a263bd7d28d5847bced7e9a2c3ab5a1359500e3026bb0ca8e50`.

The handoff records14APK/44AAB inspection commands; this offline audit does not rerun those tools. **Native public and private visual/semantic gates are met within the recorded scope. Final integration, artifact/evidence preservation and coordinator release acceptance remain pending.** The updated56-story `acceptance-matrix-draft.md` expresses this current state; `acceptance-matrix-code7-frozen.md` preserves the original historical draft. No physical hardware, human balance, store publication or production-signing claim is made.
'''
(O/'code8-final-native-audit.md').write_text(doc,encoding='utf-8')

ledger=O/'acceptance-matrix-draft.md'
old_copy=O/'acceptance-matrix-code7-frozen.md'
if old_copy.exists(): historical_bytes=old_copy.read_bytes()
else:
    ledger_bytes=ledger.read_bytes()
    delimiter=b'\r\n## Code8 five-player update' if b'\r\n## Code8 five-player update' in ledger_bytes else b'\n## Code8 five-player update'
    historical_bytes=ledger_bytes.split(delimiter)[0]
assert sha(historical_bytes)=='8d6d3661b608dd36e8ecc1391e2c92beb68589b33d3c37d4bea47e36b427a6bf'
if not old_copy.exists(): old_copy.write_bytes(historical_bytes)
historical=historical_bytes.decode('utf-8-sig')
replacements={
  26:'Code5 real release/Home/Recents/cancel/second-pointer observations retained. Code8 actual pull-held/released DEgood/ENWhite trees are public-only; normal hold/release also visually checked after settings restoration.',
  36:'All actual terminal outcomes/rosters audited, including eliminated teammates. Code8 public result→Group and both five/twenty-player navigation sequences pass; earlier Code7 failure is historical evidence.',
  50:'All six actual endings and complete result rosters/words/Kings audited. Code8 result→Group has expected public semantics; final deliberate Rematch/abandon works.',
  51:'Actual single Rematch on Code8 from labelled game7-terminal replay creates b8a9b91c8fa14f2db7605b8ba32c809c phase0/handoff0, all5active/history12. Measured Code7 word/terminal repeats and coldCode8 76ms preserve intended screens. Initial unmeasured Code7 rematch remains historical unresolved timing evidence.',
  52:'Actual abandon retains Group. Code8 five/twenty-player public return sequences pass, including final original-five-player Group with Matchnull/history12/stableIDs.',
  55:'Native public gate met: Code8 five-player8actions and twenty-player6actions retain expected semantics/bounds with TalkBack retained. DEgood/ENWhite actual pull-held/released trees each14nodes/seven public-only values; private visuals/lists inspected by root. No actual TalkBack pass-through/audible/blind-user claim.',
  56:'Root records offline API36 native execution. Actual Code8 APK/AAB hashes independently verified here; handoff documents14/44 inspection commands and no new network/service requirement. Final integration/preservation remains coordinator work.',
}
head=['# Kings56-story coverage draft — current Code8 native evidence','',
      'Current evidence: **Session74/74; rendered47/47**. Original467 state audit, Code7 addendum, Code8 core256-check audit and `code8-final-native-audit.md/json` retain exact files/hashes and failed/aborted distinctions. All56 exact requirements/ticket owners remain below. The earlier Code7 draft is preserved separately as `acceptance-matrix-code7-frozen.md`.', '',
      '**Native gates met within recorded scope:** actual update/save preservation, six Code5 Android-played endings, measured Code7/8 repeated-input regressions, corrected Code8 five/twenty-player public navigation, actual revealed max-card public-only semantics and root visual inspection, intentional final rematch/Group restoration and retained-log scans. Earlier LOW_MEMORY/no-action, transient old-card sampling, covered stationary captures and unmeasured rematch evidence remain explicit.', '',
      '**Remaining:** final integration, source/artifact/evidence preservation and coordinator release acceptance. Supplied package source5c56e63d755df24156e6f5e88f4170ae4857a0ab/version1.1.0/code8; rehashed APKd4ba4c5ba65ca03249f59455234c1df7bdf4f85b160828ca05b945a85cd78ab9 and AAB41d5a652323b3a263bd7d28d5847bced7e9a2c3ab5a1359500e3026bb0ca8e50. No physical-device, spoken TalkBack/private audio, actual pass-through gesture, blind-user, group-balance or production-store assertion.', '',
      '| Story | Exact requirement | Ticket | Current evidence and limits |','| --- | --- | --- | --- |']
count=0
for line in historical.splitlines():
    c=[x.strip() for x in line.strip().strip('|').split('|')]
    if len(c)!=4 or not c[0].isdigit(): continue
    i=int(c[0]); count+=1; text=replacements.get(i,c[3])
    text=text.replace('Public a11y return remains blocked on Code7.','Code8 five/twenty-player public return passes; Code7 failure remains historical.')
    text=text.replace('transition a11y remains open.','Code8 public transition retests pass.')
    text=text.replace('Final correction package metadata/hash gate remains pending.','Code8 packages are available and independently rehashed; final integration/preservation remains pending.')
    head.append(f'| {i} | {c[1]} | {c[2]} | {text} |')
assert count==56
ledger.write_text('\n'.join(head)+'\n',encoding='utf-8')
print(json.dumps({'checks':audit['checksSummary'],'snapshots':len(snapshots),'inputs':len(inputs),
                  'logs':[{k:l[k] for k in ['pid','savedBytes','nonemptyLines','sha256']} for l in logs],
                  'outputs':{x:sha((O/x).read_bytes()) for x in ['code8-final-native-audit.md','code8-final-native-audit.json','acceptance-matrix-draft.md','acceptance-matrix-code7-frozen.md']}},indent=2))
if failed: raise SystemExit(1)
