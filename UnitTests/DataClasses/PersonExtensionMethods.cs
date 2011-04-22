using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugarSharp_UnitTests {
    /// <summary>
    /// 
    /// </summary>
    public static class PersonExtensionMethods {

        public static string Format(this Person person, string format, params object[] args) {

            return DynamicSugar.ExtendedFormat.Format(person, format, args);
        }
        public static Dictionary<string,object> Dictionary(this Person person, List<string> propertiesToInclude = null) {

            return DynamicSugar.ReflectionHelper.GetDictionary(person, propertiesToInclude);
        }
        /*
        public static string RazorTemplate(this Person person, string razorTemplate) {

            return DynamicSugarSharp.RazorHelper.RazorTemplateForInstance(person, razorTemplate);
        }
        */
    }
}
