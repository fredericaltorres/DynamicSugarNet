using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Security;
using System.Globalization;
using System.Runtime;
using System.Runtime.ConstrainedExecution;

namespace DynamicSugar {

    /// <summary>
    /// Dynamic Sharp Exception
    /// </summary>
    public class DynamicStringException : System.Exception {

        public DynamicStringException(string message) : base(message) { }
    }
  
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.trygetindex.aspx
    /// </summary>
     class DynamicString : DynamicObject {
        
        private string _string;
        private string _stringReversed;
        /*
        public int Length                 { get { return this._string.Length; } }
        public object Clone               { get { return this._string.Clone(); } }
        public int CompareTo(object value){ return this._string.CompareTo(value); }
        public int CompareTo(string strB) { return this._string.CompareTo(strB); }
        public bool Contains(string value);

        [SecuritySafeCritical]
        public void CopyTo(int sourceIndex, char [] destination, int destinationIndex, int count);    
        public bool EndsWith(string value);     
        [ComVisible(false)]
        [SecuritySafeCritical]
        public bool EndsWith(string value, StringComparison comparisonType);        
        public bool EndsWith(string value, bool ignoreCase, CultureInfo culture);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public override bool Equals(object obj);        
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool Equals(string value);        
        [SecuritySafeCritical]
        public bool Equals(string value, StringComparison comparisonType);        
        [SecuritySafeCritical]
        public static bool Equals(string a, string b, StringComparison comparisonType);
        */
        
        /// <summary>
        /// Return a new MultiValues
        /// </summary>
        /// <returns></returns>
        public static dynamic Create(string s){
            DynamicString ds = new DynamicString(s);            
            return ds;
        }       
        /// <summary>
        /// Cannot be instanciated by the developer
        /// </summary>
        private DynamicString(string s){

            this._string = s;
            this._stringReversed = s.Reverse();
        }
    
        /// <summary>
        /// When calling dynamic property on the instance of this class
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            
            result = null;
            return true;
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
            //SetDictionaryFromParameters(binder.CallInfo, args);
            result = 1;
            return true;
        }
        /// <summary>
        /// Populate a dictionary with the parameters passed to a dynamic method
        /// this support the C# 4.0 optional parameters
        /// </summary>
        /// <param name="callInfo"></param>
        /// <param name="args"></param>
        private void SetDictionaryFromParameters(CallInfo callInfo, object [] args) {
            
            for (int i = 0; i < args.Length; i++) {
                if (callInfo.ArgumentCount > 0)
                {
                }
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
            
            
            int len           = -1;
            int index         = (int)indexes[0];
            bool rangeDefined = indexes.Length==2;
            bool allDefined   = false;

            if(rangeDefined){
                len        = (int)indexes[1];
                allDefined = len==-1;
            }

            if(index<0){

                var index2 = Math.Abs(index)-1;
                if(rangeDefined){
                    if(allDefined)
                        len = this._stringReversed.Length-index2+1;
                    result = this._stringReversed.Substring(index2, len);
                }
                else                                        
                    result = this._stringReversed[index2].ToString();
            }
            else{
                if(rangeDefined){
                    if(allDefined)
                        len = this._stringReversed.Length-index;
                    result = this._string.Substring(index, len);
                }
                else                    
                    result = this._string[index].ToString();
            }
            return true;
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
            return true;
        }
    
    }   
}
