# PowerShell Build Script for Tire Shop Accounting
# استفاده: .\Build.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building Tire Shop Accounting System" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# تنظیم مسیرها
$SolutionPath = "TireShopAccounting.sln"
$BuildConfig = "Release"
$OutputPath = "TireShopAccounting.UI\bin\Release"

# جستجوی MSBuild
$MSBuildPaths = @(
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
)

$MSBuild = $null
foreach ($path in $MSBuildPaths) {
    if (Test-Path $path) {
        $MSBuild = $path
        Write-Host "✓ MSBuild found: $path" -ForegroundColor Green
        break
    }
}

if ($null -eq $MSBuild) {
    Write-Host "✗ MSBuild not found!" -ForegroundColor Red
    Write-Host "Please install Visual Studio 2019 or 2022" -ForegroundColor Yellow
    exit 1
}

# بررسی وجود Solution
if (-not (Test-Path $SolutionPath)) {
    Write-Host "✗ Solution file not found: $SolutionPath" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Step 1: Restoring NuGet packages..." -ForegroundColor Yellow

# بازیابی NuGet Packages
nuget restore $SolutionPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ NuGet restore failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "✓ NuGet packages restored successfully" -ForegroundColor Green
Write-Host ""
Write-Host "Step 2: Building solution..." -ForegroundColor Yellow

# ساخت Solution
& $MSBuild $SolutionPath /p:Configuration=$BuildConfig /p:Platform="Any CPU" /t:Rebuild /m /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "✓ Build completed successfully" -ForegroundColor Green
Write-Host ""

# نمایش فایل‌های خروجی
Write-Host "Output files:" -ForegroundColor Cyan
Get-ChildItem -Path $OutputPath -Filter "*.exe" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}
Get-ChildItem -Path $OutputPath -Filter "*.dll" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Status: " -NoNewline
Write-Host "SUCCESS ✓" -ForegroundColor Green
Write-Host "Output: $OutputPath" -ForegroundColor White
Write-Host ""

# پرسش برای ساخت Installer
Write-Host "Do you want to create installer? (Y/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq 'Y' -or $response -eq 'y') {
    Write-Host ""
    Write-Host "Step 3: Creating installer..." -ForegroundColor Yellow
    
    $InnoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    
    if (Test-Path $InnoSetupPath) {
        & $InnoSetupPath "Installer\setup.iss"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Installer created successfully" -ForegroundColor Green
            Write-Host "Output: Installer\Output\TireShopAccounting_Setup.exe" -ForegroundColor White
        } else {
            Write-Host "✗ Installer creation failed" -ForegroundColor Red
        }
    } else {
        Write-Host "✗ Inno Setup not found at: $InnoSetupPath" -ForegroundColor Red
        Write-Host "Please install Inno Setup 6 from: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Done! Press any key to exit..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
