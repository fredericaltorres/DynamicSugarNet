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
    /// Dynamic Sharp Helper Class, dedicated methods to work with list
    /// </summary>
    public  static partial class DS {

        public static class ListHelper {

            /// <summary>
            /// Return in a List Of T, call to the instance of the default method
            /// instance[0], instance[1], instance[2],...
            /// </summary>
            /// <param name="instance"></param>
            /// <returns></returns>
            private static List<object> __GetItems(object instance) {

                int count       = (int)ReflectionHelper.GetProperty(instance, "Count");
                List<Object> lo = new List<object>();

                for (int i = 0; i < count; i++)
                    lo.Add(ReflectionHelper.ExecuteMethod(instance, "get_Item", i));
            
                return lo;
            }
            /// <summary>
            /// Return a list of string read from a text file.
            /// Each line of the text file is an entry in a list.
            /// </summary>
            /// <param name="fileName">The filename to load</param>
            /// <returns>A List Of String</returns>
            private static List<string> __FromFileAsListOfString(string fileName) {

                List<string> l = new List<string>();

                if(System.IO.File.Exists(fileName)){

                    var text          = System.IO.File.ReadAllText(fileName);
                    string [] sepa    = new string [] { Environment.NewLine };
                    var lines         = text.Split(sepa, StringSplitOptions.None);
                    foreach(var e in lines)
                        l.Add(e);
                }
                return l;
            }
            /// <summary>
            /// Save the context of a List Of T to a file
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l">The list</param>
            /// <param name="fileName">The file name</param>
            /// <param name="create">If true create the file, else append to the file if the file already exist</param>
            public static void ToFile<T>(List<T> l, string fileName, bool create = false) {
                
                System.Text.StringBuilder b = new StringBuilder(1024);
                
                foreach(var s in l){

                    b.Append(s.ToString()).AppendLine();
                }
                if((create)&&(System.IO.File.Exists(fileName))) System.IO.File.Delete(fileName);

                var t = b.ToString();
                if(t.EndsWith(Environment.NewLine))
                    t = t.Substring(0, t.Length-System.Environment.NewLine.Length);

                if(System.IO.File.Exists(fileName))
                    System.IO.File.AppendAllText(fileName, t);
                else
                    System.IO.File.WriteAllText(fileName, t);
            }

            /// <summary>
            /// Return true if the list l contains the element v
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <param name="v"></param>
            /// <returns></returns>
            public static bool Include<T>(List<T> l, T v) {

                var tmpL = new List<T>();
                tmpL.Add(v);
                return DS.ListHelper.Include(l, tmpL);
            }
            /// <summary>
            /// Return true if the list of elements from l2 are all contained
            /// in the list l
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <param name="l2"></param>
            /// <returns></returns>
            public static bool Include<T>(List<T> l, List<T> l2) {

                foreach(var i in l2) if(!l.Contains(i)) return false;
                return true;
            }             
            /// <summary>
            /// Filter out element from a List Of T based on Func.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <param name="f">A func that can be implemented as lambda expression</param>
            /// <returns></returns>
            public static List<T> Reject<T>(List<T> l, Func<T, bool> f) {

                var lr = new List<T>();
                foreach (var e in l) 
                    if (!f(e)) 
                        lr.Add(e);
                return lr;
            }
            /// <summary>
            /// Read a List Of T from a file
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public static List<T> FromFile<T>(string fileName) {

                var l           = new List<T>();                          
                List<string> ll = __FromFileAsListOfString(fileName);            

                foreach(var s in ll)
                    l.Add((T)Convert.ChangeType(s, typeof(T)));                

                return l;
            }
            /// <summary>
            /// Extract from a list of a instance, the value of a specific property and
            /// return the value in a List Of T
            /// </summary>
            /// <typeparam name="T0"></typeparam>
            /// <typeparam name="T1"></typeparam>
            /// <param name="l"></param>
            /// <param name="propertyOrFunction"></param>
            /// <returns></returns>
            public static List<T0> Pluck<T0, T1>(List<T1> l, string propertyOrFunction) {

                List<T0> l1   = new List<T0>();
                bool isMethod = propertyOrFunction.EndsWith("()");
                if(isMethod) 
                    propertyOrFunction = propertyOrFunction.Substring(0, propertyOrFunction.Length-System.Environment.NewLine.Length);

                foreach(var i in l){
                    try{
                        object value = null;

                        if(isMethod ){
                            value = DynamicSugar.ReflectionHelper.ExecuteMethod(i, propertyOrFunction);
                        }
                        else{
                            value = DynamicSugar.ReflectionHelper.GetProperty(i, propertyOrFunction);                        
                        }
                        T0 v = (T0)Convert.ChangeType(value, typeof(T0));
                        l1.Add(v);
                    }
                    catch{}
                }
                return l1;
            }
            /// <summary>
            /// Return a List Of T without the first element
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <returns></returns>
            public static List<T> Rest<T>(List<T> l) {

                var l1 = DS.ListHelper.Clone(l);
                l1.RemoveAt(0);
                return l1;
            }
            /// <summary>
            /// Return the first element of a List Of T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <returns></returns>
            public static T First<T>(List<T> l) {

                if(l.Count>0)
                    return l[0];
                else
                    throw new DynamicSugarSharpException("Empty list cannot return first element");
            }
            /// <summary>
            /// Return the last element of a List Of T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <returns></returns>
            public static T Last<T>(List<T> l) {

                if(l.Count>0)
                    return l[l.Count-1];
                else
                    throw new DynamicSugarSharpException("Empty list cannot return first element");
            }   
            /// <summary>
            /// Merge to List Of T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            /// <param name="unique"></param>
            /// <returns></returns>
            public static List<T> Merge<T>(List<T> l1, List<T> l2, bool unique = true) {

                var lr = new List<T>();

                if(unique){
                    foreach (var e in l1) 
                        lr.Add(e);
                    foreach (var e in l2) 
                        if(!lr.Contains(e)) 
                            lr.Add(e);
                }
                else{
                    foreach (var e in l1) 
                        lr.Add(e);
                    foreach (var e in l2) 
                        lr.Add(e);
                }            
                return lr;
            }   
            /// <summary>
            /// 
            /// </summary>
            /// <param name="l"></param>
            /// <param name="f"></param>
            /// <returns></returns>
            public static T Inject<T>(List<T> l, Func<T,T,T> f) {

                T v = default(T);

                foreach (var e in l)
                    v = f(e, v);
                
                return v;
            }    
            /// <summary>
            /// Format a List Of T, with default FormatString parameters
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <returns></returns>
            public static string Format<T>(List<T> l) {

                return Format<T>(l, "{0}");
            }                
            /// <summary>
            /// Format a List Of T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <param name="format"></param>
            /// <param name="separator"></param>
            /// <param name="preFix"></param>
            /// <param name="postFix"></param>
            /// <returns></returns>
            public static string Format<T>(List<T> l, string format, string separator = ", ", string preFix = "", string postFix = "") {

                System.Text.StringBuilder b = new StringBuilder(1024);
                foreach (var e in l) {
                    
                    b.AppendFormat(format, ExtendedFormat.FormatValue(e));
                    b.Append(separator);
                }
                string r = b.ToString();
                if (r.EndsWith(separator)) 
                    r = r.Substring(0, r.Length - separator.Length);
                return preFix + r + postFix;
            }          
           
            /// <summary>
            /// Apply a Func to all the element of a List Of T and return the result
            /// in another List Of T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l"></param>
            /// <param name="f"></param>
            /// <returns></returns>
            //public static List<T> Map<T>(List<T> l, Func<T, T> f) {
            public static List<TResult> Map<TSource, TResult>(List<TSource> source, Func<TSource, TResult> selector){

                return source.Select(selector).ToList();                
            }
            
            
            /// <summary>
            /// Return true is 2 List Of T are identical
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            /// <returns></returns>
            public static bool Identical<T>(List<T> l1, List<T> l2) {

                if (l1.Count != l2.Count) return false;

                for (int i = 0; i < l1.Count; i++) {

                    T t1 = l1[i];
                    T t2 = l2[i];

                    if ((t1 == null) && (t2 == null)) continue;

                    if (!t1.Equals(t2)) return false;
                }
                return true;
            }     
            /// <summary>
            /// Return the intersection of 2 List Of T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            /// <returns></returns>
            public static List<T> Intersection<T>(List<T> l1, List<T> l2) {

                return l1.Intersect(l2).ToList();
            }
            /// <summary>
            /// Add to 2 List Of T together and return the result as a brand new list
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            /// <returns></returns>
            public static List<T> Add<T>(List<T> l1, List<T> l2) {

                List<T> l = Clone(l1);
                foreach(var e in l2)
                    l.Add(e);
                return l;
            }         
            /// <summary>
            /// Substract the element of the List Of T l2 from the List Of T l1
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            /// <returns></returns>
            public static List<T> Substract<T>(List<T> l1, List<T> l2) {
                        
                return l1.Except(l2).ToList();          
            } 
            /// <summary>
            /// Clone a List Of T
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <returns></returns>
            public static List<T> Clone<T>(List<T> l1) {

                var l = new List<T>();
                foreach(var e in l1)
                    l.Add(e);
                return l;
            }
           


            /// <summary>
            /// Return a List Of T without a set of elements
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            /// <returns></returns>
            public static List<T> Without<T>(List<T> l1, List<T> l2) {

                List<T> l = DS.ListHelper.Clone(l1);

                for (int i = 0; i < l2.Count; i++) {

                    var e = l2[i];
                    if(l.Contains(e)) l.Remove(e);
                }
                return l;
            }
        }
    }
}