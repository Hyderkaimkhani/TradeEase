using System;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        ITokenRepository TokenRepository { get; }
        IAdminRepository AdminRepository { get; }
        ISupplyRepository SupplyRepository { get; }

        void DiscardChanges();

        Task<bool> SaveChangesAsync(bool overwriteDbChangesInCaseOfConcurrentUpdates = true);
    }
}
