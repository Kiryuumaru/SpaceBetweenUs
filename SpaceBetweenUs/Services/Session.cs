using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public static class Session
    {
        public static Datastore Datastore;
        public static FrameSourceFile FrameSource;

        public static async Task Start(string frameSourceFile)
        {
            Datastore = await Datastore.Initialize();
            FrameSource = await FrameSourceFile.Initialize(frameSourceFile);
        }
    }
}
