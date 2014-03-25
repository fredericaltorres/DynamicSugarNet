using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DynamicSugar {

     public class ParameterMetadata {

        public bool     IsIn;
        public bool     IsOptional;
        public bool     IsOut;
        public bool     IsRetval;
        public string   Name;
        public Type     ParameterType;
        public int      Position;
        public object   RawDefaultValue;
        public object   Value;

        public ParameterMetadata(ParameterInfo pi, object value){

            this.IsIn            = pi.IsIn;
            this.IsOptional      = pi.IsOptional;
            this.IsOut           = pi.IsOut;
            this.IsRetval        = pi.IsRetval;
            this.Name            = pi.Name;
            this.ParameterType   = pi.ParameterType;
            this.Position        = pi.Position;
            this.RawDefaultValue = pi.RawDefaultValue;
            this.Value           = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReflectionHelper {

        const BindingFlags GET_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty;
        const BindingFlags GET_FLAGS_STATIC = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty;
        const BindingFlags CALL_METHOD_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase;
        const BindingFlags CALL_STATIC_METHOD_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase;
        
        public static string GetStackCall() {
            try {
                System.Reflection.MethodBase method = null;
                System.Text.StringBuilder B = new StringBuilder();
                int i = 1;
                int max = (new System.Diagnostics.StackTrace(true)).FrameCount;
                while (i < max) {

                    method = new System.Diagnostics.StackTrace(true).GetFrame(i).GetMethod();

                    if (method == null)
                        break;
                    else {
                        if (
                            (method.Name.ToLower() != "trace")          &&
                            (method.Name.ToLower() != "trace_error")    &&
                            (method.Name.ToLower() != "trace_method")
                        ) {
                            B.AppendFormat("{1}.{0} -> ", method.Name, method.ReflectedType.FullName);
                        }
                    }
                    i++;
                }
                return B.ToString();
            }
            catch {
                return null;
            }
        }
        public static System.Reflection.MethodBase GetCallingMethod(params string[] methodToIgnore) {

            return GetCallingMethod(methodToIgnore.ToList());
        }
        public static System.Reflection.MethodBase GetCallingMethod(List<string> methodToIgnore = null) {
            try {
                if(methodToIgnore==null)
                    methodToIgnore = new List<string>();

                methodToIgnore.Add("GetCallingMethod");// Alway ignore the current method                

                MethodBase method = null;
                int i             = 1;
                int max           = (new System.Diagnostics.StackTrace(true)).FrameCount;

                while (i <= max) {

                    method = new System.Diagnostics.StackTrace(true).GetFrame(i).GetMethod();

                    if(!method.Name.In(methodToIgnore))
                        break;
                    i++;
                }
                return method;
            }
            catch {
                return null;
            }
        }
        /// <summary>
        /// Create new instance of the type
        /// </summary>
        /// <param name="typeToInstantiate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object Constructor(Type typeToInstantiate, params object[] parameters) {

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
        public static T Constructor<T>(Type typeToInstantiate, params object[] parameters) {

            object o = Activator.CreateInstance(typeToInstantiate, parameters);
            T t = (T)Convert.ChangeType(o, typeof(T));
            return t;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static Dictionary<string, Object> GetLocals(params object[] parameterValues){

            var dic = new Dictionary<string,Object>();
            var m   = GetCallingMethod("GetLocals");
            var i   = 0;
            foreach(var mp in m.GetParameters())
                dic.Add(mp.Name, parameterValues[i++]);
            return dic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static Dictionary<string, ParameterMetadata> GetLocalsEx(params object[] parameterValues){

            Dictionary<string, ParameterMetadata> dic = new Dictionary<string,ParameterMetadata>();
            MethodBase m                              = GetCallingMethod(DS.List("GetLocalsEx"));
            int i                                     = 0;
            foreach(var mp in m.GetParameters())
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
        public static Dictionary<K,V> CloneDictionary<K,V>(IDictionary<K,V> dic){

            Dictionary<K,V> d = new Dictionary<K,V>();
            foreach(KeyValuePair<K,V> v in dic)
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
        public static Dictionary<K,V> MergeDictionary<K,V>(IDictionary<K,V> dic1, IDictionary<K,V> dic2, bool overwrite = false){

            Dictionary<K,V> d = CloneDictionary(dic1);
            foreach(KeyValuePair<K,V> v in dic2)
                if(overwrite){
                    if(d.ContainsKey(v.Key)) 
                        d.Remove(v.Key);
                    d.Add(v.Key, v.Value);
                }
                else{
                    if(!d.ContainsKey(v.Key))                        
                        d.Add(v.Key, v.Value);
                }
            return d;
        } 

        #if USE_FAST_REFLECTION
            private static Dictionary<string, List<string>> __TypeDefined = new Dictionary<string, List<string>>();
        #endif
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Dictionary<string,object> GetDictionary(object o, List<string> propertiesToInclude = null) {

            var dic = new Dictionary<string,object>();

            // Support Dictionary and ExpandoObect
            if(o is IDictionary<string, object>) {

                // We have to make a copy, because returning an ExpandoObject as an IDictionary, do not support the dic["name"] syntax.
                foreach(KeyValuePair<string, object> v in ((IDictionary<string, object>)o))
                    if((propertiesToInclude==null)||(propertiesToInclude.Contains(v.Key)))
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
                dic = DynamicSugar.ReflectionHelper.GetDictionaryReflection(o, propertiesToInclude);
            #endif
            return dic;            
        }
        /// <summary>
        /// Get a dictionary from an poco object using reflection only
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertiesToInclude"></param>
        /// <returns></returns>
        private static Dictionary<string,object> GetDictionaryReflection(object o, List<string> propertiesToInclude = null) {

            var dic = new Dictionary<string,object>();

            foreach(var p in o.GetType().GetProperties(GET_FLAGS))
                if((propertiesToInclude==null)||(propertiesToInclude.Contains(p.Name)))
                    dic.Add( p.Name, p.GetValue(o, new object[0]) );

            foreach(var p in o.GetType().GetFields(GET_FLAGS))
                if((propertiesToInclude==null)||(propertiesToInclude.Contains(p.Name)))
                    dic.Add( p.Name, p.GetValue(o) );

            return dic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool PropertyExist(object o, string propertyName) {
            try {
                object oValue = o.GetType().InvokeMember(propertyName, GET_FLAGS, null, o, null);
                return true;
            }
            catch {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetProperty(object o, string propertyName, object defaultValue = null) {
            try {
                object oValue = o.GetType().InvokeMember(propertyName, GET_FLAGS, null, o, null);
                return oValue;
            }
            catch {
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
                object oValue = o.InvokeMember(propertyName, GET_FLAGS_STATIC, null, null, null);
                return oValue;
            }
            catch(System.Exception ex)
            {
                throw;
            }
        }
        public enum IndexerType {
            Get,
            Set
        }
        /// <summary>
        /// Return the name of the indexer function for a get or a set
        /// </summary>
        /// <param name="o">The instance</param>
        /// <param name="indexerType">Define get or set</param>
        /// <returns></returns>
        private static string GetIndexerMethodName(object o, IndexerType indexerType) {
            
            foreach (PropertyInfo property in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
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
                if(instance == null)
                    throw new ArgumentException("Instance null has not indexer");

                var getIndexerMethod = GetIndexerMethodName(instance, IndexerType.Get);
                if(getIndexerMethod == null)
                    throw new ArgumentException(string.Format("Type:{0} has not indexer", instance.GetType().FullName));

                var v = ReflectionHelper.ExecuteMethod(instance, getIndexerMethod, parameters);
                return v;
            }
            catch(System.Exception ex)
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
                if(instance == null)
                    throw new ArgumentException("Instance null has not indexer");

                var getIndexerMethod = GetIndexerMethodName(instance, IndexerType.Set);
                if(getIndexerMethod == null)
                    throw new ArgumentException(string.Format("Type:{0} has not indexer", instance.GetType().FullName));

                var v = ReflectionHelper.ExecuteMethod(instance, getIndexerMethod, parameters);
                return v;
            }
            catch(System.Exception ex)
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
        public static void SetProperty(object o, string strPropertyName, object Value, bool setIfNull = false){

            if (o == null)throw new System.ArgumentException("parameter o cannot be null");

            if ((!setIfNull) && (Value == null)) return;

            object[] PropertyValue = new object[1];
            PropertyValue[0]       = Value;
            o.GetType().InvokeMember(strPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.IgnoreCase, null, o, PropertyValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteMethod(object instance, string method, params object[] parameters) {

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
        public static object ExecuteStaticMethod(Type type, string method, params object[] parameters) {
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
        public static Type GetListType(Type type) {

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
        public static bool GetDictionaryType(Type type, out Type keyType, out Type valueType) {

            keyType   = null;
            valueType = null;

            if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {

                Type[] typeParameters = type.GetGenericArguments();
                keyType               =  typeParameters[0];
                valueType             =  typeParameters[1];                
                return true;
            }
            else return false;          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsDictionaryOfKV(Type t){

            if(t.IsGenericType)
                return (t.GetGenericTypeDefinition() == typeof(Dictionary<,>));
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsTypeListOfT(Type t){
            if(t.IsGenericType)
                return (t.GetGenericTypeDefinition() == typeof(List<>));
            return false;
        }
    }
}


