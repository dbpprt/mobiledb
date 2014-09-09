using System;

namespace MobileDB.Exceptions
{
    public class EntityTypeException : Exception
    {
        public EntityTypeException(string message, string type)
            : base(message)
        {
            Type = type;
        }

        public string Type { get; private set; }
    }
}