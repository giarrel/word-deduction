"""Read-only evidence audit; writes only distinct Code7 addendum/draft files."""
from pathlib import Path
import base64
import hashlib
import json
import re
import subprocess
from datetime import datetime, timezone

BASE = Path(__file__).resolve().parents[2]
OUT = BASE / 'work/kings-implementation'
NATIVE = BASE / 'work/kings-native'
REPO = BASE / 'outputs/word-deduction'
CORRECTION = BASE / 'work/kings-worktrees/final-corrections/artifacts/kings-final-corrections'
PIN = '85fa17555b44bca61640bcb13a738c3e37ffc34f'
ORIGINALS = {
    'native-state-audit.md': 'b57b872ef09df20097fe128082d8f1a9ee05c35526f244bd403f1325469a1ef9',
    'native-state-audit.json': 'f207fae79deb64cf21891558a43036d1463eba5dc9b2509af3a4fcc261612d19',
    'audit-native-state.py': '8a2a5227fdd89a668a1f6df79275be09811ada92cb38674bad449be24428cff4',
}
inputs = {}
checks = []
snapshots = {}

def sha(data):
    return hashlib.sha256(data).hexdigest()

def read(path):
    data = path.read_bytes()
    key = path.relative_to(BASE).as_posix()
    evidence = {'sha256': sha(data), 'bytes': len(data)}
    if key in inputs:
        assert inputs[key] == evidence, f'Input changed during audit: {key}'
    inputs[key] = evidence
    return data

def js(path):
    return json.loads(read(path).decode('utf-8-sig'))

def check(name, actual, expected):
    checks.append({'name': name, 'actual': actual, 'expected': expected, 'pass': actual == expected})

def original_check(stage):
    for name, expected in ORIGINALS.items():
        check(f'original467/{stage}/{name}', sha(read(OUT / name)), expected)

original_check('before')

def load_pair(label):
    folder = NATIVE / label
    manifest = js(folder / 'manifest.json') if (folder / 'manifest.json').exists() else None
    pair = {}
    for filename in ['session.json', 'session.previous.json']:
        path = folder / filename
        raw = read(path)
        envelope = json.loads(raw.decode('utf-8-sig'))
        payload = envelope['Payload']
        computed = base64.b64encode(hashlib.sha256(payload.encode('utf-8')).digest()).decode('ascii')
        state = json.loads(payload)
        key = path.relative_to(BASE).as_posix()
        check(f'{label}/{filename}/checksum', computed, envelope['Checksum'])
        check(f'{label}/{filename}/schema', envelope['Version'], 5)
        if manifest is not None:
            check(f'{label}/{filename}/manifestHash', sha(raw), manifest['files'][filename])
        match = state.get('Match')
        snapshot = {'path': key, 'sha256': sha(raw), 'bytes': len(raw),
                    'version': envelope['Version'], 'checksum': envelope['Checksum'],
                    'computedChecksum': computed, 'state': state}
        snapshots[key] = snapshot
        pair[filename] = snapshot
        if filename == 'session.json' and manifest is not None:
            check(f'{label}/manifestMatch', match, manifest['match'])
            check(f'{label}/manifestPlayers', state['Players'], manifest['players'])
            check(f'{label}/manifestHistory', state['History']['UsedPairIds'], manifest['used_pairs'])
    return pair

labels = [
    'code5-final-update-baseline-state', 'code5-game3-word-committed-state',
    'code7-before-first-launch-state', 'code7-resumed-before-confirm-state',
    'code7-doubletap-good-king-result-state', 'code7-sync-doubletap-result-state',
    'code7-word-answer-repeat-state', 'code7-word-judgment-repeat-state',
    'code7-before-next-update-state',
]
pairs = {label: load_pair(label) for label in labels}
baseline = pairs['code5-final-update-baseline-state']
wordbase = pairs['code5-game3-word-committed-state']

comparisons = []
for name, target in [('code7-update-byte-equality.json', 'code7-before-first-launch-state'),
                     ('code7-resumed-byte-equality.json', 'code7-resumed-before-confirm-state')]:
    reported = js(NATIVE / name)
    computed = {f: read(NATIVE / 'code5-final-update-baseline-state' / f) == read(NATIVE / target / f)
                for f in baseline}
    for f, equal in computed.items():
        check(f'{name}/{f}/independentByteEquality', equal, True)
    check(f'{name}/reportedPass', reported['pass'], True)
    comparisons.append({'file': name, 'reported': reported, 'independentByteEquality': computed})

replays = []
for label, source in [('code7-timing-sync-replay', baseline),
                      ('code7-word-repeat-replay-replay', wordbase)]:
    manifest = js(NATIVE / label / 'manifest.json')
    before = load_pair(label + '/before')
    for filename in source:
        check(f'{label}/{filename}/expectedFixtureHash', manifest['expected'][filename], source[filename]['sha256'])
        check(f'{label}/{filename}/preservedBeforeHash', manifest['before'][filename], before[filename]['sha256'])
    check(f'{label}/explicitReplay', manifest['provenance'], 'Explicit fixture replay, not Android-played match')
    replays.append({'path': label, 'manifest': manifest})

def projection(snapshot):
    s = snapshot['state']
    m = s['Match']
    return {k: m[k] for k in ['Id', 'Mode', 'Language', 'PairId', 'CivilianWord', 'UndercoverWord',
                              'StartingIndex', 'GoodKingId', 'Handoff'] } | {
        'participants': [{k: p[k] for k in ['Id', 'DisplayName', 'Role']} for p in m['Participants']],
        'players': s['Players'], 'history': s['History']}

measured = []
for prefix, source, phase, outcome, gap in [
    ('code7-sync-doubletap', baseline, 3, 6, 75),
    ('code7-word-answer-repeat', wordbase, 10, None, 76),
    ('code7-word-judgment-repeat', pairs['code7-word-answer-repeat-state'], 3, 9, 75),
]:
    timing = js(NATIVE / (prefix + '-timing.json'))
    result_label = prefix + ('-result-state' if prefix == 'code7-sync-doubletap' else '-state')
    result = pairs[result_label]
    m = result['session.json']['state']['Match']
    events = timing['deviceEvents']
    computed_gap = events[2]['event'] - events[1]['event']
    check(f'{prefix}/eventActions', [e['action'] for e in events], [0, 1, 0, 1])
    check(f'{prefix}/injected', [e['injected'] for e in events], ['true'] * 4)
    check(f'{prefix}/returnCode', timing['returnCode'], 0)
    check(f'{prefix}/actualUpToDownGap', computed_gap, gap)
    check(f'{prefix}/reportedGap', timing['actualBetweenTapsMs'], computed_gap)
    check(f'{prefix}/phase', m['Phase'], phase)
    check(f'{prefix}/outcome', m['Outcome'], outcome)
    check(f'{prefix}/sameDealRosterHistory', projection(result['session.json']), projection(source['session.json']))
    check(f'{prefix}/backupExactlyPrecedingPrimary', result['session.previous.json']['sha256'], source['session.json']['sha256'])
    measured.append({'label': prefix, 'classification': 'bounded measured native replay observation',
                     'timing': timing, 'computedFirstUpToSecondDownMs': computed_gap,
                     'computedDownToDownMs': events[2]['event'] - events[0]['event'],
                     'matchId': m['Id'], 'phase': m['Phase'], 'outcome': m['Outcome'],
                     'primarySha256': result['session.json']['sha256'],
                     'previousSha256': result['session.previous.json']['sha256']})

unmeasured = pairs['code7-doubletap-good-king-result-state']
um = unmeasured['session.json']['state']['Match']
check('unmeasured/observedNewMatch', um['Id'] != baseline['session.json']['state']['Match']['Id'], True)
check('unmeasured/observedNewHandoff', [um['Phase'], um['Handoff'], um['Outcome']], [0, 0, None])
check('unmeasured/previousEqualsMeasuredTerminal', unmeasured['session.previous.json']['sha256'],
      pairs['code7-sync-doubletap-result-state']['session.json']['sha256'])
check('beforeNextUpdate/primary', pairs['code7-before-next-update-state']['session.json']['sha256'],
      pairs['code7-word-judgment-repeat-state']['session.json']['sha256'])
check('beforeNextUpdate/previous', pairs['code7-before-next-update-state']['session.previous.json']['sha256'],
      pairs['code7-word-judgment-repeat-state']['session.previous.json']['sha256'])

trees = {}
for filename in ['code7-result-public.json', 'code7-result-to-group.json', 'code7-result-to-group-settled.json']:
    tree = js(NATIVE / filename)
    semantic = [{'index': i, 'field': field, 'value': node[field]}
                for i, node in enumerate(tree['nodes']) for field in ['text', 'description']
                if node.get(field) not in [None, '', 'null']]
    trees[filename] = {'nodeCount': len(tree['nodes']), 'semanticValueCount': len(semantic),
                       'semanticValues': semantic, 'tree': tree}
    check(f'{filename}/servicesNotSuppressed', tree['dontSuppressAccessibilityServices'], True)
    if 'to-group' in filename:
        check(f'{filename}/reproducedEmptyFrameworkNodes', len(tree['nodes']), 7)
        check(f'{filename}/reproducedNoSemanticValues', semantic, [])
        check(f'{filename}/reproducedDisabledInvisibleLastNode',
              {k: tree['nodes'][-1][k] for k in ['bounds', 'enabled', 'visible', 'actions']},
              {'bounds': '0 0 0 0', 'enabled': False, 'visible': False, 'actions': '[]'})
    else:
        check(f'{filename}/resultHasPublicSemantics', len(semantic) > 0, True)
services = read(NATIVE / 'code7-result-to-group-services.txt').decode('utf-8-sig')
service_lines = [line.strip() for line in services.splitlines() if any(
    x in line for x in ['touchExplorationEnabled=', 'Bound services:', 'Enabled services:'])]
check('groupA11y/TalkBackBound', 'Bound services:{Service[label=TalkBack,' in services, True)
check('groupA11y/touchExploration', 'touchExplorationEnabled=true' in services, True)
installed = read(NATIVE / 'code7-installed-package.txt').decode('utf-8-sig')
check('package/versionCode7', bool(re.search(r'versionCode=7\s', installed)), True)
check('package/versionName110', 'versionName=1.1.0' in installed, True)

correction = {}
for filename, expected in [('20-a11y-late-red-result.json', (1, 0, 1)), ('21-a11y-late-green-result.json', (1, 1, 0))]:
    obj = js(CORRECTION / filename)
    result = obj['data']['result']
    summary = result['summary']
    check(f'{filename}/testSummary', [summary[k] for k in ['total', 'passed', 'failed']], list(expected))
    check(f'{filename}/completed', result['status'], 'completed')
    correction[filename] = obj
for filename in ['late-layout-standards-review.md', 'late-layout-spec-review.md']:
    body = read(OUT / filename).decode('utf-8-sig')
    check(f'{filename}/exactCommit', PIN in body, True)
    correction[filename] = body

source_files = {}
for path in ['game/Assets/WordDeduction/UI/AccessibleMenu.cs',
             'game/Assets/WordDeduction/Tests/PlayMode/KingsEliminationScreenTests.cs']:
    proc = subprocess.run(['git', '-c', f'safe.directory={REPO.as_posix()}', '-C', str(REPO),
                           'show', f'{PIN}:{path}'], check=True, capture_output=True)
    source_files[path] = {'commit': PIN, 'sha256': sha(proc.stdout), 'bytes': len(proc.stdout),
                          'relevantLines': [{'line': i, 'text': line} for i, line in
                            enumerate(proc.stdout.decode('utf-8-sig').splitlines(), 1)
                            if any(x in line for x in ['refreshFrame', 'schedule.Execute', 'WaitForEndOfFrame',
                                                      'new[] { 5, 20 }', 'new[] { false, true }'])]}

# Freeze the evidence named in this bounded addendum. Do not absorb later Code8 captures.
for prefix in ['code7-before-first-launch', 'code7-resumed-before-confirm', 'code7-doubletap-good-king-result',
               'code7-sync-doubletap-result', 'code7-word-answer-repeat', 'code7-word-judgment-repeat',
               'code7-before-next-update']:
    for suffix in ['-summary.txt', '.png']:
        path = NATIVE / (prefix + suffix)
        if path.exists():
            read(path)
for filename in ['code7-result-to-group.png', 'code7-result-public-stderr.txt',
                 'code7-result-to-group-stderr.txt', 'code7-result-to-group-settled-stderr.txt', 'code7-install.txt',
                 'observations-code5.md']:
    read(NATIVE / filename)
for filename in ['acceptance-matrix.md']:
    read(REPO / 'docs/validation/kings' / filename)
read(REPO / 'docs/validation/kings-acceptance/report.md')
read(REPO / 'docs/specs/kings-mode.md')
original_check('after')
failed = [c for c in checks if not c['pass']]
audit = {
    'generatedUtc': datetime.now(timezone.utc).isoformat(),
    'scope': 'Offline consistency addendum for retained Code7 evidence. No device/Editor/build/source mutation.',
    'original467Preserved': ORIGINALS,
    'consistencyChecks': {'total': len(checks), 'passed': len(checks)-len(failed), 'failed': failed,
                          'meaning': 'Consistency with expected observations, including known app failures; NOT app acceptance.'},
    'actualCode5To7Update': comparisons,
    'explicitReplays': replays,
    'measuredRepeatedInputs': measured,
    'unmeasuredRepeatedInput': {
        'classification': 'Unintended rematch observed; actual tap timing unresolved; not a passing test.',
        'primarySha256': unmeasured['session.json']['sha256'],
        'previousSha256': unmeasured['session.previous.json']['sha256'], 'match': um,
        'timingRecord': None,
        'screenshotVisualInspection': 'Shows covered Nora card 1 of 5, not a terminal result.'},
    'publicResultToGroup': {
        'classification': 'Reproduced Code7 native accessibility failure; final correction retest pending.',
        'trees': trees, 'serviceLines': service_lines,
        'screenshotVisualInspection': 'German Group, 5 active, Nora and Luca visible, Kings selected, roster/input/start controls rendered.',
        'limit': 'Screenshot confirms the visible Group; state is paired to the reported transition, not a fresh post-return device capture.'},
    'laterSourceCorrection': {'commit': PIN, 'sourceFiles': source_files, 'evidence': correction,
                             'standardsFindings': 0, 'specFindings': 0,
                             'nativeCode8': 'Pending within this frozen addendum; no Code8 evidence read.'},
    'snapshots': snapshots, 'checks': checks, 'inputs': inputs,
    'limits': ['Measured repeat-input runs are explicit fixture replays, not additional full Android-played games.',
               'The real Code5-to-Code7 install is distinct from the separately replayed timing/word checkpoints.',
               'No new visual/audio/physical-device claims beyond the two screenshots inspected offline.',
               'No final release/package or all-native-pass assertion. Earlier failed and mislabelled evidence remains unchanged.'],
}
(OUT / 'code7-native-audit.json').write_text(json.dumps(audit, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')

def evidence_hash(rel):
    return inputs['work/kings-native/' + rel]['sha256']

rows = '\n'.join(f"| `{r['label']}` | `{r['matchId']}` | {r['computedFirstUpToSecondDownMs']} ms | {r['phase']} | {r['outcome'] if r['outcome'] is not None else 'null'} | `{r['primarySha256']}` |" for r in measured)
doc = f'''# Code7 native evidence addendum

Frozen offline audit; generated {audit['generatedUtc']}. This does not replace or rerun the original **467-check** audit. **{len(checks)-len(failed)}/{len(checks)} evidence-consistency checks agree with the expected records**; those records include the still-failing accessibility transition. This count is not an app acceptance score. No ADB, Unity, build, push or production-source edits were performed.

The original Markdown, JSON and audit script are byte-identical before/after this audit:

| Original file | SHA256 |
| --- | --- |
''' + '\n'.join(f'| `{k}` | `{v}` |' for k,v in ORIGINALS.items()) + f'''

## Actual Code5 → Code7 update

Independently recomputed file equality agrees with both `code7-update-byte-equality.json` and `code7-resumed-byte-equality.json`. Both V5 generations of `code5-final-update-baseline-state` are unchanged at `code7-before-first-launch-state` and `code7-resumed-before-confirm-state`:

- Primary: `{baseline['session.json']['sha256']}`.
- Previous: `{baseline['session.previous.json']['sha256']}`.

Match `69fc9f23a2b44963b61cc5e954f24af4` remains Kings, phase6/TablePlay, outcome null, handoff5, pending suspect Nora, Ratte/Maus. The five-player roster (including stable-ID Jonas2), roles, settings and history are identical because the complete files are identical. The retained installed-package record says version1.1.0/code7, min26/target36. This is the actual in-place update observation, distinct from the later fixture replays. The replayed starting baseline itself was explicitly reconstructed from earlier Android gameplay, as recorded in the Code5 notes; no claim that it was never replayed is made.

## Measured repeated input

| Capture | Same match ID | First UP → second DOWN | Phase | Outcome | Primary SHA256 |
| --- | --- | --- | --- | --- | --- |
{rows}

Each timing file records action sequence `[0,1,0,1]`, four `injected=true` device events, injectionMode2 and returnCode0. Recomputing device event timestamps gives **75/76/75 ms**. DOWN→DOWN is **{measured[0]['computedDownToDownMs']}/{measured[1]['computedDownToDownMs']}/{measured[2]['computedDownToDownMs']} ms**, a different measurement. The host-requested spacing was75 ms; it is not substituted for the recorded device gap.

The synchronized good-King confirmation stays on the same game's phase3/GoodKingEliminated(6), rather than creating a new deal. The word-answer repeat stops at phase10/KingsWordJudgment with null outcome; the repeated judgment ends phase3/KingsWordIncorrect(9), awarding the good team. IDs, assigned roles/words/King, roster, handoff and complete history remain unchanged through each measured transition. Each previous file equals the exact preceding primary. Both generations of `code7-before-next-update-state` equal the word-judgment result pair.

These measured cases use explicit replay manifests `code7-timing-sync-replay` and `code7-word-repeat-replay-replay`. Their expected fixture hashes match the actual earlier saved files, and their preserved-before hashes match the saved backup copies. They are bounded native regression observations, not three new fully played matches or a proof for every timing window.

## Initial unmeasured attempt remains unresolved

`code7-doubletap-good-king-result-state` actually contains a new match **`{um['Id']}`**, phase0/handoff0/outcome null, Parkplatz/Parkhaus. Its primary SHA256 is `{unmeasured['session.json']['sha256']}`; backup is the old match's terminal result `{unmeasured['session.previous.json']['sha256']}`. Offline image inspection confirms `code7-doubletap-good-king-result.png` is covered Nora/card1of5, not the intended result screen.

No actual event-timing record exists for that initial attempt in the supplied evidence. The unintended rematch observation is retained as unresolved timing evidence. The subsequent measured cases do not turn the original attempt into a pass or establish its cause.

## Public Result → Group still fails on Code7

`code7-result-public.json` has **{trees['code7-result-public.json']['nodeCount']} nodes / {trees['code7-result-public.json']['semanticValueCount']} nonempty semantic values**. Both immediate and settled Group captures contain **seven framework nodes, zero nonempty text/description values**, and a final disabled/invisible node with zero bounds and no actions. Both dumps explicitly preserve accessibility services. The saved service record has TalkBack bound and touch exploration enabled.

The offline-inspected screenshot displays the real German Group with **5 playing**, Nora and Luca visible, selected Kings mode, entry field and start controls. This is not merely a screenshot of a blank screen or the synthetic twenty-player fixture. Root associates these retained records with the actual result→Group action; this audit did not acquire a new post-return save or query the device.

| Retained transition evidence | SHA256 |
| --- | --- |
''' + '\n'.join(f'| `{f}` | `{evidence_hash(f)}` |' for f in ['code7-result-public.json','code7-result-to-group.json','code7-result-to-group-settled.json','code7-result-to-group.png','code7-result-to-group-services.txt']) + f'''

This remains an actual **Code7 accessibility defect**. Empty private-card nodes alone are not a success criterion for public navigation. Neither a later redraw nor a green rendered test retroactively makes this transition pass.

## Later source correction and pending gates

The later correction **`{PIN}`** lets the panel scheduler run before waiting until a later frame to rebuild public accessibility. Exact source blob hashes/line excerpts are in the JSON. The retained late-frame test has a behavioral RED: completed1, passed0, failed1, message “A newly visible Group must restore its public controls and nested player list without another redraw action” with an empty actual string. The GREEN of the same named test is completed1, passed1, failed0. The test source uses end-of-frame transitions; the reviews cover Group returns and subsequent start at5/20 participants.

`late-layout-standards-review.md`: **0 new violations, 0 new reportable baseline smells**. `late-layout-spec-review.md`: **0 findings**. Both reviews identify the exact later source pin and explicitly withhold corrected native acceptance. Their hashes and complete retained text, together with RED/GREEN result JSON, are frozen in this addendum's machine-readable evidence.

**Code8 corrected-native accessibility transitions, final source-pinned package verification and final release acceptance remain pending within this addendum.** No Code8 captures are read or inferred here. The separate `acceptance-matrix-draft.md` maps all56 stories to prior source/rendered and bounded native evidence; it is a coverage draft, not a release assertion. Earlier mislabelled images, failed interaction attempts, Classic-fixture provenance, silent-emulator/physical-device limitations and the original467 audit remain intact.
'''
(OUT / 'code7-native-audit.md').write_text(doc, encoding='utf-8')

# Retain the exact 56 requirements and ticket ownership from the source ledger.
matrix = read(REPO / 'docs/validation/kings/acceptance-matrix.md').decode('utf-8-sig')
native_notes = {}
def note(ids, text):
    for i in ids:
        native_notes[i] = text
note([1,3,4,5,9], 'Code5 actual Group: add Mila/Jonas, pause/restore minimum, rename Jonas→Jonas2; original audit verifies stable IDs. Public a11y return remains blocked on Code7.')
note([2], 'Actual old Quick update/play/rematch; explicit old Classic V4 resume/advance replay. These have different provenance; see original audit.')
note([6,12,13], 'Seven actual Kings deals preserve valid assigned roles/King identities in snapshots. Randomness/possible repeats are Session-test claims, not statistically proved by seven games.')
note([7,8,10,11], 'Synthetic max native setup: manual preference8 retained at18active/effective7; saved mode-switch trees Quick/Classic/Kings. Original audit validates counts and retained preference; transition a11y remains open.')
note([14,15], 'Actual DE deals and seven-game persistent history independently audited; EN/DE max cards use explicitly synthetic fixtures.')
note([16,17,18,19,20,21,22], 'Code5 actual role-card observations plus four explicit DE/EN good/evil King max fixtures. Eight held/released native trees exactly public-only; original audit preserves secret exclusions. No spoken/private-audio claim.')
note([23,54], 'Root visually inspected all four native max headers and list bottoms at150%/360×640dp, including duplicate disambiguators/Unicode; synthetic provenance and bounded tree/log audit retained.')
note([24,25,27], 'Code5 actual card holds/drags and native max list reachability observed. Timing freedom and alternate input also have rendered evidence; no blind-user usability claim.')
note([26], 'Code5 real release/Home/Recents/cancel/second-pointer observations recorded; held/released trees omit private associations. Final-candidate native regression still pending.')
note([28,29], 'Actual unfinished Nora force-stop/resume keeps both generations byte-equal and covered; completed handoff progression recorded. No later private access remains covered in Session/rendered suites.')
note([30,31,32,41,43], 'Bilingual table/help and native Help scrolling observed; interpersonal speech, voting, no-advice and semantic judgment are group rules, not machine-enforced native assertions.')
note([33,34,35,37], 'Actual ordinary eliminations, neutral “Kein König”, parity continuation, cancel/correct pending Mila→Nora and target Nora→Jonas2 recorded; snapshots audited.')
note([36,50], 'Actual results disclose all original participants including eliminated members; six terminal outcomes and rosters audited. Code7 public result has semantic nodes; result→Group loses them.')
note([38], 'Code5 games1/7 GoodKingEliminated(6); game1 unintended rematch preserved. Measured Code7 replay75ms stays same69fc9f atphase3/outcome6; initial unmeasured Code7 rematch unresolved.')
note([39,40,42,47], 'Actual last-chance choice/answer/target restart generations byte-equal. Measured Code7 spoken-answer repeat76ms stops same5dfed8 phase10/null; do not treat wrong earlier result label as terminal.')
note([44], 'Actual game5 pending wrong Nora survives restart; corrected Jonas2 produces correct King ending. Game6 targets Luca and fails; both final snapshots audited.')
note([45], 'All four last-chance outcomes8/9/10/11 independently audited from actual games3–6. Measured Code7 judgment repeat75ms stays same5dfed8 phase3/outcome9.')
note([46], 'Actual game2 result: exactly two Kings remain, phase3/outcome7; source-state survivor identities audited.')
note([48], 'Storage fault/atomic replacement and recovery covered by retained Session74/74 and focused3/3. No new native forced-save-failure claim in this offline addendum.')
note([49], 'Actual code4→5 live Quick bytes/migration and actual code5→7 bytes independently audited. V4 Classic separately replayed/advanced safely; not a second real-install update.')
note([51], 'Actual intentional one-action rematches restore active group. Initial code5 and unmeasured code7 unintended rematches remain recorded; three measured Code7 repeats are bounded passes only.')
note([52], 'Actual deliberate abandon preserves Group/roster; native public accessibility on return fails Code5/Code7 and needs corrected-package retest.')
note([53], 'Actual DE gameplay/EN Group and four DE/EN max fixtures, plus full rendered bilingual coverage. Not every translated string was separately spoken or inspected natively.')
note([55], 'OPEN NATIVE DEFECT: Code7 result→Group immediate/settled7empty nodes with TalkBack bound; private-tree secrecy passes bounded audit. Later85fa175 renderedRED/GREEN and reviews0; Code8native not yet accepted.')
note([56], 'Root records offline API36 native execution; prior APK/AAB manifests inspected noINTERNET/new services and retained logs scanned. Final correction package metadata/hash gate remains pending.')
draft = ['# Kings 56-story coverage draft — through frozen Code7 evidence', '',
         'Outside-repository coordination draft. The exact requirements/ticket ownership below come from the tracked acceptance matrix; this file does not change its acceptance status. Prior source908d95fc evidence is Session74/74 and rendered44/44 as documented in `outputs/word-deduction/docs/validation/kings-acceptance/report.md`; it is not rerun here. Native mappings use `native-state-audit.md/json` (original467), root’s retained Code5 observations, and `code7-native-audit.md/json`.', '',
         '**Open cross-cutting gates:** corrected Code8 native public accessibility transitions, final source-pinned APK/AAB identity/signature/inspection/hashes, and final integrated release acceptance. Later85fa175 review deltas have0Standards/0Spec findings and focused renderedRED/GREEN, which do not close native gates. No physical-device, spoken TalkBack, blind-user, group balance or production-store claim.', '',
         '| Story | Exact requirement | Ticket | Evidence mapping and limits |',
         '| --- | --- | --- | --- |']
count = 0
for line in matrix.splitlines():
    cells = [x.strip() for x in line.strip().strip('|').split('|')]
    if len(cells) == 4 and cells[0].isdigit():
        n = int(cells[0]); count += 1
        draft.append(f'| {n} | {cells[1]} | {cells[2]} | {native_notes[n]} |')
assert count == 56 and set(native_notes) == set(range(1, 57))
(OUT / 'acceptance-matrix-draft.md').write_text('\n'.join(draft)+'\n', encoding='utf-8')
print(json.dumps({'checks': audit['consistencyChecks'], 'snapshotFiles': len(snapshots),
                  'inputFiles': len(inputs), 'outputs': {name: sha((OUT/name).read_bytes()) for name in
                  ['code7-native-audit.md','code7-native-audit.json','acceptance-matrix-draft.md']}}, ensure_ascii=False, indent=2))
if failed:
    raise SystemExit(1)
