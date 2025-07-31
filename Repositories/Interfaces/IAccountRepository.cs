using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;

namespace Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> AddAccount(Account entity);
        Task<Account?> GetAccount(int id);
        Task<Account?> GetAccount(string name);
        Task<Account?> GetAccountWithDetails(int id);
        Task<PaginatedResponseModel<Account>> GetAccounts(FilterModel filter);
    }
}
