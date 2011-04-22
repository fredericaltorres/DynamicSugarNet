using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Linq.Expressions;

namespace DynamicSugarSharp {

    /// <summary>
    /// Dynamic Sharp Exception
    /// </summary>
    public class DynaExperimentException : System.Exception {

        public DynaExperimentException(string message) : base(message) { }
    }
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.trygetindex.aspx
    /// </summary>
    public class DynaExperiment : DynamicObject, IDictionary<string, object> {

        Dictionary<string, object> _dictionary = new Dictionary<string,object>();

        Stack<string> _actions = new Stack<string>();

        public IDictionary<string, object> Dictionary{
            get{
                return this._dictionary;
            }
        }
        /// <summary>
        /// Static helper to use
        /// </summary>
        /// <param name="anonymousInstance"></param>
        /// <returns></returns>
        public static dynamic Create(object anonymousInstance){
            
            return new DynaExperiment().MoreValues(anonymousInstance);
        }
        public static dynamic New{
            get{
                return new DynaExperiment();
            }
        }
        /// <summary>
        /// Cannot be instanciated by the developer
        /// </summary>
        private DynaExperiment(){

        }
        public DynaExperiment MoreValues(object value) {

            if(this._dictionary==null) this._dictionary = new Dictionary<string,object>();
            
            foreach(KeyValuePair<string, object> v in ReflectionHelper.GetProperties(value)){

                this._dictionary.Add(v.Key, v.Value);
            }
            return this;
        }      
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if(this._dictionary.ContainsKey(binder.Name))
                this._dictionary.Remove(binder.Name);

            this._dictionary[binder.Name] = value;
            return true;
        }  
        public override bool TryGetMember(GetMemberBinder binder, out object result) {

            result = null;

            if(this._dictionary.ContainsKey(binder.Name)){

                result = this._dictionary[binder.Name];
                return true;
            }
            else {                
                _actions.Push(binder.Name);
                result = this;
                return true;
            }
        }
        // http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.tryinvoke.aspx
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {            
            var a  = binder.CallInfo;
            this._dictionary.Clear();

            for (int i = 0; i < args.Length; i++)
            {
                if(binder.CallInfo.ArgumentCount>0)
                    this._dictionary.Add(binder.CallInfo.ArgumentNames[i], args[i]);
            }
            result = this;
            return true;
        }
        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result){
             
            this._dictionary.Add(this._actions.Peek(), indexes[0]);
            result = this;
            return true;
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return true;
            //base.TrySetIndex(binder, indexes, value);
        }
                
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            var d1                       = this._dictionary;
            var d2                       = ((DynaExperiment)arg).Dictionary;
            result                       = null;
            Dictionary<string,object> d3 = null;

            switch (binder.Operation)
            {
                case ExpressionType.Add:
                    d3     = ReflectionHelper.MergeDictionary(d1,d2);
                    result = DynaExperiment.Create(d3);
                    break;
                case ExpressionType.Subtract:
                    break;
                default:
                    Console.WriteLine(binder.Operation +": This binary operation is not implemented");
                    return false;
            }
            return true;
        }
        public dynamic Assign{
            get{
                return this;
            }
        }

        void IDictionary<string, object>.Add(string key, object value) {
            this._dictionary.Add(key, value);
        }
        bool IDictionary<string, object>.ContainsKey(string key) {
            return this._dictionary.ContainsKey(key);
        }

        ICollection<string> IDictionary<string, object>.Keys {
            get { return this._dictionary.Keys; }
        }

        bool IDictionary<string, object>.Remove(string key) {
            return this._dictionary.Remove(key);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value) {
            return this._dictionary.TryGetValue(key, out value);
        }

        ICollection<object> IDictionary<string, object>.Values {
            get { return this._dictionary.Values; }
        }

        object IDictionary<string, object>.this [string key] {
            get {
                return this._dictionary[key];
            }
            set {
                this._dictionary[key] = value;
            }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, object>>.Clear() {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object> [] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        int ICollection<KeyValuePair<string, object>>.Count {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }   
}
