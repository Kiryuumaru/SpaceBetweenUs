using FirstFloor.ModernUI.Windows.Controls;
using MvvmHelpers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SpaceBetweenUs.Model;
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
    public class StreamViewViewModel : BaseViewModel
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

        private readonly TimeSpan logDebounce = TimeSpan.FromSeconds(10);

        private readonly Session session;
        private readonly Dispatcher dispatcher;
        private bool isStopped = false;
        private int violationsCount;
        private int lastViolationsCount;
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
        private Mat processedFrame;
        private Mat currentFrame;
        private Mat resultFrame;
        private OpenCvSharp.Size resultSize;

        private int dotRelativeRadius;
        private int innerDotRelativeRadius;
        private int itemDotRelativeRadius;
        private int gridDotRelativeRadius;

        private int borderLineRelativeThickness;
        private int itemLineRelativeThickness;

        private double largeTextFontRelativeSize;
        private double normalTextFontRelativeSize;
        private double smallTextFontRelativeSize;
        private int largeTextFontRelativeThickness;
        private int normalTextFontRelativeThickness;
        private int smallTextFontRelativeThickness;

        private Point violationTextPos;
        private IEnumerable<Human> humans = new List<Human>();
        private IEnumerable<HumanDistance> humanDistance = new List<HumanDistance>();

        private RelativePoint mousePos;

        private bool logLastFrame = false;
        private DateTime? lastLog = null;

        private SpeechSynthesizer synth;

        #endregion

        public StreamViewViewModel(Session session, Dispatcher dispatcher)
        {
            this.session = session;
            this.dispatcher = dispatcher;
            GetPersistent();
            Start();
        }

        private void Start()
        {
            violationsCount = 0;
            lastViolationsCount = 0;

            resultSize = new OpenCvSharp.Size(
                Defaults.DesirableWidth,
                (session.FrameSource.Height / session.FrameSource.Width) * Defaults.DesirableWidth);

            synth = new SpeechSynthesizer();
            resultFrame = new Mat();
            processedFrame = new Mat();
            currentFrame = new Mat();
            dotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.AnchorDotRadius, Defaults.MaxNormWidth, session.FrameSource.Width);
            innerDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.InnerDotRadius, Defaults.MaxNormWidth, session.FrameSource.Width);
            itemDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.ItemDotRadius, Defaults.MaxNormWidth, session.FrameSource.Width);
            gridDotRelativeRadius = (int)GeometryHelpers.Convert(Defaults.GridDotRadius, Defaults.MaxNormWidth, session.FrameSource.Width);
            borderLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.BorderLineThickness, Defaults.MaxNormWidth, session.FrameSource.Width);
            itemLineRelativeThickness = (int)GeometryHelpers.Convert(Defaults.ItemLineThickness, Defaults.MaxNormWidth, session.FrameSource.Width);
            largeTextFontRelativeSize = GeometryHelpers.Convert(Defaults.LargeTextFontSize, Defaults.MaxNormWidth, session.FrameSource.Width);
            normalTextFontRelativeSize = GeometryHelpers.Convert(Defaults.NormalTextFontSize, Defaults.MaxNormWidth, session.FrameSource.Width);
            smallTextFontRelativeSize = GeometryHelpers.Convert(Defaults.SmallTextFontSize, Defaults.MaxNormWidth, session.FrameSource.Width);
            largeTextFontRelativeThickness = (int)GeometryHelpers.Convert(Defaults.LargeTextFontThickness, Defaults.MaxNormWidth, session.FrameSource.Width);
            normalTextFontRelativeThickness = (int)GeometryHelpers.Convert(Defaults.NormalTextFontThickness, Defaults.MaxNormWidth, session.FrameSource.Width);
            smallTextFontRelativeThickness = (int)GeometryHelpers.Convert(Defaults.SmallTextFontThickness, Defaults.MaxNormWidth, session.FrameSource.Width);

            synth.SetOutputToDefaultAudioDevice();

            violationTextPos = new Point(
                GeometryHelpers.Convert(Defaults.ViolationTextXPos, Defaults.MaxNormWidth, session.FrameSource.Width),
                GeometryHelpers.Convert(Defaults.ViolationTextYPos, Defaults.MaxNormHeight, session.FrameSource.Height));
            
            Stopwatch s = new Stopwatch();

            StartDetection();

            _ = Task.Run(async delegate
              {
                  while (true)
                  {
                      if (isStopped)
                      {
                          return;
                      }

                      s.Restart();

                      try
                      {
                          DrawResult();
                      }
                      catch { }

                      int delayMillis = (int)((1000 / Defaults.Fps) - s.ElapsedMilliseconds);
                      await Task.Delay(delayMillis > 0 ? delayMillis : 0);
                  }
              });
        }

        public void Stop()
        {
            isStopped = true;
            session.Stop();
        }

        public void StartDetection()
        {
            Task.Run(async delegate
            {
                var s = new Stopwatch();
                while (true)
                {
                    try
                    {
                        if (isStopped) break;
                        s.Restart();

                        if (!await session.FrameSource.TryReadFrame(currentFrame)) break;

                        (humanDistance, humans) = await session.HumanDetector.Detect(currentFrame);
                        violationsCount = humanDistance?.Where(i => i.IsViolation).Count() ?? 0;

                        if (violationsCount > 0)
                        {
                            if (lastLog == null ||
                                (lastLog.Value + logDebounce < DateTime.Now))
                            {
                                lastLog = DateTime.Now;
                                logLastFrame = true;
                            }
                        }

                        lastViolationsCount = violationsCount;

                        lock (currentFrame)
                        {
                            processedFrame = currentFrame.Clone();
                        }
                        lock (resultFrame)
                        {
                            session.FrameSource.OutputFrame(resultFrame);
                        }

                        int delayMillis = (int)((1000 / Defaults.Fps) - s.ElapsedMilliseconds);
                        await Task.Delay(delayMillis > 0 ? delayMillis : 0);
                    }
                    catch { }
                }
                session.FrameSource.OutputFrameEnd();
            }).ConfigureAwait(false);
        }

        public void DrawResult()
        {
            if (processedFrame == null)
            {
                return;
            }

            resultFrame = processedFrame.Clone();

            if (gridPoints != null)
            {
                foreach (RelativePoint point in gridPoints)
                {
                    if (!(humans?.Any(i => GeometryHelpers.IsInside(point, i.BL, i.TL, i.TR, i.BR)) ?? false))
                    {
                        Cv2.Circle(resultFrame, point.Frame, gridDotRelativeRadius, Defaults.YellowColor, Cv2.FILLED);
                    }
                }
            }

            if (!(humans?.Any(i => GeometryHelpers.IsInside(bl, i.BL, i.TL, i.TR, i.BR)) ?? false))
            {
                Cv2.Circle(resultFrame, bl.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.BottomLeft ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            }
            else if (hoveredEditAnchor == Anchor.BottomLeft)
            {
                Cv2.Circle(resultFrame, bl.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);
            }

            if (!(humans?.Any(i => GeometryHelpers.IsInside(tl, i.BL, i.TL, i.TR, i.BR)) ?? false))
            {
                Cv2.Circle(resultFrame, tl.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.TopLeft ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            }
            else if (hoveredEditAnchor == Anchor.TopLeft)
            {
                Cv2.Circle(resultFrame, tl.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);
            }

            if (!(humans?.Any(i => GeometryHelpers.IsInside(tr, i.BL, i.TL, i.TR, i.BR)) ?? false))
            {
                Cv2.Circle(resultFrame, tr.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.TopRight ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            }
            else if (hoveredEditAnchor == Anchor.TopRight)
            {
                Cv2.Circle(resultFrame, tr.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);
            }

            if (!(humans?.Any(i => GeometryHelpers.IsInside(br, i.BL, i.TL, i.TR, i.BR)) ?? false))
            {
                Cv2.Circle(resultFrame, br.Frame, dotRelativeRadius, hoveredEditAnchor == Anchor.BottomRight ? Defaults.GreenColor : Defaults.BlueColor, borderLineRelativeThickness);
            }
            else if (hoveredEditAnchor == Anchor.BottomRight)
            {
                Cv2.Circle(resultFrame, br.Frame, dotRelativeRadius, Defaults.GreenColor, borderLineRelativeThickness);
            }

            if (hoveredEditGridSide == GridSide.LeftRight)
            {
                Cv2.Circle(resultFrame, leftMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Circle(resultFrame, rightMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Line(resultFrame, bl.Frame, tl.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.Line(resultFrame, br.Frame, tr.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.PutText(resultFrame, leftRightDistance.ToString("0.##") + session.GridProjection.Unit, leftMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                Cv2.PutText(resultFrame, leftRightDistance.ToString("0.##") + session.GridProjection.Unit, rightMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(topMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, topMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }

                if (!(humans?.Any(i => GeometryHelpers.IsInside(bottomMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, bottomMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }
            }
            else if (hoveredEditGridSide == GridSide.TopBottom)
            {
                Cv2.Circle(resultFrame, topMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Circle(resultFrame, bottomMidPoint.Frame, innerDotRelativeRadius, Defaults.GreenColor, Cv2.FILLED);
                Cv2.Line(resultFrame, tl.Frame, tr.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.Line(resultFrame, bl.Frame, br.Frame, Defaults.GreenColor, borderLineRelativeThickness);
                Cv2.PutText(resultFrame, topBottomDistance.ToString("0.##") + session.GridProjection.Unit, topMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                Cv2.PutText(resultFrame, topBottomDistance.ToString("0.##") + session.GridProjection.Unit, bottomMidPoint.Frame, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.BlueColor, normalTextFontRelativeThickness);
                if (!(humans?.Any(i => GeometryHelpers.IsInside(leftMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, leftMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }

                if (!(humans?.Any(i => GeometryHelpers.IsInside(rightMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, rightMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }
            }
            else
            {
                if (!(humans?.Any(i => GeometryHelpers.IsInside(leftMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, leftMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }

                if (!(humans?.Any(i => GeometryHelpers.IsInside(topMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, topMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }

                if (!(humans?.Any(i => GeometryHelpers.IsInside(rightMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, rightMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }

                if (!(humans?.Any(i => GeometryHelpers.IsInside(bottomMidPoint, i.BL, i.TL, i.TR, i.BR)) ?? false))
                {
                    Cv2.Circle(resultFrame, bottomMidPoint.Frame, innerDotRelativeRadius, Defaults.BlueColor, Cv2.FILLED);
                }
            }

            if (humans != null)
            {
                foreach (Human item in humans)
                {
                    if (session.GridProjection.IsProjectionReady)
                    {
                        Cv2.Line(resultFrame, item.BL.Frame, item.TL.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                        Cv2.Line(resultFrame, item.TL.Frame, item.TR.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                        Cv2.Line(resultFrame, item.TR.Frame, item.BR.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                        Cv2.Line(resultFrame, item.BR.Frame, item.BL.Frame, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, itemLineRelativeThickness);
                        Cv2.Circle(resultFrame, item.BottomCenter.Frame, itemDotRelativeRadius, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, Cv2.FILLED);
                        Cv2.PutText(resultFrame, "Loss: " + (item.Loss * 100).ToString("0.#"), item.TL.Frame, HersheyFonts.HersheyPlain, smallTextFontRelativeSize, item.IsViolation ? Defaults.RedColor : Defaults.GreenColor, smallTextFontRelativeThickness);
                    }
                    else
                    {
                        Cv2.Line(resultFrame, item.BL.Frame, item.TL.Frame, Defaults.YellowColor, itemLineRelativeThickness);
                        Cv2.Line(resultFrame, item.TL.Frame, item.TR.Frame, Defaults.YellowColor, itemLineRelativeThickness);
                        Cv2.Line(resultFrame, item.TR.Frame, item.BR.Frame, Defaults.YellowColor, itemLineRelativeThickness);
                        Cv2.Line(resultFrame, item.BR.Frame, item.BL.Frame, Defaults.YellowColor, itemLineRelativeThickness);
                        Cv2.Circle(resultFrame, item.BottomCenter.Frame, itemDotRelativeRadius, Defaults.YellowColor, Cv2.FILLED);
                        Cv2.PutText(resultFrame, "Loss: " + (item.Loss * 100).ToString("0.#"), item.TL.Frame, HersheyFonts.HersheyPlain, smallTextFontRelativeSize, Defaults.YellowColor, smallTextFontRelativeThickness);
                    }
                }
            }

            if (humanDistance != null)
            {
                foreach (HumanDistance item in humanDistance)
                {
                    if (item.IsViolation)
                    {
                        Cv2.Line(resultFrame, item.Line.A.Frame, item.Line.B.Frame, Defaults.RedColor, itemLineRelativeThickness);
                        RelativePoint center = GeometryHelpers.GetPoint(item.Line, 0.5);
                        Cv2.PutText(resultFrame, item.Distance.ToString("0.##") + session.GridProjection.Unit, center.Frame, HersheyFonts.HersheyPlain, smallTextFontRelativeSize, Defaults.BlueColor, smallTextFontRelativeThickness);
                    }
                    //else
                    //{
                    //    Cv2.Line(resultFrame, item.Line.A.Frame, item.Line.B.Frame, Defaults.GreenColor, itemLineRelativeThickness);
                    //    RelativePoint center = GeometryHelpers.GetPoint(item.Line, 0.5);
                    //    Cv2.PutText(resultFrame, item.Distance.ToString("0.##") + session.GridProjection.Unit, center.Frame, HersheyFonts.HersheyPlain, smallTextFontRelativeSize, Defaults.BlueColor, smallTextFontRelativeThickness);
                    //}
                }
            }

            if (session.HumanDetector != null)
            {
                if (GeometryHelpers.IsInside(mousePos.Norm, Defaults.ViolationThresAreaBL, Defaults.ViolationThresAreaTL, Defaults.ViolationThresAreaTR, Defaults.ViolationThresAreaBR))
                {
                    Cv2.PutText(resultFrame, "Violations Count: " + violationsCount.ToString(), violationTextPos, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, Defaults.GreenColor, normalTextFontRelativeThickness);
                }
                else
                {
                    Cv2.PutText(resultFrame, "Violations Count: " + violationsCount.ToString(), violationTextPos, HersheyFonts.HersheyPlain, normalTextFontRelativeSize, violationsCount == 0 ? Defaults.WhiteColor : Defaults.RedColor, normalTextFontRelativeThickness);
                }
            }

            if (logLastFrame)
            {
                logLastFrame = false;
                session.Logger.SetViolationLog(resultFrame, violationsCount);
                if (Control.AudioFeedback)
                {
                    synth.SpeakAsync("Please maintain social distancing");
                }
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
            AnchorPointEdit editor = new AnchorPointEdit(session, anchor);
            ModernDialog dlg = new ModernDialog
            {
                Title = "Anchor Edit",
                Content = editor,
            };
            Button cancelButton = dlg.CancelButton;
            cancelButton.Content = "Cancel";
            Button okButton = dlg.OkButton;
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
            _ = dlg.ShowDialog();
            if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
            {
                editor.Save();
                GetPersistent();
            }
        }

        private void OpenGridSideEditWindow(GridSide side)
        {
            GridSizeEdit editor = new GridSizeEdit(session, side);
            ModernDialog dlg = new ModernDialog
            {
                Title = "Grid Side Edit",
                Content = editor,
            };
            Button cancelButton = dlg.CancelButton;
            cancelButton.Content = "Cancel";
            Button okButton = dlg.OkButton;
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
            _ = dlg.ShowDialog();
            if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
            {
                editor.Save();
                GetPersistent();
            }
        }

        private void OpenViolationThresEditWindow()
        {
            ViolationThresEdit editor = new ViolationThresEdit(session);
            ModernDialog dlg = new ModernDialog
            {
                Title = "Violation Threshold",
                Content = editor,
            };
            Button cancelButton = dlg.CancelButton;
            cancelButton.Content = "Cancel";
            Button okButton = dlg.OkButton;
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
            _ = dlg.ShowDialog();
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
            Anchor? anchor = GetPointAnchor(point);
            GridSide? side = GetGridSide(point);
            if (anchor.HasValue)
            {
                OpenAnchorEditWindow(anchor.Value);
            }
            else if (side.HasValue)
            {
                OpenGridSideEditWindow(side.Value);
            }
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
            session.GridProjection.MaxNormWidth = Defaults.MaxNormWidth;
            session.GridProjection.MaxNormHeight = Defaults.MaxNormHeight;
            session.GridProjection.BL = bl;
            session.GridProjection.TL = tl;
            session.GridProjection.TR = tr;
            session.GridProjection.BR = br;
            session.GridProjection.TopBottomDistance = topBottomDistance;
            session.GridProjection.LeftRightDistance = leftRightDistance;
        }

        public void GetPersistent()
        {
            bl = session.GridProjection.BL;
            tl = session.GridProjection.TL;
            tr = session.GridProjection.TR;
            br = session.GridProjection.BR;
            topBottomDistance = session.GridProjection.TopBottomDistance;
            leftRightDistance = session.GridProjection.LeftRightDistance;
            UpdateGridPoints();
        }

        private void UpdateGridPoints()
        {
            leftMidPoint = session.GridProjection.LeftMidPoint;
            topMidPoint = session.GridProjection.TopMidPoint;
            rightMidPoint = session.GridProjection.RightMidPoint;
            bottomMidPoint = session.GridProjection.BottomMidPoint;
            gridPoints = session.GridProjection.GetGridPoints();
        }
    }
}
