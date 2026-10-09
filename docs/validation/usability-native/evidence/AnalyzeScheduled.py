"""Summarize real presentation/input timestamps, including incomplete coverage."""
from pathlib import Path
import json, math, statistics, sys
ROOT = Path(__file__).resolve().parent
def stats(values):
    if not values: return None
    ordered = sorted(values)
    return {'count':len(values), 'medianMs':statistics.median(values),
            'p95Ms':ordered[math.ceil(len(ordered)*.95)-1], 'percentileMethod':'nearest-rank', 'maxMs':max(values),
            'over25ms':sum(x>25 for x in values), 'over50ms':sum(x>50 for x in values),
            'over100ms':sum(x>100 for x in values)}
summaries = []
for label in sys.argv[1:]:
    data = json.loads((ROOT/(label+'-input.json')).read_text(encoding='utf-8'))
    measurement = json.loads((ROOT/label/'measurement.json').read_text(encoding='utf-8'))
    presents = [x/1e6 for x in measurement['actualPresentNs']]
    intervals = list(zip(presents,presents[1:]))
    events = data['events']
    cycles = []
    for start, down in enumerate(events):
        if down['action'] != 0: continue
        end = next(i for i in range(start+1,len(events)) if events[i]['action']==1)
        up = events[end]
        moves = [e['event'] for e in events[start:end] if e['action']==2]
        cycles.append({'downMs':down['event'], 'upMs':up['event'],
            'complete':bool(presents and presents[0]<=down['event'] and presents[-1]>=up['event']+400),
            'dragFrames':stats([b-a for a,b in intervals if down['event']<=a and b<=up['event']]),
            'returnFramesFirst300ms':stats([b-a for a,b in intervals if up['event']<=a and b<=up['event']+300]),
            'actualMoveEventGap':stats([b-a for a,b in zip(moves,moves[1:])]),
            'injectionCallMs':stats([e['finished']-e['started'] for e in events[start:end+1]])})
    summaries.append({'label':label, 'kind':data['kind'], 'versionCode':data['versionCode'],
        'geometry':data['geometry'], 'presentedFrames':len(presents),
        'actualWholeWindow':stats([b-a for a,b in intervals]),
        'completeCycles':sum(c['complete'] for c in cycles), 'cycles':cycles})
output = ROOT / ('analysis-'+'--'.join(sys.argv[1:])+'.json')
if output.exists(): raise FileExistsError(output)
output.write_text(json.dumps(summaries,indent=2),encoding='utf-8')
for row in summaries:
    print(json.dumps({k:v for k,v in row.items() if k!='cycles'}))
    print('Per-cycle drag and actual move-event gaps:', [(c['dragFrames'],c['actualMoveEventGap']) for c in row['cycles']])
