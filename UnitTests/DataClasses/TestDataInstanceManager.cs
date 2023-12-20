using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugarSharp_UnitTests {

    /// <summary>
    /// Initialize and return the test instances
    /// </summary>
    public class TestDataInstanceManager {

        public const string LASTNAME      = "TORRES";
        public const string FIRSTNAME     = "Frederic";
        public const int AGE              = 45;
        public static DateTime BIRTH_DAY  = DateTime.Parse("1964/12/11");

        public const string PublicConstTitle = "PublicConstTitle";
        private const string PrivateConstTitle = "PrivateConstTitle";
        private static string PrivateStaticTitle = "PrivateStaticTitle";
        public static string PublicStaticTitle = "PublicStaticTitle";


        public static TestClassAllType TestClassAllTypeInstance => new TestClassAllType();

        public static Person TestPersonInstance {
            get {
                var p = new Person() { LastName = LASTNAME, FirstName = FIRSTNAME, Age = AGE, BirthDay = BIRTH_DAY};
                p.DrivingLicenses.Add("Car");
                p.DrivingLicenses.Add("Moto Bike");
                return p;
            }
        }
        public static ClassWithAllNumericTypes ClassWithAllTypesInstance {
            get {
                return new ClassWithAllNumericTypes();
            }
        }        
    }
}
