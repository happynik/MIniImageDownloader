using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media.Imaging;
using System;
using System.Windows;
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
                if (_imageSource == value) return;
                _imageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IImageService imageService)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            _imageService = imageService;

            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private async void NotificationMessageReceived(NotificationMessage message)
        {
            if (message.Notification == "ImageComplete")
            {
                await Application.Current.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        ImageSource = _imageService.ImageResult;
                    }));
            }
        }
    }
}