# 🏪 نرم‌افزار حسابداری مغازه لاستیک‌فروشی

نرم‌افزار دسکتاپ حسابداری ویندوزی برای مدیریت مغازه‌های لاستیک‌فروشی

---

## 🚀 ساخت فایل نصبی (برای توسعه‌دهندگان)

### سریع‌ترین روش:
```batch
BUILD_INSTALLER.bat
```

**خروجی:** `Installer\Output\TireShopAccounting_Setup.exe`

📖 راهنمای کامل: [QUICK_START.md](QUICK_START.md)

---

## 💻 نصب برنامه (برای کاربران)

### پیش‌نیاز:
- Windows 7 / 8 / 10 / 11
- .NET Framework 4.7.2 یا بالاتر

### مراحل نصب:
1. دانلود `TireShopAccounting_Setup.exe`
2. دابل کلیک و Next, Next, Install
3. اجرا از دسکتاپ یا Start Menu

---

## ⚙️ مشخصات فنی

- **Framework:** .NET Framework 4.7.2
- **UI:** WPF با معماری MVVM
- **Database:** SQLite (Local)
- **ORM:** Dapper
- **زبان:** فارسی (RTL کامل)
- **سیستم‌عامل:** Windows 7, 8, 10, 11

---

## ✨ امکانات

### مدیریت کالا
- ✅ افزودن، ویرایش، حذف کالا
- ✅ جستجوی زنده
- ✅ هشدار موجودی کم (< 5 عدد)
- ✅ قیمت خرید و فروش

### مدیریت مشتریان
- ✅ افزودن، ویرایش، حذف مشتری
- ✅ نمایش مانده حساب
- ✅ جستجو بر اساس نام یا شماره

### صدور فاکتور
- ✅ انتخاب مشتری و کالا
- ✅ محاسبه خودکار
- ✅ اعمال تخفیف
- ✅ کاهش خودکار موجودی
- ✅ بروزرسانی مانده مشتری

### چاپ و گزارش
- ✅ چاپ فاکتور A4
- ✅ پیش‌نمایش قبل از چاپ
- ✅ داشبورد آماری
- ✅ فروش روزانه

### امنیت
- ✅ پشتیبان‌گیری یک‌کلیکه
- ✅ دیتابیس محلی
- ✅ کاملاً آفلاین

---

## 📁 ساختار پروژه

```
TireShopAccounting/
├── TireShopAccounting.UI/          # رابط کاربری WPF
├── TireShopAccounting.Services/    # منطق کسب‌وکار
├── TireShopAccounting.Data/        # دسترسی به دیتابیس
├── TireShopAccounting.Core/        # Models و Entities
├── Installer/                      # فایل‌های Inno Setup
├── BUILD_INSTALLER.bat             # ⭐ اسکریپت ساخت
└── TireShopAccounting.sln          # Solution اصلی
```

---

## 📚 مستندات

| سند | توضیحات |
|-----|---------|
| [QUICK_START.md](QUICK_START.md) | راهنمای سریع ساخت Installer |
| [INSTALLATION_GUIDE.md](INSTALLATION_GUIDE.md) | راهنمای نصب و استفاده |
| [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) | راهنمای توسعه‌دهنده |
| [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) | راهنمای Deploy و انتشار |
| [TEST_SCENARIOS.md](TEST_SCENARIOS.md) | سناریوهای تست |

---

## 🎯 برای شروع سریع

### توسعه‌دهنده:
```batch
1. نصب Visual Studio 2019+
2. نصب Inno Setup 6
3. دابل کلیک: BUILD_INSTALLER.bat
4. منتظر بمانید...
5. فایل Setup آماده است! ✓
```

### کاربر:
```batch
1. دانلود TireShopAccounting_Setup.exe
2. نصب
3. استفاده
```

---

## 🔧 پشتیبانی

- **دیتابیس:** `%AppData%\TireShopAccounting\database.db`
- **بکاپ:** `Documents\TireShopBackups\`
- **نیاز به اینترنت:** خیر ❌

---

## 📝 لایسنس

این پروژه برای استفاده شخصی و تجاری آزاد است.

---

**ساخته شده با ❤️ برای مغازه‌داران**
