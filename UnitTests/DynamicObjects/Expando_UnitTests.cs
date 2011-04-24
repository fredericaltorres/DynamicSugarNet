using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class Expando_UnitTests {

        [TestMethod]
        public void ToExpando_FromOneAnonymousType() {

            var o = DS.Expando(
                new { ID = 1, Name = "Fred" }
            );
            Assert.AreEqual(1     , o.ID);
            Assert.AreEqual("Fred", o.Name);
        }
        [TestMethod]
        public void ToExpando_FromTwoAnonymousType() {

            var o = DS.Expando(
                new { ID  = 1  , Name    = "Fred"  },
                new { Age = 45 , Country = "FR"    }
            );
            Assert.AreEqual(1     , o.ID);
            Assert.AreEqual("Fred", o.Name);
            Assert.AreEqual(45    , o.Age);
            Assert.AreEqual("FR"  , o.Country);
        }
        [TestMethod]
        public void ToExpando_OneObject2PropertiesDefinedAsString() {

            var o = DS.Expando(
                "ID"    ,1      ,
                "Name"  ,"Fred"
            );
            Assert.AreEqual(1     , o.ID);
            Assert.AreEqual("Fred", o.Name);
        }
    }
}
