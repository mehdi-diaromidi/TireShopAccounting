# 🎯 راهنمای سریع ساخت فایل نصبی

## ⚡ سریع‌ترین روش (3 قدم ساده!)

### قدم 1️⃣: نصب ابزارها (فقط یک بار)

1. **نصب Visual Studio 2019 یا 2022**
   - دانلود: https://visualstudio.microsoft.com/downloads/
   - در نصب، حتماً گزینه **".NET desktop development"** را انتخاب کنید
   
2. **نصب Inno Setup 6**
   - دانلود: https://jrsoftware.org/isdl.php
   - فایل: `innosetup-6.2.0.exe`
   - نصب کامل (Next, Next, Next)

### قدم 2️⃣: آماده‌سازی پروژه

1. فایل ZIP پروژه را Extract کنید
2. به پوشه اصلی بروید (جایی که فایل `TireShopAccounting.sln` است)

### قدم 3️⃣: ساخت فایل نصبی

**فقط دابل کلیک روی:**
```
BUILD_INSTALLER.bat
```

✅ همین! فایل نصبی آماده است در:
```
Installer\Output\TireShopAccounting_Setup.exe
```

---

## 📋 جزئیات بیشتر

### چه اتفاقی می‌افتد؟

اسکریپت `BUILD_INSTALLER.bat` به ترتیب:

1. ✓ MSBuild را پیدا می‌کند
2. ✓ NuGet packages را بازیابی می‌کند
3. ✓ Build‌های قبلی را پاک می‌کند
4. ✓ پروژه را در حالت Release می‌سازد
5. ✓ فایل‌های ضروری را بررسی می‌کند
6. ✓ Installer را با Inno Setup می‌سازد

### اگر مشکلی پیش آمد؟

#### خطا: MSBuild not found
```
راه حل: Visual Studio را نصب کنید
```

#### خطا: Inno Setup not found
```
راه حل: 
- برنامه بدون مشکل Build شده
- فایل exe در: TireShopAccounting.UI\bin\Release
- برای Installer، Inno Setup نصب کنید
```

#### خطا: NuGet packages
```
راه حل:
1. Visual Studio را باز کنید
2. Solution را باز کنید
3. کلیک راست روی Solution > Restore NuGet Packages
4. دوباره BUILD_INSTALLER.bat را اجرا کنید
```

---

## 🎁 خروجی نهایی

بعد از اجرای موفق، شما دو گزینه دارید:

### گزینه A: فایل نصبی (Installer)
```
📁 Installer\Output\TireShopAccounting_Setup.exe
```
- حجم: ~15-20 MB
- نصب استاندارد ویندوزی
- ایجاد میانبر روی دسکتاپ
- اضافه شدن به Programs & Features

### گزینه B: فایل‌های مستقیم (Portable)
```
📁 TireShopAccounting.UI\bin\Release\
   ├── TireShopAccounting.exe ⭐
   ├── TireShopAccounting.Core.dll
   ├── TireShopAccounting.Data.dll
   ├── TireShopAccounting.Services.dll
   ├── System.Data.SQLite.dll
   ├── Dapper.dll
   └── ...
```
- اجرای مستقیم بدون نصب
- قابل کپی روی فلش

---

## 🚀 نصب برنامه

### روی کامپیوتر خودتان:
1. دابل کلیک `TireShopAccounting_Setup.exe`
2. Next, Next, Install
3. Done!

### روی کامپیوتر دیگران:
1. فایل `TireShopAccounting_Setup.exe` را کپی کنید
2. به آن‌ها بدهید
3. آن‌ها نصب کنند
4. همین!

---

## 💡 نکات مهم

⚠️ **قبل از توزیع:**
- [ ] حتماً روی سیستم دیگری تست کنید
- [ ] مطمئن شوید .NET Framework 4.7.2 نصب است
- [ ] ویروس اسکن کنید

✅ **مزایای فایل نصبی:**
- نصب حرفه‌ای
- آیکون در Start Menu
- قابلیت Uninstall
- به‌روزرسانی آسان

---

## ⏱️ زمان مورد نیاز

- نصب ابزارها: 20-30 دقیقه (یک بار)
- ساخت اولین Installer: 2-3 دقیقه
- ساخت‌های بعدی: 1-2 دقیقه

---

## 📞 کمک نیاز دارید؟

اگر هر مشکلی داشتید:

1. پیام خطا را کامل بخوانید
2. فایل log را چک کنید
3. مراحل را دوباره انجام دهید
4. راهنمای کامل را بخوانید: `DEPLOYMENT_GUIDE.md`

---

**موفق باشید! 🎉**
