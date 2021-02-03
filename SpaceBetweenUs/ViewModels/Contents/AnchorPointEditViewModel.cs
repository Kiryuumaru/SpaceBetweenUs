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

        public AnchorPointEditViewModel(Anchor anchor)
        {
            this.anchor = anchor;
            RelativePoint point = RelativePoint.Zero();
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    Header = "Bottom Left";
                    point = Session.GridProjection.BL;
                    break;
                case Anchor.TopLeft:
                    Header = "Top Left";
                    point = Session.GridProjection.TL;
                    break;
                case Anchor.TopRight:
                    Header = "Top Right";
                    point = Session.GridProjection.TR;
                    break;
                case Anchor.BottomRight:
                    Header = "Bottom Right";
                    point = Session.GridProjection.BR;
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
                    Session.GridProjection.BL = RelativePoint;
                    break;
                case Anchor.TopLeft:
                    Session.GridProjection.TL = RelativePoint;
                    break;
                case Anchor.TopRight:
                    Session.GridProjection.TR = RelativePoint;
                    break;
                case Anchor.BottomRight:
                    Session.GridProjection.BR = RelativePoint;
                    break;
            }
        }
    }
}
