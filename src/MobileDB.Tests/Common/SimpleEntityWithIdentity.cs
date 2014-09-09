using System;
using MobileDB.Common.Attributes;

namespace MobileDB.Tests.Common
{
    public class SimpleEntityWithIdentity
    {
        [Identity]
        public Guid Id { get; set; }

        public string Value { get; set; }
    }
}