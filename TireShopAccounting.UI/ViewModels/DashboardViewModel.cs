using System.Collections.ObjectModel;
using System.Windows.Input;
using TireShopAccounting.Core.Models;
using TireShopAccounting.Services;
using TireShopAccounting.UI.Commands;

namespace TireShopAccounting.UI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ReportingService _reportingService;
        private decimal _todaySales;
        private int _totalProducts;
        private int _totalCustomers;

        public decimal TodaySales
        {
            get => _todaySales;
            set => SetProperty(ref _todaySales, value);
        }

        public int TotalProducts
        {
            get => _totalProducts;
            set => SetProperty(ref _totalProducts, value);
        }

        public int TotalCustomers
        {
            get => _totalCustomers;
            set => SetProperty(ref _totalCustomers, value);
        }

        public ObservableCollection<Product> LowStockProducts { get; set; }

        public ICommand RefreshCommand { get; }

        public DashboardViewModel()
        {
            _reportingService = new ReportingService();
            LowStockProducts = new ObservableCollection<Product>();

            RefreshCommand = new RelayCommand(async () => await LoadDashboardDataAsync());

            // بارگذاری اولیه
            LoadDashboardDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDashboardDataAsync()
        {
            var data = await _reportingService.GetDashboardDataAsync();

            TodaySales = data.TodaySales;
            TotalProducts = data.TotalProducts;
            TotalCustomers = data.TotalCustomers;

            LowStockProducts.Clear();
            foreach (var product in data.LowStockProducts)
            {
                LowStockProducts.Add(product);
            }
        }

        public void RefreshData()
        {
            LoadDashboardDataAsync();
        }
    }
}
