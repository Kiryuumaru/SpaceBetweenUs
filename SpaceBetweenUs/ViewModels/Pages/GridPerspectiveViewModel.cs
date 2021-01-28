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
            set => SetProperty(ref frame, value);
        }

        private int blxAxis;
        public int BLXAxis
        {
            get => blxAxis;
            set => SetProperty(ref blxAxis, value);
        }

        private int blyAxis;
        public int BLYAxis
        {
            get => blyAxis;
            set => SetProperty(ref blyAxis, value);
        }

        private double bldepth;
        public double BLDepth
        {
            get => bldepth;
            set => SetProperty(ref bldepth, value);
        }

        private int tlxAxis;
        public int TLXAxis
        {
            get => tlxAxis;
            set => SetProperty(ref tlxAxis, value);
        }

        private int tlyAxis;
        public int TLYAxis
        {
            get => tlyAxis;
            set => SetProperty(ref tlyAxis, value);
        }

        private double tldepth;
        public double TLDepth
        {
            get => tldepth;
            set => SetProperty(ref tldepth, value);
        }

        private int trxAxis;
        public int TRXAxis
        {
            get => trxAxis;
            set => SetProperty(ref trxAxis, value);
        }

        private int tryAxis;
        public int TRYAxis
        {
            get => tryAxis;
            set => SetProperty(ref tryAxis, value);
        }

        private double trdepth;
        public double TRDepth
        {
            get => trdepth;
            set => SetProperty(ref trdepth, value);
        }

        private int brxAxis;
        public int BRXAxis
        {
            get => brxAxis;
            set => SetProperty(ref brxAxis, value);
        }

        private int bryAxis;
        public int BRYAxis
        {
            get => bryAxis;
            set => SetProperty(ref bryAxis, value);
        }

        private double brdepth;
        public double BRDepth
        {
            get => brdepth;
            set => SetProperty(ref brdepth, value);
        }

        private bool blRef;
        public bool BLRef
        {
            get => blRef;
            set
            {
                SetProperty(ref blRef, value);
            }
        }

        private bool tlRef;
        public bool TLRef
        {
            get => tlRef;
            set
            {
                SetProperty(ref tlRef, value);
            }
        }

        private bool trRef;
        public bool TRRef
        {
            get => trRef;
            set
            {
                SetProperty(ref trRef, value);
            }
        }

        private bool brRef;
        public bool BRRef
        {
            get => brRef;
            set => SetProperty(ref brRef, value);
        }

        public GridPerspectiveViewModel()
        {
            Start();
        }

        private async void Start()
        {
            var s = new Stopwatch();
            using var mat = new Mat();
            while (true)
            {
                s.Restart();

                Session.FrameSource.ReadFrame(mat);
                //Cv2.Resize(mat, mat, sss);
                Frame = mat.ToWriteableBitmap(PixelFormats.Bgr24);

                int delayMillis = (int)((1000 / Fps) - s.ElapsedMilliseconds);
                await Task.Delay(delayMillis > 0 ? delayMillis : 0);
            }
        }

        private void SetAnchor(Anchor anchor, int x, int y, int depth)
        {

        }
    }
}
