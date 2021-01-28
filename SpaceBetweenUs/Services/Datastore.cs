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
        private static string fileContent = "";
        private static string filePath;
        private static bool isWriting = false;
        private static bool isInitialize = false;

        public static async Task Initialize()
        {
            if (isInitialize) return;
            if (Environment.GetEnvironmentVariable("USERPROFILE").Length >= 1)
            {
                filePath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "AppData", "Roaming", "SpaceBetweenUs");
            }
            else
            {
                filePath = Path.Combine("data");
            }
            await Task.Run(async delegate
            {
                while (isWriting) { await Task.Delay(100); }
                isWriting = true;
                try
                {
                    if (File.Exists(filePath)) fileContent = File.ReadAllText(filePath);
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
