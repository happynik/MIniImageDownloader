using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MIniImageDownloader.Service;

namespace MIniImageDownloader.ViewModel
{
    public class TaskBarViewModel : ViewModelBase
    {
        private readonly IImageService _imageService;

        public TaskBarViewModel(IImageService imageService)
        {
            _imageService = imageService;
        }

        public ICommand ExitApplicationCommand => new RelayCommand(() => Application.Current.Shutdown());

        public ICommand DownloadImageCommand => new RelayCommand(_imageService.DownloadImage);
    }
}
