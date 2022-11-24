using OpenCvSharp;
using SpaceBetweenUs.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class Logger
    {
        private Session session;

        public string LogsPath { get; private set; }
        public IEnumerable<ViolationLog> ViolationLogs { get; private set; } = new List<ViolationLog>();
        public event Action OnRefreshLogs;
        private Logger() { }
        public static async Task<Logger> Initialize(Session session)
        {
            Logger logger = new Logger()
            {
                session = session,
                LogsPath = Path.Combine("Session", session.Name, "Logs")
            };
            await logger.RefreshLogs();
            return logger;
        }

        public async Task RefreshLogs()
        {
            await Task.Run(delegate
            {
                _ = Directory.CreateDirectory(LogsPath);
                string[] files = Directory.GetFiles(LogsPath);
                List<ViolationLog> logs = new List<ViolationLog>();
                foreach (string file in files)
                {
                    ViolationLog log = ViolationLog.FromFile(session, file);
                    if (log != null)
                    {
                        logs.Add(log);
                    }
                }
                ViolationLogs = logs.OrderByDescending(i => i.DateTime);
                OnRefreshLogs?.Invoke();
            });
        }

        public async void SetViolationLog(Mat frame, int violationsCount)
        {
            if (Control.LogViolationScreenshot)
            {
                ViolationLog.Create(session, DateTime.Now, violationsCount, frame);
                await RefreshLogs();
            }
        }
    }
}
