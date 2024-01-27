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
    public class ManagerRetryUnitTests
    {
        [TestMethod]
        public void Retry_ReturnTrue_NoError()
        {
            Assert.IsTrue(Managers.Retry(() => true));
        }

        [TestMethod]
        public void Retry_ReturnFalse_NoError()
        {
            Assert.IsFalse(Managers.Retry(() => false));
        }

        [TestMethod]
        public void Retry_CalleeThrowExceptionAllTheTime_DoNotThrow_ReturnDefaultValue_WithOnExceptionHandler()
        {
            var exceptionCount = 0;
            Assert.IsFalse(
                Managers.Retry(
                    () => {
                        throw new ArgumentException();
                        return true;
                    }, 
                    sleepTimeInMinute: 0.02, 
                    onException: (ex) => {
                        exceptionCount++;
                    }
                )
            );
            Assert.AreEqual(3, exceptionCount);
        }

        [TestMethod]
        public void Retry_CalleeThrowExceptionAllTheTime_DoNotThrow_ReturnDefaultValue()
        {
            Assert.IsFalse(
                Managers.Retry(
                    () => {
                        throw new ArgumentException();
                        return true;
                    },
                    sleepTimeInMinute: 0.02
                )
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ManagerReTryException))]
        public void Retry_CalleeThrowExceptionAllTheTime_NotThrow()
        {
            Assert.IsFalse(
                Managers.Retry(
                    () => {
                        throw new ArgumentException();
                        return true;
                    }, 
                    sleepTimeInMinute: 0.02, 
                    throwException: true
                )
            );
        }
    }
}