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
using System.Speech.Synthesis;
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
        #region ViewBindings

        private ImageSource frame;
        public ImageSource Frame
        {
            get => frame;
            set => SetProperty(ref frame, value);
        }

        #endregion

        #region Properties

        private readonly Dispatcher dispatcher;
        private int violationsCount;
        private int violatorsCount;
        private int lastViolatorsCount;
        private RelativePoint bl;
        private RelativePoint tl;
        private RelativePoint tr;
        private RelativePoint br;
        private RelativePoint leftMidPoint;
        private RelativePoint topMidPoint;
        private RelativePoint rightMidPoint;
        private RelativePoint bottomMidPoint;
        private double leftRightDistance;
        private double topBottomDistance;
        private IEnumerable<RelativePoint> gridPoints;
        private Anchor? selectedEditAnchor;
        private GridSide? selectedEditGridSide;
        private Anchor? hoveredEditAnchor;
        private GridSide? hoveredEditGridSide;
        private Mat currentFrame;
        private Mat resultFrame;

        private int dotRelativeRadius;
        private int innerDotRelativeRadius;
        private int itemDotRelativeRadius;
        private int gridDotRelativeRadius;

        private int borderLineRelativeThickness;
        private int itemLineRelativeThickness;

        private int largeTextFontRelativeSize;
        private int normalTextFontRelativeSize;
        private int smallTextFontRelativeSize;
        private int largeTextFontRelativeThickness;
        private int normalTextFontRelativeThickness;
        private int smallTextFontRelativeThickness;

        private Point gpuTextPos;
        private Point violationTextPos;
        private Point violatorsTextPos;
        private IEnumerable<Human> humans = new List<Human>();
        private IEnumerable<Violation> violations = new List<Violation>();

        private RelativePoint mousePos;

        private bool logLastFrame = false;


        #endregion

        public CameraViewModel(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            GetPersistent();
            Start();
        }

        private void Start()
        {
            violationsCount = 0;
            violatorsCount = 0;
            lastViolatorsCount = 0;

            currentFrame = new Mat();
            resultFrame = new Mat();
            dotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.AnchorDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            innerDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.InnerDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            itemDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.ItemDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            gridDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.GridDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            borderLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.BorderLineThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            itemLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.ItemLineThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            largeTextFontRelativeSize = (int)GeometryHelpers.Convert(Defaults.LargeTextFontSize, Defaults.MaxNormWidth, Session.FrameSource.Width);
            normalTextFontRelativeSize = (int)GeometryHelpers.Convert(Defaults.NormalTextFontSize, Defaults.MaxNormWidth, Session.FrameSource.Width);
            smallTextFontRelativeSize = (int)GeometryHelpers.Convert(Defaults.SmallTextFontSize, Defaults.MaxNormWidth, Session.FrameSource.Width);
            largeTextFontRelativeThickness = (int)GeometryHelpers.Convert(Defaults.LargeTextFontThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            normalTextFontRelativeThickness = (int)GeometryHelpers.Convert(Defaults.NormalTextFontThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            smallTextFontRelativeThickness = (int)GeometryHelpers.Convert(Defaults.SmallTextFontThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);

            gpuTextPos = new Point(
                GeometryHelpers.Convert(Defaults.GPUTextXPos, Defaults.MaxNormWidth, Session.FrameSource.Width),
                GeometryHelpers.Convert(Defaults.GPUTextYPos, Defaults.MaxNormHeight, Session.FrameSource.Height));
            violationTextPos = new Point(
                GeometryHelpers.Convert(Defaults.ViolationTextXPos, Defaults.MaxNormWidth, Session.FrameSource.Width),
                GeometryHelpers.Convert(Defaults.ViolationTextYPos, Defaults.MaxNormHeight, Session.FrameSource.Height));
            violatorsTextPos = new Point(
                GeometryHelpers.Convert(Defaults.ViolatorsTextXPos, Defaults.MaxNormWidth, Session.FrameSource.Width),
                GeometryHelpers.Convert(Defaults.ViolatorsTextYPos, Defaults.MaxNormHeight, Session.FrameSource.Height));

            var s = new Stopwatch();

            Task.Run(async delegate
            {
                while (true)
                {
                    s.Restart();

                    Session.FrameSource.ReadFrame(currentFrame);
                    Detect();

                    int delayMillis = (int)((1000 / Defaults.Fps) - s.ElapsedMilliseconds);
                    await Task.Delay(delayMillis > 0 ? delayMillis : 0);
                }
            });

            Task.Run(async delegate
            {
                while (true)
                {
                    s.Restart();
                    DrawResult();
                    int delayMillis = (int)((1000 / Defaults.Fps) - s.ElapsedMilliseconds);
                    await Task.Delay(delayMillis > 0 ? delayMillis : 0);
                }
            });
        }

        public void Detect()
        {
            var detection = Session.HumanDetector?.DetectHuman(currentFrame.ToBytes());
            violations = detection.HasValue ? detection.Value.Violations : null;
            humans = detection.HasValue ? detection.Value.Humans : null;
            violationsCount = violations?.Count() ?? 0;
            violatorsCount = humans?.Where(i => i.IsViolation).Count() ?? 0;

            if (violationsCount > 0)
            {
                if (violatorsCount != lastViolatorsCount)
                {
                    logLastFrame = true;
                }
            }

            lastViolatorsCount = violatorsCount;
        }

        public void DrawResult()
        {
            resultFrame = currentFrame.Clone();

            if (gridPoints != null)
            {
                foreach (var point in gridPoints)
                {
                    if (!(humans?.Any(i => GeometryHelpers.IsInside(point, i.BL, i.TL, i.TR, i.BR)) ?? false))
                        Cv2.Circle(resultFrame, point.Frame, gridDotRelativeRadius, Defaults.YellowColor, Cv2.FILLED);
                }
            }

            if (!(humans?.Any(i => GeometryHelpers.IsInside(bl, i.BL, i.TL, i.TR, i.BR)) ?? false))
                Cv2.Circle(resultFrame, bl.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.BottomLeft ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            else if (hoveredEditAnchor == Anchor.BottomLeft)
                Cv2.Circle(resultFrame, bl.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);
            if (!(humans?.Any(i => GeometryHelpers.IsInside(tl, i.BL, i.TL, i.TR, i.BR)) ?? false))
                Cv2.Circle(resultFrame, tl.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.TopLeft ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            else if (hoveredEditAnchor == Anchor.TopLeft)
                Cv2.Circle(resultFrame, tl.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);
            if (!(humans?.Any(i => GeometryHelpers.IsInside(tr, i.BL, i.TL, i.TR, i.BR)) ?? false))
                Cv2.Circle(resultFrame, tr.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.TopRight ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            else if (hoveredEditAnchor == Anchor.TopRight)
                Cv2.Circle(resultFrame, tr.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);
            if (!(humans?.Any(i => GeometryHelpers.IsInside(br, i.BL, i.TL, i.TR, i.BR)) ?? false))
                Cv2.Circle(resultFrame, br.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.BottomRight ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            else if (hoveredEditAnchor == Anchor.BottomRight)
                Cv2.Circle(resultFrame, br.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);

            if (hoveredEditGridSide == GridSide.LeftRight)
            {
                Cv2.Circle(resultFrame, leftMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Circle(resultFrame, rightMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Line(resultFrame, bl.Frame, tl.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.Line(resultFrame, br.Frame, tr.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.PutText(resultFrame, leftRightDistance.ToString("0.##") + "m", leftMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                Cv2.PutText(resultFrame, leftRightDistance.ToString("0.##") + "m", rightMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(topMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, topMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(bottomMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, bottomMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
            }
            else if (hoveredEditGridSide == GridSide.TopBottom)
            {
                Cv2.Circle(resultFrame, topMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Circle(resultFrame, bottomMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Line(resultFrame, tl.Frame, tr.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.Line(resultFrame, bl.Frame, br.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.PutText(resultFrame, topBottomDistance.ToString("0.##") + "m", topMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                Cv2.PutText(resultFrame, topBottomDistance.ToString("0.##") + "m", bottomMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(leftMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, leftMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(rightMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, rightMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
            }
            else
            {
                if (!(humans?.Any(i => GeometryHelpers.IsInside(leftMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, leftMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(topMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, topMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(rightMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, rightMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(bottomMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    Cv2.Circle(resultFrame, bottomMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
            }

            if (humans != null)
            {
                foreach (var item in humans)
                {
                    Cv2.Line(resultFrame, item.BL.Frame, item.TL.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Line(resultFrame, item.TL.Frame, item.TR.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Line(resultFrame, item.TR.Frame, item.BR.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Line(resultFrame, item.BR.Frame, item.BL.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                    Cv2.Circle(resultFrame, item.BottomCenter.Frame, itemDotRelativeRadius, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, Cv2.FILLED);
                }
            }

            if (violations != null)
            {
                foreach (var item in violations)
                {
                    Cv2.Line(resultFrame, item.Line.A.Frame, item.Line.B.Frame, Defaults.RedColor, itemLineRelativeThickness);
                    var center = GeometryHelpers.GetPoint(item.Line, 0.5);
                    Cv2.PutText(resultFrame, item.Distance.ToString("0.##") + "m", center.Frame, HersheyFonts.HersheyPlain, smallTextFontRelativeSize, Defaults.BlueColor, smallTextFontRelativeThickness);
                }
            }

            if (Session.HumanDetector != null)
            {
                Cv2.PutText(resultFrame, Session.HumanDetector.GPUMode ? "GPU ON" : "GPU OFF", gpuTextPos, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Session.HumanDetector.GPUMode ? Defaults.GreenColor : Defaults.RedColor, normalTextFontRelativeThickness);
                if (GeometryHelpers.IsInside(mousePos.Norm, Defaults.ViolationThresAreaBL, Defaults.ViolationThresAreaTL, Defaults.ViolationThresAreaTR, Defaults.ViolationThresAreaBR))
                {
                    Cv2.PutText(resultFrame, "Violations Count: " + violationsCount.ToString(), violationTextPos, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.GreenColor, normalTextFontRelativeThickness);
                    Cv2.PutText(resultFrame, "Violators Count: " + violatorsCount.ToString(), violatorsTextPos, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.GreenColor, normalTextFontRelativeThickness);
                }
                else
                {
                    Cv2.PutText(resultFrame, "Violations Count: " + violationsCount.ToString(), violationTextPos, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, violationsCount == 0 ? Defaults.WhiteColor : Defaults.RedColor, normalTextFontRelativeThickness);
                    Cv2.PutText(resultFrame, "Violators Count: " + violatorsCount.ToString(), violatorsTextPos, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, violatorsCount == 0 ? Defaults.WhiteColor : Defaults.RedColor, normalTextFontRelativeThickness);
                }
            }

            if (logLastFrame)
            {
                logLastFrame = false;
                Session.Logger.SetViolationLog(resultFrame, violationsCount, violatorsCount);
            }

            try
            {
                dispatcher.Invoke(delegate
                {
                    try
                    {
                        Frame = resultFrame.ToWriteableBitmap(PixelFormats.Bgr24);
                    }
                    catch { }
                });
            }
            catch { }
        }

        private void OpenAnchorEditWindow(Anchor anchor)
        {
            var editor = new AnchorPointEdit(anchor);
            var dlg = new ModernDialog
            {
                Title = "Anchor Edit",
                Content = editor,
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
                editor.Save();
                GetPersistent();
            }
        }

        private void OpenGridSideEditWindow(GridSide side)
        {
            var editor = new GridSizeEdit(side);
            var dlg = new ModernDialog
            {
                Title = "Grid Side Edit",
                Content = editor,
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
                editor.Save();
                GetPersistent();
            }
        }

        private void OpenViolationThresEditWindow()
        {
            var editor = new ViolationThresEdit();
            var dlg = new ModernDialog
            {
                Title = "Violation Threshold",
                Content = editor,
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
                editor.Save();
                GetPersistent();
            }
        }

        private Anchor? GetPointAnchor(RelativePoint point)
        {
            if (GeometryHelpers.IsInside(point, bl, Defaults.AnchorDotRadius * 2))
            {
                return Anchor.BottomLeft;
            }
            else if (GeometryHelpers.IsInside(point, tl, Defaults.AnchorDotRadius * 2))
            {
                return Anchor.TopLeft;
            }
            else if (GeometryHelpers.IsInside(point, tr, Defaults.AnchorDotRadius * 2))
            {
                return Anchor.TopRight;
            }
            else if (GeometryHelpers.IsInside(point, br, Defaults.AnchorDotRadius * 2))
            {
                return Anchor.BottomRight;
            }
            return null;
        }

        private GridSide? GetGridSide(RelativePoint point)
        {
            if (GeometryHelpers.IsInside(point, topMidPoint, Defaults.AnchorDotRadius * 2) ||
                GeometryHelpers.IsInside(point, bottomMidPoint, Defaults.AnchorDotRadius * 2))
            {
                return GridSide.TopBottom;
            }
            else if (GeometryHelpers.IsInside(point, leftMidPoint, Defaults.AnchorDotRadius * 2) ||
                GeometryHelpers.IsInside(point, rightMidPoint, Defaults.AnchorDotRadius * 2))
            {
                return GridSide.LeftRight;
            }
            return null;
        }

        public void PointerDown(RelativePoint point)
        {
            selectedEditAnchor = GetPointAnchor(point);
            selectedEditGridSide = GetGridSide(point);
        }

        public void PointerDoubleDown(RelativePoint point)
        {
            var anchor = GetPointAnchor(point);
            var side = GetGridSide(point);
            if (anchor.HasValue) OpenAnchorEditWindow(anchor.Value);
            else if (side.HasValue) OpenGridSideEditWindow(side.Value);
            else if (GeometryHelpers.IsInside(point.Norm, Defaults.ViolationThresAreaBL, Defaults.ViolationThresAreaTL, Defaults.ViolationThresAreaTR, Defaults.ViolationThresAreaBR))
            {
                OpenViolationThresEditWindow();
            }
        }

        public void PointerMove(RelativePoint point)
        {
            hoveredEditAnchor = GetPointAnchor(point);
            hoveredEditGridSide = GetGridSide(point);
            mousePos = point;

            if (selectedEditAnchor != null)
            {
                if (HasAnchorAxisChanges(selectedEditAnchor.Value, point))
                {
                    switch (selectedEditAnchor.Value)
                    {
                        case Anchor.BottomLeft:
                            bl = point;
                            break;
                        case Anchor.TopLeft:
                            tl = point;
                            break;
                        case Anchor.TopRight:
                            tr = point;
                            break;
                        case Anchor.BottomRight:
                            br = point;
                            break;
                    }
                    SetPersistent();
                    UpdateGridPoints();
                }
            }
        }

        public void PointerUp()
        {
            if (selectedEditAnchor != null)
            {
                selectedEditAnchor = null;
            }
            if (selectedEditGridSide != null)
            {
                selectedEditGridSide = null;
            }
        }

        public bool HasAnchorAxisChanges(Anchor anchor, RelativePoint point)
        {
            switch (anchor)
            {
                case Anchor.BottomLeft:
                    return bl != point;
                case Anchor.TopLeft:
                    return tl != point;
                case Anchor.TopRight:
                    return tr != point;
                case Anchor.BottomRight:
                    return br != point;
            }
            return false;
        }

        public void SetPersistent()
        {
            Session.GridProjection.MaxNormWidth = Defaults.MaxNormWidth;
            Session.GridProjection.MaxNormHeight = Defaults.MaxNormHeight;
            Session.GridProjection.BL = bl;
            Session.GridProjection.TL = tl;
            Session.GridProjection.TR = tr;
            Session.GridProjection.BR = br;
            Session.GridProjection.TopBottomDistance = topBottomDistance;
            Session.GridProjection.LeftRightDistance = leftRightDistance;
        }

        public void GetPersistent()
        {
            bl = Session.GridProjection.BL;
            tl = Session.GridProjection.TL;
            tr = Session.GridProjection.TR;
            br = Session.GridProjection.BR;
            topBottomDistance = Session.GridProjection.TopBottomDistance;
            leftRightDistance = Session.GridProjection.LeftRightDistance;
            UpdateGridPoints();
        }

        private void UpdateGridPoints()
        {
            leftMidPoint = Session.GridProjection.LeftMidPoint;
            topMidPoint = Session.GridProjection.TopMidPoint;
            rightMidPoint = Session.GridProjection.RightMidPoint;
            bottomMidPoint = Session.GridProjection.BottomMidPoint;
            gridPoints = Session.GridProjection.GetGridPoints();
        }
    }
}
