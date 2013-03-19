using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace DynamicSugar {

    public static class ExtensionMethods_General {

        public static List<string> ToList(this MatchCollection matchCollection){ 

            var l = new List<string>();
            foreach (var m in matchCollection)
                l.Add(m.ToString());
            return l;
        }
        public static bool In<T>(this T value, params T[] values) {
        
            return values.Contains(value);
        }
        public static bool In<T>(this T value, List<T> l2) {
        
            return l2.Contains(value);
        }
        public static bool In<T, V>(this T value,  Dictionary <T, V> dic) {

            return dic.ContainsKey(value);
        }
    }
}
