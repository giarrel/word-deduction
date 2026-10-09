"""Authenticated host framebuffer recording of the owned synthetic test AVD.

Secure-window flags in the app are not changed. Recording is separate from
the unrecorded performance denominator because encoding can perturb timing.
"""
from pathlib import Path
import argparse, re, socket, json, time
ROOT = Path(__file__).resolve().parent
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('action', choices=['help','start','stop'])
parser.add_argument('label', nargs='?')
args = parser.parse_args()
if args.action == 'start':
    assert args.label and re.fullmatch('[a-z0-9-]+',args.label)
    target = ROOT/(args.label+'.webm')
    if target.exists(): raise FileExistsError(target)
with socket.create_connection(('127.0.0.1',5582), timeout=10) as connection:
    connection.settimeout(20)
    def read():
        data=b''
        while True:
            part=connection.recv(8192)
            if not part: raise RuntimeError('Console closed before reply')
            data+=part
            normalized=data.replace(b'\r',b'')
            if normalized.endswith(b'OK\n') or b'KO:' in normalized: return normalized
    def command(value):
        connection.sendall((value+'\n').encode())
        result=read()
        if b'KO:' in result: raise RuntimeError(result.decode(errors='replace'))
        return result
    greeting=read()
    if b'Authentication required' in greeting:
        token=(Path.home()/'.emulator_console_auth_token').read_text().strip()
        command('auth '+token)
    assert b'word_deduction_api36_16k' in command('avd name')
    query = 'help screenrecord start' if args.action == 'help' else 'screenrecord stop' if args.action == 'stop' else 'screenrecord start --time-limit 15 --fps 60 '+target.as_posix()
    started=time.monotonic()
    reply=command(query).decode(errors='replace')
    if args.action=='start':
        (ROOT/(args.label+'-record-start.json')).write_text(json.dumps({'hostMonotonicBefore':started,'hostMonotonicAfter':time.monotonic(),'fpsRequested':60,'maxSeconds':15},indent=2),encoding='utf-8')
    connection.sendall(b'quit\n')
    print(reply)
    if args.action=='help': (ROOT/'console-record-help.txt').write_text(reply,encoding='utf-8')
