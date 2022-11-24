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
        TopBottom, LeftRight
    }

    public class GridProjection
    {
        private Session session;

        private double? maxNormWidth;
        public double MaxNormWidth
        {
            get
            {
                if (maxNormWidth == null)
                {
                    string data = session.Datastore.GetValue("max_norm_w");
                    if (!double.TryParse(data, out double value))
                    {
                        return Defaults.MaxNormWidth;
                    }

                    maxNormWidth = value;
                }
                return maxNormWidth.Value;
            }
            set
            {
                maxNormWidth = value;
                _ = Task.Run(delegate
                  {
                      session.Datastore.SetValue("max_norm_w", value.ToString());
                  });
            }
        }

        private double? maxNormHeight;
        public double MaxNormHeight
        {
            get
            {
                if (maxNormHeight == null)
                {
                    string data = session.Datastore.GetValue("max_norm_h");
                    if (!double.TryParse(data, out double value))
                    {
                        return Defaults.MaxNormHeight;
                    }

                    maxNormHeight = value;
                }
                return maxNormHeight.Value;
            }
            set
            {
                maxNormHeight = value;
                _ = Task.Run(delegate
                  {
                      session.Datastore.SetValue("max_norm_h", value.ToString());
                  });
            }
        }

        private RelativePoint? bl;
        public RelativePoint BL
        {
            get
            {
                if (bl == null)
                {
                    if (Defaults.MaxNormWidth != MaxNormWidth ||
                        Defaults.MaxNormHeight != MaxNormHeight)
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    };
                    string data = session.Datastore.GetValue("grid_bl");
                    if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight))
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    };
                    bl = new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
                }
                return bl.Value;
            }
            set
            {
                bl = value;
                _ = Task.Run(delegate
                  {
                      string data = "";
                      data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                      data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                      session.Datastore.SetValue("grid_bl", data);
                      SolvePerspective();
                  });
            }
        }

        private RelativePoint? tl;
        public RelativePoint TL
        {
            get
            {
                if (tl == null)
                {
                    if (Defaults.MaxNormWidth != MaxNormWidth ||
                        Defaults.MaxNormHeight != MaxNormHeight)
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    }

                    string data = session.Datastore.GetValue("grid_tl");
                    if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight))
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    }

                    tl = new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
                }
                return tl.Value;
            }
            set
            {
                tl = value;
                _ = Task.Run(delegate
                  {
                      string data = "";
                      data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                      data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                      session.Datastore.SetValue("grid_tl", data);
                      SolvePerspective();
                  });
            }
        }

        private RelativePoint? tr;
        public RelativePoint TR
        {
            get
            {
                if (tr == null)
                {
                    if (Defaults.MaxNormWidth != MaxNormWidth ||
                        Defaults.MaxNormHeight != MaxNormHeight)
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    }

                    string data = session.Datastore.GetValue("grid_tr");
                    if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight))
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    }

                    tr = new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
                }
                return tr.Value;
            }
            set
            {
                tr = value;
                _ = Task.Run(delegate
                  {
                      string data = "";
                      data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                      data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                      session.Datastore.SetValue("grid_tr", data);
                      SolvePerspective();
                  });
            }
        }

        private RelativePoint? br;
        public RelativePoint BR
        {
            get
            {
                if (br == null)
                {
                    if (Defaults.MaxNormWidth != MaxNormWidth ||
                        Defaults.MaxNormHeight != MaxNormHeight)
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    }

                    string data = session.Datastore.GetValue("grid_br");
                    if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                        !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight))
                    {
                        return RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), session.FrameSource.Width, session.FrameSource.Height);
                    }

                    br = new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight);
                }
                return br.Value;
            }
            set
            {
                br = value;
                _ = Task.Run(delegate
                  {
                      string data = "";
                      data = CommonHelpers.BlobSetValue(data, "x_norm", value.Norm.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_norm", value.Norm.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "x_frame", value.Frame.X.ToString());
                      data = CommonHelpers.BlobSetValue(data, "y_frame", value.Frame.Y.ToString());
                      data = CommonHelpers.BlobSetValue(data, "w_frame", value.FrameWidth.ToString());
                      data = CommonHelpers.BlobSetValue(data, "h_frame", value.FrameHeight.ToString());
                      session.Datastore.SetValue("grid_br", data);
                      SolvePerspective();
                  });
            }
        }

        private double? topBottomDistance;
        public double TopBottomDistance
        {
            get
            {
                if (topBottomDistance == null)
                {
                    if (Defaults.MaxNormWidth != MaxNormWidth ||
                        Defaults.MaxNormHeight != MaxNormHeight)
                    {
                        return 0;
                    }

                    string data = session.Datastore.GetValue("tb_dist");
                    if (!double.TryParse(data, out double dist))
                    {
                        return 0;
                    }

                    topBottomDistance = dist;
                }
                return topBottomDistance.Value;
            }
            set
            {
                topBottomDistance = value;
                _ = Task.Run(delegate
                  {
                      session.Datastore.SetValue("tb_dist", value.ToString());
                      SolvePerspective();
                  });
            }
        }

        private double? leftRightDistance;
        public double LeftRightDistance
        {
            get
            {
                if (leftRightDistance == null)
                {
                    if (Defaults.MaxNormWidth != MaxNormWidth ||
                        Defaults.MaxNormHeight != MaxNormHeight)
                    {
                        return 0;
                    }

                    string data = session.Datastore.GetValue("lr_dist");
                    if (!double.TryParse(data, out double dist))
                    {
                        return 0;
                    }

                    leftRightDistance = dist;
                }
                return leftRightDistance.Value;
            }
            set
            {
                leftRightDistance = value;
                _ = Task.Run(delegate
                  {
                      session.Datastore.SetValue("lr_dist", value.ToString());
                      SolvePerspective();
                  });
            }
        }

        private string unit;
        public string Unit
        {
            get
            {
                if (unit == null)
                {
                    unit = session.Datastore.GetValue("unit") ?? "";
                }
                return unit;
            }
            set
            {
                unit = value;
                Task.Run(delegate
                {
                    session.Datastore.SetValue("unit", value);
                });
            }
        }

        public RelativePoint LeftMidPoint => GeometryHelpers.GetPoint(BL, TL, 0.5);
        public RelativePoint TopMidPoint => GeometryHelpers.GetPoint(TL, TR, 0.5);
        public RelativePoint RightMidPoint => GeometryHelpers.GetPoint(TR, BR, 0.5);
        public RelativePoint BottomMidPoint => GeometryHelpers.GetPoint(BR, BL, 0.5);

        public bool IsProjectionReady => LeftRightDistance > 0 && TopBottomDistance > 0;

        // This is the projected quadrilateral
        private readonly Point2d[] p = new Point2d[4];

        // homographic coefficients
        private double a, b, d, e, g, h;

        private GridProjection() { }

        public static async Task<GridProjection> Initialize(Session session)
        {
            GridProjection grid = new GridProjection()
            {
                session = session
            };
            _ = grid.MaxNormWidth;
            _ = grid.MaxNormHeight;
            _ = grid.BL;
            _ = grid.TL;
            _ = grid.TR;
            _ = grid.BR;
            _ = grid.TopBottomDistance;
            _ = grid.LeftRightDistance;
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
            double x = point.X / TopBottomDistance;
            double y = point.Y / LeftRightDistance;
            double T = g * x + h * y + 1;
            Point normPoint = new Point((a * x + b * y) / (double)T + p[0].X, (d * x + e * y) / (double)T + p[0].Y);
            return RelativePoint.FromNorm(normPoint, session.FrameSource.Width, session.FrameSource.Height);
        }

        public Point2d? Perspective(RelativePoint point)
        {
            if (!GeometryHelpers.IsInside(point, BL, TL, TR, BR))
            {
                return null;
            }

            double px = p[0].X;
            double py = p[0].Y;
            double xp = point.Norm.X;
            double yp = point.Norm.Y;

            double detD = ((a + g * px - g * xp) * (e + h * py - h * yp)) - ((d + g * py - g * yp) * (b + h * px - h * xp));
            double detDx = ((xp - px) * (e + h * py - h * yp)) - ((yp - py) * (b + h * px - h * xp));
            double detDy = ((a + g * px - g * xp) * (yp - py)) - ((d + g * py - g * yp) * (xp - px));

            double x = detDx / detD;
            double y = detDy / detD;

            double normX = x * TopBottomDistance;
            double normY = y * LeftRightDistance;

            return new Point2d(normX, normY);
        }

        public IEnumerable<RelativeLine> GetGrid()
        {
            if (TopBottomDistance == 0 || LeftRightDistance == 0)
            {
                return null;
            }

            List<RelativeLine> lines = new List<RelativeLine>();

            SolvePerspective();

            double currentDist = 0;
            while (currentDist <= LeftRightDistance)
            {
                lines.Add(new RelativeLine(Perspective(new Point2d(0, currentDist)), Perspective(new Point2d(1, currentDist))));
                currentDist += Defaults.GridNotchDistance;
            }
            currentDist = 0;
            while (currentDist <= TopBottomDistance)
            {
                lines.Add(new RelativeLine(Perspective(new Point2d(currentDist, 0)), Perspective(new Point2d(currentDist, 1))));
                currentDist += Defaults.GridNotchDistance;
            }

            return lines;
        }

        public IEnumerable<RelativePoint> GetGridPoints()
        {
            if (TopBottomDistance == 0 || LeftRightDistance == 0)
            {
                return null;
            }

            List<RelativeLine> vLines = new List<RelativeLine>();
            List<RelativeLine> hLines = new List<RelativeLine>();
            List<RelativePoint> points = new List<RelativePoint>();

            SolvePerspective();

            double currentDist = 0;
            while (currentDist <= LeftRightDistance)
            {
                hLines.Add(new RelativeLine(Perspective(new Point2d(0, currentDist)), Perspective(new Point2d(1, currentDist))));
                currentDist += Defaults.GridNotchDistance;
            }
            currentDist = 0;
            while (currentDist <= TopBottomDistance)
            {
                vLines.Add(new RelativeLine(Perspective(new Point2d(currentDist, 0)), Perspective(new Point2d(currentDist, 1))));
                currentDist += Defaults.GridNotchDistance;
            }

            foreach (RelativeLine vLine in vLines)
            {
                foreach (RelativeLine hLine in hLines)
                {
                    try
                    {
                        points.Add(GeometryHelpers.LineIntersection(vLine, hLine));
                    }
                    catch { }
                }
            }

            return points;
        }
    }
}
