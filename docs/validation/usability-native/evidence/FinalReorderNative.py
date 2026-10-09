"""Replay the observed code10 cancel failure on the verified code11 update.

Only the owned AVD. Requires the original five-person group already visible.
Preserves every raw sequence and both save generations; never restores silently.
"""
from pathlib import Path
import json, subprocess, sys, time

root = Path(__file__).resolve().parent
assert json.loads((root/'code10-to11-update-verified.json').read_text(encoding='utf-8'))['pass']
report = root/'code11-native-reorder-result.json'
assert not report.exists()

def helper(name, *args):
    subprocess.run([sys.executable, str(root/name), *args], check=True, timeout=90)

helper('CheckpointRelease.py', 'code11-reorder-start', 'code8-original')
cases = [
    ('cancel', 'begin|at 100|down 7 150 590|at 350|move 7 150 820|at 800|cancel|quit'),
    ('cancel-repeat', 'begin|at 100|down 7 150 590|at 350|move 7 150 820|at 800|cancel|quit'),
    ('cancel-new-contact', 'begin|at 100|down 7 150 590|at 350|move 7 150 820|at 800|cancel|down 9 500 400|at 1100|up 9|quit'),
    ('outside', 'begin|at 100|down 7 150 590|at 350|move 7 150 820|at 700|move 7 1045 820|at 900|up 7|quit'),
]
results=[]
for case, sequence in cases:
    label='code11-native-'+case
    helper('NativeSequence.py', label, sequence, '--version-code', '11')
    time.sleep(1.2)
    helper('CheckpointRelease.py', label, 'code11-reorder-start')
    results.append({'case':case,'pass':True,'assertion':'Both saved generations remain byte-identical to initial state.'})
label='code11-native-valid-drop'
helper('NativeSequence.py',label,'begin|at 100|down 7 150 590|at 350|move 7 150 820|at 900|up 7|quit','--version-code','11')
time.sleep(1.2)
helper('CheckpointRelease.py',label)
before=json.loads(json.loads((root/'code11-reorder-start-state/session.json').read_text(encoding='utf-8'))['Payload'])
after=json.loads(json.loads((root/(label+'-state/session.json')).read_text(encoding='utf-8'))['Payload'])
expected=[p['Id'] for p in before['Players']]
expected[0],expected[1]=expected[1],expected[0]
assert [p['Id'] for p in after['Players']]==expected
assert after['History']==before['History'] and after['Match']==before['Match']
assert {p['Id']:p['Name'] for p in after['Players']}=={p['Id']:p['Name'] for p in before['Players']}
results.append({'case':'valid-drop-after-aborts','pass':True,'assertion':'Exactly the first two identities move; names, history and match unchanged.'})
report.write_text(json.dumps({'versionCode':11,'results':results,'pass':True},indent=2),encoding='utf-8')
print(report)
