#Requires -Version 5.1
<#
.SYNOPSIS
  Build Release output and create Windows installer.

.DESCRIPTION
  Requirements:
  - Visual Studio / Build Tools (MSBuild)
  - Inno Setup 6 (ISCC.exe)
#>

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $Root

function Find-MsBuild {
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere) {
        $msb = & $vswhere -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" 2>$null | Select-Object -First 1
        if ($msb -and (Test-Path $msb)) { return $msb }
    }

    $candidates = @(
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    )
    foreach ($p in $candidates) {
        if (Test-Path $p) { return $p }
    }
    return $null
}

function Find-Iscc {
    $candidates = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles}\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles(x86)}\Inno Setup 5\ISCC.exe"
    )
    foreach ($p in $candidates) {
        if (Test-Path $p) { return $p }
    }
    $cmd = Get-Command iscc.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    return $null
}

$msbuild = Find-MsBuild
if (-not $msbuild) {
    throw "MSBuild not found. Install Visual Studio Build Tools."
}

Write-Host "MSBuild: $msbuild"
& $msbuild "$Root\TireShopAccounting.sln" /restore /t:Rebuild /p:Configuration=Release /v:m
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$releaseDir = Join-Path $Root "TireShopAccounting.UI\bin\Release"
$mainExe = Join-Path $releaseDir "TireShopAccounting.exe"
if (-not (Test-Path $mainExe)) {
    throw "Release build failed. TireShopAccounting.exe was not found."
}

$iscc = Find-Iscc
if (-not $iscc) {
    Write-Warning "Inno Setup not found. Release output is ready at: $releaseDir"
    exit 0
}

Write-Host "ISCC: $iscc"
& $iscc "$Root\Installer\TireShopAccounting.iss"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$dist = Join-Path $Root "dist"
if (-not (Test-Path $dist)) {
    throw "Installer build did not create dist folder."
}

Write-Host ""
Write-Host "Done. Installer output:"
Get-ChildItem $dist -Filter "*.exe" | ForEach-Object { Write-Host "  $($_.FullName)" }
