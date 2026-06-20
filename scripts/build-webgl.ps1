param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\6000.0.30f1\Editor\Unity.exe",
    [string]$OutputPath = "docs"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$resolvedOutput = Join-Path $repoRoot $OutputPath
$logPath = Join-Path $repoRoot "Logs\webgl-build.log"

if (-not (Test-Path $UnityPath)) {
    throw "Unity was not found at '$UnityPath'. Update -UnityPath to the installed editor."
}

if (Test-Path $resolvedOutput) {
    Remove-Item -LiteralPath $resolvedOutput -Recurse -Force
}

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $logPath) | Out-Null

$env:UNITY_BUILD_PATH = $resolvedOutput

$arguments = @(
    "-quit",
    "-batchmode",
    "-nographics",
    "-projectPath", "`"$repoRoot`"",
    "-executeMethod", "WebGLBuild.Build",
    "-logFile", "`"$logPath`""
)

$process = Start-Process `
    -FilePath $UnityPath `
    -ArgumentList $arguments `
    -NoNewWindow `
    -PassThru `
    -Wait

if ($process.ExitCode -ne 0) {
    throw "Unity WebGL build failed with exit code $($process.ExitCode). See $logPath"
}

$indexPath = Join-Path $resolvedOutput "index.html"
if (-not (Test-Path $indexPath)) {
    throw "Unity completed but '$indexPath' was not created."
}

Copy-Item -LiteralPath $indexPath -Destination (Join-Path $resolvedOutput "404.html") -Force
New-Item -ItemType File -Force -Path (Join-Path $resolvedOutput ".nojekyll") | Out-Null

Write-Host "WebGL build created at $resolvedOutput"
