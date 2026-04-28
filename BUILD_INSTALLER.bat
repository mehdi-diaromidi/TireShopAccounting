@echo off
chcp 65001 >nul
color 0A
cls

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║     ساخت فایل نصبی سیستم حسابداری لاستیک‌فروشی           ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

REM ===== تنظیم مسیرها =====
set "SOLUTION=TireShopAccounting.sln"
set "BUILD_CONFIG=Release"
set "OUTPUT_DIR=TireShopAccounting.UI\bin\Release"
set "INSTALLER_SCRIPT=Installer\setup.iss"
set "INSTALLER_OUTPUT=Installer\Output"

REM ===== جستجوی MSBuild =====
echo [1/5] در حال جستجوی MSBuild...
set "MSBUILD="

for %%p in (
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
) do (
    if exist %%p (
        set "MSBUILD=%%p"
        echo       ✓ MSBuild یافت شد
        goto :MSBUILD_FOUND
    )
)

echo       ✗ MSBuild یافت نشد!
echo.
echo لطفاً Visual Studio 2019 یا 2022 را نصب کنید
echo دانلود: https://visualstudio.microsoft.com/downloads/
echo.
pause
exit /b 1

:MSBUILD_FOUND

REM ===== بررسی NuGet =====
echo [2/5] بررسی NuGet...
where nuget >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo       ⚠ NuGet در PATH یافت نشد
    echo       ⚠ از بازیابی packages در Visual Studio استفاده خواهد شد
) else (
    echo       ✓ NuGet یافت شد
    echo       در حال بازیابی packages...
    nuget restore %SOLUTION%
    if %ERRORLEVEL% NEQ 0 (
        echo       ✗ خطا در بازیابی packages
        pause
        exit /b 1
    )
    echo       ✓ Packages بازیابی شد
)

REM ===== پاک کردن Build قبلی =====
echo [3/5] پاک کردن build قبلی...
if exist "%OUTPUT_DIR%" (
    rmdir /s /q "%OUTPUT_DIR%" 2>nul
)
if exist "TireShopAccounting.UI\obj\Release" (
    rmdir /s /q "TireShopAccounting.UI\obj\Release" 2>nul
)
if exist "%INSTALLER_OUTPUT%" (
    rmdir /s /q "%INSTALLER_OUTPUT%" 2>nul
)
echo       ✓ پاک‌سازی انجام شد

REM ===== Build کردن Solution =====
echo [4/5] در حال Build کردن پروژه...
echo       لطفاً صبر کنید...
%MSBUILD% %SOLUTION% /p:Configuration=%BUILD_CONFIG% /p:Platform="Any CPU" /t:Rebuild /m /v:minimal /nologo

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo       ✗ Build ناموفق!
    echo.
    pause
    exit /b %ERRORLEVEL%
)

echo       ✓ Build با موفقیت انجام شد

REM ===== بررسی فایل‌های خروجی =====
echo       بررسی فایل‌های ضروری...
set "ALL_FILES_EXIST=1"

if not exist "%OUTPUT_DIR%\TireShopAccounting.exe" (
    echo       ✗ فایل exe یافت نشد
    set "ALL_FILES_EXIST=0"
)
if not exist "%OUTPUT_DIR%\System.Data.SQLite.dll" (
    echo       ✗ SQLite dll یافت نشد
    set "ALL_FILES_EXIST=0"
)
if not exist "%OUTPUT_DIR%\Dapper.dll" (
    echo       ✗ Dapper dll یافت نشد
    set "ALL_FILES_EXIST=0"
)

if "%ALL_FILES_EXIST%"=="0" (
    echo.
    echo       ✗ برخی فایل‌های ضروری یافت نشدند
    pause
    exit /b 1
)

echo       ✓ تمام فایل‌های ضروری موجود است

REM ===== ساخت Installer =====
echo [5/5] ساخت فایل نصبی...

set "INNO_SETUP="
for %%p in (
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    "C:\Program Files\Inno Setup 6\ISCC.exe"
    "C:\Program Files (x86)\Inno Setup 5\ISCC.exe"
) do (
    if exist %%p (
        set "INNO_SETUP=%%p"
        goto :INNO_FOUND
    )
)

echo       ✗ Inno Setup یافت نشد!
echo.
echo       برنامه با موفقیت Build شد اما Installer ساخته نشد.
echo       فایل اجرایی در: %OUTPUT_DIR%\TireShopAccounting.exe
echo.
echo       برای ساخت Installer، Inno Setup را نصب کنید:
echo       دانلود: https://jrsoftware.org/isdl.php
echo.
goto :SUCCESS_WITHOUT_INSTALLER

:INNO_FOUND
echo       ✓ Inno Setup یافت شد
echo       در حال ساخت Installer...

%INNO_SETUP% %INSTALLER_SCRIPT% /Q

if %ERRORLEVEL% NEQ 0 (
    echo       ✗ خطا در ساخت Installer
    pause
    exit /b %ERRORLEVEL%
)

echo       ✓ Installer با موفقیت ساخته شد
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                    ✓ موفقیت آمیز                           ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo فایل نصبی آماده است:
echo %INSTALLER_OUTPUT%\TireShopAccounting_Setup.exe
echo.
echo برای نصب روی ویندوز، فایل بالا را اجرا کنید.
echo.
goto :END

:SUCCESS_WITHOUT_INSTALLER
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Build موفق (بدون Installer)                  ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo فایل اجرایی:
echo %OUTPUT_DIR%\TireShopAccounting.exe
echo.

:END
echo آیا می‌خواهید فولدر خروجی را باز کنید؟ (Y/N)
set /p OPEN_FOLDER=
if /i "%OPEN_FOLDER%"=="Y" (
    if exist "%INSTALLER_OUTPUT%\TireShopAccounting_Setup.exe" (
        explorer "%INSTALLER_OUTPUT%"
    ) else (
        explorer "%OUTPUT_DIR%"
    )
)

echo.
echo فشار دهید برای خروج...
pause >nul
