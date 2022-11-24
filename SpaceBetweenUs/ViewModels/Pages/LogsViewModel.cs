using MvvmHelpers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SpaceBetweenUs.Model;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SpaceBetweenUs.ViewModels.Pages
{
    public class LogsViewModel : BaseViewModel
    {
        private Session session;

        private IEnumerable<ViolationLog> violationLogs;
        public IEnumerable<ViolationLog> ViolationLogs
        {
            get => violationLogs;
            set => SetProperty(ref violationLogs, value);
        }

        private ImageSource frame;
        public ImageSource Frame
        {
            get => frame;
            set => SetProperty(ref frame, value);
        }

        private string frameHeader;
        public string FrameHeader
        {
            get => frameHeader;
            set => SetProperty(ref frameHeader, value);
        }

        private readonly Dispatcher dispatcher;

        public LogsViewModel(Session session, Dispatcher dispatcher)
        {
            this.session = session;
            this.dispatcher = dispatcher;
            session.Logger.OnRefreshLogs += Logger_OnRefreshLogs;
            Logger_OnRefreshLogs();
            if (ViolationLogs.Count() > 0)
            {
                SelectFrame(ViolationLogs.First());
            }
        }

        private void Logger_OnRefreshLogs()
        {
            ViolationLogs = session.Logger.ViolationLogs;
            if (ViolationLogs.Count() > 0 && Frame == null)
            {
                try
                {
                    dispatcher.Invoke(delegate
                    {
                        try
                        {
                            SelectFrame(ViolationLogs.First());
                        }
                        catch { }
                    });
                }
                catch { }
            }
        }

        public void SelectFrame(ViolationLog log)
        {
            Mat mat = Cv2.ImRead(log.LogFile);
            if (mat != null)
            {
                Frame = mat.ToWriteableBitmap(PixelFormats.Bgr24);
                FrameHeader = log.DateTime.ToString();
            }
        }
    }
}
