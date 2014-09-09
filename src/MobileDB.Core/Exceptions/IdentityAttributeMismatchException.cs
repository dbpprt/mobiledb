using System;

namespace MobileDB.Exceptions
{
    public class IdentityAttributeMismatchException : Exception
    {
        public IdentityAttributeMismatchException(string message)
            : base(message)
        {
        }
    }
}