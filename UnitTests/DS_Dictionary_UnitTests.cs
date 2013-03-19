using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;

namespace DynamicSugarSharp_UnitTests {

    //TODO:Try extension method to List<T>

    [TestClass]
    public class DS_Dictionary_UnitTests {
        [TestMethod]
        public void Dictionary_In() {

            var d1 = DS.Dictionary(new { a = 1, b = 2, c = 3 });
            Assert.IsTrue("a".In(d1));
            Assert.IsFalse("aaaa".In(d1));
        }
        [TestMethod]
        public void Dictionary_Identical() {

            var d1 = DS.Dictionary( new { a=1, b=2, c=3 } );
            Assert.IsTrue(DS.DictionaryHelper.Identical<string,object>(d1,d1));
            DS.DictionaryHelper.AssertDictionaryEqual(d1,d1);
        }
        [TestMethod,ExpectedException(typeof(DynamicSugarSharpException))]
        public void Dictionary_Identical_NegativeCase() {

            var d1 = DS.Dictionary( new { a=1, b=2, c=3 } );
            var d2 = DS.Dictionary( new { a=1, b=2, c="3" } );
            Assert.IsFalse(DS.DictionaryHelper.Identical(d1,d2));
            DS.DictionaryHelper.AssertDictionaryEqual(d1,d2);
        }
        [TestMethod]
        public void DictionaryFormat_StaticMemberAndExtensionMethod() {
            
            var expected = @"{ a:1, b:2, c:3 }";
            Assert.AreEqual(expected, DS.DictionaryHelper.Format(DS.Dictionary( new { a=1, b=2, c=3 } )));
            
            IDictionary<string, object> dic = DS.Dictionary( new { a=1, b=2, c=3 } );
            Assert.AreEqual(expected, dic.Format());
                        
            Dictionary<string, object> dic2 = DS.Dictionary( new { a=1, b=2, c=3 } );
            Assert.AreEqual(expected, dic2.Format());

            var dic3 = DS.Dictionary( new { a=1, b=2, c=3 } );
            Assert.AreEqual(expected, dic3.Format());

            Assert.AreEqual(expected, DS.Dictionary(  new { a=1, b=2, c=3 } ).Format());
        }
        
    }
}

