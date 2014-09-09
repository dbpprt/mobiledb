using System.Collections.Generic;

namespace MobileDB.Common.Factory
{
    public class BsonContextBuilder<T>
    {
        private readonly Dictionary<string, string> _tuples;

        public BsonContextBuilder(Dictionary<string, string> tuples)
        {
            _tuples = tuples;
        }

        public Builder<T> WithDatabaseDirectory(string path)
        {
            _tuples.Add(ConnectionStringConstants.Path, path);
            return new Builder<T>(_tuples);
        }
    }
}