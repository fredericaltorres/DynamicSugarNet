using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicSugarSharp_UnitTests{

    [TestClass]
    public class ExtendedFormat_UnitTests {

        [TestMethod]
        public void OneProperty() {

            string format   = "LastName:{LastName}";
            string expected = "LastName:TORRES";

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format));
        }
        [TestMethod, ExpectedException(typeof(System.MissingMethodException))]
        public void OneFunctionThatDoesNotExist() {

            string format   = "LastName:{GetThisNonExistingMethod()}";
            DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format);
        }
        [TestMethod]
        public void OneFunction() {

            string format   = "LastName:{GetLastName()}";
            string expected = "LastName:TORRES";

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format));
        }
        [TestMethod, ExpectedException(typeof(DynamicSugar.ExtendedFormatException))]
        public void OneNonExistingProperty() {

            string format   = "LastName:{Last_Name}";
            string expected = "LastName:TORRES";

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format));
        }
        [TestMethod]
        public void DefaultSyntaxOnValue() {

            string format   = "LastName:{0}";
            string expected = "LastName:TORRES";

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format, "TORRES"));
        }
        [TestMethod]
        public void DefaultSyntaxOnDateTimeValueWithFormat() {

            string format   = "BirthDay:{BirthDay:MM/dd/yyyy}";
            string expected = "BirthDay:12/11/1964";

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format));
        }
        [TestMethod]
        public void DefaultSyntaxOnIntValueWithFormat() {
            
            string format   = "Age:{Age:000}";
            string expected = "Age:045";

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format));
        }
        [TestMethod]
        public void DefaultSyntaxOnDCurrencyValueWithFormatting() {

            string format   = "CurrencyValue:{0:c}";
            string expected = "CurrencyValue:$1.23";
            Decimal currency = 1.234M;

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format, currency));
        }
        [TestMethod]
        public void MixingOnePropertyAndOneDefaultValue() {

            string format   = "LastName:{LastName}, FirstName:{0}";
            string expected = "LastName:TORRES, FirstName:Frederic";

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format, "Frederic"));
        }
        [TestMethod]
        public void MixingTwoPropertyAndOneDefaultValue() {

            string format    = "LastName:{LastName}, FirstName:{FirstName}, Amount:{0:c}";
            string expected  = "LastName:TORRES, FirstName:Frederic, Amount:$1.23";
            Decimal currency = 1.234M;

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format,  currency));
        }
        [TestMethod]
        public void MixingTwoPropertyAndOneDefaultValue_UsingExtensionMethod() {

            string format    = "LastName:{LastName}, FirstName:{FirstName}, Amount:{0:c}";
            string expected  = "LastName:TORRES, FirstName:Frederic, Amount:$1.23";
            Decimal currency = 1.234M;
            
            var sss = TestDataInstanceManager.TestPersonInstance.Format(format, currency);
            Assert.AreEqual(expected, sss);
        }
        [TestMethod]
        public void ClassPerson_ToString() {

            string expected = @"LastName:TORRES, FirstName:Frederic, Age:45, BirthDay:12/11/1964 12:00:00 AM, DrivingLicenses:[""Car"", ""Moto Bike""]";            
            var s = TestDataInstanceManager.TestPersonInstance.ToString();
            Assert.AreEqual(expected, s);
        }
        [TestMethod, ExpectedException(typeof(System.FormatException))]
        public void MissingPropertyNameInBracket() {

            string format    = "LastName:{LastName}, FirstName:{}, Amount:{0:c}";            
            Decimal currency = 1.234M;

            TestDataInstanceManager.TestPersonInstance.Format(format, currency);        
        }
        [TestMethod]
        public void IsDotNetID() {

            Assert.IsTrue(DynamicSugar.ExtendedFormat.IsDotNetID("a"));
            Assert.IsTrue(DynamicSugar.ExtendedFormat.IsDotNetID("_____"));
            Assert.IsTrue(DynamicSugar.ExtendedFormat.IsDotNetID("LastName"));
            Assert.IsTrue(DynamicSugar.ExtendedFormat.IsDotNetID("Last_Name"));
            Assert.IsTrue(DynamicSugar.ExtendedFormat.IsDotNetID("Last_Name32472389"));
            Assert.IsTrue(DynamicSugar.ExtendedFormat.IsDotNetID("__Last_Name32472389"));
            Assert.IsTrue(DynamicSugar.ExtendedFormat.IsDotNetID("__Last_Name3247__"));

            Assert.IsFalse(DynamicSugar.ExtendedFormat.IsDotNetID("123"));
            Assert.IsFalse(DynamicSugar.ExtendedFormat.IsDotNetID("123Last_Name3247__"));            
            Assert.IsFalse(DynamicSugar.ExtendedFormat.IsDotNetID("LastName!"));
            Assert.IsFalse(DynamicSugar.ExtendedFormat.IsDotNetID("LastName@"));
            Assert.IsFalse(DynamicSugar.ExtendedFormat.IsDotNetID("LastName~"));
            Assert.IsFalse(DynamicSugar.ExtendedFormat.IsDotNetID("LastName~"));
        }
        [TestMethod]
        public void SupportOfCurlyBraketAsLiteral() {

            string format    = "{{LastName}}:{LastName}, {{FirstName}}:{FirstName}, {{Amount}}:{0:c}";
            string expected  = "{LastName}:TORRES, {FirstName}:Frederic, {Amount}:$1.23";
            Decimal currency = 1.234M;

            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format, currency));
        }
        [TestMethod]
        public void StringFormat_SupportOfCurlyBraketAsLiteral() {

            string format = "{{{0}}}";
            var s         = DynamicSugar.ExtendedFormat.Format(null, format,"yes");
            Assert.AreEqual("{yes}", s);
        }
        [TestMethod]
        public void FormatAllIntegerTypes() {

            string format   = "_byte:{_byte:000},_Byte:{_Byte:000},_sbyte:{_sbyte:000},_SByte:{_SByte:000},_int:{_int:000},_Int32:{_Int32:000},_uint:{_uint:000},_UInt32:{_UInt32:000},_long:{_long:000},_Int64:{_Int64:000},_ulong:{_ulong:000},_UInt64:{_UInt64:000},_short:{_short:000},_Int16:{_Int16:000},_ushort:{_ushort:000},_UInt16:{_UInt16:000}";
            string expected = "_byte:064,_Byte:064,_sbyte:064,_SByte:064,_int:064,_Int32:064,_uint:064,_UInt32:064,_long:064,_Int64:064,_ulong:064,_UInt64:064,_short:064,_Int16:064,_ushort:064,_UInt16:064";
            string result   = DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.ClassWithAllTypesInstance, format);

            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void FormatAllDecimalTypes() {

            string format   = "_decimal:{_decimal:00.00},_Decimal:{_Decimal:00.00},_double:{_double:00.00},_Double:{_double:00.00},_float:{_float:00.00},_single:{_float:00.00}";
            string expected = "_decimal:01.20,_Decimal:01.20,_double:01.20,_Double:01.20,_float:01.20,_single:01.20";
            string result   = DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.ClassWithAllTypesInstance, format);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void FormatListOfTAsProperty() {
                            
            string format    = "LastName:{LastName}, DrivingLicenses:{DrivingLicenses} ";
            string expected  = "LastName:TORRES, DrivingLicenses:[\"Car\", \"Moto Bike\"] ";            
            Assert.AreEqual(expected, DynamicSugar.ExtendedFormat.Format(TestDataInstanceManager.TestPersonInstance, format));
        }
    }
}
