using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using DynamicSugar.Experimental;

namespace DynamicSugarSharp_UnitTests {
    
    [TestClass]
    public class DynamicTextFileManager_UnitTests {

        static string TEST_PATH                = "";
        static string TEST_FILE_1              = "";
        static string TEST_FILE_2              = "";
        static string TEST_FILE_3_DO_NOT_EXIST = "";
        static string TEST_FILE_4              = "";
        const string  TEST_STRING              = "Hello World";
        
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) { 

            TEST_PATH = @"{0}\DynamicTextFileManager_UnitTests".format(Environment.GetEnvironmentVariable("TEMP"));

            if(System.IO.Directory.Exists(TEST_PATH))
                System.IO.Directory.Delete(TEST_PATH,true);
            
            System.IO.Directory.CreateDirectory(TEST_PATH);

            TEST_FILE_1              = @"{0}\TestFile1.txt".format(TEST_PATH);
            TEST_FILE_2              = @"{0}\TestFile2.txt".format(TEST_PATH);
            TEST_FILE_3_DO_NOT_EXIST = @"{0}\TestFile3.txt".format(TEST_PATH);
            TEST_FILE_4              = @"{0}\TestFile4.txt".format(TEST_PATH);
                        
            System.IO.File.WriteAllText(TEST_FILE_1, TEST_STRING);
            System.IO.File.WriteAllText(TEST_FILE_2, "{0}\r\n{1}".format(TEST_STRING, TEST_STRING));
        }        

        [TestMethod]
        public void ReadProperty() {

            var f = DynamicTextFileManager.Create(TEST_PATH, "txt");
            Assert.AreEqual(TEST_STRING, f.TestFile1);
        }
        [TestMethod]
        public void ReadProperty_TwoLine() {

            var f = DynamicTextFileManager.Create(TEST_PATH,"txt");
            Assert.AreEqual("{0}\r\n{1}".format(TEST_STRING, TEST_STRING), f.TestFile2);
        }
        [TestMethod,ExpectedException(typeof(DynamicTextFileManagerException))]
        public void ReadProperty_ThatDoNotExist() {

            var f       = DynamicTextFileManager.Create(TEST_PATH,"txt");
            string s    = f.TestFile3;
        }
        [TestMethod]
        public void WriteProperty() {

            var f       = DynamicTextFileManager.Create(TEST_PATH,"txt");
            f.TestFile4 = TEST_STRING;
            dynamic f2  = new DynamicTextFileManager(TEST_PATH,"txt");
            Assert.AreEqual(TEST_STRING, f2.TestFile4);
        }
        [TestMethod,ExpectedException(typeof(DynamicTextFileManagerException))]
        public void InitializeConstructor_InvalidExtension() {
            
            var f = DynamicTextFileManager.Create(TEST_PATH,"-*-");
        }
        [TestMethod,ExpectedException(typeof(DynamicTextFileManagerException))]
        public void WriteProperty_InvalidDueToExtension() {

            var f = DynamicTextFileManager.Create(TEST_PATH,"-*-");
            // TODO:Need to define a case
        }
    }
}
