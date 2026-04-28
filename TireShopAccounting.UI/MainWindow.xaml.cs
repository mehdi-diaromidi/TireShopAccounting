using System;
using System.Windows;
using TireShopAccounting.Data;

namespace TireShopAccounting.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var backupPath = DbConnectionFactory.BackupDatabase();
                MessageBox.Show(
                    $"نسخه پشتیبان با موفقیت ایجاد شد:\n{backupPath}",
                    "موفقیت",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"خطا در تهیه نسخه پشتیبان:\n{ex.Message}",
                    "خطا",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
