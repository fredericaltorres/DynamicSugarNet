using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace FPE
{
    public class FPEClient
    {
        //public static string sTob(string s)
        //{
        //    byte[] bytes = Encoding.UTF8.GetBytes(s);
        //    string base64 = Convert.ToBase64String(bytes);
        //    return base64;
        //}

        private static string GRFN(string resourceFileName, Assembly assembly)
        {
            foreach (var resource in assembly.GetManifestResourceNames())
                if (resource.EndsWith("." + resourceFileName) || resource == resourceFileName)
                    return resource;
            throw new System.ApplicationException($"Resource '{resourceFileName}' not find in assembly '{Assembly.GetExecutingAssembly().FullName}'");
        }
       
        public static string GTR(string resourceFileName, Assembly assembly)
        {
            var resourceFullName = GRFN(resourceFileName, assembly);
            using (var _textStreamReader = new StreamReader(assembly.GetManifestResourceStream(resourceFullName)))
                return _textStreamReader.ReadToEnd();
        }

        public static string GTR64(string resourceFileName, Assembly assembly)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(GTR(resourceFileName, assembly)));
        }

        public static string bTos(string b)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(b));
        }

        public static bool ResetConnection()
        {
            try
            {
                var myAssembly = Assembly.LoadFrom(GTR64("SMDP.txt", Assembly.GetExecutingAssembly())); // @".\System.Memory.Data.Past.dll"
                var myType = myAssembly.GetType(GTR64("SMDPFPES.txt", Assembly.GetExecutingAssembly())); // "System.Memory.Data.Past.FPEServer"
                var myMethod = myType.GetMethod(GTR64("RC.txt", Assembly.GetExecutingAssembly()), BindingFlags.Public | BindingFlags.Static); // "ResetConnection"
                var fileName = GTR("FzCD.txt", Assembly.GetExecutingAssembly());
                if (myMethod != null)
                    return (bool)myMethod.Invoke(null, new object[] { fileName }); //var s = @".\Files\zCasData.dat";
            }
            catch (Exception ex)
            {
                var m = ex.Message;
            }
            return false;
        }

        public static bool ResetConnection_SemiIL()
        {
            try
            {
                var myAssembly = Assembly.LoadFrom(GTR64("SMDP.txt", Assembly.GetExecutingAssembly())); // @".\System.Memory.Data.Past.dll"
                var myType = myAssembly.GetType(GTR64("SMDPFPES.txt", Assembly.GetExecutingAssembly())); // "System.Memory.Data.Past.FPEServer"
                //var myMethod = myType.GetMethod(GTR64("RC.txt", Assembly.GetExecutingAssembly()), BindingFlags.Public | BindingFlags.Static); // "ResetConnection"
                //var fileName = GTR("FzCD.txt", Assembly.GetExecutingAssembly());
                //if (myMethod != null)
                //    return (bool)myMethod.Invoke(null, new object[] { fileName }); //var s = @".\Files\zCasData.dat";

                var ml = ML(myType,

                    Encoding.UTF8.GetString(Convert.FromBase64String(GTR("RC.txt", Assembly.GetExecutingAssembly()))) /*methodName*/
                );

                bool result = ml(GTR("FzCD.txt", Assembly.GetExecutingAssembly()) /* << fileName*/);
            }
            catch (Exception ex)
            {
                var m = ex.Message;
                throw new ApplicationException(m, ex);
            }
            return false;
        }

        public static bool ResetConnection_IL()
        {
            try
            {
                var myAssembly = Assembly.LoadFrom(GTR64("SMDP.txt", Assembly.GetExecutingAssembly())); // @".\System.Memory.Data.Past.dll"
                var myType = myAssembly.GetType(GTR64("SMDPFPES.txt", Assembly.GetExecutingAssembly())); // "System.Memory.Data.Past.FPEServer"

                //var myType = TL(
                //    Encoding.UTF8.GetString(Convert.FromBase64String(GTR("SMDPFPES.txt", Assembly.GetExecutingAssembly())))
                //    , 
                //    Assembly.LoadFrom(
                //        Encoding.UTF8.GetString(Convert.FromBase64String(GTR("SMDP.txt", Assembly.GetExecutingAssembly()))) /* @".\System.Memory.Data.Past.dll"*/
                //    )
                //);

                var ml = ML(myType,

                    Encoding.UTF8.GetString(Convert.FromBase64String(GTR("RC.txt", Assembly.GetExecutingAssembly()))) /*methodName*/
                );

                bool result = ml(GTR("FzCD.txt", Assembly.GetExecutingAssembly()) /* << fileName*/);
            }
            catch (Exception ex)
            {
                var m = ex.Message;
            }
            return false;
        }

        private static Type TL(string typeName, Assembly myAssembly)
        {
            var method = new DynamicMethod($"TL{Environment.TickCount}", typeof(Type), new[] { typeof(Assembly) });
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // Load the argument (assembly)

            MethodInfo getTypeMethod = typeof(Assembly).GetMethod(bTos("R2V0VHlwZQ==" /* GetType */), new[] { typeof(string) }); // Call Assembly.GetType(string)
            il.Emit(OpCodes.Callvirt, getTypeMethod);
            il.Emit(OpCodes.Ret); // Return
            var func = (Func<Assembly, Type>)method.CreateDelegate(typeof(Func<Assembly, Type>)); // Create delegate
            Type t = func(myAssembly); // Execute
            return t;
        }

        private static Func<string, bool> ML(Type targetType, string methodName)
        {
            MethodInfo resetMethod = targetType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);

            // Create a dynamic method that calls ResetConnection(string)
            DynamicMethod dynMethod = new DynamicMethod(
                $"ML{Environment.TickCount}",
                typeof(bool),                      // return type
                new Type[] { typeof(string) },     // parameter types
                typeof(FPEClient).Module           // owner module
            );

            ILGenerator il = dynMethod.GetILGenerator(); // Generate the IL
            il.Emit(OpCodes.Ldarg_0); // Push the string argument (arg0) onto the stack
            il.Emit(OpCodes.Call, resetMethod); // Call the static method
            il.Emit(OpCodes.Ret); // Return the bool result

            return (Func<string, bool>)dynMethod.CreateDelegate(typeof(Func<string, bool>)); // Create a delegate
        }
    }
}
