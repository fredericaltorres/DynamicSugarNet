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
        public void ProcessTemplate()
        {
            var dic1 = DS.Dictionary(new { a = 1, b = 2, date = new DateTime(1964, 12, 11) });
            Assert.AreEqual("a=1, b=002, date=1964-12-11", dic1.PreProcess("a={a}, b={b:000}, date={date:yyyy-MM-dd}"));
        }
        [TestMethod]
        public void Include_AnonymousType()
        {
            var dic1 = DS.Dictionary(new { a = 1, b = 2, c = 3, d = 4, e = 5 });
            Assert.IsTrue(dic1.Include(dic1));
            Assert.IsTrue(dic1.Include(new { a = 1, b = 2, }));
            Assert.IsFalse(dic1.Include(new { a = 1, b = 2, c = 33 }));
            Assert.IsFalse(dic1.Include(new { a = 1, b = 2, d = 3 }));
        }
        [TestMethod]
        public void Include_Dictionary()
        {
            var dic1 = DS.Dictionary(new { a = 1, b = 2, c = 3, d = 4, e = 5 });
            Assert.IsTrue(dic1.Include(dic1));
            Assert.IsTrue(dic1.Include(DS.Dictionary(new { a = 1, b = 2, })));
            Assert.IsFalse(dic1.Include(DS.Dictionary(new { a = 1, b = 2, c=33})));
            Assert.IsFalse(dic1.Include(DS.Dictionary(new { a = 1, b = 2, d =3 })));            
        }
        [TestMethod]
        public void Include_List()
        {
            var dic1 = DS.Dictionary( new { a = 1, b = 2, c = 3, d = 4, e = 5 } );
            Assert.IsTrue(dic1.Include(DS.List("a", "b", "c")));
            Assert.IsTrue(dic1.Include("a", "b", "c"));
            Assert.IsFalse(dic1.Include(DS.List("a", "b", "c", "dd")));
            Assert.IsFalse(dic1.Include("a", "b", "c", "dd"));
        }
        [TestMethod]
        public void SystemWebRoutingRouteValueDictionary_Dictionary()
        {
            var dic1 = DS.Dictionary(new { a = 1, b = 2, c = 3 });
            var dic2 = DS.Dictionary(new Dictionary<string, object>() { { "a", 1 }, { "b", 2 }, { "c", 3 } } );
            dynamic dic3 = new ExpandoObject();
            dic3.a = 1;
            dic3.b = 2;
            dic3.c = 3;
            
            var dic4 = new System.Web.Routing.RouteValueDictionary(new { a = 1, b = 2, c = 3 });
            DS.DictionaryHelper.AssertDictionaryEqual(dic1, dic4);

            dic4 = new System.Web.Routing.RouteValueDictionary(new Dictionary<string, object>() { { "a", 1 }, { "b", 2 }, { "c", 3 } });
            DS.DictionaryHelper.AssertDictionaryEqual(dic1, dic4);

            dic4 = new System.Web.Routing.RouteValueDictionary(dic3);
            DS.DictionaryHelper.AssertDictionaryEqual(dic1, dic4);

            dic4 = new System.Web.Routing.RouteValueDictionary(TestDataInstanceManager.TestPersonInstance);
         
        }
        [TestMethod]
        public void Dictionary_Argument_AnonymousType_Dictionary_ExpandoObject()
        {
            var dic1 = DS.Dictionary(new { a = 1, b = 2, c = 3 });
            var dic2 = DS.Dictionary(
                new Dictionary<string, object>() { { "a",1 },{ "b",2 },{ "c",3 } } 
            );
            DS.DictionaryHelper.AssertDictionaryEqual(dic1, dic2);

            dynamic dic3 = new ExpandoObject();
            dic3.a = 1;
            dic3.b = 2;
            dic3.c = 3;
            DS.DictionaryHelper.AssertDictionaryEqual(dic1, dic3);
        }
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
            Assert.AreEqual("d", dic.Max());
            Assert.AreEqual("d", dic.Max( DS.List("a","b","c","d")));
            Assert.AreEqual("b", dic.Max( DS.List("a","b")));
        }
        [TestMethod]
        public void Min_Int()
        {
            Dictionary<string, int> dic = DS.Dictionary<int>(new { a = 1, b = 2, c = 3, d = 4 });
            Assert.AreEqual("a", dic.Min());
            Assert.AreEqual("b", dic.Min(DS.List("b", "c", "d")));            
        }
        [TestMethod]
        public void Max_String() {

            Dictionary<string, int> dic = DS.Dictionary<int>( new { a="1", b="2", c="3", d="4" } );
            Assert.AreEqual("d", dic.Max( DS.List("a","b","c","d")));
            Assert.AreEqual("b", dic.Max( DS.List("a","b")));
        }
    }
}
