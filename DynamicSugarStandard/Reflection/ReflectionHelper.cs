using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics.SymbolStore;
using System.Text.RegularExpressions;

namespace DynamicSugar
{

    public class ParameterMetadata
    {

        public bool IsIn;
        public bool IsOptional;
        public bool IsOut;
        public bool IsRetval;
        public string Name;
        public Type ParameterType;
        public int Position;
        public object RawDefaultValue;
        public object Value;

        public ParameterMetadata(ParameterInfo pi, object value)
        {

            this.IsIn = pi.IsIn;
            this.IsOptional = pi.IsOptional;
            this.IsOut = pi.IsOut;
            this.IsRetval = pi.IsRetval;
            this.Name = pi.Name;
            this.ParameterType = pi.ParameterType;
            this.Position = pi.Position;
            this.RawDefaultValue = pi.RawDefaultValue;
            this.Value = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReflectionHelper
    {
        const BindingFlags GET_PRIVATE_PROPERTY_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty;
        const BindingFlags GET_PUBLIC_PROPERTY_FLAGS  = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public    | BindingFlags.GetField | BindingFlags.GetProperty;

        const BindingFlags GET_PRIVATE_AND_PUBLIC_PROPERTY_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty;

        const BindingFlags GET_PUBLIC_STATIC_PROPERTY_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty;

        const BindingFlags CALL_METHOD_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod;
        const BindingFlags CALL_STATIC_METHOD_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;

        public static string GetStackCall()
        {
            try
            {
                System.Reflection.MethodBase method = null;
                var b = new StringBuilder();
                int i = 1;
                int max = (new System.Diagnostics.StackTrace(true)).FrameCount;
                while (i < max)
                {

                    method = new System.Diagnostics.StackTrace(true).GetFrame(i).GetMethod();

                    if (method == null)
                        break;
                    else
                    {
                        if (
                            (method.Name.ToLower() != "trace") &&
                            (method.Name.ToLower() != "trace_error") &&
                            (method.Name.ToLower() != "trace_method")
                        )
                        {
                            b.AppendFormat("{1}.{0} -> ", method.Name, method.ReflectedType.FullName);
                        }
                    }
                    i++;
                }
                return b.ToString();
            }
            catch
            {
                return null;
            }
        }
        public static System.Reflection.MethodBase GetCallingMethod(params string[] methodToIgnore)
        {

            return GetCallingMethod(methodToIgnore.ToList());
        }
        public static System.Reflection.MethodBase GetCallingMethod(List<string> methodToIgnore = null)
        {
            try
            {
                if (methodToIgnore == null)
                    methodToIgnore = new List<string>();

                methodToIgnore.Add("GetCallingMethod");// Alway ignore the current method                

                MethodBase method = null;
                int i = 1;
                int max = (new System.Diagnostics.StackTrace(true)).FrameCount;

                while (i <= max)
                {

                    method = new System.Diagnostics.StackTrace(true).GetFrame(i).GetMethod();

                    if (!method.Name.In(methodToIgnore))
                        break;
                    i++;
                }
                return method;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Create new instance of the type
        /// </summary>
        /// <param name="typeToInstantiate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object Constructor(Type typeToInstantiate, params object[] parameters)
        {

            object o = Activator.CreateInstance(typeToInstantiate, parameters);
            return o;
        }
        /// <summary>
        /// Create new instance of the type with casting in the right type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeToInstantiate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T Constructor<T>(Type typeToInstantiate, params object[] parameters)
        {

            object o = Activator.CreateInstance(typeToInstantiate, parameters);
            T t = (T)Convert.ChangeType(o, typeof(T));
            return t;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static Dictionary<string, Object> GetLocals(params object[] parameterValues)
        {

            var dic = new Dictionary<string, Object>();
            var m = GetCallingMethod("GetLocals");
            var i = 0;
            foreach (var mp in m.GetParameters())
                dic.Add(mp.Name, parameterValues[i++]);
            return dic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static Dictionary<string, ParameterMetadata> GetLocalsEx(params object[] parameterValues)
        {

            Dictionary<string, ParameterMetadata> dic = new Dictionary<string, ParameterMetadata>();
            MethodBase m = GetCallingMethod(DS.List("GetLocalsEx"));
            int i = 0;
            foreach (var mp in m.GetParameters())
                dic.Add(mp.Name, new ParameterMetadata(mp, parameterValues[i++]));
            return dic;
        }
        /// <summary>
        /// Clone a dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<K, V> CloneDictionary<K, V>(IDictionary<K, V> dic)
        {

            Dictionary<K, V> d = new Dictionary<K, V>();
            foreach (KeyValuePair<K, V> v in dic)
                d.Add(v.Key, v.Value);
            return d;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic1"></param>
        /// <param name="dic2"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static Dictionary<K, V> MergeDictionary<K, V>(IDictionary<K, V> dic1, IDictionary<K, V> dic2, bool overwrite = false)
        {

            Dictionary<K, V> d = CloneDictionary(dic1);
            foreach (KeyValuePair<K, V> v in dic2)
                if (overwrite)
                {
                    if (d.ContainsKey(v.Key))
                        d.Remove(v.Key);
                    d.Add(v.Key, v.Value);
                }
                else
                {
                    if (!d.ContainsKey(v.Key))
                        d.Add(v.Key, v.Value);
                }
            return d;
        }

#if USE_FAST_REFLECTION
            private static Dictionary<string, List<string>> __TypeDefined = new Dictionary<string, List<string>>();
#endif

        public static Dictionary<string, string> GetDictionaryWithType(object o, List<string> propertiesToInclude = null, bool removeSystemDot = true)
        {
            var dic = new Dictionary<string, string>();
            dic = DynamicSugar.ReflectionHelper.GetDictionaryWithTypeReflection(o, propertiesToInclude);
            if(removeSystemDot)
            {
                const string TAG = "System.";
                var dic2 = new Dictionary<string, string>();
                foreach (var k in dic)
                {
                    if (k.Value.StartsWith(TAG))
                        dic2.Add(k.Key, k.Value.Replace(TAG, ""));
                    else
                        dic2.Add(k.Key, k.Value);
                }

                dic = dic2;
            }
            return dic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetDictionary(object o, List<string> propertiesToInclude = null, bool allProperties = false)
        {

            var dic = new Dictionary<string, object>();

            // Support Dictionary and ExpandoObect
            if (o is IDictionary<string, object>)
            {

                // We have to make a copy, because returning an ExpandoObject as an IDictionary, do not support the dic["name"] syntax.
                foreach (KeyValuePair<string, object> v in ((IDictionary<string, object>)o))
                    if ((propertiesToInclude == null) || (propertiesToInclude.Contains(v.Key)))
                        dic.Add(v.Key, v.Value);
                return dic;
            }
#if USE_FAST_REFLECTION
                var typeFullName  = o.GetType().FullName;
                if (!__TypeDefined.ContainsKey(typeFullName))
                {
                    var l = new List<string>();
                    foreach (var k in DynamicSugar.ReflectionHelper.GetDictionaryReflection(o))
                        l.Add(k.Key);                    
                    __TypeDefined.Add(typeFullName, l);
                }
                foreach (var p in __TypeDefined[typeFullName]) {
                    var pp = o.GetType().GetProperty(p);                    
                    if(pp==null)
                        throw new System.ApplicationException("Field '{0}' in object '{1}' is not supported. Only properties are supported".FormatString(p, o));
                    dic.Add(p, FastProperty<object, object>.Make(pp).Get(o));
                }
#else
            dic = DynamicSugar.ReflectionHelper.GetDictionaryReflection(o, propertiesToInclude, allProperties);
#endif
            return dic;
        }

        private static string ExtractListType(Type type)
        {
            if (type.Name.StartsWith("List`"))
            {
                if (type.GenericTypeArguments.Length > 0)
                    return $"List<{type.GenericTypeArguments[0].Name}>";
            }
            return "List<Unknown>";
        }

        private static string ExtractDictionaryType(Type type)
        {
            if (type.Name.StartsWith("Dictionary`"))
            {
                if (type.GenericTypeArguments.Length > 1)
                    return $"Dictionary<{type.GenericTypeArguments[0].Name}, {type.GenericTypeArguments[1].Name}>";
            }
            return "Dictionary<Unknown, Unknown>";
        }

        /// <summary>
        /// Get a dictionary from an poco object using reflection only, the value is the type of the property
        /// as a string
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertiesToInclude"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetDictionaryWithTypeReflection(object o, List<string> propertiesToInclude = null)
        {
            var dic = new Dictionary<string, string>();

            foreach (var p in o.GetType().GetProperties(GET_PUBLIC_PROPERTY_FLAGS))
            {
                if ((propertiesToInclude == null) || (propertiesToInclude.Contains(p.Name)))
                {
                    if (p.GetMethod.ReturnType.Name.StartsWith("List`"))
                        dic.Add(p.Name, ExtractListType(p.GetMethod.ReturnType));
                    else if (p.GetMethod.ReturnType.Name.StartsWith("Dictionary`"))
                        dic.Add(p.Name, ExtractDictionaryType(p.GetMethod.ReturnType));
                    else
                        dic.Add(p.Name, p.GetMethod.ReturnParameter.ParameterType.FullName);
                }
            }

            foreach (var p in o.GetType().GetFields(GET_PUBLIC_PROPERTY_FLAGS))
            {
                if ((propertiesToInclude == null) || (propertiesToInclude.Contains(p.Name)))
                {
                    if (p.FieldType.Name.StartsWith("List`"))
                        dic.Add(p.Name, ExtractListType(p.FieldType));
                    else if (p.FieldType.Name.StartsWith("Dictionary`"))
                        dic.Add(p.Name, ExtractDictionaryType(p.FieldType));
                    else
                        dic.Add(p.Name, p.FieldType.FullName);
                }
            }

            return dic;
        }
        /// <summary>
        /// Get a dictionary from an poco object using reflection only
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertiesToInclude"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetDictionaryReflection(object o, List<string> propertiesToInclude = null, bool allProperties = false)
        {
            var dic = new Dictionary<string, object>();

            var flags = allProperties ? GET_PRIVATE_AND_PUBLIC_PROPERTY_FLAGS : GET_PUBLIC_PROPERTY_FLAGS;

            foreach (var p in o.GetType().GetProperties(flags))
                if ((propertiesToInclude == null) || (propertiesToInclude.Contains(p.Name)))
                    dic.Add(p.Name, p.GetValue(o, new object[0]));

            foreach (var p in o.GetType().GetFields(flags))
                if ((propertiesToInclude == null) || (propertiesToInclude.Contains(p.Name)))
                    dic.Add(p.Name, p.GetValue(o));

            return dic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool PropertyExist(object o, string propertyName)
        {
            try
            {
                object oValue = o.GetType().InvokeMember(propertyName, GET_PUBLIC_PROPERTY_FLAGS, null, o, null);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool MethodExist(object o, string methodName)
        {
            try
            {
                return o.GetType().GetMethod(methodName) != null;
            }
            catch (AmbiguousMatchException)
            {
                return true; // ambiguous means there is more than one result which means: a method with that name does exist
            }
        }

        public static object GetStaticProperty(Type typer, string propertyName, object defaultValue = null, bool isPrivate = false)
        {
            try
            {
                if (isPrivate)
                {
                    object oValue = typer.GetProperty(propertyName).GetValue(null, null);
                    return oValue;
                }
                else
                {
                    object oValue = typer.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
                    return oValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public class EvaluationPathItem
        {
            public string Name;
            public object Value;

            public override string ToString()
            {
                var r = $"{this.Name}:";
                var isNull = Value == null;

                if (Value.GetType().IsClass)
                {
                    r += $"[{Value.GetType().Name}]";
                    if (isNull)
                        r += "null";
                }
                else
                {
                    r += $"{this.Value}";
                }
                return r;
            }
        }


        public class EvaluationPathItems : List<EvaluationPathItem>
        {
            public override string ToString()
            {
                var sb = new StringBuilder();
                foreach(var i in this)
                {
                    sb.Append(i.ToString());
                }
                return sb.ToString();
            }
        }

        public static EvaluationPathItems EvaluatePath(object o, string path)
        {
            var oO = o;
            var r = new EvaluationPathItems();
            var parts = path.Split('.');
            foreach(var part in parts)
            {
                var v = GetProperty(oO, part);
                r.Add(new EvaluationPathItem { Name = part, Value = v });
            }
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetProperty(object o, string propertyName, object defaultValue = null, bool isPrivate = false)
        {
            try
            {
                if (isPrivate)
                {
                    object oValue = o.GetType().InvokeMember(propertyName, GET_PRIVATE_PROPERTY_FLAGS, null, o, null);
                    return oValue;
                }
                else
                {
                    object oValue = o.GetType().InvokeMember(propertyName, GET_PUBLIC_PROPERTY_FLAGS, null, o, null);
                    return oValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object GetPropertyStatic(Type o, string propertyName, object defaultValue = null)
        {
            try
            {
                object oValue = o.InvokeMember(propertyName, GET_PUBLIC_STATIC_PROPERTY_FLAGS, null, null, null);
                return oValue;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        public enum IndexerType
        {
            Get,
            Set
        }
        /// <summary>
        /// Return the name of the indexer function for a get or a set
        /// </summary>
        /// <param name="o">The instance</param>
        /// <param name="indexerType">Define get or set</param>
        /// <returns></returns>
        private static string GetIndexerMethodName(object o, IndexerType indexerType)
        {

            foreach (PropertyInfo property in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetIndexParameters().Length > 0)
                {
                    return string.Format("{0}_{1}", indexerType.ToString().ToLowerInvariant(), property.Name);
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object GetIndexer(object instance, params object[] parameters)
        {
            try
            {
                if (instance == null)
                    throw new ArgumentException("Instance null has not indexer");

                var getIndexerMethod = GetIndexerMethodName(instance, IndexerType.Get);
                if (getIndexerMethod == null)
                    throw new ArgumentException(string.Format("Type:{0} has not indexer", instance.GetType().FullName));

                var v = ReflectionHelper.ExecuteMethod(instance, getIndexerMethod, parameters);
                return v;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object SetIndexer(object instance, params object[] parameters)
        {
            try
            {
                if (instance == null)
                    throw new ArgumentException("Instance null has not indexer");

                var getIndexerMethod = GetIndexerMethodName(instance, IndexerType.Set);
                if (getIndexerMethod == null)
                    throw new ArgumentException(string.Format("Type:{0} has not indexer", instance.GetType().FullName));

                var v = ReflectionHelper.ExecuteMethod(instance, getIndexerMethod, parameters);
                return v;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="strPropertyName"></param>
        /// <param name="Value"></param>
        /// <param name="setIfNull"></param>
        public static void SetProperty(object o, string strPropertyName, object Value, bool setIfNull = false)
        {

            if (o == null) throw new System.ArgumentException("parameter o cannot be null");

            if ((!setIfNull) && (Value == null)) return;

            object[] PropertyValue = new object[1];
            PropertyValue[0] = Value;
            o.GetType().InvokeMember(strPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.IgnoreCase, null, o, PropertyValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteMethod(object instance, string method, params object[] parameters)
        {

            object retValue;

            if (parameters.Length == 0)
                retValue = instance.GetType().InvokeMember(method, CALL_METHOD_FLAGS, null, instance, null);
            else
                retValue = instance.GetType().InvokeMember(method, CALL_METHOD_FLAGS, null, instance, parameters);
            return retValue;
        }
        /// <summary>
        /// Execute a static method
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteStaticMethod(Type type, string method, params object[] parameters)
        {
            object retValue;

            if (parameters.Length == 0)
                retValue = type.InvokeMember(method, CALL_STATIC_METHOD_FLAGS, null, null, null);
            else
                retValue = type.InvokeMember(method, CALL_STATIC_METHOD_FLAGS, null, null, parameters);
            return retValue;
        }
        /// <summary>
        /// Returns the type of a generic List
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetListType(Type type)
        {

            foreach (Type intType in type.GetInterfaces())
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IList<>))
                    return intType.GetGenericArguments()[0];
            return null;
        }
        /// <summary>
        /// Returns the type of the key and of the value for a 
        /// generic Dictionary
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool GetDictionaryType(Type type, out Type keyType, out Type valueType)
        {

            keyType = null;
            valueType = null;

            if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {

                Type[] typeParameters = type.GetGenericArguments();
                keyType = typeParameters[0];
                valueType = typeParameters[1];
                return true;
            }
            else return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsDictionaryOfKV(Type t)
        {

            if (t.IsGenericType)
                return (t.GetGenericTypeDefinition() == typeof(Dictionary<,>));
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsTypeListOfT(Type t)
        {
            if (t.IsGenericType)
                return (t.GetGenericTypeDefinition() == typeof(List<>));
            return false;
        }
   
        public static bool AssertPoco(object sourcePoco, object propertiesPoco, bool throwException = false)
        {
            var sb = new StringBuilder();
            var dic = ReflectionHelper.GetDictionary(propertiesPoco);
            var dicType = DynamicSugar.ReflectionHelper.GetDictionaryWithType(sourcePoco);
            sb.AppendLine($"Assert Object: {sourcePoco.GetType().Name}:");
            var errorCount = 0;
            foreach (var e in dic)
            {
                var expectedValue = e.Value;
                var actualValue = ReflectionHelper.GetProperty(sourcePoco, e.Key);
                var expectedType = dicType[e.Key];

                if (expectedValue is Regex)
                {
                    var expectedRegEx = expectedValue as Regex;
                    if (!expectedRegEx.IsMatch(actualValue.ToString()))
                    {
                        sb.AppendLine($"Property: {e.Key}, expected:{expectedRegEx}, actual:{actualValue}");
                        errorCount += 1;
                    }
                }
                else
                {
                    var isEquals = false;
                    switch(expectedType)
                    {
                        case "Byte": isEquals = (byte)expectedValue == (byte)actualValue; break;
                        case "SByte":  isEquals = (sbyte)expectedValue == (sbyte)actualValue; break;

                        case "short":
                        case "Int16": isEquals = (Int16)expectedValue == (Int16)actualValue; break;

                        case "int": 
                        case "Int32": isEquals = (Int32)expectedValue == (Int32)actualValue; break;

                        case "long":
                        case "Int64": isEquals = (Int64)expectedValue == (Int64)actualValue; break;

                        case "ushort":
                        case "UInt16": isEquals = (UInt16)expectedValue == (UInt16)actualValue; break;

                        case "uint":
                        case "UInt32": isEquals = (UInt32)expectedValue == (UInt32)actualValue; break;

                        case "ulong":
                        case "UInt64": isEquals = (UInt64)expectedValue == (UInt64)actualValue; break;

                        case "Double":
                        case "double": isEquals = (double)expectedValue == (double)actualValue; break;

                        case "Decimal":
                        case "decimal": isEquals = (decimal)expectedValue == (decimal)actualValue; break;

                        case "bool":
                        case "Boolean": isEquals = (bool)expectedValue == (bool)actualValue; break;

                        case "Single":
                        case "Float":
                        case "float": isEquals = (float)expectedValue == (float)actualValue; break;

                        case "DateTime": isEquals = (DateTime)expectedValue == (DateTime)actualValue; break;
                        case "DateTimeOffset": 
                            isEquals = (DateTimeOffset)expectedValue == (DateTimeOffset)actualValue; break;

                        default:  isEquals = expectedValue.ToString() == actualValue.ToString(); break;
                    }
                    if (!isEquals)
                    {
                        sb.AppendLine($"Property: {e.Key}, expected:{expectedValue}, actual:{actualValue}");
                        errorCount += 1;
                    }
                }
            }
            if(throwException)
                throw new ApplicationException(sb.ToString());

            return errorCount == 0;
        }
    }
}


