namespace MobileDB.Stores.Contracts
{
    public interface IStatefulStore
    {
        void Release();

        void EnsureInitialized();
    }
}