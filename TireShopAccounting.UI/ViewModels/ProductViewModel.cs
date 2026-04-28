using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TireShopAccounting.Core.Models;
using TireShopAccounting.Services;
using TireShopAccounting.UI.Commands;

namespace TireShopAccounting.UI.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly InventoryService _inventoryService;
        private Product _selectedProduct;
        private string _searchText;
        private bool _isEditMode;

        // فیلدهای فرم
        private string _productName;
        private string _productBrand;
        private int _productQuantity;
        private decimal _buyPrice;
        private decimal _sellPrice;

        public ObservableCollection<Product> Products { get; set; }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                SetProperty(ref _selectedProduct, value);
                if (value != null)
                {
                    LoadProductToForm(value);
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchProductsAsync();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public string ProductBrand
        {
            get => _productBrand;
            set => SetProperty(ref _productBrand, value);
        }

        public int ProductQuantity
        {
            get => _productQuantity;
            set => SetProperty(ref _productQuantity, value);
        }

        public decimal BuyPrice
        {
            get => _buyPrice;
            set => SetProperty(ref _buyPrice, value);
        }

        public decimal SellPrice
        {
            get => _sellPrice;
            set => SetProperty(ref _sellPrice, value);
        }

        public ICommand AddNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }

        public ProductViewModel()
        {
            _inventoryService = new InventoryService();
            Products = new ObservableCollection<Product>();

            AddNewCommand = new RelayCommand(PrepareNewProduct);
            SaveCommand = new RelayCommand(async () => await SaveProductAsync());
            DeleteCommand = new RelayCommand(async () => await DeleteProductAsync(), () => SelectedProduct != null);
            CancelCommand = new RelayCommand(ClearForm);

            LoadProductsAsync();
        }

        private async void LoadProductsAsync()
        {
            var products = await _inventoryService.GetAllProductsAsync();
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        public void RefreshData()
        {
            LoadProductsAsync();
        }

        private async void SearchProductsAsync()
        {
            var products = await _inventoryService.SearchProductsAsync(SearchText);
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        private void PrepareNewProduct()
        {
            IsEditMode = false;
            SelectedProduct = null;
            ClearForm();
        }

        private void LoadProductToForm(Product product)
        {
            IsEditMode = true;
            ProductName = product.Name;
            ProductBrand = product.Brand;
            ProductQuantity = product.Quantity;
            BuyPrice = product.BuyPrice;
            SellPrice = product.SellPrice;
        }

        private async System.Threading.Tasks.Task SaveProductAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ProductName) || string.IsNullOrWhiteSpace(ProductBrand))
                {
                    MessageBox.Show("نام کالا و برند الزامی است", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var product = new Product
                {
                    Name = ProductName,
                    Brand = ProductBrand,
                    Quantity = ProductQuantity,
                    BuyPrice = BuyPrice,
                    SellPrice = SellPrice
                };

                if (IsEditMode && SelectedProduct != null)
                {
                    product.Id = SelectedProduct.Id;
                    await _inventoryService.UpdateProductAsync(product);
                    MessageBox.Show("کالا با موفقیت ویرایش شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await _inventoryService.AddProductAsync(product);
                    MessageBox.Show("کالا با موفقیت اضافه شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadProductsAsync();
                ClearForm();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task DeleteProductAsync()
        {
            if (SelectedProduct == null)
                return;

            var result = MessageBox.Show(
                $"آیا مطمئن هستید که می‌خواهید {SelectedProduct.DisplayName} را حذف کنید؟",
                "تأیید حذف",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _inventoryService.DeleteProductAsync(SelectedProduct.Id);
                    MessageBox.Show("کالا با موفقیت حذف شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProductsAsync();
                    ClearForm();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"خطا در حذف: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearForm()
        {
            ProductName = string.Empty;
            ProductBrand = string.Empty;
            ProductQuantity = 0;
            BuyPrice = 0;
            SellPrice = 0;
            IsEditMode = false;
            SelectedProduct = null;
        }
    }
}
