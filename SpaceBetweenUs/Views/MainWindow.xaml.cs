using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        private BackgroundWorker worker;

        public MLYoloModel mlModel;
        public CVFrameSource cvSource;
        public MLYolo ml;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            cvSource = new CVFrameSource(@"..\..\..\..\Additionals\SampleVideos\test1.mp4");
            mlModel = MLYoloModel.YoloV3;
            ml = new MLYolo(mlModel, false);

            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            if (mlModel.IsReady)
            {
                ml.Start();
                cvSource.Start();
                worker.RunWorkerAsync();
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
                        mlModel.DownloadDatasets(args =>
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

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.Dispose();
            worker = null;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!(e.UserState is Mat mat)) return;
            image.Source = mat.ToWriteableBitmap(PixelFormats.Bgr24);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var mat = new Mat())
            {
                while (worker != null && !worker.CancellationPending)
                {
                    cvSource.ReadFrame(mat);

                    if (!mat.Empty())
                    {
                        var items = ml.Detect(mat.ToBytes()).ToList();
                        var s = items.Count;
                    }

                    worker.ReportProgress(0, mat);
                }
            }
        }
    }
}
