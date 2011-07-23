using System;
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
    public class DS_Resource_GetTextResource {

        [TestMethod]
        public void GetTextResource() {

            var exepectedAlphabet = "ABCDEFGHIJKLMNOPQRSTVWXYZ";
            var alphabet = DS.Resources.GetTextResource("Alphabet.txt", Assembly.GetExecutingAssembly());
            Assert.AreEqual(exepectedAlphabet, alphabet);
        }
        [TestMethod]
        public void GetBitmapResource() {

            var b = DS.Resources.GetBitmapResource("EmbedBitmap.bmp", Assembly.GetExecutingAssembly());            
            Assert.AreEqual(100, b.Width);
            Assert.AreEqual(100, b.Height);
        }
        [TestMethod]
        public void GetBinaryResource() {

            var b = DS.Resources.GetBinaryResource("EmbedBitmap.bmp", Assembly.GetExecutingAssembly());            
            Assert.IsTrue(b.Length>0);            
            Assert.AreEqual(30054, b.Length);
        }
        [TestMethod]
        public void SaveBinaryResourceAsFiles() {

            string path = @"{0}\GetBinaryResource".format(Environment.GetEnvironmentVariable("TEMP"));
            var d = DS.Resources.SaveBinaryResourceAsFiles(Assembly.GetExecutingAssembly(), path, "EmbedBitmap.bmp", "Alphabet.txt");
            Assert.IsTrue(System.IO.File.Exists(d["EmbedBitmap.bmp"]));
            Assert.IsTrue(System.IO.File.Exists(d["Alphabet.txt"]));
        }
        [TestMethod]
        public void SaveBinaryResourceAsFile() {

            string path = @"{0}\GetBinaryResource".format(Environment.GetEnvironmentVariable("TEMP"));
            var f = DS.Resources.SaveBinaryResourceAsFile(Assembly.GetExecutingAssembly(), path, "EmbedBitmap.bmp");
            Assert.IsTrue(System.IO.File.Exists(f));
        }
    }    
}
