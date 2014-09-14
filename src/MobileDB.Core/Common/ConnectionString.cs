using System;
using System.Collections.Generic;
using System.Linq;
using MobileDB.Exceptions;

namespace MobileDB.Common
{
    public class ConnectionString
    {
        private readonly Dictionary<string, string> _connectionStringTuples;
        private readonly string _connectionString;

        public ConnectionString(
            string connectionString
            )
        {
            _connectionString = connectionString;

            var segments = connectionString.Split(ConnectionStringConstants.TupleSeperator);
            _connectionStringTuples = segments
                .Select(segment => segment.Split(ConnectionStringConstants.SegmentSeperator))
                .ToDictionary(parts => parts.First().ToLowerInvariant().Trim(), parts => parts.Last().Trim());
        }

        public override string ToString()
        {
            return _connectionString;
        }

        public string GetPart(string key)
        {
            string value;
            if (!_connectionStringTuples.TryGetValue(key, out value))
            {
                throw new InvalidConnectionStringException(
                    String.Format("ConnectionString must provide a valid {0} segment",
                        key),
                    _connectionString);
            }

            return value;
        }
    }
}
