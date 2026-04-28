using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TireShopAccounting.Core.Models;

namespace TireShopAccounting.Data.Repositories
{
    public class ProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        brand as Brand,
                        quantity as Quantity,
                        buy_price as BuyPrice,
                        sell_price as SellPrice,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM products
                    ORDER BY name
                ";

                var products = await connection.QueryAsync<Product>(sql);
                return products;
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        brand as Brand,
                        quantity as Quantity,
                        buy_price as BuyPrice,
                        sell_price as SellPrice,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM products
                    WHERE id = @Id
                ";

                return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        brand as Brand,
                        quantity as Quantity,
                        buy_price as BuyPrice,
                        sell_price as SellPrice,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM products
                    WHERE name LIKE @SearchTerm OR brand LIKE @SearchTerm
                    ORDER BY name
                ";

                var products = await connection.QueryAsync<Product>(
                    sql, 
                    new { SearchTerm = $"%{searchTerm}%" }
                );
                return products;
            }
        }

        public async Task<int> AddAsync(Product product)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    INSERT INTO products (name, brand, quantity, buy_price, sell_price, created_date)
                    VALUES (@Name, @Brand, @Quantity, @BuyPrice, @SellPrice, @CreatedDate);
                    SELECT last_insert_rowid();
                ";

                product.CreatedDate = DateTime.Now;
                var id = await connection.ExecuteScalarAsync<int>(sql, product);
                return id;
            }
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    UPDATE products
                    SET name = @Name,
                        brand = @Brand,
                        quantity = @Quantity,
                        buy_price = @BuyPrice,
                        sell_price = @SellPrice,
                        updated_date = @UpdatedDate
                    WHERE id = @Id
                ";

                product.UpdatedDate = DateTime.Now;
                var affectedRows = await connection.ExecuteAsync(sql, product);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM products WHERE id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateQuantityAsync(int productId, int quantityChange)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    UPDATE products
                    SET quantity = quantity + @QuantityChange,
                        updated_date = @UpdatedDate
                    WHERE id = @ProductId
                ";

                var affectedRows = await connection.ExecuteAsync(sql, new 
                { 
                    ProductId = productId,
                    QuantityChange = quantityChange,
                    UpdatedDate = DateTime.Now
                });
                return affectedRows > 0;
            }
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        brand as Brand,
                        quantity as Quantity,
                        buy_price as BuyPrice,
                        sell_price as SellPrice,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM products
                    WHERE quantity < @Threshold
                    ORDER BY quantity ASC
                ";

                return await connection.QueryAsync<Product>(sql, new { Threshold = threshold });
            }
        }
    }
}
