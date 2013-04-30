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

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class DS_Compression {
        
        string STRING_REF = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        
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
