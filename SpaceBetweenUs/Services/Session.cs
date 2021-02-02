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
        public static MLModel MLModel
        {
            get
            {
                string data = Datastore.GetValue("ml_model");
                return MLModel.GetMLModels().FirstOrDefault(i => i.Name.Equals(data));
            }
            set
            {
                Datastore.SetValue("ml_model", value.Name);
            }
        }

        public static async Task Start(string frameSourceFile)
        {
            Datastore = await Datastore.Initialize();
            FrameSource = await FrameSourceFile.Initialize(frameSourceFile);
            GridProjection = await GridProjection.Initialize();
            await InitializeHumanDetector();
        }

        public static async Task InitializeHumanDetector()
        {
            HumanDetector = null;
            await Task.Run(delegate
            {
                HumanDetector = MLModel?.GetDetector(FrameSource.Width, FrameSource.Height, UseGPU);
            });
        }

        public static void DisposeHumanDetector()
        {
            HumanDetector = null;
        }
    }
}
