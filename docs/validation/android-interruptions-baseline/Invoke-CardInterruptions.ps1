param([ValidateSet('Card','System','Sleep','Reentry')][string]$Scenario = 'Card')
$ErrorActionPreference = 'Stop'
$adb = 'C:/Users/lucac/AppData/Local/Android/Sdk/platform-tools/adb.exe'
$probeDir = $PSScriptRoot
$start = [Diagnostics.ProcessStartInfo]::new()
$start.FileName = $adb
$start.Arguments = '-s emulator-5580 shell "CLASSPATH=/data/local/tmp/word-deduction-input.jar app_process /system/bin WordDeductionInput"'
$start.UseShellExecute = $false
$start.CreateNoWindow = $true
$start.RedirectStandardInput = $true
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
$inputProcess = [Diagnostics.Process]::Start($start)
$output = $inputProcess.StandardOutput.ReadToEndAsync()
$errors = $inputProcess.StandardError.ReadToEndAsync()
function Contact([string]$command) {
    $inputProcess.StandardInput.WriteLine($command)
    $inputProcess.StandardInput.Flush()
    Start-Sleep -Milliseconds 350
}
function Capture([string]$name) {
    Start-Sleep -Milliseconds 800
    & $adb -s emulator-5580 emu screenrecord screenshot $probeDir | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Screenshot failed' }
    $latest = Get-ChildItem -LiteralPath $probeDir -Filter 'Screenshot_*.png' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    Copy-Item -LiteralPath $latest.FullName -Destination "$probeDir/$name.png"
    Write-Output "Captured $name"
}
try {
    Start-Sleep -Milliseconds 500
    if ($Scenario -eq 'Card') {
    Contact 'down 0 540 1470'
    Contact 'move 0 540 1120'
    Capture '01-open-before-cancel'
    Contact 'cancel'
    Capture '02-cancel-covered'
    Contact 'down 0 540 1470'
    Contact 'move 0 540 1120'
    Capture '03-open-before-outside-release'
    Contact 'move 0 1065 600'
    Contact 'up 0'
    Capture '04-outside-release-covered'
    Contact 'down 0 540 2010'
    Capture '05-open-before-home'
    & $adb -s emulator-5580 shell input keyevent 3
    Start-Sleep -Milliseconds 600
    Contact 'cancel'
    & $adb -s emulator-5580 shell am start -W -n com.giarrel.worddeduction/com.unity3d.player.UnityPlayerGameActivity | Out-Null
    Start-Sleep -Milliseconds 900
    Capture '06-home-return-covered-pause'
    } elseif ($Scenario -eq 'System') {
        & $adb -s emulator-5580 shell input touchscreen swipe 540 2110 540 2110 120
        Start-Sleep -Milliseconds 500
        Contact 'down 0 540 2010'
        Capture '07-open-before-recents'
        & $adb -s emulator-5580 shell input keyevent 187
        Start-Sleep -Milliseconds 1000
        Capture '08-protected-recents-preview'
        Contact 'cancel'
        & $adb -s emulator-5580 shell am start -W -n com.giarrel.worddeduction/com.unity3d.player.UnityPlayerGameActivity | Out-Null
        Start-Sleep -Milliseconds 650
        Capture '09-recents-return-covered-pause'
        & $adb -s emulator-5580 shell input touchscreen swipe 540 2110 540 2110 120
        Start-Sleep -Milliseconds 500
        & $adb -s emulator-5580 shell input touchscreen swipe 2 1250 400 1250 400
        Start-Sleep -Milliseconds 650
        Capture '10-system-edge-back'
    } elseif ($Scenario -eq 'Reentry') {
        Contact 'down 0 540 2010'
        Capture '16-reentry-first-open'
        Contact 'down 1 980 900'
        Capture '17-reentry-second-covers'
        Contact 'up 1'
        Contact 'down 1 540 2010'
        Capture '18-reentry-while-first-remains'
        Contact 'cancel'
        Capture '19-reentry-all-cancelled'
    } else {
        Contact 'down 0 540 2010'
        Capture '11-open-before-sleep'
        & $adb -s emulator-5580 shell input keyevent 223
        Start-Sleep -Milliseconds 700
        Contact 'cancel'
        & $adb -s emulator-5580 shell input keyevent 224
        & $adb -s emulator-5580 shell input keyevent 82
        & $adb -s emulator-5580 shell am start -W -n com.giarrel.worddeduction/com.unity3d.player.UnityPlayerGameActivity | Out-Null
        Start-Sleep -Milliseconds 1500
        Capture '12-wake-covered-pause'
    }
} finally {
    if (-not $inputProcess.HasExited) {
        $inputProcess.StandardInput.WriteLine('cancel')
        $inputProcess.StandardInput.WriteLine('quit')
        $inputProcess.StandardInput.Flush()
        $inputProcess.StandardInput.Close()
        $inputProcess.WaitForExit(5000) | Out-Null
    }
    if ($inputProcess.HasExited) {
        $output.Result | Set-Content -LiteralPath "$probeDir/contacts-$Scenario.txt"
        $errors.Result | Set-Content -LiteralPath "$probeDir/contact-errors-$Scenario.txt"
        Write-Output $output.Result
        Write-Output $errors.Result
    }
}
