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
        Left, Top, Right, Bottom
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
