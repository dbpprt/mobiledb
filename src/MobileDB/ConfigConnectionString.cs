using System.Configuration;
using System.Linq;
using MobileDB.Common;

namespace MobileDB
{
    internal class ConfigConnectionString : ConnectionString
    {
        public ConfigConnectionString(string nameOrConnectionString)
            : base(FromAppConfig(nameOrConnectionString))
        {
        }

        public static string FromAppConfig(string nameOrConnectionString)
        {
            var connectionString = ConfigurationManager.ConnectionStrings.OfType<ConnectionStringSettings>()
                .FirstOrDefault(_ => _.Name == nameOrConnectionString);

            return connectionString == null ? nameOrConnectionString : connectionString.ConnectionString;
        }
    }
}
