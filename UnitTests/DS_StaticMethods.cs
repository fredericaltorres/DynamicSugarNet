using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Linq.Expressions;

namespace DynamicSugarSharp_UnitTests {

    //TODO:Try extension method to List<T>

    [TestClass]
    public class DS_StaticMethods_UnitTests {

        private string InitializeSettings(IDictionary<string, object> settings){

            return settings["UserName"].ToString();
        }
        [TestMethod]
        public void DS_Dictionary(){

            Assert.AreEqual("RRabbit",
                InitializeSettings( DS.Dictionary(
                        new {
                            UserName = "RRabbit" ,
                            Domain   = "ToonTown",
                            UserID   = 234873
                        }
                ))
            );                             
        }
        [TestMethod]
        public void DS_Values(){

            var bag = DS.Values( new { a=1, b=2, c=3 } );
            Assert.AreEqual(1,bag.a);
            Assert.AreEqual(2,bag.b);
            Assert.AreEqual(3,bag.c);
        }
        [TestMethod]
        public void DS_Values_PassInFunctionAsIDictionary(){
          
            // Dynamic Sugar syntaxes
            InitializeSettings( DS.Values( new {
                UserName= "RRabbit" ,
                Domain  = "ToonTown",
                UserID  = 234873
            }));
            Assert.AreEqual("RRabbit",
                InitializeSettings( DS.Values(new {
                        UserName = "RRabbit" ,
                        Domain   = "ToonTown",
                        UserID   = 234873
                    }
                ))
            );                          
        }
    }
}
