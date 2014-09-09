using MobileDB.Contracts;

namespace MobileDB.Tests.Common
{
    public class ContextWithSimpleIdentity : DbContext
    {
        public ContextWithSimpleIdentity(string connectionString)
            : base(connectionString)
        {
        }

        public IEntitySet<SimpleEntityWithIdentity> SimpleEntities { get; set; }
    }
}