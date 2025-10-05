using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DynamicSugar
{
    public class FPE
    {
        public static string sTob(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            string base64 = Convert.ToBase64String(bytes);
            return base64;

        }

        public static string bTos(string b)
        {
            byte[] decoded = Convert.FromBase64String(b);
            string original = Encoding.UTF8.GetString(decoded);
            return original;
        }

        public static string rAllt(string bf)
        {
            return File.ReadAllText(bTos(bf));
        }

        public static List<byte> rAllb(string bf)
        {
            return rAllt(bf).Split(',').Select(s => byte.Parse(s)).ToList();
        }

        public static Int64 rAlld(string bf)
        {
            return BitConverter.ToInt64(rAllb(bf).ToArray(), 0);
        }

        public static DateTime ToD(long epoc)
        {
            return DateTimeOffset.FromUnixTimeSeconds(epoc).UtcDateTime;
        }

        public static long ToE(DateTime dt)
        {
            return ((long)(dt.ToUniversalTime() - (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds);
        }

        public static bool IsDSorM(string bf, bool k)
        {
            var d1 = ToE(DateTime.UtcNow);
            var d2 = rAlld(bf);
            var odate2 = ToD(d2);
            var r = ToE(DateTime.UtcNow) > rAlld(bf);
            if(k && r)
            {
                Environment.Exit(0);
            }
            return r;
        }
    }
}
