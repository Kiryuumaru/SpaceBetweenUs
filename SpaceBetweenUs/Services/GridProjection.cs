using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public enum Anchor
    {
        BottomLeft, TopLeft, TopRight, BottomRight
    }

    public enum GridSide
    {
        Top, Bottom, Left, Right
    }

    public class GridProjection
    {
        public double MaxNormWidth
        {
            get
            {
                string data = Session.Datastore.GetValue("max_norm_w");
                if (!double.TryParse(data, out double maxNormWidth)) return Defaults.MaxNormWidth;
                return maxNormWidth;
            }
            set
            {
                Session.Datastore.SetValue("max_norm_w", value.ToString());
            }
        }

        public double MaxNormHeight
        {
            get
            {
                string data = Session.Datastore.GetValue("max_norm_h");
                if (!double.TryParse(data, out double maxNormHeight)) return Defaults.MaxNormHeight;
                return maxNormHeight;
            }
            set
            {
                Session.Datastore.SetValue("max_norm_h", value.ToString());
            }
        }

        public RelativePoint BL
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height); ;
                string data = Session.Datastore.GetValue("grid_bl");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height); ;
                return new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                Session.Datastore.SetValue("grid_bl", data);
                SolvePerspective();
            }
        }

        public RelativePoint TL
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
                string data = Session.Datastore.GetValue("grid_tl");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
                return new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                Session.Datastore.SetValue("grid_tl", data);
                SolvePerspective();
            }
        }

        public RelativePoint TR
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
                string data = Session.Datastore.GetValue("grid_tr");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
                return new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                Session.Datastore.SetValue("grid_tr", data);
                SolvePerspective();
            }
        }

        public RelativePoint BR
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
                string data = Session.Datastore.GetValue("grid_br");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
                return new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                Session.Datastore.SetValue("grid_br", data);
                SolvePerspective();
            }
        }

        public double LeftDistance
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return 0;
                string data = Session.Datastore.GetValue("l_dist");
                if (!double.TryParse(data, out double dist)) return 0;
                return dist;
            }
            set
            {
                Session.Datastore.SetValue("l_dist", value.ToString());
                SolvePerspective();
            }
        }

        public double TopDistance
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return 0;
                string data = Session.Datastore.GetValue("t_dist");
                if (!double.TryParse(data, out double dist)) return 0;
                return dist;
            }
            set
            {
                Session.Datastore.SetValue("t_dist", value.ToString());
                SolvePerspective();
            }
        }

        public double RightDistance
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return 0;
                string data = Session.Datastore.GetValue("r_dist");
                if (!double.TryParse(data, out double dist)) return 0;
                return dist;
            }
            set
            {
                Session.Datastore.SetValue("r_dist", value.ToString());
                SolvePerspective();
            }
        }

        public double BottomDistance
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return 0;
                string data = Session.Datastore.GetValue("b_dist");
                if (!double.TryParse(data, out double dist)) return 0;
                return dist;
            }
            set
            {
                Session.Datastore.SetValue("b_dist", value.ToString());
                SolvePerspective();
            }
        }

        public RelativePoint LeftMidPoint => GeometryHelpers.GetPoint(BL, TL, 0.5);
        public RelativePoint TopMidPoint => GeometryHelpers.GetPoint(TL, TR, 0.5);
        public RelativePoint RightMidPoint => GeometryHelpers.GetPoint(TR, BR, 0.5);
        public RelativePoint BottomMidPoint => GeometryHelpers.GetPoint(BR, BL, 0.5);

        // This is the projected quadrilateral
        private readonly Point2d[] p = new Point2d[4];

        // homographic coefficients
        private double a, b, d, e, g, h;

        private GridProjection() { }

        public static async Task<GridProjection> Initialize()
        {
            var grid = new GridProjection();

            return await Task.FromResult(grid);
        }

        private void SolvePerspective()
        {
            // Initialize corners
            p[0] = TL.Norm;
            p[1] = TR.Norm;
            p[2] = BR.Norm;
            p[3] = BL.Norm;

            // Compute the transform coefficients
            double T = (p[2].X - p[1].X) * (p[2].Y - p[3].Y) - (p[2].X - p[3].X) * (p[2].Y - p[1].Y);

            g = ((p[2].X - p[0].X) * (p[2].Y - p[3].Y) - (p[2].X - p[3].X) * (p[2].Y - p[0].Y)) / (double)T;
            h = ((p[2].X - p[1].X) * (p[2].Y - p[0].Y) - (p[2].X - p[0].X) * (p[2].Y - p[1].Y)) / (double)T;

            a = g * (p[1].X - p[0].X);
            d = g * (p[1].Y - p[0].Y);
            b = h * (p[3].X - p[0].X);
            e = h * (p[3].Y - p[0].Y);

            g -= 1;
            h -= 1;
        }

        public RelativePoint Perspective(Point2d point)
        {
            // Evaluate the homographic transform
            double x = point.X / TopDistance;
            double y = point.Y / LeftDistance;
            double T = g * x + h * y + 1;
            var normPoint = new Point((a * x + b * y) / (double)T + p[0].X, (d * x + e * y) / (double)T + p[0].Y);
            return RelativePoint.FromNorm(normPoint, Session.FrameSource.Width, Session.FrameSource.Height);
        }

        public Point2d? Perspective(RelativePoint point)
        {
            if (!GeometryHelpers.IsInside(point, BL, TL, TR, BR)) return null;

            RelativePoint bl = BL;
            RelativePoint tl = TL;
            RelativePoint tr;
            RelativePoint br;
            double currentXDist = 0;
            while (currentXDist <= TopDistance)
            {
                currentXDist += Defaults.GridPrecision;
                tr = Perspective(new Point2d(currentXDist, 0));
                br = Perspective(new Point2d(currentXDist, LeftDistance));
                if (GeometryHelpers.IsInside(point, bl, tl, tr, br)) break;
                bl = br;
                tl = tr;
            }
            tl = TL;
            tr = TR;
            double currentYDist = 0;
            while (currentYDist <= LeftDistance)
            {
                currentYDist += Defaults.GridPrecision;
                bl = Perspective(new Point2d(0, currentYDist));
                br = Perspective(new Point2d(TopDistance, currentYDist));
                if (GeometryHelpers.IsInside(point, bl, tl, tr, br)) break;
                tl = bl;
                tr = br;
            }
            return new Point2d(currentXDist, currentYDist);
        }

        public IEnumerable<RelativeLine> GetGrid()
        {
            if (TopDistance == 0 || BottomDistance == 0 || LeftDistance == 0 || RightDistance == 0) return null;

            var lines = new List<RelativeLine>();

            SolvePerspective();

            double currentDist = 0;
            while (currentDist < (LeftDistance + Defaults.GridNotchDistance))
            {
                lines.Add(new RelativeLine(Perspective(new Point2d(0, currentDist)), Perspective(new Point2d(1, currentDist))));
                currentDist += Defaults.GridNotchDistance;
            }
            currentDist = 0;
            while (currentDist < (TopDistance + Defaults.GridNotchDistance))
            {
                lines.Add(new RelativeLine(Perspective(new Point2d(currentDist, 0)), Perspective(new Point2d(currentDist, 1))));
                currentDist += Defaults.GridNotchDistance;
            }

            return lines;
        }

        public IEnumerable<RelativePoint> GetGridPoints()
        {
            if (TopDistance == 0 || BottomDistance == 0 || LeftDistance == 0 || RightDistance == 0) return null;

            var vLines = new List<RelativeLine>();
            var hLines = new List<RelativeLine>();
            var points = new List<RelativePoint>();

            SolvePerspective();

            double currentDist = 0;
            while (currentDist < (LeftDistance + Defaults.GridNotchDistance))
            {
                hLines.Add(new RelativeLine(Perspective(new Point2d(0, currentDist)), Perspective(new Point2d(1, currentDist))));
                currentDist += Defaults.GridNotchDistance;
            }
            currentDist = 0;
            while (currentDist < (TopDistance + Defaults.GridNotchDistance))
            {
                vLines.Add(new RelativeLine(Perspective(new Point2d(currentDist, 0)), Perspective(new Point2d(currentDist, 1))));
                currentDist += Defaults.GridNotchDistance;
            }

            foreach (var vLine in vLines)
            {
                foreach (var hLine in hLines)
                {
                    points.Add(GeometryHelpers.LineIntersection(vLine, hLine));

                }
            }

            return points;
        }

        public static RelativePoint GetPoint(Point point)
        {
            return new RelativePoint();
        }
    }
}
