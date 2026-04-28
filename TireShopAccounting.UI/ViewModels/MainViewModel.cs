using System.Windows.Input;
using TireShopAccounting.UI.Commands;

namespace TireShopAccounting.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

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
            // ایجاد ViewModelها
            DashboardViewModel = new DashboardViewModel();
            ProductViewModel = new ProductViewModel();
            CustomerViewModel = new CustomerViewModel();
            InvoiceViewModel = new InvoiceViewModel();

            // تعریف دستورات
            ShowDashboardCommand = new RelayCommand(() => CurrentViewModel = DashboardViewModel);
            ShowProductsCommand = new RelayCommand(() => CurrentViewModel = ProductViewModel);
            ShowCustomersCommand = new RelayCommand(() => CurrentViewModel = CustomerViewModel);
            ShowInvoiceCommand = new RelayCommand(() => CurrentViewModel = InvoiceViewModel);

            // نمایش اولیه داشبورد
            CurrentViewModel = DashboardViewModel;
        }
    }
}
