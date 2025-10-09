using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using FPE;


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

            var result = FPEClient.ResetConnection();
            Assert.IsFalse(result);

            object o = "foo";
            var result2 = o.ResetConnection();
            Assert.AreEqual(o.ToString(), result2.ToString());
        }
    }
}
