using System;
using System.Collections.Generic;
using System.IO;
#if !MONOTOUCH
#endif

namespace DynamicSugar
{
    public class FileSequenceManager : TestFileHelper
    {
        public string TargetFolder { get; set; }
        public List<string> FileNames = new List<string>();

        private int _sequence = 0;

        public FileSequenceManager(string targetFolder = null, bool reCreateIfExists = true, bool cleanInTheEnd = true, string wildCard = "*.*") 
        {
            base.CleanInTheEnd = cleanInTheEnd;
            
            TargetFolder = targetFolder ?? base.GetTempPath();
            if (reCreateIfExists && Directory.Exists(TargetFolder))
                base.DeleteDirectory(TargetFolder);

            base.CreateDirectory(targetFolder);
            DirectoryToDelete.Add(TargetFolder);

            this.Load(wildCard);
        }

        public void Load(string wildCard = "*.*")
        {
            var files = Directory.GetFiles(TargetFolder, wildCard);
            FileNames.AddRange(files);
        } 

        private string GetSequencedFileName(int seq, string fileName)
        {
            var sequencedfileName = Path.Combine(TargetFolder, $"{seq:000000}{Path.GetExtension(fileName)}");
            return sequencedfileName;
        }

        public void AddFile(string fileName, bool move = true)
        {
            var sequencedfileName = GetSequencedFileName(_sequence++, fileName);
            FileNames.Add(sequencedfileName);
            if (move)
                File.Move(fileName, sequencedfileName);
            else
                File.Copy(fileName, sequencedfileName);
        }
    }
}
