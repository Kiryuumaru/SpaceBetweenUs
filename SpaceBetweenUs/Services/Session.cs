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
        }
    }
}
