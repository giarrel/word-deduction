param(
    [ValidateSet('Capture','Tap','Swipe','State','Start','Restart','Back','Home')][string]$Action,
    [string]$Name = 'capture',
    [int]$X = 540, [int]$Y = 2110,
    [int]$ToX = 540, [int]$ToY = 1120,
    [int]$Duration = 400
)
$ErrorActionPreference = 'Stop'
$adb = 'C:\Users\lucac\AppData\Local\Android\Sdk\platform-tools\adb.exe'
$serial = 'emulator-5580'
$package = 'com.giarrel.worddeduction'
$component = "$package/com.unity3d.player.UnityPlayerGameActivity"
$outputDirectory = $PSScriptRoot
$ready = $false
for ($attempt = 0; $attempt -lt 6; $attempt++) {
    & $adb -s $serial get-state 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { $ready = $true; break }
    Start-Sleep -Milliseconds 750
}
if (-not $ready) { throw 'Test emulator is not connected; no action dispatched.' }
function Invoke-Adb([string[]]$Arguments) {
    $result = & $adb -s $serial @Arguments
    if ($LASTEXITCODE -ne 0) { throw "ADB failed: $Arguments" }
    return $result
}
switch ($Action) {
    'Tap' { Invoke-Adb @('shell','input','touchscreen','swipe',"$X","$Y","$X","$Y",'120') }
    'Swipe' { Invoke-Adb @('shell','input','touchscreen','swipe',"$X","$Y","$ToX","$ToY","$Duration") }
    'Back' { Invoke-Adb @('shell','input','keyevent','--duration','120','4') }
    'Home' { Invoke-Adb @('shell','input','keyevent','3') }
    'Start' { Invoke-Adb @('shell','am','start','-W','-n',$component) }
    'Restart' {
        Invoke-Adb @('shell','am','force-stop',$package)
        Invoke-Adb @('shell','am','start','-W','-n',$component)
    }
    'State' {
        $raw = (Invoke-Adb @('shell','run-as',$package,'cat','files/word-deduction/session.json')) -join "`n"
        $raw | Set-Content -LiteralPath (Join-Path $outputDirectory "$Name.json") -Encoding utf8
        $envelope = $raw | ConvertFrom-Json
        $state = $envelope.Payload | ConvertFrom-Json
        [pscustomobject]@{ Version=$envelope.Version; Match=$state.Match; Players=$state.Players.Count; Language=$state.Language; Used=$state.History.UsedPairIds; Remaining=$state.History.RemainingPairIds.Count } | ConvertTo-Json -Depth 10
    }
    'Capture' {
        Start-Sleep -Milliseconds 1800
        Invoke-Adb @('emu','screenrecord','screenshot',$outputDirectory)
        $latest = Get-ChildItem -LiteralPath $outputDirectory -Filter 'Screenshot_*.png' | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
        if ($null -eq $latest) { throw 'No host screenshot was returned.' }
        $destination = Join-Path $outputDirectory "$Name.png"
        Copy-Item -LiteralPath $latest.FullName -Destination $destination -Force
        Write-Output $destination
    }
}
