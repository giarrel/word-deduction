"""Initialize native input before starting short capture; avoid recorder timeout."""
from pathlib import Path
import json, re, subprocess, sys, time
root=Path(__file__).resolve().parent
label,version=sys.argv[1:]
assert re.fullmatch('[a-z0-9-]+',label) and version.isdigit()
adb=[r'C:\Users\lucac\AppData\Local\Unity\Editors\6000.3.25f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe','-P','5038','-s','127.0.0.1:5583']
def run(*args):
    return subprocess.run(adb+list(args),capture_output=True,check=True,timeout=40).stdout.decode()
assert run('shell','getprop','ro.boot.qemu.avd_name').strip()=='word_deduction_api36_16k'
assert re.search(r'\bversionCode='+version+r'\b',run('shell','dumpsys','package','com.giarrel.worddeduction'))
p=subprocess.Popen(adb+['shell','CLASSPATH=/data/local/tmp/word-deduction-input.jar','app_process','/system/bin','WordDeductionInput'],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True,bufsize=1)
assert p.stdout.readline().startswith('READY')
trace=[]
def cmd(value):
    before=time.monotonic();p.stdin.write(value+'\n');p.stdin.flush();reply=p.stdout.readline().strip()
    trace.append({'command':value,'before':before,'after':time.monotonic(),'reply':reply})
    assert 'injected=true' in reply
def helper(name,*args):
    subprocess.run([sys.executable,str(root/name),*args],check=True,timeout=40)
try:
    cmd('down 7 500 1160');cmd('move 7 500 880');time.sleep(1)
    helper('ConsoleScreenshot.py',label+'-held')
    helper('ConsoleRecord.py','start',label)
    time.sleep(1)
    cmd('up 7')
    time.sleep(1.2)
    helper('ConsoleRecord.py','stop')
    helper('ConsoleScreenshot.py',label+'-released')
finally:
    p.stdin.write('cancel\nquit\n');p.stdin.flush();tail=p.communicate(timeout=15)[0]
    (root/(label+'-trace.json')).write_text(json.dumps({'trace':trace,'tail':tail,'exitCode':p.returncode},indent=2),encoding='utf-8')
