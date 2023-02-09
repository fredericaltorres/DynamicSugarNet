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

            /// <summary>
            /// Assert that 2 List Of T are equal
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            public static void AreEqual<T>(List<T> l1, List<T> l2) {

                if (!DS.ListHelper.Identical(l1, l2)) {

                    throw new AssertFailedException(String.Format("List are not equal L1:'{0}', L2:'{1}'", DS.ListHelper.Format(l1), DS.ListHelper.Format(l2)));
                }
            }

            public static void AreEqualProperties(object poco, Dictionary<string, object> propertyNameValues) {

                if (poco is Dictionary<string, object>)
                {
                    var dic = poco as Dictionary<string, object>;
                    foreach (var k in dic)
                    {
                        var actualValue = k.Value;
                        if (actualValue == null && propertyNameValues[k.Key] == null)
                        {
                            // null == null
                        }
                        else if (!actualValue.Equals(propertyNameValues[k.Key]))
                            throw new AssertFailedException("AssertValueTypeProperties failed Property:{0}, Actual:{1}, Expected:{2}".FormatString(k.Key, actualValue, propertyNameValues[k.Key]));
                    }
                }
                else
                {
                    foreach (var k in propertyNameValues)
                    {
                        var actualValue = ReflectionHelper.GetProperty(poco, k.Key);
                        if (actualValue == null && propertyNameValues[k.Key] == null)
                        {
                            // null == null
                        }
                        else if (!actualValue.Equals(propertyNameValues[k.Key]))
                            throw new AssertFailedException("AssertValueTypeProperties failed Property:{0}, Actual:{1}, Expected:{2}".FormatString(k.Key, actualValue, propertyNameValues[k.Key]));
                    }
                }
            }

            public static void AreEqualProperties(object poco, object propertyNameValues) {

                if(propertyNameValues is Dictionary<string, object>) {
                    AreEqualProperties(poco,  propertyNameValues as Dictionary<string, object>);
                }
                else {
                    AreEqualProperties(poco, DS.Dictionary(propertyNameValues));
                }
            }
        }
    }
}
