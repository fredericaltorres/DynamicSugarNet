using System;

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
}
