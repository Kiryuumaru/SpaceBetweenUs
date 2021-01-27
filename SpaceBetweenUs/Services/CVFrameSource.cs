using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class CVFrameSource
    {
        private readonly VideoCapture capture;

        public CVFrameSource(string file)
        {
            capture = new VideoCapture(file);
        }

        public void Release()
        {
            capture.Release();
            capture.Dispose();
        }

        public Mat GetFrame()
        {
            var mat = new Mat();
            capture.Read(mat);
            return mat;
        }
    }
}
