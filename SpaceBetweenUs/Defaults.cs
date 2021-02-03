using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs
{
    public static class Defaults
    {
        public static readonly string YoloNames = Path.Combine("Assets", "coco.names");
        public static readonly string YoloConfig = Path.Combine("Assets", "yolov3.cfg");
        public static readonly string YoloWeights = Path.Combine("Assets", "yolov3.weights");

        public const double MaxNormWidth = 1000;
        public const double MaxNormHeight = 1000;
        public const double GridEdgeOffset = 10;
        public const double Fps = 40;
        public const double AnchorDotRadius = 7;
        public const double InnerDotRadius = 6;
        public const double ItemDotRadius = 3.5;
        public const double BorderLineThickness = 2;
        public const double GridlineThickness = 1;
        public const double ItemLineThickness = 1.5;

        public const double ConfidenceThreshold = 0.3;
        public const double NonMaximaSupressionThreshold = 0.3;

        public const double GridNotchDistance = 1; // meters

        public static Scalar RedColor = new Scalar(0, 0, 255);
        public static Scalar GreenColor = new Scalar(0, 255, 0);
        public static Scalar BlueColor = new Scalar(255, 0, 0);
        public static Scalar PurpleColor = new Scalar(128, 0, 128);
        public static Scalar YellowColor = new Scalar(0, 238, 238);
    }
}
