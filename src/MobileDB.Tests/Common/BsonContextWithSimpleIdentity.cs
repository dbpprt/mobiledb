using MobileDB.Common.Attributes;
using MobileDB.Contracts;

namespace MobileDB.Tests.Common
{
    public class BsonContextWithSimpleIdentity : DbContext
    {
        public BsonContextWithSimpleIdentity(string connectionString)
            : base(connectionString)
        {
        }

        [LazyBson]
        public IEntitySet<SimpleEntityWithIdentity> SimpleEntities { get; set; }
    }
}