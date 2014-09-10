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
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MobileDB.Common;
using MobileDB.Common.Attributes;
using MobileDB.Common.Utilities;
using MobileDB.Contracts;
using MobileDB.Exceptions;
using MobileDB.FileSystem.Contracts;
using MobileDB.Stores;
using MobileDB.Stores.Json;

namespace MobileDB
{
    public class DbContext : IDbContext
    {
        private static readonly Dictionary<string, ContextConfiguration> CachedContextConfigurations;
        private static readonly ReaderWriterLockSlim Lock;
        private readonly Dictionary<EntityConfiguration, ChangeSet> _changeTracker;
        private readonly Dictionary<Type, object> _setInstances;

        static DbContext()
        {
            Lock = new ReaderWriterLockSlim();
            CachedContextConfigurations = new Dictionary<string, ContextConfiguration>();
        }

        public DbContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            ConnectionString = connectionString;

            ValidateEntities = true;

            _changeTracker = new Dictionary<EntityConfiguration, ChangeSet>();
            _setInstances = new Dictionary<Type, object>();

            bool initialized;
            var contextConfiguration = TryGetContextConfiguration(connectionString, out initialized);

            if (!initialized)
            {
                contextConfiguration = BuildEntityConfiguration(connectionString);
                AddContextConfiguration(connectionString, contextConfiguration);
            }

            InitializeInstance(contextConfiguration.EntityConfigurations);
            FileSystem = contextConfiguration.FileSystem;
        }

        public bool ValidateEntities { get; protected set; }

        public string ConnectionString { get; private set; }

        public IFileSystem FileSystem { get; private set; }

        //public int SaveChanges()
        //{
        //    var changedEntities =
        //        (from trackedChangeSet in _changeTracker
        //            let configuration = trackedChangeSet.Key
        //            let changeset = trackedChangeSet.Value
        //            select configuration.EntityStore.SaveChanges(changeset)).Sum();

        //    return changedEntities;
        //}

        public async Task<int> SaveChangesAsync()
        {
            // TODO: this could cause problems because the SaveChanges operations are asynchronous
            // and im not quite sure wether everything is threadsafe behind the scenes
            var tasks =
                (from trackedChangeSet in _changeTracker
                    let configuration = trackedChangeSet.Key
                    let changeset = trackedChangeSet.Value
                    select configuration.EntityStore.SaveChangesAsync(changeset));
            return await Task.WhenAll(tasks).ContinueWith(task => task.Result.Sum());
        }

        public IEntitySet<T> Set<T>() where T : new()
        {
            return Set(typeof (T)) as IEntitySet<T>;
        }

        public object Set(Type entityType)
        {
            object result;
            _setInstances.TryGetValue(entityType, out result);
            return result;
        }

        private ContextConfiguration BuildEntityConfiguration(string connectionString)
        {
            var fileSystemTypeName = connectionString.ConnectionStringPart(ConnectionStringConstants.Filesystem);

            Type fileSystemType;
            TypeHelper.TryGetTypeByName(fileSystemTypeName, out fileSystemType);

            if (fileSystemType == null)
            {
                throw new InvalidProviderException("The requested filesystem was not found",
                    fileSystemTypeName);
            }

            var configuration = new ContextConfiguration
            {
                EntityConfigurations = new List<EntityConfiguration>()
            };

            var fileSystem = Activator.CreateInstance(fileSystemType, connectionString) as IFileSystem;

            if (fileSystem == null)
            {
                throw new InvalidProviderException("The requested filesystem must implement IFileSystem",
                    fileSystemTypeName);
            }
            configuration.FileSystem = fileSystem;

            // its magic! do not touch it!
            // only god and i understood what i was doing
            configuration.EntityConfigurations =
                (from propertyInfo in GetType().GetRuntimeProperties()
                    let propertyType = propertyInfo.PropertyType
                    let genericTypeArgument = propertyType.GenericTypeArguments.FirstOrDefault()
                    where genericTypeArgument != null
                    let genericInterface = typeof (IEntitySet<>).MakeGenericType(genericTypeArgument)
                    where genericInterface.GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo())
                    let storeAttribute = propertyInfo.GetCustomAttribute<StoreAttribute>()
                    let storeType = storeAttribute != null ? storeAttribute.StoreType : typeof (JsonStore)
                    let storeInstance = Activator.CreateInstance(storeType, fileSystem, genericTypeArgument)
                    let setType = typeof (EntitySet<>).MakeGenericType(genericTypeArgument)
                    select new EntityConfiguration
                    {
                        EntityStore = storeInstance as StoreBase,
                        EntityType = genericTypeArgument,
                        PropertyInfo = propertyInfo,
                        EntitySetType = setType
                    }).ToList();

            return configuration;
        }


        private void InitializeInstance(IEnumerable<EntityConfiguration> entityConfigurations)
        {
            foreach (var entityConfiguration in entityConfigurations)
            {
                var changeSet = new ChangeSet();
                var set = Activator.CreateInstance(
                    entityConfiguration.EntitySetType,
                    changeSet,
                    entityConfiguration.EntityStore,
                    ValidateEntities);

                _changeTracker.Add(entityConfiguration, changeSet);
                _setInstances.Add(entityConfiguration.EntityType, set);

                entityConfiguration.PropertyInfo.SetValue(this, set);
            }
        }

        private void AddContextConfiguration(string connectionString, ContextConfiguration contextConfiguration)
        {
            try
            {
                Lock.EnterWriteLock();
                CachedContextConfigurations.Add(connectionString, contextConfiguration);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        private ContextConfiguration TryGetContextConfiguration(string connectionString, out bool initialized)
        {
            ContextConfiguration result;

            try
            {
                Lock.EnterReadLock();
                initialized = CachedContextConfigurations.TryGetValue(connectionString, out result);
            }
            finally
            {
                Lock.ExitReadLock();
            }

            return result;
        }

        internal Dictionary<Type, object> Sets()
        {
            return _setInstances;
        }
    }
}
