using OpenCvSharp;
using SpaceBetweenUs.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Model
{
    public class ViolationLog
    {
        public Session session;

        public string LogFile { get; private set; }
        public DateTime DateTime
        {
            get
            {
                var name = Path.GetFileNameWithoutExtension(LogFile);
                var dateTime = CommonHelpers.DecodeDateTime(CommonHelpers.BlobGetValue(name, "dt"));
                return dateTime.Value;
            }
        }
        public int ViolationsCount
        {
            get
            {
                var name = Path.GetFileNameWithoutExtension(LogFile);
                return int.Parse(CommonHelpers.BlobGetValue(name, "v1"));
            }
        }
        public int ViolatorsCount
        {
            get
            {
                var name = Path.GetFileNameWithoutExtension(LogFile);
                return int.Parse(CommonHelpers.BlobGetValue(name, "v2"));
            }
        }
        public string DateString
        {
            get
            {
                return DateTime.ToLongDateString();
            }
        }
        public string TimeString
        {
            get
            {
                return DateTime.ToLongTimeString();
            }
        }
        public string DateTimeString
        {
            get
            {
                return DateString + " " + TimeString;
            }
        }

        private ViolationLog() { }
        public static void Create(Session session, DateTime dateTime, int violationsCount, int violatorsCount, Mat frame)
        {
            Directory.CreateDirectory(session.Logger.LogsPath);
            string filename = "";
            filename = CommonHelpers.BlobSetValue(filename, "dt", CommonHelpers.EncodeDateTime(dateTime));
            filename = CommonHelpers.BlobSetValue(filename, "v1", violationsCount.ToString());
            filename = CommonHelpers.BlobSetValue(filename, "v2", violatorsCount.ToString());
            filename += ".jpg";
            filename = Path.Combine(session.Logger.LogsPath, filename);
            Cv2.ImWrite(filename, frame);
        }

        public static ViolationLog FromFile(Session session, string logPath)
        {
            var log = new ViolationLog()
            {
                session = session,
                LogFile = logPath
            };
            try
            {
                _ = log.DateTime;
                return log;
            }
            catch
            {
                return null;
            }
        }

        public async void Delete()
        {
            try
            {
                File.Delete(LogFile);
                await session.Logger.RefreshLogs();
            }
            catch { }
        }
    }
}
