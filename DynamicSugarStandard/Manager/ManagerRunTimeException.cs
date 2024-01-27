using System;

namespace DynamicSugar
{
    public class ManagerRunTimeException : Exception
    {
        public ManagerRunTimeException()
        {
        }
        public ManagerRunTimeException(string text) : base(text)
        {

        }
    }
}
