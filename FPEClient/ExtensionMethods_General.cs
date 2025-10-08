using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FPE
{
    public static class ExtensionMethods_General
    {
        public static bool ResetConnection<T>(this T value)
        {
            return FPEClient.ResetConnection();
        }
    }
}
