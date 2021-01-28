using MvvmHelpers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class GridPerspectiveViewModel : BaseViewModel
    {
        private const double Fps = 30;

        private ImageSource frame;
        public ImageSource Frame
        {
            get => frame;
            set
            {
                SetProperty(ref frame, value);
            }
        }

        public GridPerspectiveViewModel()
        {
            Start();
        }

        private async void Start()
        {
            var s = new Stopwatch();
            var ss = new Stopwatch();
            using var mat = new Mat();
            while (true)
            {
                s.Restart();
                ss.Restart();

                Session.FrameSource.ReadFrame(mat);



                Frame = mat.ToWriteableBitmap(PixelFormats.Bgr24);

                int delayMillis = (int)((1000 / Fps) - s.ElapsedMilliseconds);
                await Task.Delay(delayMillis > 0 ? delayMillis : 0);
                Console.WriteLine(ss.ElapsedMilliseconds);
            }
        }
    }
}
