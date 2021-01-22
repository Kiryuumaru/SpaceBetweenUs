using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Windows.Graphics.Imaging;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using OpenCvSharp;

namespace SpaceBetweenUsGUI.ImgProc
{
    public class FrameRenderer
    {
        private readonly Image imageElement;
        private SoftwareBitmap backBuffer;
        private bool _taskRunning = false;

        public FrameRenderer(Image imageElement)
        {
            this.imageElement = imageElement;
            this.imageElement.Source = new SoftwareBitmapSource();
        }

        public void RenderFrame(SoftwareBitmap softwareBitmap)
        {
            if (softwareBitmap != null)
            {
                // Swap the processed frame to _backBuffer and trigger UI thread to render it
                softwareBitmap = Interlocked.Exchange(ref backBuffer, softwareBitmap);

                // UI thread always reset _backBuffer before using it.  Unused bitmap should be disposed.
                softwareBitmap?.Dispose();

                // Changes to xaml ImageElement must happen in UI thread through Dispatcher
                var task = imageElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        // Don't let two copies of this task run at the same time.
                        if (_taskRunning)
                        {
                            return;
                        }
                        _taskRunning = true;

                        // Keep draining frames from the backbuffer until the backbuffer is empty.
                        SoftwareBitmap latestBitmap;
                        while ((latestBitmap = Interlocked.Exchange(ref backBuffer, null)) != null)
                        {
                            var imageSource = (SoftwareBitmapSource)imageElement.Source;
                            await imageSource.SetBitmapAsync(latestBitmap);
                            latestBitmap.Dispose();
                        }

                        _taskRunning = false;
                    });
            }
        }
    }
}
