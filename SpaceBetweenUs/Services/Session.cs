using OpenCvSharp;
using SpaceBetweenUs.Services.Detectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class Session
    {
        private static readonly List<string> sourcesOpen = new List<string>();
        public string Source { get; private set; }
        public string FileOutput { get; private set; }
        public bool IsSourceFromCamera { get; private set; }
        public string Name { get; private set; }
        public Datastore Datastore { get; private set; }
        public FrameSource FrameSource { get; private set; }
        public GridProjection GridProjection { get; private set; }
        public Logger Logger { get; private set; }
        public HumanDetector HumanDetector { get; private set; }

        private Session() { }
        public static async Task<Session> Start(string source, string name, string fileOutput = null)
        {
            if (sourcesOpen.Any(i => i.Equals(source)))
            {
                return null;
            }

            Session session = new Session()
            {
                IsSourceFromCamera = false,
                Source = source,
                FileOutput = fileOutput,
                Name = name
            };
            session.Datastore = await Datastore.Initialize(session);
            session.FrameSource = await FrameSource.Initialize(session);
            session.GridProjection = await GridProjection.Initialize(session);
            session.Logger = await Logger.Initialize(session);
            session.HumanDetector = await HumanDetector.Initialize(session);
            sourcesOpen.Add(source);
            return session;
        }

        public static async Task<Session> Start(CameraObject cameraObject, string name, string fileOutput = null)
        {
            if (sourcesOpen.Any(i => i.Equals(cameraObject.Name)))
            {
                return null;
            }

            Session session = new Session()
            {
                IsSourceFromCamera = true,
                Source = cameraObject.ToString(),
                FileOutput = fileOutput,
                Name = name
            };
            session.Datastore = await Datastore.Initialize(session);
            session.FrameSource = await FrameSource.Initialize(session);
            session.GridProjection = await GridProjection.Initialize(session);
            session.Logger = await Logger.Initialize(session);
            session.HumanDetector = await HumanDetector.Initialize(session);
            sourcesOpen.Add(cameraObject.ToString());
            return session;
        }

        public static bool HasSourceOpen(string source) => sourcesOpen.Any(i => i.Equals(source));

        public void Stop()
        {
            FrameSource.Stop();
            _ = sourcesOpen.RemoveAll(i => i.Equals(Source));
        }
    }
}
