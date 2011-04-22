using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;

namespace DynamicSugarSharp_UnitTests {
    
    [TestClass]
    public class DynamicBag_UnitTests {

        [TestMethod]
        public void BasicProperties() {

            dynamic d    = new DynamicBag();
            d.LastName  = "TORRES";
            d.Age       = 46;

            Assert.AreEqual("TORRES", d.LastName);
            Assert.AreEqual(46, d.Age);
        }      
        [TestMethod]
        public void AsIDictionary() {

            dynamic d   = new DynamicBag();
            d.LastName  = "TORRES";
            d.Age       = 46;

            IDictionary<string, object> dic = d as IDictionary<string, object>;

            Assert.AreEqual("TORRES", dic["LastName"]);
            Assert.AreEqual(46      , dic["Age"]);
        }   
        [TestMethod]
        public void AssignVoid() {

            dynamic d = new DynamicBag();
            string  s = null;
            
            Action Speak = () => {
                Console.WriteLine("Hello World");
                s = "OK";
            };

            d.Speak = Speak;
            d.Speak();
            Assert.AreEqual("OK",s);
        }
        [TestMethod]
        public void AssignFunction() {

            dynamic This       = new DynamicBag();
            This.InternalValue = 100;
            
            Func<int> Compute = () => {
                return  This.InternalValue;
            };
            This.Compute = Compute;
            Assert.AreEqual(100, This.Compute());
        }
        [TestMethod]
        public void Inherite() {

            dynamic oSuper          = new DynamicBag();
            oSuper.InternalValue    = 123;
            oSuper.Hi               = "Hi";
            dynamic o               = new DynamicBag(oSuper);

            Assert.AreEqual(123, o.InternalValue);
            Assert.AreEqual("Hi", o.Hi);
        }
        [TestMethod]
        public void Prototype() {

            dynamic oRef          = new DynamicBag();
            oRef.InternalValue    = 123;
            oRef.Hi               = "Hi";

            dynamic o               = DynamicBag.Prototype(oRef);

            Assert.AreEqual(123, o.InternalValue);
            Assert.AreEqual("Hi", o.Hi);
        }
        [TestMethod]
        public void AssignVoid2() {

            dynamic d = new DynamicBag();
            string  s = null;
            
            Action Run = () => {
                Console.WriteLine("Run");  
            };
            d.Run = Run;

            Action Speak = () => {
                Console.WriteLine("Hello World");
                s = "OK";
                d.Run();
            };
            d.Speak = Speak;
            d.Speak();
            Assert.AreEqual("OK",s);
        }
    }
}

