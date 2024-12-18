using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Net.Cache;
using System.Text.RegularExpressions;

namespace DynamicSugarSharp_UnitTests
{
    [TestClass]
    public class ReflectionHelper_UnitTests
    {
        public class FredPropertyClass
        {
            public string FieldString { get; set; }
            public DateTime FieldDate { get; set; }
            public double FieldDouble { get; set; }
            public bool FieldBool { get; set; }

            public FredPropertyClass()
            {

                FieldString = "Hello World";
                FieldDate = DateTime.Parse("12/13/2013 11:15:35 PM");
                FieldDouble = 12.34;
                FieldBool = true;
            }

            public int this[int index]
            {
                get
                {
                    return index * 2;
                }
            }
        }

        public class IndexerTestClass
        {
            private int _index;
            public int this[int index]
            {
                get
                {
                    return this._index;
                }
                set
                {
                    this._index = index * value;
                }
            }
        }

        class ConsTest1
        {
            public string Name = "Toto";
            private string Secret = "Secret";
        }
        class ConsTest2
        {
            public string Name = "Toto";

            public ConsTest2()
            {
            }
            public ConsTest2(string name)
            {
                this.Name = name;
            }
        }

        [TestMethod]
        public void Indexer_Set()
        {
            var f = new IndexerTestClass();
            ReflectionHelper.SetIndexer(f, 2, 123);
            Assert.AreEqual(2 * 123, ReflectionHelper.GetIndexer(f, 1));
        }

        [TestMethod]
        public void Indexer_Get()
        {
            var f = new FredPropertyClass();
            Assert.AreEqual(2, ReflectionHelper.GetIndexer(f, 1));
            Assert.AreEqual(16, ReflectionHelper.GetIndexer(f, 8));
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Indexer_Get_NoIndexerDefined()
        {
            var f = new ConsTest2();
            ReflectionHelper.GetIndexer(f, 1);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Indexer_Get_FromNull()
        {
            ReflectionHelper.GetIndexer(null, 1);
        }

        [TestMethod]
        public void Constructor_MultipleConstructor_GenericMethod()
        {
            var consTestInstance = ReflectionHelper.Constructor<ConsTest2>(typeof(ConsTest2));
            Assert.AreEqual("Toto", consTestInstance.Name);

            var consTestInstance2 = ReflectionHelper.Constructor<ConsTest2>(typeof(ConsTest2), "Tata");
            Assert.AreEqual("Tata", consTestInstance2.Name);
        }

        [TestMethod]
        public void Constructor_MultipleConstructor()
        {
            var consTestInstance = ReflectionHelper.Constructor(typeof(ConsTest2)) as ConsTest2;
            Assert.AreEqual("Toto", consTestInstance.Name);

            var consTestInstance2 = ReflectionHelper.Constructor(typeof(ConsTest2), "Tata") as ConsTest2;
            Assert.AreEqual("Tata", consTestInstance2.Name);
        }

        [TestMethod, ExpectedException(typeof(MissingMethodException))]
        public void Constructor_ConstructorWithInvalidParameters()
        {
            var consTestInstance = ReflectionHelper.Constructor(typeof(ConsTest2), "Tata", true);
        }

        [TestMethod]
        public void Constructor_DefaultConstructor()
        {
            var consTestInstance = ReflectionHelper.Constructor(typeof(ConsTest1)) as ConsTest1;
            Assert.AreEqual("Toto", consTestInstance.Name);
        }

        private Dictionary<string, ParameterMetadata> MyMethod_ParameterMetadata(int i, double d, string s)
        {

            var dic = ReflectionHelper.GetLocalsEx(i, d, s);
            Assert.AreEqual(1, dic["i"].Value);
            Assert.AreEqual(2.0, dic["d"].Value);
            Assert.AreEqual("A", dic["s"].Value);
            return dic;
        }

        [TestMethod]
        public void GetLocalsEx()
        {
            var d = MyMethod_ParameterMetadata(1, 2.0, "A");
        }

        private Dictionary<string, object> MyMethod(int i, double d, string s)
        {
            var dic = ReflectionHelper.GetLocals(i, d, s);
            return dic;
        }


        [TestMethod]
        public void Dictionary_Format()
        {
            var dic = DS.Dictionary(new { i = 1, f = 1.1f, s = "string", b = true });
            Assert.AreEqual(@"{ i:1, f:1.1, s:""string"", b:True }", dic.Format());

            var a = dic.Format("{0} ~ {1}", ",", "<", ">"); // Custom formatting
            Assert.AreEqual(@"<i ~ 1,f ~ 1.1,s ~ ""string"",b ~ True>", dic.Format("{0} ~ {1}", ",", "<", ">"));
        }

        [TestMethod]
        public void GetLocals()
        {

            var dic = MyMethod(1, 2.0, "A");

            Assert.AreEqual(@"{ i:1, d:2, s:""A"" }", dic.Format());
            Assert.AreEqual(1, dic["i"]);
            Assert.AreEqual(2.0, dic["d"]);
            Assert.AreEqual("A", dic["s"]);
        }

        [TestMethod]
        public void CloneDictionary()
        {

            var d = new Dictionary<string, object>() {
                { "A", 1 },
                { "B", 2.0 },
                { "C", "3" }
            };
            var d2 = ReflectionHelper.CloneDictionary<string, object>(d);

            Assert.AreEqual(1, d2["A"]);
            Assert.AreEqual(2.0, d2["B"]);
            Assert.AreEqual("3", d2["C"]);
        }

        [TestMethod]
        public void DictionaryViaReflection()
        {

            var d = new Dictionary<string, int>() {
                { "A", 1 },
                { "B", 2 },
                { "C", 3 }
            };
        }
        [TestMethod]
        public void GetListType()
        {

            var li = new List<int>();
            Assert.AreEqual(typeof(int), ReflectionHelper.GetListType(li.GetType()));

            var ls = new List<string>();
            Assert.AreEqual(typeof(string), ReflectionHelper.GetListType(ls.GetType()));

            var aString = "";
            Assert.AreNotEqual(typeof(string), ReflectionHelper.GetListType(aString.GetType()));
        }
        [TestMethod]
        public void GetDictionaryType_StringInt()
        {

            var dsi = new Dictionary<string, int>();
            Type keyType, valueType;
            ReflectionHelper.GetDictionaryType(dsi.GetType(), out keyType, out valueType);
            Assert.AreEqual(typeof(string), keyType);
            Assert.AreEqual(typeof(int), valueType);
        }
        [TestMethod]
        public void GetDictionaryType_DoubleDateTime()
        {

            var dsi = new Dictionary<double, DateTime>();
            Type keyType, valueType;
            ReflectionHelper.GetDictionaryType(dsi.GetType(), out keyType, out valueType);
            Assert.AreEqual(typeof(double), keyType);
            Assert.AreEqual(typeof(DateTime), valueType);
        }
        [TestMethod]
        public void IsTypeListOfT()
        {

            var o = 1;
            Assert.IsFalse(ReflectionHelper.IsTypeListOfT(o.GetType()));

            var li = new List<int>();
            Assert.IsTrue(ReflectionHelper.IsTypeListOfT(li.GetType()));

            var ld = new List<double>();
            Assert.IsTrue(ReflectionHelper.IsTypeListOfT(ld.GetType()));
        }
        [TestMethod]
        public void IsDictionaryOfKV()
        {

            var o = 1;
            Assert.IsFalse(ReflectionHelper.IsDictionaryOfKV(o.GetType()));

            var li = new List<int>();
            Assert.IsFalse(ReflectionHelper.IsDictionaryOfKV(li.GetType()));

            var dsi = new Dictionary<string, int>();
            Assert.IsTrue(ReflectionHelper.IsDictionaryOfKV(dsi.GetType()));
        }

        [TestMethod]
        public void GetDictionary()
        {
            var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance);
            Assert.AreEqual("TORRES", dic["LastName"]);
            Assert.AreEqual("Frederic", dic["FirstName"]);
            Assert.AreEqual(45, dic["Age"]);
            Assert.AreEqual(new DateTime(1964, 12, 11), dic["BirthDay"]);

            dic = TestDataInstanceManager.TestPersonInstance.Dictionary();
            Assert.AreEqual("TORRES", dic["LastName"]);
            Assert.AreEqual("Frederic", dic["FirstName"]);
            Assert.AreEqual(45, dic["Age"]);
            Assert.AreEqual(new DateTime(1964, 12, 11), dic["BirthDay"]);
        }

        [TestMethod]
        public void GetDictionary_PrivateProperty()
        {
            var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance, allProperties: true);
            Assert.AreEqual("privateSomething", dic["PrivateTitle"]);

            dic = DynamicSugar.ReflectionHelper.GetDictionary(TestDataInstanceManager.TestPersonInstance, allProperties: true);
            Assert.AreEqual("privateSomething", dic["PrivateTitle"]);
        }

        [TestMethod]
        public void GetDictionary_GetSubList()
        {

            var dic = DynamicSugar.ReflectionHelper.GetDictionary(TestDataInstanceManager.TestPersonInstance, DS.List("LastName", "Age"));
            Assert.AreEqual("TORRES", dic["LastName"]);
            Assert.AreEqual(45, dic["Age"]);
            Assert.IsFalse(dic.ContainsKey("FirstName"));
            Assert.IsFalse(dic.ContainsKey("BirthDay"));

            dic = TestDataInstanceManager.TestPersonInstance.Dictionary(DS.List("LastName", "Age"));
            Assert.AreEqual("TORRES", dic["LastName"]);
            Assert.AreEqual(45, dic["Age"]);
            Assert.IsFalse(dic.ContainsKey("FirstName"));
            Assert.IsFalse(dic.ContainsKey("BirthDay"));
        }

        [TestMethod]
        public void GetProperties_WithExpandoObject()
        {
            dynamic expandoO = new ExpandoObject();
            expandoO.LastName = "TORRES";
            expandoO.FirstName = "Frederic";
            expandoO.Age = 45;
            expandoO.BirthDay = new DateTime(1964, 12, 11);

            var dic = DynamicSugar.ReflectionHelper.GetDictionary(expandoO);
            Assert.AreEqual("TORRES", dic["LastName"]);
            Assert.AreEqual("Frederic", dic["FirstName"]);
            Assert.AreEqual(45, dic["Age"]);
            Assert.AreEqual(new DateTime(1964, 12, 11), dic["BirthDay"]);
        }

        [TestMethod]
        public void PropertyExist()
        {
            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "LastName"));
            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "FirstName"));
            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "Age"));
            Assert.IsTrue(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "BirthDay"));

            Assert.IsFalse(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "lastname"));
            Assert.IsFalse(DynamicSugar.ReflectionHelper.PropertyExist(TestDataInstanceManager.TestPersonInstance, "NotAvailableProperty"));
        }

        [TestMethod]
        public void MethodExist()
        {
            Assert.IsTrue(DynamicSugar.ReflectionHelper.MethodExist(TestDataInstanceManager.TestPersonInstance, "GetLastName"));
            Assert.IsFalse(DynamicSugar.ReflectionHelper.MethodExist(TestDataInstanceManager.TestPersonInstance, "InvalidMethod"));
        }

        [TestMethod]
        public void GetPrivateProperty()
        {
            var privateTitle = DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "PrivateTitle", isPrivate: true);
            Assert.AreEqual("privateSomething", privateTitle);
        }


        [TestMethod]
        public void GetPublicStaticProperty()
        {
            // WE CAN CALL A PUBLIC STATIC PROPERTY USING AN INSTANCE 
            // BUT USING A TYPE DOES NOT WORK
            Assert.AreEqual("PublicStaticTitle", DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "PublicStaticTitle").ToString());
            // Assert.AreEqual("PublicStaticTitle", DynamicSugar.ReflectionHelper.GetProperty(typeof(Person), "PublicStaticTitle").ToString());
        }


        [TestMethod]
        public void GetPrivateStaticProperty()
        {
            Assert.AreEqual("PrivateStaticTitle", DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "PrivateStaticTitle", isPrivate: true).ToString());
        }

        [TestMethod]
        public void GetProperty()
        {

            Assert.AreEqual(TestDataInstanceManager.LASTNAME, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "LastName").ToString());
            Assert.AreEqual(TestDataInstanceManager.FIRSTNAME, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "FirstName").ToString());
            Assert.AreEqual(TestDataInstanceManager.AGE, (int)DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "Age"));
            Assert.AreEqual(TestDataInstanceManager.BIRTH_DAY, (DateTime)DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "BirthDay"));

            // Is case sensitive
            Assert.AreEqual(null, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "lastname"));

            Assert.AreEqual(null, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "NotAvailableProperty"));
        }


        [TestMethod]
        public void EvaluatePath()
        {

            //Assert.AreEqual(TestDataInstanceManager.LASTNAME, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "LastName").ToString());
            //Assert.AreEqual(TestDataInstanceManager.FIRSTNAME, DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "FirstName").ToString());
            //Assert.AreEqual(TestDataInstanceManager.AGE, (int)DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "Age"));
            //Assert.AreEqual(TestDataInstanceManager.BIRTH_DAY, (DateTime)DynamicSugar.ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "BirthDay"));
            var r = DynamicSugar.ReflectionHelper.EvaluatePath(TestDataInstanceManager.TestPersonInstance, "LastName");


            // Is case sensitive

        }

        [TestMethod]
        public void GetPropertyForObject()
        {

            var streetValue = "41 Lakewood road";
            var p = TestDataInstanceManager.TestPersonInstance;
            p.Address.Street = streetValue;
            dynamic address = DynamicSugar.ReflectionHelper.GetProperty(p, "Address");

            Assert.AreEqual(streetValue, address.Street);

            var privateTitle = DynamicSugar.ReflectionHelper.GetProperty(p, "PrivateTitle", isPrivate: true);
            Assert.AreEqual("privateSomething", privateTitle);
        }

        [TestMethod]
        public void GetDictionaryWithType__TryToGetTheNameAndTypeOfAllPropertyOfAnNotInitializedObject()
        {
            var testInstance = new TypeTestClass();
            var dicType = DynamicSugar.ReflectionHelper.GetDictionaryWithType(testInstance);
            Assert.AreEqual("String", dicType["FirstName"]);
            Assert.AreEqual("String", dicType["LastName"]);
            Assert.AreEqual("Int32", dicType["Age"]);
            Assert.AreEqual("DateTime", dicType["BirthDay"]);
            Assert.AreEqual("List<String>", dicType["ListOfString1"]);
            Assert.AreEqual("List<String>", dicType["ListOfString2"]);
            Assert.AreEqual("Dictionary<String, String>", dicType["DictionaryOfString1"]);
            Assert.AreEqual("Dictionary<String, String>", dicType["DictionaryOfString2"]);

            var dicValue = DynamicSugar.ReflectionHelper.GetDictionary(testInstance);
            Assert.AreEqual(null, dicValue["FirstName"]);
            Assert.AreEqual(null, dicValue["LastName"]);
            Assert.AreEqual(0, (int)dicValue["Age"]);
            Assert.AreEqual(new DateTime(), (DateTime)dicValue["BirthDay"]);
            Assert.AreEqual(0, ((List<string>)dicValue["ListOfString1"]).Count);
            Assert.AreEqual(0, ((List<string>)dicValue["ListOfString2"]).Count);
            Assert.AreEqual(0, ((Dictionary<String, String>)dicValue["DictionaryOfString1"]).Count);
            Assert.AreEqual(0, ((Dictionary<String, String>)dicValue["DictionaryOfString2"]).Count);
        }
        [TestMethod]
        public void AssertPoco()
        {
            var testInstance = TestDataInstanceManager.TestPersonInstance;
            Assert.IsTrue(ReflectionHelper.AssertPoco(testInstance, new { LastName = "TORRES", Age = 45, FirstName = new Regex("F.*c") }));
        }

        [TestMethod]
        public void AssertPoco_AllType()
        {
            var testInstance = TestDataInstanceManager.TestClassAllTypeInstance;
            Assert.IsTrue(ReflectionHelper.AssertPoco(testInstance, new {
                IntValue = 123,
                LongValue = 123l,
                DoubleValue = 123.456,
                DecimalValue = 123.456m,
                DateTimeValue = new DateTime(1964, 12, 11, 0, 0, 0),
                StringValue = "Hello",
                BoolValue = true,
                CharValue = 'A',
                ByteValue = (byte)123,
                ShortValue = (short)123,
                FloatValue = 123.456f,
                SingleValue = 123.456f,
                UIntValue = (uint)123,
                ULongValue = (ulong)123,
                UShortValue = (ushort)123,
                SByteValue = (sbyte)123,
                 GuidValue = Guid.Parse("A4E7E546-D75C-4B4C-B717-EC0D66085CA0"),
                TimeSpanValue = TimeSpan.FromDays(1),
                //DateTimeOffsetValue = new DateTimeOffset(1234567, new TimeSpan(0, 0, 1)),
                UriValue = new Uri("http://www.flogviewer.com")
        }));
        }

        internal class PatchClass
        {
            public string FirstName { get; set; }
            public int Age { get; set; }
            public double Height { get; set; }
            public bool IsAlive { get; set; }
            public DateTime BirthDate { get; set; }
        }

        [TestMethod]
        public void PatchObject()
        {
            var pocoInput = new PatchClass { FirstName = "Fred", Age = 16, Height = 4, BirthDate = new DateTime(1964, 12, 11) };
            var dic = new Dictionary<string, object>();
            dic.Add("FirstName", "Freddy");
            dic.Add("Age", 32);
            dic.Add("Height", 5.9);
            dic.Add("IsAlive", true);
            dic.Add("BirthDate", new DateTime(1964, 12, 12));
            var r = ReflectionHelper.PatchPoco(pocoInput, dic) as PatchClass;
            Assert.AreEqual("Freddy", r.FirstName);
            Assert.AreEqual(32, r.Age);
            Assert.AreEqual(5.9, r.Height);
            Assert.AreEqual(true, r.IsAlive);
            Assert.AreEqual(new DateTime(1964, 12, 12), r.BirthDate);
        }
    }
}
