using System.Collections.Generic;
using MobileDB.FileSystem.Contracts;

namespace MobileDB.Common
{
    public class ContextConfiguration
    {
        public List<EntityConfiguration> EntityConfigurations { get; set; }

        public IFileSystem FileSystem { get; set; }
    }
}