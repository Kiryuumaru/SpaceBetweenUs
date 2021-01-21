using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridInitializer
{
    #region Helpers

    public struct PersPoint
    {
        public Point Norm;
        public Point Frame;

        public static PersPoint Zero()
        {
            return new PersPoint(new Point(0, 0), new Point(0, 0));
        }

        public PersPoint(Point norm, Point frame)
        {
            Norm = norm;
            Frame = frame;
        }

        public override bool Equals(object obj)
        {
            if (obj is PersPoint pPoint)
            {
                return Norm.X == pPoint.Norm.X && Norm.Y == pPoint.Norm.Y && Frame.X == pPoint.Frame.X && Frame.Y == pPoint.Frame.Y;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(PersPoint left, PersPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PersPoint left, PersPoint right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct PersLine
    {
        public PersPoint A;
        public PersPoint B;

        public PersLine(PersPoint a, PersPoint b)
        {
            A = a;
            B = b;
        }

        public override bool Equals(object obj)
        {
            if (obj is PersLine line)
            {
                return (A.Equals(line.A) && B.Equals(line.B)) || (A.Equals(line.B) && B.Equals(line.A));
            }
            return false;
        }

        public static bool operator ==(PersLine left, PersLine right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PersLine left, PersLine right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum PointEdge
    {
        BL, TL, TR, BR
    }

    #endregion

    public partial class MainForm : Form
    {
        private const int XMax = 1000;
        private const int YMax = 1000;
        private const int DotRadius = 4;
        private const int LineThickness = 1;
        private const double GridDistance = 0.5; // meters

        private MCvScalar RedColor = new MCvScalar(0, 0, 255);
        private MCvScalar GreenColor = new MCvScalar(0, 255, 0);
        private MCvScalar BlueColor = new MCvScalar(255, 0, 0);
        private MCvScalar PurpleColor = new MCvScalar(128, 0, 128);
        private MCvScalar YellowColor = new MCvScalar(0, 238, 238);

        private PointEdge refPoint = PointEdge.BL;
        private int depthBL = 0;
        private int depthTL = 0;
        private int depthTR = 0;
        private int depthBR = 0;
        private PersPoint bl = PersPoint.Zero();
        private PersPoint tl = PersPoint.Zero();
        private PersPoint tr = PersPoint.Zero();
        private PersPoint br = PersPoint.Zero();
        private readonly List<PersLine> gridX = new List<PersLine>();
        private readonly List<PersLine> gridY = new List<PersLine>();

        private bool isSelectUp = false;
        private bool isPlaying = false;

        private Capture capture;
        private Mat frame;
        private Mat pyrDown;

        public MainForm()
        {
            InitializeComponent();

            ChangeImageSize(500, 300);

            radioButtonBL.Checked = true;
        }

        #region Helpers

        private int NormToFrameX(int normX)
        {
            return (int)(imageBox.Width * ((double)normX / XMax));
        }

        private int NormToFrameY(int normY)
        {
            return (int)(imageBox.Height * ((double)normY / YMax));
        }

        private int FrameToNormX(int frameX)
        {
            return (int)(XMax * ((double)frameX / imageBox.Width));
        }

        private int FrameToNormY(int frameY)
        {
            return (int)(YMax * ((double)frameY / imageBox.Height));
        }
        public bool IsInside(Point interest, params Point[] polygon)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > interest.Y) != (polygon[j].Y > interest.Y)) &&
                (interest.X < (polygon[j].X - polygon[i].X) * (interest.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        public int GetPointX(PersPoint point1, PersPoint point2, double yOffSet)
        {
            return point1.Norm.X + (int)((point2.Norm.X - point1.Norm.X) * yOffSet);
        }

        public int GetPointY(PersPoint point1, PersPoint point2, double xOffSet)
        {
            return point1.Norm.Y + (int)((point2.Norm.Y - point1.Norm.Y) * xOffSet);
        }

        public int GetPointX(PersLine line, double yOffSet)
        {
            return line.A.Norm.X + (int)((line.B.Norm.X - line.A.Norm.X) * yOffSet);
        }

        public int GetPointY(PersLine line, double xOffSet)
        {
            return line.A.Norm.Y + (int)((line.B.Norm.Y - line.A.Norm.Y) * xOffSet);
        }

        public bool IsParallel(PersPoint A, PersPoint B, PersPoint C, PersPoint D)
        {
            int a1 = B.Norm.Y - A.Norm.Y;
            int b1 = A.Norm.X - B.Norm.X;
            int a2 = D.Norm.Y - C.Norm.Y;
            int b2 = C.Norm.X - D.Norm.X;

            double determinant = a1 * b2 - a2 * b1;

            return (determinant == 0);
        }

        public bool IsParallel(PersLine ab, PersLine cd)
        {
            return IsParallel(ab.A, ab.B, cd.A, cd.B);
        }

        public PersPoint LineIntersection(PersPoint A, PersPoint B, PersPoint C, PersPoint D)
        {
            // Line AB represented as a1x + b1y = c1  
            int a1 = B.Norm.Y - A.Norm.Y;
            int b1 = A.Norm.X - B.Norm.X;
            int c1 = a1 * (A.Norm.X) + b1 * (A.Norm.Y);

            // Line CD represented as a2x + b2y = c2  
            int a2 = D.Norm.Y - C.Norm.Y;
            int b2 = C.Norm.X - D.Norm.X;
            int c2 = a2 * (C.Norm.X) + b2 * (C.Norm.Y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                throw new Exception("Lines are in parallel");
            }
            else
            {
                int x = (int)((b2 * c1 - b1 * c2) / determinant);
                int y = (int)((a1 * c2 - a2 * c1) / determinant);
                return new PersPoint(new Point(x, y), new Point(NormToFrameX(x), NormToFrameY(y)));
            }
        }

        public PersPoint LineIntersection(PersLine ab, PersLine cd)
        {
            return LineIntersection(ab.A, ab.B, cd.A, cd.B);
        }

        public Point GetPoint(PersPoint a, PersPoint b, double offSet)
        {
            return new Point(GetPointX(a, b, offSet), GetPointY(a, b, offSet));
        }

        public Point GetPoint(PersLine line, double offSet)
        {
            return new Point(GetPointX(line, offSet), GetPointY(line, offSet));
        }

        public float[] PointsToArray(params PersPoint[] points)
        {
            List<float> ret = new List<float>();
            foreach (PersPoint p in points)
            {
                ret.Add(p.Norm.X);
                ret.Add(p.Norm.Y);
            }
            return ret.ToArray();
        }

        public float[] LinesToArray(params PersLine[] lines)
        {
            List<float> ret = new List<float>();
            foreach (PersLine l in lines)
            {
                ret.Add(l.A.Norm.X);
                ret.Add(l.A.Norm.Y);
                ret.Add(l.B.Norm.X);
                ret.Add(l.B.Norm.Y);
            }
            return ret.ToArray();
        }

        public double GetDistance(PersPoint pointA, PersPoint pointB)
        {
            int a = pointB.Norm.X - pointA.Norm.X;
            int b = pointB.Norm.Y - pointA.Norm.Y;

            return Math.Sqrt(a * a + b * b);
        }

        public double PolygonArea(params PersPoint[] polygon)
        {
            int i, j;
            double area = 0;

            for (i = 0; i < polygon.Length; i++)
            {
                j = (i + 1) % polygon.Length;

                area += polygon[i].Norm.X * polygon[j].Norm.Y;
                area -= polygon[i].Norm.Y * polygon[j].Norm.X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }

        #endregion

        private void ChangeImageSize(int width, int height)
        {
            imageBox.Size = new Size(width, height);
            imageBox.Location = new Point(panelMain.Width > width ? (panelMain.Width / 2) - (width / 2) : 0, 0);

            panelControl.Location = new Point(
                panelMain.Width > panelControl.Width ? (panelMain.Width / 2) - (panelControl.Width / 2) : 0,
                imageBox.Location.Y + imageBox.Height);
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
            if (frame != null)
                frame.Dispose();
            if (pyrDown != null)
                pyrDown.Dispose();
            capture = null;
            frame = null;
            pyrDown = null;
        }

        private void SelectToggle(object buttonSelect)
        {
            if (buttonSelect.Equals(buttonSelectBL))
            {
                buttonSelectBL.Text = isSelectUp ? "Select" : "Done";
                buttonSelectTL.Enabled = isSelectUp;
                buttonSelectTR.Enabled = isSelectUp;
                buttonSelectBR.Enabled = isSelectUp;
                isSelectUp = !isSelectUp;
            }
            else if (buttonSelect.Equals(buttonSelectTL))
            {
                buttonSelectBL.Enabled = isSelectUp;
                buttonSelectTL.Text = isSelectUp ? "Select" : "Done";
                buttonSelectTR.Enabled = isSelectUp;
                buttonSelectBR.Enabled = isSelectUp;
                isSelectUp = !isSelectUp;
            }
            else if (buttonSelect.Equals(buttonSelectTR))
            {
                buttonSelectBL.Enabled = isSelectUp;
                buttonSelectTL.Enabled = isSelectUp;
                buttonSelectTR.Text = isSelectUp ? "Select" : "Done";
                buttonSelectBR.Enabled = isSelectUp;
                isSelectUp = !isSelectUp;
            }
            else if (buttonSelect.Equals(buttonSelectBR))
            {
                buttonSelectBL.Enabled = isSelectUp;
                buttonSelectTL.Enabled = isSelectUp;
                buttonSelectTR.Enabled = isSelectUp;
                buttonSelectBR.Text = isSelectUp ? "Select" : "Done";
                isSelectUp = !isSelectUp;
            }
        }

        private void InitCapture()
        {
            Application.Idle -= ProcessFrame;

            isPlaying = false;

            ReleaseData();

            capture = new Capture(openFileDialog.FileName);
            frame = new Mat();
            pyrDown = new Mat();

            QueryFrame();

            ChangeImageSize(pyrDown.Width, pyrDown.Height);

            buttonPausePlay.Text = "Play";
        }

        private void StartCapture()
        {
            if (capture == null)
            {
                MessageBox.Show("Please select file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            buttonPausePlay.Text = "Pause";
            isPlaying = true;

            Application.Idle += ProcessFrame;
        }

        private void StopCapture()
        {
            buttonPausePlay.Text = "Play";
            isPlaying = false;

            Application.Idle -= ProcessFrame;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            try
            {
                QueryFrame();

                if (frame == null)
                {
                    InitCapture();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QueryFrame(bool consume = true)
        {
            try
            {
                if (consume) frame = capture.QueryFrame();

                if (frame != null)
                {
                    CvInvoke.PyrDown(frame, pyrDown);

                    if ((bl.Frame.X != 0 || bl.Frame.Y != 0) && (tl.Frame.X != 0 || tl.Frame.Y != 0))
                        CvInvoke.Line(pyrDown, bl.Frame, tl.Frame, YellowColor, NormToFrameX(LineThickness));
                    if ((tl.Frame.X != 0 || tl.Frame.Y != 0) && (tr.Frame.X != 0 || tr.Frame.Y != 0))
                        CvInvoke.Line(pyrDown, tl.Frame, tr.Frame, YellowColor, NormToFrameX(LineThickness));
                    if ((tr.Frame.X != 0 || tr.Frame.Y != 0) && (br.Frame.X != 0 || br.Frame.Y != 0))
                        CvInvoke.Line(pyrDown, tr.Frame, br.Frame, YellowColor, NormToFrameX(LineThickness));
                    if ((br.Frame.X != 0 || br.Frame.Y != 0) && (bl.Frame.X != 0 || bl.Frame.Y != 0))
                        CvInvoke.Line(pyrDown, br.Frame, bl.Frame, YellowColor, NormToFrameX(LineThickness));

                    foreach (var line in gridX)
                    {
                        CvInvoke.Line(pyrDown, line.A.Frame, line.B.Frame, YellowColor, NormToFrameX(LineThickness));
                    }
                    foreach (var line in gridY)
                    {
                        CvInvoke.Line(pyrDown, line.A.Frame, line.B.Frame, YellowColor, NormToFrameX(LineThickness));
                    }

                    if (bl.Frame.X != 0 || bl.Frame.Y != 0)
                        CvInvoke.Circle(pyrDown, bl.Frame, NormToFrameX(DotRadius), RedColor, NormToFrameX(DotRadius));
                    if (tl.Frame.X != 0 || tl.Frame.Y != 0)
                        CvInvoke.Circle(pyrDown, tl.Frame, NormToFrameX(DotRadius), GreenColor, NormToFrameX(DotRadius));
                    if (tr.Frame.X != 0 || tr.Frame.Y != 0)
                        CvInvoke.Circle(pyrDown, tr.Frame, NormToFrameX(DotRadius), BlueColor, NormToFrameX(DotRadius));
                    if (br.Frame.X != 0 || br.Frame.Y != 0)
                        CvInvoke.Circle(pyrDown, br.Frame, NormToFrameX(DotRadius), PurpleColor, NormToFrameX(DotRadius));

                    imageBox.Image = pyrDown;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateGrid()
        {
            gridX.Clear();
            gridY.Clear();

            
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            SelectToggle(sender);
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    InitCapture();

                    if (frame == null)
                    {
                        MessageBox.Show("Invalid file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ButtonPausePlay_Click(object sender, EventArgs e)
        {
            try
            {
                if (isPlaying)
                {
                    StopCapture();
                }
                else
                {
                    StartCapture();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImageBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (isSelectUp)
            {
                int normX = FrameToNormX(e.X);
                int normY = FrameToNormY(e.Y);
                if (buttonSelectBL.Enabled)
                {
                    bl = new PersPoint(new Point(normX, normY), new Point(NormToFrameX(normX), NormToFrameY(normY)));
                    textBoxBLX.Text = normX.ToString();
                    textBoxBLY.Text = normY.ToString();
                }
                else if (buttonSelectTL.Enabled)
                {
                    tl = new PersPoint(new Point(normX, normY), new Point(NormToFrameX(normX), NormToFrameY(normY)));
                    textBoxTLX.Text = normX.ToString();
                    textBoxTLY.Text = normY.ToString();
                }
                else if (buttonSelectTR.Enabled)
                {
                    tr = new PersPoint(new Point(normX, normY), new Point(NormToFrameX(normX), NormToFrameY(normY)));
                    textBoxTRX.Text = normX.ToString();
                    textBoxTRY.Text = normY.ToString();
                }
                else if (buttonSelectBR.Enabled)
                {
                    br = new PersPoint(new Point(normX, normY), new Point(NormToFrameX(normX), NormToFrameY(normY)));
                    textBoxBRX.Text = normX.ToString();
                    textBoxBRY.Text = normY.ToString();
                }
                CalculateGrid();
                QueryFrame(false);
            }
        }

        private void EvaluateTextBoxInput()
        {
            if (!int.TryParse(textBoxBLD.Text, out int blD) ||
                !int.TryParse(textBoxBLY.Text, out int blY) ||
                !int.TryParse(textBoxBLX.Text, out int blX) ||
                !int.TryParse(textBoxTLD.Text, out int tlD) ||
                !int.TryParse(textBoxTLY.Text, out int tlY) ||
                !int.TryParse(textBoxTLX.Text, out int tlX) ||
                !int.TryParse(textBoxTRD.Text, out int trD) ||
                !int.TryParse(textBoxTRY.Text, out int trY) ||
                !int.TryParse(textBoxTRX.Text, out int trX) ||
                !int.TryParse(textBoxBRD.Text, out int brD) ||
                !int.TryParse(textBoxBRY.Text, out int brY) ||
                !int.TryParse(textBoxBRX.Text, out int brX))
            {
                textBoxBLD.Text = depthBL.ToString();
                textBoxBLY.Text = bl.Norm.Y.ToString();
                textBoxBLX.Text = bl.Norm.X.ToString();
                textBoxTLD.Text = depthTL.ToString();
                textBoxTLY.Text = tl.Norm.Y.ToString();
                textBoxTLX.Text = tl.Norm.X.ToString();
                textBoxTRD.Text = depthTR.ToString();
                textBoxTRY.Text = tr.Norm.Y.ToString();
                textBoxTRX.Text = tr.Norm.X.ToString();
                textBoxBRD.Text = depthBR.ToString();
                textBoxBRY.Text = br.Norm.Y.ToString();
                textBoxBRX.Text = br.Norm.X.ToString();
                MessageBox.Show("Invalid parameter entered", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            depthBL = blD;
            depthTL = tlD;
            depthTR = trD;
            depthBR = brD;
            bl = new PersPoint(new Point(blX, blY), new Point(NormToFrameX(blX), NormToFrameY(blY)));
            tl = new PersPoint(new Point(tlX, tlY), new Point(NormToFrameX(tlX), NormToFrameY(tlY)));
            tr = new PersPoint(new Point(trX, trY), new Point(NormToFrameX(trX), NormToFrameY(trY)));
            br = new PersPoint(new Point(brX, brY), new Point(NormToFrameX(brX), NormToFrameY(brY)));

            CalculateGrid();
            QueryFrame(false);
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            EvaluateTextBoxInput();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EvaluateTextBoxInput();
                e.Handled = true;
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {

        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.Equals(radioButtonBL) && radioButtonBL.Checked)
            {
                refPoint = PointEdge.BL;
                radioButtonTL.Checked = false;
                radioButtonTR.Checked = false;
                radioButtonBR.Checked = false;
                textBoxBLD.Enabled = false;
                textBoxTLD.Enabled = true;
                textBoxTRD.Enabled = true;
                textBoxBRD.Enabled = true;
            }
            else if (sender.Equals(radioButtonTL) && radioButtonTL.Checked)
            {
                radioButtonBL.Checked = false;
                refPoint = PointEdge.TL;
                radioButtonTR.Checked = false;
                radioButtonBR.Checked = false;
                textBoxBLD.Enabled = true;
                textBoxTLD.Enabled = false;
                textBoxTRD.Enabled = true;
                textBoxBRD.Enabled = true;
            }
            else if (sender.Equals(radioButtonTR) && radioButtonTR.Checked)
            {
                radioButtonBL.Checked = false;
                radioButtonTL.Checked = false;
                refPoint = PointEdge.TR;
                radioButtonBR.Checked = false;
                textBoxBLD.Enabled = true;
                textBoxTLD.Enabled = true;
                textBoxTRD.Enabled = false;
                textBoxBRD.Enabled = true;
            }
            else if (sender.Equals(radioButtonBR) && radioButtonBR.Checked)
            {
                radioButtonBL.Checked = false;
                radioButtonTL.Checked = false;
                radioButtonTR.Checked = false;
                refPoint = PointEdge.BR;
                textBoxBLD.Enabled = true;
                textBoxTLD.Enabled = true;
                textBoxTRD.Enabled = true;
                textBoxBRD.Enabled = false;
            }
        }
    }
}
