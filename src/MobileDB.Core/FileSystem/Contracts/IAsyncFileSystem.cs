using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileDB.FileSystem.Contracts
{
    public interface IAsyncFileSystem : IDisposable
    {
        Task<IEnumerable<FileSystemPath>> GetEntities(FileSystemPath path, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> Exists(FileSystemPath path, CancellationToken cancellationToken = default(CancellationToken));

        Task<Stream> CreateFile(FileSystemPath path, CancellationToken cancellationToken = default(CancellationToken));

        Task<Stream> OpenFile(FileSystemPath path, DesiredFileAccess access, CancellationToken cancellationToken = default(CancellationToken));

        Task CreateDirectory(FileSystemPath path, CancellationToken cancellationToken = default(CancellationToken));

        Task Delete(FileSystemPath path, CancellationToken cancellationToken = default(CancellationToken));
    }
}
