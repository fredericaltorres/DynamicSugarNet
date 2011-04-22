using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Dynamic;
using System.Reflection;

namespace DynamicSugar {
    
    /// <summary>
    /// Dynamic Sharp Helper Class
    /// </summary>
    public  static partial class DS {

        public static class DictionaryHelper {

/// <summary>
        /// Format a dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="format"></param>
        /// <param name="separator"></param>
        /// <param name="preFix"></param>
        /// <param name="postFix"></param>
        /// <returns></returns>
        public static string Format<K,V>(IDictionary<K,V> dictionary, string format="{0}:{1}", string separator = ", ", string preFix = "{ ", string postFix = " }") {

            if (dictionary == null) { return "Null"; } // just in case
            
            System.Text.StringBuilder b = new StringBuilder(1024);
            b.Append(preFix);

            foreach(KeyValuePair<K,V> kv in dictionary){
                
                string value = ExtendedFormat.FormatValue(kv.Value, true);
                string key   = ExtendedFormat.FormatValue(kv.Key, false);
                b.AppendFormat(format, key, value);
                b.Append(separator);
            }
            b.Remove(b.Length-separator.Length,separator.Length); // Remove the separator
            b.Append(postFix);
            return b.ToString();
        }

            public static void AssertDictionaryEqual<K,V>(IDictionary<K,V> d1, IDictionary<K,V> d2) {

                if (!DS.DictionaryHelper.Identical(d1,d2)) {

                    throw new DynamicSugarSharpException(String.Format("Dictionary are not equal D1:'{0}', D2:'{1}'", DS.DictionaryHelper.Format(d1), DS.DictionaryHelper.Format(d2)));
                }
            }
                        
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