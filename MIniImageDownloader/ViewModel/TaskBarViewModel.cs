using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MIniImageDownloader.ViewModel
{
    public class TaskBarViewModel : ViewModelBase
    {
        private IImageService _imageService;

        public TaskBarViewModel(IImageService imageService)
        {
            _imageService = imageService;
        }

        public ICommand ExitApplicationCommand
        {
            get { return new RelayCommand(() => Application.Current.Shutdown()); }
        }

        public ICommand DownloadImageCommand
        {
            get { return new RelayCommand(DownloadImage); }
        }

        private void DownloadImage()
        {
            var path = "http://disgustingmen.com/wp-content/uploads/2015/12/3-theories-about-star-wars-episode-7-the-force-awakens-you-don-t-know-the-power-of-the-664771.jpg";
            _imageService.StartGetImage(path);

            Messenger.Default.Send(new NotificationMessage(typeof(MainWindow).Name));
        }
    }
}
