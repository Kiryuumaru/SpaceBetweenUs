using Alturos.Yolo;
using Alturos.Yolo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class MLModel
    {
        private static readonly string ModelsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Models");
        private YoloWrapper yoloWrapper;

        public string Name { get; private set; }
        public DowloadableFile ConfigFile { get; private set; }
        public DowloadableFile NamesFile { get; private set; }
        public DowloadableFile WeightsFile { get; private set; }
        public bool IsUsingGPU { get; private set; }
        public bool IsReady => yoloWrapper != null;

        private MLModel() { }

        #region Statics

        public static MLModel FromUrl(string name, string configUrl, string namesFileUrl, string weightsFileUrl)
        {
            return new MLModel()
            {
                Name = name,
                ConfigFile = new DowloadableFile(configUrl, Path.Combine(ModelsDirectory, name, Path.GetFileName(configUrl))),
                NamesFile = new DowloadableFile(namesFileUrl, Path.Combine(ModelsDirectory, name, Path.GetFileName(namesFileUrl))),
                WeightsFile = new DowloadableFile(weightsFileUrl, Path.Combine(ModelsDirectory, name, Path.GetFileName(weightsFileUrl)))
            };
        }

        public static IEnumerable<MLModel> GetMLModels()
        {
            return new List<MLModel>()
            {
                FromUrl(
                    "YOLOv3",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov3.cfg",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names",
                    "https://pjreddie.com/media/files/yolov3.weights"),
                FromUrl(
                    "YOLOv3-Tiny",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov3-tiny.cfg",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names",
                    "https://pjreddie.com/media/files/yolov3-tiny.weights"),
                FromUrl(
                    "YOLOv2",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov2.cfg",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names",
                    "https://pjreddie.com/media/files/yolov2.weights"),
                FromUrl(
                    "YOLOv2-Tiny",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov2-tiny.cfg",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/voc.names",
                    "https://pjreddie.com/media/files/yolov2-tiny.weights")
            };
        }

        #endregion

        #region Methods

        public IEnumerable<DowloadableFile> GetAllDownloadableFiles()
        {
            return new DowloadableFile[] { ConfigFile, NamesFile, WeightsFile };
        }

        public async void Download(Action<OverallDowloadableFileOnProgress> onProgress)
        {
            var dfs = new DowloadableFile[]
            {
                ConfigFile,
                NamesFile,
                WeightsFile
            };
            await DowloadableFile.Download(dfs, onProgress);
        }

        public YoloWrapper Start(bool useGpu)
        {
            return new YoloWrapper(
                new YoloConfiguration(ConfigFile.AbsolutePath, WeightsFile.AbsolutePath, NamesFile.AbsolutePath),
                useGpu ? new GpuConfig() : null,
                new MLSystemValidator());
        }

        public void Stop()
        {
            yoloWrapper = null;
        }

        public IEnumerable<YoloItem> Detect(byte[] imageData)
        {
            if (yoloWrapper == null) return new List<YoloItem>();
            return yoloWrapper.Detect(imageData);
        }

        #endregion
    }
}
