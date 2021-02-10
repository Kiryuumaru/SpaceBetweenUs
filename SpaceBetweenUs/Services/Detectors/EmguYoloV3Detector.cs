using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Cuda;
using Emgu.CV.Util;

namespace SpaceBetweenUs.Services.Detectors
{
    public class EmguYoloV3Detector : IDetector
    {
        public static readonly string YoloNames = Path.Combine("Assets", "coco.names");
        public static readonly string YoloConfig = Path.Combine("Assets", "yolov3.cfg");
        public static readonly string YoloWeights = Path.Combine("Assets", "yolov3.weights");

        public double FrameWidth { get; private set; }
        public double FrameHeight { get; private set; }

        private readonly string[] labels;
        private readonly Emgu.CV.Dnn.Net net;
        private readonly Size configSize;
        private readonly Scalar blobFromImageMeanParams;
        private readonly double scalarFactor;
        private readonly string[] outputNames;
        private readonly IOutputArrayOfArrays outputLayers;

        public EmguYoloV3Detector(double frameWidth, double frameHeight)
        {
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            labels = File.ReadAllLines(YoloNames).ToArray();
            net = DnnInvoke.ReadNetFromDarknet(YoloConfig, YoloWeights);
            net.SetPreferableBackend(Emgu.CV.Dnn.Backend.Cuda);
            net.SetPreferableTarget(Emgu.CV.Dnn.Target.Cuda);
            outputNames = net.UnconnectedOutLayersNames;
            var outputMatLayers = outputNames.Select(_ => new Emgu.CV.Mat()).ToArray();
            outputLayers = new VectorOfMat(outputMatLayers);
            var configs = File.ReadAllLines(YoloConfig).ToArray();
            var configWidth = configs.Where(x => x.StartsWith("width="))
                .Select(x => int.Parse(x.Split('=')[1]))
                .FirstOrDefault();
            var configHeight = configs.Where(x => x.StartsWith("height="))
                .Select(x => int.Parse(x.Split('=')[1]))
                .FirstOrDefault();
            configSize = new Size(configWidth, configHeight);
            blobFromImageMeanParams = new Scalar();
            scalarFactor = 1.0 / 255; 
        }

        public IEnumerable<(float Confidence, double CenterX, double CenterY, double Width, double Height)> Detect(OpenCvSharp.Mat image)
        {
            image.
            var blob = DnnInvoke.BlobFromImage(image.ToBytes(), scalarFactor, configSize, blobFromImageMeanParams, true, false);
            net.SetInput(blob);
            net.Forward(outputLayers, outputNames);
            var items = new List<(float Confidence, double CenterX, double CenterY, double Width, double Height)>();
            foreach (var prob in outputLayers)
            {
                for (var i = 0; i < prob.Rows; i++)
                {
                    var confidence = prob.At<float>(i, 4);
                    if (confidence > Defaults.ConfidenceThreshold)
                    {
                        Cv2.MinMaxLoc(prob.Row(i).ColRange(5, prob.Cols), out _, out Point max);
                        var type = max.X;
                        var label = labels[type];
                        var probability = prob.At<float>(i, type + 5);
                        if (probability > Defaults.ConfidenceThreshold && label.Equals("person"))
                        {
                            var centerX = prob.At<float>(i, 0) * FrameWidth;
                            var centerY = prob.At<float>(i, 1) * FrameHeight;
                            var width = prob.At<float>(i, 2) * FrameWidth;
                            var height = prob.At<float>(i, 3) * FrameHeight;
                            items.Add((confidence, centerX, centerY, width, height));
                        }
                    }
                }
            }
            //return items;
            CvDnn.NMSBoxes(
                items.Select(i => new Rect((int)(i.CenterX - (i.Width / 2)), (int)(i.CenterY - (i.Height / 2)), (int)i.Width, (int)i.Height)),
                items.Select(i => i.Confidence),
                (float)Defaults.ConfidenceThreshold,
                (float)Defaults.NonMaximaSupressionThreshold,
                out int[] indices);
            for (int i = 0; i < indices.Length; i++)
            {
                yield return items[indices[i]];
            }
        }
    }
}
