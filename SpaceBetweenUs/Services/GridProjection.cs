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

    public struct GridPoint
    {
        public RelativePoint Point;
        public double Depth;

        public GridPoint(RelativePoint point, double depth)
        {
            Point = point;
            Depth = depth;
        }

        public static GridPoint Zero()
        {
            return new GridPoint(RelativePoint.Zero(), 0);
        }

        public static GridPoint Zero(double frameWidth, double frameHeight)
        {
            return new GridPoint(RelativePoint.Zero(frameWidth, frameHeight), 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GridPoint gridPoint)) return false;
            return Point == gridPoint.Point && Depth == gridPoint.Depth;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(GridPoint left, GridPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridPoint left, GridPoint right)
        {
            return !(left == right);
        }
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

        public Anchor ReferenceAnchor
        {
            get
            {
                string data = Session.Datastore.GetValue("grid_ref");
                return data switch
                {
                    "bl" => Anchor.BottomLeft,
                    "tl" => Anchor.TopLeft,
                    "tr" => Anchor.TopRight,
                    "br" => Anchor.BottomRight,
                    _ => Anchor.BottomLeft,
                };
            }
            set
            {
                Session.Datastore.SetValue("grid_ref", value switch
                {
                    Anchor.BottomLeft => "bl",
                    Anchor.TopLeft => "tl",
                    Anchor.TopRight => "tr",
                    Anchor.BottomRight => "br",
                    _ => "br"
                });
            }
        }

        public GridPoint BL
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return GridPoint.Zero();
                string data = Session.Datastore.GetValue("grid_bl");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "depth"), out double depth)) return GridPoint.Zero();
                return new GridPoint(
                    new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight),
                    depth);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Point.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Point.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Point.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Point.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.Point.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.Point.FrameHeight.ToString());
                data = CommonHelpers.BlobSetValue(data, "depth", value.Depth.ToString());
                Session.Datastore.SetValue("grid_bl", data);
            }
        }

        public GridPoint TL
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return GridPoint.Zero();
                string data = Session.Datastore.GetValue("grid_tl");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "depth"), out double depth)) return GridPoint.Zero();
                return new GridPoint(
                    new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight),
                    depth);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Point.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Point.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Point.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Point.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.Point.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.Point.FrameHeight.ToString());
                data = CommonHelpers.BlobSetValue(data, "depth", value.Depth.ToString());
                Session.Datastore.SetValue("grid_tl", data);
            }
        }

        public GridPoint TR
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return GridPoint.Zero();
                string data = Session.Datastore.GetValue("grid_tr");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "depth"), out double depth)) return GridPoint.Zero();
                return new GridPoint(
                    new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight),
                    depth);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Point.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Point.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Point.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Point.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.Point.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.Point.FrameHeight.ToString());
                data = CommonHelpers.BlobSetValue(data, "depth", value.Depth.ToString());
                Session.Datastore.SetValue("grid_tr", data);
            }
        }

        public GridPoint BR
        {
            get
            {
                if (Defaults.MaxNormWidth != MaxNormWidth ||
                    Defaults.MaxNormHeight != MaxNormHeight) return GridPoint.Zero();
                string data = Session.Datastore.GetValue("grid_br");
                if (!double.TryParse(CommonHelpers.BlobGetValue(data, "x_norm"), out double xNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_norm"), out double yNormAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "x_frame"), out double xFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "y_frame"), out double yFrameAxis) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "w_frame"), out double frameWidth) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "h_frame"), out double frameHeight) ||
                    !double.TryParse(CommonHelpers.BlobGetValue(data, "depth"), out double depth)) return GridPoint.Zero();
                return new GridPoint(
                    new RelativePoint(new Point(xNormAxis, yNormAxis), new Point(xFrameAxis, yFrameAxis), frameWidth, frameHeight),
                    depth);
            }
            set
            {
                string data = "";
                data = CommonHelpers.BlobSetValue(data, "x_norm", value.Point.Norm.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_norm", value.Point.Norm.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "x_frame", value.Point.Frame.X.ToString());
                data = CommonHelpers.BlobSetValue(data, "y_frame", value.Point.Frame.Y.ToString());
                data = CommonHelpers.BlobSetValue(data, "w_frame", value.Point.FrameWidth.ToString());
                data = CommonHelpers.BlobSetValue(data, "h_frame", value.Point.FrameHeight.ToString());
                data = CommonHelpers.BlobSetValue(data, "depth", value.Depth.ToString());
                Session.Datastore.SetValue("grid_br", data);
            }
        }

        private GridProjection() { }

        public static async Task<GridProjection> Initialize()
        {
            var grid = new GridProjection();

            return await Task.FromResult(grid);
        }
    }
}
