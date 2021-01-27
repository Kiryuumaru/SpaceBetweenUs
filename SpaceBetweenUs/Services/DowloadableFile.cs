using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class DowloadableFileOnProgress
    {
        public DowloadableFile File { get; private set; }
        public long CurrentBytes { get; private set; }
        public long TotalBytes { get; private set; }
        public long Percentage => (long)(100 * ((double)CurrentBytes / TotalBytes));

        public DowloadableFileOnProgress(DowloadableFile file, long currentBytes, long totalBytes)
        {
            File = file;
            CurrentBytes = currentBytes;
            TotalBytes = totalBytes;
        }
    }

    public class OverallDowloadableFileOnProgress : EventArgs
    {
        public DowloadableFile File { get; private set; }
        public long FileCurrentBytes { get; private set; }
        public long FileTotalBytes { get; private set; }
        public long OverallCurrentBytes { get; private set; }
        public long OverallTotalBytes { get; private set; }
        public long FilePercentage => (long)(100 * ((double)FileCurrentBytes / FileTotalBytes));
        public long OverallPercentage => (long)(100 * ((double)OverallCurrentBytes / OverallTotalBytes));

        public OverallDowloadableFileOnProgress(
            DowloadableFile file,
            long fileCurrentBytes,
            long fileTotalBytes,
            long overallCurrentBytes,
            long overallTotalBytes)
        {
            File = file;
            FileCurrentBytes = fileCurrentBytes;
            FileTotalBytes = fileTotalBytes;
            OverallCurrentBytes = overallCurrentBytes;
            OverallTotalBytes = overallTotalBytes;
        }
    }

    public class DowloadableFile
    {
        private const string TempFileNameAddition = ".temp";

        public string Url { get; private set; }
        public string AbsolutePath { get; private set; }
        public bool LocalExist => File.Exists(AbsolutePath);
        public string FileName => Path.GetFileName(Url);

        public DowloadableFile(string url, string absolutePath)
        {
            Url = url;
            AbsolutePath = absolutePath;
        }

        public async Task<long> GetLocalFileSize()
        {
            if (!File.Exists(AbsolutePath)) return -1;
            var info = new FileInfo(AbsolutePath);
            return await Task.FromResult(info.Length);
        }

        public async Task<long> GetOnlineFileSize()
        {
            var uri = new Uri(Url);

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(30);

                using (var httpResponseMessage = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    if (!httpResponseMessage.IsSuccessStatusCode) return -1;

                    if (httpResponseMessage.Content.Headers.ContentLength.HasValue)
                    {
                        return httpResponseMessage.Content.Headers.ContentLength.Value;
                    }
                    else return -1;
                }
            }
        }

        public async Task Download(Action<DowloadableFileOnProgress> onProgress)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(AbsolutePath));
            var uri = new Uri(Url);
            WebClient client = new WebClient();
            var fileInfo = new FileInfo(AbsolutePath);
            string tempAbsFile = AbsolutePath + TempFileNameAddition;
            if (fileInfo.Exists)
            {
                File.Delete(AbsolutePath);
            }
            long currentBytes = 0;
            long totalBytes = 0;
            client.DownloadProgressChanged += (s, e) =>
            {
                currentBytes = e.BytesReceived;
                totalBytes = e.TotalBytesToReceive;
                onProgress?.Invoke(new DowloadableFileOnProgress(this, e.BytesReceived, e.TotalBytesToReceive));
            };
            onProgress?.Invoke(new DowloadableFileOnProgress(this, 0, await GetOnlineFileSize()));
            await client.DownloadFileTaskAsync(uri, tempAbsFile);
            if (currentBytes == totalBytes)
            {
                if (File.Exists(tempAbsFile))
                {
                    File.Move(tempAbsFile, AbsolutePath);
                }
            }
        }

        public static async Task Download(IEnumerable<DowloadableFile> dowloadableFiles, Action<OverallDowloadableFileOnProgress> onProgress)
        {
            List<(DowloadableFile File, long Size)> dfs = new List<(DowloadableFile File, long Size)>();
            long overallTotalSize = 0;
            foreach (var df in dowloadableFiles)
            {
                long fileSize = await df.GetOnlineFileSize();
                overallTotalSize += fileSize;
                dfs.Add((df, fileSize));
            }

            long currentDownloaded = 0;
            foreach (var df in dfs)
            {
                await df.File.Download(args =>
                {
                    onProgress?.Invoke(new OverallDowloadableFileOnProgress(
                        df.File,
                        args.CurrentBytes,
                        args.TotalBytes,
                        currentDownloaded + args.CurrentBytes,
                        overallTotalSize));
                });
                currentDownloaded += df.Size;
            }
        }
    }
}
