using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Memory.Data.Past;

namespace DynamicSugarSharp_UnitTests
{
    [TestClass]
    public class FPEServer_UnitTests
    {
        [TestMethod]
        public void sTob_Test()
        {
            var s = "ResetConnection";
            var result = FPEServer.sTob(s);
        }

        [TestMethod]
        public void sTob()
        {
            var s = "Hello!";
            var result = FPEServer.sTob(s);
            Assert.AreEqual("SGVsbG8h", result);
            var z = FPEServer.bTos(result);
            Assert.AreEqual(s, z);
        }

        [TestMethod]
        public void sTob_2()
        {
            var s = @".\zCasData.dat";
            var result = FPEServer.sTob(s);
            Assert.AreEqual("Llx6Q2FzRGF0YS5kYXQ=", result);
            var z = FPEServer.bTos(result);
            Assert.AreEqual(s, z);
        }

        [TestMethod]
        public void rAllT()
        {
            var s = @".\Files\zCasData.dat";
            var result = FPEServer.sTob(s);
            Assert.AreEqual("LlxGaWxlc1x6Q2FzRGF0YS5kYXQ=", result);
            var z = FPEServer.rAllt(result);
        }

        [TestMethod]
        public void rAlld()
        {
            var s = @".\Files\zCasData.dat";
            var result = FPEServer.sTob(s);
            var z = FPEServer.rAlld(result);
            Assert.AreEqual(1768588873, z);
        }

        [TestMethod]
        public void dTob()
        {
            var byteArray = FPEServer.dTob(new DateTime(1964, 12, 11));
            var str = string.Join(",", byteArray);
            Assert.AreEqual("208,67,125,246,255,255,255,255", str);
        }
 
    }
}
