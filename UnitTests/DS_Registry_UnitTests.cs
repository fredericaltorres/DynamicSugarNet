//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using DynamicSugar;
//using System.Dynamic;

//namespace DynamicSugarSharp_UnitTests {

//    [TestClass, Ignore]
//    public class DS_Registry_UnitTests {

//        [TestMethod]
//        public void Read() {
           
//            var d1 = DS.Register.LocalMachine.SOFTWARE.Microsoft.COM3.BuildNumber;
//            var d2 = DS.Register.LocalMachine["SOFTWARE"]["Microsoft"]["COM3"]["BuildNumber"];
//            var d3 = DS.Register.LocalMachine[@"SOFTWARE\Microsoft\COM3"].BuildNumber;
//            var d4 = DS.Register.LocalMachine.SOFTWARE.Microsoft.COM3.GetValue("BuildNumber");

//            Assert.AreEqual(d1, d2);
//            Assert.AreEqual(d1, d3);
//            Assert.AreEqual(d1, d4);
//        }
//        [TestMethod]
//        public void Write() {

//            var registryKey = DS.Register.LocalMachine.SOFTWARE.Microsoft.COM3;
//            registryKey.SetValue("nameDW", 123);
//            registryKey.SetValue("nameString", "Fred");
//            registryKey.SetValue("nameArray", DS.List<byte>(1, 2, 3).ToArray());

//            Assert.AreEqual(123, registryKey.nameDW);
//            Assert.AreEqual("Fred", registryKey.nameString);
//            DS.Assert.AreEqual(DS.List<byte>(1, 2, 3), DS.SystemArrayToList<byte>(registryKey.nameArray));

//            registryKey.DeleteValue("nameDW");
//            registryKey.DeleteValue("nameString");
//            registryKey.DeleteValue("nameArray");
//        }
//    }
//}

