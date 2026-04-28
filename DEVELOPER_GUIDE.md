# 🔧 راهنمای توسعه‌دهنده

## 📐 معماری پروژه

این پروژه بر اساس الگوی **MVVM** (Model-View-ViewModel) طراحی شده است.

### ساختار Layered:

```
┌─────────────────────────────────────┐
│   UI Layer (WPF - XAML)             │  ← Views + ViewModels
├─────────────────────────────────────┤
│   Services Layer                     │  ← Business Logic
├─────────────────────────────────────┤
│   Data Layer                         │  ← Repositories + Dapper
├─────────────────────────────────────┤
│   Core Layer                         │  ← Models + Entities
├─────────────────────────────────────┤
│   Database (SQLite)                  │  ← Local File
└─────────────────────────────────────┘
```

---

## 📦 پروژه‌ها و مسئولیت‌ها

### 1. TireShopAccounting.Core
**مسئولیت:** تعریف موجودیت‌های اصلی (Models)

**کلاس‌های موجود:**
- `Product.cs` - مدل کالا
- `Customer.cs` - مدل مشتری
- `Invoice.cs` - مدل فاکتور
- `InvoiceItem.cs` - مدل اقلام فاکتور

**Dependencies:**
- System.ComponentModel.DataAnnotations (برای Validation)

---

### 2. TireShopAccounting.Data
**مسئولیت:** دسترسی به دیتابیس و عملیات CRUD

**کلاس‌های اصلی:**
- `DbConnectionFactory.cs` - مدیریت اتصال SQLite
- `ProductRepository.cs` - عملیات CRUD کالا
- `CustomerRepository.cs` - عملیات CRUD مشتری
- `InvoiceRepository.cs` - عملیات CRUD فاکتور

**Technologies:**
- **Dapper** - Micro ORM برای سرعت بالا
- **SQLite** - دیتابیس محلی

**نکات مهم:**
- همه متدها `async/await` هستند
- استفاده از Transaction برای عملیات مرتبط
- Auto-initialization دیتابیس در اولین اجرا

---

### 3. TireShopAccounting.Services
**مسئولیت:** منطق کسب‌وکار و orchestration

**سرویس‌ها:**
- `InventoryService.cs` - مدیریت موجودی
- `InvoiceService.cs` - منطق فاکتور
- `ReportingService.cs` - گزارش‌گیری
- `PrintService.cs` - چاپ فاکتور

**نکات:**
- اعتبارسنجی business rules
- Coordination بین repositories
- محاسبات مالی

---

### 4. TireShopAccounting.UI
**مسئولیت:** رابط کاربری و نمایش

**ساختار:**
```
UI/
├── Views/              # XAML Views
│   ├── DashboardView.xaml
│   ├── ProductsView.xaml
│   ├── CustomersView.xaml
│   └── InvoiceView.xaml
├── ViewModels/         # Logic پشت Views
│   ├── ViewModelBase.cs
│   ├── MainViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── ProductViewModel.cs
│   ├── CustomerViewModel.cs
│   └── InvoiceViewModel.cs
├── Commands/           # ICommand implementations
│   └── RelayCommand.cs
├── Converters/         # Value Converters
│   └── ValueConverters.cs
├── App.xaml           # Resources & Styles
└── MainWindow.xaml    # Shell window
```

---

## 🎨 MVVM Pattern

### Data Flow:

```
View (XAML)
    ↕ (Data Binding)
ViewModel
    ↕ (Commands/Properties)
Services
    ↕ (Business Logic)
Repositories
    ↕ (Data Access)
Database
```

### مثال CRUD کامل:

#### 1. Model (Core Layer)
```csharp
public class Product
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}
```

#### 2. Repository (Data Layer)
```csharp
public async Task<int> AddAsync(Product product)
{
    using (var connection = DbConnectionFactory.CreateConnection())
    {
        var sql = @"
            INSERT INTO products (name, quantity)
            VALUES (@Name, @Quantity);
            SELECT last_insert_rowid();
        ";
        return await connection.ExecuteScalarAsync<int>(sql, product);
    }
}
```

#### 3. Service (Services Layer)
```csharp
public async Task<int> AddProductAsync(Product product)
{
    // Business validation
    if (product.Quantity < 0)
        throw new Exception("موجودی نمی‌تواند منفی باشد");
        
    return await _productRepository.AddAsync(product);
}
```

#### 4. ViewModel (UI Layer)
```csharp
public class ProductViewModel : ViewModelBase
{
    private readonly InventoryService _service;
    private string _productName;
    
    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value);
    }
    
    public ICommand SaveCommand { get; }
    
    public ProductViewModel()
    {
        _service = new InventoryService();
        SaveCommand = new RelayCommand(async () => await SaveAsync());
    }
    
    private async Task SaveAsync()
    {
        var product = new Product { Name = ProductName };
        await _service.AddProductAsync(product);
    }
}
```

#### 5. View (XAML)
```xml
<TextBox Text="{Binding ProductName, UpdateSourceTrigger=PropertyChanged}"/>
<Button Content="ذخیره" Command="{Binding SaveCommand}"/>
```

---

## 🗄️ دیتابیس

### Schema:

```sql
-- Products
CREATE TABLE products (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    brand TEXT NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 0,
    buy_price REAL NOT NULL DEFAULT 0,
    sell_price REAL NOT NULL DEFAULT 0,
    created_date TEXT NOT NULL,
    updated_date TEXT
);

-- Customers
CREATE TABLE customers (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    phone TEXT NOT NULL,
    address TEXT,
    balance REAL NOT NULL DEFAULT 0,
    created_date TEXT NOT NULL,
    updated_date TEXT
);

-- Invoices
CREATE TABLE invoices (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    customer_id INTEGER NOT NULL,
    date TEXT NOT NULL,
    total_amount REAL NOT NULL,
    discount REAL NOT NULL DEFAULT 0,
    final_amount REAL NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES customers(id)
);

-- Invoice Items
CREATE TABLE invoice_items (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    invoice_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    quantity INTEGER NOT NULL,
    price REAL NOT NULL,
    FOREIGN KEY (invoice_id) REFERENCES invoices(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);
```

### Indexes:
```sql
CREATE INDEX idx_invoices_customer ON invoices(customer_id);
CREATE INDEX idx_invoice_items_invoice ON invoice_items(invoice_id);
CREATE INDEX idx_invoice_items_product ON invoice_items(product_id);
```

---

## 🔄 Transaction Management

مثال ثبت فاکتور (چند عملیات در یک Transaction):

```csharp
using (var connection = DbConnectionFactory.CreateConnection())
{
    connection.Open();
    using (var transaction = connection.BeginTransaction())
    {
        try
        {
            // 1. درج فاکتور
            var invoiceId = await InsertInvoice(connection, transaction);
            
            // 2. درج اقلام
            await InsertInvoiceItems(connection, transaction, invoiceId);
            
            // 3. کاهش موجودی
            await UpdateInventory(connection, transaction);
            
            // 4. بروزرسانی مانده مشتری
            await UpdateCustomerBalance(connection, transaction);
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
```

---

## 🎯 Data Binding

### One-Way Binding:
```xml
<TextBlock Text="{Binding TotalAmount}"/>
```

### Two-Way Binding:
```xml
<TextBox Text="{Binding ProductName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
```

### Command Binding:
```xml
<Button Content="ذخیره" Command="{Binding SaveCommand}"/>
```

### Collection Binding:
```xml
<DataGrid ItemsSource="{Binding Products}" SelectedItem="{Binding SelectedProduct}"/>
```

---

## 💡 Best Practices

### 1. ViewModelBase Pattern
```csharp
public class ViewModelBase : INotifyPropertyChanged
{
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

### 2. RelayCommand Pattern
```csharp
public ICommand SaveCommand { get; }

SaveCommand = new RelayCommand(
    execute: async () => await SaveAsync(),
    canExecute: () => !string.IsNullOrEmpty(ProductName)
);
```

### 3. Async/Await در ViewModel
```csharp
private async Task LoadDataAsync()
{
    try
    {
        var products = await _service.GetAllProductsAsync();
        Products.Clear();
        foreach (var product in products)
            Products.Add(product);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"خطا: {ex.Message}");
    }
}
```

---

## 🧪 نحوه تست

### Unit Test نمونه:

```csharp
[TestClass]
public class ProductServiceTests
{
    [TestMethod]
    public async Task AddProduct_ValidProduct_ReturnsId()
    {
        // Arrange
        var service = new InventoryService();
        var product = new Product 
        { 
            Name = "Test", 
            Quantity = 10 
        };
        
        // Act
        var id = await service.AddProductAsync(product);
        
        // Assert
        Assert.IsTrue(id > 0);
    }
}
```

---

## 🚀 افزودن ویژگی جدید

### مثال: اضافه کردن تاریخ انقضا به کالا

#### گام 1: Model
```csharp
public class Product
{
    // ... existing properties
    public DateTime? ExpiryDate { get; set; }
}
```

#### گام 2: Migration
```csharp
ALTER TABLE products ADD COLUMN expiry_date TEXT;
```

#### گام 3: Repository
```csharp
public async Task<IEnumerable<Product>> GetExpiredProductsAsync()
{
    var sql = @"
        SELECT * FROM products 
        WHERE expiry_date < @Today
    ";
    return await connection.QueryAsync<Product>(sql, new { Today = DateTime.Today });
}
```

#### گام 4: ViewModel
```csharp
public DateTime? ExpiryDate
{
    get => _expiryDate;
    set => SetProperty(ref _expiryDate, value);
}
```

#### گام 5: View
```xml
<DatePicker SelectedDate="{Binding ExpiryDate}"/>
```

---

## 📚 منابع مفید

- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [MVVM Pattern](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [SQLite Documentation](https://www.sqlite.org/docs.html)

---

## 🐛 Debug و Troubleshooting

### چک کردن SQL Queries:
```csharp
// در Dapper می‌توانید query را log کنید
Debug.WriteLine($"Executing: {sql}");
var result = await connection.QueryAsync<Product>(sql);
```

### بررسی Data Binding:
```xml
<!-- اضافه کردن Trace برای Debug -->
<TextBox Text="{Binding ProductName, 
    PresentationTraceSources.TraceLevel=High}"/>
```

### چک کردن Database Path:
```csharp
var dbPath = DbConnectionFactory.ConnectionString;
Console.WriteLine($"Database: {dbPath}");
```

---

**موفق باشید! 💻**
