using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]

    public class FormatValueBasedOnType_UnitTests {

        const string A_STRING = "OK";

        private object o      (object o)      { return o; }
        private object _byte  (byte o)        { return o; }
        private object _sbyte (sbyte o)       { return o; }
        private object _short (short o)       { return o; }
        private object _single(Single o)      { return o; }
        private object _int   (int o)         { return o; }
        private object _long  (long o)        { return o; }
        private object _ulong (ulong o)       { return o; }
        private object _UInt64(UInt64 o)      { return o; }
        private object _Int64 (UInt64 o)      { return o; }

        [TestMethod]
        public void String() {

            Assert.AreEqual(A_STRING, Generated.FormatValueBasedOnType(A_STRING, null));
        }
        [TestMethod]
        public void Null() {

            Assert.AreEqual(null, Generated.FormatValueBasedOnType(null, null));
        }
        [TestMethod]
        public void TestMultipleType() {
                        
            var DateTestCases = DS.List( 
                Tuple.Create(A_STRING,             o(A_STRING),                   ""),
                Tuple.Create("12/11/1964",         o(new DateTime(1964, 12, 11)), "MM/dd/yyyy"),
                Tuple.Create("12 12 Dec December", o(new DateTime(1964, 12, 11)), "M MM MMM MMMM"),
                Tuple.Create("001.10",             o(1.1f),                       "000.00"),
                Tuple.Create("001.10",             o(1.1M),                       "000.00"),
                Tuple.Create("001.100",            o(1.1),                        "000.000"),
                Tuple.Create("001",               _byte(1),                       "000"),
                Tuple.Create("001",               _sbyte(1),                      "000"),                
                Tuple.Create("001",               _short(1),                      "000"),
                Tuple.Create("001",               _single(1),                     "000"),
                Tuple.Create("001",               _int(1),                        "000"),
                Tuple.Create("001",               _long(1),                       "000"),
                Tuple.Create("001",               _ulong(1),                      "000"),
                Tuple.Create("001",               _UInt64(1),                     "000"),
                Tuple.Create("001",               _Int64(1),                      "000")
            );
            
            foreach(var t in DateTestCases){

                Assert.AreEqual(t.Item1, Generated.FormatValueBasedOnType(t.Item2, t.Item3));
            }
        }
    }
}
