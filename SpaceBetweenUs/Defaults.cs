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
        public const double GridEdgeOffset = 10;
        public const double Fps = 40;
        public const int AnchorDotRadius = 7;
        public const int BorderLineThickness = 2;
        public const int GridlineThickness = 1;
        public const double GridlineDistance = 0.5; // meters
        public const int ItemLineThickness = 2;

        public const double ConfidenceThreshold = 0.3;
        public const double NonMaximaSupressionThreshold = 0.3;

        public static Scalar RedColor = new Scalar(0, 0, 255);
        public static Scalar GreenColor = new Scalar(0, 255, 0);
        public static Scalar BlueColor = new Scalar(255, 0, 0);
        public static Scalar PurpleColor = new Scalar(128, 0, 128);
        public static Scalar YellowColor = new Scalar(0, 238, 238);
    }
}
