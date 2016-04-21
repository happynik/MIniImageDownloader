using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MIniImageDownloader.View;

namespace MIniImageDownloader
{
    public class WindowsManager
    {
        #region Instance
        private static readonly object Locker = new object();
        private static volatile WindowsManager _instance;
        public static WindowsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new WindowsManager();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        private MainWindow _mainWindow;

        private MainWindow MainWindow => _mainWindow ?? (_mainWindow = new MainWindow());

        public const string NotificationOpenWindow = "OpenWindow";
        
        private WindowsManager()
        {
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage message)
        {
            if (message.Notification == NotificationOpenWindow)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => MainWindow.Show()));
            }
        }
    }
}