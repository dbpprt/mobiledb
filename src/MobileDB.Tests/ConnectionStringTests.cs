using System;
using FluentAssertions;
using MobileDB.Common;
using MobileDB.Exceptions;
using Xunit;

namespace MobileDB.Tests
{
    public  class ConnectionStringTests
    {
        private readonly ConnectionString _connectionStringInstance;
        private const string ConnectionString = @"filesystem=MobileDB.FileSystem.PhysicalFileSystem;path=C:\Development\database";

        public ConnectionStringTests()
        {
            _connectionStringInstance = new ConnectionString(ConnectionString);
        }

        [Fact(DisplayName = "GetPart should return the path of string.")]
        public void ShouldGetPathPartOfConnectionstring()
        {
            const string expectedPath = @"C:\Development\database";

            var path = _connectionStringInstance.GetPart("path");

            path.Should().Be(expectedPath);
        }

        [Fact(DisplayName = "GetPart should return the fileSystem of supplied connection string.")]
        public void ShouldGetFileSystemPartOfConnectionstring()
        {
            const string expectedFileSystem = @"MobileDB.FileSystem.PhysicalFileSystem";

            var fileSystem = _connectionStringInstance.GetPart("filesystem");

            fileSystem.Should().NotBeEmpty();
            fileSystem.Should().Be(expectedFileSystem);
        }

        [Fact(DisplayName = "Should Throw InvalidConnectionStringException if key is not found.")]
        public void ShouldThrowExceptionIfKeyIsNotFound()
        {
            try
            {
                _connectionStringInstance.GetPart("filesystem");
            }
            catch (Exception exception)
            {
                exception.Should().BeOfType<InvalidConnectionStringException>();
            }
        }

        [Fact(DisplayName = "ToString Should return complete path.")]
        public void ShouldReturnConnectionString()
        {

            _connectionStringInstance.ToString().Should().Be(ConnectionString);
        }
    }
}
