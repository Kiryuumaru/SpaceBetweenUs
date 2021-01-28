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
        private Mat matToRead;
        private bool isReadingFrame = false;
        private readonly VideoCapture capture;

        public double Fps => capture.Fps;

        public FrameSource(string file)
        {
            capture = new VideoCapture(file);
        }

        public async void Start()
        {
            // Mock realtime cam simulation
            var s = new Stopwatch();
            using var mat = new Mat();
            while (true)
            {
                s.Restart();
                if (isReadingFrame)
                {
                    capture.Read(matToRead);
                    if (matToRead.Empty())
                    {
                        capture.Set(VideoCaptureProperties.PosAviRatio, 0);
                        capture.Read(matToRead);
                    }
                    isReadingFrame = false;
                }
                else
                {
                    capture.Read(mat);
                    if (mat.Empty())
                    {
                        capture.Set(VideoCaptureProperties.PosAviRatio, 0);
                        capture.Read(mat);
                    }
                }
                int delayMillis = (int)((1000 / capture.Fps) - s.ElapsedMilliseconds);
                await Task.Delay(delayMillis > 0 ? delayMillis : 0);
            }
        }

        public void ReadFrame(Mat mat)
        {
            matToRead = mat;
            isReadingFrame = true;
            while (isReadingFrame) { }
        }
    }
}
