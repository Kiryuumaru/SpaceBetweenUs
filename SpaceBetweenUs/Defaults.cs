using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs
{
    public static class Defaults
    {
        public const double MaxNormWidth = 1000;
        public const double MaxNormHeight = 1000;
        public const double Fps = 40;
        public const int DotRadius = 5;
        public const int LineThickness = 2;
        public const double GridDistance = 0.5; // meters

        public static Scalar RedColor = new Scalar(0, 0, 255);
        public static Scalar GreenColor = new Scalar(0, 255, 0);
        public static Scalar BlueColor = new Scalar(255, 0, 0);
        public static Scalar PurpleColor = new Scalar(128, 0, 128);
        public static Scalar YellowColor = new Scalar(0, 238, 238);
    }
}
