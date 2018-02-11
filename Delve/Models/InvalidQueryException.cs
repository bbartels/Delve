using System;

namespace Delve.Models
{
    public class InvalidQueryException : Exception
    {
        public InvalidQueryException(string message) : base(message){}
    }
}
