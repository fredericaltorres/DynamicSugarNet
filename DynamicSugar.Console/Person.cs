using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugar.ConsoleApplication {

    /// <summary>
    /// A test class
    /// </summary>
    public class Person {

        public string LastName;
        public string FirstName     { get; set; }
        public DateTime BirthDay    { get; set; }

        public List<string> DrivingLicenses = new List<string>();
        
        public int Age { 
            get{
                return DateTime.Now.Year - this.BirthDay.Year;
            }
        }
        
        public override string ToString() {

            return this.Format("LastName:{LastName}, FirstName:{FirstName}, Age:{Age}, BirthDay:{BirthDay}");
        }
        /// <summary>
        /// Implemented to test calling method with no parameter
        /// </summary>
        /// <returns></returns>
        public string LoadInformation() { 

            return "Information";            
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
