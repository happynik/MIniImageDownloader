using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Navigation;
using System;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using MIniImageDownloader.Service;
using MIniImageDownloader.Utils;
using MIniImageDownloader.ViewModel;
using Application = System.Windows.Application;

namespace MIniImageDownloader
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Hotkey _hotkey;
        private TaskbarIcon _taskBarIcon;

        public static void SetHandle(Window window)
        {
            _hotkey = new Hotkey
            {
                KeyCode = Keys.D8,
                Control = true,
                Alt = true,
                Shift = true
            };
            _hotkey.Pressed += delegate { ServiceLocator.Current.GetInstance<IImageService>().DownloadImage(); };

            if (!_hotkey.GetCanRegister(window))
            { Console.WriteLine("Whoops, looks like attempts to register will fail or throw an exception, show an error/visual user feedback"); }
            else
            { _hotkey.Register(window); }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var locator = (ViewModelLocator)Current.Resources["Locator"];

            _taskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
            if (_taskBarIcon != null) _taskBarIcon.DataContext = locator.TaskBar;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_hotkey.Registered)
            {
                _hotkey.Unregister();
            }
            base.OnExit(e);
        }
    }
}
