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

        public (IEnumerable<Violation> Violations, IEnumerable<Human> Humans) DetectHuman(byte[] image)
        {
            var items = Detect(image).Where(i => i.Confidence > Defaults.ConfidenceThreshold && i.Type.Equals("person")).ToArray();
            var violations = new List<Violation>();
            CvDnn.NMSBoxes(
                items.Select(i => new Rect(i.X, i.Y, i.Width, i.Height)),
                items.Select(i => (float)i.Confidence),
                (float)Defaults.ConfidenceThreshold,
                (float)Defaults.NonMaximaSupressionThreshold,
                out int[] indices);
            var humans = new List<Human>();
            foreach (var i in indices)
            {
                var human = new Human(items[i].X, items[i].Y, items[i].Width, items[i].Height, FrameWidth, FrameHeight);
                try
                {
                    var pers = Session.GridProjection.Perspective(human.BottomCenter);
                    if (pers.HasValue)
                    {
                        human.PerspectivePoint = pers.Value;
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
                    double dis = GeometryHelpers.GetDistance(i.PerspectivePoint, j.PerspectivePoint);
                    if (dis <= 2)
                    {
                        i.IsViolation = true;
                        j.IsViolation = true;
                        violations.Add(new Violation(new RelativeLine(i.BottomCenter, j.BottomCenter), dis));
                    }
                }
            }
            return (violations, humans);
        }
    }
}
