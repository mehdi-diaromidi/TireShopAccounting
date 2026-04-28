using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using TireShopAccounting.Core.Models;

namespace TireShopAccounting.Data.Repositories
{
    public class CustomerRepository
    {
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        phone as Phone,
                        address as Address,
                        balance as Balance,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM customers
                    ORDER BY name
                ";

                return await connection.QueryAsync<Customer>(sql);
            }
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        phone as Phone,
                        address as Address,
                        balance as Balance,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM customers
                    WHERE id = @Id
                ";

                return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        phone as Phone,
                        address as Address,
                        balance as Balance,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM customers
                    WHERE name LIKE @SearchTerm OR phone LIKE @SearchTerm
                    ORDER BY name
                ";

                return await connection.QueryAsync<Customer>(
                    sql,
                    new { SearchTerm = $"%{searchTerm}%" }
                );
            }
        }

        public async Task<int> AddAsync(Customer customer)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    INSERT INTO customers (name, phone, address, balance, created_date)
                    VALUES (@Name, @Phone, @Address, @Balance, @CreatedDate);
                    SELECT last_insert_rowid();
                ";

                customer.CreatedDate = DateTime.Now;
                var id = await connection.ExecuteScalarAsync<int>(sql, customer);
                return id;
            }
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    UPDATE customers
                    SET name = @Name,
                        phone = @Phone,
                        address = @Address,
                        balance = @Balance,
                        updated_date = @UpdatedDate
                    WHERE id = @Id
                ";

                customer.UpdatedDate = DateTime.Now;
                var affectedRows = await connection.ExecuteAsync(sql, customer);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM customers WHERE id = @Id";
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateBalanceAsync(int customerId, decimal amount)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    UPDATE customers
                    SET balance = balance + @Amount,
                        updated_date = @UpdatedDate
                    WHERE id = @CustomerId
                ";

                var affectedRows = await connection.ExecuteAsync(sql, new
                {
                    CustomerId = customerId,
                    Amount = amount,
                    UpdatedDate = DateTime.Now
                });
                return affectedRows > 0;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithDebtAsync()
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        id as Id,
                        name as Name,
                        phone as Phone,
                        address as Address,
                        balance as Balance,
                        created_date as CreatedDate,
                        updated_date as UpdatedDate
                    FROM customers
                    WHERE balance > 0
                    ORDER BY balance DESC
                ";

                return await connection.QueryAsync<Customer>(sql);
            }
        }
    }
}
