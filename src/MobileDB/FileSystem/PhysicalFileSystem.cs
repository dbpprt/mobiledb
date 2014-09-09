using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MobileDB.Common;
using MobileDB.Common.Utilities;
using MobileDB.FileSystem.Contracts;

namespace MobileDB.FileSystem
{
    public class PhysicalFileSystem : IFileSystem
    {
        public PhysicalFileSystem(string connectionString)
        {
            var physicalRoot = connectionString.ConnectionStringPart(ConnectionStringConstants.Path);

            if (!Path.IsPathRooted(physicalRoot))
                physicalRoot = Path.GetFullPath(physicalRoot);
            if (physicalRoot[physicalRoot.Length - 1] != Path.DirectorySeparatorChar)
                physicalRoot = physicalRoot + Path.DirectorySeparatorChar;
            PhysicalRoot = physicalRoot;
        }

        public string PhysicalRoot { get; private set; }

        public ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            var physicalPath = GetPhysicalPath(path);
            var directories = Directory.GetDirectories(physicalPath);
            var files = Directory.GetFiles(physicalPath);
            var virtualDirectories =
                directories.Select(GetVirtualDirectoryPath);
            var virtualFiles =
                files.Select(GetVirtualFilePath);
            return new EnumerableCollection<FileSystemPath>(virtualDirectories.Concat(virtualFiles),
                directories.Length + files.Length);
        }

        public bool Exists(FileSystemPath path)
        {
            return path.IsFile ? File.Exists(GetPhysicalPath(path)) : Directory.Exists(GetPhysicalPath(path));
        }

        public Stream CreateFile(FileSystemPath path)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is not a file.", "path");
            return File.Create(GetPhysicalPath(path));
        }

        public Stream OpenFile(FileSystemPath path, DesiredFileAccess access)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is not a file.", "path");
            return File.Open(GetPhysicalPath(path), FileMode.Open, access.ToFileAccess());
        }

        public void CreateDirectory(FileSystemPath path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is not a directory.", "path");
            Directory.CreateDirectory(GetPhysicalPath(path));
        }

        public void Delete(FileSystemPath path)
        {
            if (path.IsFile)
                File.Delete(GetPhysicalPath(path));
            else
                Directory.Delete(GetPhysicalPath(path), true);
        }

        public void Dispose()
        {
        }

        public string GetPhysicalPath(FileSystemPath path)
        {
            return Path.Combine(PhysicalRoot,
                path.ToString().Remove(0, 1).Replace(FileSystemPath.DirectorySeparator, Path.DirectorySeparatorChar));
        }

        public FileSystemPath GetVirtualFilePath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("The specified path is not member of the PhysicalRoot.", "physicalPath");
            var virtualPath = FileSystemPath.DirectorySeparator +
                              physicalPath.Remove(0, PhysicalRoot.Length)
                                  .Replace(Path.DirectorySeparatorChar, FileSystemPath.DirectorySeparator);
            return FileSystemPath.Parse(virtualPath);
        }

        public FileSystemPath GetVirtualDirectoryPath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("The specified path is not member of the PhysicalRoot.", "physicalPath");
            var virtualPath = FileSystemPath.DirectorySeparator +
                              physicalPath.Remove(0, PhysicalRoot.Length)
                                  .Replace(Path.DirectorySeparatorChar, FileSystemPath.DirectorySeparator);
            if (virtualPath[virtualPath.Length - 1] != FileSystemPath.DirectorySeparator)
                virtualPath += FileSystemPath.DirectorySeparator;
            return FileSystemPath.Parse(virtualPath);
        }
    }
}
