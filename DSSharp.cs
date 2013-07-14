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
    public class DynamicSugarSharpException : System.Exception {

        public DynamicSugarSharpException(string message) : base(message) { }
    }
    /// <summary>
    /// Dynamic Sharp Helper Class
    /// </summary>
    public static partial class DS {

        #if !MONOTOUCH

        /// <summary>
        /// Convert a System.Array into a list
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static List<T0> SystemArrayToList<T0>(System.Array a) {

            var l = new List<T0>();

            for (var i = 0; i < a.Length; i++) {

                T0 v = (T0)Convert.ChangeType(a.GetValue(i), typeof(T0));
                l.Add(v);
            }
            return l;
        }

        /// <summary>
        /// Initialize an Expando object with the properties of one or more instances passed 
        /// as parameters. Then return the expando object.
        /// </summary>
        /// <param name="instances"></param>
        /// <returns>An expando object</returns>
        public static dynamic Expando(params object [] instances) {

            dynamic expando   = new ExpandoObject();
            var expandoAsDict = expando as IDictionary<String, object>;

            for (int i = 0; i < instances.Length; i++){

                if(instances[i] is string){
                    expandoAsDict.Add(instances[i].ToString(), instances[i+1]);
                    i++;
                }
                else
                    foreach (KeyValuePair<string, object> k in ReflectionHelper.GetDictionary(instances[i])) 
                        expandoAsDict.Add(k.Key, k.Value);                
            }
            return expando;
        }       

        /// <summary>
        /// Wrap an anonynous type into a MultiValues object to be returned
        /// as a function result.
        /// </summary>
        /// <param name="anonymousType"></param>
        /// <returns></returns>
        public static dynamic Values(object anonymousType){

            return DynamicSugar.MultiValues.Values(anonymousType);
        }
        #endif
        /// <summary>
        /// Convert the parameters passed to this function into a Queue Of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Queue<T> Queue<T>(params T[] values) {

            var q = new Queue<T>();

            foreach(var v in values)
                q.Enqueue(v);

            return q;          
        }  
        /// <summary>
        /// Convert the parameters passed to this function into a Queue Of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Stack<T> Stack<T>(params T[] values) {

            var q = new Stack<T>();

            foreach(var v in values)
                q.Push(v);

            return q;          
        }  
        /// <summary>
        /// Convert the parameters passed to this function into a List Of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>       
        public static List<T> List<T>(params T[] values) {

            return values.ToList<T>();            
        }      
        /// <summary>
        /// Convert the parameters passed to this function into an array Of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>       
        public static T[] Array<T>(params T[] values) {

            return values;       
        }  
        /// <summary>
        /// Return in a dictionary of string, object all the properties and fields of an instance
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <param name="properties">Define the list of property and field to return. All properties and fields are returned if this parameter is not defined</param>
        /// <returns></returns>
        public static Dictionary<string,object> Dictionary(object instance, List<string> properties = null) {

            return ReflectionHelper.GetDictionary(instance, properties);
        }
        /// <summary>
        /// Return in a dictionary of string, T all the properties and fields of an instance.
        /// With the method the type of the dictionart value can be defined.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <param name="properties">Define the list of property and field to return. All properties and fields are returned if this parameter is not defined</param>
        /// <returns></returns>
        public static Dictionary<string, TValue> Dictionary<TValue>(object instance, List<string> properties = null) {

            Dictionary<string, TValue> d = new Dictionary<string,TValue>();
            foreach(var k in ReflectionHelper.GetDictionary(instance, properties)){
                TValue v = (TValue)Convert.ChangeType(k.Value, typeof(TValue));
                d.Add(k.Key, v);
            }
            return d;
        }                    
        /// <summary>
        /// Return a list of integer from 0 to max-1
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static List<int> Range(int max) {

            return Range(max, 1);
        }   
        /// <summary>
        /// Return a list of integer from 0 to max-1 with an increment
        /// </summary>
        /// <param name="max"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static List<int> Range(int max, int increment) {

            return Range(0, max, increment);
        }        
        /// <summary>
        /// Return a list of integer from start to max-1 with an increment
        /// </summary>
        /// <param name="start"></param>
        /// <param name="max"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static List<int> Range(int start, int max, int increment) {

            int i = start;
            var l = new List<int>();
            while (i < max) {
                l.Add(i);
                i += increment;
            }
            return l;
        }
    }
}
