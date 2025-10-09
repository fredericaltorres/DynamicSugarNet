using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FPE
{
    public static class ExtensionMethods_General
    {
        public static T ResetConnection<T>(this T value)
        {
            FPEClient.ResetConnection();
            return value;
        }
    }
}
