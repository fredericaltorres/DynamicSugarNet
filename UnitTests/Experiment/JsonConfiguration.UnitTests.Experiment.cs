using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugarSharp;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DynamicSugarSharp_UnitTests {
    
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.trygetindex.aspx
    /// </summary>
    public class JSonConfiguration : DynamicObject {

        JObject _jSonObject = null;

        public JSonConfiguration(string json){

            _jSonObject = JObject.Parse(json); 
        }
        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result){
            
            return __TryGetMember(indexes[0].ToString(), out result);
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            
            return __TryGetMember(binder.Name, out result);
        }
        /// <summary>
        /// Return true if s is an Iso DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsDate(string s) {

            // "2000-12-15T22:11:03.055Z"
            var rx = @"^""\d{2,4}-\d{1,2}-\d{1,2}T\d{1,2}:\d{1,2}:\d{1,2}.*Z""$";
            return System.Text.RegularExpressions.Regex.IsMatch(s,rx);            
        }
        private bool __TryGetMember(string Name, out object result) {

            result = _jSonObject[Name];

            if(result!=null){

                if(IsDate(result.ToString())){ // Date

                    result = JsonConvert.DeserializeObject<DateTime>(result.ToString(), new IsoDateTimeConverter());
                }
            }
            return true;
        }
    }
    [TestClass]
    public class JSonConfiguration_UnitTests {

        const string JSon_TestString = @" {
            'Name'    : 'Apple',
            'Expiry'  : '2000-01-02T00:00:00.000Z',
            'Price'   : 3.99,
            'Quantity': 123,
            'Sizes'   : ['Small','Medium','Large']
        }";

        [TestMethod]
        public void ParseJSon_PropertySyntax() {

            dynamic config  = new JSonConfiguration(JSon_TestString);
            
            DateTime expiry = config.Expiry;
            string name     = config.Name;
            double price    = config.Price;            
            int quantity    = config.Quantity;
            int? quantity2  = config.Quantity2;
            string size0    = config.Sizes[0];           

            Assert.AreEqual(new DateTime(2000,01,02), expiry);
            Assert.AreEqual("Apple"          , name);
            Assert.AreEqual(3.99             , price);
            Assert.AreEqual(123              , quantity);
            Assert.AreEqual(null             , quantity2);
            Assert.AreEqual(size0            , "Small");
        }  
        [TestMethod]
        public void ParseJSon_BraketSyntax() {

            dynamic config  = new JSonConfiguration(JSon_TestString);
            
            DateTime expiry = config["Expiry"];
            string name     = config["Name"];
            double price    = config["Price"];
            int quantity    = config["Quantity"];
            int? quantity2  = config["Quantity2"];
            string size0    = config["Sizes"][0];           

            Assert.AreEqual(new DateTime(2000,01,02), expiry);
            Assert.AreEqual("Apple"          , name);
            Assert.AreEqual(3.99             , price);
            Assert.AreEqual(123              , quantity);
            Assert.AreEqual(null             , quantity2);
            Assert.AreEqual(size0            , "Small");
        }  

    }
}

