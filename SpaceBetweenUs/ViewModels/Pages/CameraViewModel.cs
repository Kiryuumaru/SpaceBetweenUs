using Alturos.Yolo.Model;
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
using System.Windows.Threading;
using Point = OpenCvSharp.Point;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class CameraViewModel : BaseViewModel
    {
        private readonly Dispatcher dispatcher;
        public Anchor? SelectedEditAnchor;
        public GridSide? SelectedEditGridSide;
        private Mat currentFrame;
        private Mat resultFrame;
        private int dotRelativeRadius;
        private int innerDotRelativeRadius;
        private int borderLineRelativeThickness;
        private int itemLineRelativeThickness;
        private IEnumerable<Human> items = new List<Human>();

        #region ViewBindings

        private ImageSource frame;
        public ImageSource Frame
        {
            get => frame;
            set => SetProperty(ref frame, value);
        }

        public int violationCount;
        public int ViolationCount
        {
            get => violationCount;
            set => SetProperty(ref violationCount, value);
        }

        #endregion

        #region Properties

        public Anchor ReferencedAnchor { get; private set; }
        public RelativePoint BL { get; private set; }
        public RelativePoint TL { get; private set; }
        public RelativePoint TR { get; private set; }
        public RelativePoint BR { get; private set; }
        public double LeftDistance { get; private set; }
        public double TopDistance { get; private set; }
        public double RightDistance { get; private set; }
        public double BottomDistance { get; private set; }
        public RelativePoint LeftMidPoint { get; private set; }
        public RelativePoint TopMidPoint { get; private set; }
        public RelativePoint RightMidPoint { get; private set; }
        public RelativePoint BottomMidPoint { get; private set; }

        #endregion

        public CameraViewModel(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            GetPersistent();
            Task.Run(Start);
        }

        private async void Start()
        {
            ViolationCount = 0;

            currentFrame = new Mat();
            resultFrame = new Mat();
            dotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.AnchorDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            innerDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.InnerDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            borderLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.BorderLineThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            itemLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.ItemLineThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            
            var s = new Stopwatch();
            while (true)
            {
                s.Restart();

                Session.FrameSource.ReadFrame(currentFrame);
                Detect();
                DrawResult();

                int delayMillis = (int)((1000 / Defaults.Fps) - s.ElapsedMilliseconds);
                await Task.Delay(delayMillis > 0 ? delayMillis : 0);
            }
        }

        public void Detect()
        {
            items = Session.HumanDetector?.DetectHuman(currentFrame.ToBytes());
            ViolationCount = items?.Where(i => i.IsViolation).Count() ?? 0;
        }

        public void DrawResult()
        {
            resultFrame = currentFrame.Clone();

            var bl = BL;
            var tl = TL;
            var tr = TR;
            var br = BR;

            if ((bl.Frame.X != 0 || bl.Frame.Y != 0) && (tl.Frame.X != 0 || tl.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    bl.Frame,
                    tl.Frame,
                    Defaults.YellowColor,
                    borderLineRelativeThickness);
            if ((tl.Frame.X != 0 || tl.Frame.Y != 0) && (tr.Frame.X != 0 || tr.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    tl.Frame,
                    tr.Frame,
                    Defaults.YellowColor,
                    borderLineRelativeThickness);
            if ((tr.Frame.X != 0 || tr.Frame.Y != 0) && (br.Frame.X != 0 || br.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    tr.Frame,
                    br.Frame,
                    Defaults.YellowColor,
                    borderLineRelativeThickness);
            if ((br.Frame.X != 0 || br.Frame.Y != 0) && (bl.Frame.X != 0 || bl.Frame.Y != 0))
                Cv2.Line(
                    resultFrame,
                    br.Frame,
                    bl.Frame,
                    Defaults.YellowColor,
                    borderLineRelativeThickness);

            if (bl.Frame.X != 0 || bl.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    bl.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.BottomLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (tl.Frame.X != 0 || tl.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    tl.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.TopLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (tr.Frame.X != 0 || tr.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    tr.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.TopRight ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (br.Frame.X != 0 || br.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    br.Frame,
                    dotRelativeRadius,
                    SelectedEditAnchor == Anchor.BottomRight ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);

            if (bl.Frame.X != 0 || bl.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    LeftMidPoint.Frame,
                    innerDotRelativeRadius,
                    SelectedEditGridSide == GridSide.Left ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
            if (tl.Frame.X != 0 || tl.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    TopMidPoint.Frame,
                    innerDotRelativeRadius,
                    SelectedEditGridSide == GridSide.Top ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
            if (tr.Frame.X != 0 || tr.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    RightMidPoint.Frame,
                    innerDotRelativeRadius,
                    SelectedEditGridSide == GridSide.Right ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
            if (br.Frame.X != 0 || br.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    BottomMidPoint.Frame,
                    innerDotRelativeRadius,
                    SelectedEditGridSide == GridSide.Bottom ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);

            if (items != null)
            {
                foreach (var item in items)
                {
                    Cv2.Line(resultFrame, item.BL.Frame, item.TL.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Line(resultFrame, item.TL.Frame, item.TR.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Line(resultFrame, item.TR.Frame, item.BR.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Line(resultFrame, item.BR.Frame, item.BL.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Circle(
                        resultFrame,
                        item.BottomCenter.Frame,
                        innerDotRelativeRadius,
                        item.IsViolation ? Defaults.RedColor : Defaults.GreenColor,
                        Cv2.FILLED);
                }
            }
            try
            {
                dispatcher.Invoke(delegate
                {
                    Frame = resultFrame.ToWriteableBitmap(PixelFormats.Bgr24);
                });
            }
            catch { }
        }

        private void OpenAnchorEditWindow(Anchor anchor)
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
            var gridEditor = new AnchorPointEdit(anchor);
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
                GetPersistent();
            }
        }

        private void OpenGridSideEditWindow(GridSide side)
        {
            string title = "Grid Point Edit ";
            switch (side)
            {
                case GridSide.Left:
                    title += "(Left)";
                    break;
                case GridSide.Top:
                    title += "(Top)";
                    break;
                case GridSide.Right:
                    title += "(Right)";
                    break;
                case GridSide.Bottom:
                    title += "(Bottom)";
                    break;
            }
            var sideEditor = new GridSideEdit(side);
            var dlg = new ModernDialog
            {
                Title = title,
                Content = sideEditor,
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
                sideEditor.Save();
                GetPersistent();
            }
        }

        private Anchor? GetPointAnchor(RelativePoint point)
        {
            if (GeometryHelpers.IsInside(point, BL, Defaults.AnchorDotRadius))
            {
                return Anchor.BottomLeft;
            }
            else if (GeometryHelpers.IsInside(point, TL, Defaults.AnchorDotRadius))
            {
                return Anchor.TopLeft;
            }
            else if (GeometryHelpers.IsInside(point, TR, Defaults.AnchorDotRadius))
            {
                return Anchor.TopRight;
            }
            else if (GeometryHelpers.IsInside(point, BR, Defaults.AnchorDotRadius))
            {
                return Anchor.BottomRight;
            }
            return null;
        }

        private GridSide? GetGridSide(RelativePoint point)
        {
            if (GeometryHelpers.IsInside(point, LeftMidPoint, Defaults.AnchorDotRadius))
            {
                return GridSide.Left;
            }
            else if (GeometryHelpers.IsInside(point, TopMidPoint, Defaults.AnchorDotRadius))
            {
                return GridSide.Top;
            }
            else if (GeometryHelpers.IsInside(point, RightMidPoint, Defaults.AnchorDotRadius))
            {
                return GridSide.Right;
            }
            else if (GeometryHelpers.IsInside(point, BottomMidPoint, Defaults.AnchorDotRadius))
            {
                return GridSide.Bottom;
            }
            return null;
        }

        public void PointerDown(RelativePoint point)
        {
            SelectedEditAnchor = GetPointAnchor(point);
            SelectedEditGridSide = GetGridSide(point);
        }

        public void PointerDoubleDown(RelativePoint point)
        {
            var anchor = GetPointAnchor(point);
            var gridSide = GetGridSide(point);
            if (anchor.HasValue) OpenAnchorEditWindow(anchor.Value);
            else if (gridSide.HasValue) OpenGridSideEditWindow(gridSide.Value);
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
            SelectedEditGridSide = null;
            SetPersistent();
        }

        public void SetAnchorAxis(Anchor anchor, RelativePoint point)
        {
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    BL = point;
                    DrawResult();
                    break;
                case Anchor.TopLeft:
                    TL = point;
                    DrawResult();
                    break;
                case Anchor.TopRight:
                    TR = point;
                    DrawResult();
                    break;
                case Anchor.BottomRight:
                    BR = point;
                    DrawResult();
                    break;
            }
            LeftMidPoint = GeometryHelpers.GetPoint(BL, TL, 0.5);
            TopMidPoint = GeometryHelpers.GetPoint(TL, TR, 0.5);
            RightMidPoint = GeometryHelpers.GetPoint(TR, BR, 0.5);
            BottomMidPoint = GeometryHelpers.GetPoint(BR, BL, 0.5);
        }

        public void SetPersistent()
        {
            Session.GridProjection.MaxNormWidth = Defaults.MaxNormWidth;
            Session.GridProjection.MaxNormHeight = Defaults.MaxNormHeight;
            Session.GridProjection.ReferenceAnchor = ReferencedAnchor;
            Session.GridProjection.BL = BL;
            Session.GridProjection.TL = TL;
            Session.GridProjection.TR = TR;
            Session.GridProjection.BR = BR;
            Session.GridProjection.LeftDistance = LeftDistance;
            Session.GridProjection.TopDistance = TopDistance;
            Session.GridProjection.RightDistance = RightDistance;
            Session.GridProjection.BottomDistance = BottomDistance;
        }

        public void GetPersistent()
        {
            ReferencedAnchor = Session.GridProjection.ReferenceAnchor;
            BL = Session.GridProjection.BL;
            TL = Session.GridProjection.TL;
            TR = Session.GridProjection.TR;
            BR = Session.GridProjection.BR;
            LeftDistance = Session.GridProjection.LeftDistance;
            TopDistance = Session.GridProjection.TopDistance;
            RightDistance = Session.GridProjection.RightDistance;
            BottomDistance = Session.GridProjection.BottomDistance;
            if (BL.FrameWidth == 0 || BL.FrameHeight == 0)
                BL = RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            if (TL.FrameWidth == 0 || TL.FrameHeight == 0)
                TL = RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            if (TR.FrameWidth == 0 || TR.FrameHeight == 0)
                TR = RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            if (BR.FrameWidth == 0 || BR.FrameHeight == 0)
                BR = RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            LeftMidPoint = GeometryHelpers.GetPoint(BL, TL, 0.5);
            TopMidPoint = GeometryHelpers.GetPoint(TL, TR, 0.5);
            RightMidPoint = GeometryHelpers.GetPoint(TR, BR, 0.5);
            BottomMidPoint = GeometryHelpers.GetPoint(BR, BL, 0.5);
        }
    }
}
