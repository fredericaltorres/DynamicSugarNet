using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
#if !MONOTOUCH
using System.Dynamic;
#endif

namespace DynamicSugar {

    /// <summary>
    /// 
    /// </summary>
    public class ExtendedFormatException : System.Exception {

        public ExtendedFormatException(string message) : base(message) {

        }
    }
    /// <summary>
    /// String.Format reference
    ///     http://blog.stevex.net/string-formatting-in-csharp/
    /// </summary>
    public class ExtendedFormat {
        /// <summary>
        /// 
        /// </summary>
        const string SYNTAX_ERROR_IN_FORMAT                        = "Syntax error in the FormatString";
        const string SYNTAX_ERROR_IN_FORMAT_PROPERTY_NAME_IS_EMPTY = SYNTAX_ERROR_IN_FORMAT + ". Property name is empty:'{0}'";
        const string PROPERTY_NOT_FIND_IN_OBJECT                   = "Property '{0}' not find in object '{1}'";
        const string SYNTAX_ERROR_IN_FORMAT_MISSING_CLOSING_BRAKET = "Syntax error in the FormatString - missing closing braket:'{0}'";
        const string KEY_NOT_FIND_IN_DICTIONARY                    = "Key '{0}' not find in dictionary ";

        /// <summary>
        /// Tokeninze the FormatString and return a list.
        /// There 2 type of token:
        ///   (1) { .NET-ID or string.FormatString-FormatString }
        ///   (2) Anything else
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        /// 
        public static List<string> TokenizeFormat(string format) {

            List<string> l               = new List<string>();
            StringBuilder s              = new StringBuilder(1024);
            int i                        = 0; 
            while (i < format.Length) {

                if ( // support escaping { and } by doubling it
                    ((format[i] == '{') && (i < format.Length - 1) && (format[i + 1] == '{')) ||
                    ((format[i] == '}') && (i < format.Length - 1) && (format[i + 1] == '}'))
                    ) {

                    if (s.Length>0) l.Add(s.ToString());
                    s.Clear();
                    l.Add(format[i].ToString() + format[i + 1].ToString());
                    i++;
                }
                else if (format[i] == '{') {

                    if (s.Length > 0) l.Add(s.ToString());
                    s.Clear();

                    while ((i < format.Length)&&(format[i] != '}')) {

                        s.Append(format[i]); i++;
                    }
                    if (i < format.Length) {

                        s.Append(format[i]);  // i will be incremented at the end of the loop
                        l.Add(s.ToString());
                        s.Clear();
                    }
                    else {
                        throw new ExtendedFormatException(String.Format(SYNTAX_ERROR_IN_FORMAT_MISSING_CLOSING_BRAKET, format));
                    }
                }
                else if (format[i] == '}') {
                    
                    throw new ExtendedFormatException(String.Format(SYNTAX_ERROR_IN_FORMAT_MISSING_CLOSING_BRAKET, format));
                }
                else {
                    s.Append(format [i]); 
                }
                i++;
            }
            if (s.Length > 0)
                l.Add(s.ToString());
            return l;
        }        
        /// <summary>
        /// returns true is value is a string contains a valid .NET (C#, VB.NET) ID.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDotNetID(string value) {
            
            Regex rx         = new Regex("^[A-Z,_][0-9,A-Z,_]*$", RegexOptions.IgnoreCase);
            Regex rxFunction = new Regex(@"^[A-Z,_][0-9,A-Z,_]*\(\)$", RegexOptions.IgnoreCase);
            return rx.IsMatch(value)||rxFunction.IsMatch(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(object instance, string format, params object[] args) {

            return Format(instance, format, null, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="dictionary"></param>
        /// <param name="provider"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(string format, IDictionary<string, object> dictionary, IFormatProvider provider, params object[] args)
        {
            return __Format(null, dictionary, format, provider, args); 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(object instance, string format, IFormatProvider provider, params object[] args)
        {
            return __Format(instance, null, format, provider, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dictionary"></param>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string __Format(object instance, IDictionary<string, object> dictionary, string format, IFormatProvider provider, params object[] args) {

            if (format == null)
                throw new ArgumentNullException("format");

            StringBuilder b = new StringBuilder(1024);            
            var values      = TokenizeFormat(format);

            for (int i = 0; i < values.Count; i++) {

                if ((values[i].StartsWith("{"))&&(values[i].EndsWith("}"))) {

                    var methodOrPropertyName = values[i].Substring(1, values[i].Length - 2).Trim();
                    var valueFormat          = "";

                    if(methodOrPropertyName.Contains(":")){

                        int p                = methodOrPropertyName.IndexOf(":");
                        valueFormat          = methodOrPropertyName.Substring(p+1);
                        methodOrPropertyName = methodOrPropertyName.Substring(0, p);
                    }
                    if (IsDotNetID(methodOrPropertyName)) {

                        string v = null;
                        if (instance != null)
                        {
                            v = EvaluatePropertyOrFunction(instance, methodOrPropertyName, valueFormat);
                        }
                        else if (dictionary!=null)
                        {
                            if (dictionary.ContainsKey(methodOrPropertyName))
                            {
                                v = EvaluateValue(valueFormat, dictionary[methodOrPropertyName]);
                            }
                            else throw new ExtendedFormatException(String.Format("Property '{0}' not found in dictionary, you may have a typo in the FormatString", methodOrPropertyName));
                        }
                        b.Append(v);
                    }
                    else {
                        b.Append(values[i]);
                    }
                }
                else {
                    b.Append(values[i]);
                }
            }
            string s = "";
            if(provider==null)
                s = String.Format(b.ToString(), args);
            else
                s = String.Format(b.ToString(), provider, args);
            return s;
        }
        /// <summary>
        /// Evaluate a property or function on an instance and return the result.
        /// List Of T value are evaluated as [1,2,3]
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodOrPropertyName"></param>
        /// <param name="valueFormat"></param>
        /// <returns></returns>
        private static string EvaluatePropertyOrFunction(object instance, string methodOrPropertyName, string valueFormat) {

            bool functionCallMode = false;
            string v              = null;

            if (methodOrPropertyName.EndsWith("()")) {

                methodOrPropertyName = methodOrPropertyName.Substring(0, methodOrPropertyName.Length - 2);
                functionCallMode     = true;
            }
            
            if (functionCallMode){

                var o  = ReflectionHelper.ExecuteMethod(instance, methodOrPropertyName);                
                var ov = Generated.FormatValueBasedOnType(o, valueFormat);
                v      = EvaluateValue(valueFormat, ov);
            }
            else{
                if (ReflectionHelper.PropertyExist(instance, methodOrPropertyName)) {                    

                    var ov = ReflectionHelper.GetProperty(instance, methodOrPropertyName);
                    v      = EvaluateValue(valueFormat, ov);
                }
                else{
                    throw new ExtendedFormatException(String.Format(PROPERTY_NOT_FIND_IN_OBJECT, methodOrPropertyName, instance.GetType().FullName));
                }
            }
            return v;
        }
        
        /// <summary>
        /// if the value is a List Of T, we FormatString it as a list [1,2,3]
        /// else we FormatString the value
        /// </summary>
        /// <param name="valueFormat"></param>
        /// <param name="ov"></param>
        /// <returns></returns>
        private static string EvaluateValue(string valueFormat, object ov) {

            string v = "";
            if(ov==null) return null;
			if(ov==System.DBNull.Value) return null;
           
            if(ReflectionHelper.IsTypeListOfT(ov.GetType())){
                
                Type lt = ReflectionHelper.GetListType(ov.GetType());
                if (lt != null) {                                                        
                    v = typeof(ExtendedFormat).GetMethod("ListToString").MakeGenericMethod(lt).Invoke(null, new object [] { ov }).ToString();
                }
            }
            else {
                v = Generated.FormatValueBasedOnType(ov, valueFormat);
            }
            return v;
        }
        /// <summary>
        /// Convert to string the value in v and apply the FormatString
        /// </summary>
        /// <param name="v"></param>
        /// <param name="format"></param>
        /// <returns></returns>       
        //public static string Format(string FormatString, dynamic expandoBag,params object[] args) {

        //    IDictionary<string, object> dic = null;

        //    if ((expandoBag != null) && (expandoBag is ExpandoObject))
        //        dic = expandoBag as IDictionary<string, object>;
        //    else
        //        throw new System.FormatException("parameter expandoBag is not of type System.Dynamic.ExpandoObject");
        //    return Format(FormatString, dic, null, args);
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="dictionary"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(string format, IDictionary<string, object> dictionary, params object[] args) {

            return Format(format, dictionary, null, args);
        }
       
        /// <summary>
        /// Returns true if a value of the type passed must be formatted with double quote
        /// like string and datetime
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsTypeNeedDoubleQuote(Type t) {

            return (t == typeof(System.String)) || (t == typeof(System.DateTime));
        }
        /// <summary>
        /// Returns true if the value passed must be formatted with double quote
        /// like string and datetime
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsTypeNeedDoubleQuote(object o) {
            
            return IsTypeNeedDoubleQuote(o.GetType());
        }
        /// <summary>
        /// Format a IDictionary of K,V to a { key1:value1, key2:value2} string.
        /// Type string and datetime are doublequoted for the keys and values.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string DictionaryToString<K,V>(IDictionary<K,V> dict) {

            if (dict == null) { return "Null"; } // just in case
            
            System.Text.StringBuilder b = new StringBuilder(1024);
            b.Append("{");

            foreach(KeyValuePair<K,V> kv in dict){

                string value = ExtendedFormat.FormatValue(kv.Value);
                string key   = ExtendedFormat.FormatValue(kv.Key);
                b.AppendFormat("{0}:{1}, ", key, value);
            }
            b.Remove(b.Length-System.Environment.NewLine.Length,System.Environment.NewLine.Length); // Remove the coma and the space
            b.Append("}");
            return b.ToString();
        }
        /// <summary>
        /// Format an array to a [value1, value2] string.
        /// Type string and datetime are doublequoted.
        /// A null value will be returned a "Null"
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string ArrayToString(System.Array arr) {

            System.Text.StringBuilder arrText = new StringBuilder(16384);
            
            if (arr == null) { return "Null"; } // just in case

            arrText.Append("["); 

            for (int i = 0; i < arr.Length; i++) {

                arrText.Append(FormatValue(arr.GetValue(i)));                
                if (i < arr.Length - 1) arrText.Append(", ");
            }
            arrText.Append("]");
            return arrText.ToString();
        }
        /// <summary>
        /// Format a list of T to a [value1, value2] string.
        /// Type string and datetime are doublequoted
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToString<T>(IList<T> list) {

            if (list == null) { return "Null"; } // just in case

            System.Text.StringBuilder b = new StringBuilder(1024);
            b.Append("[");
            for (int i = 0; i < list.Count; i++) {
                var e = list [i];
                b.Append(FormatValue(e));
                if (i < list.Count - 1) b.Append(", ");
            }
            b.Append("]");
            return b.ToString();
        }
        /// <summary>
        /// Format a value. A value here is the value of a parameter passed to a
        /// C# method.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatValue(object value, bool doubleQuoteIfNeeded = true) {

            if (value == null)
                return "null";
            else if (value == DBNull.Value)
                return "DBNull";
            else if (doubleQuoteIfNeeded && ExtendedFormat.IsTypeNeedDoubleQuote(value))
                return String.Format("\"{0}\"", value);
            else
                return value.ToString();
        }
         
      
    }
}
