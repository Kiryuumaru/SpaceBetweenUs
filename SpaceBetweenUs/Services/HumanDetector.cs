using OpenCvSharp;
using SpaceBetweenUs.Services.Detectors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class Human
    {
        public RelativePoint BL;
        public RelativePoint TL;
        public RelativePoint TR;
        public RelativePoint BR;
        public RelativePoint Center;
        public RelativePoint BottomCenter;
        public Point2d PerspectivePoint;
        public bool IsViolation;
        public string Test;

        public Human(double x, double y, double width, double height, double frameWidth, double frameHeight)
        {
            BL = RelativePoint.FromFrame(new Point(x, y + height), frameWidth, frameHeight);
            TL = RelativePoint.FromFrame(new Point(x, y), frameWidth, frameHeight);
            TR = RelativePoint.FromFrame(new Point(x + width, y), frameWidth, frameHeight);
            BR = RelativePoint.FromFrame(new Point(x + width, y + height), frameWidth, frameHeight);
            Center = RelativePoint.FromFrame(new Point(x + width / 2, y + height / 2), frameWidth, frameHeight);
            BottomCenter = RelativePoint.FromFrame(new Point(x + width / 2, y + height), frameWidth, frameHeight);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Human human)) return false;
            return
                BL == human.BL &&
                TL == human.TL &&
                TR == human.TR &&
                BR == human.BR &&
                Center == human.Center &&
                BottomCenter == human.BottomCenter;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Human left, Human right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Human left, Human right)
        {
            return !(left == right);
        }
    }

    public class Violation
    {
        public RelativeLine Line;
        public double Distance;
        public Violation(RelativeLine line, double distance)
        {
            Line = line;
            Distance = distance;
        }
    }

    public class HumanDetector
    {
        private static ConcurrentQueue<(Session Session, Mat Image, Action<(IEnumerable<Violation> Violations, IEnumerable<Human> Humans)> onDetection)> detectionTasks = new ConcurrentQueue<(Session Session, Mat image, Action<(IEnumerable<Violation> Violations, IEnumerable<Human> Humans)> onDetection)>();
        private static IDetector detector;
        private static bool isDetectorBusy = false;
        private Session session;

        public double? violationThreshold;
        public double ViolationThreshold
        {
            get
            {
                if (violationThreshold == null)
                {
                    string data = session.Datastore.GetValue("violation_thres");
                    if (!double.TryParse(data, out double value)) return Defaults.ViolationDistanceDefault;
                    violationThreshold = value;
                }
                return violationThreshold.Value;
            }
            set
            {
                violationThreshold = value;
                Task.Run(delegate
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
                var humanDetector = new HumanDetector
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
                detector = new AlturosYoloV3Detector();
                //detector = new YoloV3Detector();
            }
        }

        public void Detect(Mat image, Action<(IEnumerable<Violation> Violations, IEnumerable<Human> Humans)> onDetection)
        {
            detectionTasks.Enqueue((session, image, onDetection));
            if (isDetectorBusy) return;
            isDetectorBusy = true;
            while (true)
            {
                if (!detectionTasks.TryDequeue(out (Session Session, Mat Image, Action<(IEnumerable<Violation> Violations, IEnumerable<Human> Humans)> OnDetection) task)) break;
                var humans = new List<Human>();
                var violations = new List<Violation>();
                try
                {
                    var items = detector.Detect(task.Image);
                    foreach (var (Confidence, CenterX, CenterY, Width, Height) in items)
                    {
                        var human = new Human(CenterX - (Width / 2), CenterY - (Height / 2), Width, Height, task.Image.Width, task.Image.Height);

                        var pers = task.Session.GridProjection.Perspective(human.BottomCenter);
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
                            if (i == j) continue;
                            if (violations.Any(v =>
                                (i.BottomCenter == v.Line.A && j.BottomCenter == v.Line.B) ||
                                (i.BottomCenter == v.Line.B && j.BottomCenter == v.Line.A) ||
                                (j.BottomCenter == v.Line.A && i.BottomCenter == v.Line.B) ||
                                (j.BottomCenter == v.Line.B && i.BottomCenter == v.Line.A))) continue;
                            if (task.Session.GridProjection.IsProjectionReady)
                            {
                                double dis = GeometryHelpers.GetDistance(i.PerspectivePoint, j.PerspectivePoint);
                                if (dis < ViolationThreshold)
                                {
                                    i.IsViolation = true;
                                    j.IsViolation = true;
                                    violations.Add(new Violation(new RelativeLine(i.BottomCenter, j.BottomCenter), dis));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                task.OnDetection?.Invoke((violations, humans));
            }
            isDetectorBusy = false;
        }
    }
}
