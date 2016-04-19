using MIniImageDownloader.ViewModel;
using System.Windows;

namespace MIniImageDownloader.Service
{
    class ClipboardService : IClipboardService
    {
        public string GetText()
        {
            return Clipboard.GetText();
        }
    }
}
