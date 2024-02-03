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
    public class ManagerTimeOutGenericUnitTests
    {
        public class Foo
        {
            public int Count { get; set; } = 0;
            public bool Completed => this.Count > 1;
        }

        [TestMethod]
        public void TimeOutManager_Generic_Success()
        {
            var foo = new Foo();
            var finalFoo = Managers.TimeOutManager<Foo>("Test", 1, () => {

                foo.Count += 1;
                return foo.Completed ? foo : null;
            });
            Assert.IsTrue(finalFoo.Completed);
            Assert.AreEqual(2, finalFoo.Count);
        }

        [TestMethod]
        public void TimeOutManager_Generic_Fail_NoThrowError()
        {
            var foo = new Foo();
            var finalFoo = Managers.TimeOutManager<Foo>("Test", 0.3, () => {

                return foo.Completed ? foo : null;
            }, throwError: false);
            Assert.AreEqual(null, finalFoo);
        }

        [TestMethod, ExpectedException(typeof(ManagerTimeOutException))]
        public void TimeOutManager_Generic_Fail_ThrowError()
        {
            var foo = new Foo();
            var finalFoo = Managers.TimeOutManager<Foo>("Test", 0.3, () => {

                return foo.Completed ? foo : null;
            }, throwError: true);
        }
    }
}