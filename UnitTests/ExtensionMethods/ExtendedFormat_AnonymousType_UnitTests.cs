using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;


namespace DynamicSugarSharp_UnitTests
{
    [TestClass]
    public class ExtendedFormat_AnonymousType_UnitTests
    {
        [TestMethod]
        public void OneProperty()
        {
            string format   = "LastName:{LastName}";
            string expected = "LastName:TORRES";
            Assert.AreEqual(expected, ExtendedFormat.Format(new { LastName = "TORRES" }, format));
        }
        [TestMethod]
        public void Format_DictionaryOfAnonymousTypeWithPropertyAsIntList()
        {
            var dic = new Dictionary<int, object>() { 
                { 100, new { LastName = "TORRES", Values = DS.List(1,2,3) } },
                { 101, new { LastName = "ALBERT", Values = DS.List(1,2,3) } },
                { 102, new { LastName = "LEROY" , Values = DS.List(1,2,3) } },
            };

            StringBuilder b = new StringBuilder(1024);
            string format = "LastName:{LastName}, Values:{Values}";

            var expected = @"LastName:TORRES, Values:[1, 2, 3]
LastName:ALBERT, Values:[1, 2, 3]
LastName:LEROY, Values:[1, 2, 3]
";
            foreach(var v in dic.Values){
                b.Append(ExtendedFormat.Format(v, format)).AppendLine();
            }
            var s = b.ToString();
            Assert.AreEqual(expected, b.ToString());
        }

        [TestMethod]
        public void Format_DictionaryOfAnonymousTypeWithPropertyAsStringList(){

            var dic = new Dictionary<int, object>() { 
                { 100, new { LastName = "TORRES", Values = DS.List("1","2","3") } },                
            };

            StringBuilder b = new StringBuilder(1024);
            string format = "LastName:{LastName}, Values:{Values}";

            var expected = @"LastName:TORRES, Values:[""1"", ""2"", ""3""]
";
            foreach (var v in dic.Values) {
                b.Append(ExtendedFormat.Format(v, format)).AppendLine();
            }
            var s = b.ToString();
            Assert.AreEqual(expected, b.ToString());
        }
    }
}
