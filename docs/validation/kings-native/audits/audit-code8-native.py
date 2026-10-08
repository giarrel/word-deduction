"""Offline Code8 evidence addendum; never controls Android or edits source/evidence."""
from pathlib import Path
import base64
import hashlib
import json
import re
from datetime import datetime, timezone

BASE = Path(__file__).resolve().parents[2]
OUT = BASE / 'work/kings-implementation'
N = BASE / 'work/kings-native'
FROZEN = {
    'native-state-audit.md': 'b57b872ef09df20097fe128082d8f1a9ee05c35526f244bd403f1325469a1ef9',
    'native-state-audit.json': 'f207fae79deb64cf21891558a43036d1463eba5dc9b2509af3a4fcc261612d19',
    'audit-native-state.py': '8a2a5227fdd89a668a1f6df79275be09811ada92cb38674bad449be24428cff4',
    'code7-native-audit.md': 'a6340392e8719a170a35636a08375b31628f3b962ac54d62419569d5d4e37143',
    'code7-native-audit.json': 'b91add67ba11cc68bd5d91460613194377021e6b8166ed6ade82a8a5022fb9a8',
}
inputs, checks, snapshots = {}, [], {}

def sha(data): return hashlib.sha256(data).hexdigest()

def read(path):
    data = path.read_bytes()
    key = path.relative_to(BASE).as_posix()
    meta = {'sha256': sha(data), 'bytes': len(data)}
    if key in inputs: assert inputs[key] == meta, 'Evidence changed during audit: ' + key
    inputs[key] = meta
    return data

def js(path): return json.loads(read(path).decode('utf-8-sig'))

def check(name, actual, expected):
    checks.append({'name': name, 'actual': actual, 'expected': expected, 'pass': actual == expected})

def frozen(stage):
    for name, digest in FROZEN.items(): check(f'frozen/{stage}/{name}', sha(read(OUT / name)), digest)

frozen('before')

def pair(label):
    folder = N / (label + '-state')
    manifest = js(folder / 'manifest.json')
    out = {}
    for filename in ['session.json', 'session.previous.json']:
        raw = read(folder / filename)
        e = json.loads(raw.decode('utf-8-sig'))
        computed = base64.b64encode(hashlib.sha256(e['Payload'].encode('utf-8')).digest()).decode('ascii')
        s = json.loads(e['Payload'])
        check(f'{label}/{filename}/checksum', computed, e['Checksum'])
        check(f'{label}/{filename}/hash', sha(raw), manifest['files'][filename])
        check(f'{label}/{filename}/schema', e['Version'], 5)
        record = {'sha256': sha(raw), 'bytes': len(raw), 'version': e['Version'],
                  'checksum': computed, 'state': s}
        snapshots[(folder / filename).relative_to(BASE).as_posix()] = record
        out[filename] = record
        if filename == 'session.json':
            check(f'{label}/manifestMatch', s['Match'], manifest['match'])
            check(f'{label}/manifestPlayers', s['Players'], manifest['players'])
            check(f'{label}/manifestHistory', s['History']['UsedPairIds'], manifest['used_pairs'])
    return out

labels = ['code7-code8-pending-baseline', 'code8-before-first-launch',
          'code8-first-cold-doubletap', 'code8-after-low-memory'] + [f'code8-five-bound-{i:02}' for i in range(8)]
pairs = {label: pair(label) for label in labels}
comparisons = []
for filename, a, b in [
    ('code8-update-byte-equality.json', 'code7-code8-pending-baseline', 'code8-before-first-launch'),
    ('code8-low-memory-preservation.json', 'code8-first-cold-doubletap', 'code8-after-low-memory'),
]:
    reported = js(N / filename)
    equality = {}
    for f in pairs[a]:
        equality[f] = read(N / (a+'-state') / f) == read(N / (b+'-state') / f)
        check(f'{filename}/{f}/independentByteEquality', equality[f], True)
        generation = next(g for g in reported['generations'] if g['file'] == f)
        check(f'{filename}/{f}/reportedHashes',
              [generation['beforeSha256'], generation['afterSha256']],
              [pairs[a][f]['sha256'], pairs[b][f]['sha256']])
    check(f'{filename}/reportedPass', reported['pass'], True)
    comparisons.append({'file': filename, 'reported': reported, 'independentByteEquality': equality})

timing = js(N / 'code8-first-cold-doubletap-timing.json')
events = timing['deviceEvents']
gap = events[2]['event'] - events[1]['event']
check('firstColdRepeat/eventActions', [x['action'] for x in events], [0,1,0,1])
check('firstColdRepeat/injected', [x['injected'] for x in events], ['true']*4)
check('firstColdRepeat/gap', gap, 76)
check('firstColdRepeat/reportedGap', timing['actualBetweenTapsMs'], gap)
check('firstColdRepeat/returnCode', timing['returnCode'], 0)
baseline = pairs['code8-before-first-launch']['session.json']['state']
result = pairs['code8-first-cold-doubletap']['session.json']['state']
check('firstColdRepeat/sameMatchAndTerminal', [result['Match'][k] for k in ['Id','Phase','Outcome']],
      ['69fc9f23a2b44963b61cc5e954f24af4',3,6])
check('firstColdRepeat/sameRoster', result['Players'], baseline['Players'])
check('firstColdRepeat/sameHistory', result['History'], baseline['History'])
check('firstColdRepeat/exactBackup', pairs['code8-first-cold-doubletap']['session.previous.json']['sha256'],
      pairs['code8-before-first-launch']['session.json']['sha256'])

sequence = js(N / 'code8-five-bound-sequence.json')
check('fiveBound/reportedPass', sequence['pass'], True)
check('fiveBound/actionCount', len(sequence['actions']), 8)
expected_labels = ['Gruppe bearbeiten','Schnell','Klassisch','Könige',"Los geht's",'Zurück','Partie abbrechen','Ja, Partie abbrechen']
expected_counts = [35,31,32,35,16,15,15,35]
expected_modes = [2,0,1,2,2,2,2,2]
actions = []

def values(tree, visible_only=False):
    result = []
    for node in tree['nodes']:
        if visible_only and not node.get('visible'): continue
        d = node.get('description')
        t = node.get('text')
        v = d if d not in [None,'','null'] else t
        if v not in [None,'','null']: result.append(v)
    return result

for i, action in enumerate(sequence['actions']):
    label = f'code8-five-bound-{i:02}'
    tree = js(N / (label+'.json'))
    stderr = read(N / (label+'-stderr.txt')).decode('utf-8-sig')
    probe = read(N / (label+'-probe-output.txt')).decode('utf-8-sig')
    probe_head = json.loads(probe.splitlines()[0])
    s = pairs[label]['session.json']['state']
    check(f'{label}/actionLabel', action['label'], expected_labels[i])
    check(f'{label}/actionEvidence', action['evidence'], label)
    check(f'{label}/nodeCount', len(tree['nodes']), expected_counts[i])
    check(f'{label}/reportedNodeCount', action['nodeCount'], len(tree['nodes']))
    check(f'{label}/visibleLabels', action['visibleLabels'], values(tree, True))
    check(f'{label}/probeTreeHash', probe_head['sha256'], sha(read(N/(label+'.json'))))
    check(f'{label}/actionSucceeded', 'NAMED_APP_ACTION click success=true' in stderr, True)
    check(f'{label}/servicesPreserved', tree['dontSuppressAccessibilityServices'], True)
    check(f'{label}/fiveStablePlayers', s['Players'], baseline['Players'])
    check(f'{label}/selectedMode', s['Mode'], expected_modes[i])
    semantic_nodes = [node for node in tree['nodes'] if node.get('visible') and
                      any(node.get(k) not in [None,'','null'] for k in ['text','description'])]
    zero_bounds = [node for node in semantic_nodes if node['bounds'] == '0 0 0 0']
    check(f'{label}/visibleSemanticNodesHaveBounds', zero_bounds, [])
    if i in [0,1,2,3,7]:
        check(f'{label}/groupNoLiveMatch', s['Match'], None)
    else:
        check(f'{label}/handoffSameNewMatch', [s['Match'][k] for k in ['Id','Phase','Handoff','Outcome']],
              ['2cb873a240ed4170ad073fd6d6f8c29e',0,0,None])
    actions.append({'label': action['label'], 'evidence': label, 'nodeCount': len(tree['nodes']),
                    'visibleValues': values(tree,True), 'allValues': values(tree), 'tree': tree,
                    'actionStderr': stderr, 'primarySha256': pairs[label]['session.json']['sha256']})
    read(N / (label+'.png'))
for i in [5,6]:
    for f in ['session.json','session.previous.json']:
        check(f'fiveBound/{i}/pauseModalPreserves/{f}', pairs[f'code8-five-bound-{i:02}'][f]['sha256'],
              pairs['code8-five-bound-04'][f]['sha256'])

aborted = js(N / 'code8-five-public-sequence.json')
check('initialAborted/zeroActions', aborted['actions'], [])
check('initialAborted/notPassed', aborted['pass'], False)
initial_tree = js(N / 'code8-five-public-initial.json')
exit_text = read(N / 'code8-accessibility-exit-info.txt').decode('utf-8-sig')
exit_first = exit_text.split('ApplicationExitInfo #0:')[1].split('ApplicationExitInfo #1:')[0].strip()
for fragment in ['2026-10-08 23:19:53.427 pid=13388', 'reason=3 (LOW_MEMORY)', 'status=0', 'rss=543MB']:
    check('initialAborted/exitRecord/'+fragment, fragment in exit_first, True)
crash = read(N / 'code8-accessibility-crash-buffer.txt')
fatal = read(N / 'code8-accessibility-fatal-log.txt')
check('initialAborted/crashBufferEmpty', len(crash.strip()), 0)
check('initialAborted/fatalLogEmpty', len(fatal.strip()), 0)
services = read(N / 'code8-first-result-services.txt').decode('utf-8-sig')
check('services/TalkBackBoundAtRetainedCheckpoint', 'Bound services:{Service[label=TalkBack,' in services, True)
check('services/touchExploration', 'touchExplorationEnabled=true' in services, True)
installed = read(N / 'code8-installed-package.txt').decode('utf-8-sig')
check('installed/versionCode8', bool(re.search(r'versionCode=8\s',installed)), True)

def private_values(states):
    out = set()
    for s in states:
        for p in s['Players']:
            out.update([p['Id'],p['Name']])
        m = s.get('Match')
        if m:
            out.update([m['Id'],m['CivilianWord'],m['UndercoverWord']])
            for p in m['Participants']: out.update([p['Id'],p['DisplayName']])
    return sorted(v for v in out if v)

logs = []
diagnostic = re.compile(r'FATAL EXCEPTION|Fatal signal|\bANR\b|AndroidJavaException|NullReferenceException|InvalidOperationException|\bException\b|StackOverflow|OutOfMemoryError',re.I)
for filename, pid, states in [
    ('code8-killed-process-log.txt','13388',[baseline,result]),
    ('code8-five-complete-log.txt','14363',[result,pairs['code8-five-bound-04']['session.json']['state']]),
]:
    raw = read(N / filename)
    text = raw.decode('utf-8-sig')
    lines = [x for x in text.splitlines() if x.strip()]
    tested = private_values(states)
    hits = [{'line': n,'value':v,'text':line} for n,line in enumerate(lines,1) for v in tested if v in line]
    diagnostics = [{'line':n,'text':line} for n,line in enumerate(lines,1) if diagnostic.search(line)]
    pid_values = sorted(set(m.group(1) for line in lines if (m:=re.match(r'^\d\d-\d\d\s+\S+\s+(\d+)\s+',line))))
    warning_errors = [{'line':n,'text':line} for n,line in enumerate(lines,1) if re.match(r'^\d\d-\d\d\s+\S+\s+\d+\s+\d+\s+[WEF]\s',line)]
    check(f'{filename}/onlyExpectedPid',pid_values,[pid])
    check(f'{filename}/exactSensitiveValues',hits,[])
    check(f'{filename}/boundedCrashExceptionPatterns',diagnostics,[])
    logs.append({'file':filename,'pid':pid,'sha256':sha(raw),'savedBytes':len(raw),
                 'nonemptyLines':len(lines),'lfBytes':raw.count(b'\n'),
                 'testedValues':tested,'literalMatches':hits,'diagnosticPattern':diagnostic.pattern,
                 'diagnosticMatches':diagnostics,'warningErrorLines':warning_errors,
                 'limit':'All warning/error severity lines are retained; zero tested crash/exception hits does not mean no platform warnings or no memory issue.'})
reported_scan = js(N / 'code8-five-complete-log.json')

for filename in ['code8-first-cold-doubletap.png','code8-first-result-settled.json',
                 'code8-restarted-accessibility.json','code8-restarted-ready.json','code8-restarted-ready.png',
                 'code8-after-low-memory.txt','code8-first-launch.png','code8-first-launch-settled.png',
                 'code8-five-bound-initial.json','code8-install.txt','code8-pending-nora.png',
                 'code8-five-public-initial-probe-output.txt','code8-five-public-initial-stderr.txt']:
    read(N / filename)

# The first max-card observation is deliberately retained as non-passing for visibility.
first_max = N / 'code8-max-good-header-held.png'
first_max_hash = sha(read(first_max)) if first_max.exists() else None
frozen('after')
failed = [c for c in checks if not c['pass']]
audit = {
    'generatedUtc':datetime.now(timezone.utc).isoformat(),
    'scope':'Offline Code8 five-player update/repeat/public-navigation/low-memory audit. No device/Editor/source operation.',
    'earlierAuditsPreserved':FROZEN,
    'consistencyChecks':{'total':len(checks),'passed':len(checks)-len(failed),'failed':failed,
                         'meaning':'Evidence consistency, including correctly retained failed/aborted observations; not release acceptance.'},
    'comparisons':comparisons,
    'coldRepeat':{'timing':timing,'firstUpToSecondDownMs':gap,'downToDownMs':events[2]['event']-events[0]['event'],
                  'result':result['Match'],'primarySha256':pairs['code8-first-cold-doubletap']['session.json']['sha256']},
    'fivePlayerPublicSequence':{'classification':'Eight observed native public actions after restart; bounded pass.',
                               'sequence':sequence,'actions':actions,'serviceRecord':services},
    'initialAbortedRun':{'classification':'Did not pass; zero actions. OS low-memory exit, retained separately.',
                        'sequence':aborted,'initialTree':initial_tree,'exitRecord':exit_first,
                        'crashBufferBytes':len(crash),'fatalLogBytes':len(fatal),
                        'limit':'OS-reported exit reason identifies the termination category; no root cause, allocation leak, or general memory-safety conclusion follows.'},
    'retainedLogRescans':logs,'originalFivePlayerLogMetadata':reported_scan,
    'firstMaxCoveredObservation':{'file':first_max.name,'sha256':first_max_hash,
                                  'classification':'Root reports stationary post-resume capture remained covered. Not private-visible proof, even if helper textual checks pass.'},
    'pending':['Code8 twenty-player/private-visibility/semantic captures are being collected by root; not inferred in this five-player audit.',
               'Final source-pinned APK/AAB verification and release acceptance remain pending.',
               'No physical-device, spoken TalkBack, blind-user usability or group-balance assertion.'],
    'snapshots':snapshots,'checks':checks,'inputs':inputs,
}
(OUT/'code8-native-audit.json').write_text(json.dumps(audit,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')

def h(file): return inputs['work/kings-native/'+file]['sha256']
rows = '\n'.join(f"| {i+1} | {a['label']} | {a['nodeCount']} | `{a['primarySha256']}` |" for i,a in enumerate(actions))
log_rows = '\n'.join(f"| {l['pid']} | {l['nonemptyLines']} | {l['savedBytes']} | {len(l['testedValues'])} | {len(l['literalMatches'])} | `{l['sha256']}` |" for l in logs)
doc = f'''# Code8 native addendum — first cold repeat and five-player public navigation

Frozen offline evidence audit generated {audit['generatedUtc']}. **{len(checks)-len(failed)}/{len(checks)} consistency checks**, {len(snapshots)} saved generations. Original467 and the separate Code7 audit remain byte-identical. Checks include preserving non-passing observations; this is not a final release score. No ADB, Editor, build, push or production-source operation was performed.

## Actual update and first cold repeated confirmation

`code8-update-byte-equality.json` agrees with independently compared Code7 pending baseline → Code8 before-first-launch files. Both are V5: primary **`{pairs['code8-before-first-launch']['session.json']['sha256']}`**, previous **`{pairs['code8-before-first-launch']['session.previous.json']['sha256']}`**. The package record says version1.1.0/code8, min26/target36. The replay-derived pending baseline's provenance remains explicit; the install/update itself is real.

`code8-first-cold-doubletap-timing.json` contains four successful device events, DOWN/UP/DOWN/UP, injectionMode2, returnCode0. The measured first-UP→second-DOWN interval is **76ms** (DOWN→DOWN **{events[2]['event']-events[0]['event']}ms**). The result remains match **`69fc9f23a2b44963b61cc5e954f24af4`**, phase3/Result, outcome6/GoodKingEliminated, primary SHA256 **`{pairs['code8-first-cold-doubletap']['session.json']['sha256']}`**. Roster/history are unchanged; previous equals the exact pending primary. This is a bounded measured repeat pass and does not erase the earlier unmeasured Code7 unintended rematch.

## Initial accessibility attempt: zero actions, low-memory termination

`code8-five-public-sequence.json` is **pass=false with actions=[]**. Its initial capture has {len(initial_tree['nodes'])} nodes and only the retained initial semantics; no completed public-navigation sequence is claimed for this attempt. Android's saved `ApplicationExitInfo #0` says **2026-10-08 23:19:53.427, PID13388, reason3 LOW_MEMORY, status0, RSS543MB**. The saved crash buffer and fatal-log files contain no non-whitespace content. The reason identifies the OS termination category; it does not diagnose an allocation leak or prove memory robustness.

`code8-low-memory-preservation.json` independently checks out: both generations after termination are byte-identical to the cold-repeat result. Primary **`{pairs['code8-after-low-memory']['session.json']['sha256']}`**; previous **`{pairs['code8-after-low-memory']['session.previous.json']['sha256']}`**. This establishes retained committed state, while the interrupted run remains not passed.

## Eight actual public actions after restart

Every action's raw stderr reports `NAMED_APP_ACTION click success=true`; the audit additionally checks the saved resulting tree, exact visible semantic values, nonzero visible semantic bounds, snapshot checksums/hashes, roster and selected mode. All dumps use `dontSuppressAccessibilityServices=true`; the retained service checkpoint records TalkBack bound with touch exploration enabled. This is native hierarchy/action evidence with the service retained, not an audible or blind-user evaluation.

| Step | Actual public action | Resulting nodes | Primary state SHA256 |
| --- | --- | --- | --- |
{rows}

The direct result→Group return now contains35 nodes and expected names/public controls; no extra language redraw is needed in this recorded sequence. Switching Quick/Classic/Kings yields31/32/35 nodes. Start yields16 nodes for the covered first card. Back and abandon confirmation each yield15; confirmed abandon restores35 Group nodes. All five saved player records are unchanged, including Jonas2's stable ID. Back and the confirmation dialog preserve both generations exactly; confirmed abandon clears Match and retains the Group. These five-player observations address the previously failed transition on Code7. Twenty-player and private-card retests are separate.

## Retained log rescans

| PID | Nonempty lines | Saved bytes | Distinct literal values tested | Literal hits | Saved SHA256 |
| --- | --- | --- | --- | --- | --- |
{log_rows}

The killed PID13388 scan covers the actual five player names/IDs, match ID and Ratte/Maus. The later PID14363 scan also includes the actual new handoff's Parkhaus/Parkplatz and new match ID. Both retained logs contain only their expected PID among parsed log records. Neither contains a tested private value or the explicitly listed crash/exception patterns. The JSON retains every W/E/F severity line, including emulator EGL/platform warnings and the `session.json` link-denial diagnostic; **this is not a claim that logs contain no warnings/errors or that LOW_MEMORY was harmless**. The exact scanned values/patterns and raw-file hashes are recorded.

## Evidence limits and remaining gates

The first stationary max-card image `code8-max-good-header-held.png` remains **covered according to root's visual observation** and is not private-visible proof, even if the helper's text checks pass. Its exact SHA256 is `{first_max_hash}`. Root is separately collecting actual pull-held/released and list-bottom evidence plus twenty-player public transitions; none is inferred from this initial frame or from the five-player pass.

Code8 maximal private-card/20-player native checks, final source-pinned APK/AAB hashes/inspection and final release acceptance remain pending within this snapshot. No physical-device, spoken TalkBack, blind-user usability, haptic or human group-balance claim is made. The full input hash inventory, all snapshot states, raw trees, both comparison reports, timing events, exit record and bounded scans are in `code8-native-audit.json`.
'''
(OUT/'code8-native-audit.md').write_text(doc,encoding='utf-8')
ledger = OUT/'acceptance-matrix-draft.md'
old = ledger.read_text(encoding='utf-8')
marker = '\n## Code8 five-player update'
if marker in old: old = old.split(marker)[0]
old += '''
## Code8 five-player update

The56 rows above are frozen mappings through Code7. A separate `code8-native-audit.md/json` now records real Code7→8 byte preservation, a first-cold76ms repeated confirmation that remains on the same terminal result, and eight successful native public actions after restart. Result→Group35, Quick31, Classic32, Kings35, Start16, Back15, Abandon15 and confirmedGroup35 nodes. This adds evidence for stories1–3,38,49,51–52 and the five-player public-navigation part of55. It does not retroactively pass the initial zero-action run that Android terminated for LOW_MEMORY; committed state survived byte-identically. The original467 and Code7 audits remain unchanged.

Twenty-player/private-visibility Code8 follow-up and final source-pinned package/release gates remain pending. The initial stationary max good-King header image remained covered and is explicitly not accepted as private-visible evidence. See the Code8 addendum for exact hashes, timing, bounded log scans and limitations.
'''
ledger.write_text(old,encoding='utf-8')
print(json.dumps({'checks':audit['consistencyChecks'],'snapshots':len(snapshots),'inputs':len(inputs),
                  'logs':[{k:l[k] for k in ['pid','savedBytes','nonemptyLines','sha256','literalMatches','diagnosticMatches']} for l in logs],
                  'outputs':{n:sha((OUT/n).read_bytes()) for n in ['code8-native-audit.md','code8-native-audit.json','acceptance-matrix-draft.md']}},ensure_ascii=False,indent=2))
if failed: raise SystemExit(1)
