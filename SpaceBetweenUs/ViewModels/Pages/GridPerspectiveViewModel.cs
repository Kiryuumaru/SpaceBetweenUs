using MvvmHelpers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Point = OpenCvSharp.Point;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class GridPerspectiveViewModel : BaseViewModel
    {
        public Anchor? SelectedEditAnchor;

        private Mat currentFrame;
        private Mat resultFrame;
        private int dotRelativeRadius;
        private int lineRelativeThickness;


        #region ViewBindings

        private ImageSource frame;
        public ImageSource Frame
        {
            get => frame;
            set => SetProperty(ref frame, value);
        }

        private double blxAxis;
        public double BLXAxis
        {
            get => blxAxis;
            set => SetProperty(ref blxAxis, Math.Round(value, 2));
        }

        private double blyAxis;
        public double BLYAxis
        {
            get => blyAxis;
            set => SetProperty(ref blyAxis, Math.Round(value, 2));
        }

        private double bldepth;
        public double BLDepth
        {
            get => bldepth;
            set => SetProperty(ref bldepth, Math.Round(value, 2));
        }

        private double tlxAxis;
        public double TLXAxis
        {
            get => tlxAxis;
            set => SetProperty(ref tlxAxis, Math.Round(value, 2));
        }

        private double tlyAxis;
        public double TLYAxis
        {
            get => tlyAxis;
            set => SetProperty(ref tlyAxis, Math.Round(value, 2));
        }

        private double tldepth;
        public double TLDepth
        {
            get => tldepth;
            set => SetProperty(ref tldepth, Math.Round(value, 2));
        }

        private double trxAxis;
        public double TRXAxis
        {
            get => trxAxis;
            set => SetProperty(ref trxAxis, Math.Round(value, 2));
        }

        private double tryAxis;
        public double TRYAxis
        {
            get => tryAxis;
            set => SetProperty(ref tryAxis, Math.Round(value, 2));
        }

        private double trdepth;
        public double TRDepth
        {
            get => trdepth;
            set => SetProperty(ref trdepth, Math.Round(value, 2));
        }

        private double brxAxis;
        public double BRXAxis
        {
            get => brxAxis;
            set => SetProperty(ref brxAxis, Math.Round(value, 2));
        }

        private double bryAxis;
        public double BRYAxis
        {
            get => bryAxis;
            set => SetProperty(ref bryAxis, Math.Round(value, 2));
        }

        private double brdepth;
        public double BRDepth
        {
            get => brdepth;
            set => SetProperty(ref brdepth, Math.Round(value, 2));
        }

        private bool blRef;
        public bool BLRef
        {
            get => blRef;
            set
            {
                SetProperty(ref blRef, value);
            }
        }

        private bool tlRef;
        public bool TLRef
        {
            get => tlRef;
            set
            {
                SetProperty(ref tlRef, value);
            }
        }

        private bool trRef;
        public bool TRRef
        {
            get => trRef;
            set
            {
                SetProperty(ref trRef, value);
            }
        }

        private bool brRef;
        public bool BRRef
        {
            get => brRef;
            set => SetProperty(ref brRef, value);
        }

        #endregion

        #region ConvertedBindings

        public Anchor ReferencedAnchor
        {
            get
            {
                if (blRef) return Anchor.BottomLeft;
                else if (tlRef) return Anchor.TopLeft;
                else if (trRef) return Anchor.TopRight;
                else return Anchor.BottomRight;
            }
            set
            {
                switch (value)
                {
                    case Anchor.BottomLeft:
                        BLRef = true;
                        break;
                    case Anchor.TopLeft:
                        TLRef = true;
                        break;
                    case Anchor.TopRight:
                        TRRef = true;
                        break;
                    case Anchor.BottomRight:
                        BRRef = true;
                        break;
                }
            }
        }

        public GridPoint BL
        {
            get => new GridPoint(
                RelativePoint.FromNorm(new Point(BLXAxis, BLYAxis), Session.FrameSource.Width, Session.FrameSource.Height),
                BLDepth);
            set
            {
                BLXAxis = value.Point.Norm.X;
                BLYAxis = value.Point.Norm.Y;
                BLDepth = value.Depth;
            }
        }

        public GridPoint TL
        {
            get => new GridPoint(
                RelativePoint.FromNorm(new Point(TLXAxis, TLYAxis), Session.FrameSource.Width, Session.FrameSource.Height),
                TLDepth);
            set
            {
                TLXAxis = value.Point.Norm.X;
                TLYAxis = value.Point.Norm.Y;
                TLDepth = value.Depth;
            }
        }

        public GridPoint TR
        {
            get => new GridPoint(
                RelativePoint.FromNorm(new Point(TRXAxis, TRYAxis), Session.FrameSource.Width, Session.FrameSource.Height),
                TRDepth);
            set
            {
                TRXAxis = value.Point.Norm.X;
                TRYAxis = value.Point.Norm.Y;
                TRDepth = value.Depth;
            }
        }

        public GridPoint BR
        {
            get => new GridPoint(
                RelativePoint.FromNorm(new Point(BRXAxis, BRYAxis), Session.FrameSource.Width, Session.FrameSource.Height),
                BRDepth);
            set
            {
                BRXAxis = value.Point.Norm.X;
                BRYAxis = value.Point.Norm.Y;
                BRDepth = value.Depth;
            }
        }

        #endregion

        public GridPerspectiveViewModel()
        {
            GetAnchorPersistent();
            Start();
        }

        private async void Start()
        {
            currentFrame = new Mat();
            resultFrame = new Mat();
            dotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.DotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            lineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.LineThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);

            var s = new Stopwatch();
            while (true)
            {
                s.Restart();

                Session.FrameSource.ReadFrame(currentFrame);
                DrawResult();

                int delayMillis = (int)((1000 / Defaults.Fps) - s.ElapsedMilliseconds);
                await Task.Delay(delayMillis > 0 ? delayMillis : 0);
            }
        }

        public void DrawResult()
        {
            resultFrame = currentFrame.Clone();

            var bl = BL;
            var tl = TL;
            var tr = TR;
            var br = BR;

            if ((bl.Point.Frame.X != 0 || bl.Point.Frame.Y != 0) && (tl.Point.Frame.X != 0 || tl.Point.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    bl.Point.Frame,
                    tl.Point.Frame,
                    Defaults.YellowColor,
                    lineRelativeThickness);
            if ((tl.Point.Frame.X != 0 || tl.Point.Frame.Y != 0) && (tr.Point.Frame.X != 0 || tr.Point.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    tl.Point.Frame,
                    tr.Point.Frame,
                    Defaults.YellowColor,
                    lineRelativeThickness);
            if ((tr.Point.Frame.X != 0 || tr.Point.Frame.Y != 0) && (br.Point.Frame.X != 0 || br.Point.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    tr.Point.Frame,
                    br.Point.Frame,
                    Defaults.YellowColor,
                    lineRelativeThickness);
            if ((br.Point.Frame.X != 0 || br.Point.Frame.Y != 0) && (bl.Point.Frame.X != 0 || bl.Point.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    br.Point.Frame,
                    bl.Point.Frame,
                    Defaults.YellowColor,
                    lineRelativeThickness);

            if (bl.Point.Frame.X != 0 || bl.Point.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    bl.Point.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.BottomLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    lineRelativeThickness);
            if (tl.Point.Frame.X != 0 || tl.Point.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    tl.Point.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.TopLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    lineRelativeThickness);
            if (tr.Point.Frame.X != 0 || tr.Point.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    tr.Point.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.TopRight ? Defaults.GreenColor : Defaults.BlueColor,
                    lineRelativeThickness);
            if (br.Point.Frame.X != 0 || br.Point.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    br.Point.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.BottomRight ? Defaults.GreenColor : Defaults.BlueColor,
                    lineRelativeThickness);

            Frame = resultFrame.ToWriteableBitmap(PixelFormats.Bgr24);
        }

        public void SetAnchorAxis(Anchor anchor, RelativePoint point)
        {
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    BL = new GridPoint(point, BL.Depth);
                    DrawResult();
                    break;
                case Anchor.TopLeft:
                    TL = new GridPoint(point, TL.Depth);
                    DrawResult();
                    break;
                case Anchor.TopRight:
                    TR = new GridPoint(point, TR.Depth);
                    DrawResult();
                    break;
                case Anchor.BottomRight:
                    BR = new GridPoint(point, BR.Depth);
                    DrawResult();
                    break;
            }
        }

        public void SetAnchorPersistent()
        {
            Session.GridProjection.MaxNormWidth = Defaults.MaxNormWidth;
            Session.GridProjection.MaxNormHeight = Defaults.MaxNormHeight;
            Session.GridProjection.ReferenceAnchor = ReferencedAnchor;
            Session.GridProjection.BL = BL;
            Session.GridProjection.TL = TL;
            Session.GridProjection.TR = TR;
            Session.GridProjection.BR = BR;
        }

        public void GetAnchorPersistent()
        {
            ReferencedAnchor = Session.GridProjection.ReferenceAnchor;
            BL = Session.GridProjection.BL;
            TL = Session.GridProjection.TL;
            TR = Session.GridProjection.TR;
            BR = Session.GridProjection.BR;
        }
    }
}
