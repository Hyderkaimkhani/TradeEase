using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Repositories.Interfaces
{
    public interface IAccountTransactionRepository
    {
        Task<AccountTransaction> AddTransaction(AccountTransaction transaction);
        Task<AccountTransaction?> GetTransaction(int id);
        Task<AccountTransaction?> GetTransactionDetail(int id);
        Task<AccountTransaction?> GetTransaction(string referenceType, int referenceId);
        Task<PaginatedResponseModel<AccountTransaction>> GetTransactions(FilterModel filterModel);
        Task<AccountStatementResponseModel> GetAccountStatement(AccountStatementRequestModel request);
        Task<decimal> GetAccountBalanceAsOfDate(int accountId, DateTime asOfDate);
        Task<List<UnallocatedTransaction>> GetUnallocatedTransactions(int entityId, int accountId, string transactionDirection, string referenceType);

    }
}
