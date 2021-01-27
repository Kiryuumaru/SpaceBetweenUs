using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Window = System.Windows.Window;

namespace SpaceBetweenUs.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MLYoloModel mlModel;
        public CVFrameSource cvSource;
        public MLYolo ml;
        public CVVideo cv;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            mlModel = MLYoloModel.YoloV3();
            cvSource = new CVFrameSource(@"..\..\Additionals\SampleVideos\test1.mp4");
            ml = new MLYolo(mlModel);
            cv = new CVVideo(cvSource, ml);

            if (ml.IsDatasetReady)
            {
                cv.OnFrame += Cv_OnFrame;
                cv.Start();
            }
            else
            {
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult rsltMessageBox = MessageBox.Show(
                    "Do you want download model dependencies?",
                    "Model Not Downloaded",
                    btnMessageBox,
                    icnMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        ml.DownloadDatasets(args =>
                        {
                            Console.WriteLine("Progress Download: " + args.File.FileName + " - " + args.OverallCurrentBytes + "/" + args.OverallTotalBytes);
                        });
                        break;

                    case MessageBoxResult.No:

                        break;

                    case MessageBoxResult.Cancel:

                        break;
                }
            }
        }

        private void Cv_OnFrame(object sender, OnFrameEventArgs e)
        {
            var mat = e.Mat;
            var item = ml.Detect(mat.ToBytes());
            image.Source = mat.ToWriteableBitmap(PixelFormats.Bgr24);
        }
    }
}
