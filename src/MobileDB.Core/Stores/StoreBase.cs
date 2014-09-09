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
        protected readonly IFileSystem FileSystem;
        private readonly Type _entityType;

        public StoreBase(
            IFileSystem fileSystem,
            Type entityType)
        {
            FileSystem = fileSystem;
            _entityType = entityType;
        }

        protected Type EntityType
        {
            get { return _entityType; }
        }

        public abstract FileSystemPath Path { get; }

        public abstract int SaveChanges(ChangeSet changeSet);

        public abstract Task<int> SaveChangesAsync(ChangeSet changeSet);

        public abstract object FindById(object key);

        public abstract int Count();

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