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

        public FileSequenceManager()
        {
        }

        public FileSequenceManager(string targetFolder = null, bool reCreateIfExists = true, bool cleanInTheEnd = true, string wildCard = "*.*", int sequence = 0) 
        {
            base.CleanInTheEnd = cleanInTheEnd;
            
            _sequence = sequence;
            TargetFolder = targetFolder ?? base.GetTempPath();
            if (reCreateIfExists && Directory.Exists(TargetFolder))
                base.DeleteDirectory(TargetFolder);

            base.CreateDirectory(targetFolder);
            DirectoryToDelete.Add(TargetFolder);

            this.Load(wildCard);
        }

        public List<string> LoadSequenceFile(string filename, bool verifyExistenceOfFile)
        {
            var errors = new List<string>();
            var text = File.ReadAllText(filename);
            var lines = text.SplitByCRLF();
            var lineIndex = 0;
            while ( lineIndex < lines.Count)
            {
                var line = lines[lineIndex];
                if (line.Trim() == "" || line.Trim().StartsWith("//"))
                {
                    lineIndex++;
                    continue;
                }
                else if (line.Trim() == "#directory")
                {
                    line = lines[++lineIndex];
                    this.TargetFolder = line;
                    if(verifyExistenceOfFile && !Directory.Exists(this.TargetFolder))
                        errors.Add($"Directory does not exist {this.TargetFolder}");
                }
                else if (line.Trim() == "#sequence")
                {
                }
                else
                {
                    var p = Path.Combine(this.TargetFolder, line);
                    if (verifyExistenceOfFile)
                    {
                        if (File.Exists(p))
                            this.FileNames.Add(p);
                        else
                            errors.Add($"File not found: {p}");
                    }
                    else
                    {
                        this.FileNames.Add(p);
                    }
                }
                lineIndex++;
            }
            return errors;
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
