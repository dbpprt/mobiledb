using System.Linq;

namespace MobileDB.Contracts
{
    public interface IEntitySet<TEntity> where TEntity : new()
    {
        void Add(TEntity entity);

        void Remove(TEntity entity);

        void RemoveById(object key);

        TEntity FindById(object key);

        void Update(TEntity entity);

        IQueryable<TEntity> AsQueryable();

        void Release();

        int Count();
    }
}