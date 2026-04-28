# 🚀 راهنمای دیپلوی و ساخت فایل نصبی

## 📋 فهرست مطالب
1. [پیش‌نیازها](#پیش‌نیازها)
2. [مراحل ساخت فایل نصبی](#مراحل-ساخت-فایل-نصبی)
3. [تنظیمات Inno Setup](#تنظیمات-inno-setup)
4. [نصب برنامه](#نصب-برنامه)
5. [حذف برنامه](#حذف-برنامه)

---

## پیش‌نیازها

### 1. نصب Visual Studio
- دانلود: https://visualstudio.microsoft.com/downloads/
- حداقل نسخه: Visual Studio 2019
- Workload مورد نیاز: `.NET desktop development`

### 2. نصب .NET Framework 4.7.2 SDK
- معمولاً با Visual Studio نصب می‌شود
- لینک جداگانه: https://dotnet.microsoft.com/download/dotnet-framework/net472

### 3. نصب Inno Setup
- **دانلود:** https://jrsoftware.org/isdl.php
- نسخه: 6.2.0 یا بالاتر
- فایل: `innosetup-6.2.0.exe`
- نصب در: `C:\Program Files (x86)\Inno Setup 6`

### 4. نصب NuGet CLI (اختیاری)
```powershell
# دانلود از:
https://www.nuget.org/downloads

# یا نصب از Chocolatey:
choco install nuget.commandline
```

---

## مراحل ساخت فایل نصبی

### مرحله 1: آماده‌سازی پروژه

```bash
# 1. باز کردن Command Prompt یا PowerShell
cd C:\path\to\TireShopAccounting

# 2. پاک کردن build‌های قبلی
rmdir /s /q TireShopAccounting.UI\bin\Release
rmdir /s /q TireShopAccounting.UI\obj\Release
```

### مرحله 2: بازیابی NuGet Packages

```bash
# روش اول: از طریق Visual Studio
# File > Open > Project/Solution > TireShopAccounting.sln
# کلیک راست روی Solution > Restore NuGet Packages

# روش دوم: از Command Line
nuget restore TireShopAccounting.sln
```

### مرحله 3: Build کردن پروژه

#### روش A: استفاده از Visual Studio
1. `Build > Configuration Manager`
2. انتخاب `Release` به جای `Debug`
3. انتخاب `Any CPU`
4. `Build > Rebuild Solution` (یا `Ctrl+Shift+B`)

#### روش B: استفاده از MSBuild (Command Line)
```bash
# پیدا کردن مسیر MSBuild
# برای VS 2019:
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"

# برای VS 2022:
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

# اجرای Build:
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" TireShopAccounting.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild /m
```

#### روش C: استفاده از اسکریپت آماده
```bash
# اجرای فایل Batch:
Build.bat

# یا اجرای PowerShell:
powershell -ExecutionPolicy Bypass -File Build.ps1
```

### مرحله 4: بررسی خروجی

فایل‌های زیر باید در `TireShopAccounting.UI\bin\Release` وجود داشته باشند:

```
✓ TireShopAccounting.exe
✓ TireShopAccounting.Core.dll
✓ TireShopAccounting.Data.dll
✓ TireShopAccounting.Services.dll
✓ System.Data.SQLite.dll
✓ Dapper.dll
✓ x86/SQLite.Interop.dll
✓ x64/SQLite.Interop.dll
```

### مرحله 5: ساخت Installer با Inno Setup

#### روش A: استفاده از GUI
1. باز کردن Inno Setup Compiler
2. `File > Open` و انتخاب `Installer\setup.iss`
3. `Build > Compile` (یا `Ctrl+F9`)
4. منتظر بمانید تا Compile کامل شود
5. فایل خروجی در `Installer\Output\TireShopAccounting_Setup.exe`

#### روش B: استفاده از Command Line
```bash
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Installer\setup.iss
```

---

## تنظیمات Inno Setup

فایل `Installer\setup.iss` شامل تنظیمات زیر است:

```ini
[Setup]
AppName=سیستم حسابداری لاستیک فروشی
AppVersion=1.0.0
DefaultDirName={pf}\TireShopAccounting
DefaultGroupName=حسابداری لاستیک
OutputDir=Output
OutputBaseFilename=TireShopAccounting_Setup
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\TireShopAccounting.exe
```

### سفارشی‌سازی Installer:

#### تغییر نام فایل خروجی:
```ini
OutputBaseFilename=TireShopAccounting_Setup_v1.0
```

#### تغییر مسیر نصب پیش‌فرض:
```ini
DefaultDirName=C:\TireShopAccounting
```

#### اضافه کردن آیکون:
```ini
SetupIconFile=..\TireShopAccounting.UI\Resources\app.ico
```

---

## نصب برنامه

### برای کاربر نهایی:

1. **دانلود فایل نصب:**
   - فایل: `TireShopAccounting_Setup.exe`
   - حجم: تقریباً 15-20 MB

2. **اجرای فایل نصب:**
   - دابل کلیک روی `TireShopAccounting_Setup.exe`
   - اگر Windows SmartScreen هشدار داد، `More info > Run anyway` کلیک کنید

3. **مراحل نصب:**
   ```
   ✓ Welcome Screen
   ✓ License Agreement (اختیاری)
   ✓ Select Destination Location
     پیش‌فرض: C:\Program Files\TireShopAccounting
   ✓ Select Start Menu Folder
   ✓ Select Additional Tasks
     ☑ Create a desktop icon (ایجاد میانبر روی دسکتاپ)
   ✓ Ready to Install
   ✓ Installing...
   ✓ Completed
     ☑ Launch TireShopAccounting (اجرا بعد از نصب)
   ```

4. **پس از نصب:**
   - میانبر روی دسکتاپ: `سیستم حسابداری لاستیک`
   - منوی Start: `حسابداری لاستیک > سیستم حسابداری`
   - مسیر نصب: `C:\Program Files\TireShopAccounting`

---

## پیکربندی اولیه

### اولین بار اجرا:

1. برنامه را اجرا کنید
2. دیتابیس خودکار ایجاد می‌شود در:
   ```
   C:\Users\[YourUsername]\AppData\Roaming\TireShopAccounting\database.db
   ```
3. صفحه داشبورد نمایش داده می‌شود
4. شروع به استفاده کنید!

### دیتا اولیه (اختیاری):

اگر می‌خواهید با دیتای نمونه شروع کنید:

```sql
-- افزودن کالای نمونه
INSERT INTO products (name, brand, quantity, buy_price, sell_price, created_date)
VALUES 
('لاستیک 185/65R15', 'میشلن', 20, 5000000, 6500000, datetime('now')),
('لاستیک 195/65R15', 'هانکوک', 15, 4500000, 6000000, datetime('now'));

-- افزودن مشتری نمونه
INSERT INTO customers (name, phone, address, balance, created_date)
VALUES ('علی احمدی', '09123456789', 'تهران', 0, datetime('now'));
```

---

## حذف برنامه

### روش 1: از طریق Windows Settings
```
Settings > Apps > Apps & features > 
جستجوی "حسابداری" > Uninstall
```

### روش 2: از طریق Control Panel
```
Control Panel > Programs > Programs and Features >
انتخاب برنامه > Uninstall
```

### روش 3: از طریق Uninstaller خود برنامه
```
C:\Program Files\TireShopAccounting\unins000.exe
```

### ⚠️ توجه:
حذف برنامه دیتابیس را پاک نمی‌کند!

برای حذف کامل دیتا:
```
C:\Users\[YourUsername]\AppData\Roaming\TireShopAccounting\
```

---

## 🔧 رفع مشکلات نصب

### مشکل 1: خطای "MSBuild not found"

**راه حل:**
```powershell
# نصب Build Tools
https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2019
```

### مشکل 2: خطای "NuGet packages not restored"

**راه حل:**
```bash
# حذف packages و restore دوباره
rmdir /s /q packages
nuget restore TireShopAccounting.sln
```

### مشکل 3: خطای "SQLite.Interop.dll not found"

**راه حل:**
```bash
# نصب دوباره SQLite package
nuget install System.Data.SQLite.Core -OutputDirectory packages
```

### مشکل 4: برنامه نصب شده اجرا نمی‌شود

**راه حل:**
```bash
# بررسی نصب .NET Framework
# دانلود و نصب:
https://dotnet.microsoft.com/download/dotnet-framework/net472
```

### مشکل 5: خطای امضای دیجیتال

**راه حل:**
```
# در ویندوز 10/11:
Settings > Update & Security > Windows Security > 
App & browser control > Reputation-based protection settings >
Turn off "Check apps and files"
# (موقتاً برای نصب)
```

---

## 📦 ساخت Portable Version

اگر می‌خواهید نسخه Portable (بدون نیاز به نصب) داشته باشید:

1. فایل‌های `bin\Release` را در یک فولدر کپی کنید
2. فایل `TireShopAccounting.exe` را اجرا کنید
3. دیتابیس در `AppData` ایجاد می‌شود

---

## 🎯 Checklist قبل از انتشار

- [ ] Build در حالت Release انجام شده
- [ ] تمام NuGet packages نصب هستند
- [ ] SQLite DLL‌ها برای x86 و x64 موجودند
- [ ] فایل Installer ساخته شده
- [ ] Installer روی Windows 7 تست شده
- [ ] Installer روی Windows 10 تست شده
- [ ] Installer روی Windows 11 تست شده
- [ ] نصب و حذف برنامه تست شده
- [ ] دیتابیس خودکار ایجاد می‌شود
- [ ] تمام ویژگی‌ها کار می‌کنند
- [ ] فونت‌ها صحیح نمایش داده می‌شوند
- [ ] RTL کامل است
- [ ] چاپ کار می‌کند

---

## 📝 نکات مهم

✅ **همیشه در حالت Release build کنید**
✅ **تست روی سیستم تمیز انجام دهید**
✅ **از آنتی‌ویروس‌ها Exception بگیرید**
✅ **دستورالعمل نصب واضح بدهید**
✅ **پشتیبانی برای ویندوز 7 تا 11**

---

## 🎁 فایل‌های اضافی برای توزیع

همراه با Installer، این فایل‌ها را ارائه دهید:

1. `README.md` - راهنمای استفاده
2. `INSTALLATION_GUIDE.md` - راهنمای نصب
3. `LICENSE.txt` - مجوز استفاده
4. `CHANGELOG.md` - تغییرات نسخه‌ها

---

**آماده برای انتشار! 🚀**

فایل نصبی شما در:
```
Installer\Output\TireShopAccounting_Setup.exe
```
