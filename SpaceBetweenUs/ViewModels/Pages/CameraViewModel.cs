using FirstFloor.ModernUI.Windows.Controls;
using MvvmHelpers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.Views.Contents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = OpenCvSharp.Point;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class CameraViewModel : BaseViewModel
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

        #endregion

        #region Properties

        public Anchor ReferencedAnchor { get; private set; }
        public GridPoint BL { get; private set; }
        public GridPoint TL { get; private set; }
        public GridPoint TR { get; private set; }
        public GridPoint BR { get; private set; }

        #endregion

        public CameraViewModel()
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

        private void OpenGridEditWindow(Anchor anchor)
        {
            string title = "Grid Point Edit ";
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    title += "(Bottom Left)";
                    break;
                case Anchor.TopLeft:
                    title += "(Top Left)";
                    break;
                case Anchor.TopRight:
                    title += "(Top Right)";
                    break;
                case Anchor.BottomRight:
                    title += "(Bottom Right)";
                    break;
            }
            var gridEditor = new GridPointEdit(anchor);
            var dlg = new ModernDialog
            {
                Title = title,
                Content = gridEditor,
            };
            var cancelButton = dlg.CancelButton;
            cancelButton.Content = "Cancel";
            var okButton = dlg.OkButton;
            okButton.Content = "Ok";
            dlg.Buttons = new Button[] { cancelButton, okButton };
            dlg.MinWidth = 0;
            dlg.MinHeight = 0;
            dlg.SizeChanged += (s, e) =>
            {
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double windowWidth = e.NewSize.Width;
                double windowHeight = e.NewSize.Height;
                dlg.Left = (screenWidth / 2) - (windowWidth / 2);
                dlg.Top = (screenHeight / 2) - (windowHeight / 2);
            };
            dlg.ShowDialog();
            if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
            {
                gridEditor.Save();
                GetAnchorPersistent();
            }
        }

        private Anchor? GetPointAnchor(RelativePoint point)
        {
            if (GeometryHelpers.IsInside(point, BL.Point, Defaults.DotRadius))
            {
                return Anchor.BottomLeft;
            }
            else if (GeometryHelpers.IsInside(point, TL.Point, Defaults.DotRadius))
            {
                return Anchor.TopLeft;
            }
            else if (GeometryHelpers.IsInside(point, TR.Point, Defaults.DotRadius))
            {
                return Anchor.TopRight;
            }
            else if (GeometryHelpers.IsInside(point, BR.Point, Defaults.DotRadius))
            {
                return Anchor.BottomRight;
            }
            return null;
        }

        public void PointerDown(RelativePoint point)
        {
            SelectedEditAnchor = GetPointAnchor(point);
        }

        public void PointerDoubleDown(RelativePoint point)
        {
            var anchor = GetPointAnchor(point);
            if (anchor.HasValue)  OpenGridEditWindow(anchor.Value);
        }

        public void PointerMove(RelativePoint point)
        {
            if (SelectedEditAnchor != null)
            {
                SetAnchorAxis(SelectedEditAnchor.Value, point);
            }
        }

        public void PointerUp()
        {
            SelectedEditAnchor = null;
            SetAnchorPersistent();
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
            if (BL.Point.FrameWidth == 0 || BL.Point.FrameHeight == 0)
                BL = new GridPoint(RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height), 0);
            if (TL.Point.FrameWidth == 0 || TL.Point.FrameHeight == 0)
                TL = new GridPoint(RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height), 0);
            if (TR.Point.FrameWidth == 0 || TR.Point.FrameHeight == 0)
                TR = new GridPoint(RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height), 0);
            if (BR.Point.FrameWidth == 0 || BR.Point.FrameHeight == 0)
                BR = new GridPoint(RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height), 0);
        }
    }
}
