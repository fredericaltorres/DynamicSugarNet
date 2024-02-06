using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Web.UI.WebControls;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class FileSequenceManager_UnitTests
    {
        const string TestSequenceFolder = @"c:\temp\FileSequenceManager";

        [TestMethod]
        public void FileSequenceManager_CreateSequence()
        {
            using (var fileSequences = new FileSequenceManager(TestSequenceFolder))
            {
                var aFileName = fileSequences.CreateFile("toto", @"c:\temp\toto.txt");

                fileSequences.AddFile(aFileName, move: false);
                fileSequences.AddFile(aFileName, move: false);
                fileSequences.AddFile(aFileName, move: false);

                Assert.AreEqual(3, fileSequences.FileNames.Count);
            }
        }

        [TestMethod]
        public void FileSequenceManager_CreateSequence_ReloadSequence()
        {
            using (var fileSequences = new FileSequenceManager(TestSequenceFolder))
            {
                var aFileName = fileSequences.CreateFile("toto", @"c:\temp\toto.txt");

                fileSequences.AddFile(aFileName, move: false);
                fileSequences.AddFile(aFileName, move: false);
                fileSequences.AddFile(aFileName, move: false);

                Assert.AreEqual(3, fileSequences.FileNames.Count);

                using (var fileSequences2 = new FileSequenceManager(TestSequenceFolder, reCreateIfExists: false, cleanInTheEnd: false))
                {
                    Assert.AreEqual(3, fileSequences2.FileNames.Count);
                    Assert.AreEqual(@"c:\temp\FileSequenceManager\000000.txt", fileSequences2.FileNames[0]);
                }
            }
        }

        [TestMethod]
        public void FileSequenceManager_LoadSequenceFile()
        {
            var text = DS.Resources.GetTextResource("sequence.md", Assembly.GetExecutingAssembly());
            var tfh = new TestFileHelper();
            var localSequenceFileName = tfh.CreateTempFile(text, ".md");

            var fileSequences = new FileSequenceManager();
            var errors = fileSequences.LoadSequenceFile(localSequenceFileName, verifyExistenceOfFile: false);
            Assert.AreEqual(28, fileSequences.FileNames.Count);
        }

        [TestMethod]
        public void FileSequenceManager_LoadSequenceFile_FolderNotFound()
        {
            var text = DS.Resources.GetTextResource("sequence.folder.not.found.md", Assembly.GetExecutingAssembly());
            var tfh = new TestFileHelper();
            var localSequenceFileName = tfh.CreateTempFile(text, ".md");

            var fileSequences = new FileSequenceManager();
            var errors = fileSequences.LoadSequenceFile(localSequenceFileName, verifyExistenceOfFile: true);
            Assert.AreEqual(5, errors.Count);
        }


        [TestMethod]
        public void FileSequenceManager_LoadSequenceFile_FilesNotFound()
        {
            var text = DS.Resources.GetTextResource("sequence.file.not.found.md", Assembly.GetExecutingAssembly());
            var tfh = new TestFileHelper();
            var localSequenceFileName = tfh.CreateTempFile(text, ".md");

            var fileSequences = new FileSequenceManager();
            var errors = fileSequences.LoadSequenceFile(localSequenceFileName, verifyExistenceOfFile: true);
            Assert.AreEqual(4, errors.Count);
        }
    }
}
