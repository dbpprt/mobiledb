using MobileDB.Common.Attributes;
using MobileDB.Contracts;
using MobileDB.Stores.Json;

namespace MobileDB.Tests.Common
{
    public class JsonContextWithSimpleIdentity : DbContext
    {
        public JsonContextWithSimpleIdentity(string connectionString)
            : base(connectionString)
        {
        }

        [Store(typeof (JsonStore))]
        public IEntitySet<SimpleEntityWithIdentity> SimpleEntities { get; set; }
    }
}