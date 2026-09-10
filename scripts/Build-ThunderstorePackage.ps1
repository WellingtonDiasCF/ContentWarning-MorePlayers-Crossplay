[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$packageSource = Join-Path $repositoryRoot "thunderstore"
$manifestPath = Join-Path $packageSource "manifest.json"
$iconPath = Join-Path $packageSource "icon.png"
$pluginPath = Join-Path $packageSource "BepInEx\plugins\MorePlayersCrossplay\ContentWarningHostOnlyLobby.dll"

foreach ($requiredPath in @($manifestPath, $iconPath, $pluginPath, (Join-Path $packageSource "README.md"))) {
    if (-not (Test-Path -LiteralPath $requiredPath)) {
        throw "Required package file not found: $requiredPath"
    }
}

$manifest = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
if ($manifest.version_number -notmatch '^\d+\.\d+\.\d+$') {
    throw "manifest.json version_number must use Major.Minor.Patch."
}

$assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($pluginPath).Version
$pluginVersion = "$($assemblyVersion.Major).$($assemblyVersion.Minor).$($assemblyVersion.Build)"
if ($pluginVersion -ne $manifest.version_number) {
    throw "Plugin version $pluginVersion does not match manifest version $($manifest.version_number)."
}

Add-Type -AssemblyName System.Drawing
$icon = [System.Drawing.Image]::FromFile($iconPath)
try {
    if ($icon.Width -ne 256 -or $icon.Height -ne 256) {
        throw "icon.png must be exactly 256x256 pixels."
    }
}
finally {
    $icon.Dispose()
}

$outputDirectory = Join-Path $repositoryRoot "artifacts\thunderstore"
New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
$outputPath = Join-Path $outputDirectory "MorePlayersCrossplay-$($manifest.version_number).zip"
Compress-Archive -Path (Join-Path $packageSource "*") -DestinationPath $outputPath -CompressionLevel Optimal -Force

Write-Host "Thunderstore package created: $outputPath"
