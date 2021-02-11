using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class FrameSource
    {
        private VideoCapture capture;

        public double Width => capture.FrameWidth;
        public double Height => capture.FrameHeight;

        private FrameSource() { }

        public static async Task<FrameSource> Initialize(Session session)
        {
            return await Task.Run(delegate
            {
                var frameSource = new FrameSource
                {
                    capture = new VideoCapture(session.Source),
                };

                return frameSource;
            });
        }

        public void ReadFrame(Mat mat)
        {
            capture.Grab();
            capture.Grab();
            capture.Grab();
            capture.Grab();
            capture.Read(mat);
            if (mat.Empty())
            {
                capture.Set(VideoCaptureProperties.PosAviRatio, 0);
                capture.Read(mat);
            }
        }
    }
}
