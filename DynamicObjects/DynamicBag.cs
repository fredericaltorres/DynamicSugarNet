using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace DynamicSugar {

    public class DynamicBag : DynamicObject, IDictionary<string, object> {

        public DynamicBag Super = null;

        public Dictionary<string, object> Dictionary = new Dictionary<string, object>();

        private ICollection<KeyValuePair<string, object>> _Collection {
            get{
                ICollection<KeyValuePair<string, object>> c = ((ICollection<KeyValuePair<string, object>>)this.Dictionary);
                return c;
            }
        }        
        public static dynamic Create(DynamicBag super = null){

            DynamicBag b = new DynamicBag(super);
            return b;
        }
        public static DynamicBag Prototype(DynamicBag o){

            DynamicBag oo  = new DynamicBag();
            oo.Dictionary =  ReflectionHelper.CloneDictionary<string, object>(o.Dictionary);
            return oo;
        }      
        public DynamicBag(){

        }
        public DynamicBag(DynamicBag super){

            this.Super = super;
        }
        public virtual object GetMember(string name) {
            object r;
            if (Dictionary.TryGetValue(name, out r))
                return r;
            else {
                if( Super!=null){
                     return Super.GetMember(name);
                }
                else throw new MemberAccessException();
            }
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result) {

            if (Dictionary.TryGetValue(binder.Name, out result)) {

                return true;
            }
            else if( Super!=null){

                return Super.TryGetMember(binder, out result);
            }
            else{
                return false;
            }
        }
        public virtual void SetMember(string name, object value) {

            if (!TrySetMember(name, value))
                throw new MemberAccessException();
        }
        public override bool TrySetMember(SetMemberBinder memberBinder, object value) {

            return TrySetMember(memberBinder.Name, value);
        }
        private bool TrySetMember(string name, object value) {
            try {
                this.Dictionary [name] = value;
                return true;
            }
            catch {
                return false;
            }
        }
        public override bool TryConvert(ConvertBinder binder, out object result) {
            try {
                //result = Generator.GenerateProxy(binder.Type, this);
                result = null;
                return true;
            }
            catch {
                result = null;
                return false;
            }
        }
        void IDictionary<string, object>.Add(string key, object value) {
            this.Dictionary.Add(key, value);
        }
        bool IDictionary<string, object>.ContainsKey(string key) {
            return this.Dictionary.ContainsKey(key);
        }
        ICollection<string> IDictionary<string, object>.Keys {
            get { 
                return this.Dictionary.Keys;
            }
        }
        bool IDictionary<string, object>.Remove(string key) {

            return this.Dictionary.Remove(key);
        }
        bool IDictionary<string, object>.TryGetValue(string key, out object value) {

            return this.Dictionary.TryGetValue(key, out value);
        }
        ICollection<object> IDictionary<string, object>.Values {
            get { 
                return this.Dictionary.Values;
            }
        }

        object IDictionary<string, object>.this [string key] {
            get {
                return this.Dictionary[key];
            }
            set {
                this.Dictionary[key] = value;
            }
        }
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) {

            this._Collection.Add(item);
        }
        void ICollection<KeyValuePair<string, object>>.Clear() {

            this._Collection.Clear();
        }
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) {

            return this._Collection.Contains(item);
        }
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object> [] array, int arrayIndex) {

            this._Collection.CopyTo(array, arrayIndex);
        }
        int ICollection<KeyValuePair<string, object>>.Count {
            get { 
                return this._Collection.Count;
            }
        }
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
            get { 
                return this._Collection.IsReadOnly;
            }
        }
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) {

            return this._Collection.Remove(item);
        }
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {

            return this._Collection.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {

            return this._Collection.GetEnumerator();
        }
    }
}
