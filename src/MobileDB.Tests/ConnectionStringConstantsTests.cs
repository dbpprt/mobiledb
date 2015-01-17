using FluentAssertions;
using MobileDB.Common;
using Xunit;

namespace MobileDB.Tests
{
    public class ConnectionStringConstantsTests
    {
        [Fact(DisplayName = "Filesystem should return filesystem constant")]
        public void ShouldReturnSpecifiedFilesystem()
        {
            ConnectionStringConstants.Filesystem.Should().Be("filesystem");
        }

        [Fact(DisplayName = "Path should return path constant")]
        public void ShouldReturnSpecifiedPath()
        {
            ConnectionStringConstants.Path.Should().Be("path");
        }

        [Fact(DisplayName = "TupleSeperator should return semicolon")]
        public void ShouldReturnSpecifiedTupleSeperator()
        {
            ConnectionStringConstants.TupleSeperator.Should().Be(';');
        }

        [Fact(DisplayName = "SegmentSeperator should return equal")]
        public void ShouldReturnSpecifiedSegmentSeperator()
        {
            ConnectionStringConstants.SegmentSeperator.Should().Be('=');
        }

    }
}
