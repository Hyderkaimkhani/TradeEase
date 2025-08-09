using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> AddAccount(Account entity);
        Task<Account?> GetAccount(int id);
        Task<Account?> GetAccount(string name);
        Task<Account?> GetAccountWithDetails(int id);
        Task<Account?> GetAccountReceivable();
        Task<Account?> GetAccountPayable();

        Task<PaginatedResponseModel<Account>> GetAccounts(FilterModel filter);

        Task<List<Account>> GetAccountsByType(string type);

        Task<List<DropDownModel>> GetAccounts();
    }
}
