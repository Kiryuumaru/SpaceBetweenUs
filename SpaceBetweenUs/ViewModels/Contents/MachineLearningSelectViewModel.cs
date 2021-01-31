using FirstFloor.ModernUI.Presentation;
using MvvmHelpers;
using SpaceBetweenUs.Services;
using SpaceBetweenUs.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpaceBetweenUs.ViewModels.Contents
{
    public class MachineLearningSelectViewModel : BaseViewModel
    {
        public static List<UIDownloadableFile> CachedDownloadableModels = new List<UIDownloadableFile>();

        public class UIDownloadableFile : BaseViewModel
        {
            public string absolutePath;
            public string AbsolutePath
            {
                get => absolutePath;
                set => SetProperty(ref absolutePath, value);
            }

            public string fileName;
            public string FileName
            {
                get => fileName;
                set => SetProperty(ref fileName, value);
            }

            private string url;
            public string Url
            {
                get => url;
                set => SetProperty(ref url, value);
            }

            public string status;
            public string Status
            {
                get => status;
                set => SetProperty(ref status, value);
            }

            private string cachedFileSizeString;
            public string CachedFileSizeString
            {
                get => cachedFileSizeString;
                set => SetProperty(ref cachedFileSizeString, value);
            }

            public DownloadableFileState CurrentState;

            public Action<DownloadableFileState> OnDowloadStateChange;

            private Action<Action> onDownload;
            private Action onCancel;

            private UIDownloadableFile() { }

            public void Download(Action onFinish)
            {
                onDownload?.Invoke(onFinish);
            }

            public void Cancel()
            {
                onCancel?.Invoke();
            }

            public static UIDownloadableFile FromDowloadableFile(DowloadableFile downloadableFile)
            {
                var file = new UIDownloadableFile();
                string stringSize = Session.Datastore.GetValue(downloadableFile.Url + "_size");
                file.AbsolutePath = downloadableFile.AbsolutePath;
                file.FileName = downloadableFile.FileName;
                file.Url = downloadableFile.Url;
                file.Status = downloadableFile.LocalExist ? "Downloaded" : "Not downloaded";
                file.onDownload = async onFinish =>
                {
                    file.CurrentState = DownloadableFileState.Downloading;
                    file.OnDowloadStateChange?.Invoke(DownloadableFileState.Downloading);
                    await downloadableFile.Download(progress =>
                    {
                        file.Status = "Downloading - " + progress.Percentage.ToString("0.##") + "%";
                        if (progress.Percentage == 100)
                        {
                            file.CurrentState = DownloadableFileState.Downloaded;
                            file.OnDowloadStateChange?.Invoke(DownloadableFileState.Downloaded);
                            file.Status = "Downloaded";
                        }
                    });
                    onFinish?.Invoke();
                };
                file.onCancel = delegate
                {
                    downloadableFile.CancelDownload();
                    file.CurrentState = DownloadableFileState.NotDownloaded;
                    file.OnDowloadStateChange?.Invoke(DownloadableFileState.NotDownloaded);
                    file.Status = downloadableFile.LocalExist ? "Downloaded" : "Not downloaded";
                };
                file.CachedFileSizeString = string.IsNullOrEmpty(stringSize) ? "N/A" : stringSize;
                file.CurrentState = downloadableFile.State;
                file.OnDowloadStateChange?.Invoke(downloadableFile.State);
                Task.Run(async delegate
                {
                    var size = await downloadableFile.GetOnlineFileSize();
                    stringSize = CommonHelpers.SizeSuffix(size);
                    Session.Datastore.SetValue(downloadableFile.Url + "_size", stringSize);
                    file.CachedFileSizeString = stringSize;
                });
                return file;
            }
        }

        private IEnumerable<UIDownloadableFile> dowloadableFiles;
        public IEnumerable<UIDownloadableFile> DownloadableFiles
        {
            get => dowloadableFiles;
            set => SetProperty(ref dowloadableFiles, value);
        }

        private ICommand selectCurrentModel;
        public ICommand SelectCurrentModel
        {
            get
            {
                if (selectCurrentModel == null)
                {
                    selectCurrentModel = new RelayCommand(delegate
                    {
                        Session.MLModel = model;
                        Update();
                    });
                }
                return selectCurrentModel;
            }
        }

        private bool isButtonSelectActive;
        public bool IsButtonSelectActive
        {
            get => isButtonSelectActive;
            set => SetProperty(ref isButtonSelectActive, value);
        }

        private string selectButtonContent;
        public string SelectButtonContent
        {
            get => selectButtonContent;
            set => SetProperty(ref selectButtonContent, value);
        }

        public bool IsSelected
        {
            get => !IsButtonSelectActive;
            set
            {
                SelectButtonContent = value ? "Model Selected" : "Select Model";
                IsButtonSelectActive = !value;
            }
        }

        private ICommand downloadFiles;
        public ICommand DownloadFiles
        {
            get
            {
                if (downloadFiles == null)
                {
                    downloadFiles = new RelayCommand(delegate
                    {
                        switch (State)
                        {
                            case DownloadableFileState.NotDownloaded:
                            case DownloadableFileState.Downloaded:
                                foreach (var file in DownloadableFiles)
                                {
                                    file.Download(null);
                                }
                                break;
                            case DownloadableFileState.Downloading:
                                foreach (var file in DownloadableFiles)
                                {
                                    file.Cancel();
                                }
                                break;
                        }
                    });
                }
                return downloadFiles;
            }
        }

        private string downloadButtonContent;
        public string DownloadButtonContent
        {
            get => downloadButtonContent;
            set => SetProperty(ref downloadButtonContent, value);
        }

        private DownloadableFileState state;
        public DownloadableFileState State
        {
            get => state;
            set
            {
                state = value;
                switch (value)
                {
                    case DownloadableFileState.NotDownloaded:
                        DownloadButtonContent = "Download All";
                        break;
                    case DownloadableFileState.Downloading:
                        DownloadButtonContent = "Cancel All Downloads";
                        break;
                    case DownloadableFileState.Downloaded:
                        DownloadButtonContent = "Re-download All";
                        break;
                }
            }
        }

        private MLModel model;

        public MachineLearningSelectViewModel()
        {
            Update();
            MachineLearningViewModel.JumperModelTabChange += delegate { Update(); };
            foreach (var file in DownloadableFiles)
            {
                file.OnDowloadStateChange = state =>
                {
                    if (DownloadableFiles.Any(i => i.CurrentState == DownloadableFileState.Downloading))
                        State = DownloadableFileState.Downloading;
                    else if (DownloadableFiles.All(i => i.CurrentState == DownloadableFileState.Downloaded))
                        State = DownloadableFileState.Downloaded;
                    else
                        State = DownloadableFileState.NotDownloaded;
                };
                file.OnDowloadStateChange?.Invoke(file.CurrentState);
            }
        }

        public void Update()
        {
            var uiFiles = new List<UIDownloadableFile>();

            model = MLModel.GetMLModels()?.FirstOrDefault(i => i.Name.Equals(MachineLearningViewModel.JumperCurrentModelSelect));
            var files = model?.GetAllDownloadableFiles();

            foreach (var file in files)
            {
                var cachedFile = CachedDownloadableModels.FirstOrDefault(i => i.AbsolutePath.Equals(file.AbsolutePath));
                if (cachedFile == null)
                {
                    cachedFile = UIDownloadableFile.FromDowloadableFile(file);
                    CachedDownloadableModels.Add(cachedFile);
                }
                uiFiles.Add(cachedFile);
            }

            DownloadableFiles = uiFiles;
            IsSelected = MachineLearningViewModel.JumperCurrentModelSelect.Equals(Session.MLModel?.Name);
        }
    }
}
