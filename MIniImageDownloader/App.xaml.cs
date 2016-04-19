﻿using Hardcodet.Wpf.TaskbarNotification;
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
        TaskbarIcon _taskBarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var locator = (ViewModelLocator)Current.Resources["Locator"];

            _taskBarIcon = (TaskbarIcon)FindResource("TaskBarIcon");
            _taskBarIcon.DataContext = locator.TaskBar;
        }

        protected override void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            base.OnFragmentNavigation(e);
        }

        protected override void OnNavigated(NavigationEventArgs e)
        {
            base.OnNavigated(e);

            //Current.MainWindow.Hide();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
