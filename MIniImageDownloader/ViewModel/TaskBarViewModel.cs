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
        private readonly IClipboardService _clipboardService;

        public TaskBarViewModel(IImageService imageService, IClipboardService clipboardService)
        {
            _imageService = imageService;
            _clipboardService = clipboardService;
        }

        public ICommand ExitApplicationCommand => new RelayCommand(() => Application.Current.Shutdown());

        public ICommand DownloadImageCommand => new RelayCommand(DownloadImage);

        private void DownloadImage()
        {
            //var path = "http://disgustingmen.com/wp-content/uploads/2015/12/3-theories-about-star-wars-episode-7-the-force-awakens-you-don-t-know-the-power-of-the-664771.jpg";
            var clipboardText = _clipboardService.GetText();
            if (IsImageUrl(clipboardText))
            {
                _imageService.GetImageStart(clipboardText);

                var message = new NotificationMessage(WindowsManager.NotificationOpenWindow);
                Messenger.Default.Send(message);
            }
            else
            {
                //TODO: get system notification
            }
        }

        private static bool IsImageUrl(string text)
        {
            var urlImageValidator = new Regex(@"(https?:)?//?[^\'""<>]+?\.(jpg|jpeg|gif|png)");
            return urlImageValidator.IsMatch(text);
        }
    }
}
