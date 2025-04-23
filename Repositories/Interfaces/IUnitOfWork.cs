using System;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        ITokenRepository TokenRepository { get; }
        IAdminRepository AdminRepository { get; }

        void DiscardChanges();

        Task<bool> SaveChangesAsync(bool overwriteDbChangesInCaseOfConcurrentUpdates = true);
    }
}
