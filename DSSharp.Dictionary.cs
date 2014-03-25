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
    /// Dynamic Sharp Helper Class, dedicated methods to work with dictionary
    /// </summary>
    public static partial class DS {

        public static class DictionaryHelper {
        
            /// <summary>
            /// Format the content of a dictionary to a string
            /// </summary>
            /// <typeparam name="K">The type of the key</typeparam>
            /// <typeparam name="V">The type of the value</typeparam>
            /// <param name="dictionary">The dictionary to FormatString</param>
            /// <param name="format">The FormatString of each key and value. The default is "{0}:{1}"</param>
            /// <param name="separator">The separator between each key and value. The default is ", "</param>
            /// <param name="preFix">A string to output at the beginning of the string, the default is "{ "</param>
            /// <param name="postFix">A string to output at the end of the string, the default is " }"</param>
            /// <returns></returns>
            public static string Format<K,V>(IDictionary<K,V> dictionary, string format="{0}:{1}", string separator = ", ", string preFix = "{ ", string postFix = " }") {

                if (dictionary == null) { return "Null"; } // just in case
            
                System.Text.StringBuilder b = new StringBuilder(1024);
                b.Append(preFix);

                foreach(KeyValuePair<K,V> kv in dictionary){
                    
                    b.AppendFormat(format, ExtendedFormat.FormatValue(kv.Key, false), ExtendedFormat.FormatValue(kv.Value, true));
                    b.Append(separator);
                }
                b.Remove(b.Length-separator.Length,separator.Length); // Remove the separator
                b.Append(postFix);
                return b.ToString();
            }
            /// <summary>
            /// Assert that 2 dictionaries are equal. Used for unit tests.
            /// </summary>
            /// <typeparam name="K"></typeparam>
            /// <typeparam name="V"></typeparam>
            /// <param name="d1"></param>
            /// <param name="d2"></param>
            public static void AssertDictionaryEqual<K,V>(IDictionary<K,V> d1, IDictionary<K,V> d2) {

                if (!DS.DictionaryHelper.Identical(d1,d2)) {

                    throw new DynamicSugarSharpException(String.Format("Dictionary are not equal D1:'{0}', D2:'{1}'", DS.DictionaryHelper.Format(d1), DS.DictionaryHelper.Format(d2)));
                }
            }                        
            /// <summary>
            /// Return true if the 2 passed dictionary contain the same key/value.
            /// </summary>
            /// <typeparam name="K">The type of the key</typeparam>
            /// <typeparam name="V">The type of the value</typeparam>
            /// <param name="d1">The first dictionary</param>
            /// <param name="d2">The second dictionary</param>
            /// <returns></returns>
            public static bool Identical<K,V>(IDictionary<K,V> d1, IDictionary<K,V> d2){

                if (d1.Count != d2.Count) return false;

                foreach(var k in d1){
                
                    if(d2.ContainsKey(k.Key)){
                        V v1 = d1[k.Key];
                        V v2 = d2[k.Key];
                        if ((v1 == null) && (v2 == null)) continue;
                        if (!v1.Equals(v2)) return false;
                    }
                    else return false;
                }
                return true;
            } 
        }
    }
}