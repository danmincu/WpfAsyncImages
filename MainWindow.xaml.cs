using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
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

    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get { return this._imageSource; }
            set
            {
                this._imageSource = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                }
            }
        }

        public async Task<BitmapImage> GetImage()
        {
            var imgUrl = new Uri("https://miro.medium.com/max/1400/1*mk1-6aYaf_Bes1E3Imhc0A.jpeg");
            var imageData = await (new WebClient()).DownloadDataTaskAsync(imgUrl);

            // or you can download it Async won't block your UI
            // var imageData = await new WebClient().DownloadDataTaskAsync(imgUrl);

            var bitmapImage = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad };
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageData);
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            this.ImageSource = bitmapImage;
           

            return bitmapImage;
        }

    }
}
