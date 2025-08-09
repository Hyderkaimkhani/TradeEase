using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Services.Interfaces
{
    public interface IAccountTransactionService
    {
        Task<ResponseModel<AccountTransactionResponseModel>> AddTransaction(AccountTransactionAddModel requestModel);
        Task<ResponseModel<AccountTransactionResponseModel>> GetTransaction(int id);
        Task<PaginatedResponseModel<AccountTransactionResponseModel>> GetTransactions(FilterModel filterModel);
        Task<ResponseModel<AccountStatementResponseModel>> GetAccountStatement(AccountStatementRequestModel requestModel);
        Task<ResponseModel<bool>> DeleteTransaction(int id);

        // Service methods to be called from other services
        Task<ResponseModel<bool>> RecordOrderTransaction(int orderId, int customerId, decimal amount, DateTime transactionDate);
        Task<ResponseModel<bool>> RecordSupplyTransaction(int supplyId, int supplierId, decimal amount, DateTime transactionDate);
        Task<ResponseModel<bool>> RecordPaymentTransaction(int paymentId, int entityId, decimal amount, string paymentMethod, int accountId, string transactionDirection, string party);
        Task<ResponseModel<bool>> RecordExpenseTransaction(string category, string party, decimal amount, int accountId);
        Task<ResponseModel<bool>> RecordIncomeTransaction(string category, string party, decimal amount, int accountId);

        void UpdateAccountBalance(Account account, AccountTransaction transaction);
    }
}