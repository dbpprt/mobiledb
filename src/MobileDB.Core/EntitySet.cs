using System;
using System.Linq;
using MobileDB.Common;
using MobileDB.Common.Utilities;
using MobileDB.Contracts;
using MobileDB.Stores;
using MobileDB.Stores.Contracts;

namespace MobileDB
{
    public class EntitySet<TEntity> : IEntitySet<TEntity> where TEntity : class, new()
    {
        private readonly ChangeSet _changeSet;
        private readonly StoreBase _dataSource;
        private readonly bool _validateEntity;
        private readonly IEntityValidator _validator;

        public EntitySet(
            ChangeSet changeSet,
            StoreBase dataSource,
            bool validateEntity
            )
        {
            _changeSet = changeSet;
            _dataSource = dataSource;
            _validateEntity = validateEntity;
            if (validateEntity)
                _validator = ServiceLocator.Validator;
        }

        public void Add(TEntity entity)
        {
            Validate(entity);
            _changeSet.Add(entity, EntityState.Added);
        }

        public void Remove(TEntity entity)
        {
            _changeSet.Add(entity, EntityState.Deleted);
        }

        public TEntity FindById(object key)
        {
            return _dataSource.FindById(key) as TEntity;
        }

        public void Update(TEntity entity)
        {
            Validate(entity);
            _changeSet.Add(entity, EntityState.Updated);
        }

        public void RemoveById(object key)
        {
            var dummy = new TEntity();
            dummy.SetEntityKey(key);

            _changeSet.Add(dummy, EntityState.Deleted);
        }

        public int Count()
        {
            return _dataSource.Count();
        }

        public IQueryable<TEntity> AsQueryable()
        {
            var queryable = _dataSource as IQueryableStore;

            if (queryable == null)
                throw new NotSupportedException(
                    "The store for this entity doesnt support AsQueryable. See docs for more info.");

            return queryable.AsQueryable<TEntity>();
        }

        public void Release()
        {
            var statefulStore = _dataSource as IStatefulStore;

            if (statefulStore != null)
            {
                statefulStore.Release();
            }
        }

        private void Validate(TEntity entity)
        {
            if (_validateEntity)
                _validator.ValidateEntity(entity);
        }
    }
}