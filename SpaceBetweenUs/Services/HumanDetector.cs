using OpenCvSharp;
using SpaceBetweenUs.Model;
using SpaceBetweenUs.Services.Detectors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class HumanDetector
    {
        private static object detectorLockLink = new object();
        private static IDetector detector;
        private Session session;

        public double? violationThreshold;
        public double ViolationThreshold
        {
            get
            {
                if (violationThreshold == null)
                {
                    string data = session.Datastore.GetValue("violation_thres");
                    if (!double.TryParse(data, out double value))
                    {
                        return Defaults.ViolationDistanceDefault;
                    }

                    violationThreshold = value;
                }
                return violationThreshold.Value;
            }
            set
            {
                violationThreshold = value;
                _ = Task.Run(delegate
                  {
                      session.Datastore.SetValue("violation_thres", value.ToString());
                  });
            }
        }

        private HumanDetector() { }
        public static async Task<HumanDetector> Initialize(Session session)
        {
            return await Task.Run(delegate
            {
                InitializeDetector();
                HumanDetector humanDetector = new HumanDetector
                {
                    session = session
                };
                return humanDetector;
            });
        }

        public static void InitializeDetector()
        {
            if (detector == null)
            {
                try
                {
                    detector = new YoloV3Detector();
                }
                catch { }
            }
        }

        public async Task<(IEnumerable<HumanDistance> HumanDistances, IEnumerable<Human> Humans)> Detect(Mat image)
        {
            return await Task.Run(delegate
            {
                lock (detectorLockLink)
                {
                    var humans = new List<Human>();
                    var humanDistances = new List<HumanDistance>();
                    try
                    {
                        var items = detector.Detect(image);
                        foreach (var (Confidence, CenterX, CenterY, Width, Height) in items)
                        {
                            var human = new Human(CenterX - (Width / 2), CenterY - (Height / 2), Width, Height, image.Width, image.Height, Confidence);

                            var pers = session.GridProjection.Perspective(human.BottomCenter);
                            if (pers.HasValue)
                            {
                                human.PerspectivePoint = pers.Value;
                                humans.Add(human);
                            }
                        }
                        foreach (var i in humans)
                        {
                            foreach (var j in humans)
                            {
                                if (i == j)
                                {
                                    continue;
                                }

                                if (session.GridProjection.IsProjectionReady)
                                {
                                    double dis = GeometryHelpers.GetDistance(i.PerspectivePoint, j.PerspectivePoint);
                                    bool isViolation = dis < ViolationThreshold;
                                    if (!i.IsViolation)
                                    {
                                        i.IsViolation = isViolation;
                                    }
                                    if (!j.IsViolation)
                                    {
                                        j.IsViolation = isViolation;
                                    }
                                    humanDistances.Add(new HumanDistance(i, j, dis, isViolation));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return (humanDistances, humans);
                }
            });
        }
    }
}
