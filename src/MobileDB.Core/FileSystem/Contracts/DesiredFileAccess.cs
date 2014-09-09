using System;

namespace MobileDB.FileSystem.Contracts
{
    [Flags]
    public enum DesiredFileAccess
    {
        Read = 1,
        Write = 2,
        ReadWrite = Write | Read,
    }
}