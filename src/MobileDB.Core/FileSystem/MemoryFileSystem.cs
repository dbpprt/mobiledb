#region Copyright (C) 2014 Dennis Bappert
// The MIT License (MIT)

// Copyright (c) 2014 Dennis Bappert

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MobileDB.Common;
using MobileDB.Common.Utilities;
using MobileDB.Exceptions;
using MobileDB.FileSystem.Contracts;

namespace MobileDB.FileSystem
{
    public class MemoryFileSystem : FileSystemBase, IAsyncFileSystem
    {
        private readonly IDictionary<FileSystemPath, LinkedList<FileSystemPath>> _directories;
        private readonly IDictionary<FileSystemPath, MemoryFile> _files;

        /// <summary>
        ///     Connection string is required by convention for every FileSystems
        /// </summary>
        /// <param name="connectionString"></param>
        public MemoryFileSystem(ConnectionString connectionString)
            : base(connectionString)
        {
            _files = new Dictionary<FileSystemPath, MemoryFile>();
            _directories = new Dictionary<FileSystemPath, LinkedList<FileSystemPath>>
            {
                {
                    FileSystemPath.Root,
                    new LinkedList<FileSystemPath>()
                }
            };
        }

        public async Task<IEnumerable<FileSystemPath>> GetEntities(FileSystemPath path, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is no directory.", "path");
            LinkedList<FileSystemPath> subentities;
            if (!_directories.TryGetValue(path, out subentities))
                throw new DirectoryNotFoundException();
            return subentities;
        }

        public async Task<bool> Exists(FileSystemPath path, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            return path.IsDirectory ? _directories.ContainsKey(path) : _files.ContainsKey(path);
        }

        public async Task<Stream> CreateFile(FileSystemPath path, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (!path.IsFile)
                throw new ArgumentException("The specified path is no file.", "path");
            if (!_directories.ContainsKey(path.ParentPath))
                throw new DirectoryNotFoundException();
            _directories[path.ParentPath].AddLast(path);
            return new MemoryFileStream(_files[path] = new MemoryFile());
        }

        public async Task<Stream> OpenFile(FileSystemPath path, DesiredFileAccess access, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (!path.IsFile)
                throw new ArgumentException("The specified path is no file.", "path");
            MemoryFile file;
            if (!_files.TryGetValue(path, out file))
                throw new FileNotFoundException();
            return new MemoryFileStream(file);
        }

        public async Task CreateDirectory(FileSystemPath path, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is no directory.", "path");
            LinkedList<FileSystemPath> subentities;
            if (_directories.ContainsKey(path))
                throw new ArgumentException("The specified directory-path already exists.", "path");
            if (!_directories.TryGetValue(path.ParentPath, out subentities))
                throw new DirectoryNotFoundException();
            subentities.AddLast(path);
            _directories[path] = new LinkedList<FileSystemPath>();
        }

        public async Task Delete(FileSystemPath path, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (path.IsRoot)
                throw new ArgumentException("The root cannot be deleted.");
            bool removed;
            if (path.IsDirectory)
                removed = _directories.Remove(path);
            else
                removed = _files.Remove(path);
            if (!removed)
                throw new ArgumentException("The specified path does not exist.");
            var parent = _directories[path.ParentPath];
            parent.Remove(path);
        }

        public void Dispose()
        {
        }

        public class MemoryFile
        {
            public MemoryFile()
                : this(new byte[0])
            {
            }

            public MemoryFile(byte[] content)
            {
                Content = content;
            }

            public byte[] Content { get; set; }
        }

        public class MemoryFileStream : Stream
        {
            private readonly MemoryFile _file;

            public MemoryFileStream(MemoryFile file)
            {
                _file = file;
            }

            public byte[] Content
            {
                get { return _file.Content; }
                set { _file.Content = value; }
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override long Length
            {
                get { return _file.Content.Length; }
            }

            public override long Position { get; set; }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                if (origin == SeekOrigin.Begin)
                    return Position = offset;
                if (origin == SeekOrigin.Current)
                    return Position += offset;
                return Position = Length - offset;
            }

            public override void SetLength(long value)
            {
                var newLength = (int) value;
                var newContent = new byte[newLength];
                Buffer.BlockCopy(Content, 0, newContent, 0, Math.Min(newLength, (int) Length));
                Content = newContent;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var mincount = Math.Min(count, Math.Abs((int) (Length - Position)));
                Buffer.BlockCopy(Content, 0, buffer, offset, mincount);
                Position += mincount;
                return mincount;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (Length - Position < count)
                    SetLength(Position + count);
                Buffer.BlockCopy(buffer, offset, Content, (int) Position, count);
                Position += count;
            }
        }
    }
}
