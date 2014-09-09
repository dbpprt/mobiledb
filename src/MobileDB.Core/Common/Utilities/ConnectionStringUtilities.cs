using System;
using System.Linq;
using MobileDB.Exceptions;

namespace MobileDB.Common.Utilities
{
    public static class ConnectionStringUtilities
    {
        public static string ConnectionStringPart(this string connectionString, string key)
        {
            var segments = connectionString.Split(ConnectionStringConstants.TupleSeperator);
            var tuples = segments
                .Select(segment => segment.Split(ConnectionStringConstants.SegmentSeperator))
                .ToDictionary(parts => parts.First().ToLowerInvariant().Trim(), parts => parts.Last().Trim());

            string value;
            if (!tuples.TryGetValue(key, out value))
            {
                throw new InvalidConnectionStringException(
                    String.Format("ConnectionString must provide a valid {0} segment",
                        key),
                    connectionString);
            }
            return value;
        }
    }
}