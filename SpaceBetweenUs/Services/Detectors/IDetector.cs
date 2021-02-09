using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services.Detectors
{
    public interface IDetector
    {
        IEnumerable<(string Label, float Confidence, double CenterX, double CenterY, double Width, double Height)> Detect(Mat image);
    }
}
