param([string]$Prefix='match1')
$ErrorActionPreference='Stop'
$adb='C:/Users/lucac/AppData/Local/Android/Sdk/platform-tools/adb.exe'
$start=[Diagnostics.ProcessStartInfo]::new()
$start.FileName=$adb
$start.Arguments='-s emulator-5580 shell "CLASSPATH=/data/local/tmp/word-deduction-input.jar app_process /system/bin WordDeductionInput"'
$start.UseShellExecute=$false
$start.CreateNoWindow=$true
$start.RedirectStandardInput=$true
$start.RedirectStandardOutput=$true
$start.RedirectStandardError=$true
$inputProcess=[Diagnostics.Process]::Start($start)
$output=$inputProcess.StandardOutput.ReadToEndAsync()
$errors=$inputProcess.StandardError.ReadToEndAsync()
function Contact([string]$command) { $inputProcess.StandardInput.WriteLine($command); $inputProcess.StandardInput.Flush(); Start-Sleep -Milliseconds 400 }
function State { $raw=(& $adb -s emulator-5580 shell run-as com.giarrel.worddeduction cat files/word-deduction/session.json) -join "`n"; return ($raw|ConvertFrom-Json).Payload|ConvertFrom-Json }
try {
    $initial=State
    if($initial.Match.Phase -ne 0){throw 'Expected live covered handoff'}
    $matchId=$initial.Match.Id
    $total=$initial.Match.Participants.Count
    for($index=$initial.Match.Handoff;$index -lt $total;$index++) {
        $before=State
        if($before.Match.Id -ne $matchId -or $before.Match.Handoff -ne $index -or $before.Match.Phase -ne 0){throw "Unexpected owner before $index"}
        Contact 'down 0 540 2010'
        if($index -eq 0 -or $before.Match.Participants[$index].Role -ne 0){ & "$PSScriptRoot/Device.ps1" -Action Capture -Name "$Prefix-owner-$($index+1)-open" }
        Contact 'up 0'
        & "$PSScriptRoot/Device.ps1" -Action Tap -Y 2280
        Start-Sleep -Milliseconds 350
        $after=State
        if($index -lt ($total-1)) { if($after.Match.Handoff -ne ($index+1) -or $after.Match.Phase -ne 0){throw "Owner failed advance $index"} }
        elseif($after.Match.Phase -ne 1){throw 'Last owner did not reach clues'}
        Write-Output "Native reveal/release/Next: owner $($index+1)/$total -> phase $($after.Match.Phase), handoff $($after.Match.Handoff)"
    }
    & "$PSScriptRoot/Device.ps1" -Action Capture -Name "$Prefix-clues"
    & "$PSScriptRoot/Device.ps1" -Action State -Name "$Prefix-after-deal"
} finally {
    if(-not $inputProcess.HasExited){$inputProcess.StandardInput.WriteLine('cancel');$inputProcess.StandardInput.WriteLine('quit');$inputProcess.StandardInput.Flush();$inputProcess.StandardInput.Close();$inputProcess.WaitForExit(5000)|Out-Null}
    if($inputProcess.HasExited){$output.Result|Set-Content -LiteralPath "$PSScriptRoot/$Prefix-contacts.txt"; $errors.Result|Set-Content -LiteralPath "$PSScriptRoot/$Prefix-contact-errors.txt"}
}
