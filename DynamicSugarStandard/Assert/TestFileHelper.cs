using System;
using System.Collections.Generic;
using System.IO;
#if !MONOTOUCH
#endif
using System.Reflection;

namespace DynamicSugar
{
    public class TestFileHelper : IDisposable
    {
        public List<string> FileNamesToDelete = new List<string>();
        public List<Exception> Exceptions = new List<Exception>();

        public bool DeleteFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    return true;
                }
                else return true;
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                return false;
            }
        }

        public void Clean()
        {
            var notDeletedFileName = new List<string>();
            foreach(var fileName in FileNamesToDelete)
            {
                if(!DeleteFile(fileName))
                    notDeletedFileName.Add(fileName);
            }
            FileNamesToDelete = notDeletedFileName;
        }

        public string GetTempFileName(string extension = null)
        {
            var fileName = Path.GetTempFileName();
            if (extension != null)
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;

                fileName += extension;
            }
                
            FileNamesToDelete.Add(fileName);
            return fileName;
        }

        public string CreateTempFile(string content, string extension = null)
        {
            var fileName = this.GetTempFileName(extension);
            File.WriteAllText(fileName, content);
            return fileName;
        }

        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public string GetExecutingPath()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return path;
        }

        public void Dispose()
        {
            this.Clean();
        }
    }
}
