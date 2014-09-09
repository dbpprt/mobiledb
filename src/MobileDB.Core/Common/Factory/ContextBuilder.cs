using System.Collections.Generic;
using MobileDB.FileSystem;

namespace MobileDB.Common.Factory
{
    public class ContextBuilder<T>
    {
        public readonly Dictionary<string, string> Tuples;

        public ContextBuilder()
        {
            Tuples = new Dictionary<string, string>();
        }
    }

    public static class ContextBuilderExtensions
    {
        public static Builder<T> WithMemoryFilesystem<T>(this ContextBuilder<T> contextBuilder)
        {
            contextBuilder.Tuples.Add(ConnectionStringConstants.Filesystem, typeof (MemoryFileSystem).FullName);
            return new Builder<T>(contextBuilder.Tuples);
        }
    }
}