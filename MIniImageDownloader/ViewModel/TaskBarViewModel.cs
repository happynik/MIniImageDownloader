using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MIniImageDownloader.ViewModel
{
    public class TaskBarViewModel : ViewModelBase
    {
        public TaskBarViewModel()
        {

        }

        public ICommand ExitApplicationCommand
        {
            get { return new RelayCommand(() => Application.Current.Shutdown()); }
        }

        public ICommand DownloadImageCommand
        {
            get { return new RelayCommand(DownloadImage); }
        }

        private async void DownloadImage()
        {
            await Task.Factory.StartNew(() =>
            {
                
            });
        }
    }
}
