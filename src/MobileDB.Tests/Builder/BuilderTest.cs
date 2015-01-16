using System.Collections.Generic;
using FluentAssertions;
using MobileDB.Common;
using MobileDB.Common.Factory;
using MobileDB.FileSystem;
using MobileDB.Tests.Common;
using Xunit;

namespace MobileDB.Tests.Builder
{
    public class BuilderTest
    {
        private Builder<BsonContextWithSimpleIdentity> _contextBuilder;

        public BuilderTest()
        {
            const string databasePath = @"C:\Development\database";
            var contextBuilder = new Dictionary<string, string>
            {
                {ConnectionStringConstants.Filesystem, typeof (PhysicalFileSystem).FullName},
                {ConnectionStringConstants.Path, databasePath}
            };

            _contextBuilder = new Builder<BsonContextWithSimpleIdentity>(contextBuilder);
        }

        [Fact(DisplayName = "ConnectionString should return the formatted connection string.")]
        public void ShouldGetExpectedConnectionString()
        {
            const string expectedConnectionString = @"filesystem=MobileDB.FileSystem.PhysicalFileSystem;path=C:\Development\database";

            var connectionString = _contextBuilder.ConnectionString;

            connectionString.Should().NotBeEmpty();
            connectionString.Should().Be(expectedConnectionString);
        }

        [Fact(DisplayName = "Build method should return the correct instance of the supplied context class. ")]
        public void ShouldGetInstanceOfContext()
        {
            MobileDB.AddFileSystem(typeof(PhysicalFileSystem));

            var instance = _contextBuilder.Build();

            instance.Should().NotBeNull();
            instance.Should().BeOfType<BsonContextWithSimpleIdentity>();
        }
    }
}
