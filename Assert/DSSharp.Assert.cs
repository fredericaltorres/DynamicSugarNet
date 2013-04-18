using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
#if !MONOTOUCH
using System.Dynamic;
#endif
using System.Reflection;

namespace DynamicSugar {
    /// <summary>
    /// Dynamic Sharp Exception
    /// </summary>
    public class AssertFailedException : System.Exception {

        public AssertFailedException(string message) : base(message) { }
    }
    /// <summary>
    /// Dynamic Sharp Helper Class
    /// </summary>
    public static partial class DS {

        public static class Assert {

            public static void ValueTypeProperties(object poco, Dictionary<string, object> propertyNameValues) {

                foreach(var k in propertyNameValues) {
                    var actualValue = ReflectionHelper.GetProperty(poco, k.Key);
                    if(!actualValue.Equals(propertyNameValues[k.Key]))
                        throw new AssertFailedException("AssertValueTypeProperties failed Property:{0}, Actual:{1}, Expected:{2}".format(k.Key, actualValue, propertyNameValues[k.Key]));
                }
            }

            public static void ValueTypeProperties(object poco, object propertyNameValues) {

                if(propertyNameValues is Dictionary<string, object>) {
                    ValueTypeProperties(poco,  propertyNameValues as Dictionary<string, object>);
                }
                else {
                    ValueTypeProperties(poco, DS.Dictionary(propertyNameValues));
                }
            }
        }
    }
}
