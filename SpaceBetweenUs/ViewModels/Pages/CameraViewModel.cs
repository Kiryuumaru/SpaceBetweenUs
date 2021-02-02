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
        public Anchor? SelectedEditAnchor;
        public GridSide? SelectedEditGridSide;
        private readonly Dispatcher dispatcher;
        private Mat currentFrame;
        private Mat resultFrame;
        private int dotRelativeRadius;
        private int innerDotRelativeRadius;
        private int itemDotRelativeRadius;
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

        private RelativePoint bl;
        private RelativePoint tl;
        private RelativePoint tr;
        private RelativePoint br;
        private double leftDistance;
        private double topDistance;
        private double rightDistance;
        private double bottomDistance;
        private RelativePoint leftMidPoint;
        private RelativePoint topMidPoint;
        private RelativePoint rightMidPoint;
        private RelativePoint bottomMidPoint;
        private List<RelativePoint> gridPoints;

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
            itemDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.ItemDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
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

            foreach (var point in gridPoints)
            {
                Cv2.Circle(
                    resultFrame,
                    point.Frame,
                    itemDotRelativeRadius,
                    Defaults.YellowColor,
                    Cv2.FILLED);
            }

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
                    leftMidPoint.Frame,
                    innerDotRelativeRadius,
                    SelectedEditGridSide == GridSide.Left ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
            if (tl.Frame.X != 0 || tl.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    topMidPoint.Frame,
                    innerDotRelativeRadius,
                    SelectedEditGridSide == GridSide.Top ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
            if (tr.Frame.X != 0 || tr.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    rightMidPoint.Frame,
                    innerDotRelativeRadius,
                    SelectedEditGridSide == GridSide.Right ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
            if (br.Frame.X != 0 || br.Frame.Y != 0)
                Cv2.Circle(
                    resultFrame,
                    bottomMidPoint.Frame,
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
                        itemDotRelativeRadius,
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
            if (GeometryHelpers.IsInside(point, bl, Defaults.AnchorDotRadius))
            {
                return Anchor.BottomLeft;
            }
            else if (GeometryHelpers.IsInside(point, tl, Defaults.AnchorDotRadius))
            {
                return Anchor.TopLeft;
            }
            else if (GeometryHelpers.IsInside(point, tr, Defaults.AnchorDotRadius))
            {
                return Anchor.TopRight;
            }
            else if (GeometryHelpers.IsInside(point, br, Defaults.AnchorDotRadius))
            {
                return Anchor.BottomRight;
            }
            return null;
        }

        private GridSide? GetGridSide(RelativePoint point)
        {
            if (GeometryHelpers.IsInside(point, leftMidPoint, Defaults.AnchorDotRadius))
            {
                return GridSide.Left;
            }
            else if (GeometryHelpers.IsInside(point, topMidPoint, Defaults.AnchorDotRadius))
            {
                return GridSide.Top;
            }
            else if (GeometryHelpers.IsInside(point, rightMidPoint, Defaults.AnchorDotRadius))
            {
                return GridSide.Right;
            }
            else if (GeometryHelpers.IsInside(point, bottomMidPoint, Defaults.AnchorDotRadius))
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
                    bl = point;
                    DrawResult();
                    break;
                case Anchor.TopLeft:
                    tl = point;
                    DrawResult();
                    break;
                case Anchor.TopRight:
                    tr = point;
                    DrawResult();
                    break;
                case Anchor.BottomRight:
                    br = point;
                    DrawResult();
                    break;
            }
            UpdateGridPoints();
        }

        public void SetPersistent()
        {
            Session.GridProjection.MaxNormWidth = Defaults.MaxNormWidth;
            Session.GridProjection.MaxNormHeight = Defaults.MaxNormHeight;
            Session.GridProjection.BL = bl;
            Session.GridProjection.TL = tl;
            Session.GridProjection.TR = tr;
            Session.GridProjection.BR = br;
            Session.GridProjection.LeftDistance = leftDistance;
            Session.GridProjection.TopDistance = topDistance;
            Session.GridProjection.RightDistance = rightDistance;
            Session.GridProjection.BottomDistance = bottomDistance;
        }

        public void GetPersistent()
        {
            bl = Session.GridProjection.BL;
            tl = Session.GridProjection.TL;
            tr = Session.GridProjection.TR;
            br = Session.GridProjection.BR;
            leftDistance = Session.GridProjection.LeftDistance;
            topDistance = Session.GridProjection.TopDistance;
            rightDistance = Session.GridProjection.RightDistance;
            bottomDistance = Session.GridProjection.BottomDistance;
            if (bl.FrameWidth == 0 || bl.FrameHeight == 0)
                bl = RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            if (tl.FrameWidth == 0 || tl.FrameHeight == 0)
                tl = RelativePoint.FromNorm(new Point(Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            if (tr.FrameWidth == 0 || tr.FrameHeight == 0)
                tr = RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            if (br.FrameWidth == 0 || br.FrameHeight == 0)
                br = RelativePoint.FromNorm(new Point(Defaults.MaxNormWidth - Defaults.GridEdgeOffset, Defaults.MaxNormHeight - Defaults.GridEdgeOffset), Session.FrameSource.Width, Session.FrameSource.Height);
            UpdateGridPoints();
        }

        private void UpdateGridPoints()
        {
            leftMidPoint = GeometryHelpers.GetPoint(bl, tl, 0.5);
            topMidPoint = GeometryHelpers.GetPoint(tl, tr, 0.5);
            rightMidPoint = GeometryHelpers.GetPoint(tr, br, 0.5);
            bottomMidPoint = GeometryHelpers.GetPoint(br, bl, 0.5);
            gridPoints = new List<RelativePoint>();
            if (leftDistance != 0)
            {
                double currentDist = 0;
                while (leftDistance > (currentDist + Defaults.GridNotchDistance))
                {
                    currentDist += Defaults.GridNotchDistance;
                    gridPoints.Add(GeometryHelpers.GetPoint(bl, tl, currentDist / leftDistance));
                }
            }
            if (topDistance != 0)
            {
                double currentDist = 0;
                while (topDistance > (currentDist + Defaults.GridNotchDistance))
                {
                    currentDist += Defaults.GridNotchDistance;
                    gridPoints.Add(GeometryHelpers.GetPoint(tl, tr, currentDist / topDistance));
                }
            }
            if (rightDistance != 0)
            {
                double currentDist = 0;
                while (rightDistance > (currentDist + Defaults.GridNotchDistance))
                {
                    currentDist += Defaults.GridNotchDistance;
                    gridPoints.Add(GeometryHelpers.GetPoint(br, tr, currentDist / rightDistance));
                }
            }
            if (bottomDistance != 0)
            {
                double currentDist = 0;
                while (bottomDistance > (currentDist + Defaults.GridNotchDistance))
                {
                    currentDist += Defaults.GridNotchDistance;
                    gridPoints.Add(GeometryHelpers.GetPoint(bl, br, currentDist / bottomDistance));
                }
            }
        }
    }
}
