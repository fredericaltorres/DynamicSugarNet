using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DynamicSugar
{
    public class ManagerTimeOutException : Exception
    {
        public ManagerTimeOutException()
        {
        }
        public ManagerTimeOutException(string text) : base(text)
        {
        }
    }

    public class ManagerRunTimeException : Exception
    {
        public ManagerRunTimeException()
        {
        }
        public ManagerRunTimeException(string text) : base(text)
        {

        }
    }

    public class Managers
    {
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
