"""Prepare isolated diagnostic copies; never modify the prior evidence helpers."""
from pathlib import Path

root = Path(__file__).resolve().parent
source = root.parent / 'kings-native/input-timing/WordDeductionInputTiming.java'
destination = root / 'input-timing'
destination.mkdir(exist_ok=True)
java = source.read_text(encoding='utf-8')
java = java.replace('WordDeductionInputTiming', 'WordDeductionScheduledInput')
java = java.replace('long downTime = 0;', 'long downTime = 0;\n    long scheduleOrigin = -1;')
marker = 'if (p[0].equals("quit")) break;'
assert java.count(marker) == 1
java = java.replace(marker, marker + '''
      if (p[0].equals("begin")) {
        if (!ids.isEmpty()) throw new IllegalArgumentException("active contact");
        scheduleOrigin = SystemClock.uptimeMillis();
        System.out.println("BEGIN uptime=" + scheduleOrigin); System.out.flush();
        continue;
      }
      if (p[0].equals("at")) {
        if (scheduleOrigin < 0) throw new IllegalArgumentException("missing begin");
        long due = scheduleOrigin + Long.parseLong(p[1]);
        long remaining = due - SystemClock.uptimeMillis();
        if (remaining > 0) SystemClock.sleep(remaining);
        continue;
      }
''')
(destination / 'WordDeductionScheduledInput.java').write_text(java, encoding='utf-8')
measure = (root.parent / 'ux-motion-investigation/MeasureFrames.py').read_text(encoding='utf-8-sig')
marker = 'before=time.perf_counter()'
assert measure.count(marker) == 1
measure = measure.replace(marker, "(out/'collection-start.json').write_text(json.dumps({'startDeviceUptimeNs':start}),encoding='utf-8')\n" + marker)
(root / 'MeasureFrames.py').write_text(measure, encoding='utf-8')
print('Prepared new external diagnostic source and frame collector; historical helpers untouched.')
