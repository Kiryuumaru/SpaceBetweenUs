using System;
using Windows.Graphics.Imaging;
using OpenCvSharp;
using System.Runtime.InteropServices;

namespace SpaceBetweenUsGUI.ImgProc
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public static class CVHelper
    {
        #region Helpers

        public static unsafe Mat SoftwareBitmap2Mat(SoftwareBitmap softwareBitmap)
        {
            using BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Write);
            using var reference = buffer.CreateReference();
            ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);

            Mat outputMat = new Mat(softwareBitmap.PixelHeight, softwareBitmap.PixelWidth, MatType.CV_8UC4, (IntPtr)dataInBytes);
            return outputMat;
        }

        public static unsafe void Mat2SoftwareBitmap(Mat input, SoftwareBitmap output)
        {
            using BitmapBuffer buffer = output.LockBuffer(BitmapBufferAccessMode.ReadWrite);
            using var reference = buffer.CreateReference();
            ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);
            BitmapPlaneDescription bufferLayout = buffer.GetPlaneDescription(0);

            for (int i = 0; i < bufferLayout.Height; i++)
            {
                for (int j = 0; j < bufferLayout.Width; j++)
                {
                    dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 0] =
                        input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 0];
                    dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 1] =
                        input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 1];
                    dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 2] =
                        input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 2];
                    dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 3] =
                        input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 3];
                }
            }
        }

        #endregion

        #region Methods

        public static void Blur(
            SoftwareBitmap input,
            SoftwareBitmap output,
            Size kSize,
            Point? anchor = null,
            BorderTypes borderType = BorderTypes.Default)
        {
            using Mat mInput = SoftwareBitmap2Mat(input);
            using Mat mOutput = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);

            Cv2.Blur(mInput, mOutput, kSize, anchor, borderType);
            Mat2SoftwareBitmap(mOutput, output);
        }

        private static BackgroundSubtractorMOG2 mog2;
        public static void MotionDetector(
            SoftwareBitmap input,
            SoftwareBitmap output,
            double learningRate = -1,
            bool reset = false)
        {
            if (mog2 == null || reset) mog2 = BackgroundSubtractorMOG2.Create();

            using Mat mInput = SoftwareBitmap2Mat(input);
            using Mat mOutput = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);
            using Mat fgMaskMOG2 = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);
            using Mat temp = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);

            mog2.Apply(mInput, fgMaskMOG2, learningRate);
            Cv2.CvtColor(fgMaskMOG2, temp, ColorConversionCodes.GRAY2BGRA);

            using Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            Cv2.Erode(temp, temp, element);
            temp.CopyTo(mOutput);
            Mat2SoftwareBitmap(mOutput, output);
        }

        #endregion
    }
}
