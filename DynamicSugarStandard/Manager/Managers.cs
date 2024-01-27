using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DynamicSugar
{
    public class Managers
    {
        public static T Retry<T>(
            Func<T> callBack,
            int maxRetry = 2,
            double sleepTimeInMinute = 1,
            bool throwException = false,
            Action<Exception> onException = null)
        {
            for (var x = 0; x <= maxRetry; x++)
            {
                try
                {
                    var r = callBack();
                    return r;
                }
                catch (Exception ex)
                {
                    if(onException != null)
                        onException(ex);
                    Thread.Sleep(((int)(sleepTimeInMinute*60)) * 1000);
                }
            }
            if(throwException)
                throw new ManagerReTryException($"[RETRY] Failed after {maxRetry} retries");  

            return default(T);
        }

        public static void Wait(int second)
        {
            Thread.Sleep(second * 1000);
        }

        private static string GetDuration(DateTime startWaitingTime)
        {
            TimeSpan ts = DateTime.Now - startWaitingTime;
            return $"{ts.TotalSeconds:00}s";
        }

        public static bool TimeOutManager(string message, double minuteTimeOut, Func<bool> callBack, int sleepDuration = 10, bool throwError = true)
        {
            var startWaitingTime = DateTime.Now;
            var actualSleepDuration = sleepDuration / 2; // The first time we wait half the time
            while (true)
            {
                Wait(actualSleepDuration); // The first time we wait half the time
                actualSleepDuration = sleepDuration;
                if (DateTime.Now.Subtract(startWaitingTime).Seconds > ((int)(minuteTimeOut * 60)))
                {
                    if (throwError)
                        throw new ManagerTimeOutException($"[TIMEOUT]{message} timed out, duration: {GetDuration(startWaitingTime)}");
                    return false;
                }
                try
                {
                    if (callBack())
                        return true;
                }
                catch (Exception ex)
                {
                    if (throwError)
                        throw new ManagerRunTimeException($"aborting - error:{ex.Message}, duration: {GetDuration(startWaitingTime)}");
                    return false;
                }
            }
        }
    }
}
