using MvvmHelpers;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Contents
{
    public class AnchorPointEditViewModel : BaseViewModel
    {
        private Session session;

        private readonly Anchor anchor;
        private readonly double frameWidth;
        private readonly double frameHeight;

        public RelativePoint RelativePoint
        {
            get
            {
                return RelativePoint.FromNorm(new OpenCvSharp.Point(XAxis, YAxis), frameWidth, frameHeight);
            }
            set
            {
                XAxis = value.Norm.X;
                YAxis = value.Norm.Y;
            }
        }

        private double xAxis;
        public double XAxis
        {
            get => xAxis;
            set => SetProperty(ref xAxis, value);
        }

        private double yAxis;
        public double YAxis
        {
            get => yAxis;
            set => SetProperty(ref yAxis, value);
        }

        public AnchorPointEditViewModel(Session session, Anchor anchor)
        {
            this.session = session;
            this.anchor = anchor;
            RelativePoint point = RelativePoint.Zero();
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    Header = "Bottom Left";
                    point = session.GridProjection.BL;
                    break;
                case Anchor.TopLeft:
                    Header = "Top Left";
                    point = session.GridProjection.TL;
                    break;
                case Anchor.TopRight:
                    Header = "Top Right";
                    point = session.GridProjection.TR;
                    break;
                case Anchor.BottomRight:
                    Header = "Bottom Right";
                    point = session.GridProjection.BR;
                    break;
            }
            frameWidth = point.FrameWidth;
            frameHeight = point.FrameHeight;
            RelativePoint = point;
        }

        public void Save()
        {
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    session.GridProjection.BL = RelativePoint;
                    break;
                case Anchor.TopLeft:
                    session.GridProjection.TL = RelativePoint;
                    break;
                case Anchor.TopRight:
                    session.GridProjection.TR = RelativePoint;
                    break;
                case Anchor.BottomRight:
                    session.GridProjection.BR = RelativePoint;
                    break;
            }
        }
    }
}
