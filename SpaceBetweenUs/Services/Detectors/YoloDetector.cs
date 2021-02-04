using Alturos.Yolo;
using Alturos.Yolo.Model;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services.Detectors
{
    public class YoloDetector : YoloWrapper, IHumanDetector
    {
        public double FrameWidth { get; private set; }
        public double FrameHeight { get; private set; }
        public bool GPUMode { get; private set; }

        public YoloDetector(
            double frameWidth,
            double frameHeight,
            string configurationFilename,
            string weightsFilename,
            string namesFilename,
            GpuConfig gpuConfig = null,
            IYoloSystemValidator yoloSystemValidator = null)
            : base(configurationFilename, weightsFilename, namesFilename, gpuConfig, yoloSystemValidator)
        {
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            GPUMode = gpuConfig != null;
        }

        public IEnumerable<Human> DetectHuman(byte[] image)
        {
            var items = Detect(image).Where(i => i.Confidence > Defaults.ConfidenceThreshold && i.Type.Equals("person")).ToArray();
            CvDnn.NMSBoxes(
                items.Select(i => new Rect(i.X, i.Y, i.Width, i.Height)),
                items.Select(i => (float)i.Confidence),
                (float)Defaults.ConfidenceThreshold,
                (float)Defaults.NonMaximaSupressionThreshold,
                out int[] indices);
            var humans = new List<Human>();
            foreach (var i in indices)
            {
                var human = new Human(items[i].X, items[i].Y, items[i].Width, items[i].Height, FrameWidth, FrameHeight, false);
                try
                {
                    if (GeometryHelpers.IsInside(
                        human.BottomCenter,
                        Session.GridProjection.BL,
                        Session.GridProjection.TL,
                        Session.GridProjection.TR,
                        Session.GridProjection.BR))
                    {
                        humans.Add(human);
                    }
                }
                catch
                {
                    humans.Add(human);
                }
            }
            foreach (var i in humans)
            {
                foreach (var j in humans)
                {
                    if (i == j) continue;
                    double dis = GeometryHelpers.GetDistance(i.BottomCenter, j.BottomCenter);
                    if (dis <= 100)
                    {
                        i.IsViolation = true;
                        j.IsViolation = true;
                    }
                }
            }
            return humans;
        }
    }
}
