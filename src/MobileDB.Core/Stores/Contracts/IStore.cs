using System.Threading.Tasks;
using MobileDB.Common;
using MobileDB.FileSystem;

namespace MobileDB.Stores.Contracts
{
    public interface IStore
    {
        FileSystemPath Path { get; }

        int SaveChanges(ChangeSet changeSet);

        Task<int> SaveChangesAsync(ChangeSet changeSet);

        int Count();

        object FindById(object key);
    }
}