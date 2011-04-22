using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;

namespace ExtendedFormat_UnitTests {

    [TestClass]
    public class FormatWithDictionary_UnitTests {

        [TestMethod]
        public void OneName() {

            var format    = "LastName:{LastName}";
            var expected  = "LastName:TORRES";
            var Values    = new Dictionary<string, object>() { { "LastName", "TORRES" } };

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, Values));
        }
        [TestMethod]
        public void OneNameOneIndexValue() {

            var format    = "LastName:{LastName}, FirstName:{0}";
            var expected  = "LastName:TORRES, FirstName:Frederic";
            var Values    = new Dictionary<string, object>() { { "LastName", "TORRES" } };
            
            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, Values, "Frederic"));
        }
        [TestMethod]
        public void _3Names_2IndexedValues() {

            string format   = "LastName:{LastName}, FirstName:{FirstName}, Age:{Age}, BirthDate:{0}, BirthCountry:{1}";
            string expected = "LastName:TORRES, FirstName:Frederic, Age:45, BirthDate:11/12/1964 12:00:00 AM, BirthCountry:France";            
            var Values      = new Dictionary<string, object>() { 
                { "LastName" , "TORRES"  }, 
                { "FirstName","Frederic" },
                { "Age"      , 45        }
            };
            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, Values, DateTime.Parse("11/12/1964"), "France"));
        }
        [TestMethod]
        public void _3Names_WithCurlyBraketInTheFormat_Case1() {

            var format      = "{{LastName:{LastName}}}, {{FirstName:{FirstName}, Age:{Age}}}";
            var expected    = "{LastName:TORRES}, {FirstName:Frederic, Age:45}";
            var Values      = new Dictionary<string, object>() { 
                { "LastName" , "TORRES"  },
                { "FirstName","Frederic" },
                { "Age"      , 45        }
            };
            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, Values));
        }
        [TestMethod]
        public void _3Names_WithCurlyBraketInTheFormat_Case2() {

            var format      = "{{LastName:{LastName}}}, {{FirstName:{{{FirstName}}}, Age:{{{Age}}}}}";
            var expected    = "{LastName:TORRES}, {FirstName:{Frederic}, Age:{45}}";
            var Values      = new Dictionary<string, object>() { 
                { "LastName" , "TORRES"  },
                { "FirstName","Frederic" },
                { "Age"      , 45        }
            };
            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, Values));
        }
        [TestMethod]
        public void _1Name_WithCurlyBraketSpecialCase() {

            string format                     = "{{LastName:{LastName}}}";
            string expected                   = "{LastName:TORRES}";
            var Values      = new Dictionary<string, object>() { 
                { "LastName" , "TORRES" }
            };            
            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, Values));
        }
        [TestMethod, ExpectedException(typeof(DynamicSugar.ExtendedFormatException))]
        public void _1Names_WithCurlyBraketInTheFormat_InvalidCurlyBraketSyntax() {

            string format                     = "{{LastName:{LastName}}";            
            Dictionary<string, object> Values = new Dictionary<string, object>();            
            DynamicSugar.ExtendedFormat.Format(format, Values);
        }
        [TestMethod, ExpectedException(typeof(DynamicSugar.ExtendedFormatException))]
        public void NameInFormatNotDefinedInDictionary() {

            string format                     = "LastName:{LastName}";
            Dictionary<string, object> Values = new Dictionary<string, object>();            
            DynamicSugar.ExtendedFormat.Format(format, Values);
        }
        [TestMethod]
        public void ExpandoObject_Basic()
        {
            var format   = "Name:{Name}, Age:{Age}";
            var expected = "Name:Descartes, Age:45";
            dynamic bag  = new ExpandoObject();
            bag.Name     = "Descartes";
            bag.Age      = 45;

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, bag));
        }
        [TestMethod]
        public void ExpandoObject_AndFormat()
        {
            var format   = "Name:{Name}, Age:{Age:000}";
            var expected = "Name:Descartes, Age:045";
            dynamic bag  = new ExpandoObject();
            bag.Name     = "Descartes";
            bag.Age      = 45;

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(format, bag));
        }
    }
}
