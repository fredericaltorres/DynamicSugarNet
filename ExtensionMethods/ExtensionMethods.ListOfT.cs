using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

namespace DynamicSugar {

    public static class ExtensionMethods_ListOfT{

        public static bool IsNullOrEmpty<T>(this List<T> items)
        {
            return items == null || items.Count==0;
        }
        public static bool IsEmpty<T>(this List<T> l1) {

            return l1.Count==0;
        }
        public static void ToFile<T>(this List<T> l, string fileName, bool create = false) {
            
            DS.ListHelper.ToFile(l, fileName, create);
        }
        public static List<T>  Without<T>(this List<T> l1, params T[] values) {

            return DS.ListHelper.Without(l1, values.ToList());
        }
        public static List<T>  Without<T>(this List<T> l1, List<T> l2) {

            return DS.ListHelper.Without(l1, l2);
        }
        public static bool Include<T>(this List<T> l1, T v) {
            
            return DS.ListHelper.Include(l1, v);
        }
        public static bool Include<T>(this List<T> l1, List<T> l2) {

            return DS.ListHelper.Include(l1, l2);
        }
        public static bool Include<T>(this List<T> l1, params T[] values) {

            return DS.ListHelper.Include(l1, values.ToList<T>());
        }
        public static List<T0> Pluck<T0,T1>(this List<T1> l, string propertyOrFunction) {

            return DS.ListHelper.Pluck<T0,T1>(l, propertyOrFunction);
        }
        public static List<T> Rest<T>(this List<T> l){

            return DS.ListHelper.Rest(l);
        }
        public static T First<T>(this List<T> l){

            return DS.ListHelper.First(l);
        }
        public static T Last<T>(this List<T> l){

            return DS.ListHelper.Last(l);
        }        
        public static List<T> Merge<T>(this List<T> l1, List<T> l2, bool unique = false) {

            return DS.ListHelper.Merge(l1, l2, unique);
        }
        public static T Inject<T>(this List<T> l, Func<T,T,T> f) {

            return DS.ListHelper.Inject(l, f);
        }
        public static T Reduce<T>(this List<T> l, Func<T, T, T> f) {

            return l.Inject(f);
        }
        public static bool Identical<T>(this List<T> l1, List<T> l2) {

            return DS.ListHelper.Identical(l1, l2);
        }
        public static List<TResult> Map<TSource, TResult>(this List<TSource> source, Func<TSource, TResult> selector){

            return DS.ListHelper.Map(source, selector);
        }
        public static string Format<T>(this List<T> l) {

            return DS.ListHelper.Format(l);
        }
        public static string Format<T>(this List<T> l, string format, string separator) {

            return DS.ListHelper.Format<T>(l, format, separator);
        }
        /// <summary>
        ///  HERE
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="format"></param>
        /// <param name="separator"></param>
        /// <param name="preFix"></param>
        /// <param name="postFix"></param>
        /// <returns></returns>
        public static string Format<K,V>(this IDictionary<K,V> dictionary, string format="{0}:{1}", string separator = ", ", string preFix = "{ ", string postFix = " }") {

            return DS.DictionaryHelper.Format(dictionary, format, separator, preFix, postFix);                
        }
        public static string Format<K,V>(this Dictionary<K,V> dictionary, string format="{0}:{1}", string separator = ", ", string preFix = "{ ", string postFix = " }") {

            return DS.DictionaryHelper.Format(dictionary, format, separator, preFix, postFix);                
        }

        public static List<T> Filter<T>(this List<T> l, Predicate<T> match) {
                        
            return l.FindAll(match);
        }
        public static List<T> Reject<T>(this List<T> l, Func<T, bool> f) {
                        
            return DS.ListHelper.Reject(l, f);
        }
        public static List<T> Add<T>(this List<T> l1, List<T> l2) {

            return DS.ListHelper.Add(l1, l2);
        }
        public static List<T> Substract<T>(this List<T> l1, List<T> l2) {

            return DS.ListHelper.Substract(l1, l2);
        }
        public static List<T> Intersection<T>(this List<T> l1, List<T> l2) {

            return DS.ListHelper.Intersection(l1, l2);
        }      
        public static List<T> Clone<T>(this List<T> l) {

            return DS.ListHelper.Clone(l);
        }   
    }
}
