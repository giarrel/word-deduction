param([ValidateSet('Commit','Cancel')][string]$Scenario='Commit')
$ErrorActionPreference='Stop'
$adb='C:/Users/lucac/AppData/Local/Android/Sdk/platform-tools/adb.exe'
$package='com.giarrel.worddeduction'
& $adb -s emulator-5580 logcat -c
if($LASTEXITCODE -ne 0){throw 'Cannot reset test log buffer'}
if($Scenario -eq 'Commit') {
    & $adb -s emulator-5580 shell input touchscreen swipe 2 1250 400 1250 250
    Start-Sleep -Milliseconds 200
    & $adb -s emulator-5580 shell input touchscreen swipe 2 1250 400 1250 250
} else {
    $start=[Diagnostics.ProcessStartInfo]::new()
    $start.FileName=$adb
    $start.Arguments='-s emulator-5580 shell "CLASSPATH=/data/local/tmp/word-deduction-input.jar app_process /system/bin WordDeductionInput"'
    $start.UseShellExecute=$false
    $start.CreateNoWindow=$true
    $start.RedirectStandardInput=$true
    $start.RedirectStandardOutput=$true
    $start.RedirectStandardError=$true
    $p=[Diagnostics.Process]::Start($start)
    $stdout=$p.StandardOutput.ReadToEndAsync()
    $stderr=$p.StandardError.ReadToEndAsync()
    function Contact([string]$line){$p.StandardInput.WriteLine($line);$p.StandardInput.Flush();Start-Sleep -Milliseconds 160}
    try {
        & $adb -s emulator-5580 shell input touchscreen swipe 2 1250 400 1250 250
        Start-Sleep -Milliseconds 150
        Contact 'down 0 2 1250'
        Contact 'move 0 120 1250'
        Contact 'move 0 350 1250'
        Contact 'move 0 120 1250'
        Contact 'move 0 2 1250'
        Contact 'up 0'
    } finally {
        if(-not $p.HasExited){$p.StandardInput.WriteLine('cancel');$p.StandardInput.WriteLine('quit');$p.StandardInput.Flush();$p.StandardInput.Close();$p.WaitForExit(5000)|Out-Null}
        if($p.HasExited){$stdout.Result|Set-Content -LiteralPath "$PSScriptRoot/back-cancel-contacts.txt";$stderr.Result|Set-Content -LiteralPath "$PSScriptRoot/back-cancel-errors.txt"}
    }
}
Start-Sleep -Milliseconds 800
& "$PSScriptRoot/Device.ps1" -Action Capture -Name "back-$Scenario"
& $adb -s emulator-5580 logcat -d -v time | Select-String 'CoreBackPreview|BackNavigation|EdgeBackGestureHandler|OnBackInvoked|AndroidRuntime|FATAL EXCEPTION|Unity' | Set-Content -LiteralPath "$PSScriptRoot/back-$Scenario-log.txt"
& $adb -s emulator-5580 shell dumpsys activity activities | Select-String 'topResumedActivity|mResumedActivity|ResumedActivity' | Set-Content -LiteralPath "$PSScriptRoot/back-$Scenario-activity.txt"
