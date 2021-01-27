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

        private readonly YoloWrapper yoloWrapper;
        public MLYoloModel MLYoloModel { get; private set; }

        #endregion

        #region Initializer

        public MLYolo(MLYoloModel yoloModel, bool useGpu)
        {
            MLYoloModel = yoloModel;
            yoloWrapper = new YoloWrapper(
                new YoloConfiguration(MLYoloModel.ConfigFile.AbsolutePath, MLYoloModel.WeightsFile.AbsolutePath, MLYoloModel.NamesFile.AbsolutePath),
                useGpu ? new GpuConfig() : null,
                new MLYoloSystemValidator());
        }

        #endregion

        #region Methods

        public IEnumerable<YoloItem> Detect(byte[] imageData)
        {
            return yoloWrapper.Detect(imageData);
        }

        #endregion
    }
}
