using System;

namespace MobileDB.Exceptions
{
    public class InvalidConnectionStringException : Exception
    {
        public InvalidConnectionStringException(string message, string connectionString) : base(message)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
    }
}