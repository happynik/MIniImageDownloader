using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Navigation;
using System;
using MIniImageDownloader.ViewModel;

namespace MIniImageDownloader
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        HotkeyHelper _hotkeyHelper;
        TaskbarIcon _taskBarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var locator = (ViewModelLocator)Current.Resources["Locator"];

            _taskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
            _taskBarIcon.DataContext = locator.TaskBar;

            _hotkeyHelper.Register(Current.MainWindow = new MainWindow());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _hotkeyHelper.Unregister();
            base.OnExit(e);
        }
    }
}
