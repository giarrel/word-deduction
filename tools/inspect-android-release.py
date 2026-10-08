"""Inspect exact APK/AAB artifacts with the installed Android tools. No device access."""
import argparse
import hashlib
import json
import os
import re
import struct
import subprocess
from pathlib import Path
from zipfile import ZipFile

parser = argparse.ArgumentParser()
parser.add_argument("artifact", type=Path)
parser.add_argument("--android-player", required=True, type=Path)
parser.add_argument("--output", required=True, type=Path)
parser.add_argument("--version-code", default=8, type=int)
parser.add_argument("--version-name", default="1.1.0")
args = parser.parse_args()
artifact, sdk, out = args.artifact.resolve(), args.android_player.resolve(), args.output.resolve()
out.mkdir(parents=True, exist_ok=True)
buildtools = sdk / "SDK/build-tools/36.0.0"
java = sdk / "OpenJDK/bin/java.exe"
bundletool = sdk / "Tools/bundletool-all-1.17.2.jar"
readelf = sdk / "NDK/toolchains/llvm/prebuilt/windows-x86_64/bin/llvm-readelf.exe"
commands = []


def run(name, command):
    result = subprocess.run([str(x) for x in command], capture_output=True, text=True, encoding="utf-8", errors="replace",
                            env={**os.environ, "JAVA_HOME": str(sdk / "OpenJDK")})
    (out / (name + ".txt")).write_text(result.stdout + result.stderr, encoding="utf-8")
    commands.append({"name": name, "command": [str(x) for x in command], "exitCode": result.returncode})
    if result.returncode:
        save()
        raise RuntimeError(f"{name} failed ({result.returncode}); inspect saved output")
    return result.stdout


def identity(path):
    with path.open("rb") as stream:
        digest = hashlib.file_digest(stream, "sha256").hexdigest()
    return {"path": str(path), "bytes": path.stat().st_size, "sha256": digest}


report = {"artifact": identity(artifact), "commands": commands, "packages": []}


def save():
    (out / "inspection.json").write_text(json.dumps(report, indent=2), encoding="utf-8")


def inspect_native(path, label):
    rows = []
    with ZipFile(path) as archive, path.open("rb") as raw:
        for info in archive.infolist():
            if info.filename.endswith("boot.config"):
                text = archive.read(info).decode("utf-8")
                (out / (label + "-boot.config.txt")).write_text(text, encoding="utf-8")
                if "player-connection" in text.lower() or "wait-for-managed-debugger=1" in text:
                    raise RuntimeError("Release contains PlayerConnection/debugger boot configuration")
            if not info.filename.endswith(".so"):
                continue
            data = archive.read(info)
            if data[:6] != b"\x7fELF\x02\x01" or struct.unpack_from("<H", data, 18)[0] != 183:
                raise RuntimeError(f"Unexpected non-ARM64 ELF: {info.filename}")
            phoff = struct.unpack_from("<Q", data, 32)[0]
            phsize, phnum = struct.unpack_from("<HH", data, 54)
            segments = []
            for i in range(phnum):
                typ, flags, offset, va, _, filesz, memsz, align = struct.unpack_from("<IIQQQQQQ", data, phoff + i * phsize)
                if typ in (1, 0x6474e552):
                    segments.append({"type": "LOAD" if typ == 1 else "GNU_RELRO", "flags": flags, "offset": offset,
                                     "vaddr": va, "fileBytes": filesz, "memoryBytes": memsz, "align": align})
            loads = [s for s in segments if s["type"] == "LOAD"]
            load_pass = bool(loads) and all(s["align"] >= 16384 and (s["vaddr"] - s["offset"]) % 16384 == 0 for s in loads)
            relros = []
            for relro in (s for s in segments if s["type"] == "GNU_RELRO"):
                start, end = relro["vaddr"], relro["vaddr"] + relro["memoryBytes"]
                lo, hi = start & ~16383, (end + 16383) & ~16383
                conflicts = []
                for load in (s for s in loads if s["flags"] & 2):
                    a, b = load["vaddr"], load["vaddr"] + load["memoryBytes"]
                    for p, q in ((a, min(b, start)), (max(a, end), b)):
                        if max(p, lo) < min(q, hi):
                            conflicts.append([max(p, lo), min(q, hi)])
                relros.append({"start": start, "end": end, "endModulo16k": end % 16384,
                               "roundedProtection": [lo, hi], "conflictingWritableBytes": conflicts})
            raw.seek(info.header_offset)
            header = raw.read(30)
            name_bytes, extra_bytes = struct.unpack_from("<HH", header, 26)
            zip_offset = info.header_offset + 30 + name_bytes + extra_bytes
            file = out / "native" / label / info.filename
            file.parent.mkdir(parents=True, exist_ok=True)
            file.write_bytes(data)
            run(label + "-" + file.name + "-readelf", [readelf, "-Wl", file])
            row = {"file": info.filename, "sha256": hashlib.sha256(data).hexdigest(), "zipCompression": info.compress_type,
                   "zipDataOffset": zip_offset, "zipOffsetModulo16k": zip_offset % 16384,
                   "load16kPass": load_pass, "segments": segments, "relro": relros}
            rows.append(row)
            if not load_pass or any(r["conflictingWritableBytes"] for r in relros):
                raise RuntimeError(f"ELF has incompatible LOAD or conflicting writable RELRO payload: {info.filename}")
            if path.suffix == ".apk" and info.compress_type == 0 and zip_offset % 16384:
                raise RuntimeError(f"Uncompressed native APK entry is not 16KB aligned: {info.filename}")
    return rows


def inspect_apk(path, label, manifest=True):
    package = {"identity": identity(path), "native": inspect_native(path, label)}
    report["packages"].append(package)
    if manifest:
        badging = run(label + "-badging", [buildtools / "aapt2.exe", "dump", "badging", path])
        manifest_text = run(label + "-manifest", [buildtools / "aapt2.exe", "dump", "xmltree", path, "--file", "AndroidManifest.xml"])
        forbidden = ("android.permission.INTERNET", "android.permission.ACCESS_NETWORK_STATE",
                     "android.permission.CAMERA", "android.permission.RECORD_AUDIO", "android.permission.READ_CONTACTS",
                     "android.permission.ACCESS_FINE_LOCATION", "android.permission.WRITE_EXTERNAL_STORAGE")
        # Isolated ABI splits inherit uses-sdk from base. aapt badging otherwise
        # invents legacy implied permissions; inspect actual XML declarations.
        if any(value in manifest_text for value in forbidden) or "application-debuggable" in badging:
            save()
            raise RuntimeError("Release has an unexpected network/sensitive permission or is debuggable")
        if label == "apk" or label.startswith("universal-") or label == "default-base-master":
            expected = ("package: name='com.giarrel.worddeduction'", f"versionCode='{args.version_code}'", f"versionName='{args.version_name}'",
                        "minSdkVersion:'26'", "targetSdkVersion:'36'")
            if package["native"]:
                expected += ("native-code: 'arm64-v8a'",)
            if not all(value in badging for value in expected):
                save()
                raise RuntimeError("Unexpected package identity, version, SDK or ABI")
            if not re.search(r"android:allowBackup\([^)]*\)=false", manifest_text):
                raise RuntimeError("Release must disable platform backup")
            inspect_backup(path, label)
            package["inputLogging"] = inspect_input_logging(path, label)
    run(label + "-signature", [buildtools / "apksigner.bat", "verify", "--verbose", "--print-certs", path])
    run(label + "-zipalign", [buildtools / "zipalign.exe", "-v", "-c", "-P", "16", "4", path])
    save()


def inspect_backup(path, label):
    resources = run(label + "-resources", [buildtools / "aapt2.exe", "dump", "resources", path])
    for name in ("session_backup_rules", "session_data_extraction_rules"):
        match = re.search(r"resource \S+ xml/" + re.escape(name) + r"\s+\(\) \(file\) (\S+)", resources)
        if not match:
            raise RuntimeError(f"Backup resource not found: {name}")
        rules = run(label + "-" + name, [buildtools / "aapt2.exe", "dump", "xmltree", path, "--file", match.group(1)])
        expected = 2 if name == "session_data_extraction_rules" else 1
        if rules.count('path="word-deduction"') != expected or rules.count('domain="file"') != expected:
            raise RuntimeError(f"Incomplete app-private backup exclusion: {name}")


def inspect_input_logging(path, label):
    classes, logging_calls = [], []
    with ZipFile(path) as archive:
        for name in archive.namelist():
            if not re.fullmatch(r"classes\d*\.dex", name):
                continue
            dex = out / (label + "-" + name)
            dex.write_bytes(archive.read(name))
            disassembly = run(label + "-" + name + "-disassembly", [buildtools / "dexdump.exe", "-d", dex])
            for section in re.split(r"(?m)^Class #\d+", disassembly):
                descriptor = re.search(r"Class descriptor\s*:\s*'(Lcom/google/androidgamesdk/gametextinput/[^']+)'", section)
                if descriptor:
                    classes.append(descriptor.group(1))
                    logging_calls.extend(line.strip() for line in section.splitlines()
                                         if re.search(r"invoke-\S+.*Landroid/util/Log;\.[vdi]:", line))
    result = {"classes": classes, "debugVerboseInfoCalls": logging_calls}
    (out / (label + "-input-logging.json")).write_text(json.dumps(result, indent=2), encoding="utf-8")
    if "Lcom/google/androidgamesdk/gametextinput/InputConnection;" not in classes or logging_calls:
        save()
        raise RuntimeError("Release GameTextInput DEX is missing or still contains private debug logging")
    return result


if artifact.suffix == ".apk":
    inspect_apk(artifact, "apk")
elif artifact.suffix == ".aab":
    run("bundle-validate", [java, "-jar", bundletool, "validate", "--bundle=" + str(artifact)])
    config = run("bundle-config", [java, "-jar", bundletool, "dump", "config", "--bundle=" + str(artifact)])
    if "PAGE_ALIGNMENT_16K" not in config:
        raise RuntimeError("AAB does not declare PAGE_ALIGNMENT_16K")
    run("bundle-manifest", [java, "-jar", bundletool, "dump", "manifest", "--bundle=" + str(artifact), "--module=base"])
    run("bundle-signature", [sdk / "OpenJDK/bin/jarsigner.exe", "-verify", "-verbose", "-certs", artifact])
    report["packages"].append({"identity": identity(artifact), "native": inspect_native(artifact, "aab")})
    for mode in ("universal", "default"):
        apks = out / (mode + ".apks")
        run("build-apks-" + mode, [java, "-jar", bundletool, "build-apks", "--bundle=" + str(artifact),
                                  "--output=" + str(apks), "--mode=" + mode, "--aapt2=" + str(buildtools / "aapt2.exe")])
        with ZipFile(apks) as archive:
            for name in archive.namelist():
                if not name.endswith(".apk"):
                    continue
                target = out / mode / name
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(archive.read(name))
                with ZipFile(target) as apk:
                    native = any(entry.endswith(".so") for entry in apk.namelist())
                if mode == "universal" or native or name.endswith("master.apk"):
                    inspect_apk(target, mode + "-" + target.stem)
else:
    raise RuntimeError("Expected an APK or AAB")
save()
print(json.dumps({"artifact": report["artifact"], "inspectedPackages": len(report["packages"]),
                  "commands": len(commands), "result": "passed; inspect report for scalar RELRO remainders and runtime limits"}, indent=2))
