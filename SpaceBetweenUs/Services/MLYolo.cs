using Alturos.Yolo;
using Alturos.Yolo.Model;
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
    public class MLYolo
    {
        #region Properties

        private YoloWrapper yoloWrapper;
        public MLYoloModel MLYoloModel { get; private set; }
        public bool IsDatasetReady => MLYoloModel.ConfigFile.LocalExist && MLYoloModel.NamesFile.LocalExist && MLYoloModel.WeightsFile.LocalExist;
        public bool IsStarted => yoloWrapper != null;

        #endregion

        #region Initializer

        public MLYolo(MLYoloModel yoloModel)
        {
            MLYoloModel = yoloModel;
        }

        #endregion

        #region Methods

        public void Start()
        {
            yoloWrapper = new YoloWrapper(MLYoloModel.ConfigFile.AbsolutePath, MLYoloModel.WeightsFile.AbsolutePath, MLYoloModel.NamesFile.AbsolutePath);

        }

        public async void DownloadDatasets(Action<OverallDowloadableFileOnProgress> onProgress)
        {
            var dfs = new DowloadableFile[]
            {
                MLYoloModel.ConfigFile,
                MLYoloModel.NamesFile,
                MLYoloModel.WeightsFile
            };
            await DowloadableFile.Download(dfs, onProgress);
        }

        public IEnumerable<YoloItem> Detect(byte[] imageData)
        {
            return yoloWrapper.Detect(imageData);
        }

        #endregion
    }
}
