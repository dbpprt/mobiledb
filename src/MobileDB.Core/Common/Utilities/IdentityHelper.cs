using System;
using System.Linq;
using System.Reflection;
using MobileDB.Common.Attributes;
using MobileDB.Exceptions;

namespace MobileDB.Common.Utilities
{
    public static class IdentityHelper
    {
        public static object GetKeyFromEntity(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var type = obj.GetType();

            var identityProperty = type.GetKeyFromEntityType();

            return identityProperty == null
                ? null
                : identityProperty.GetValue(obj);
        }

        public static void SetEntityKey(this object obj, object key)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var type = obj.GetType();

            var identityProperty = type.GetKeyFromEntityType();
            identityProperty.SetValue(obj, key);
        }

        public static PropertyInfo GetKeyFromEntityType(this Type type)
        {
            var properties = type.GetRuntimeProperties();
            PropertyInfo identityProperty;
            try
            {
                identityProperty = properties
                    .GroupBy(attr => attr.GetCustomAttribute<IdentityAttribute>())
                    .Where(group => @group != null && @group.Key != null)
                    .Where(prop => prop.Any())
                    .Select(prop => prop.SingleOrDefault())
                    .SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw new IdentityAttributeMismatchException("You must only specify one primary key per entity");
            }

            return identityProperty;
        }
    }
}