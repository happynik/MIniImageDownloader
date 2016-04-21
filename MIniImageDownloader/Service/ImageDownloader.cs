using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;

namespace MIniImageDownloader.Service
{
    class ImageDownloader : IImageService
    {
        private readonly IClipboardService _clipboardService;

        public BitmapImage ImageResult { get; private set; }

        public ImageDownloader(IClipboardService clipboardService)
        {
            _clipboardService = clipboardService;
        }

        public void DownloadImage()
        {
            //var path = "http://disgustingmen.com/wp-content/uploads/2015/12/3-theories-about-star-wars-episode-7-the-force-awakens-you-don-t-know-the-power-of-the-664771.jpg";
            var clipboardText = _clipboardService.GetText();
            if (IsImageUrl(clipboardText))
            {
                GetImageStart(clipboardText);

                var message = new NotificationMessage(WindowsManager.NotificationOpenWindow);
                Messenger.Default.Send(message);
            }
            else
            {
                //TODO: get system notification
            }
        }

        private async void GetImageStart(string path)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(new Uri(path, UriKind.RelativeOrAbsolute));
            using (var imageStream = await result.Content.ReadAsStreamAsync())
            {
                ImageResult = new BitmapImage();
                ImageResult.BeginInit();
                ImageResult.CacheOption = BitmapCacheOption.OnLoad;
                //_imageResult.DecodePixelWidth = 30;
                ImageResult.StreamSource = imageStream;
                ImageResult.EndInit();
            }
            Messenger.Default.Send(new NotificationMessage("ImageComplete"));
        }

        private static bool IsImageUrl(string text)
        {
            var urlImageValidator = new Regex(@"(https?:)?//?[^\'""<>]+?\.(jpg|jpeg|gif|png)");
            return urlImageValidator.IsMatch(text);
        }
    }
}
