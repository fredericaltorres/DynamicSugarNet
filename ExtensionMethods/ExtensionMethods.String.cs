using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

#if !MONOTOUCH
using System.Dynamic;
using System.Globalization;
#endif

namespace DynamicSugar {

    public static class ExtensionMethods_Format {

        public static int      ToInt     (this string s, int? defaultValue = null) { try { return int.Parse(s); } catch { if(defaultValue == null) throw;return (int)defaultValue; } }
        public static uint     ToUInt    (this string s, uint? defaultValue = null) { try { return uint.Parse(s);      } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static long     ToLong    (this string s, long? defaultValue = null) { try { return long.Parse(s);      } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static ulong    ToULong   (this string s, ulong? defaultValue = null) { try { return ulong.Parse(s);     } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }     
        public static UInt16   ToUInt16  (this string s, UInt16? defaultValue = null) { try { return UInt16.Parse(s);    } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static Int16    ToInt16   (this string s, Int16? defaultValue = null) { try { return Int16.Parse(s);     } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static UInt32   ToUInt32  (this string s, UInt32? defaultValue = null) { try { return UInt32.Parse(s);    } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static Int32    ToInt32   (this string s, Int32? defaultValue = null) { try { return Int32.Parse(s);     } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static UInt64   ToUInt64  (this string s, UInt64? defaultValue = null) { try { return UInt64.Parse(s);    } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static Int64    ToInt64   (this string s, Int64? defaultValue = null) { try { return Int64.Parse(s);     } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static decimal  ToDecimal (this string s, decimal? defaultValue = null) { try { return decimal.Parse(s);   } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }    
        public static double   ToDouble  (this string s, double? defaultValue = null) { try { return double.Parse(s);    } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }    
        public static DateTime ToDateTime(this string s, DateTime? defaultValue = null) { try { return DateTime.Parse(s);  } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }     
        public static Guid     ToGuid    (this string s, Guid? defaultValue = null) { try { return new Guid(s);        } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
        public static bool     ToBool    (this string s, bool? defaultValue = null) { try { return bool.Parse(s);      } catch { if(defaultValue == null) throw;return defaultValue.Value; }  }   
  
        /// <summary>
        /// The slice() method selects a part of a string.
        /// The original string is not be changed.
        /// </summary>
        /// <param name="s">the string</param>
        /// <param name="index">An integer that specifies where to start the selection 
        /// (The first element has an index of 0). 
        /// You can also use negative numbers to select from the end of the string</param>
        /// <param name="len">
        /// Optional. An integer that specifies how many char to return. If omitted, slice() selects 
        /// all elements from the start position and to the end of the string.
        /// </param>
        /// <returns></returns>
        public static string Slice(this string s, int index, int len=-1){

            int x             = index;
            bool allDefined   = len == -1;
            string result     = null;
        
            if(x<0){
                string _stringReversed = s.Reverse();
                var index2             = Math.Abs(x)-1;
                
                if(allDefined)
                    len = _stringReversed.Length-index2;
                if (index2 < _stringReversed.Length)
                    result = _stringReversed.Substring(index2, len);
                else
                    result = ""; // If the index goes over the limit of the string we return ""                
            }
            else{
                
                if(allDefined)
                    len = s.Length-x;
                if(x<s.Length)
                    result = s.Substring(x, len);
                else
                    result = ""; // If the index goes over the limit of the string we return ""                
            }
            return result;
        }

        /// <summary>
        /// Capitalize the string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="charToRemove">If defined only remove the first char if it is equal to charToRemove</param>
        /// <returns></returns>
        public static string Capitalize(this string s, CultureInfo cultureInfo = null)
        {
            if(s == null)
                return null;

            if(cultureInfo == null)
                cultureInfo = new CultureInfo("en-US", false);

            s        = s.ToLower(cultureInfo);
            var t    = cultureInfo.TextInfo;
            return s = t.ToTitleCase(s.ToLower(cultureInfo));
        }       
        /// <summary>
        /// Remove the first char
        /// </summary>
        /// <param name="s"></param>
        /// <param name="charToRemove">If defined only remove the first char if it is equal to charToRemove</param>
        /// <returns></returns>
        public static string RemoveFirstChar(this string s, char charToRemove = '\0')
        {
            if(s == null)
                return null;
            if (charToRemove == '\0') {
                if(s.Length > 0) {
                    s = s.Substring(1);
                }
            }
            else {
                if(s.Length > 0 && s[0] == charToRemove) {
                    s = s.Substring(1);
                }
            }
            return s;
        }       
        /// <summary>
        /// Remove the first char
        /// </summary>
        /// <param name="s"></param>
        /// <param name="charToRemove">If defined only remove the first char if it is equal to charToRemove</param>
        /// <returns></returns>
        public static string RemoveLastChar(this string s, char charToRemove = '\0')
        {
            if(s == null)
                return null;
            if (charToRemove == '\0') {
                if(s.Length > 0) {
                    s = s.Substring(0, s.Length-1);
                }
            }
            else {
                if(s.Length > 0 && s[s.Length-1] == charToRemove) {
                    s = s.Substring(0, s.Length-1);
                }
            }
            return s;
        }       
        /// <summary>
        /// Return the string reversed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Reverse(this string s) {
	     
            char[] arr = s.ToCharArray();
	        Array.Reverse(arr);
	        return new string(arr);
        }                
        ///// <summary>
        /////  Concatenates the elements of an object array, using the specified separator
        ////   between each element.
        ///// </summary>
        ///// <param name="s"></param>
        ///// <param name="values">An array that contains the elements to concatenate.</param>
        ///// <returns>A string that consists of the elements of values delimited by the separator
        ///// string.</returns>
        //public static string join(this string s, params object[] values) {
            
        //    return string.Join(s, values);
        //}      
        /// <summary>
        ///  Concatenates the elements of a List Of T, using the specified separator
        ///   between each element.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values">A List Of T contains the elements to concatenate.</param>
        /// <returns>A string that consists of the elements of values delimited by the separator
        /// string.</returns>
        public static string Join<T>(this string s, List<T> values) {
            
            return string.Join(s, values.ToArray());
        }
        /// <summary>
        /// Return the value of the string <paramref name="s"/> if not null
        /// or empty else return the default value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string s, string defaultValue)
        {
            return string.IsNullOrEmpty(s) ? defaultValue : s;
        }
        /// <summary>
        /// Indicates whether the specified string is null or an System.String.Empty
        /// string.
        /// </summary>
        /// <param name="s">The string to test</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string s) {
            
            return string.IsNullOrEmpty(s);            
        }      
        /// <summary>
        /// Indicates whether the specified string is an System.String.Empty
        /// string.
        /// </summary>
        /// <param name="s">The string to test</param>
        /// <returns>true if the value parameter is an empty string (""); otherwise, false.</returns>
        public static bool IsEmpty(this string s) {
            
            return (s!=null) && s.Length==0;
        }      
        /// <summary>
        ///  Replaces the format item in the string with the string representation
        ///  of a corresponding object in a specified array.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args">An object array that contains zero or more objects to format</param>
        /// <returns>A copy of format in which the format items have been replaced by the string
        /// representation of the corresponding objects in args.
        /// </returns>
        public static string format(this string s, params object[] args) {
            
            return string.Format(s, args);
        }
        /// <summary>
        ///  Replaces the format item in the string with the string representation
        ///  of a corresponding property/field in the object passed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="poco">Any poco object</param>
        /// <returns></returns>
        public static string Format(this string s, object poco) {
         
            return ExtendedFormat.Format(s, ReflectionHelper.GetDictionary(poco));
        }       
    }
}
/*
 * public static string Slice(this string _string, int index, int len0=int.MinValue){
            
            int len           = -1;
            int x             = index;
            bool rangeDefined = len0!=int.MinValue;
            bool allDefined   = false;
            string result     = null;

            if(rangeDefined){
                len        = len0;
                allDefined = len == -1;
            }
            if(x<0){
                string _stringReversed = _string.Reverse();
                var index2             = Math.Abs(x)-1;
                if(rangeDefined){
                    if(allDefined)
                        len = _stringReversed.Length-index2;
                    if (index2 < _stringReversed.Length)
                        result = _stringReversed.Substring(index2, len);
                    else
                        result = ""; // If the index goes over the limit of the string we return ""
                }
                else                                        
                    result = _stringReversed[index2].ToString();
            }
            else{
                if(rangeDefined){
                    if(allDefined)
                        len = _string.Length-x;
                    if(x<_string.Length)
                        result = _string.Substring(x, len);
                    else
                        result = ""; // If the index goes over the limit of the string we return ""
                }
                else
                    result = _string[x].ToString();
            }
            return result;
        }   
*/