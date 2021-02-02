using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    #region HelperClasses

    public struct RelativePoint
    {
        public Point Norm;
        public Point Frame;
        public double FrameWidth;
        public double FrameHeight;

        public static RelativePoint Zero()
        {
            return new RelativePoint(new Point(0, 0), new Point(0, 0), 0, 0);
        }

        public static RelativePoint Zero(double frameWidth, double frameHeight)
        {
            return new RelativePoint(new Point(0, 0), new Point(0, 0), frameWidth, frameHeight);
        }

        public static RelativePoint FromFrame(Point frame, double frameWidth, double frameHeight)
        {
            return new RelativePoint(new Point(
                GeometryHelpers.Convert(frame.X, frameWidth, Defaults.MaxNormWidth),
                GeometryHelpers.Convert(frame.Y, frameHeight, Defaults.MaxNormHeight)), frame,
                frameWidth, frameHeight);
        }

        public static RelativePoint FromNorm(Point norm, double frameWidth, double frameHeight)
        {
            return new RelativePoint(norm, new Point(
                GeometryHelpers.Convert(norm.X, Defaults.MaxNormWidth, frameWidth),
                GeometryHelpers.Convert(norm.Y, Defaults.MaxNormHeight, frameHeight)),
                frameWidth, frameHeight);
        }

        public RelativePoint(Point norm, Point frame, double frameWidth, double frameHeight)
        {
            Norm = norm;
            Frame = frame;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
        }

        public override bool Equals(object obj)
        {
            if (obj is RelativePoint pPoint)
            {
                return Norm.X == pPoint.Norm.X && Norm.Y == pPoint.Norm.Y && Frame.X == pPoint.Frame.X && Frame.Y == pPoint.Frame.Y;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(RelativePoint left, RelativePoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RelativePoint left, RelativePoint right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct RelativeLine
    {
        public RelativePoint A;
        public RelativePoint B;

        public RelativeLine(RelativePoint a, RelativePoint b)
        {
            A = a;
            B = b;
        }

        public override bool Equals(object obj)
        {
            if (obj is RelativeLine line)
            {
                return (A.Equals(line.A) && B.Equals(line.B)) || (A.Equals(line.B) && B.Equals(line.A));
            }
            return false;
        }

        public static bool operator ==(RelativeLine left, RelativeLine right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RelativeLine left, RelativeLine right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    #endregion

    public static class GeometryHelpers
    {
        public static double Convert(double val, double maxVal, double destMax)
        {
            return destMax * (val / maxVal);
        }

        public static bool IsRelated(params RelativePoint[] points)
        {
            if (points.Length < 2) return true;
            var first = points[0];
            return points.All(s => s.FrameWidth == first.FrameWidth && s.FrameHeight == first.FrameHeight);
        }

        public static bool IsInside(RelativePoint interest, params RelativePoint[] polygon)
        {
            var points = new List<RelativePoint> { interest };
            points.AddRange(polygon);
            if (!IsRelated(points.ToArray()))
            {
                throw new Exception("Points are unrelated");
            }

            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Norm.Y > interest.Norm.Y) != (polygon[j].Norm.Y > interest.Norm.Y)) &&
                (interest.Norm.X < (polygon[j].Norm.X - polygon[i].Norm.X) * (interest.Norm.Y - polygon[i].Norm.Y) / (polygon[j].Norm.Y - polygon[i].Norm.Y) + polygon[i].Norm.X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        public static bool IsInside(RelativePoint interest, RelativePoint point, double normalRadius)
        {
            var points = new List<RelativePoint> { interest, point };
            if (!IsRelated(points.ToArray()))
            {
                throw new Exception("Points are unrelated");
            }
            return GetDistance(interest, point) <= normalRadius;
        }

        public static double GetPointX(RelativePoint point1, RelativePoint point2, double yOffSet)
        {
            return point1.Norm.X + ((point2.Norm.X - point1.Norm.X) * yOffSet);
        }

        public static double GetPointY(RelativePoint point1, RelativePoint point2, double xOffSet)
        {
            return point1.Norm.Y + ((point2.Norm.Y - point1.Norm.Y) * xOffSet);
        }

        public static double GetPointX(RelativeLine line, double yOffSet)
        {
            return line.A.Norm.X + ((line.B.Norm.X - line.A.Norm.X) * yOffSet);
        }

        public static double GetPointY(RelativeLine line, double xOffSet)
        {
            return line.A.Norm.Y + ((line.B.Norm.Y - line.A.Norm.Y) * xOffSet);
        }

        public static RelativePoint GetPoint(RelativePoint a, RelativePoint b, double offSet)
        {
            var points = new List<RelativePoint> { a, b };
            if (!IsRelated(points.ToArray()))
            {
                throw new Exception("Points are unrelated");
            }
            return RelativePoint.FromNorm(new Point(GetPointX(a, b, offSet), GetPointY(a, b, offSet)), a.FrameWidth, a.FrameHeight);
        }

        public static Point GetPoint(RelativeLine line, double offSet)
        {
            return new Point(GetPointX(line, offSet), GetPointY(line, offSet));
        }

        public static bool IsParallel(RelativePoint a, RelativePoint b, RelativePoint c, RelativePoint d)
        {
            if (!IsRelated(a, b, c, d))
            {
                throw new Exception("Points are unrelated");
            }

            double a1 = b.Norm.Y - a.Norm.Y;
            double b1 = a.Norm.X - b.Norm.X;
            double a2 = d.Norm.Y - c.Norm.Y;
            double b2 = c.Norm.X - d.Norm.X;

            double determinant = a1 * b2 - a2 * b1;

            return (determinant == 0);
        }

        public static bool IsParallel(RelativeLine ab, RelativeLine cd)
        {
            return IsParallel(ab.A, ab.B, cd.A, cd.B);
        }

        public static RelativePoint LineIntersection(RelativePoint a, RelativePoint b, RelativePoint c, RelativePoint d)
        {
            if (!IsRelated(a, b, c, d))
            {
                throw new Exception("Points are unrelated");
            }

            // Line AB represented as a1x + b1y = c1  
            double a1 = b.Norm.Y - a.Norm.Y;
            double b1 = a.Norm.X - b.Norm.X;
            double c1 = a1 * (a.Norm.X) + b1 * (a.Norm.Y);

            // Line CD represented as a2x + b2y = c2  
            double a2 = d.Norm.Y - c.Norm.Y;
            double b2 = c.Norm.X - d.Norm.X;
            double c2 = a2 * (c.Norm.X) + b2 * (c.Norm.Y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                throw new Exception("Lines are in parallel");
            }
            else
            {
                int x = (int)((b2 * c1 - b1 * c2) / determinant);
                int y = (int)((a1 * c2 - a2 * c1) / determinant);
                return RelativePoint.FromNorm(new Point(x, y), a.FrameWidth, a.FrameHeight);
            }
        }

        public static RelativePoint LineIntersection(RelativeLine ab, RelativeLine cd)
        {
            return LineIntersection(ab.A, ab.B, cd.A, cd.B);
        }

        public static double GetDistance(RelativePoint pointA, RelativePoint pointB)
        {
            if (!IsRelated(pointA, pointB))
            {
                throw new Exception("Points are unrelated");
            }

            double a = pointB.Norm.X - pointA.Norm.X;
            double b = pointB.Norm.Y - pointA.Norm.Y;

            return Math.Sqrt(a * a + b * b);
        }

        public static double PolygonArea(params RelativePoint[] polygon)
        {
            if (!IsRelated(polygon))
            {
                throw new Exception("Points are unrelated");
            }

            int i, j;
            double area = 0;

            for (i = 0; i < polygon.Length; i++)
            {
                j = (i + 1) % polygon.Length;

                area += polygon[i].Norm.X * polygon[j].Norm.Y;
                area -= polygon[i].Norm.Y * polygon[j].Norm.X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }
    }
}
