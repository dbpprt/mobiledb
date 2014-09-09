using MobileDB.FileSystem;

namespace MobileDB.Common.Factory
{
    public static class ContextBuilderExtensions
    {
        public static Builder<T> WithPhysicalFilesystem<T>(this ContextBuilder<T> contextBuilder, string databasePath)
        {
            ServiceLocator.AddStore(typeof (PhysicalFileSystem));

            contextBuilder.Tuples.Add(ConnectionStringConstants.Filesystem, typeof (PhysicalFileSystem).FullName);
            contextBuilder.Tuples.Add(ConnectionStringConstants.Path, databasePath);
            return new Builder<T>(contextBuilder.Tuples);
        }
    }
}
