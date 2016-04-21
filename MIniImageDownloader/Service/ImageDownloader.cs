using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;

namespace MIniImageDownloader.Service
{
    class ImageDownloader : IImageService
    {
        public const string ImageDownloadStartNotification = "ImageDownloadStart";
        public const string ImageDownloadCompleteNotification = "ImageDownloadComplete";
        private readonly IClipboardService _clipboardService;

        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cancellationSource;
        private Task<HttpResponseMessage> _responseTask;

        public BitmapImage ImageResult { get; private set; }

        public ImageDownloader(IClipboardService clipboardService)
        {
            _clipboardService = clipboardService;

            var progressHandler = new ProgressMessageHandler();
            progressHandler.HttpReceiveProgress += (sender, args) =>
            {
                OnReceiveProgress(args);
            };
            _httpClient = HttpClientFactory.Create(progressHandler);
            _cancellationSource = new CancellationTokenSource();
        }

        public void DownloadImage()
        {
            var clipboardText = _clipboardService.GetText();
            try
            {
                if (!IsImageUrl(clipboardText)) throw new Exception("");

                GetImageStart(clipboardText);

                var message = new NotificationMessage(WindowsManager.NotificationOpenWindow);
                Messenger.Default.Send(message);
            }
            catch (Exception)
            {
                //TODO: get system notification
            }
        }

        public void Cleanup()
        {
            ImageResult = null;
            _cancellationSource.Cancel();
        }

        private void OnReceiveProgress(ProgressChangedEventArgs args)
        {
            var message = new ReceiveProgressNotificationMessage("ImageDownloading", args.ProgressPercentage);
            Messenger.Default.Send(message);
        }

        private async void GetImageStart(string path)
        {
            Messenger.Default.Send(new NotificationMessage(ImageDownloadStartNotification));

            if (_responseTask != null && 
                !_responseTask.IsCompleted)
            {
                _cancellationSource.Cancel();
            }

            HttpResponseMessage response;
            try
            {
                _cancellationSource = new CancellationTokenSource();
                _responseTask = _httpClient.GetAsync(new Uri(path, UriKind.RelativeOrAbsolute), _cancellationSource.Token);
                response = await _responseTask;
            }
            catch (TaskCanceledException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            using (var imageStream = await response.Content.ReadAsStreamAsync())
            {
                ImageResult = new BitmapImage();
                ImageResult.BeginInit();
                ImageResult.CacheOption = BitmapCacheOption.OnLoad;
                ImageResult.StreamSource = imageStream;
                ImageResult.EndInit();
            }
            Messenger.Default.Send(new NotificationMessage(ImageDownloadCompleteNotification));
        }

        private static bool IsImageUrl(string text)
        {
            var urlImageValidator = new Regex(@"(https?:)?//?[^\'""<>]+?\.(jpg|jpeg|gif|png)");
            return urlImageValidator.IsMatch(text);
        }
    }

    internal class ReceiveProgressNotificationMessage : NotificationMessage
    {
        public int ProgressPercentage { get; private set; }

        public ReceiveProgressNotificationMessage(string notification, int progressPercentage) 
            : base(notification)
        {
            ProgressPercentage = progressPercentage;
        }
    }
}
