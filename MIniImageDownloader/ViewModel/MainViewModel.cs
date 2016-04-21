using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media.Imaging;
using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MIniImageDownloader.Service;

namespace MIniImageDownloader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IImageService _imageService;

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (Equals(_imageSource, value)) return;
                _imageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                if (_progress.Equals(value)) return;
                _progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        private bool _progressVisibility;
        public bool ProgressVisibility
        {
            get { return _progressVisibility; }
            set
            {
                if (_progressVisibility == value) return;
                _progressVisibility = value;
                RaisePropertyChanged(() => ProgressVisibility);
            }
        }

        public ICommand ClosingCommand => new RelayCommand(ClosingAction);
        private void ClosingAction()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => ImageSource = null));
            _imageService.Cleanup();
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IImageService imageService)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Progress = 50;
            }
            else
            {
                // Code runs "for real"
            }

            _imageService = imageService;

            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Messenger.Default.Register<ReceiveProgressNotificationMessage>(this, ReceiveProgressNotificationMessageRecieved);
        }

        private void ReceiveProgressNotificationMessageRecieved(ReceiveProgressNotificationMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Progress = message.ProgressPercentage));
        }

        private async void NotificationMessageReceived(NotificationMessage message)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    switch (message.Notification)
                    {
                        case ImageDownloader.ImageDownloadCompleteNotification:
                            ImageSource = _imageService.ImageResult;
                            ProgressVisibility = false;
                            break;
                        case ImageDownloader.ImageDownloadStartNotification:
                            ImageSource = null;
                            ProgressVisibility = true;
                            break;
                    }
                }));
        }
    }
}