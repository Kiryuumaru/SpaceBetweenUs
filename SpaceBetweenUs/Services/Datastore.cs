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

        private static void Initialize()
        {
            if (Environment.GetEnvironmentVariable("USERPROFILE").Length >= 1)
            {
                filePath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "AppData", "Roaming", "SpaceBetweenUs");
            }
            else
            {
                filePath = Path.Combine("data");
            }
            Task.Run(async delegate
            {
                while (isWriting) { await Task.Delay(100); }
                isWriting = true;
                try
                {
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
            if (!isInitialize) Initialize();
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
            if (!isInitialize) Initialize();
            fileContent = Helpers.BlobSetValue(fileContent, key, value);
            Save();
        }

        public static string GetValue(string key)
        {
            if (!isInitialize) Initialize();
            return Helpers.BlobGetValue(fileContent, key);
        }
    }
}
