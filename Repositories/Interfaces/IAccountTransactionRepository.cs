using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;

namespace Repositories.Interfaces
{
    public interface IAccountTransactionRepository
    {
        Task<AccountTransaction> AddAccountTransaction(AccountTransaction entity);
        Task<AccountTransaction?> GetAccountTransaction(int id);
        Task<AccountTransaction?> GetAccountTransactionWithDetails(int id);
        Task<PaginatedResponseModel<AccountTransaction>> GetAccountTransactions(FilterModel filter);
    }
}
