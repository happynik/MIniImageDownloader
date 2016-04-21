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
        private WindowsManager _windowsManager;

        //readonly HotkeyHelper _hotkeyHelper;
        private TaskbarIcon _taskBarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _windowsManager = WindowsManager.Instance;

            var locator = (ViewModelLocator)Current.Resources["Locator"];

            _taskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
            if (_taskBarIcon != null) _taskBarIcon.DataContext = locator.TaskBar;

            //_hotkeyHelper.Register(Current.MainWindow = new MainWindow());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //_hotkeyHelper.Unregister();
            base.OnExit(e);
        }
    }
}
