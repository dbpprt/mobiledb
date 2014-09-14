namespace MobileDB
{
    public class DbContext : DbContextBase
    {
        public DbContext(string nameOrConnectionString) 
            : base(new ConfigConnectionString(nameOrConnectionString))
        {
        }
    }
}
