using System;
using System.ComponentModel;
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
        private ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.viewModel = new ViewModel("https://miro.medium.com/max/1400/1*mk1-6aYaf_Bes1E3Imhc0A.jpeg");
            this.DataContext = viewModel;
        }

        private void img_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private string[] images = new string[] { "https://miro.medium.com/max/1400/1*mk1-6aYaf_Bes1E3Imhc0A.jpeg1",
            "https://static.wikia.nocookie.net/starwars/images/d/d6/Yoda_SWSB.png/revision/latest?cb=20150206140125",
            "https://miro.medium.com/max/1400/1*mk1-6aYaf_Bes1E3Imhc0A.jpeg","https://miro.medium.com/max/1400/1*mk1-6aYaf_Bes1E3Imhc0A.jpeg1",
            "https://storage.googleapis.com/spikeybits-staging-bucket/2021/03/fcfb56f3-yoda-fight.jpg",
            "https://storage.googleapis.com/spikeybits-staging-bucket/2021/03/fcfb56f3-yoda-fight.jpg1"};


        private int i;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            this.viewModel.ImageUrl = images[i++ % images.Length];
        }
    }


    //inspired by https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/march/async-programming-patterns-for-asynchronous-mvvm-applications-data-binding

    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel(string imgUrl)
        {
            this.imgUrl = imgUrl;
            this.ImageSource = new NotifyTaskCompletion<BitmapImage>(this.GetImage());
        }

        private NotifyTaskCompletion<BitmapImage> imageSource;
        public NotifyTaskCompletion<BitmapImage> ImageSource
        {
            get
            {
                return this.imageSource;
            }
            private set
            {
                this.imageSource = value;
                this.OnPropertyChange(nameof(ImageSource));
            }
        }


        private string imgUrl;

        public string ImageUrl
        {
            get
            {
                return this.imgUrl;
            }
            set
            {
                this.imgUrl = value;
                this.ImageSource = new NotifyTaskCompletion<BitmapImage>(this.GetImage());
                //this.OnPropertyChange(nameof(ImageSource));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //by returning the Task instead of an async call we are actually preserving the ability to change the UI based on task status (e.g. "IsFaulted" )
        public Task<BitmapImage> GetImage()
        {
            var imgUrl = new Uri(this.ImageUrl);
            return Task.Run(() => (new WebClient()).DownloadDataTaskAsync(imgUrl)).ContinueWith(task =>
                {
                    var bitmapImage = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad };
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(task.Result);
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
            );
        }

    }
}
