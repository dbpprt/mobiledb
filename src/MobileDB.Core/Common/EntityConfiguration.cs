using System;
using System.Reflection;
using MobileDB.Stores;

namespace MobileDB.Common
{
    public class EntityConfiguration
    {
        public PropertyInfo PropertyInfo { get; set; }

        public Type EntityType { get; set; }

        public StoreBase EntityStore { get; set; }

        public Type EntitySetType { get; set; }
    }
}