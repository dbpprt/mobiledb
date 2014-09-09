namespace MobileDB.Common.Factory
{
    public static class ContextFactory
    {
        public static ContextBuilder<T> Create<T>()
        {
            return new ContextBuilder<T>();
        }
    }
}