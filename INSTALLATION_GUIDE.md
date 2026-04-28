# 📘 راهنمای کامل نصب و استفاده

## 🛠️ پیش‌نیازهای توسعه

### نرم‌افزارهای مورد نیاز:
1. **Visual Studio 2019 یا جدیدتر**
   - حتماً Workload "‎.NET desktop development" را نصب کنید
   - دانلود: https://visualstudio.microsoft.com/downloads/

2. **‎.NET Framework 4.7.2 SDK**
   - معمولاً با Visual Studio نصب می‌شود
   - در صورت نیاز: https://dotnet.microsoft.com/download/dotnet-framework

3. **Inno Setup 6** (برای ساخت فایل نصب)
   - دانلود: https://jrsoftware.org/isdl.php

4. **NuGet Package Manager**
   - معمولاً با Visual Studio نصب می‌شود

---

## 📦 نصب Dependencies

### روش اول: استفاده از NuGet Package Manager Console

```powershell
# در Visual Studio، منوی Tools > NuGet Package Manager > Package Manager Console

# نصب Dapper
Install-Package Dapper -Version 2.0.123 -ProjectName TireShopAccounting.Data

# نصب SQLite
Install-Package System.Data.SQLite.Core -Version 1.0.115.5 -ProjectName TireShopAccounting.Data
```

### روش دوم: استفاده از NuGet CLI

```bash
cd TireShopAccounting
nuget restore TireShopAccounting.sln
```

---

## 🏗️ ساخت پروژه

### روش اول: استفاده از Visual Studio

1. فایل `TireShopAccounting.sln` را باز کنید
2. `Build > Rebuild Solution` را انتخاب کنید
3. منتظر بمانید تا Build کامل شود

### روش دوم: استفاده از MSBuild (Command Line)

```bash
# اجرای فایل Build.bat
Build.bat

# یا دستی:
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" TireShopAccounting.sln /p:Configuration=Release /t:Rebuild
```

---

## 📁 ساختار خروجی

بعد از Build موفق، فایل‌های زیر در `TireShopAccounting.UI\bin\Release` ایجاد می‌شوند:

```
TireShopAccounting.exe          # فایل اجرایی اصلی
TireShopAccounting.Core.dll
TireShopAccounting.Data.dll
TireShopAccounting.Services.dll
System.Data.SQLite.dll          # کتابخانه SQLite
Dapper.dll                      # کتابخانه Dapper
```

---

## 💿 ساخت فایل نصب (Installer)

### قدم اول: آماده‌سازی

مطمئن شوید پروژه در حالت **Release** Build شده است.

### قدم دوم: ایجاد Installer

```bash
# اجرا در ریشه پروژه
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Installer\setup.iss
```

یا می‌توانید فایل `Installer\setup.iss` را در Inno Setup باز کنید و `Compile` را بزنید.

### خروجی:
فایل `TireShopAccounting_Setup.exe` در پوشه `Installer\Output` ایجاد می‌شود.

---

## 🚀 نصب برنامه

### برای کاربر نهایی:

1. فایل `TireShopAccounting_Setup.exe` را دانبل کلیک کنید
2. مراحل نصب را دنبال کنید
3. برنامه در `C:\Program Files\TireShopAccounting` نصب می‌شود
4. میانبر روی دسکتاپ ایجاد می‌شود

---

## ▶️ اجرای برنامه

### اجرای مستقیم (بدون نصب):

```bash
cd TireShopAccounting.UI\bin\Release
TireShopAccounting.exe
```

### اجرای بعد از نصب:

- از میانبر دسکتاپ
- یا از منوی Start

---

## 💾 محل ذخیره دیتابیس

دیتابیس SQLite در مسیر زیر ذخیره می‌شود:

```
C:\Users\[نام کاربر]\AppData\Roaming\TireShopAccounting\database.db
```

### نسخه‌های پشتیبان:

```
C:\Users\[نام کاربر]\Documents\TireShopBackups\
```

---

## 🎯 راهنمای استفاده

### 1️⃣ اولین بار اجرا

- برنامه را اجرا کنید
- دیتابیس خودکار ایجاد می‌شود
- به صفحه داشبورد منتقل می‌شوید

### 2️⃣ افزودن کالا

1. به بخش **کالاها** بروید
2. دکمه **➕ جدید** را بزنید
3. اطلاعات را وارد کنید:
   - نام کالا: مثلاً "لاستیک"
   - برند: مثلاً "میشلن"
   - موجودی: تعداد
   - قیمت خرید و فروش
4. **💾 ذخیره** را بزنید

### 3️⃣ افزودن مشتری

1. به بخش **مشتریان** بروید
2. **➕ جدید**
3. نام، شماره تلفن، آدرس را وارد کنید
4. **💾 ذخیره**

### 4️⃣ ثبت فاکتور فروش

1. به بخش **فاکتور فروش** بروید
2. مشتری را انتخاب کنید
3. کالا را جستجو و انتخاب کنید
4. تعداد را وارد کنید
5. **➕ افزودن** را بزنید
6. می‌توانید چند کالا اضافه کنید
7. در صورت نیاز تخفیف اعمال کنید
8. **💾 ثبت فاکتور** را بزنید
9. برنامه پرسش می‌کند آیا می‌خواهید چاپ کنید

### 5️⃣ چاپ فاکتور

- بعد از ثبت فاکتور، پنجره پیش‌نمایش باز می‌شود
- می‌توانید فاکتور را چاپ کنید یا ببندید
- یا از دکمه **🖨️ چاپ آخرین فاکتور** استفاده کنید

### 6️⃣ پشتیبان‌گیری

1. از منوی سمت راست، **💾 پشتیبان‌گیری** را بزنید
2. فایل backup در پوشه Documents ذخیره می‌شود
3. مسیر فایل به شما نمایش داده می‌شود

---

## ⚙️ رفع مشکلات رایج

### مشکل 1: برنامه اجرا نمی‌شود

**راه حل:**
- مطمئن شوید .NET Framework 4.7.2 نصب است
- دانلود: https://dotnet.microsoft.com/download/dotnet-framework/net472

### مشکل 2: خطای دیتابیس

**راه حل:**
- برنامه را Close کنید
- فایل دیتابیس را حذف کنید (مسیر بالا)
- برنامه را دوباره اجرا کنید

### مشکل 3: فونت فارسی نمایش داده نمی‌شود

**راه حل:**
- فونت Vazir را نصب کنید
- یا از فونت‌های سیستمی فارسی استفاده کنید (Tahoma, Arial)

### مشکل 4: چاپ کار نمی‌کند

**راه حل:**
- مطمئن شوید پرینتر نصب و آماده است
- درایور پرینتر را بروز کنید

---

## 🔧 تنظیمات پیشرفته

### تغییر رنگ‌ها:

فایل `App.xaml` را باز کنید و رنگ‌ها را تغییر دهید:

```xml
<!-- Background -->
<SolidColorBrush x:Key="BackgroundBrush" Color="#f3eadd"/>

<!-- Primary -->
<SolidColorBrush x:Key="PrimaryBrush" Color="#31241d"/>

<!-- Secondary -->
<SolidColorBrush x:Key="SecondaryBrush" Color="#c6ad8b"/>

<!-- Accent -->
<SolidColorBrush x:Key="AccentBrush" Color="#04256b"/>
```

### تغییر فونت:

در `App.xaml`:

```xml
<Style TargetType="Window">
    <Setter Property="FontFamily" Value="Tahoma"/>
    <Setter Property="FontSize" Value="14"/>
</Style>
```

---

## 📊 گزارش‌های آماری

### داشبورد:

- فروش امروز (به تومان)
- تعداد کالاها
- تعداد مشتریان
- هشدار موجودی کم (کمتر از 5)

---

## 🔐 امنیت

- دیتابیس به صورت Local ذخیره می‌شود
- نیازی به اینترنت نیست
- اطلاعات در سیستم شما باقی می‌ماند
- حتماً نسخه پشتیبان تهیه کنید

---

## 📞 پشتیبانی

برای هرگونه سوال یا مشکل:

1. فایل log را بررسی کنید
2. از بخش Issues گیت‌هاب استفاده کنید
3. یا با توسعه‌دهنده تماس بگیرید

---

## 🎓 نکات مهم

✅ همیشه قبل از بروزرسانی، نسخه پشتیبان بگیرید
✅ موجودی کالاها را مرتب چک کنید
✅ اطلاعات مشتریان را کامل وارد کنید
✅ فاکتورها را به ترتیب ثبت کنید

---

## 📝 تاریخچه نسخه‌ها

### نسخه 1.0.0 (اولین نسخه)
- مدیریت کالا
- مدیریت مشتریان
- صدور فاکتور
- چاپ فاکتور
- داشبورد
- پشتیبان‌گیری

---

**موفق باشید! 🎉**
