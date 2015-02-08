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
    public class DS_Assert_GetTextResource {

        [TestMethod]
        public void AreEqualProperties_Poco() {

            var o = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            DS.Assert.AreEqualProperties(o, o);
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void AreEqualProperties_Poco_Fail() {

            var o1 = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            var o2 = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.1f };
            DS.Assert.AreEqualProperties(o1, o2);
        }

        [TestMethod]
        public void AreEqualProperties_Dictionary() {

            var o = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            DS.Assert.AreEqualProperties(DS.Dictionary(o), DS.Dictionary(o));
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void AreEqualProperties_Dictionary_Fail() {

            var o1 = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            var o2 = new { a=2, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.1f };
            DS.Assert.AreEqualProperties(DS.Dictionary(o1), DS.Dictionary(o2));
        }

    }
}
