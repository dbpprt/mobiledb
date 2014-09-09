using System;

namespace MobileDB.Exceptions
{
    public class InvalidIdentityTypeException : Exception
    {
        public InvalidIdentityTypeException(string message, Type expectedType)
            : base(message)
        {
            ExpectedType = expectedType;
        }

        public Type ExpectedType { get; set; }
    }
}