using System;
using System.Collections.Generic;
using System.IO;

namespace MobileDB.FileSystem.Contracts
{
    public interface IFileSystem : IDisposable
    {
        ICollection<FileSystemPath> GetEntities(FileSystemPath path);

        bool Exists(FileSystemPath path);

        Stream CreateFile(FileSystemPath path);

        Stream OpenFile(FileSystemPath path, DesiredFileAccess access);

        void CreateDirectory(FileSystemPath path);

        void Delete(FileSystemPath path);
    }
}