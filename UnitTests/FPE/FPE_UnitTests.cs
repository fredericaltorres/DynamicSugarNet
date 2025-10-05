using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;

namespace DynamicSugarSharp_UnitTests
{

    [TestClass]
    public class FPE_UnitTests
    {
        [TestMethod]
        public void sTob()
        {
            var s = "Hello!";
            var result = FPE.sTob(s);
            Assert.AreEqual("SGVsbG8h", result);
            var z = FPE.bTos(result);
            Assert.AreEqual(s, z);
        }

        [TestMethod]
        public void sTob_2()
        {
            var s = @".\zCasData.dat";
            var result = FPE.sTob(s);
            Assert.AreEqual("Llx6Q2FzRGF0YS5kYXQ=", result);
            var z = FPE.bTos(result);
            Assert.AreEqual(s, z);
        }

        [TestMethod]
        public void rAllT()
        {
            var s = @".\Files\zCasData.dat";
            var result = FPE.sTob(s);
            Assert.AreEqual("LlxGaWxlc1x6Q2FzRGF0YS5kYXQ=", result);
            var z = FPE.rAllt(result);
        }

        [TestMethod]
        public void rAlld()
        {
            var s = @".\Files\zCasData.dat";
            var result = FPE.sTob(s);
            var z = FPE.rAlld(result);
            Assert.AreEqual(1768588873, z);
        }

        [TestMethod]
        public void ResetConnection()
        {
            var s = @".\Files\zCasData.dat";
            var fNameB64 = FPE.sTob(s);
            var result2 = FPE.ResetConnection(fNameB64); // Is it time or Is Dark Side Of the Moon?
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void dTob()
        {
            var byteArray = FPE.dTob(new DateTime(1964, 12, 11));
            var str = string.Join(",", byteArray);
            Assert.AreEqual("208,67,125,246,255,255,255,255", str);
        }
    }
}
