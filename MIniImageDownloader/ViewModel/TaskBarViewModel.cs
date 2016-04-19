using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MIniImageDownloader.ViewModel
{
    public class TaskBarViewModel : ViewModelBase
    {
        private readonly IImageService _imageService;
        private readonly IClipboardService _clipboardService;

        public TaskBarViewModel(IImageService imageService, IClipboardService clipboardService)
        {
            _imageService = imageService;
            _clipboardService = clipboardService;
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
            //var path = "http://disgustingmen.com/wp-content/uploads/2015/12/3-theories-about-star-wars-episode-7-the-force-awakens-you-don-t-know-the-power-of-the-664771.jpg";
            var clipboardText = _clipboardService.GetText();
            if (IsImageUrl(clipboardText))
            {
                _imageService.StartGetImage(clipboardText);
                Messenger.Default.Send(new NotificationMessage(typeof(MainWindow).Name));
            }
            else
            {
                //TODO: get system notification
            }
        }

        private bool IsImageUrl(string text)
        {
            var urlImageValidator = new Regex(@"(https?:)?//?[^\'""<>]+?\.(jpg|jpeg|gif|png)");
            return urlImageValidator.IsMatch(text);
        }
    }
}
