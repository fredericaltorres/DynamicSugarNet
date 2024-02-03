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
        public List<string> DirectoryToDelete = new List<string>();
        public List<Exception> Exceptions = new List<Exception>();

        public bool CleanInTheEnd { get; set; } = true;

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


        public bool CreateDirectory(string dirName)
        {
            try
            {
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
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

        public bool DeleteDirectory(string dirName)
        {
            try
            {
                if (Directory.Exists(dirName))
                {
                    Directory.Delete(dirName, true);
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

        public static void RemoveReadOnlyAttribute(string fileName)
        {
            if (File.Exists(fileName))
            {
                var f = new FileInfo(fileName);
                if (f.IsReadOnly)
                    f.IsReadOnly = false;
            }
        }

        public void Clean(bool removeReadOnlyFileAtrribute = false)
        {
            var notDeletedFileName = new List<string>();
            foreach(var fileName in FileNamesToDelete)
            {
                if(removeReadOnlyFileAtrribute)
                    RemoveReadOnlyAttribute(fileName);

                if(!DeleteFile(fileName))
                    notDeletedFileName.Add(fileName);
            }
            FileNamesToDelete = notDeletedFileName;

            foreach (var dir in this.DirectoryToDelete)
            {
                DeleteDirectory(dir);
            }
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

        public string CreateFile(string content, string fileName, string extension = null)
        {
            this.FileNamesToDelete.Add(fileName);
            File.WriteAllText(fileName, content);
            return fileName;
        }

        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public string GetTempFolder(string subDirName = null)
        {
            if(subDirName == null)
                subDirName = Environment.TickCount.ToString();
            var p = Path.Combine(GetTempPath(), subDirName);
            Directory.CreateDirectory(p);
            this.DirectoryToDelete.Add(p);
            return p;
        }

        public string GetExecutingPath()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return path;
        }

        public void Dispose()
        {
            if (CleanInTheEnd)
                this.Clean();
        }
    }
}
