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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MobileDB.Common;
using MobileDB.Common.Utilities;
using MobileDB.FileSystem;
using MobileDB.FileSystem.Contracts;
using MobileDB.Stores.Contracts;
using Newtonsoft.Json;

namespace MobileDB.Stores.Json
{
    public class JsonStore : StoreBase, IQueryableStore, IStatefulStore
    {
        private const string FileFormatString = "{0}.json";

        private readonly SimpleListSerializer _listSerializer;

        private readonly ReaderWriterLockSlim _lock;
        private readonly FileSystemPath _path;
        private readonly JsonSerializer _serializer;
        private Dictionary<object, Tuple<object, MetadataEntity>> _entities;
        private bool _initialized;

        // <Key, Tuple<Entity, Metadata>>

        public JsonStore(
            FileSystemBase fileSystem,
            Type entityType
            )
            : base(fileSystem, entityType)
        {
            _entities = new Dictionary<object, Tuple<object, MetadataEntity>>();
            _listSerializer = new SimpleListSerializer();
            _serializer = JsonSerializer.CreateDefault();
            _lock = new ReaderWriterLockSlim();
            _initialized = false;

            _path = FileSystemPath.Root.AppendFile(
                String.Format(FileFormatString, EntityType.ToFriendlyName()));
        }

        public override FileSystemPath Path
        {
            get { return _path; }
        }

        public IQueryable<T> AsQueryable<T>()
        {
            //EnsureInitialized();

            using (_lock.ReadLock())
            {
                return _entities.Values
                    .Select(_ => _.Item1)
                    .OfType<T>()
                    .ToList()
                    .AsQueryable();
            }
        }

        public async Task<IQueryable<T>> AsQueryableAsync<T>()
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(default(CancellationToken));
            await EnsureInitializedAsync();

            using (_lock.ReadLock())
            {
                return _entities.Values
                    .Select(_ => _.Item1)
                    .OfType<T>()
                    .ToList()
                    .AsQueryable();
            }
        }

        public void Release()
        {
            using (_lock.WriteLock())
            {
                _initialized = false;
                _entities = new Dictionary<object, Tuple<object, MetadataEntity>>();
            }
        }

        public async Task EnsureInitializedAsync()
        {
            if (_initialized) return;

            using (_lock.WriteLock())
            {
                if (!await AsyncFileSystem.ExistsAsync(Path))
                {
                    _initialized = true;
                    return;
                }

                using (var stream = await AsyncFileSystem.OpenFileAsync(Path, DesiredFileAccess.Read))
                using (var instream = new StreamReader(stream))
                {
                    var json = "[" + instream.ReadToEnd().Replace(Environment.NewLine, ",") + "]";
                    var entities = JsonConvert.DeserializeObject(
                        json,
                        typeof (List<>).MakeGenericType(typeof (MetadataEntity))
                        ) as IEnumerable<MetadataEntity>;

                    _entities = entities.ToDictionary(
                        key => key.Identity,
                        value => new Tuple<object, MetadataEntity>(value.EntityOfType(EntityType), value)
                        );
                    _initialized = true;
                }
            }
        }

        public void EnsureInitialized()
        {
            if (_initialized) return;

            using (_lock.WriteLock())
            {
                if (!FileSystem.Exists(Path))
                {
                    _initialized = true;
                    return;
                }

                using (var stream = FileSystem.OpenFile(Path, DesiredFileAccess.Read))
                using (var instream = new StreamReader(stream))
                {
                    var json = "[" + instream.ReadToEnd().Replace(Environment.NewLine, ",") + "]";
                    var entities = JsonConvert.DeserializeObject(
                        json,
                        typeof (List<>).MakeGenericType(typeof (MetadataEntity))
                        ) as IEnumerable<MetadataEntity>;

                    _entities = entities.ToDictionary(
                        key => key.Identity,
                        value => new Tuple<object, MetadataEntity>(value.EntityOfType(EntityType), value)
                        );
                    _initialized = true;
                }
            }
        }

        public override async Task<int> SaveChangesAsync(ChangeSet changeSet)
        {
            await EnsureInitializedAsync();

            using (_lock.WriteLock())
            {
                var affectedEntities = 0;

                foreach (var change in changeSet)
                {
                    var key = change.Key.GetKeyFromEntity();
                    Tuple<object, MetadataEntity> existing;
                    _entities.TryGetValue(key, out existing);

                    ApplyChange(key, change.Key, change.Value, existing);

                    affectedEntities++;
                }

                var raw = _entities.Values
                    .Select(_ => _.Item2)
                    .ToList();

                await FlushAsync(raw);

                return affectedEntities;
            }
        }

        private async Task FlushAsync(List<MetadataEntity> raw)
        {
            using (var stream = await AsyncFileSystem.CreateFileAsync(Path))
            using (var outstream = new StreamWriter(stream))
            {
                var writer = new JsonTextWriter(outstream);
                _listSerializer.WriteJson(writer, raw, _serializer);
            }
        }

        private void Flush(List<MetadataEntity> raw)
        {
            using (var stream = FileSystem.CreateFile(Path))
            using (var outstream = new StreamWriter(stream))
            {
                var writer = new JsonTextWriter(outstream);
                _listSerializer.WriteJson(writer, raw, _serializer);
            }
        }

        private void ApplyChange(
            object key,
            object entity,
            EntityState entityState,
            Tuple<object, MetadataEntity> existing)
        {
            switch (entityState)
            {
                case EntityState.Deleted:
                    _entities.Remove(key);
                    break;

                case EntityState.Added:
                    var metadata = EntityMetadata(key, entity, EntityState.Added, null);
                    _entities.Add(key, new Tuple<object, MetadataEntity>(entity, metadata));
                    break;

                case EntityState.Updated:
                    var updatedMetadata = EntityMetadata(key, entity, EntityState.Updated, existing.Item2);
                    _entities[key] = new Tuple<object, MetadataEntity>(entity, updatedMetadata);
                    break;
            }
        }

        public override async Task<int> CountAsync()
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(default(CancellationToken));
            await EnsureInitializedAsync();
            return Count();
        }

        public override int SaveChanges(ChangeSet changeSet)
        {
            EnsureInitialized();

            using (_lock.WriteLock())
            {
                var affectedEntities = 0;

                foreach (var change in changeSet)
                {
                    var key = change.Key.GetKeyFromEntity();
                    Tuple<object, MetadataEntity> existing;
                    _entities.TryGetValue(key, out existing);

                    ApplyChange(key, change.Key, change.Value, existing);

                    affectedEntities++;
                }

                var raw = _entities.Values
                    .Select(_ => _.Item2)
                    .ToList();

                Flush(raw);

                return affectedEntities;
            }
        }

        public override object FindById(object key)
        {
            EnsureInitialized();

            using (_lock.ReadLock())
            {
                Tuple<object, MetadataEntity> entity;
                _entities.TryGetValue(key, out entity);
                return entity != null
                    ? entity.Item1
                    : null;
            }
        }

        public override int Count()
        {
            EnsureInitialized();

            using (_lock.ReadLock())
            {
                return _entities.Count;
            }
        }

        public override async Task<object> FindByIdAsync(object key)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(default(CancellationToken));
            await EnsureInitializedAsync();

            using (_lock.ReadLock())
            {
                Tuple<object, MetadataEntity> entity;
                _entities.TryGetValue(key, out entity);
                return entity != null
                    ? entity.Item1
                    : null;
            }
        }
    }
}