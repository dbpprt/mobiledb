using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileDB.Common;
using MobileDB.Common.Utilities;
using MobileDB.FileSystem;
using MobileDB.FileSystem.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Nito.AsyncEx;

namespace MobileDB.Stores
{
    public class BsonStore : StoreBase
    {
        private readonly AsyncReaderWriterLock _lock;
        private readonly FileSystemPath _path;
        private readonly JsonSerializer _serializer;

        public BsonStore(
            IFileSystem fileSystem,
            Type entityType)
            : base(fileSystem, entityType)
        {
            _lock = new AsyncReaderWriterLock();
            _serializer = JsonSerializer.CreateDefault();

            _path = FileSystemPath.Root.AppendDirectory(EntityType.ToFriendlyName());
        }

        public override FileSystemPath Path
        {
            get { return _path; }
        }

        public override int SaveChanges(ChangeSet changeSet)
        {
            using (_lock.WriterLock())
            {
                EnsureInitialized();
                var affectedEntities = 0;

                foreach (var change in changeSet)
                {
                    var key = change.Key.GetKeyFromEntity();
                    ApplyChange(key, change.Key, change.Value);
                    affectedEntities++;
                }

                return affectedEntities;
            }
        }

        private void EnsureInitialized()
        {
            if (!FileSystem.Exists(Path))
            {
                FileSystem.CreateDirectory(Path);
            }
        }

        private void ApplyChange(
            object key,
            object entity,
            EntityState entityState)
        {
            var targetPath = Path.AppendFile(key.ToString());

            switch (entityState)
            {
                case EntityState.Deleted:
                    FileSystem.Delete(targetPath);
                    break;

                case EntityState.Added:
                    var metadata = EntityMetadata(key, entity, EntityState.Added, null);
                    using (var stream = FileSystem.CreateFile(targetPath))
                        Serialize(stream, metadata);
                    break;

                case EntityState.Updated:
                    var updatedMetadata = EntityMetadata(key, entity, EntityState.Updated, FindByIdInternal(key));
                    using (var stream = FileSystem.CreateFile(targetPath))
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

        public override int Count()
        {
            using (_lock.ReaderLock())
            {
                EnsureInitialized();

                return FileSystem.GetEntities(Path).Count();
            }
        }

        private MetadataEntity FindByIdInternal(object key)
        {
            EnsureInitialized();
            var targetPath = Path.AppendFile(key.ToString());

            using (var stream = FileSystem.OpenFile(targetPath, DesiredFileAccess.Read))
                return Deserialize(stream);
        }

        public override object FindById(object key)
        {
            using (_lock.ReaderLock())
            {
                return FindByIdInternal(key).EntityOfType(EntityType);
            }
        }
    }
}