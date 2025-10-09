using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Reflection.FPE
{
    public static class ExtensionMethods_General
    {
        public static int ResetConnection<T>(this T value)
        {
            return FPEClient.ResetConnection();
        }
    }
}
