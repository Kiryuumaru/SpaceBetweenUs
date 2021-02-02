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
            var point = anchor switch
            {
                Anchor.BottomLeft => Session.GridProjection.BL,
                Anchor.TopLeft => Session.GridProjection.TL,
                Anchor.TopRight => Session.GridProjection.TR,
                Anchor.BottomRight => Session.GridProjection.BR,
                _ => Session.GridProjection.BL
            };
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
