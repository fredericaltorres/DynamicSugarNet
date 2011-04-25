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
    public class Dictionary_EM_UnitTests {

        [TestMethod]
        public void Substract_Dictionary() {

            var dic1        = DS.Dictionary( new { a=1, b=2, c=3, d=4, e=5  } );
            var dic2        = DS.Dictionary( new { a=1, c=3, e=5 } );
            var dic3        = dic1.Substract(dic2);
            var expectedDic = DS.Dictionary( new { b=2, d=4, } );
            
            DS.DictionaryHelper.AssertDictionaryEqual(expectedDic, dic3);
        }
        [TestMethod]
        public void Substract_list() {

            var dic1        = DS.Dictionary ( new { a=1, b=2, c=3, d=4, e=5 } );
            var dic3        = dic1.Substract( DS.List("a","c","e") );
            var expectedDic = DS.Dictionary ( new { b=2, d=4, } );            

            DS.DictionaryHelper.AssertDictionaryEqual(expectedDic, dic3);
        }
        [TestMethod]
        public void Add_DifferentTypeOfValues() {

            var dic1 = DS.Dictionary( new { a=1, b="B", c=33 }  ); // c=33 will be ovweritten
            var dic2 = DS.Dictionary( new { c=3, d=2.2 }  );
            var dic3 = DS.Dictionary( new { a=1, b="B", c=3, d=2.2 }  );
            var r    = dic1.Add(dic2);

            DS.DictionaryHelper.AssertDictionaryEqual(dic3, r);
        }
        [TestMethod]
        public void Add_DifferentTypeOfValues_DoNotOverWrite() {

            var dic1 = DS.Dictionary( new { a=1, b="B" }  );
            var dic2 = DS.Dictionary( new { b="BB", c=3, d=2.2 }  ); //b="BB" will be ignore
            var dic3 = DS.Dictionary( new { a=1, b="B", c=3, d=2.2 }  );                        

            DS.DictionaryHelper.AssertDictionaryEqual(dic3, dic1.Add(dic2, false));
        }
        [TestMethod]
        public void Add_Int() {

            var dic1 = DS.Dictionary<int>( new { a=1, b=2 }  );
            var dic2 = DS.Dictionary<int>( new { c=3, d=4 }  );
            var dic3 = DS.Dictionary<int>( new { a=1, b=2, c=3, d=4 }  );

            DS.DictionaryHelper.AssertDictionaryEqual(dic3, dic1.Add(dic2));
        }
        [TestMethod]
        public void Clone() {

            Dictionary<string, int> dic = DS.Dictionary<int>( new { a=1, b=2, c=3 } );
            DS.DictionaryHelper.AssertDictionaryEqual(dic, dic.Clone());
        }
        [TestMethod]
        public void Max_Int() {

            Dictionary<string, int> dic = DS.Dictionary<int>( new { a=1, b=2, c=3, d=4 } );
            Assert.AreEqual("d", dic.Max( DS.List("a","b","c","d")));
            Assert.AreEqual("b", dic.Max( DS.List("a","b")));
        }
        [TestMethod]
        public void Max_String() {

            Dictionary<string, int> dic = DS.Dictionary<int>( new { a="1", b="2", c="3", d="4" } );
            Assert.AreEqual("d", dic.Max( DS.List("a","b","c","d")));
            Assert.AreEqual("b", dic.Max( DS.List("a","b")));
        }        
    }
}
