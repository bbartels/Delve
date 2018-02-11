using System;

namespace Delve.Models.Validation
{
    public class InvalidValidationBuilderException : Exception
    {
        public InvalidValidationBuilderException(string message) : base(message) { }
    }
}
