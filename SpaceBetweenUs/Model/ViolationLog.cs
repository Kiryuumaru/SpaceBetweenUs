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

        private ViolationLog() { }
        public static void Create(DateTime dateTime, int violationsCount, int violatorsCount, Mat frame)
        {
            Directory.CreateDirectory(Defaults.LogsPath);
            string filename = "";
            filename = CommonHelpers.BlobSetValue(filename, "dt", CommonHelpers.EncodeDateTime(dateTime));
            filename = CommonHelpers.BlobSetValue(filename, "v1", violationsCount.ToString());
            filename = CommonHelpers.BlobSetValue(filename, "v2", violatorsCount.ToString());
            filename += ".jpg";
            filename = Path.Combine(Defaults.LogsPath, filename);
            Cv2.ImWrite(filename, frame);
        }

        public static ViolationLog FromFile(string logPath)
        {
            var log = new ViolationLog()
            {
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

        public void Delete()
        {
            File.Delete(LogFile);
        }
    }
}
