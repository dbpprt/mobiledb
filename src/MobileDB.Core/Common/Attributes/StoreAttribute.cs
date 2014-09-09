using System;

namespace MobileDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StoreAttribute : Attribute
    {
        public StoreAttribute(Type store)
        {
            StoreType = store;
        }

        public Type StoreType { get; private set; }
    }
}