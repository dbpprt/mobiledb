using System;
using System.Collections.Generic;
using System.Linq;
using MobileDB.Common.Validation;
using MobileDB.Contracts;
using MobileDB.Stores;
using MobileDB.Stores.Json;

namespace MobileDB
{
    public static class ServiceLocator
    {
        private static readonly List<Type> SupportedStores;
        private static IEntityValidator _entityValidator;

        private static readonly object Sync;

        static ServiceLocator()
        {
            SupportedStores = new List<Type>();
            Sync = new object();

            RegisterDefaultServices();
        }

        public static IEntityValidator Validator
        {
            get
            {
                lock (Sync)
                {
                    return _entityValidator;
                }
            }
            set
            {
                lock (Sync)
                {
                    _entityValidator = value;
                }
            }
        }

        private static void RegisterDefaultServices()
        {
            SupportedStores.Add(typeof (JsonStore));
            SupportedStores.Add(typeof (BsonStore));
            _entityValidator = new EntityValidator();
        }

        public static void AddStore(Type type)
        {
            lock (Sync)
            {
                if (!SupportedStores.Contains(type))
                    SupportedStores.Add(type);
            }
        }

        public static IEnumerable<Type> Stores()
        {
            lock (Sync)
            {
                return SupportedStores.ToList();
            }
        }
    }
}