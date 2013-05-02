using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
using DynamicSugar.Compression;
using System.IO;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class DS_Compression {
        
        public const string STRING_REF = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        
        [TestMethod]
        public void GZipATextFile_UnGZipATextFile() {
            
            var fileName = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "DS_Compression.txt");
            File.WriteAllText(fileName, STRING_REF);

            var gzipFilename = DynamicSugar.Compression.GZip.GZipFile(fileName);
            Assert.IsTrue(File.Exists(gzipFilename));

            File.Delete(fileName);
            var newTextFileName = DynamicSugar.Compression.GZip.UnGZipFile(gzipFilename);
            Assert.IsTrue(File.Exists(newTextFileName));

            var text = System.IO.File.ReadAllText(newTextFileName);
            Assert.AreEqual(STRING_REF, text);
            
            //var textfile = @"C:\Users\frederic.torres\Desktop\TextHighlighterExtensionV30\TextHighlighterExtension2012\LanguageServices\Languages\PS1\GenerateIntellisense\Txt\Add-Computer.txt.gzip";
            //var aaa = DynamicSugar.Compression.GZip.UnGZipFile(textfile);

        }

        [TestMethod]
        public void ZipString() {
            
            var compressed = GZip.Zip(STRING_REF);
            Assert.AreEqual(STRING_REF, GZip.UnzipAsString(compressed));
        }

        [TestMethod]
        public void TestCompressionPerformance() {
            
            var refText        = DS.Resources.GetTextResource("24k.txt", Assembly.GetExecutingAssembly());
            var compressed     = GZip.Zip(refText);
            var originalSize   = refText.Length * 2;
            var compressedSize = compressed.Length;
            double ratio       = originalSize/compressedSize;

            Assert.AreEqual(refText, GZip.UnzipAsString(compressed));
        }

    }    
}
