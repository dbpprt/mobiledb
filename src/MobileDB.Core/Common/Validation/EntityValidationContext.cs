using System;

namespace MobileDB.Common.Validation
{
    public class EntityValidationContext
    {
        public object ObjectInstance { get; set; }
        public Type ObjectType { get; set; }
        public string DisplayName { get; set; }
        public object MemberName { get; set; }
    }
}