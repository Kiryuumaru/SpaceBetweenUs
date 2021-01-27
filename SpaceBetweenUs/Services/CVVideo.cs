using Alturos.Yolo;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    #region EventArgs

    public class OnFrameEventArgs : EventArgs
    {
        public Mat Mat { get; private set; }
        public OnFrameEventArgs(Mat mat)
        {
            Mat = mat;
        }
    }

    #endregion

    public class CVVideo
    {
        #region Properties

        public CVFrameSource source;
        public MLYolo yolo;
        public event EventHandler<OnFrameEventArgs> OnFrame;

        public bool IsRunning { get; private set; } = false;

        #endregion

        #region Initializers

        public CVVideo(CVFrameSource source, MLYolo yolo)
        {
            this.source = source;
            this.yolo = yolo;
        }

        public void Release()
        {
            source.Release();
        }

        #endregion

        #region Methods

        public async void Start()
        {
            await Task.Run(delegate
            {
                IsRunning = true;
                var configurationDetector = new ConfigurationDetector();
                var config = configurationDetector.Detect();
                using (var yoloWrapper = new YoloWrapper(config))
                {
                    //var items = yoloWrapper.Detect(@"image.jpg");
                    //items[0].Type -> "Person, Car, ..."
                    //items[0].Confidence -> 0.0 (low) -> 1.0 (high)
                    //items[0].X -> bounding box
                    //items[0].Y -> bounding box
                    //items[0].Width -> bounding box
                    //items[0].Height -> bounding box

                    while (true)
                    {
                        using (Mat mat = source.GetFrame())
                        {
                            if (mat.Empty() || IsRunning) break;
                            var items = yoloWrapper.Detect(mat.ToBytes());
                            items.ToList();
                            var args = new OnFrameEventArgs(mat);
                            OnFrame?.Invoke(this, args);
                        }
                    }
                }

                IsRunning = false;
            });
        }

        public void Stop()
        {

        }

        #endregion
    }
}
