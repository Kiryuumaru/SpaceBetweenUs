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

        public void SetValue(string key, string value)
        {
            fileContent = Helpers.BlobSetValue(fileContent, key, value);
            Save();
        }

        public string GetValue(string key)
        {
            return Helpers.BlobGetValue(fileContent, key);
        }
    }
}
