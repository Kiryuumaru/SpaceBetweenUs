using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBetweenUs.Services
{
    public class Datastore
    {
        private string filePath;
        private string fileContent;
        private bool isWriting = false;
        private int saveRequests = 0;

        private Datastore() { }

        public static async Task<Datastore> Initialize()
        {
            return await Task.Run(delegate
            {
                var datastore = new Datastore
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Datastore")
                };
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(datastore.filePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(datastore.filePath));
                    if (!File.Exists(datastore.filePath))
                        File.WriteAllText(datastore.filePath, "");
                    datastore.fileContent = File.ReadAllText(datastore.filePath);
                }
                catch { }
                return datastore;
            });
        }

        private void Save()
        {
            saveRequests++;
            if (isWriting) return;
            Task.Run(async delegate
            {
                isWriting = true;
                while (saveRequests > 0)
                {
                    try
                    {
                        string contentCopy = fileContent;
                        File.WriteAllText(filePath, contentCopy);
                        await Task.Delay(50);
                    }
                    catch { }
                    saveRequests--;
                }
                isWriting = false;
            });
        }

        public void SetValue(string key, string value)
        {
            fileContent = CommonHelpers.BlobSetValue(fileContent, key, value);
            Save();
        }

        public string GetValue(string key)
        {
            return CommonHelpers.BlobGetValue(fileContent, key);
        }
    }
}
