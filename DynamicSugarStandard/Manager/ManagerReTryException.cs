using System;

namespace DynamicSugar
{
    public class ManagerReTryException : Exception
    {
        public ManagerReTryException()
        {
        }
        public ManagerReTryException(string text) : base(text)
        {

        }
    }
}
