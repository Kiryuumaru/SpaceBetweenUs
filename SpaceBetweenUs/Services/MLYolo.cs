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
        public bool UseGPU { get; private set; }
        public bool IsStarted => yoloWrapper != null;

        #endregion

        #region Initializer

        public MLYolo(MLYoloModel yoloModel, bool useGpu)
        {
            MLYoloModel = yoloModel;
            UseGPU = useGpu;
        }

        #endregion

        #region Methods

        public void Start()
        {
            var yoloConfig = new YoloConfiguration(MLYoloModel.ConfigFile.AbsolutePath, MLYoloModel.WeightsFile.AbsolutePath, MLYoloModel.NamesFile.AbsolutePath);
            var gpuConfig = UseGPU ? new GpuConfig() : null;
            var validator = new MLYoloSystemValidator();
            yoloWrapper = new YoloWrapper(yoloConfig, gpuConfig, validator);
        }

        public IEnumerable<YoloItem> Detect(byte[] imageData)
        {
            return yoloWrapper.Detect(imageData);
        }

        #endregion
    }
}
