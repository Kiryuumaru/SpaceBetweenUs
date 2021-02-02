using Alturos.Yolo;
using Alturos.Yolo.Model;
using SpaceBetweenUs.Services.Detectors;
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

        public string Name { get; private set; }
        public DowloadableFile ConfigFile { get; private set; }
        public DowloadableFile NamesFile { get; private set; }
        public DowloadableFile WeightsFile { get; private set; }
        public bool IsUsingGPU { get; private set; }

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
                    "YOLOv4",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov4.cfg",
                    "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names",
                    "https://github.com/AlexeyAB/darknet/releases/download/darknet_yolo_v3_optimal/yolov4.weights"),
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

        public IHumanDetector GetDetector(double frameWidth, double frameHeight, bool useGpu)
        {
            if (useGpu)
            {
                try
                {
                    return new YoloDetector(frameWidth, frameHeight,
                        new YoloConfiguration(ConfigFile.AbsolutePath, WeightsFile.AbsolutePath, NamesFile.AbsolutePath),
                        new GpuConfig(),
                        new MLSystemValidator());
                }
                catch { }
            }
            return new YoloDetector(frameWidth, frameHeight,
                new YoloConfiguration(ConfigFile.AbsolutePath, WeightsFile.AbsolutePath, NamesFile.AbsolutePath),
                null,
                new MLSystemValidator());
        }

        #endregion
    }
}
