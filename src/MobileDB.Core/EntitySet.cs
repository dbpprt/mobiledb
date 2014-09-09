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
