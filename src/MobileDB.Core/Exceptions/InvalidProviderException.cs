using System;

namespace MobileDB.Exceptions
{
    public class InvalidProviderException : Exception
    {
        public InvalidProviderException(string message, string provider)
            : base(message)
        {
            Provider = provider;
        }

        public string Provider { get; private set; }
    }
}