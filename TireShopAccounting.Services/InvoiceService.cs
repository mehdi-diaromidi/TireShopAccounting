using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TireShopAccounting.Core.Models;
using TireShopAccounting.Data.Repositories;

namespace TireShopAccounting.Services
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly ProductRepository _productRepository;
        private readonly CustomerRepository _customerRepository;

        public InvoiceService()
        {
            _invoiceRepository = new InvoiceRepository();
            _productRepository = new ProductRepository();
            _customerRepository = new CustomerRepository();
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _invoiceRepository.GetAllAsync();
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            return await _invoiceRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateInvoiceAsync(Invoice invoice)
        {
            // اعتبارسنجی
            if (invoice.Items == null || invoice.Items.Count == 0)
                throw new Exception("فاکتور باید حداقل یک قلم کالا داشته باشد");

            // بررسی موجودی
            foreach (var item in invoice.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"کالا با شناسه {item.ProductId} یافت نشد");

                if (product.Quantity < item.Quantity)
                    throw new Exception($"موجودی کافی برای {product.DisplayName} وجود ندارد. موجودی فعلی: {product.Quantity}");
            }

            // محاسبه مبالغ
            decimal totalAmount = 0;
            foreach (var item in invoice.Items)
            {
                totalAmount += item.TotalPrice;
            }

            invoice.TotalAmount = totalAmount;
            invoice.Date = DateTime.Now;

            // ثبت فاکتور
            return await _invoiceRepository.AddAsync(invoice);
        }

        public async Task<IEnumerable<Invoice>> GetTodayInvoicesAsync()
        {
            return await _invoiceRepository.GetTodayInvoicesAsync();
        }

        public async Task<decimal> GetTodaySalesAsync()
        {
            return await _invoiceRepository.GetTodaySalesAsync();
        }
    }
}
