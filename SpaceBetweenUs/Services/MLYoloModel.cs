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

        public static MLYoloModel YoloV3()
        {
            string name = "YOLOv3";
            string configUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov3.cfg";
            string namesFileUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names";
            string weightsFileUrl = "https://pjreddie.com/media/files/yolov3.weights";
            return FromUrl(name, configUrl, namesFileUrl, weightsFileUrl);
        }

        public static MLYoloModel YoloV3Tiny()
        {
            string name = "YOLOv3-tiny";
            string configUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov3-tiny.cfg";
            string namesFileUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names";
            string weightsFileUrl = "https://pjreddie.com/media/files/yolov3-tiny.weights";
            return FromUrl(name, configUrl, namesFileUrl, weightsFileUrl);
        }

        public static MLYoloModel YoloV2()
        {
            string name = "YOLOv2";
            string configUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov2.cfg";
            string namesFileUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/coco.names";
            string weightsFileUrl = "https://pjreddie.com/media/files/yolov2.weights";
            return FromUrl(name, configUrl, namesFileUrl, weightsFileUrl);
        }

        public static MLYoloModel YoloV2Tiny()
        {
            string name = "YOLOv2-tiny";
            string configUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/yolov2-tiny.cfg";
            string namesFileUrl = "https://raw.githubusercontent.com/AlexeyAB/darknet/master/cfg/voc.names";
            string weightsFileUrl = "https://pjreddie.com/media/files/yolov2-tiny.weights";
            return FromUrl(name, configUrl, namesFileUrl, weightsFileUrl);
        }

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
    }
}
