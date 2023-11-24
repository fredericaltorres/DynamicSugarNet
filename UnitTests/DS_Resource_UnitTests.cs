using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DynamicSugarSharp_UnitTests {

    //TODO:Try extension method to List<T>

    [TestClass]
    public class DS_Resource_GetTextResource {

        [TestMethod]
        public void GetMultipleGzipTextResource(){ 

            var files = DS.Resources.GetTextResource(new Regex("DS_Compression.txt.gzip", RegexOptions.IgnoreCase), Assembly.GetExecutingAssembly(), true, DS.TextResourceEncoding.UTF8);
            Assert.AreEqual(DS_Compression.STRING_REF, files["DynamicSugar_UnitTests.Files.DS_Compression.txt.gzip"]);
        }

        [TestMethod]
        public void GetMultipleGzipTextResource_3File(){ 

            // var text = DynamicSugar.Compression.GZip.UnGZipFile(@"C:\DVT\.NET\DynamicSugar\Source\DynamicSugarNet\UnitTests\Files\%.txt.gzip");

            var files = DS.Resources.GetTextResource(new Regex("(ac|%).txt.gzip$", RegexOptions.IgnoreCase), Assembly.GetExecutingAssembly(), true);
            
            Assert.AreEqual(2, files.Count);
            
            Assert.IsTrue(files["DynamicSugar_UnitTests.Files.ac.txt.gzip"].Contains("Adds content to the specified items, such as adding words to a file."));
            Assert.IsTrue(files["DynamicSugar_UnitTests.Files.%.txt.gzip"].Contains("ForEach-Object"));
        }

        [TestMethod]
        public void GetGzipTextResource() {

            var text = DS.Resources.GetTextResource("DS_Compression.txt.gzip", Assembly.GetExecutingAssembly(), true, DS.TextResourceEncoding.UTF8);
            Assert.AreEqual(DS_Compression.STRING_REF, text);
        }

        [TestMethod]
        public void GetMultipleTextResource() {
            
            var alphabetDic = DS.Resources.GetTextResource(new Regex("DataClasses.Alphabet", RegexOptions.IgnoreCase), Assembly.GetExecutingAssembly());
            Assert.AreEqual(3, alphabetDic.Count);
            foreach(var e in alphabetDic) {
                Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTVWXYZ", alphabetDic[e.Key]);
            }
        }

        [TestMethod]
        public void GetTextResource() {

            var exepectedAlphabet = "ABCDEFGHIJKLMNOPQRSTVWXYZ";
            var alphabet = DS.Resources.GetTextResource("Alphabet.txt", Assembly.GetExecutingAssembly());
            Assert.AreEqual(exepectedAlphabet, alphabet);
        }

//#if !DYNAMIC_SUGAR_STANDARD
//        [TestMethod]
//        public void GetBitmapResource() {

//            var b = DS.Resources.GetBitmapResource("EmbedBitmap.bmp", Assembly.GetExecutingAssembly());            
//            Assert.AreEqual(100, b.Width);
//            Assert.AreEqual(100, b.Height);
//        }
//#endif

        [TestMethod]
        public void GetBinaryResource() {

            var b = DS.Resources.GetBinaryResource("EmbedBitmap.bmp", Assembly.GetExecutingAssembly());            
            Assert.IsTrue(b.Length>0);            
            Assert.AreEqual(30054, b.Length);
        }
        [TestMethod]
        public void SaveBinaryResourceAsFiles() {

            string path = @"{0}\GetBinaryResource".FormatString(Environment.GetEnvironmentVariable("TEMP"));
            var d = DS.Resources.SaveBinaryResourceAsFiles(Assembly.GetExecutingAssembly(), path, "EmbedBitmap.bmp", "Alphabet.txt");
            Assert.IsTrue(System.IO.File.Exists(d["EmbedBitmap.bmp"]));
            Assert.IsTrue(System.IO.File.Exists(d["Alphabet.txt"]));
        }
        [TestMethod]
        public void SaveBinaryResourceAsFile() {

            string path = @"{0}\GetBinaryResource".FormatString(Environment.GetEnvironmentVariable("TEMP"));
            var f = DS.Resources.SaveBinaryResourceAsFile(Assembly.GetExecutingAssembly(), path, "EmbedBitmap.bmp");
            Assert.IsTrue(System.IO.File.Exists(f));
        }
    }    
}
