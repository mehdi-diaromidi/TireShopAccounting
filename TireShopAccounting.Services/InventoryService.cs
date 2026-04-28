using System.Collections.Generic;
using System.Threading.Tasks;
using TireShopAccounting.Core.Models;
using TireShopAccounting.Data.Repositories;

namespace TireShopAccounting.Services
{
    public class InventoryService
    {
        private readonly ProductRepository _productRepository;

        public InventoryService()
        {
            _productRepository = new ProductRepository();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllProductsAsync();

            return await _productRepository.SearchAsync(searchTerm);
        }

        public async Task<int> AddProductAsync(Product product)
        {
            return await _productRepository.AddAsync(product);
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _productRepository.GetLowStockProductsAsync(5);
        }

        public async Task<bool> CheckStockAvailability(int productId, int requestedQuantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product != null && product.Quantity >= requestedQuantity;
        }
    }
}
