using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public static class Datastore
    {
        private static string fileContent;
        private static string filePath;
        private static bool isWriting = false;
        private static bool isInitialize = false;

        public static async Task Initialize()
        {
            if (isInitialize) return;
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "Datastore");
            await Task.Run(async delegate
            {
                while (isWriting) { await Task.Delay(100); }
                isWriting = true;
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(filePath))) Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    if (!File.Exists(filePath)) File.WriteAllText(filePath, "");
                    fileContent = File.ReadAllText(filePath);
                }
                catch { }
                isWriting = false;
                isInitialize = true;
            });
        }

        private static void Save()
        {
            Task.Run(async delegate
            {
                while (isWriting) { await Task.Delay(100); }
                isWriting = true;
                try
                {
                    File.WriteAllText(filePath, fileContent);
                }
                catch { }
                isWriting = false;
            });
        }

        public static void SetValue(string key, string value)
        {
            fileContent = Helpers.BlobSetValue(fileContent, key, value);
            Save();
        }

        public static string GetValue(string key)
        {
            return Helpers.BlobGetValue(fileContent, key);
        }
    }
}
