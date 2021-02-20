using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class CameraObject
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public CameraObject(int index, string name)
        {
            Index = index;
            Name = name;
        }
        public override string ToString()
        {
            return Index + ":" + Name;
        }
    }

    public class FrameSource
    {
        private VideoCapture capture;

        public double Width => capture.FrameWidth;
        public double Height => capture.FrameHeight;

        private FrameSource() { }

        public static async Task<FrameSource> Initialize(Session session)
        {
            return await Task.Run(delegate
            {
                if (session.IsSourceFromCamera)
                {
                    int index = 0;
                    try
                    {
                        index = int.Parse(session.Source.Substring(0, session.Source.IndexOf(':')));
                    }
                    catch { }
                    return new FrameSource
                    {
                        capture = new VideoCapture(index),
                    };
                }
                else
                {
                    return new FrameSource
                    {
                        capture = new VideoCapture(session.Source),
                    };
                }
            });
        }

        public static List<CameraObject> GetAllConnectedCameras()
        {
            var cameras = new List<CameraObject>();
            int index = 0;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    cameras.Add(new CameraObject(index, device["Caption"].ToString()));
                    index++;
                }
            }

            return cameras;
        }

        public async Task ReadFrame(Mat mat)
        {
            try
            {
                await Task.Run(delegate
                {
                    capture.Grab();
                    capture.Grab();
                    capture.Grab();
                    capture.Grab();
                    capture.Read(mat);
                    if (mat.Empty())
                    {
                        capture.Set(VideoCaptureProperties.PosAviRatio, 0);
                        capture.Read(mat);
                    }
                });
            }
            catch { await Task.Delay(1000); }
        }

        public void Stop()
        {
            capture.Release();
            capture.Dispose();
            capture = null;
        }
    }
}
