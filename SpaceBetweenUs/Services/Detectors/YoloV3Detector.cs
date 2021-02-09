using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services.Detectors
{
    public class YoloV3Detector : IDetector
    {
        public static readonly string YoloNames = Path.Combine("Assets", "coco.names");
        public static readonly string YoloConfig = Path.Combine("Assets", "yolov3.cfg");
        public static readonly string YoloWeights = Path.Combine("Assets", "yolov3.weights");

        public double FrameWidth { get; private set; }
        public double FrameHeight { get; private set; }
        public bool GPUMode { get; private set; }

        private readonly string[] labels;
        private readonly Net net;
        private readonly Size configSize;
        private readonly Scalar blobFromImageMeanParams;
        private readonly double scalarFactor;
        private readonly IEnumerable<string> outputNames;
        private readonly IEnumerable<Mat> outputLayers;

        public YoloV3Detector(double frameWidth, double frameHeight)
        {
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            labels = File.ReadAllLines(YoloNames).ToArray();
            net = CvDnn.ReadNetFromDarknet(YoloConfig, YoloWeights);
            net.SetPreferableBackend(Backend.CUDA);
            net.SetPreferableTarget(Target.CUDA);
            outputNames = net.GetUnconnectedOutLayersNames();
            outputLayers = outputNames.Select(_ => new Mat()).ToArray();
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

        public IEnumerable<(string Label, float Confidence, double CenterX, double CenterY, double Width, double Height)> Detect(Mat image)
        {
            var blob = CvDnn.BlobFromImage(image, scalarFactor, configSize, blobFromImageMeanParams, true, false);
            net.SetInput(blob);
            net.Forward(outputLayers, outputNames);
            var items = new List<(string Label, float Confidence, double CenterX, double CenterY, double Width, double Height)>();
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
                        if (probability > Defaults.ConfidenceThreshold)
                        {
                            var centerX = prob.At<float>(i, 0) * FrameWidth;
                            var centerY = prob.At<float>(i, 1) * FrameHeight;
                            var width = prob.At<float>(i, 2) * FrameWidth;
                            var height = prob.At<float>(i, 3) * FrameHeight;
                            items.Add((label, confidence, centerX, centerY, width, height));
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
