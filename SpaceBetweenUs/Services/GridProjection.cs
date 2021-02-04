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
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.Zero();
                string data = Session.Datastore.GetValue("grid_bl");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.Zero();
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
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.Zero();
                string data = Session.Datastore.GetValue("grid_tl");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.Zero();
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
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.Zero();
                string data = Session.Datastore.GetValue("grid_tr");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.Zero();
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
                    Defaults.MaxNormHeight != MaxNormHeight) return RelativePoint.Zero();
                string data = Session.Datastore.GetValue("grid_br");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight)) return RelativePoint.Zero();
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

        public double OriginElevation
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return 0;
                string data = Session.Datastore.GetValue("orig_elev");
                if (!double.TryParse(data, out double elev)) return 0;
                return elev;
            }
            set
            {
                Session.Datastore.SetValue("orig_elev", value.ToString());
                SolvePerspective();
            }
        }

        public double OriginAngle
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return 0;
                string data = Session.Datastore.GetValue("orig_angle");
                if (!double.TryParse(data, out double angle)) return 0;
                return angle;
            }
            set
            {
                Session.Datastore.SetValue("orig_angle", value.ToString());
                SolvePerspective();
            }
        }

        public double FOVAngle
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return 0;
                string data = Session.Datastore.GetValue("fov_angle");
                if (!double.TryParse(data, out double angle)) return 0;
                return angle;
            }
            set
            {
                Session.Datastore.SetValue("fov_angle", value.ToString());
                SolvePerspective();
            }
        }

        public double RealOriginTotalBaseLength
        {
            get
            {
                if (FOVAngle == 0) return 0;
                if ((FOVAngle / 2) > OriginAngle)
                {
                    return Math.Tan(GeometryHelpers.ConvertToRadians(FOVAngle)) * OriginElevation;
                }
                else
                {
                    return Math.Tan(GeometryHelpers.ConvertToRadians((FOVAngle / 2) + OriginAngle)) * OriginElevation;
                }
            }
        }

        public double RealFOVBaseHeight
        {
            get
            {
                if (FOVAngle == 0) return 0;
                if ((FOVAngle / 2) > OriginAngle)
                {

                    return
                        Math.Tan(GeometryHelpers.ConvertToRadians((FOVAngle / 2) + OriginAngle)) * OriginElevation +
                        Math.Tan(GeometryHelpers.ConvertToRadians(FOVAngle - ((FOVAngle / 2) + OriginAngle)) * OriginElevation);
                }
                else
                {
                    return
                        Math.Tan(GeometryHelpers.ConvertToRadians((FOVAngle / 2) + OriginAngle)) * OriginElevation -
                        Math.Tan(GeometryHelpers.ConvertToRadians(OriginAngle - (FOVAngle / 2))) * OriginElevation;
                }
            }
        }

        // This is the projected quadrilateral
        private Point2d[] P = new Point2d[4];

        // homographic coefficients
        private double A, B, D, E, G, H;

        private GridProjection() { }

        public static async Task<GridProjection> Initialize()
        {
            var grid = new GridProjection();

            return await Task.FromResult(grid);
        }

        private void SolvePerspective()
        {
            // Initialize corners
            P[0] = BL.Norm;
            P[1] = TL.Norm;
            P[2] = TR.Norm;
            P[3] = BR.Norm;

            // Compute the transform coefficients
            double T = (P[2].X - P[1].X) * (P[2].Y - P[3].Y) - (P[2].X - P[3].X) * (P[2].Y - P[1].Y);

            G = ((P[2].X - P[0].X) * (P[2].Y - P[3].Y) - (P[2].X - P[3].X) * (P[2].Y - P[0].Y)) / (double)T;
            H = ((P[2].X - P[1].X) * (P[2].Y - P[0].Y) - (P[2].X - P[0].X) * (P[2].Y - P[1].Y)) / (double)T;

            A = G * (P[1].X - P[0].X);
            D = G * (P[1].Y - P[0].Y);
            B = H * (P[3].X - P[0].X);
            E = H * (P[3].Y - P[0].Y);

            G -= 1;
            H -= 1;
        }

        private RelativePoint Perspective(double U, double V)
        {
            // Evaluate the homographic transform
            double T = G * U + H * V + 1;
            var point = new Point((A * U + B * V) / (double)T + P[0].X, (D * U + E * V) / (double)T + P[0].Y);
            return RelativePoint.FromNorm(point, Session.FrameSource.Width, Session.FrameSource.Height);
        }

        public IEnumerable<RelativeLine> GetGrid()
        {
            var grid = new List<RelativeLine>();
            SolvePerspective();
            // Draw the perspective grid
            for (var U = 0.0625; U <= 1 - 0.0625; U += 0.0625)
                grid.Add(new RelativeLine(Perspective(U, 0), Perspective(U, 1)));
            for (var V = 0.0625; V <= 1 - 0.0625; V += 0.0625)
                grid.Add(new RelativeLine(Perspective(0, V), Perspective(1, V)));
            return grid;
        }

        public static RelativePoint GetPoint(Point point)
        {
            return new RelativePoint();
        }
    }
}
