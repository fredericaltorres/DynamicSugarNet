using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;


#if !MONOTOUCH
using System.Dynamic;

namespace DynamicSugar {

    /// <summary>
    /// Dynamic Sharp Exception
    /// </summary>
    public class MultiValuesException : System.Exception {

        public MultiValuesException(string message) : base(message) { }
    }
    /// <summary>
    /// When a MultiValues is created and initialized via the DynamicObject api
    /// the MultiValues instance is returned, creating a chaining effect.
    /// When use as a Dictionary we must return the instance as a Dictionary
    /// else as a DynamicObject.
    /// </summary>
    public enum MultiValuesBehavior {
        Bag,
        Dictionary,
        Undefined
    }
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.trygetindex.aspx
    /// </summary>
    public class MultiValues : DynamicObject, IDictionary<string, object> {
        
        private const string EXCEPTION_MESSAGE__PROPERTY_NOT_DEFINED = "The property '{0}' is not defined in the MultiValues dictionary";
        private const string EXCEPTION_MESSAGE__BEHAVIOR_NOT_DEFINED = "Behavior not defined in MultiValues instance, defining Dictionary:{0}";
        /// <summary>
        /// Internal Dictionary object
        /// </summary>
        private Dictionary<string, object>  _dictionary = null;

        internal MultiValuesBehavior Behavior = MultiValuesBehavior.Undefined;

        /// <summary>
        /// Return the instance as the internal Dictionary<string, object>
        /// The method is named so when used it indicate clearly that it 
        /// return this instance as a Dictionary
        /// </summary>
        public Dictionary<string, object> AsDictionary{
            get{
                return this.Dictionary;
            }
        }
        /// <summary>
        /// Return the internal Dictionary instance
        /// </summary>
        public Dictionary<string, object> Dictionary{
            get{
                if(_dictionary==null)
                    _dictionary = new Dictionary<string,object>();
                return _dictionary;
            }
        }
        /// <summary>
        /// Return a new MultiValues
        /// </summary>
        /// <returns></returns>
        public static MultiValues Create(MultiValuesBehavior behavior){
            var m      = new MultiValues();
            m.Behavior = behavior;
            return m;
        }
        /// <summary>
        /// Static helper to create MultiValues instance and populate it with member from
        /// an anonymous type.
        /// </summary>
        /// <param name="anonymousInstance"></param>
        /// <returns></returns>
        public static MultiValues Values(object anonymousInstance){
            
            return new MultiValues().MoreValues(anonymousInstance);
        }
        /// <summary>
        /// Cannot be instanciated by the developer
        /// </summary>
        private MultiValues(){

        }
        /// <summary>
        /// Add more values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private MultiValues MoreValues(object value) {           

            if(value is IDictionary<string, object>){                
                foreach(KeyValuePair<string, object> v in (value as IDictionary<string, object>))
                    this.Dictionary.Add(v.Key, v.Value);
            }
            else{            
                foreach(KeyValuePair<string, object> v in ReflectionHelper.GetDictionary(value))
                    this.Dictionary.Add(v.Key, v.Value);
            }
            return this;
        }
        /// <summary>
        /// When calling dynamic property on the instance of this class
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            
            result = null;

            if(this.Dictionary.ContainsKey(binder.Name)){

                result = this.Dictionary[binder.Name];
                return true;
            }
            else throw new MultiValuesException(EXCEPTION_MESSAGE__PROPERTY_NOT_DEFINED.FormatString(binder.Name));
        }

        /// <summary>
        /// This is where it's get weird, when any dynamic/aka nonexisting method 
        /// is called on this object, this method is called
        /// http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.tryinvoke.aspx
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            SetDictionaryFromParameters(binder.CallInfo, args);

            result = null;
            
            if(this.Behavior==MultiValuesBehavior.Dictionary)
                result = this.Dictionary;
            else if(this.Behavior==MultiValuesBehavior.Bag)
                result = this;
            else if(this.Behavior==MultiValuesBehavior.Undefined)
                throw new MultiValuesException(EXCEPTION_MESSAGE__BEHAVIOR_NOT_DEFINED.FormatString(this.Dictionary.Format()));
            
            return true;
        }
        /// <summary>
        /// Populate a dictionary with the parameters passed to a dynamic method
        /// this support the C# 4.0 optional parameters
        /// </summary>
        /// <param name="callInfo"></param>
        /// <param name="args"></param>
        private void SetDictionaryFromParameters(CallInfo callInfo, object [] args) {

            this.Dictionary.Clear();
            for (int i = 0; i < args.Length; i++) {
                if (callInfo.ArgumentCount > 0)
                    this.Dictionary.Add(callInfo.ArgumentNames [i], args [i]);
            }
        }       
        /// <summary>        
        /// Populate a dictionary with the parameters passed to a [ ] invoquation
        /// this support the C# 4.0 optional parameters
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result){

            result   = null;
            var name = indexes[0].ToString();
            if(this.Dictionary.ContainsKey(name)){

                result = this.Dictionary[name];
                return true;
            }
            else throw new MultiValuesException(EXCEPTION_MESSAGE__PROPERTY_NOT_DEFINED.FormatString(name));

            //SetDictionaryFromParameters(binder.CallInfo, indexes);
            //result = this;
            //return true;
        }
        /// <summary>
        /// Support the following 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {            
            this.Dictionary[indexes[0].ToString()] = value;
            return true;
        }
        #region IDictionary Interface Implementation

        void IDictionary<string, object>.Add(string key, object value) {
            this.Dictionary.Add(key, value);
        }
        bool IDictionary<string, object>.ContainsKey(string key) {
            return this.Dictionary.ContainsKey(key);
        }

        ICollection<string> IDictionary<string, object>.Keys {
            get { return this.Dictionary.Keys; }
        }

        bool IDictionary<string, object>.Remove(string key) {
            return this.Dictionary.Remove(key);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value) {
            return this.Dictionary.TryGetValue(key, out value);
        }

        ICollection<object> IDictionary<string, object>.Values {
            get { return this.Dictionary.Values; }
        }

        object IDictionary<string, object>.this [string key] {
            get {
                return this.Dictionary[key];
            }
            set {
                this.Dictionary[key] = value;
            }
        }
        #endregion
        #region ICollection Interface Implementation

        private ICollection<KeyValuePair<string, object>> GetICollection(){
            
            return this._dictionary as ICollection<KeyValuePair<string, object>>;            
        }
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) {            
            this.GetICollection().Add(item);
        }
        void ICollection<KeyValuePair<string, object>>.Clear() {
            this.GetICollection().Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) {
            return this.GetICollection().Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object> [] array, int arrayIndex) {
            this.GetICollection().CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, object>>.Count {
            get{
                return this.GetICollection().Count;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
            get { return this.GetICollection().IsReadOnly; }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) {
            return this.GetICollection().Remove(item);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            return this.GetICollection().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetICollection().GetEnumerator();
        }
        #endregion

    }   
}
#endif