using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using GalaSoft.MvvmLight.Messaging;

namespace MIniImageDownloader.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowsManager.Instance.MainWindow = this;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            App.SetHandle(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
            Hide();
        }
    }
}
