using System;
using System.ComponentModel;
using System.Windows;
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
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage messsage)
        {
            if (messsage.Notification == GetType().Name)
            Show();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
            Hide();
        }
    }
}
