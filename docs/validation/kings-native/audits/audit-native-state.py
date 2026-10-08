"""Read-only audit of preserved code5 native evidence; writes only this audit's outputs."""
from pathlib import Path
import base64
import datetime
import hashlib
import json
import re
import subprocess

ROOT = Path('C:/Users/lucac/Documents/Codex/2026-10-06/sie')
NATIVE = ROOT / 'work/kings-native'
OUT = ROOT / 'work/kings-implementation'
REPO = ROOT / 'outputs/word-deduction'
COMMIT = '908d95fc80fc8173df02593422ff85264089b945'
checks, inputs, snapshots = [], {}, {}

def digest(raw):
    return hashlib.sha256(raw).hexdigest()

def record(path):
    raw = path.read_bytes()
    inputs[path.relative_to(ROOT).as_posix()] = {'sha256': digest(raw), 'bytes': len(raw)}
    return raw

def read_json(path):
    return json.loads(record(path).decode('utf-8-sig'))

def check(name, passed, detail=None):
    item = {'name': name, 'pass': bool(passed)}
    if detail is not None:
        item['detail'] = detail
    checks.append(item)
    return bool(passed)

def read_pair(folder):
    if folder in snapshots:
        return snapshots[folder]
    directory = (NATIVE / folder).resolve()
    manifest = read_json(directory / 'manifest.json')
    pair = {}
    for filename in ('session.json', 'session.previous.json'):
        raw = record(directory / filename)
        envelope = json.loads(raw.decode('utf-8-sig'))
        payload_bytes = envelope['Payload'].encode('utf-8')
        checksum = base64.b64encode(hashlib.sha256(payload_bytes).digest()).decode('ascii')
        state = json.loads(envelope['Payload'])
        file_hash = digest(raw)
        check(f'{folder}/{filename}: payload checksum', checksum == envelope['Checksum'])
        check(f'{folder}/{filename}: manifest file hash', file_hash == manifest['files'][filename])
        pair[filename] = {'state': state, 'version': envelope['Version'], 'sha256': file_hash,
                          'payloadSha256': digest(payload_bytes), 'checksum': envelope['Checksum']}
    primary = pair['session.json']['state']
    check(f'{folder}: manifest extracted fields',
          manifest.get('match') == primary.get('Match') and manifest.get('players') == primary['Players']
          and manifest.get('used_pairs') == primary['History']['UsedPairIds'])
    snapshots[folder] = pair
    return pair

def primary(folder):
    return read_pair(folder)['session.json']['state']

def diff(a, b, prefix=''):
    if isinstance(a, dict) and isinstance(b, dict):
        return sum((diff(a[k], b[k], prefix+'/'+k) if k in a and k in b
                    else [prefix+'/'+k] for k in sorted(a.keys() | b.keys())), [])
    if isinstance(a, list) and isinstance(b, list) and len(a) == len(b):
        return sum((diff(x, y, prefix+'/'+str(i)) for i, (x, y) in enumerate(zip(a, b))), [])
    return [] if a == b else [prefix]

def legacy_diff(old, new, prefix=''):
    if isinstance(old, dict) and isinstance(new, dict):
        return sum((legacy_diff(value, new[key], prefix+'/'+key) if key in new else [prefix+'/'+key]
                    for key, value in old.items()), [])
    if isinstance(old, list) and isinstance(new, list) and len(old) == len(new):
        return sum((legacy_diff(x, y, prefix+'/'+str(i)) for i, (x, y) in enumerate(zip(old, new))), [])
    return [] if old == new else [prefix]

source_blobs = {}
for name in ('Match.cs', 'SnapshotStore.cs', 'WordHistory.cs'):
    path = 'game/Assets/WordDeduction/Session/' + name
    raw = subprocess.check_output(['git', '-c', 'safe.directory='+REPO.as_posix(), '-C', str(REPO), 'show', COMMIT+':'+path])
    source_blobs[path] = {'commit': COMMIT, 'sha256': digest(raw), 'bytes': len(raw)}

actual_update = read_json(NATIVE / 'actual-update-verified.json')
notes = record(NATIVE / 'observations-code5.md').decode('utf-8-sig')
record(OUT / 'native-double-tap-finding.md')
install_text = record(NATIVE / 'code5-install.txt').decode('utf-8-sig')
check('update provenance pins code4 -> code5', actual_update['fromVersionCode'] == 4
      and actual_update['toVersionCode'] == 5 and actual_update['sourceCommit'] == COMMIT)
check('preserved installation output reports success', 'Success' in install_text)

comparison_records = []
for name in ('code5-update-bytes.json', 'code5-unfinished-nora-restored-comparison.json',
             'code5-game3-branch-lock.json', 'code5-game3-answer-lock.json',
             'code5-game5-target-persistence.json'):
    comparison = read_json(NATIVE / name)
    before = read_pair(comparison['before']+'-state')
    after = read_pair(comparison['after']+'-state')
    verified = []
    for generation in comparison['generations']:
        filename = generation['file']
        a, b = before[filename], after[filename]
        equal = a['sha256'] == b['sha256']
        check(name+': '+filename+' byte equality independently recomputed', equal)
        check(name+': '+filename+' recorded hashes and versions agree',
              a['sha256'] == generation['beforeSha256'] and b['sha256'] == generation['afterSha256']
              and a['version'] == generation['beforeVersion'] and b['version'] == generation['afterVersion']
              and generation['byteEqual'] == equal and generation['pass'] is True)
        verified.append({'file': filename, 'beforeSha256': a['sha256'], 'afterSha256': b['sha256'], 'byteEqual': equal})
    comparison_records.append({'record': name, 'before': comparison['before'], 'after': comparison['after'], 'verified': verified})

for before_name, after_name in (('before-emulator-recovery-state', 'after-emulator-recovery-state'),
                                ('code4-immediately-before-install-state', 'code5-resumed-old-match-state')):
    a, b = read_pair(before_name), read_pair(after_name)
    rows = []
    for filename in a:
        equal = a[filename]['sha256'] == b[filename]['sha256']
        check(before_name+' -> '+after_name+': '+filename+' byte equality', equal)
        rows.append({'file': filename, 'beforeSha256': a[filename]['sha256'], 'afterSha256': b[filename]['sha256'], 'byteEqual': equal})
    comparison_records.append({'before': before_name, 'after': after_name, 'verified': rows})

migration_record = read_json(NATIVE / 'code5-migrated-advance.json')
old = read_pair('code4-immediately-before-install-state')['session.json']
advanced = read_pair('code5-first-advanced-state')
new = advanced['session.json']
migration_differences = legacy_diff(old['state'], new['state'])
check('first durable advance migrates V4 to V5; V4 primary becomes exact backup',
      old['version'] == 4 and new['version'] == 5 and advanced['session.previous.json']['version'] == 4
      and old['sha256'] == advanced['session.previous.json']['sha256'])
check('first durable advance changes only Handoff among legacy fields',
      migration_differences == ['/Match/Handoff'] and old['state']['Match']['Handoff'] == 1
      and new['state']['Match']['Handoff'] == 2, migration_differences)
check('migration record matches actual new primary', new['sha256'] == migration_record['primarySha256'])

baseline = primary('code5-kings-five-group-state')
renamed = primary('code5-renamed-group-state')
rename_diff = diff(baseline, renamed)
check('rename changes only fifth player Name', rename_diff == ['/Players/4/Name'], rename_diff)
check('Jonas -> Jonas2 retains stable identity', baseline['Players'][4]['Name'] == 'Jonas'
      and renamed['Players'][4]['Name'] == 'Jonas2' and baseline['Players'][4]['Id'] == renamed['Players'][4]['Id'])
check('rename backup is exact unrenamed five-player primary',
      read_pair('code5-renamed-group-state')['session.previous.json']['sha256'] == read_pair('code5-kings-five-group-state')['session.json']['sha256'])
expected_roster = renamed['Players']
expected_ids = [p['Id'] for p in expected_roster]
expected_names = [p['Name'] for p in expected_roster]

expected_outcomes = {2: (7, 'OnlyKingsRemain', 'evil team'), 3: (8, 'KingsWordCorrect', 'evil team'),
                     4: (9, 'KingsWordIncorrect', 'good team'), 5: (10, 'KingsKingCorrect', 'evil team'),
                     6: (11, 'KingsKingIncorrect', 'good team'), 7: (6, 'GoodKingEliminated', 'evil team')}
games = []
for game, (outcome, outcome_name, winner) in expected_outcomes.items():
    folder = f'code5-game{game}-'+('terminal-state' if game == 3 else 'result-state')
    pair = read_pair(folder)
    s, prev = pair['session.json']['state'], pair['session.previous.json']['state']
    m, pm = s['Match'], prev['Match']
    check(f'game{game}: result phase and expected outcome', m['Phase'] == 3 and m['Mode'] == 2 and m['Outcome'] == outcome)
    check(f'game{game}: predecessor has same match and no outcome', pm['Id'] == m['Id'] and pm['Outcome'] is None)
    check(f'game{game}: expected predecessor phase', pm['Phase'] == {2:6,3:10,4:10,5:11,6:11,7:6}[game])
    check(f'game{game}: both generations retain exact roster', s['Players'] == expected_roster and prev['Players'] == expected_roster)
    check(f'game{game}: both generations retain participant order/IDs/names', all(
        [p['Id'] for p in st['Match']['Participants']] == expected_ids and
        [p['DisplayName'] for p in st['Match']['Participants']] == expected_names for st in (s, prev)))
    for key in ('Id','Mode','Language','PairId','CivilianWord','UndercoverWord','StartingIndex','Handoff','GoodKingId','LastChanceTarget'):
        check(f'game{game}: terminal preserves predecessor {key}', m[key] == pm[key])
    check(f'game{game}: terminal preserves predecessor history', s['History'] == prev['History'])
    if game == 2:
        survivors = [p for p in m['Participants'] if not p['Eliminated']]
        check('game2: exactly good King and White survive', len(survivors) == 2 and
              any(p['Id'] == m['GoodKingId'] for p in survivors) and any(p['Role'] == 2 for p in survivors))
    if game in (3,4,5,6):
        white = next(p for p in m['Participants'] if p['Role'] == 2)
        check(f'game{game}: eliminated White owns last chance', white['Eliminated'] and m['Suspect'] == white['Id'])
    if game in (5,6):
        check(f'game{game}: King target comparison matches outcome', (m['LastChanceTarget'] == m['GoodKingId']) == (game == 5))
    if game == 7:
        check('game7: eliminated suspect is good King', m['Suspect'] == m['GoodKingId'] and
              next(p for p in m['Participants'] if p['Id'] == m['GoodKingId'])['Eliminated'])
    deal = primary(f'code5-game{game}-deal-state')['Match']
    for key in ('Id','Mode','Language','PairId','CivilianWord','UndercoverWord','StartingIndex','GoodKingId'):
        check(f'game{game}: original deal preserves {key}', m[key] == deal[key])
    check(f'game{game}: original assignment identities/names/roles preserved',
          [{k:p[k] for k in ('Id','DisplayName','Role')} for p in deal['Participants']] ==
          [{k:p[k] for k in ('Id','DisplayName','Role')} for p in m['Participants']])
    games.append({'game': game, 'folder': folder, 'matchId': m['Id'], 'phase': m['Phase'],
                  'previousPhase': pm['Phase'], 'outcome': outcome, 'outcomeName': outcome_name, 'winner': winner,
                  'pairId': m['PairId'], 'usedCount': len(s['History']['UsedPairIds']),
                  'remainingCount': len(s['History']['RemainingPairIds']),
                  'primarySha256': pair['session.json']['sha256'], 'previousSha256': pair['session.previous.json']['sha256']})
check('games2-7 cover six distinct expected endings', sorted(g['outcome'] for g in games) == list(range(6,12)))

game1_pair = read_pair('code5-game1-result-state')
game1 = game1_pair['session.previous.json']['state']
check('game1 terminal is backup; primary is unintended game2 handoff', game1['Match']['Phase'] == 3
      and game1['Match']['Outcome'] == 6 and game1_pair['session.json']['state']['Match']['Phase'] == 0
      and game1_pair['session.json']['state']['Match']['Id'] == games[0]['matchId'])
check('game1 terminal retains same five-person roster', game1['Players'] == expected_roster)
check('game1 terminal retains participant order/IDs/names',
      [p['Id'] for p in game1['Match']['Participants']] == expected_ids and
      [p['DisplayName'] for p in game1['Match']['Participants']] == expected_names)
game1_deal = primary('code5-kings-game1-deal-state')['Match']
check('game1 terminal preserves original assignment IDs/names/roles',
      [{k:p[k] for k in ('Id','DisplayName','Role')} for p in game1_deal['Participants']] ==
      [{k:p[k] for k in ('Id','DisplayName','Role')} for p in game1['Match']['Participants']])
bad_game3 = primary('code5-game3-result-state')['Match']
check('mislabelled game3 result is nonterminal judgment', bad_game3['Phase'] == 10 and bad_game3['Outcome'] is None)

history_steps = []
last_history = baseline['History']
sequence = [(1, game1)] + [(g['game'], primary(g['folder'])) for g in games]
for game, state in sequence:
    history, pair_id = state['History'], state['Match']['PairId']
    check(f'game{game}: used history appends exactly this pair', history['UsedPairIds'] == last_history['UsedPairIds']+[pair_id])
    check(f'game{game}: remaining queue removes only this pair, order preserved',
          history['RemainingPairIds'] == [v for v in last_history['RemainingPairIds'] if v != pair_id])
    check(f'game{game}: bounded recent queue keeps latest ten', history['RecentPairIds'] == (last_history['RecentPairIds']+[pair_id])[-10:])
    check(f'game{game}: no duplicate/lost/disjoint pair entries',
          len(history['UsedPairIds']) == len(set(history['UsedPairIds'])) and
          len(history['RemainingPairIds']) == len(set(history['RemainingPairIds'])) and
          not set(history['UsedPairIds']) & set(history['RemainingPairIds']) and
          set(history['UsedPairIds']+history['RemainingPairIds']) == set(baseline['History']['UsedPairIds']+baseline['History']['RemainingPairIds']))
    history_steps.append({'game':game, 'addedPairId':pair_id, 'usedPairIds':history['UsedPairIds'],
                          'recentPairIds':history['RecentPairIds'], 'remainingCount':len(history['RemainingPairIds'])})
    last_history = history
final_group = primary('code5-group-after-seven-games-state')
check('post-seven-game group preserves roster/history', final_group['Players'] == expected_roster and final_group['History'] == last_history)

target_before = primary('code5-game5-target-pending-state')['Match']
check('game5 persisted pending wrong target is Nora before later correction', target_before['Phase'] == 11
      and target_before['LastChanceTarget'] == expected_ids[0] and target_before['LastChanceTarget'] != target_before['GoodKingId'])

max_clamped = primary('code5-max-clamped-state')
max_end = primary('code5-max-end-state')
active_count = sum(p['Active'] for p in max_end['Players'])
effective_limit = max(0,(active_count-1)//2-1)
check('max group retains all20 players,18 active and preference8', all(
    len(s['Players']) == 20 and sum(p['Active'] for p in s['Players']) == 18
    and s['KingsUndercoverPreference'] == 8 for s in (max_clamped,max_end)))
check('max group after mode switches retains exact roster and returns to Kings',
      max_clamped['Players'] == max_end['Players'] and max_end['Mode'] == 2 and max_end['Match'] is None)
group_trees = []
for index, mode in enumerate(('Schnell','Klassisch','Könige')):
    name = f'code5-group-return-repro-{index}.json'
    tree = read_json(NATIVE/name)
    values = [node.get(k) for node in tree['nodes'] for k in ('text','description') if node.get(k) not in (None,'','null')]
    selected = [node['description'] for node in tree['nodes'] if node.get('selected') and node.get('description') in ('Schnell','Klassisch','Könige')]
    check(name+': selected mode and18 active agree', selected == [mode] and '18 spielen mit' in values)
    group_trees.append({'file':name,'sha256':inputs[(NATIVE/name).relative_to(ROOT).as_posix()]['sha256'],
                        'selectedMode':selected,'completeSemanticValues':values})
check('max Kings semantics shows computed effective7 while saved preference remains8', effective_limit == 7
      and '10 Gute inkl. König · 7 Undercover + Mr. White' in group_trees[2]['completeSemanticValues'])
max_group = {'provenance':'Synthetic max roster and actual subsequent native group actions; not an eighth authentic played match.',
             'savedPreference':max_end['KingsUndercoverPreference'],'active':active_count,'totalPlayers':len(max_end['Players']),
             'effectiveUndercoverLimit':effective_limit,'modeSwitchTrees':group_trees,
             'returnTransitionBug':'Root-observed accessibility transition defect remains pending correction; these settled mode trees do not disprove it.'}

classic_source = read_pair('../android-16kb-probe/final-long-word-state')
classic_before = read_pair('code5-old-classic-before-resume-state')
classic_after = read_pair('code5-old-classic-after-advance-state')
for filename in classic_source:
    check('Classic fixture replay before resume preserves '+filename, classic_source[filename]['sha256'] == classic_before[filename]['sha256'])
cb, ca = classic_before['session.json'], classic_after['session.json']
classic_differences = legacy_diff(cb['state'],ca['state'])
check('Classic first advance V4->V5 preserves exact V4 primary as backup', cb['version'] == 4 and ca['version'] == 5
      and classic_after['session.previous.json']['version'] == 4
      and classic_after['session.previous.json']['sha256'] == cb['sha256'])
check('Classic migration changes only Handoff1->2 among legacy fields', classic_differences == ['/Match/Handoff']
      and cb['state']['Match']['Handoff'] == 1 and ca['state']['Match']['Handoff'] == 2, classic_differences)
check('Classic preserves actual match and word pair', ca['state']['Match']['Id'] == 'b7203215ad014899bfda2147685f8ddd'
      and ca['state']['Match']['Mode'] == 1 and ca['state']['Match']['Phase'] == 0
      and [ca['state']['Match']['CivilianWord'],ca['state']['Match']['UndercoverWord']] == ['Sprachnachricht','Videoanruf'])
classic_compatibility = {'provenance':'Explicit replay of a preserved real V4 Classic fixture; compatibility evidence, not a second in-place install update.',
                         'sourceFolder':'work/android-16kb-probe/final-long-word-state','matchId':ca['state']['Match']['Id'],
                         'beforePrimarySha256':cb['sha256'],'afterPrimarySha256':ca['sha256'],
                         'afterBackupSha256':classic_after['session.previous.json']['sha256'],
                         'legacyChangedPaths':classic_differences,'handoffBefore':1,'handoffAfter':2,
                         'oldVersion':4,'newVersion':5,'backupVersion':4}

fixture_manifest = read_json(NATIVE/'session-fixtures/manifest.json')
max_cards = []
fixture_states = {}
for fixture, stem, replay in (
    ('German-GoodKing','code5-de-good-talkback-private','code5-german-good-max-replay'),
    ('English-EvilKing','code5-en-evil-max-header','code5-en-evil-max-replay'),
    ('German-EvilKing','code5-de-evil-max-header','code5-de-evil-max-replay'),
    ('English-GoodKing','code5-en-good-max-header','code5-en-good-max-replay')):
    entry = next(f for f in fixture_manifest['fixtures'] if f['name'] == fixture)
    replay_manifest = read_json(NATIVE/replay/'manifest.json')
    envelope = read_json(NATIVE/'session-fixtures'/fixture/'session.json')
    payload = envelope['Payload'].encode('utf-8')
    check(f'{fixture}: synthetic fixture checksum', base64.b64encode(hashlib.sha256(payload).digest()).decode() == envelope['Checksum'])
    fixture_sha = inputs[(NATIVE/'session-fixtures'/fixture/'session.json').relative_to(ROOT).as_posix()]['sha256']
    check(f'{fixture}: replay expected primary matches fixture', fixture_sha == replay_manifest['expected']['session.json'])
    state = json.loads(envelope['Payload'])
    fixture_states[fixture] = state
    m = state['Match']
    owner = m['Participants'][m['Handoff']]
    check(f'{fixture}: owner agrees with fixture manifest', owner['Id'] == entry['owner']['Id'] and owner['DisplayName'] == entry['owner']['DisplayName'])
    de = fixture.startswith('German')
    expected = ['Zurück','Könige','Hilfe',f'KARTE {m["Handoff"]+1} VON 20',owner['DisplayName'],
                'Gib dieser Person das Handy.',
                'Zum Lesen hochziehen. Für mehr hoch- oder runterstreichen. Loslassen verdeckt alles.',
                'Private Karte. Wörter werden nie vorgelesen. Zum visuellen Lesen TalkBack kurz durchreichen lassen und dieses Feld halten. Loslassen verdeckt die Karte.',
                'Handy weitergeben'] if de else [
                'Back','Kings','Help',f'CARD {m["Handoff"]+1} OF 20',owner['DisplayName'],
                'Pass the phone to this person.',
                'Pull up to read. Slide up or down for more. Release to hide.',
                "Private card. Words are never spoken. To read it visually, use TalkBack's pass-through gesture, then hold here. Release to hide.",
                'Hide & pass on']
    secret_values = [m['CivilianWord'],m['UndercoverWord']]+[p['DisplayName'] for p in m['Participants'] if p['Id'] != owner['Id']]
    stages = []
    for stage in ('held','released'):
        filename = f'{stem}-{stage}-tree.json'
        tree = read_json(NATIVE/filename)
        fields = [{'node':i,'field':key,'value':node.get(key)} for i,node in enumerate(tree['nodes'])
                  for key in ('text','description') if node.get(key) not in (None,'','null')]
        values = [f['value'] for f in fields]
        # Exact allowlist rejects concatenated role/name/word associations, not only standalone secrets.
        hits = [v for v in secret_values if v in values]
        check(f'{fixture}/{stage}: complete semantic values are public allowlist only', values == expected,
              None if values == expected else values)
        check(f'{fixture}/{stage}: no full non-owner participant name or word exposed', not hits, hits)
        action_strings = list(dict.fromkeys(node.get('actions','') for node in tree['nodes']))
        action_labels = [label for text in action_strings for label in re.findall(r'AccessibilityAction: [A-Z0-9_]+ - ([^,\]]+)',text)]
        check(f'{fixture}/{stage}: no custom accessibility-action label', action_labels and all(label == 'null' for label in action_labels))
        stages.append({'file':filename,'sha256':inputs[(NATIVE/filename).relative_to(ROOT).as_posix()]['sha256'],
                       'nodeCount':len(tree['nodes']),'completeSemanticFields':fields,'completeActionStrings':action_strings,'secretExactValueHits':hits,
                       'publicAllowlistMatches':values == expected})
    check(f'{fixture}: held/released semantic text identical', stages[0]['completeSemanticFields'] == stages[1]['completeSemanticFields'])
    max_cards.append({'fixture':fixture,'provenance':'Synthetic replay; never counted among seven actual matches.',
                      'fixtureSha256':fixture_sha,'ownerId':owner['Id'],'publicOwnerName':owner['DisplayName'],
                      'pairWords':[m['CivilianWord'],m['UndercoverWord']],
                      'knownSecretParticipantNames':[p['DisplayName'] for p in entry['knownParticipants']],
                      'allNonOwnerNamesChecked':[p['DisplayName'] for p in m['Participants'] if p['Id'] != owner['Id']],
                      'stages':stages})

original_log_record = read_json(NATIVE/'code5-de-good-max-log.json')
log_raw = record(NATIVE/'code5-de-good-max-log.txt')
log_text = log_raw.decode('utf-8-sig')
de_fixture = fixture_states['German-GoodKing']
de_match = de_fixture['Match']
log_values = list(dict.fromkeys([de_match['CivilianWord'],de_match['UndercoverWord'],de_match['Id'],de_match['GoodKingId']]
    + [p['Name'] for p in de_fixture['Players']] + [p['DisplayName'] for p in de_match['Participants']]
    + [p['Id'] for p in de_match['Participants']]))
log_hits = [{'value':value,'lineNumbers':[i for i,line in enumerate(log_text.splitlines(),1) if value.casefold() in line.casefold()]}
            for value in log_values if value.casefold() in log_text.casefold()]
check('German Good retained log is nonempty and includes its recorded PID', bool(log_text.strip()) and str(original_log_record['pid']) in log_text)
check('German Good actual pair is Pfirsich/Aprikose', [de_match['CivilianWord'],de_match['UndercoverWord']] == ['Pfirsich','Aprikose'])
check('German Good corrected full-fixture-value log scan has no matches', not log_hits, log_hits)
log_rescan = {'file':'code5-de-good-max-log.txt','sha256':digest(log_raw),'bytes':len(log_raw),'lineCount':len(log_text.splitlines()),
              'pid':original_log_record['pid'],'method':'Case-insensitive literal search of actual pair, all raw/display names and match/player IDs.',
              'testedValues':log_values,'hits':log_hits,
              'originalGap':'Original code5-de-good-max-log.json tested Nektarine and omitted actual Aprikose; original preserved, this offline rescan closes that value gap.'}
log_rescan['rawLineFeeds'] = log_raw.count(b'\n')
log_rescan['nonEmptyLines'] = len([line for line in log_text.splitlines() if line.strip()])
log_rescan['originalRecordedBytes'] = original_log_record['bytes']
log_rescan['originalRecordedLines'] = original_log_record['lines']
log_rescan['bytesAfterNormalizingDoubledCR'] = len(log_raw.replace(b'\r\r\n',b'\r\n'))

end_record = read_json(NATIVE/'code5-max-end-log.json')
end_raw = record(NATIVE/'code5-max-end-log.txt')
end_text = end_raw.decode('utf-8-sig')
end_values = list(dict.fromkeys(['Kirsche','Pflaume','Peach','Apricot']
    + [p['Name'] for p in max_end['Players']]
    + [p['Name']+' · '+str(p['Number']) if p['Distinguished'] else p['Name'] for p in max_end['Players']]
    + [p['Id'] for p in max_end['Players']]))
end_hits = [{'value':value,'lineNumbers':[i for i,line in enumerate(end_text.splitlines(),1) if value.casefold() in line.casefold()]}
            for value in end_values if value.casefold() in end_text.casefold()]
check('max end retained log includes PID10823 and is nonempty', end_record['pid'] == '10823' and '10823' in end_text and bool(end_text.strip()))
check('max end corrected20-player name/ID and actual word scan has no matches', not end_hits, end_hits)
end_rescan = {'file':'code5-max-end-log.txt','sha256':digest(end_raw),'bytes':len(end_raw),
              'rawLineFeeds':end_raw.count(b'\n'),'nonEmptyLines':len([line for line in end_text.splitlines() if line.strip()]),
              'pid':end_record['pid'],'rosterSource':'code5-max-end-state/session.json','playersCovered':len(max_end['Players']),
              'testedValues':end_values,'hits':end_hits,
              'originalGap':'Original record tested four words only. This offline rescan adds all20 players using real Name/Distinguished/Number fields plus IDs; original preserved.'}

snapshot_output = {}
for folder, pair in snapshots.items():
    snapshot_output[folder] = {}
    for filename, item in pair.items():
        m = item['state'].get('Match')
        snapshot_output[folder][filename] = {k:item[k] for k in ('version','sha256','payloadSha256','checksum')}
        snapshot_output[folder][filename].update({'matchId':m['Id'] if m else None,
                                                'phase':m['Phase'] if m else None,
                                                'outcome':m['Outcome'] if m else None})

audit = {'auditVersion':1, 'generatedAtUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),
         'scope':'Offline consistency of preserved code5 actual states, plus separately identified synthetic max-card semantic trees/log; not source-code6, visual, physical-device or release acceptance.',
         'sourceCommit':COMMIT, 'reportedApkSha256':actual_update['apkSha256'],
         'apkHashIndependentlyRecomputed':False, 'sourceBlobs':source_blobs,
         'summary':{'checks':len(checks),'passed':sum(c['pass'] for c in checks),'failed':sum(not c['pass'] for c in checks),
                    'snapshotFiles':sum(len(p) for p in snapshots.values()),'allOfflineChecksPass':all(c['pass'] for c in checks)},
         'terminalGames':games,'roster':expected_roster,'renameChangedPaths':rename_diff,
         'history':{'baselineUsedPairs':baseline['History']['UsedPairIds'],'steps':history_steps,
                    'finalUsedPairs':last_history['UsedPairIds'],'finalRecentPairs':last_history['RecentPairIds'],
                    'recentLimit':10,'catalogCycleTotal':len(last_history['UsedPairIds'])+len(last_history['RemainingPairIds'])},
         'comparisons':comparison_records,'migrationLegacyChangedPaths':migration_differences,
         'syntheticMaxCardSemantics':max_cards,'correctedGermanGoodLogRescan':log_rescan,
         'maxGroupPreferenceAndModeChecks':max_group,'classicV4FixtureCompatibility':classic_compatibility,
         'correctedMaxEndLogRescan':end_rescan,
         'evidenceCautions':[
            'Game1 good-king-result screenshot is an unintended new handoff: double-tap failure; terminal exists only in backup.',
            'Game3 result-state is phase10; terminal-state is the actual phase3 result.',
            'Game3 judgment-resumed and word-correct-result image labels do not establish visual resume/result; parent notes report pause.',
            'Game5 target-safe-restart image was startup blank; later byte equality proves durable target, not that frame.',
            'Observations-code5.md is an in-progress earlier note: pending game5/6/7 assertions are superseded only for durable state by these files.',
            'Four synthetic max-card held/released trees are separately audited for exact public semantic strings; no inference of screenshot legibility, spoken output, touch usability or all accessibility passing.',
            'Original English Good MaxFixturePass stopped on a false substring match: private display name ending in middle-dot 1 is a prefix of public owner ending in middle-dot 10. Both complete trees are checked here; original run is not relabelled passed.',
            'Root reports a real max-group accessibility return-transition bug pending correction; this narrow private-tree audit does not override it.',
            'Known code5 cross-screen double-tap failure remains; code6 verification is pending.'],
         'snapshots':snapshot_output,'inputs':dict(sorted(inputs.items())),'checks':checks}
destination = OUT/'native-state-audit.json'
destination.write_text(json.dumps(audit,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(json.dumps(audit['summary']))
print(json.dumps([c for c in checks if not c['pass']],ensure_ascii=False))
print('auditJsonSha256='+digest(destination.read_bytes()))
