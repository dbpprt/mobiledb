using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileDB.Common;

namespace MobileDB.FileSystem.Contracts
{
    public abstract class FileSystemBase
    {
        protected readonly ConnectionString ConnectionString;

        public FileSystemBase(
            ConnectionString connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
