using System.Drawing;
using System.Windows.Media.Imaging;

namespace MIniImageDownloader.ViewModel
{
    public interface IImageService
    {
        BitmapImage ImageResult { get; }

        void StartGetImage(string path);
    }
}