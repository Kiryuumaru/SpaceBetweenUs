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

        private readonly string[] labels;
        private readonly Net net;
        private readonly Size configSize;
        private readonly Scalar blobFromImageMeanParams;
        private readonly double scalarFactor;
        private readonly IEnumerable<string> outputNames;
        private readonly IEnumerable<Mat> outputLayers;

        public YoloV3Detector()
        {
            labels = File.ReadAllLines(YoloNames).ToArray();
            net = CvDnn.ReadNetFromDarknet(YoloConfig, YoloWeights);
            net.SetPreferableBackend(Backend.CUDA);
            net.SetPreferableTarget(Target.CUDA);
            outputNames = net.GetUnconnectedOutLayersNames();
            outputLayers = outputNames.Select(_ => new Mat()).ToArray();
            string[] configs = File.ReadAllLines(YoloConfig).ToArray();
            int configWidth = configs.Where(x => x.StartsWith("width="))
                .Select(x => int.Parse(x.Split('=')[1]))
                .FirstOrDefault();
            int configHeight = configs.Where(x => x.StartsWith("height="))
                .Select(x => int.Parse(x.Split('=')[1]))
                .FirstOrDefault();
            configSize = new Size(configWidth, configHeight);
            blobFromImageMeanParams = new Scalar();
            scalarFactor = 1.0 / 255; 
        }

        public IEnumerable<(float Confidence, double CenterX, double CenterY, double Width, double Height)> Detect(Mat image)
        {
            Mat blob = CvDnn.BlobFromImage(image, scalarFactor, configSize, blobFromImageMeanParams, true, false);
            net.SetInput(blob);
            net.Forward(outputLayers, outputNames);
            List<(float Confidence, double CenterX, double CenterY, double Width, double Height)> items = new List<(float Confidence, double CenterX, double CenterY, double Width, double Height)>();
            foreach (Mat prob in outputLayers)
            {
                for (int i = 0; i < prob.Rows; i++)
                {
                    float confidence = prob.At<float>(i, 4);
                    if (confidence > Defaults.ConfidenceThreshold)
                    {
                        Cv2.MinMaxLoc(prob.Row(i).ColRange(5, prob.Cols), out _, out Point max);
                        int type = max.X;
                        string label = labels[type];
                        float probability = prob.At<float>(i, type + 5);
                        if (probability > Defaults.ConfidenceThreshold && label.Equals("person"))
                        {
                            float centerX = prob.At<float>(i, 0) * image.Width;
                            float centerY = prob.At<float>(i, 1) * image.Height;
                            float width = prob.At<float>(i, 2) * image.Width;
                            float height = prob.At<float>(i, 3) * image.Height;
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
