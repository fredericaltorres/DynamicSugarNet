using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugar.ConsoleApplication {
    /// <summary>
    /// 
    /// </summary>
    public static class DynaCSConsoleExtendedMethods {

        public static string Format(this Person person, string format, params object[] args) {

            return DynamicSugar.ExtendedFormat.Format(person, format, args);
        }
        public static IDictionary<string,object> Dict(this Person person) {

            return DynamicSugar.ReflectionHelper.GetDictionary(person);
        }
    }
}
