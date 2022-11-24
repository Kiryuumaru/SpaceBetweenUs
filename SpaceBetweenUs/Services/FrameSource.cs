using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private VideoWriter writer;
        private readonly Stopwatch fpsTimer = new Stopwatch();
        private readonly List<(Mat mat, long delay)> writerBuffer = new List<(Mat mat, long fps)>();

        public Session Session { get; private set; }
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
                        Session = session,
                        capture = new VideoCapture(index),
                    };
                }
                else
                {
                    return new FrameSource
                    {
                        Session = session,
                        capture = new VideoCapture(session.Source),
                    };
                }
            });
        }

        public static List<CameraObject> GetAllConnectedCameras()
        {
            List<CameraObject> cameras = new List<CameraObject>();
            int index = 0;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (ManagementBaseObject device in searcher.Get())
                {
                    cameras.Add(new CameraObject(index, device["Caption"].ToString()));
                    index++;
                }
            }

            return cameras;
        }

        public async Task ReadFrame(Mat mat)
        {
            if (!await TryReadFrame(mat))
            {
                _ = capture.Set(VideoCaptureProperties.PosAviRatio, 0);
                _ = capture.Read(mat);
            }
        }

        public async Task<bool> TryReadFrame(Mat mat)
        {
            try
            {
                return await Task.Run(delegate
                {
                    _ = capture.Grab();
                    _ = capture.Grab();
                    _ = capture.Grab();
                    _ = capture.Grab();
                    _ = capture.Read(mat);
                    return !mat.Empty();
                });
            }
            catch
            {
                await Task.Delay(1000);
                return false;
            }
        }

        public void OutputFrame(Mat mat)
        {
            if (string.IsNullOrEmpty(Session.FileOutput)) return;
            if (writerBuffer.Count <= 10 && writer == null)
            {
                if (File.Exists(Session.FileOutput)) File.Delete(Session.FileOutput);
                var delay = fpsTimer.ElapsedMilliseconds;
                writerBuffer.Add((mat, delay));
                fpsTimer.Restart();
            }
            else if (writerBuffer.Count != 0 && writer == null)
            {
                //var avgFps = 1000 / writerBuffer.Skip(1).Average(i => i.delay);
                var avgFps = capture.Fps;
                writer = new VideoWriter(Session.FileOutput, FourCC.H264, avgFps, new Size(capture.FrameWidth, capture.FrameHeight));
                foreach (var buffer in writerBuffer) writer.Write(buffer.mat);
                writer.Write(mat);
                writerBuffer.Clear();
                fpsTimer.Stop();
            }
            else
            {
                writer.Write(mat);
            }
        }

        public void OutputFrameEnd()
        {
            writer?.Release();
            writer = null;
            writerBuffer.Clear();
            fpsTimer.Stop();
        }

        public void Stop()
        {
            capture.Release();
            capture.Dispose();
            capture = null;
        }
    }
}
