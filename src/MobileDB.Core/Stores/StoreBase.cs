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
using System.Threading.Tasks;
using MobileDB.Common;
using MobileDB.FileSystem;
using MobileDB.FileSystem.Contracts;
using MobileDB.Stores.Contracts;

namespace MobileDB.Stores
{
    public abstract class StoreBase : IStore
    {
        protected IFileSystem FileSystem
        {
            get
            {
                if (_fileSystem == null)
                    throw new NotSupportedException("The loaded filesystem doesnt support synchronous access!");
                return _fileSystem;
            }
        }

        protected IAsyncFileSystem AsyncFileSystem
        {
            get
            {
                if (_asyncFileSystem == null)
                    throw new NotSupportedException("The loaded filesystem doesnt support async access!");
                return _asyncFileSystem;
            }
        }

        private readonly Type _entityType;
        private readonly IAsyncFileSystem _asyncFileSystem;
        private readonly IFileSystem _fileSystem;

        public StoreBase(
            FileSystemBase fileSystem,
            Type entityType)
        {
            _entityType = entityType;

            _asyncFileSystem = fileSystem as IAsyncFileSystem;
            _fileSystem = fileSystem as IFileSystem;

        }

        protected Type EntityType
        {
            get { return _entityType; }
        }

        public abstract FileSystemPath Path { get; }

        public abstract Task<int> SaveChangesAsync(ChangeSet changeSet);

        public abstract Task<object> FindById(object key);

        public abstract Task<int> Count();

        protected virtual MetadataEntity EntityMetadata(
            object key,
            object entity,
            EntityState state,
            MetadataEntity existing)
        {
            if (state == EntityState.Added)
            {
                var result = new MetadataEntity
                {
                    Identity = key,
                    Entity = entity,
                    Metadata = new Dictionary<string, string>()
                };

                return result;
            }

            if (state == EntityState.Updated && existing != null)
            {
                existing.Entity = entity;
                existing.Identity = key;
                // existing.Metadata setLastModified
            }

            throw new ArgumentException("Only god and me know how to use this function. Wrong parameter constellation.");
        }
    }
}
