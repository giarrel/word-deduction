"""Record foreground/window evidence around Home; does not assume transition timing."""
from pathlib import Path
import json, re, subprocess, sys, time
root = Path(__file__).resolve().parent
label, version = sys.argv[1:]
assert re.fullmatch('[a-z0-9-]+',label) and version.isdigit()
directory = root / (label+'-window-evidence'); directory.mkdir(exist_ok=False)
adb = [r'C:\Users\lucac\AppData\Local\Unity\Editors\6000.3.25f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe','-P','5038','-s','127.0.0.1:5583']
def run(*args):
    return subprocess.run(adb+list(args),capture_output=True,check=True,timeout=35).stdout.decode(errors='replace')
assert run('shell','getprop','ro.boot.qemu.avd_name').strip()=='word_deduction_api36_16k'
assert re.search(r'\bversionCode='+version+r'\b',run('shell','dumpsys','package','com.giarrel.worddeduction'))
p = subprocess.Popen(adb+['shell','CLASSPATH=/data/local/tmp/word-deduction-input.jar','app_process','/system/bin','WordDeductionInput'],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True,bufsize=1)
assert p.stdout.readline().startswith('READY')
trace=[]
def command(value):
    p.stdin.write(value+'\n'); p.stdin.flush(); reply=p.stdout.readline().strip()
    trace.append({'command':value,'reply':reply}); assert 'injected=true' in reply
def capture(stage):
    subprocess.run([sys.executable,str(root/'ConsoleScreenshot.py'),label+'-'+stage],check=True,timeout=30)
def state(stage):
    window=run('shell','dumpsys','window')
    activity=run('shell','dumpsys','activity','activities')
    (directory/(stage+'-window.txt')).write_text(window,encoding='utf-8')
    (directory/(stage+'-activity.txt')).write_text(activity,encoding='utf-8')
    focused=[line.strip() for line in window.splitlines() if 'mCurrentFocus=' in line or 'mFocusedApp=' in line]
    resumed=[line.strip() for line in activity.splitlines() if 'topResumedActivity=' in line]
    trace.append({'stage':stage,'hostMonotonic':time.monotonic(),'focused':focused,'resumed':resumed})
    return any('mCurrentFocus=' in line and 'nexuslauncher' in line for line in focused) and any('nexuslauncher' in line for line in resumed)
try:
    command('down 7 500 1160'); command('move 7 500 880'); time.sleep(1)
    state('before'); capture('held-before')
    run('shell','input','keyevent','3')
    for attempt in range(12):
        if state('home-'+str(attempt)): break
        time.sleep(.3)
    else: raise RuntimeError('Home never became the focused/resumed activity')
    capture('launcher-focused'); time.sleep(2); state('home-settled'); capture('launcher-settled')
    command('up 7')
    run('shell','am','start','-n','com.giarrel.worddeduction/com.unity3d.player.UnityPlayerGameActivity')
    time.sleep(3); state('resumed'); capture('resumed')
finally:
    p.stdin.write('cancel\nquit\n'); p.stdin.flush()
    tail=p.communicate(timeout=10)[0]
    (directory/'trace.json').write_text(json.dumps({'versionCode':int(version),'trace':trace,'tail':tail,'exitCode':p.returncode},indent=2),encoding='utf-8')
assert p.returncode==0
