@echo off
echo ========================================
echo Building Tire Shop Accounting System
echo ========================================

REM Set paths
set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
set SOLUTION=TireShopAccounting.sln

REM Check if MSBuild exists
if not exist %MSBUILD% (
    echo MSBuild not found! Please install Visual Studio 2019 or adjust the path.
    pause
    exit /b 1
)

echo.
echo Step 1: Restoring NuGet packages...
nuget restore %SOLUTION%

echo.
echo Step 2: Building solution in Release mode...
%MSBUILD% %SOLUTION% /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild /m

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================
echo Build completed successfully!
echo Output: TireShopAccounting.UI\bin\Release
echo ========================================

echo.
echo Step 3: Creating installer with Inno Setup...
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Installer\setup.iss
    echo.
    echo ========================================
    echo Installer created successfully!
    echo Output: Installer\Output\TireShopAccounting_Setup.exe
    echo ========================================
) else (
    echo Inno Setup not found. Skipping installer creation.
    echo You can manually create the installer using Inno Setup.
)

echo.
pause
