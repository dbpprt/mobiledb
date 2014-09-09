using System.Reflection;

namespace MobileDB.Tests.Common
{
    public static class ReflectionHelper
    {
        public static void ClearInternalContextCaches()
        {
            var type = typeof (DbContext);
            var cacheField = type.GetField("CachedContextConfigurations", BindingFlags.NonPublic | BindingFlags.Static);
            var cache = cacheField.GetValue(null);
            cache.GetType().GetMethod("Clear").Invoke(cache, null);
        }
    }
}