using System;

namespace MobileDB.Exceptions
{
    public class EntityValidationException : Exception
    {
        public EntityValidationException(string message)
            : base(message)
        {
        }
    }
}