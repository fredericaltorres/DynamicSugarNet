using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;

namespace DynamicSugarSharp_UnitTests {

    //TODO:Try extension method to List<T>

    [TestClass]
    public class TestFileHelper_UnitTests
    {
        [TestMethod]
        public void GetTempFileName() 
        {
            var tfh = new TestFileHelper();
            var testFile1 = tfh.GetTempFileName();
            Assert.IsTrue(testFile1.EndsWith(".tmp"));

            var testFile2 = tfh.GetTempFileName(".json");
            Assert.IsTrue(testFile2.EndsWith(".json"));
            Assert.AreEqual(2, tfh.FileNamesToDelete.Count);
        }
        [TestMethod]
        public void CreateTempFile()
        {
            var tfh = new TestFileHelper();
            var testFile1 = tfh.CreateTempFile(@"{ ""a"" : 1 }", ".json");
            Assert.IsTrue(File.Exists(testFile1));

            var testFile2 = tfh.CreateTempFile(@"{ ""a"" : 1 }", "json");
            Assert.IsTrue(File.Exists(testFile2));

            Assert.AreEqual(2, tfh.FileNamesToDelete.Count);
            tfh.Clean();
            Assert.IsFalse(File.Exists(testFile1));
            Assert.IsFalse(File.Exists(testFile2));
            Assert.AreEqual(0, tfh.FileNamesToDelete.Count);
        }

        [TestMethod]
        public void CreateTempFile_Dispose()
        {
            var tfh = new TestFileHelper();
            var testFile1 = string.Empty;
            var testFile2 = string.Empty;
            using (tfh)
            {
                testFile1 = tfh.CreateTempFile(@"{ ""a"" : 1 }", ".json");
                Assert.IsTrue(File.Exists(testFile1));
                testFile2 = tfh.CreateTempFile(@"{ ""a"" : 1 }", "json");
                Assert.IsTrue(File.Exists(testFile2));
            }
            Assert.IsFalse(File.Exists(testFile1));
            Assert.IsFalse(File.Exists(testFile2));
            Assert.AreEqual(0, tfh.FileNamesToDelete.Count);

        }
    }
}
