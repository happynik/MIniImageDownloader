using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
using MIniImageDownloader.View;

namespace MIniImageDownloader
{
    public class ViewsManager
    {
        #region Instance
        private static readonly object Locker = new object();
        private static volatile ViewsManager _instance;
        public static ViewsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ViewsManager();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        public const string NotificationOpenWindow = "OpenWindow";
        public const string NotificationOpenBallon = "OpenBallon";

        private MainWindow _mainWindow;
        public MainWindow MainWindow
        {
            get{ return _mainWindow ?? (_mainWindow = new MainWindow()); }
            set { _mainWindow = value; }
        }

        public TaskbarIcon TaskBarIcon { get; set; }

        private ViewsManager()
        {
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Messenger.Default.Register<BallonNotificationMessage>(this, BallonNotificationMessageReceived);
        }

        private void BallonNotificationMessageReceived(BallonNotificationMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => TaskBarIcon.ShowBalloonTip(message.Title, message.Notification, BalloonIcon.Warning)));
        }

        private void NotificationMessageReceived(NotificationMessage message)
        {
            switch (message.Notification)
            {
                case NotificationOpenWindow:
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => MainWindow.Show()));
                    break;
                case NotificationOpenBallon:
                    break;
            }
        }

        public void Init()
        {
            _mainWindow = new MainWindow();
        }
    }

    internal class BallonNotificationMessage : NotificationMessage
    {
        public string Title { get; set; }

        public BallonNotificationMessage(string notification) : base(notification)
        {
        }

        public BallonNotificationMessage(object sender, string notification) : base(sender, notification)
        {
        }

        public BallonNotificationMessage(object sender, object target, string notification) : base(sender, target, notification)
        {
        }
    }
}