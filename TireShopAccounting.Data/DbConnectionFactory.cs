using System;
using System.Data;
using System.IO;
using Dapper;

namespace TireShopAccounting.Data
{
    public class DbConnectionFactory
    {
        private static readonly string DbFileName = "database.db";
        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TireShopAccounting",
            DbFileName
        );

        public static string ConnectionString => $"Data Source={DbPath};Version=3;";

        static DbConnectionFactory()
        {
            InitializeDatabase();
        }

        public static IDbConnection CreateConnection()
        {
            var sqliteType = Type.GetType("System.Data.SQLite.SQLiteConnection, System.Data.SQLite", throwOnError: true);
            return (IDbConnection)Activator.CreateInstance(sqliteType, ConnectionString);
        }

        private static void InitializeDatabase()
        {
            // ایجاد پوشه در صورت عدم وجود
            var directory = Path.GetDirectoryName(DbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // ایجاد دیتابیس در صورت عدم وجود
            bool isNewDatabase = !File.Exists(DbPath);

            if (isNewDatabase)
            {
                using (File.Create(DbPath)) { }
                CreateTables();
            }
        }

        private static void CreateTables()
        {
            using (var connection = CreateConnection())
            {
                connection.Open();

                // جدول محصولات
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS products (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        brand TEXT NOT NULL,
                        quantity INTEGER NOT NULL DEFAULT 0,
                        buy_price REAL NOT NULL DEFAULT 0,
                        sell_price REAL NOT NULL DEFAULT 0,
                        created_date TEXT NOT NULL,
                        updated_date TEXT
                    )
                ");

                // جدول مشتریان
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS customers (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        phone TEXT NOT NULL,
                        address TEXT,
                        balance REAL NOT NULL DEFAULT 0,
                        created_date TEXT NOT NULL,
                        updated_date TEXT
                    )
                ");

                // جدول فاکتورها
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS invoices (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        customer_id INTEGER NOT NULL,
                        date TEXT NOT NULL,
                        total_amount REAL NOT NULL,
                        discount REAL NOT NULL DEFAULT 0,
                        final_amount REAL NOT NULL,
                        FOREIGN KEY (customer_id) REFERENCES customers(id)
                    )
                ");

                // جدول اقلام فاکتور
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS invoice_items (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        invoice_id INTEGER NOT NULL,
                        product_id INTEGER NOT NULL,
                        quantity INTEGER NOT NULL,
                        price REAL NOT NULL,
                        FOREIGN KEY (invoice_id) REFERENCES invoices(id),
                        FOREIGN KEY (product_id) REFERENCES products(id)
                    )
                ");

                // ایجاد ایندکس‌ها برای بهبود کارایی
                connection.Execute(@"
                    CREATE INDEX IF NOT EXISTS idx_invoices_customer 
                    ON invoices(customer_id)
                ");

                connection.Execute(@"
                    CREATE INDEX IF NOT EXISTS idx_invoice_items_invoice 
                    ON invoice_items(invoice_id)
                ");

                connection.Execute(@"
                    CREATE INDEX IF NOT EXISTS idx_invoice_items_product 
                    ON invoice_items(product_id)
                ");
            }
        }

        // متد برای پشتیبان‌گیری
        public static string BackupDatabase()
        {
            try
            {
                var backupFileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                var backupPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "TireShopBackups",
                    backupFileName
                );

                var backupDirectory = Path.GetDirectoryName(backupPath);
                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }

                File.Copy(DbPath, backupPath, true);
                return backupPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در تهیه نسخه پشتیبان: {ex.Message}");
            }
        }
    }
}
