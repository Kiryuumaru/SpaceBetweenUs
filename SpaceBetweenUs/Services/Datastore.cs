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
        private static string generalFilePath;
        private static string generalFileContent;
        private static bool generalIsWriting = false;
        private static int generalSaveRequests = 0;
        private string filePath;
        private string fileContent;
        private bool isWriting = false;
        private int saveRequests = 0;

        private Datastore() { }

        public static async Task<Datastore> Initialize(Session session)
        {
            return await Task.Run(delegate
            {
                Datastore datastore = new Datastore
                {
                    filePath = Path.Combine("Session", session.Name, "Datastore")
                };
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(datastore.filePath)))
                    {
                        _ = Directory.CreateDirectory(Path.GetDirectoryName(datastore.filePath));
                    }

                    if (!File.Exists(datastore.filePath))
                    {
                        File.WriteAllText(datastore.filePath, "");
                    }

                    datastore.fileContent = File.ReadAllText(datastore.filePath);
                }
                catch { }
                return datastore;
            });
        }

        private void Save()
        {
            saveRequests++;
            if (isWriting)
            {
                return;
            }

            isWriting = true;
            _ = Task.Run(async delegate
              {
                  while (saveRequests > 0)
                  {
                      try
                      {
                          string contentCopy = fileContent;
                          File.WriteAllText(filePath, contentCopy);
                          await Task.Delay(500);
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

        private static void GeneralSave()
        {
            generalSaveRequests++;
            if (generalIsWriting)
            {
                return;
            }

            generalIsWriting = true;
            _ = Task.Run(async delegate
              {
                  while (generalSaveRequests > 0)
                  {
                      try
                      {
                          string contentCopy = generalFileContent;
                          File.WriteAllText(generalFilePath, contentCopy);
                          await Task.Delay(500);
                      }
                      catch { }
                      generalSaveRequests--;
                  }
                  generalIsWriting = false;
              });
        }

        public static void GeneralSetValue(string key, string value)
        {
            InitializeGeneralDatastore();
            generalFileContent = CommonHelpers.BlobSetValue(generalFileContent, key, value);
            GeneralSave();
        }

        public static string GeneralGetValue(string key)
        {
            InitializeGeneralDatastore();
            return CommonHelpers.BlobGetValue(generalFileContent, key);
        }

        private static void InitializeGeneralDatastore()
        {
            if (string.IsNullOrEmpty(generalFilePath))
            {
                generalFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Session", "GeneralDatastore");
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(generalFilePath)))
                    {
                        _ = Directory.CreateDirectory(Path.GetDirectoryName(generalFilePath));
                    }

                    if (!File.Exists(generalFilePath))
                    {
                        File.WriteAllText(generalFilePath, "");
                    }

                    generalFileContent = File.ReadAllText(generalFilePath);
                }
                catch { }
            }
        }
    }
}
