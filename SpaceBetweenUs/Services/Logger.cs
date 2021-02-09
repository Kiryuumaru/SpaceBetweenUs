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
        public IEnumerable<ViolationLog> ViolationLogs { get; private set; } = new List<ViolationLog>();
        public event Action OnRefreshLogs;
        private Logger() { }
        public static async Task<Logger> Initialize()
        {
            var logger = new Logger();
            await logger.RefreshLogs();
            return logger;
        }

        public async Task RefreshLogs()
        {
            await Task.Run(delegate
            {
                Directory.CreateDirectory(Defaults.LogsPath);
                string[] files = Directory.GetFiles(Defaults.LogsPath);
                var logs = new List<ViolationLog>();
                foreach (var file in files)
                {
                    var log = ViolationLog.FromFile(file);
                    if (log != null) logs.Add(log);
                }
                ViolationLogs = logs.OrderByDescending(i => i.DateTime);
                OnRefreshLogs?.Invoke();
            });
        }

        public async void SetViolationLog(Mat frame, int violationsCount, int violatorsCount)
        {
            ViolationLog.Create(DateTime.Now, violationsCount, violatorsCount, frame);
            await RefreshLogs();
        }
    }
}
