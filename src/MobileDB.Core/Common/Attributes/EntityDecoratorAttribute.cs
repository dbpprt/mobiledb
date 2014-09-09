using System;

namespace MobileDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EntityDecoratorAttribute : Attribute
    {
        public EntityDecoratorAttribute(Type decorator)
        {
            Decorator = decorator;
        }

        public Type Decorator { get; set; }
    }
}