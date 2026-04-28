using System;
using System.Globalization;
using System.Windows.Input;
using TireShopAccounting.UI.Commands;

namespace TireShopAccounting.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private readonly PersianCalendar _persianCalendar;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string PersianDate =>
            $"تاریخ: {_persianCalendar.GetYear(DateTime.Now):0000}/{_persianCalendar.GetMonth(DateTime.Now):00}/{_persianCalendar.GetDayOfMonth(DateTime.Now):00}";

        public DashboardViewModel DashboardViewModel { get; }
        public ProductViewModel ProductViewModel { get; }
        public CustomerViewModel CustomerViewModel { get; }
        public InvoiceViewModel InvoiceViewModel { get; }

        public ICommand ShowDashboardCommand { get; }
        public ICommand ShowProductsCommand { get; }
        public ICommand ShowCustomersCommand { get; }
        public ICommand ShowInvoiceCommand { get; }

        public MainViewModel()
        {
            _persianCalendar = new PersianCalendar();

            // ایجاد ViewModelها
            DashboardViewModel = new DashboardViewModel();
            ProductViewModel = new ProductViewModel();
            CustomerViewModel = new CustomerViewModel();
            InvoiceViewModel = new InvoiceViewModel();

            // تعریف دستورات
            ShowDashboardCommand = new RelayCommand(() =>
            {
                DashboardViewModel.RefreshData();
                CurrentViewModel = DashboardViewModel;
            });
            ShowProductsCommand = new RelayCommand(() =>
            {
                ProductViewModel.RefreshData();
                CurrentViewModel = ProductViewModel;
            });
            ShowCustomersCommand = new RelayCommand(() =>
            {
                CustomerViewModel.RefreshData();
                CurrentViewModel = CustomerViewModel;
            });
            ShowInvoiceCommand = new RelayCommand(() =>
            {
                InvoiceViewModel.RefreshData();
                CurrentViewModel = InvoiceViewModel;
            });

            // نمایش اولیه داشبورد
            DashboardViewModel.RefreshData();
            CurrentViewModel = DashboardViewModel;
        }
    }
}
