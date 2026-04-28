using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TireShopAccounting.Core.Models;
using TireShopAccounting.Data.Repositories;
using TireShopAccounting.Services;
using TireShopAccounting.UI.Commands;

namespace TireShopAccounting.UI.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly InvoiceService _invoiceService;
        private readonly CustomerRepository _customerRepository;
        private readonly ProductRepository _productRepository;
        private readonly PrintService _printService;

        private Customer _selectedCustomer;
        private Product _selectedProduct;
        private int _itemQuantity = 1;
        private decimal _discount;
        private string _searchProductText;

        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<InvoiceItem> InvoiceItems { get; set; }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public int ItemQuantity
        {
            get => _itemQuantity;
            set
            {
                SetProperty(ref _itemQuantity, value);
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(FinalAmount));
            }
        }

        public decimal Discount
        {
            get => _discount;
            set
            {
                SetProperty(ref _discount, value);
                OnPropertyChanged(nameof(FinalAmount));
            }
        }

        public string SearchProductText
        {
            get => _searchProductText;
            set
            {
                SetProperty(ref _searchProductText, value);
                SearchProductsAsync();
            }
        }

        public decimal TotalAmount => InvoiceItems.Sum(i => i.TotalPrice);
        public decimal FinalAmount => TotalAmount - Discount;

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SaveInvoiceCommand { get; }
        public ICommand ClearInvoiceCommand { get; }
        public ICommand PrintInvoiceCommand { get; }

        private Invoice _lastSavedInvoice;

        public InvoiceViewModel()
        {
            _invoiceService = new InvoiceService();
            _customerRepository = new CustomerRepository();
            _productRepository = new ProductRepository();
            _printService = new PrintService();

            Customers = new ObservableCollection<Customer>();
            Products = new ObservableCollection<Product>();
            InvoiceItems = new ObservableCollection<InvoiceItem>();

            AddItemCommand = new RelayCommand(AddItemToInvoice, CanAddItem);
            RemoveItemCommand = new RelayCommand<InvoiceItem>(RemoveItemFromInvoice);
            SaveInvoiceCommand = new RelayCommand(async () => await SaveInvoiceAsync(), CanSaveInvoice);
            ClearInvoiceCommand = new RelayCommand(ClearInvoice);
            PrintInvoiceCommand = new RelayCommand(PrintLastInvoice, () => _lastSavedInvoice != null);

            LoadCustomersAsync();
            LoadProductsAsync();
        }

        private async void LoadCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }

        private async void LoadProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        private async void SearchProductsAsync()
        {
            var products = await _productRepository.SearchAsync(SearchProductText);
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        private bool CanAddItem()
        {
            return SelectedProduct != null && ItemQuantity > 0;
        }

        private void AddItemToInvoice()
        {
            if (SelectedProduct == null || ItemQuantity <= 0)
            {
                MessageBox.Show("لطفاً کالا و تعداد را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedProduct.Quantity < ItemQuantity)
            {
                MessageBox.Show(
                    $"موجودی کافی نیست. موجودی فعلی: {SelectedProduct.Quantity}",
                    "خطا",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // بررسی وجود کالا در لیست
            var existingItem = InvoiceItems.FirstOrDefault(i => i.ProductId == SelectedProduct.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += ItemQuantity;
            }
            else
            {
                var newItem = new InvoiceItem
                {
                    ProductId = SelectedProduct.Id,
                    Product = SelectedProduct,
                    Quantity = ItemQuantity,
                    Price = SelectedProduct.SellPrice
                };
                InvoiceItems.Add(newItem);
            }

            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(FinalAmount));
            ItemQuantity = 1;
        }

        private void RemoveItemFromInvoice(InvoiceItem item)
        {
            if (item != null)
            {
                InvoiceItems.Remove(item);
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(FinalAmount));
            }
        }

        private bool CanSaveInvoice()
        {
            return SelectedCustomer != null && InvoiceItems.Count > 0;
        }

        private async System.Threading.Tasks.Task SaveInvoiceAsync()
        {
            if (SelectedCustomer == null)
            {
                MessageBox.Show("لطفاً مشتری را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (InvoiceItems.Count == 0)
            {
                MessageBox.Show("لطفاً حداقل یک کالا اضافه کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var invoice = new Invoice
                {
                    CustomerId = SelectedCustomer.Id,
                    Customer = SelectedCustomer,
                    Discount = Discount,
                    Items = InvoiceItems.ToList()
                };

                var invoiceId = await _invoiceService.CreateInvoiceAsync(invoice);
                invoice.Id = invoiceId;
                _lastSavedInvoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

                MessageBox.Show(
                    $"فاکتور با موفقیت ثبت شد\nشماره فاکتور: {_lastSavedInvoice.InvoiceNumber}",
                    "موفقیت",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // پیشنهاد چاپ
                var printResult = MessageBox.Show(
                    "آیا می‌خواهید فاکتور را چاپ کنید؟",
                    "چاپ فاکتور",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (printResult == MessageBoxResult.Yes)
                {
                    PrintLastInvoice();
                }

                ClearInvoice();
                LoadProductsAsync(); // به‌روزرسانی موجودی کالاها
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت فاکتور: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInvoice()
        {
            SelectedCustomer = null;
            SelectedProduct = null;
            InvoiceItems.Clear();
            Discount = 0;
            ItemQuantity = 1;
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(FinalAmount));
        }

        private void PrintLastInvoice()
        {
            if (_lastSavedInvoice != null)
            {
                try
                {
                    _printService.PreviewInvoice(_lastSavedInvoice, Application.Current.MainWindow);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در چاپ: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
