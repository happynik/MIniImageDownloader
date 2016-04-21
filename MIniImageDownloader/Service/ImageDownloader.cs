using System;
using System.Net.Http;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;

namespace MIniImageDownloader.Service
{
    class ImageDownloader : IImageService
    {
        public BitmapImage ImageResult { get; private set; }

        public ImageDownloader()
        {
            
        }

        public async void GetImageStart(string path)
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
    }
}
