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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MobileDB.Common;
using MobileDB.Common.Utilities;
using MobileDB.FileSystem;
using MobileDB.FileSystem.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MobileDB.Stores
{
    public class BsonStore : StoreBase
    {
        private readonly ReaderWriterLockSlim _lock;
        private readonly FileSystemPath _path;
        private readonly JsonSerializer _serializer;

        public BsonStore(
            IFileSystem fileSystem,
            Type entityType)
            : base(fileSystem, entityType)
        {
            _lock = new ReaderWriterLockSlim();
            _serializer = JsonSerializer.CreateDefault();

            _path = FileSystemPath.Root.AppendDirectory(EntityType.ToFriendlyName());
        }

        public override FileSystemPath Path
        {
            get { return _path; }
        }

        //public int SaveChanges(ChangeSet changeSet)
        //{
        //    using (_lock.WriteLock())
        //    {
        //        EnsureInitialized();
        //        var affectedEntities = 0;

        //        foreach (var change in changeSet)
        //        {
        //            var key = change.Key.GetKeyFromEntity();
        //            ApplyChange(key, change.Key, change.Value);
        //            affectedEntities++;
        //        }

        //        return affectedEntities;
        //    }
        //}

        private async Task EnsureInitialized()
        {
            if (!await FileSystem.Exists(Path))
            {
                await FileSystem.CreateDirectory(Path);
            }
        }

        private async Task ApplyChange(
            object key,
            object entity,
            EntityState entityState)
        {
            var targetPath = Path.AppendFile(key.ToString());

            switch (entityState)
            {
                case EntityState.Deleted:
                    await FileSystem.Delete(targetPath);
                    break;

                case EntityState.Added:
                    var metadata = EntityMetadata(key, entity, EntityState.Added, null);
                    using (var stream = await FileSystem.CreateFile(targetPath))
                        Serialize(stream, metadata);
                    break;

                case EntityState.Updated:
                    var updatedMetadata = EntityMetadata(key, entity, EntityState.Updated, await FindByIdInternal(key));
                    using (var stream = await FileSystem.CreateFile(targetPath))
                        Serialize(stream, updatedMetadata);
                    break;
            }
        }

        private void Serialize(Stream stream, object entity)
        {
            using (var writer = new BsonWriter(stream))
            {
                _serializer.Serialize(writer, entity);
            }
        }

        private MetadataEntity Deserialize(Stream stream)
        {
            using (var reader = new BsonReader(stream))
            {
                return _serializer.Deserialize<MetadataEntity>(reader);
            }
        }

        public override Task<int> SaveChangesAsync(ChangeSet changeSet)
        {
            throw new NotImplementedException();
        }

        public override async Task<int> Count()
        {
            using (_lock.ReadLock())
            {
                await EnsureInitialized();

                return (await FileSystem.GetEntities(Path)).Count();
            }
        }

        private async Task<MetadataEntity> FindByIdInternal(object key)
        {
            await EnsureInitialized();
            var targetPath = Path.AppendFile(key.ToString());

            using (var stream = await FileSystem.OpenFile(targetPath, DesiredFileAccess.Read))
                return Deserialize(stream);
        }

        public override async Task<object> FindById(object key)
        {
            using (_lock.ReadLock())
            {
                return (await FindByIdInternal(key)).EntityOfType(EntityType);
            }
        }
    }
}
