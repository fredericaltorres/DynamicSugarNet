using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class MultiValue_UnitTests {
    
        [TestMethod]
        public void MultiValues_AnonymousType() {

            dynamic mv = MultiValues.Values(new { Int=1, Double=2.0, String="Toto"});

            Assert.AreEqual(1     , mv.Int);
            Assert.AreEqual(2.0   , mv.Double);
            Assert.AreEqual("Toto", mv.String);

            Assert.AreEqual(1     , mv["Int"]);
            Assert.AreEqual(2.0   , mv["Double"]);
            Assert.AreEqual("Toto", mv["String"]);
        }
        [TestMethod]
        public void MultiValues_Dictionary() {

            var  values = new Dictionary<string, object>() {
                {"Int",1}, {"Double",2.0}, {"String","Toto"}
            };
            dynamic mv = MultiValues.Values(values);

            Assert.AreEqual(1     , mv.Int);
            Assert.AreEqual(2.0   , mv.Double);
            Assert.AreEqual("Toto", mv.String);
            Assert.AreEqual("Toto", mv.@String);
        }
        [TestMethod, ExpectedException(typeof(MultiValuesException))]
        public void MultiValues_CallingInvalidProperty() {

            dynamic mv = MultiValues.Values( new { Int=1, Double=2.0, String="Toto" } );
            int v      = mv.Bad;
        }
    }
}
