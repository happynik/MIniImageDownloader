﻿using System.Windows.Media.Imaging;

namespace MIniImageDownloader.Service
{
    public interface IImageService
    {
        BitmapImage ImageResult { get; }

        void DownloadImage();
        void Cleanup();
    }
}