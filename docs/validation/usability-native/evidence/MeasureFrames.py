"""Read native SurfaceFlinger presentation timestamps on the exact owned AVD.

No counter/log clear. Captures unmodified output and deduplicates timestamps
across snapshots. Gesture interval is explicit and performed by the caller.
"""
from pathlib import Path
import json, re, shlex, statistics, subprocess, sys, time
ROOT = Path(__file__).resolve().parent
ADB = [r'C:\Users\lucac\AppData\Local\Unity\Editors\6000.3.25f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe', '-P', '5038', '-s', '127.0.0.1:5583']
def adb(*args):
    return subprocess.run(ADB + list(args), capture_output=True, check=True, timeout=30).stdout
if adb('shell','getprop','ro.boot.qemu.avd_name').decode().strip() != 'word_deduction_api36_16k': raise RuntimeError('Wrong AVD')
label = sys.argv[1]
duration = float(sys.argv[2]) if len(sys.argv)>2 else 5
out = ROOT / label
out.mkdir(exist_ok=False)
listing=adb('shell','dumpsys SurfaceFlinger --list').decode()
(out/'layers.txt').write_text(listing, encoding='utf-8')
layer=next(re.search(r'RequestedLayerState\{(.*?) parentId=',line).group(1) for line in listing.splitlines() if 'SurfaceView[com.giarrel.worddeduction/' in line and '(BLAST)' in line)
start=int(float(adb('shell','cat /proc/uptime').decode().split()[0])*1e9)
(out/'collection-start.json').write_text(json.dumps({'startDeviceUptimeNs':start}),encoding='utf-8')
before=time.perf_counter()
presented=set()
index=0
while time.perf_counter()-before < duration:
    raw=adb('shell','dumpsys SurfaceFlinger --latency '+shlex.quote(layer)).decode()
    (out/f'latency-{index:03}.txt').write_text(raw,encoding='utf-8')
    for line in raw.splitlines()[1:]:
        vals=line.split()
        if len(vals)==3:
            actual=int(vals[1])
            if start <= actual < 9223372036854775807: presented.add(actual)
    index+=1
    time.sleep(.3)
ordered=sorted(presented)
intervals=[(b-a)/1e6 for a,b in zip(ordered,ordered[1:])]
result={'label':label,'layer':layer,'startDeviceUptimeNs':start,'durationHostSeconds':time.perf_counter()-before,'samples':index,'actualPresentNs':ordered,'intervalMs':intervals,'count':len(ordered)}
if intervals:
    s=sorted(intervals)
    result['statistics']={'meanMs':statistics.mean(intervals),'medianMs':statistics.median(intervals),'p95Ms':s[int((len(s)-1)*.95)],'maxMs':max(intervals),'fps':1e3/statistics.mean(intervals),'over25ms':sum(x>25 for x in intervals),'over50ms':sum(x>50 for x in intervals),'over100ms':sum(x>100 for x in intervals)}
(out/'measurement.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in result.items() if k not in ('actualPresentNs','intervalMs')},indent=2))
