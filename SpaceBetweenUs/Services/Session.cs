using Alturos.Yolo;
using SpaceBetweenUs.Services.Detectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public static class Session
    {
        public static Datastore Datastore { get; private set; }
        public static FrameSourceFile FrameSource { get; private set; }
        public static GridProjection GridProjection { get; private set; }
        public static bool UseGPU
        {
            get
            {
                string data = Datastore.GetValue("use_gpu");
                return data?.Equals("1") ?? false;
            }
            set
            {
                Datastore.SetValue("use_gpu", value ? "1" : "0");
            }
        }
        public static IHumanDetector HumanDetector { get; private set; }

        public static async Task Start(string frameSourceFile)
        {
            Datastore = await Datastore.Initialize();
            FrameSource = await FrameSourceFile.Initialize(frameSourceFile);
            GridProjection = await GridProjection.Initialize();
            try
            {
                HumanDetector = new YoloDetector(
                    FrameSource.Width,
                    FrameSource.Height,
                    Defaults.YoloConfig,
                    Defaults.YoloWeights,
                    Defaults.YoloNames,
                    new GpuConfig(),
                    new MLSystemValidator());
            }
            catch
            {
                HumanDetector = new YoloDetector(
                    FrameSource.Width,
                    FrameSource.Height,
                    Defaults.YoloConfig,
                    Defaults.YoloWeights,
                    Defaults.YoloNames,
                    null,
                    new MLSystemValidator());
            }
        }
    }
}
