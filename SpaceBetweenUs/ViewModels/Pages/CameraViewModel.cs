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

        private bool isSpeaking = false;

        public int violationCount;
        public int ViolationCount
        {
            get => violationCount;
            set => SetProperty(ref violationCount, value);
        }

        public double originElevation;
        public double OriginElevation
        {
            get => originElevation;
            set
            {
                SetProperty(ref originElevation, value);
                SetPersistent();
                UpdateGridPoints();
            }
        }

        public double originAngle;
        public double OriginAngle
        {
            get => originAngle;
            set
            {
                SetProperty(ref originAngle, value);
                SetPersistent();
                UpdateGridPoints();
            }
        }

        public double fovAngle;
        public double FOVAngle
        {
            get => fovAngle;
            set
            {
                SetProperty(ref fovAngle, value);
                SetPersistent();
                UpdateGridPoints();
            }
        }

        public bool showProjection;
        public bool ShowProjection
        {
            get => showProjection;
            set => SetProperty(ref showProjection, value);
        }

        public string test;
        public string Test
        {
            get => test;
            set => SetProperty(ref test, value);
        }

        #endregion

        #region Properties

        private readonly Dispatcher dispatcher;
        private RelativePoint bl;
        private RelativePoint tl;
        private RelativePoint tr;
        private RelativePoint br;
        private RelativePoint leftMidPoint;
        private RelativePoint topMidPoint;
        private RelativePoint rightMidPoint;
        private RelativePoint bottomMidPoint;
        private IEnumerable<RelativePoint> gridPoints;
        private Anchor? selectedEditAnchor;
        private Mat currentFrame;
        private Mat resultFrame;
        private int dotRelativeRadius;
        private int innerDotRelativeRadius;
        private int itemDotRelativeRadius;
        private int borderLineRelativeThickness;
        private int itemLineRelativeThickness;
        private double projectionHeight;
        private Point gpuTextPos;
        private IEnumerable<Human> items = new List<Human>();


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
            gpuTextPos = new Point(
                GeometryHelpers.Convert(Defaults.GridEdgeOffset, Defaults.MaxNormWidth, Session.FrameSource.Width),
                GeometryHelpers.Convert(Defaults.GridEdgeOffset * 2, Defaults.MaxNormWidth, Session.FrameSource.Width));

            var s = new Stopwatch();
            while (true)
            {
                s.Restart();

                Session.FrameSource.ReadFrame(currentFrame);
                Detect();
                DrawResult();
                Test = projectionHeight.ToString();

                int delayMillis = (int)((1000 / Defaults.Fps) - s.ElapsedMilliseconds);
                await Task.Delay(delayMillis > 0 ? delayMillis : 0);
            }
        }

        public void Detect()
        {
            items = Session.HumanDetector?.DetectHuman(currentFrame.ToBytes());
            ViolationCount = items?.Where(i => i.IsViolation).Count() ?? 0;

            if (ViolationCount > 0)
            {
                Task.Run(async delegate
                {
                    while (isSpeaking) { }
                    isSpeaking = true;
                    var synthesizer = new SpeechSynthesizer();
                    synthesizer.SetOutputToDefaultAudioDevice();
                    synthesizer.Speak("Please observe social distancing");
                    await Task.Delay(5000);
                    isSpeaking = false;
                });
            }
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
                    Defaults.WhiteColor,
                    Cv2.FILLED);
            }

            if (!bl.IsZero)
                Cv2.Circle(
                    resultFrame,
                    bl.Frame,
                    dotRelativeRadius,
                    selectedEditAnchor == Anchor.BottomLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (!tl.IsZero)
                Cv2.Circle(
                    resultFrame,
                    tl.Frame,
                    dotRelativeRadius,
                    selectedEditAnchor == Anchor.TopLeft ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (!tr.IsZero)
                Cv2.Circle(
                    resultFrame,
                    tr.Frame,
                    dotRelativeRadius,
                    selectedEditAnchor == Anchor.TopRight ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);
            if (!br.IsZero)
                Cv2.Circle(
                    resultFrame,
                    br.Frame,
                    dotRelativeRadius,
                    selectedEditAnchor == Anchor.BottomRight ? Defaults.GreenColor : Defaults.BlueColor,
                    borderLineRelativeThickness);

            if (!bl.IsZero && !tl.IsZero && !tr.IsZero && !br.IsZero)
            {
                Cv2.Circle(
                    resultFrame,
                    topMidPoint.Frame,
                    innerDotRelativeRadius,
                    Defaults.BlueColor,
                    Cv2.FILLED);
                Cv2.Circle(
                    resultFrame,
                    bottomMidPoint.Frame,
                    innerDotRelativeRadius,
                    Defaults.BlueColor,
                    Cv2.FILLED);
                Cv2.Circle(
                    resultFrame,
                    leftMidPoint.Frame,
                    innerDotRelativeRadius,
                    Defaults.BlueColor,
                    Cv2.FILLED);
                Cv2.Circle(
                    resultFrame,
                    rightMidPoint.Frame,
                    innerDotRelativeRadius,
                    Defaults.BlueColor,
                    Cv2.FILLED);
            }

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

            Cv2.PutText(
                resultFrame,
                Session.HumanDetector.GPUMode ? "GPU ON" : "GPU OFF",
                gpuTextPos,
                HersheyFonts.HersheyPlain,
                itemLineRelativeThickness,
                Session.HumanDetector.GPUMode ? Defaults.GreenColor : Defaults.RedColor,
                itemLineRelativeThickness * 2);

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
        {;
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

        public void PointerDown(RelativePoint point)
        {
            selectedEditAnchor = GetPointAnchor(point);
        }

        public void PointerDoubleDown(RelativePoint point)
        {
            var anchor = GetPointAnchor(point);
            if (anchor.HasValue) OpenAnchorEditWindow(anchor.Value);
        }

        public void PointerMove(RelativePoint point)
        {
            if (selectedEditAnchor != null)
            {
                SetAnchorAxis(selectedEditAnchor.Value, point);
            }
        }

        public void PointerUp()
        {
            selectedEditAnchor = null;
            SetPersistent();
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
            Session.GridProjection.OriginElevation = OriginElevation;
            Session.GridProjection.OriginAngle = OriginAngle;
            Session.GridProjection.FOVAngle = FOVAngle;
        }

        public void GetPersistent()
        {
            bl = Session.GridProjection.BL;
            tl = Session.GridProjection.TL;
            tr = Session.GridProjection.TR;
            br = Session.GridProjection.BR;
            originElevation = Session.GridProjection.OriginElevation;
            originAngle = Session.GridProjection.OriginAngle;
            fovAngle = Session.GridProjection.FOVAngle;
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
            projectionHeight = Session.GridProjection.RealFOVHeight;
            if (OriginElevation != 0)
            {

            }
            if (OriginAngle != 0)
            {

            }
            if (OriginElevation != 0 && OriginAngle != 0 && FOVAngle != 0)
            {

            }
        }
    }
}
