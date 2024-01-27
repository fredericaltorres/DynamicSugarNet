using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DynamicSugarSharp_UnitTests
{
    [TestClass]
    public class ManagerUnitTests
    {
        [TestMethod]
        public void TimeOutManager_Success_ThrowError()
        {
            var r = Managers.TimeOutManager("Test", 1, () => true);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void TimeOutManager_Success_NoThrowError()
        {
            var r = Managers.TimeOutManager("Test", 1, () => true, throwError: false);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void TimeOutManager_Fail_NoThrowError()
        {
            var r = Managers.TimeOutManager("Test", 1, () => false, throwError: false);
            Assert.IsFalse(r);
        }

        [TestMethod]
        [ExpectedException(typeof(ManagerTimeOutException))]
        public void TimeOutManager_Fail_ThrowError()
        {
            var r = Managers.TimeOutManager("Test", 0.1, () => false);
            Assert.IsTrue(r);
        }
    }
}