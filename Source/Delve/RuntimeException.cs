using System;

namespace Delve
{
    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message) { }
    }
}
