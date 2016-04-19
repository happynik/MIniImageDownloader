using GalaSoft.MvvmLight.Messaging;
using MIniImageDownloader.ViewModel;
using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Windows.Media.Imaging;

namespace MIniImageDownloader.Model
{
    class ImageDownloader : IImageService
    {
        private BitmapImage _imageResult;
        public BitmapImage ImageResult
        {
            get { return _imageResult; }
        }

        public ImageDownloader()
        {
            
        }

        public async void StartGetImage(string path)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(new Uri(path, UriKind.RelativeOrAbsolute));
            using (var imageStream = await result.Content.ReadAsStreamAsync())
            {
                _imageResult = new BitmapImage();
                _imageResult.BeginInit();
                _imageResult.CacheOption = BitmapCacheOption.OnLoad;
                //_imageResult.DecodePixelWidth = 30;
                _imageResult.StreamSource = imageStream;
                _imageResult.EndInit();
            }
            Messenger.Default.Send(new NotificationMessage("ImageComplete"));
        }
    }
}
