using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace DynamicSugar
{
    public static class ExtensionMethods_Dictionary
    {
        public static string PreProcess<K, V>(this IDictionary<K, V> d, string template, params object[] args)
        {

            var d2 = new Dictionary<string, object>();

            foreach (var e in d)
                d2.Add(e.Key.ToString(), e.Value);

            return ExtendedFormat.Format(template, d2);
        }

        public static bool Include<K, V>(this IDictionary<K, V> d, object anonymousType)
        {

            if (anonymousType is IDictionary<K, V>)
                return d.Include((IDictionary<K, V>)anonymousType);
            else
            {
                var d1 = DS.Dictionary<V>(anonymousType);
                return d.Include(d1);
            }
        }
        public static bool Include<K, V>(this IDictionary<K, V> d, IDictionary<K, V> includedDictionary)
        {
            foreach (var k in includedDictionary.Keys)
                if (d.ContainsKey(k))
                {
                    if (!d[k].Equals(includedDictionary[k]))
                        return false;
                }
                else return false;
            return true;
        }
        public static bool Include<K, V>(this IDictionary<K, V> d, params K[] listOfKeys)
        {

            return d.Include(listOfKeys.ToList());
        }
        public static bool Include<K, V>(this IDictionary<K, V> d, List<K> listOfKeys)
        {
            foreach (var k in listOfKeys)
                if (!d.ContainsKey(k))
                    return false;
            return true;
        }
        /// <summary>
        /// Clone a IDictionary Of K,V
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <returns></returns>
        public static IDictionary<K, V> Clone<K, V>(this IDictionary<K, V> d)
        {

            IDictionary<K, V> cloned = new Dictionary<K, V>();
            foreach (var k in d.Keys)
                cloned.Add(k, d[k]);
            return cloned;
        }
        /// <summary>
        /// Remove entries from the dictionary 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <param name="entrieToRemove">Dictionary containing the entries to remove</param>
        /// <returns></returns>
        public static IDictionary<K, V> Remove<K, V>(this IDictionary<K, V> d, IDictionary<K, V> entrieToRemove)
        {

            Dictionary<K, V> newD = new Dictionary<K, V>();
            foreach (var k in d.Keys)
                if (!entrieToRemove.ContainsKey(k))
                    newD.Add(k, d[k]);
            return newD;
        }
        public static IDictionary<K, V> Remove<K, V>(this IDictionary<K, V> d, List<K> keysToRemove)
        {

            Dictionary<K, V> newD = new Dictionary<K, V>();
            foreach (var k in d.Keys)
                if (!keysToRemove.Contains(k))
                    newD.Add(k, d[k]);
            return newD;
        }


        public static string FromFileString(string s)
        {
            return  s.Replace("`r`n", Environment.NewLine);
        }

        public static string ToFileString(string s)
        {
            return s.Replace(Environment.NewLine, "`r`n");
        }


        public static void ToFile<K, V>(this IDictionary<K, V> d, string fileName, bool create)
        {
            var b = new StringBuilder(1024);
            foreach (var e in d)
            {
                b.AppendLine(ToFileString(e.Key.ToString()));
                b.AppendLine(ToFileString(e.Value.ToString()));
            }

            if ((create) && (File.Exists(fileName))) File.Delete(fileName);

            var t = b.ToString();
            if (t.EndsWith(Environment.NewLine))
                t = t.Substring(0, t.Length - System.Environment.NewLine.Length);

            if (File.Exists(fileName))
                File.AppendAllText(fileName, t);
            else
                File.WriteAllText(fileName, t);
        }
        private static Dictionary<string, string> __FromFileAsDictionaryOfString(string fileName)
        {
            var d = new Dictionary<string, string>();
            if (File.Exists(fileName))
            {
                var text = File.ReadAllText(fileName);
                var sepa = new string[] { Environment.NewLine };
                var lines = text.Split(sepa, StringSplitOptions.None);
                for (var i = 0; i < lines.Length; i += 2)
                {
                    d.Add(FromFileString(lines[i]), FromFileString(lines[i + 1]));
                }
            }
            return d;
        }

        public static IDictionary<K, V> FromFile<K, V>(this IDictionary<K, V> d, string fileName)
        {
            IDictionary<K, V> dd = new Dictionary<K, V>();

            var dStr = __FromFileAsDictionaryOfString(fileName);
            foreach (var e in dStr)
            {
                var k = (K)Convert.ChangeType(e.Key, typeof(K));
                var v = (V)Convert.ChangeType(e.Value, typeof(V));
                dd.Add(k, v);
            }

            return d.Clone().Merge(dd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <param name="d2"></param>
        /// <param name="overWrite"></param>
        /// <returns></returns>
        public static IDictionary<K, V> Merge<K, V>(this IDictionary<K, V> d, IDictionary<K, V> d2, bool overWrite = true)
        {
            var newD = d.Clone();
            foreach (var k in d2.Keys)
            {
                if (newD.ContainsKey(k))
                {
                    if (overWrite)
                    {
                        newD.Remove(k);
                        newD.Add(k, d2[k]);
                    }
                    else
                    {
                        // Do nothing, keep the current entry
                    }
                }
                else newD.Add(k, d2[k]);
            }
            return newD;
        }
        /// <summary>
        /// Return the key which has the greatest value, based on a list of keys
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static K Max<K, V>(this Dictionary<K, V> d, List<K> keys = null) where V : IComparable
        {
            K maxKey = default(K);
            V maxOcc = default(V);

            if (keys == null)
                keys = d.Keys.ToList();

            foreach (var ww in keys)
            {

                if (d.ContainsKey(ww))
                {
                    V counter = d[ww];
                    if (counter.CompareTo(maxOcc) > 0)
                    {
                        maxKey = ww;
                        maxOcc = counter;
                    }
                }
            }
            return maxKey;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static K Min<K, V>(this Dictionary<K, V> d, List<K> keys = null) where V : IComparable
        {
            K maxKey = default(K);
            V maxOcc = default(V);

            if (keys == null)
                keys = d.Keys.ToList();

            bool initialized = false;

            foreach (var ww in keys)
            {

                if (initialized)
                {
                    if (d.ContainsKey(ww))
                    {
                        V counter = d[ww];
                        if (counter.CompareTo(maxOcc) < 0)
                        {
                            maxKey = ww;
                            maxOcc = counter;
                        }
                    }
                }
                else
                {
                    maxKey = ww;
                    maxOcc = d[ww];
                    initialized = true;
                }
            }
            return maxKey;
        }
        /// <summary>
        /// Return the value of a key, if the key does not exist return the default value.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <param name="key">The key to search for</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns></returns>
        public static V Get<K, V>(this Dictionary<K, V> d, K key, V defaultValue)
        {

            return d.ContainsKey(key) ? d[key] : defaultValue;
        }
    }
}
