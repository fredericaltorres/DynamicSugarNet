using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugarSharp_UnitTests {

    public class TypeTestClass
    {
        public string LastName;
        public string FirstName { get; set; }
        public int Age { get; set; }
        public DateTime BirthDay { get; set; }

        private string PrivateTitle { get; set; } = "privateSomething";

        public List<string> ListOfString1 = new List<string>();
        public List<string> ListOfString2 { get; set; } = new List<string>();

        public Dictionary<string, string> DictionaryOfString1 = new Dictionary<string, string>();
        public Dictionary<string, string> DictionaryOfString2 { get; set; } = new Dictionary<string, string>();

        public Stack<string> StackOfString2 { get; set; } = new Stack<string>();

    }

    public class Address {
        public string Street;
        public string ZipCode;
        public string State;
    }
    /// <summary>
    /// A test class
    /// </summary>
    /// 
    public class Person {

        public string LastName;
        public string FirstName  { get; set; }
        public int Age           { get; set; }
        public DateTime BirthDay { get; set; }


        public static string PublicStaticTitle = "PublicStaticTitle";
        private static string PrivateStaticTitle = "PrivateStaticTitle";
        private string PrivateTitle { get; set; } = "privateSomething";

        public Address Address = new Address();

        public List<string> DrivingLicenses = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            return this.Format("LastName:{LastName}, FirstName:{FirstName}, Age:{Age}, BirthDay:{BirthDay}, DrivingLicenses:{DrivingLicenses}");
        }
        /// <summary>
        /// Implemented to test calling method with no parameter
        /// </summary>
        /// <returns></returns>
        public string GetLastName() {

            return this.LastName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetUniqueID(){

            return this.Format("#{LastName}.{FirstName}.{BirthDay:yyyy}");
        }
        /// <summary>
        /// Define how to determine if the person are the same.
        /// Implementing this method is required to use the extension 
        /// method In().
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {

            Person p = obj as Person;
            return (this.LastName==p.LastName) && (this.FirstName==p.FirstName) && (this.BirthDay==p.BirthDay);
        }
    }
}
