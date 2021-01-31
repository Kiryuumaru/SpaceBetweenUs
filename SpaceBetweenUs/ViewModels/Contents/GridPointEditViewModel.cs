using MvvmHelpers;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.ViewModels.Contents
{
    public class GridPointEditViewModel : BaseViewModel
    {
        private Anchor anchor;
        private double frameWidth;
        private double frameHeight;

        public GridPoint GridPoint
        {
            get
            {
                var point = RelativePoint.FromNorm(new OpenCvSharp.Point(XAxis, YAxis), frameWidth, frameHeight);
                return new GridPoint(point, Depth);
            }
            set
            {
                XAxis = value.Point.Norm.X;
                YAxis = value.Point.Norm.Y;
                Depth = value.Depth;
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

        private double depth;
        public double Depth
        {
            get => depth;
            set => SetProperty(ref depth, value);
        }

        private bool isRefDepth;
        public bool IsRefDepth
        {
            get => isRefDepth;
            set => SetProperty(ref isRefDepth, value);
        }

        public GridPointEditViewModel(Anchor anchor)
        {
            this.anchor = anchor;
            var gridPoint = anchor switch
            {
                Anchor.BottomLeft => Session.GridProjection.BL,
                Anchor.TopLeft => Session.GridProjection.TL,
                Anchor.TopRight => Session.GridProjection.TR,
                Anchor.BottomRight => Session.GridProjection.BR,
                _ => Session.GridProjection.BL
            };
            frameWidth = gridPoint.Point.FrameWidth;
            frameHeight = gridPoint.Point.FrameHeight;
            GridPoint = gridPoint;
            IsRefDepth = Session.GridProjection.ReferenceAnchor == anchor;
        }

        public void Save()
        {
            if (IsRefDepth) Session.GridProjection.ReferenceAnchor = anchor;
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    Session.GridProjection.BL = GridPoint;
                    break;
                case Anchor.TopLeft:
                    Session.GridProjection.TL = GridPoint;
                    break;
                case Anchor.TopRight:
                    Session.GridProjection.TR = GridPoint;
                    break;
                case Anchor.BottomRight:
                    Session.GridProjection.BR = GridPoint;
                    break;
            }
        }
    }
}
