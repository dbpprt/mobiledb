using System;
using System.Threading.Tasks;

namespace MobileDB.Contracts
{
    public interface IDbContext
    {
        IEntitySet<T> Set<T>() where T : new();
        object Set(Type entityType);
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}