using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TireShopAccounting.Core.Models;
using TireShopAccounting.Data.Repositories;
using TireShopAccounting.UI.Commands;

namespace TireShopAccounting.UI.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        private readonly CustomerRepository _customerRepository;
        private Customer _selectedCustomer;
        private string _searchText;
        private bool _isEditMode;

        // فیلدهای فرم
        private string _customerName;
        private string _customerPhone;
        private string _customerAddress;
        private decimal _balance;

        public ObservableCollection<Customer> Customers { get; set; }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                SetProperty(ref _selectedCustomer, value);
                if (value != null)
                {
                    LoadCustomerToForm(value);
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchCustomersAsync();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        public string CustomerPhone
        {
            get => _customerPhone;
            set => SetProperty(ref _customerPhone, value);
        }

        public string CustomerAddress
        {
            get => _customerAddress;
            set => SetProperty(ref _customerAddress, value);
        }

        public decimal Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }

        public ICommand AddNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }

        public CustomerViewModel()
        {
            _customerRepository = new CustomerRepository();
            Customers = new ObservableCollection<Customer>();

            AddNewCommand = new RelayCommand(PrepareNewCustomer);
            SaveCommand = new RelayCommand(async () => await SaveCustomerAsync());
            DeleteCommand = new RelayCommand(async () => await DeleteCustomerAsync(), () => SelectedCustomer != null);
            CancelCommand = new RelayCommand(ClearForm);

            LoadCustomersAsync();
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

        public void RefreshData()
        {
            LoadCustomersAsync();
        }

        private async void SearchCustomersAsync()
        {
            var customers = await _customerRepository.SearchAsync(SearchText);
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }

        private void PrepareNewCustomer()
        {
            IsEditMode = false;
            SelectedCustomer = null;
            ClearForm();
        }

        private void LoadCustomerToForm(Customer customer)
        {
            IsEditMode = true;
            CustomerName = customer.Name;
            CustomerPhone = customer.Phone;
            CustomerAddress = customer.Address;
            Balance = customer.Balance;
        }

        private async System.Threading.Tasks.Task SaveCustomerAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(CustomerPhone))
                {
                    MessageBox.Show("نام و شماره تماس الزامی است", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var customer = new Customer
                {
                    Name = CustomerName,
                    Phone = CustomerPhone,
                    Address = CustomerAddress,
                    Balance = Balance
                };

                if (IsEditMode && SelectedCustomer != null)
                {
                    customer.Id = SelectedCustomer.Id;
                    await _customerRepository.UpdateAsync(customer);
                    MessageBox.Show("مشتری با موفقیت ویرایش شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await _customerRepository.AddAsync(customer);
                    MessageBox.Show("مشتری با موفقیت اضافه شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadCustomersAsync();
                ClearForm();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task DeleteCustomerAsync()
        {
            if (SelectedCustomer == null)
                return;

            var result = MessageBox.Show(
                $"آیا مطمئن هستید که می‌خواهید {SelectedCustomer.Name} را حذف کنید؟",
                "تأیید حذف",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _customerRepository.DeleteAsync(SelectedCustomer.Id);
                    MessageBox.Show("مشتری با موفقیت حذف شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCustomersAsync();
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
            CustomerName = string.Empty;
            CustomerPhone = string.Empty;
            CustomerAddress = string.Empty;
            Balance = 0;
            IsEditMode = false;
            SelectedCustomer = null;
        }
    }
}
