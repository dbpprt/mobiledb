using System.IO;
using MobileDB.FileSystem.Contracts;

namespace MobileDB.Common
{
    public static class FileAccessUtilities
    {
        public static FileAccess ToFileAccess(this DesiredFileAccess desiredFileAccess)
        {
            return (FileAccess) desiredFileAccess;
        }

        public static DesiredFileAccess ToDesiredFileAccess(this FileAccess desiredFileAccess)
        {
            return (DesiredFileAccess) desiredFileAccess;
        }
    }
}
