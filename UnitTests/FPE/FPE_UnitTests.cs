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
        public void ResetConnection()
        {
            //var s = @".\Files\zCasData.dat";
            //var fNameB64 = FPE.sTob(s);
            var result2 = FPE.ResetConnection("LlxGaWxlc1x6Q2FzRGF0YS5kYXQ=");
            Assert.IsTrue(result2);
        }
    }
}
