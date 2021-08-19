using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AsyncImageLoading
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new ViewModel();
            this.DataContext = viewModel;
            viewModel.GetImage();
        }

        private void img_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }


    //inspired by https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/march/async-programming-patterns-for-asynchronous-mvvm-applications-data-binding

    public class ViewModel
    {
        public ViewModel()
        {
            this.ImageSource = new NotifyTaskCompletion<BitmapImage>(this.GetImage());
        }

        public NotifyTaskCompletion<BitmapImage> ImageSource { get; private set; }

        public async Task<BitmapImage> GetImage()
        {
            var imgUrl = new Uri("https://miro.medium.com/max/1400/1*mk1-6aYaf_Bes1E3Imhc0A.jpeg");
            var bytes = await (new WebClient()).DownloadDataTaskAsync(imgUrl);
            var bitmapImage = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad };
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(bytes);
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

}
}
