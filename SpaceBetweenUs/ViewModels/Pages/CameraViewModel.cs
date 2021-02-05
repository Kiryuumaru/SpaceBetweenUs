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
        private int violationCount;
        private RelativePoint bl;
        private RelativePoint tl;
        private RelativePoint tr;
        private RelativePoint br;
        private RelativePoint leftMidPoint;
        private RelativePoint topMidPoint;
        private RelativePoint rightMidPoint;
        private RelativePoint bottomMidPoint;
        private double leftDistance;
        private double topDistance;
        private double rightDistance;
        private double bottomDistance;
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
        private Point gpuTextPos;
        private Point violationTextPos;
        private IEnumerable<Human> humans = new List<Human>();
        private IEnumerable<Violation> violations = new List<Violation>();


        private RelativePoint? mouse;
        private string mousePos;


        #endregion

        public CameraViewModel(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            GetPersistent();
            Task.Run(Start);
        }

        private async void Start()
        {
            violationCount = 0;

            currentFrame = new Mat();
            resultFrame = new Mat();
            dotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.AnchorDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            innerDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.InnerDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            itemDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.ItemDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            gridDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.GridDotRadius, Defaults.MaxNormWidth, Session.FrameSource.Width);
            borderLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.BorderLineThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            itemLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.ItemLineThickness, Defaults.MaxNormWidth, Session.FrameSource.Width);
            gpuTextPos = new Point(
                GeometryHelpers.Convert(Defaults.GridEdgeOffset, Defaults.MaxNormWidth, Session.FrameSource.Width),
                GeometryHelpers.Convert(Defaults.GridEdgeOffset * 3, Defaults.MaxNormHeight, Session.FrameSource.Height));
            violationTextPos = new Point(
                GeometryHelpers.Convert(Defaults.GridEdgeOffset, Defaults.MaxNormWidth, Session.FrameSource.Width),
                GeometryHelpers.Convert(Defaults.MaxNormHeight - (Defaults.GridEdgeOffset * 2), Defaults.MaxNormHeight, Session.FrameSource.Height));

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
            var detection = Session.HumanDetector?.DetectHuman(currentFrame.ToBytes());
            if (detection.HasValue)
            {
                violations = detection.Value.Violations;
                humans = detection.Value.Humans;
            }
            else
            {
                violations = null;
                humans = null;
            }
            violationCount = humans?.Where(i => i.IsViolation).Count() ?? 0;

            //if (ViolationCount > 0)
            //{
            //    Task.Run(async delegate
            //    {
            //        while (isSpeaking) { }
            //        isSpeaking = true;
            //        var synthesizer = new SpeechSynthesizer();
            //        synthesizer.SetOutputToDefaultAudioDevice();
            //        synthesizer.Speak("Please observe social distancing");
            //        await Task.Delay(5000);
            //        isSpeaking = false;
            //    });
            //}
        }

        public void DrawResult()
        {
            resultFrame = currentFrame.Clone();

            if (gridPoints != null)
            {
                foreach (var point in gridPoints)
                {
                    Cv2.Circle(
                        resultFrame,
                        point.Frame,
                        gridDotRelativeRadius,
                        Defaults.YellowColor,
                        Cv2.FILLED);
                }
            }

            if (!bl.IsZero)
                Cv2.Circle(
                    resultFrame,
                    bl.Frame,
                    dotRelativeRadius,
                    hoveredEditAnchor == Anchor.BottomLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (!tl.IsZero)
                Cv2.Circle(
                    resultFrame,
                    tl.Frame,
                    dotRelativeRadius,
                    hoveredEditAnchor == Anchor.TopLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (!tr.IsZero)
                Cv2.Circle(
                    resultFrame,
                    tr.Frame,
                    dotRelativeRadius,
                    hoveredEditAnchor == Anchor.TopRight ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (!br.IsZero)
                Cv2.Circle(
                    resultFrame,
                    br.Frame,
                    dotRelativeRadius,
                    hoveredEditAnchor == Anchor.BottomRight ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);

            if (!bl.IsZero && !tl.IsZero && !tr.IsZero && !br.IsZero)
            {
                Cv2.Circle(
                    resultFrame,
                    leftMidPoint.Frame,
                    innerDotRelativeRadius,
                    hoveredEditGridSide == GridSide.Left ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
                Cv2.Circle(
                    resultFrame,
                    topMidPoint.Frame,
                    innerDotRelativeRadius,
                    hoveredEditGridSide == GridSide.Top ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
                Cv2.Circle(
                    resultFrame,
                    rightMidPoint.Frame,
                    innerDotRelativeRadius,
                    hoveredEditGridSide == GridSide.Right ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
                Cv2.Circle(
                    resultFrame,
                    bottomMidPoint.Frame,
                    innerDotRelativeRadius,
                    hoveredEditGridSide == GridSide.Bottom ? Defaults.GreenColor : Defaults.BlueColor,
                    Cv2.FILLED);
            }

            if (humans != null)
            {
                foreach (var item in humans)
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

            if (violations != null)
            {
                foreach (var item in violations)
                {
                    Cv2.Line(resultFrame, item.Line.A.Frame, item.Line.B.Frame, Defaults.RedColor, itemLineRelativeThickness);
                    var center = GeometryHelpers.GetPoint(item.Line, 0.5);
                    Cv2.PutText(
                        resultFrame,
                        item.Distance.ToString("0.##") + "m",
                        center.Frame,
                        HersheyFonts.HersheyPlain,
                        itemLineRelativeThickness / 2,
                        Defaults.BlueColor,
                        itemLineRelativeThickness);
                }
            }

            if (Session.HumanDetector != null)
            {
                Cv2.PutText(
                    resultFrame,
                    Session.HumanDetector.GPUMode ? "GPU ON" : "GPU OFF",
                    gpuTextPos,
                    HersheyFonts.HersheyPlain,
                    itemLineRelativeThickness,
                    Session.HumanDetector.GPUMode ? Defaults.GreenColor : Defaults.RedColor,
                    itemLineRelativeThickness * 2);
            }

            if (Session.HumanDetector != null)
            {
                Cv2.PutText(
                    resultFrame,
                    "Violation Count: " + violationCount.ToString(),
                    violationTextPos,
                    HersheyFonts.HersheyPlain,
                    itemLineRelativeThickness,
                    violationCount == 0 ? Defaults.GreenColor : Defaults.RedColor,
                    itemLineRelativeThickness * 2);
            }

            if (mouse.HasValue)
            {
                Cv2.PutText(
                    resultFrame,
                    mousePos,
                    mouse.Value.Frame,
                    HersheyFonts.HersheyPlain,
                    itemLineRelativeThickness / 2,
                    Defaults.RedColor,
                    itemLineRelativeThickness);
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
            if (GeometryHelpers.IsInside(point, leftMidPoint, Defaults.AnchorDotRadius * 2))
            {
                return GridSide.Left;
            }
            else if (GeometryHelpers.IsInside(point, topMidPoint, Defaults.AnchorDotRadius * 2))
            {
                return GridSide.Top;
            }
            else if (GeometryHelpers.IsInside(point, rightMidPoint, Defaults.AnchorDotRadius * 2))
            {
                return GridSide.Right;
            }
            else if (GeometryHelpers.IsInside(point, bottomMidPoint, Defaults.AnchorDotRadius * 2))
            {
                return GridSide.Bottom;
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
        }

        public void PointerMove(RelativePoint point)
        {
            hoveredEditAnchor = GetPointAnchor(point);
            hoveredEditGridSide = GetGridSide(point);
            if (!point.IsZero)
            {
                var pers = Session.GridProjection.Perspective(point);
                if (pers.HasValue)
                {
                    mouse = point;
                    mousePos = "x=" + pers.Value.X.ToString("0.##") + "m y=" + pers.Value.Y.ToString("0.##") + "m";
                }
                else
                {
                    mouse = null;
                }
            }
            if (selectedEditAnchor != null)
            {
                SetAnchorAxis(selectedEditAnchor.Value, point);
                SetPersistent();
                UpdateGridPoints();
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

        public void SetAnchorAxis(Anchor anchor, RelativePoint point)
        {
            switch (anchor)
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
            UpdateGridPoints();
            DrawResult();
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
