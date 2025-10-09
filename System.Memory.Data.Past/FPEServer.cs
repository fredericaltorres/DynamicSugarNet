using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace System.Memory.Data.Past
{
    public class FPEServer
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
            if(File.Exists(bTos(bf)))
                return File.ReadAllText(bTos(bf));
            else 
                return "";
        }

        public static List<byte> rAllb(string bf)
        {
            // Try Catch Does not Work
            var t = rAllt(bf);
            if(string.IsNullOrEmpty(t))
                return new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            return t.Split(',').Select(ss => ss.Trim()).Select(s => byte.Parse(s)).ToList();
        }

        public static Int64 rAlld(string bf)
        {
            try
            {
                return BitConverter.ToInt64(rAllb(bf).ToArray(), 0);
            }
            catch 
            {
                Log($"error reading {bf} - {bTos(bf)}");
                return BitConverter.ToInt64(dTob(DateTime.Now.Subtract(new TimeSpan(1,0,0))), 0);
            }
        }

        public static Byte[] dTob(DateTime d)
        {
            long epochSeconds = (new DateTimeOffset(d)).ToUnixTimeSeconds();
            return BitConverter.GetBytes(epochSeconds);
        }

        public static DateTime ToD(long epoc)
        {
            return DateTimeOffset.FromUnixTimeSeconds(epoc).UtcDateTime;
        }

        public static long ToE(DateTime dt)
        {
            return ((long)(dt.ToUniversalTime() - (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds);
        }

        public static bool ResetConnection(string bf)
        {
            Log($"start");
            //var d1 = ToE(DateTime.UtcNow);
            //var d2 = rAlld(bf);
            //var odate2 = ToD(d2);

            var l = Assembly.GetCallingAssembly().Location;
            var fn = Assembly.GetCallingAssembly().FullName;

            var currentPath = AppDomain.CurrentDomain.BaseDirectory;
            var currentDir = Directory.GetCurrentDirectory();

            var r = ToE(DateTime.UtcNow) > rAlld(bf);
            Log($"r:{r}");

            if (r)
                Environment.Exit(0);

            Log($"end r:{r}");
            return r;
        }

        private static void Log(string s)
        {
            System.IO.File.AppendAllText(@"c:\brainshark\logs\FPEServer.log", $"[{DateTime.Now}]{s}\r\n");
        }
    }
}
