using System.Linq;

namespace MobileDB.Stores.Contracts
{
    public interface IQueryableStore
    {
        IQueryable<T> AsQueryable<T>();
    }
}