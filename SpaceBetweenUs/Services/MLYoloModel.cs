using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class MLYoloModel
    {
        private static readonly string ModelsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Models");

        public string Name { get; private set; }
        public DowloadableFile ConfigFile { get; private set; }
        public DowloadableFile NamesFile { get; private set; }
        public DowloadableFile WeightsFile { get; private set; }
        public bool IsReady => ConfigFile.LocalExist && NamesFile.LocalExist && WeightsFile.LocalExist;

        private MLYoloModel() { }

        public static MLYoloModel FromUrl(string name, string configUrl, string namesFileUrl, string weightsFileUrl)
        {
            return new MLYoloModel()
            {
                Name = name,
                ConfigFile = new DowloadableFile(configUrl, Path.Combine(ModelsDirectory, name, Path.GetFileName(configUrl))),
                NamesFile = new DowloadableFile(namesFileUrl, Path.Combine(ModelsDirectory, name, Path.GetFileName(namesFileUrl))),
                WeightsFile = new DowloadableFile(weightsFileUrl, Path.Combine(ModelsDirectory, name, Path.GetFileName(weightsFileUrl)))
            };
        }

        public static MLYoloModel YoloV3 => FromUrl(
            "YOLOv3",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov3.cfg",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names",
            "https://pjreddie.com/media/files/yolov3.weights");
        public static MLYoloModel YoloV3Tiny => FromUrl(
            "YOLOv3Tiny",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov3-tiny.cfg",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names",
            "https://pjreddie.com/media/files/yolov3-tiny.weights");
        public static MLYoloModel YoloV2 => FromUrl(
            "YOLOv2",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov2.cfg",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names",
            "https://pjreddie.com/media/files/yolov2.weights");
        public static MLYoloModel YoloV2Tiny => FromUrl(
            "YOLOv2Tiny",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov2-tiny.cfg",
            "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/voc.names",
            "https://pjreddie.com/media/files/yolov2-tiny.weights");

        public override bool Equals(object obj)
        {
            if (obj is MLYoloModel yoloModel)
            {
                return Name.Equals(yoloModel.Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(MLYoloModel left, MLYoloModel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MLYoloModel left, MLYoloModel right)
        {
            return !(left == right);
        }

        public async void DownloadDatasets(Action<OverallDowloadableFileOnProgress> onProgress)
        {
            var dfs = new DowloadableFile[]
            {
                ConfigFile,
                NamesFile,
                WeightsFile
            };
            await DowloadableFile.Download(dfs, onProgress);
        }
    }
}
