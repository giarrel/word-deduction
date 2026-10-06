param(
    [ValidateSet('DevelopmentApk','ReleaseApk','ReleaseBundle')]
    [string]$Build = 'DevelopmentApk'
)
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path -Parent $PSScriptRoot
$taskProject = Join-Path $taskRoot 'game'
$env:UNITY_NO_UPDATE_CHECK = '1'
$env:UNITY_NON_INTERACTIVE = '1'
$taskCli = (Get-Command unity -ErrorAction Stop).Source
& $taskCli build $taskProject --editor-version 6000.3.25f1 --target Android --execute-method "WordDeduction.Editor.AppBuild.$Build" --format json
if ($LASTEXITCODE -ne 0) { throw "Unity build failed with exit code $LASTEXITCODE. If this project is open, stop Play mode and use the documented live-Editor build command instead." }
