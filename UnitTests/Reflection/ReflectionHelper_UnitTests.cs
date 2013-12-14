using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class ReflectionHelper_UnitTests {

        public class FredPropertyClass {

            public string FieldString { get; set; }
            public DateTime FieldDate { get; set; }
            public double FieldDouble { get; set; }
            public bool FieldBool { get; set; }

            public  FredPropertyClass() {

                FieldString = "Hello World";
                FieldDate = DateTime.Parse("12/13/2013 11:15:35 PM");
                FieldDouble = 12.34;
                FieldBool = true;
            }

            public int this[int index] {
                get {
                    return index*2;
                }
            }
        }

        public class IndexerTestClass {

            private int _index;
            public int this[int index] {
                get{
                    return this._index;
                }
                set {
                    this._index = index*value;
                }
            }
        }

        class ConsTest1 {
            public string Name = "Toto";
        }
        class ConsTest2 {
            public string Name = "Toto";

            public ConsTest2() { 
            }
            public ConsTest2(string name) {
                this.Name = name;
            }
        }

        [TestMethod]
        public void Indexer_Set() {
            
            var f = new IndexerTestClass();
            ReflectionHelper.SetIndexer(f, 2, 123);
            Assert.AreEqual(2*123, ReflectionHelper.GetIndexer(f, 1));
        }

        [TestMethod]
        public void Indexer_Get() {
            
            var f = new FredPropertyClass();
            Assert.AreEqual(2, ReflectionHelper.GetIndexer(f, 1));
            Assert.AreEqual(16, ReflectionHelper.GetIndexer(f, 8));
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Indexer_Get_NoIndexerDefined() {
            
            var f = new ConsTest2();
            ReflectionHelper.GetIndexer(f, 1);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Indexer_Get_FromNull() {
                        
            ReflectionHelper.GetIndexer(null, 1);
        }

        [TestMethod]
        public void Constructor_MultipleConstructor_GenericMethod() {
            
            var consTestInstance = ReflectionHelper.Constructor<ConsTest2>(typeof(ConsTest2));
            Assert.AreEqual("Toto", consTestInstance.Name);

            var consTestInstance2 = ReflectionHelper.Constructor<ConsTest2>(typeof(ConsTest2), "Tata");
            Assert.AreEqual("Tata", consTestInstance2.Name);
        }

        [TestMethod]
        public void Constructor_MultipleConstructor() {

            var consTestInstance = ReflectionHelper.Constructor(typeof(ConsTest2)) as ConsTest2;
            Assert.AreEqual("Toto", consTestInstance.Name);

            var consTestInstance2 = ReflectionHelper.Constructor(typeof(ConsTest2), "Tata") as ConsTest2;
            Assert.AreEqual("Tata", consTestInstance2.Name);
        }

        [TestMethod, ExpectedException(typeof(MissingMethodException))]
        public void Constructor_ConstructorWithInvalidParameters() {

            var consTestInstance = ReflectionHelper.Constructor(typeof(ConsTest2), "Tata", true);
        }

        [TestMethod]
        public void Constructor_DefaultConstructor() {

            var consTestInstance = ReflectionHelper.Constructor(typeof(ConsTest1)) as ConsTest1;
            Assert.AreEqual("Toto", consTestInstance.Name);
        }

        private Dictionary<string, ParameterMetadata> MyMethod_ParameterMetadata(int i, double d, string s){

            var dic = ReflectionHelper.GetLocalsEx(i, d, s);
            Assert.AreEqual(1  , dic["i"].Value);
            Assert.AreEqual(2.0, dic["d"].Value);
            Assert.AreEqual("A", dic["s"].Value);
            return dic;
        }
      
        [TestMethod]
        public void GetLocalsEx() {

            var d = MyMethod_ParameterMetadata(1, 2.0, "A");
        }
        private Dictionary<string, object> MyMethod(int i, double d, string s){

            var dic = ReflectionHelper.GetLocals(i, d, s);
            return dic;
        }
        [TestMethod]
        public void GetLocals() {

            var dic = MyMethod(1, 2.0, "A");

            Assert.AreEqual(@"{ i:1, d:2, s:""A"" }", dic.Format());
            Assert.AreEqual(1  , dic["i"]);
            Assert.AreEqual(2.0, dic["d"]);
            Assert.AreEqual("A", dic["s"]);
        }
        
        [TestMethod]
        public void CloneDictionary() {

            var d = new Dictionary<string, object>() {
                { "A", 1 },
                { "B", 2.0 },
                { "C", "3" }
            };            
            var d2 = ReflectionHelper.CloneDictionary<string, object>(d);

            Assert.AreEqual(1  , d2["A"]);
            Assert.AreEqual(2.0, d2["B"]);
            Assert.AreEqual("3", d2["C"]);
        }

        [TestMethod]
        public void DictionaryViaReflection() {

            var d = new Dictionary<string, int>() {
                { "A", 1 },
                { "B", 2 },
                { "C", 3 }
            };            
        }
        [TestMethod]
        public void GetListType() {
            
            var li = new List<int>();
            Assert.AreEqual(typeof(int),ReflectionHelper.GetListType(li.GetType()));

            var ls = new List<string>();
            Assert.AreEqual(typeof(string),ReflectionHelper.GetListType(ls.GetType()));
            
            var aString = "";
            Assert.AreNotEqual(typeof(string),ReflectionHelper.GetListType(aString.GetType()));
        }
        [TestMethod]   
        public void GetDictionaryType_StringInt() {
            
            var dsi = new Dictionary<string, int>();
            Type keyType, valueType;
            ReflectionHelper.GetDictionaryType(dsi.GetType(), out keyType, out valueType);
            Assert.AreEqual(typeof(string), keyType);
            Assert.AreEqual(typeof(int), valueType);
        }
        [TestMethod]   
        public void GetDictionaryType_DoubleDateTime() {
            
            var dsi = new Dictionary<double, DateTime>();
            Type keyType, valueType;
            ReflectionHelper.GetDictionaryType(dsi.GetType(), out keyType, out valueType);
            Assert.AreEqual(typeof(double), keyType);
            Assert.AreEqual(typeof(DateTime), valueType);
        }
        [TestMethod]
        public void IsTypeListOfT() {

            var o = 1;
            Assert.IsFalse(ReflectionHelper.IsTypeListOfT(o.GetType()));

            var li = new List<int>();
            Assert.IsTrue(ReflectionHelper.IsTypeListOfT(li.GetType()));

            var ld = new List<double>();
            Assert.IsTrue(ReflectionHelper.IsTypeListOfT(ld.GetType()));
        }
        [TestMethod]
        public void IsDictionaryOfKV() {

            var o = 1;
            Assert.IsFalse(ReflectionHelper.IsDictionaryOfKV(o.GetType()));

            var li = new List<int>();
            Assert.IsFalse(ReflectionHelper.IsDictionaryOfKV(li.GetType()));

            var dsi = new Dictionary<string, int>();
            Assert.IsTrue(ReflectionHelper.IsDictionaryOfKV(dsi.GetType()));            
        }        
        [TestMethod]
        public void GetDictionary() {

            var dic = DynamicSugar.ReflectionHelper.GetDictionary(TestDataInstanceManager.TestPersonInstance);
            Assert.AreEqual("TORRES"         , dic["LastName"]);
            Assert.AreEqual("Frederic"       , dic["FirstName"]);
            Assert.AreEqual(45               , dic["Age"]);
            Assert.AreEqual(new DateTime(1964, 12, 11) , dic["BirthDay"]);

            dic = TestDataInstanceManager.TestPersonInstance.Dictionary();
            Assert.AreEqual("TORRES"         , dic["LastName"]);
            Assert.AreEqual("Frederic"       , dic["FirstName"]);
            Assert.AreEqual(45               , dic["Age"]);
            Assert.AreEqual(new DateTime(1964, 12, 11) , dic["BirthDay"]);
        }
        [TestMethod]
        public void GetDictionary_GetSubList() {

            var dic = DynamicSugar.ReflectionHelper.GetDictionary(TestDataInstanceManager.TestPersonInstance, DS.List("LastName","Age"));
            Assert.AreEqual("TORRES"         , dic["LastName"]);
            Assert.AreEqual(45               , dic["Age"]);
            Assert.IsFalse(dic.ContainsKey("FirstName"));
            Assert.IsFalse(dic.ContainsKey("BirthDay"));

            dic = TestDataInstanceManager.TestPersonInstance.Dictionary(DS.List("LastName","Age"));
            Assert.AreEqual("TORRES"         , dic["LastName"]);
            Assert.AreEqual(45               , dic["Age"]);
            Assert.IsFalse(dic.ContainsKey("FirstName"));
            Assert.IsFalse(dic.ContainsKey("BirthDay"));
        }
        [TestMethod]
        public void GetProperties_WithExpandoObject() {
    
            dynamic ex   = new ExpandoObject();
            ex.LastName  = "TORRES";
            ex.FirstName = "Frederic";
            ex.Age       = 45;
            ex.BirthDay  = new DateTime(1964, 12, 11);

            var dic = DynamicSugar.ReflectionHelper.GetDictionary(ex);
            Assert.AreEqual("TORRES"         , dic["LastName"]);
            Assert.AreEqual("Frederic"       , dic["FirstName"]);
            Assert.AreEqual(45               , dic["Age"]);
            Assert.AreEqual(new DateTime(1964, 12, 11) , dic["BirthDay"]);
        }
        [TestMethod]
        public void PropertyExist() {

            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "LastName"));
            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "FirstName"));
            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "Age"));
            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "BirthDay"));

            Assert.IsFalse(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "lastname"));
            Assert.IsFalse(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "NotAvailableProperty"));
        }
        [TestMethod]
        public void GetProperty() {

            Assert.AreEqual(TestDataInstanceManager.LASTNAME,   DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "LastName").ToString());
            Assert.AreEqual(TestDataInstanceManager.FIRSTNAME,  DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "FirstName").ToString());
            Assert.AreEqual(TestDataInstanceManager.AGE,        (int)DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "Age"));
            Assert.AreEqual(TestDataInstanceManager.BIRTH_DAY, (DateTime)DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "BirthDay"));

            Assert.AreEqual(null, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "lastname"));
            Assert.AreEqual(null, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "NotAvailableProperty"));
        }
        [TestMethod]
        public void GetPropertyForObject() {

            var streetValue  = "41 Lakewood road";
            var p            = TestDataInstanceManager.TestPersonInstance;
            p.Address.Street = streetValue;
            dynamic address  = DynamicSugar.ReflectionHelper.GetProperty(p, "Address");
            Assert.AreEqual(streetValue, address.Street);
        }
    }
}
