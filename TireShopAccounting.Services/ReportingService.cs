using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TireShopAccounting.Core.Models;
using TireShopAccounting.Data.Repositories;

namespace TireShopAccounting.Services
{
    public class ReportingService
    {
        private readonly ProductRepository _productRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly InvoiceRepository _invoiceRepository;

        public ReportingService()
        {
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository();
            _invoiceRepository = new InvoiceRepository();
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            var data = new DashboardData
            {
                TodaySales = await _invoiceRepository.GetTodaySalesAsync(),
                TotalProducts = (await _productRepository.GetAllAsync()).Count(),
                TotalCustomers = (await _customerRepository.GetAllAsync()).Count(),
                LowStockProducts = await _productRepository.GetLowStockProductsAsync(5)
            };

            return data;
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithDebtAsync()
        {
            return await _customerRepository.GetCustomersWithDebtAsync();
        }
    }

    public class DashboardData
    {
        public decimal TodaySales { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public IEnumerable<Product> LowStockProducts { get; set; }
    }
}
