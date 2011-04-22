using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugarSharp;
using System.Dynamic;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class DynaExperiment_UnitTests {

        [TestMethod]
        public void DynaExperiment_1() {

            //dynamic d = DynaExperiment.Create();
            //var d = DynaExperiment.New[1,2,3];
            //DynaExperiment.New[1,2,3] = 1;
            //DynaExperiment.New.DoThis[1,2,3].DoThat[4,5].Bla[1] = 1;
            var d = DynaExperiment.New.LastName["Torres"].FirstName["Frederic"].Age[45];
            var dic = d.Dictionary;
        }
        [TestMethod]
        public void DynaExperiment_AddOperator() {

            var d1 = DynaExperiment.New.LastName["Torres"].FirstName["Frederic"];
            var d2 = DynaExperiment.New.Age[45];
            var d0 = DynaExperiment.New.CitizenShip["USA"];
            var d3 = d0 + d1 + d2;

            Assert.AreEqual("Torres"  , d3.Dictionary["LastName"]);
            Assert.AreEqual("Frederic", d3.Dictionary["FirstName"]);
            Assert.AreEqual(45        , d3.Dictionary["Age"]);
        }
        private void AFunction(IDictionary<string, object> dic){

            Assert.AreEqual("Torres"  , dic["LastName"]);
            Assert.AreEqual("Frederic", dic["FirstName"]);
            Assert.AreEqual(45        , dic["Age"]);
        }
        [TestMethod]
        public void DynamicDictionary_1() {
            
            var d1 = DynamicDictionary.Dictionary[LastName:"Torres", FirstName:"Frederic", Age:45];
            
            AFunction(d1);
            Assert.AreEqual("Torres"  , d1.AsDictionary["LastName"]);
            Assert.AreEqual("Frederic", d1.AsDictionary["FirstName"]);
            Assert.AreEqual(45        , d1.AsDictionary["Age"]);
        }
        [TestMethod]
        public void DynamicDictionary_2() {

            var d1 = DS.Dictionary2.Init(LastName:"Torres", FirstName:"Frederic", Age:45);
            
            AFunction(d1);
            
            var d2 = d1.AsDictionary;
            Assert.AreEqual("Torres"  , d2["LastName"]);
            Assert.AreEqual("Frederic", d2["FirstName"]);
            Assert.AreEqual(45        , d2["Age"]);
        }
        [TestMethod]
        public void DynamicDictionary_3() {

            AFunction(DS.Dictionary( new { LastName="Torres", FirstName="Frederic", Age=45 } ));

            var d1 = DS.Dictionary( new { LastName="Torres", FirstName="Frederic", Age=45 } );            
            
            Assert.AreEqual("Torres"  , d1["LastName"]);
            Assert.AreEqual("Frederic", d1["FirstName"]);
            Assert.AreEqual(45        , d1["Age"]);
        }
        [TestMethod]
        public void DynamicDictionary_4() {

            var d1 = DS.Dictionary2 [ LastName:"Torres", FirstName:"Frederic", Age:45 ];
            AFunction(d1);
            Assert.AreEqual("Torres"  , d1["LastName"]);
            Assert.AreEqual("Frederic", d1["FirstName"]);
            Assert.AreEqual(45        , d1["Age"]);
        }
    }
}
