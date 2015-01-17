using FluentAssertions;
using MobileDB.Common;
using Xunit;

namespace MobileDB.Tests
{
    public class ConfigConnectionStringTests
    {
        [Fact(DisplayName = "Should return specified connection string in app.config")]
        public void ShouldReturnConnectionStringSpecifiedInConfig()
        {
            const string expectedConnectionString = @"filesystem=MobileDB.FileSystem.PhysicalFileSystem;path=C:\Development\database";

            var connectionString = ConfigConnectionString.FromAppConfig("Default");

            connectionString.Should().Be(expectedConnectionString);
        }

        [Fact(DisplayName = "Should return ConfigConnectionString instance.")]
        public void ShouldReturnConnectionStringInstanceFromSpecifiedConfiguration()
        {
            const string connectionString = @"filesystem=MobileDB.FileSystem.PhysicalFileSystem;path=C:\Development\database";

            var configConnectionStringInstance = new ConfigConnectionString(connectionString);
            var filesystem = configConnectionStringInstance.GetPart("filesystem");
            var path = configConnectionStringInstance.GetPart("path");

            filesystem.Should().Be("MobileDB.FileSystem.PhysicalFileSystem");
            path.Should().Be(@"C:\Development\database");
            configConnectionStringInstance.As<ConnectionString>().Should().NotBeNull();
        }
    }
}
