using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TireShopAccounting.Core.Models;

namespace TireShopAccounting.Data.Repositories
{
    public class InvoiceRepository
    {
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        i.id as Id,
                        i.customer_id as CustomerId,
                        i.date as Date,
                        i.total_amount as TotalAmount,
                        i.discount as Discount,
                        c.id as Id,
                        c.name as Name,
                        c.phone as Phone,
                        c.address as Address,
                        c.balance as Balance
                    FROM invoices i
                    INNER JOIN customers c ON i.customer_id = c.id
                    ORDER BY i.date DESC
                ";

                var invoices = new Dictionary<int, Invoice>();

                await connection.QueryAsync<Invoice, Customer, Invoice>(
                    sql,
                    (invoice, customer) =>
                    {
                        if (!invoices.TryGetValue(invoice.Id, out var existingInvoice))
                        {
                            existingInvoice = invoice;
                            existingInvoice.Customer = customer;
                            invoices.Add(invoice.Id, existingInvoice);
                        }
                        return existingInvoice;
                    },
                    splitOn: "Id"
                );

                return invoices.Values;
            }
        }

        public async Task<Invoice> GetByIdAsync(int id)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                // دریافت فاکتور
                var invoiceSql = @"
                    SELECT 
                        i.id as Id,
                        i.customer_id as CustomerId,
                        i.date as Date,
                        i.total_amount as TotalAmount,
                        i.discount as Discount,
                        c.id as Id,
                        c.name as Name,
                        c.phone as Phone,
                        c.address as Address,
                        c.balance as Balance
                    FROM invoices i
                    INNER JOIN customers c ON i.customer_id = c.id
                    WHERE i.id = @Id
                ";

                Invoice invoice = null;

                await connection.QueryAsync<Invoice, Customer, Invoice>(
                    invoiceSql,
                    (inv, customer) =>
                    {
                        invoice = inv;
                        invoice.Customer = customer;
                        return invoice;
                    },
                    new { Id = id },
                    splitOn: "Id"
                );

                if (invoice == null)
                    return null;

                // دریافت اقلام فاکتور
                var itemsSql = @"
                    SELECT 
                        ii.id as Id,
                        ii.invoice_id as InvoiceId,
                        ii.product_id as ProductId,
                        ii.quantity as Quantity,
                        ii.price as Price,
                        p.id as Id,
                        p.name as Name,
                        p.brand as Brand,
                        p.quantity as Quantity,
                        p.buy_price as BuyPrice,
                        p.sell_price as SellPrice
                    FROM invoice_items ii
                    INNER JOIN products p ON ii.product_id = p.id
                    WHERE ii.invoice_id = @InvoiceId
                ";

                var items = await connection.QueryAsync<InvoiceItem, Product, InvoiceItem>(
                    itemsSql,
                    (item, product) =>
                    {
                        item.Product = product;
                        return item;
                    },
                    new { InvoiceId = id },
                    splitOn: "Id"
                );

                invoice.Items = items.ToList();
                return invoice;
            }
        }

        public async Task<int> AddAsync(Invoice invoice)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // درج فاکتور
                        var invoiceSql = @"
                            INSERT INTO invoices (customer_id, date, total_amount, discount, final_amount)
                            VALUES (@CustomerId, @Date, @TotalAmount, @Discount, @FinalAmount);
                            SELECT last_insert_rowid();
                        ";

                        var invoiceId = await connection.ExecuteScalarAsync<int>(
                            invoiceSql,
                            new
                            {
                                invoice.CustomerId,
                                Date = invoice.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                                invoice.TotalAmount,
                                invoice.Discount,
                                FinalAmount = invoice.FinalAmount
                            },
                            transaction
                        );

                        // درج اقلام فاکتور
                        var itemSql = @"
                            INSERT INTO invoice_items (invoice_id, product_id, quantity, price)
                            VALUES (@InvoiceId, @ProductId, @Quantity, @Price)
                        ";

                        foreach (var item in invoice.Items)
                        {
                            await connection.ExecuteAsync(
                                itemSql,
                                new
                                {
                                    InvoiceId = invoiceId,
                                    item.ProductId,
                                    item.Quantity,
                                    item.Price
                                },
                                transaction
                            );

                            // کاهش موجودی کالا
                            var updateQuantitySql = @"
                                UPDATE products
                                SET quantity = quantity - @Quantity
                                WHERE id = @ProductId
                            ";

                            await connection.ExecuteAsync(
                                updateQuantitySql,
                                new { item.ProductId, item.Quantity },
                                transaction
                            );
                        }

                        // بروزرسانی مانده مشتری
                        var updateBalanceSql = @"
                            UPDATE customers
                            SET balance = balance + @Amount
                            WHERE id = @CustomerId
                        ";

                        await connection.ExecuteAsync(
                            updateBalanceSql,
                            new { invoice.CustomerId, Amount = invoice.FinalAmount },
                            transaction
                        );

                        transaction.Commit();
                        return invoiceId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<Invoice>> GetTodayInvoicesAsync()
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                var sql = @"
                    SELECT 
                        i.id as Id,
                        i.customer_id as CustomerId,
                        i.date as Date,
                        i.total_amount as TotalAmount,
                        i.discount as Discount,
                        c.id as Id,
                        c.name as Name,
                        c.phone as Phone
                    FROM invoices i
                    INNER JOIN customers c ON i.customer_id = c.id
                    WHERE i.date >= @Today AND i.date < @Tomorrow
                    ORDER BY i.date DESC
                ";

                var invoices = new Dictionary<int, Invoice>();

                await connection.QueryAsync<Invoice, Customer, Invoice>(
                    sql,
                    (invoice, customer) =>
                    {
                        if (!invoices.TryGetValue(invoice.Id, out var existingInvoice))
                        {
                            existingInvoice = invoice;
                            existingInvoice.Customer = customer;
                            invoices.Add(invoice.Id, existingInvoice);
                        }
                        return existingInvoice;
                    },
                    new { Today = today, Tomorrow = tomorrow },
                    splitOn: "Id"
                );

                return invoices.Values;
            }
        }

        public async Task<decimal> GetTodaySalesAsync()
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                var sql = @"
                    SELECT COALESCE(SUM(final_amount), 0)
                    FROM invoices
                    WHERE date >= @Today AND date < @Tomorrow
                ";

                return await connection.ExecuteScalarAsync<decimal>(
                    sql,
                    new { Today = today, Tomorrow = tomorrow }
                );
            }
        }
    }
}
