using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TireShopAccounting.UI
{
    public partial class App : Application
    {
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TireShopAccounting",
            "logs");

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                // تنظیم فرهنگ برنامه برای فارسی
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fa-IR");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fa-IR");

                var mainWindow = new MainWindow();
                MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                ShowFatalError(ex, "خطا هنگام شروع برنامه");
                Shutdown(-1);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("Unknown unhandled exception");
            ShowFatalError(ex, "خطای غیرمنتظره برنامه");
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowFatalError(e.Exception, "خطا در رابط کاربری");
            e.Handled = true;
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ShowFatalError(e.Exception, "خطا در پردازش پس‌زمینه");
            e.SetObserved();
        }

        private static void ShowFatalError(Exception ex, string title)
        {
            string logPath = WriteErrorLog(ex);
            string message =
                "برنامه با خطا مواجه شد و متوقف گردید." + Environment.NewLine + Environment.NewLine +
                ex.Message + Environment.NewLine + Environment.NewLine +
                "جزئیات خطا در این فایل ذخیره شد:" + Environment.NewLine +
                logPath;

            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static string WriteErrorLog(Exception ex)
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
                string logFile = Path.Combine(LogDirectory, $"error-{DateTime.Now:yyyyMMdd-HHmmss}.log");
                string content =
                    $"Time: {DateTime.Now:O}{Environment.NewLine}" +
                    $"Machine: {Environment.MachineName}{Environment.NewLine}" +
                    $"OS: {Environment.OSVersion}{Environment.NewLine}" +
                    $"Process: {Process.GetCurrentProcess().ProcessName}{Environment.NewLine}{Environment.NewLine}" +
                    ex;
                File.WriteAllText(logFile, content);
                return logFile;
            }
            catch
            {
                return "Could not write log file.";
            }
        }
    }
}
