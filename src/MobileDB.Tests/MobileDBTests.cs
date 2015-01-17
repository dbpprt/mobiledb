using System.Linq;
using FluentAssertions;
using MobileDB.Contracts;
using MobileDB.FileSystem;
using MobileDB.Tests.Common;
using Xunit;

namespace MobileDB.Tests
{
    public class MobileDBTests
    {
        [Fact(DisplayName = "Default system should be MemoryFileSystem.")]
        public void ShouldReturnDefaultMemoryFileSystem()
        {
            var fileSystem = MobileDB.FileSystems().ToList();

            Assert.NotNull(fileSystem);
            fileSystem.Count.Should().BeGreaterOrEqualTo(1);
            var memoryFileSystem=  fileSystem.First(elem=> elem == typeof(MemoryFileSystem));
            memoryFileSystem.Should().Be<MemoryFileSystem>();
        }

        [Fact(DisplayName = "Add a new PhysicalFileSystem tp MobileDB.")]
        public void ShouldAddPhysicalFileSystem()
        {
            MobileDB.AddFileSystem(typeof(PhysicalFileSystem));
            var fileSystem = MobileDB.FileSystems().ToList();
            Assert.NotNull(fileSystem);
            fileSystem.Count.Should().Be(2);

            var memoryFileSystem = fileSystem.First(elem => elem == typeof(MemoryFileSystem));
            memoryFileSystem.Should().Be<MemoryFileSystem>();

            var physicalFileSystem = fileSystem.First(elem => elem == typeof (PhysicalFileSystem));
            physicalFileSystem.Should().Be<PhysicalFileSystem>();
        }

        [Fact(DisplayName = "Should return EntityValidator Instance.")]
        public void ShouldReturnEntityValidatorInstance()
        {
            var entityValidator = MobileDB.Validator;

            Assert.NotNull(entityValidator);
            entityValidator.Should().BeAssignableTo<IEntityValidator>();
        }


        [Fact(DisplayName = "Should be able to set IEntityValidator Instance.")]
        public void ShouldSetNewEntityValidatorInstance()
        {
            MobileDB.Validator = new SimpleEntityValidator();
            var entityValidator = MobileDB.Validator;

            Assert.NotNull(entityValidator);
            entityValidator.Should().BeAssignableTo<IEntityValidator>();
        }



    }
}
