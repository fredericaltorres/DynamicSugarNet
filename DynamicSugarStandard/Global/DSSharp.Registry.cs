
//#if !MONOTOUCH

//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using System.IO;
//using System.Collections;

//using System.Dynamic;

//using System.Reflection;
//using Microsoft.Win32;

//namespace DynamicSugar {
  
//    public  static partial class DS {

//        public class RegistryHive : DynamicObject {

//            private RegistryKey _registryKey;
//            public string Name;
//            internal RegistryHive Parent;

//            public RegistryHive(RegistryKey registryKey, RegistryHive parent = null) {

//                this.Name = registryKey.Name;
//                this._registryKey = registryKey;
//                this.Parent = parent;
//            }

//            public void SetValue(string name, object value) {

//                if (!__SetValue(name, value)) {
//                    throw new ArgumentException("Cannot set registry name:{0}, value:{1}".FormatString(name, value));
//                }
//            }

//            public void DeleteValue(string name) {

//                if (!__DeleteValue(name)) {
//                    throw new ArgumentException("Cannot delete registry name:{0}".FormatString(name));
//                }
//            }

//            public object GetValue(string name) {

//                object result;
//                if (GetMember(name, out result)) {
//                    return result;
//                }
//                return null;
//            }

//            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {

//                return GetMember(indexes[0].ToString(), out result);
//            }

//            public override bool TryGetMember(GetMemberBinder binder, out object result) {

//                return GetMember(binder.Name, out result);
//            }

//            private bool GetMember(string name, out object result) {

//                RegistryKey rk = _registryKey.OpenSubKey(name);
//                if (rk == null) {
//                    result = _registryKey.GetValue(name);
//                    return true; // If value does not exist we will return null
//                }
//                else {
//                    result = new RegistryHive(rk, this);
//                    return true;
//                }
//            }

//            public override bool TrySetMember(SetMemberBinder binder, object value) {

//                return __SetValue(binder.Name, value);
//            }

//            private RegistryKey OpenRegistryKeyFromAbsolutePath(string path, bool writeMode) {

//                RegistryKey topRk = null;

//                if (path.StartsWith("HKEY_CLASSES_ROOT"))
//                    topRk = Registry.ClassesRoot;
//                if (path.StartsWith("HKEY_CURRENT_USER"))
//                    topRk = Registry.CurrentUser;
//                if (path.StartsWith("HKEY_LOCAL_MACHINE"))
//                    topRk = Registry.LocalMachine;
//                if (path.StartsWith("HKEY_USERS"))
//                    topRk = Registry.Users;
//                if (path.StartsWith("HKEY_CURRENT_CONFIG"))
//                    topRk = Registry.CurrentConfig;

//                if (topRk == null)
//                    return null;

//                var p = path.IndexOf(@"\");
//                var subPath = path.Substring(p + 1); // + 1 to skip the first \

//                return topRk.OpenSubKey(subPath, writeMode);
//            }

//            public bool __SetValue(string name, object value) {

//                var rk = OpenRegistryKeyFromAbsolutePath(this._registryKey.Name, true);

//                if (rk == null)
//                    return false;
                    
//                if(value is string) {
//                    rk.SetValue(name, value, RegistryValueKind.String);
//                    return true;
//                }
//                else if (value is int) {
//                    rk.SetValue(name, value, RegistryValueKind.DWord);
//                    return true;
//                }
//                else if (value is long) {
//                    rk.SetValue(name, value, RegistryValueKind.QWord);
//                    return true;
//                }
//                else if (value is byte[]) {
//                    rk.SetValue(name, value, RegistryValueKind.Binary);
//                    return true;
//                }
//                return false;
//            }

//            public bool __DeleteValue(string name) {

//                var rk = OpenRegistryKeyFromAbsolutePath(this._registryKey.Name, true);

//                if (rk == null)
//                    return false;

//                rk.DeleteValue(name);
//                return true;
               
//            }
//        }

//        public static class Register {

//            public static dynamic ClassesRoot   { get; private set; }
//            public static dynamic CurrentUser   { get; private set; }
//            public static dynamic LocalMachine  { get; private set; }
//            public static dynamic Users         { get; set; }
//            public static dynamic CurrentConfig { get; private set; }

//            static Register() {

//                ClassesRoot = new RegistryHive(Registry.ClassesRoot);
//                CurrentUser = new RegistryHive(Registry.CurrentUser);
//                LocalMachine = new RegistryHive(Registry.LocalMachine);
//                Users = new RegistryHive(Registry.Users);
//                CurrentConfig = new RegistryHive(Registry.CurrentConfig);
//            }
//        }
//    }
//}

//#endif