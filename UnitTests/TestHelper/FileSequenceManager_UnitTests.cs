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
        [TestMethod]
        public void FileSequenceManager()
        {
            using (var fileSequences = new FileSequenceManager(@"c:\temp\FileSequenceManager"))
            {
                var aFileName = fileSequences.CreateFile("toto", @"c:\temp\toto.txt");

                fileSequences.AddFile(aFileName, move: false);
                fileSequences.AddFile(aFileName, move: false);
                fileSequences.AddFile(aFileName, move: false);

                Assert.AreEqual(3, fileSequences.FileNames.Count);
            }
        }
    }
}
