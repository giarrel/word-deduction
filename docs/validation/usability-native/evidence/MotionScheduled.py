"""Identical device-scheduled gestures for warm code8/candidate comparison.

No fixture replacement or installation. Run only on the verified prepared screen.
Actual Android event timestamps, not requested spacing, establish input coverage.
"""
from pathlib import Path
import argparse, json, re, subprocess, sys, time

ROOT = Path(__file__).resolve().parent
ADB = [r'C:\Users\lucac\AppData\Local\Unity\Editors\6000.3.25f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe', '-P', '5038', '-s', '127.0.0.1:5583']
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('label')
parser.add_argument('kind', choices=['scroll', 'pull'])
parser.add_argument('--version-code', type=int, required=True)
parser.add_argument('--x', type=int, default=500)
parser.add_argument('--start-y', type=int)
parser.add_argument('--end-y', type=int)
args = parser.parse_args()
assert re.fullmatch('[a-z0-9-]+', args.label)
assert not (ROOT / args.label).exists()
def adb(*values):
    return subprocess.run(ADB + list(values), capture_output=True, check=True, timeout=30).stdout
assert adb('shell', 'getprop', 'ro.boot.qemu.avd_name').decode().strip() == 'word_deduction_api36_16k'
package = adb('shell', 'dumpsys', 'package', 'com.giarrel.worddeduction').decode()
assert re.search(r'\bversionCode=' + str(args.version_code) + r'\b', package)
assert 'DEBUGGABLE' not in package
y0 = args.start_y if args.start_y is not None else 800 if args.kind == 'scroll' else 1160
y1 = args.end_y if args.end_y is not None else 570 if args.kind == 'scroll' else 880
commands = ['begin']
for cycle in range(4):
    start = 1000 + cycle * 1500
    first, last = (y1, y0) if args.kind == 'scroll' and cycle % 2 else (y0, y1)
    commands += [f'at {start}', f'down 0 {args.x} {first}']
    for step in range(1, 61):
        commands += [f'at {start + round(step * 1000 / 60)}', f'move 0 {args.x} {first + (last-first)*step/60:.3f}']
    commands += [f'at {start + 1150 if args.kind == "pull" else start + 1001}', 'up 0']
commands += ['quit']
measure = subprocess.Popen([sys.executable, str(ROOT/'MeasureFrames.py'), args.label, '9'], stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
deadline = time.monotonic() + 15
while not (ROOT/args.label/'collection-start.json').exists():
    if measure.poll() is not None or time.monotonic() > deadline:
        raise RuntimeError('Frame collection failed to start: ' + measure.communicate(timeout=5)[0])
    time.sleep(.05)
started = time.monotonic()
result = subprocess.run(ADB + ['shell', 'CLASSPATH=/data/local/tmp/word-deduction-scheduled-input.jar', 'app_process', '/system/bin', 'WordDeductionScheduledInput', '0'], input='\n'.join(commands)+'\n', capture_output=True, text=True, timeout=20)
measurement_output = measure.communicate(timeout=15)[0]
report = {'label': args.label, 'kind': args.kind, 'versionCode': args.version_code,
          'geometry': {'x':args.x, 'startY':y0, 'endY':y1}, 'cycles':4, 'stepsPerCycle':60,
          'dragDurationMs':1000, 'schedule':'device uptime, whole batch sent without per-move host roundtrip',
          'commands':commands, 'stdout':result.stdout, 'stderr':result.stderr, 'exitCode':result.returncode,
          'durationHostSeconds':time.monotonic()-started, 'measurementOutput':measurement_output}
events = [dict(action=int(a), event=int(e), started=int(s), finished=int(f), injected=v=='true')
          for a,e,s,f,v in re.findall(r'TIMING action=(\d+) event=(\d+) started=(\d+) finished=(\d+) injected=(true|false)', result.stdout)]
report['events'] = events
(ROOT/(args.label+'-input.json')).write_text(json.dumps(report, indent=2), encoding='utf-8')
assert result.returncode == 0 and measure.returncode == 0, report
assert len(events) == 4*62 and all(e['injected'] for e in events), report
assert [e['action'] for e in events].count(0) == 4 and [e['action'] for e in events].count(1) == 4
print(measurement_output)
print('Recorded all four device-timed gestures and their actual dispatch timestamps.')
