using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Data;

namespace Repositories.RepositoriesImpl
{
    public class AccountTransactionRepository : IAccountTransactionRepository
    {
        private readonly Context.Context context;

        public AccountTransactionRepository(Context.Context context)
        {
            this.context = context;
        }

        public async Task<AccountTransaction> AddTransaction(AccountTransaction entity)
        {
            var accountTransaction = context.AccountTransaction.Add(entity);
            return await Task.FromResult(accountTransaction.Entity!);
        }

        public async Task<decimal> GetAccountBalanceAsOfDate(int accountId, DateTime asOfDate)
        {
            var balance =  await context.AccountTransaction.Where(t => t.AccountId == accountId &&
                t.IsActive &&
                t.TransactionDate <= asOfDate)
                .SumAsync(t => t.TransactionDirection == "Debit" ? t.Amount : -t.Amount);

            return balance;
        }

        public Task<AccountStatementResponseModel> GetAccountStatement(AccountStatementRequestModel request)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountTransaction?> GetTransaction(int id)
        {
            var result = await context.AccountTransaction.FirstOrDefaultAsync(b => b.Id == id);
           
            return result;
        }

        public async Task<AccountTransaction?> GetTransactionDetail(int id)
        {
            var result = await context.AccountTransaction.Include(a => a.Account).Include(a => a.ToAccount).Include(a => a.Customer)
                .FirstOrDefaultAsync(b => b.Id == id);

            return result;
        }

        public async Task<AccountTransaction?> GetTransaction(string referenceType, int referenceId)
        {
            var result = await context.AccountTransaction
                .FirstOrDefaultAsync(t => t.ReferenceType == referenceType && t.ReferenceId == referenceId);

            return result;
        }

        public async Task<PaginatedResponseModel<AccountTransaction>> GetTransactions(FilterModel filter)
        {
            var query = context.AccountTransaction.AsNoTracking();

            if (filter.EntityId !=0)
                query = query.Where(a=>a.EntityId == filter.EntityId );

            if (filter.AccountId!=0)
                query = query.Where(a => a.AccountId == filter.AccountId);

            if (filter.FromDate.HasValue)
                query = query.Where(b => b.TransactionDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(b => b.TransactionDate <= filter.ToDate.Value);

            var totalCount = await query.CountAsync();

            var accountTransactions = await query.Include(a=>a.Account).Include(a => a.ToAccount).Include(a => a.Customer)
                .OrderByDescending(b => b.TransactionDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseModel<AccountTransaction>
            {
                Model = accountTransactions,
                TotalCount = totalCount
            };
        }

        //public async Task<AccountStatementResponseModel> GetAccountStatementAsync(AccountStatementRequestModel request, int companyId)
        //{
        //    using var connection = context.CreateConnection();

        //    // Get account details
        //    var accountSql = @"
        //        SELECT Id, Name, Type, OpeningBalance, CurrentBalance
        //        FROM Account 
        //        WHERE Id = @AccountId AND CompanyId = @CompanyId AND IsActive = 1";

        //    var account = await connection.QueryFirstOrDefaultAsync<Account>(accountSql, new { request.AccountId, CompanyId = companyId });
        //    if (account == null)
        //        return null;

        //    // Get opening balance as of fromDate
        //    var openingBalanceSql = @"
        //        SELECT ISNULL(SUM(CASE WHEN TransactionDirection = 'Debit' THEN Amount ELSE -Amount END), 0)
        //        FROM AccountTransaction 
        //        WHERE AccountId = @AccountId AND CompanyId = @CompanyId AND IsActive = 1 
        //        AND TransactionDate < @FromDate";

        //    var openingBalance = await connection.ExecuteScalarAsync<decimal>(openingBalanceSql, 
        //        new { request.AccountId, CompanyId = companyId, request.FromDate });

        //    // Get transactions
        //    var transactionsSql = @"
        //        SELECT 
        //            at.Id, at.CompanyId, at.AccountId, a.Name as AccountName, at.TransactionType, 
        //            at.TransactionDirection, at.Amount,
        //            CASE WHEN at.TransactionDirection = 'Debit' THEN at.Amount ELSE -at.Amount END as SignedAmount,
        //            at.TransactionDate, at.PaymentMethod, at.Notes, at.EntityId, c.Name as EntityName,
        //            at.ReferenceType, at.ReferenceId, at.Category, at.Party, at.ToAccountId, 
        //            ta.Name as ToAccountName, at.IsActive, at.CreatedDate
        //        FROM AccountTransaction at
        //        LEFT JOIN Account a ON at.AccountId = a.Id
        //        LEFT JOIN Account ta ON at.ToAccountId = ta.Id
        //        LEFT JOIN Customer c ON at.EntityId = c.Id
        //        WHERE at.AccountId = @AccountId AND at.CompanyId = @CompanyId AND at.IsActive = 1
        //        AND at.TransactionDate BETWEEN @FromDate AND @ToDate";

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@AccountId", request.AccountId);
        //    parameters.Add("@CompanyId", companyId);
        //    parameters.Add("@FromDate", request.FromDate);
        //    parameters.Add("@ToDate", request.ToDate);

        //    if (request.EntityId.HasValue)
        //    {
        //        transactionsSql += " AND at.EntityId = @EntityId";
        //        parameters.Add("@EntityId", request.EntityId.Value);
        //    }

        //    if (!string.IsNullOrEmpty(request.TransactionType))
        //    {
        //        transactionsSql += " AND at.TransactionType = @TransactionType";
        //        parameters.Add("@TransactionType", request.TransactionType);
        //    }

        //    transactionsSql += " ORDER BY at.TransactionDate, at.Id";

        //    var transactions = await connection.QueryAsync<AccountTransactionResponseModel>(transactionsSql, parameters);

        //    // Calculate running balance
        //    var runningBalance = openingBalance;
        //    var transactionsWithBalance = new List<AccountTransactionResponseModel>();

        //    foreach (var transaction in transactions)
        //    {
        //        runningBalance += transaction.SignedAmount;
        //        transaction.SignedAmount = transaction.SignedAmount; // Already calculated in SQL
        //        transactionsWithBalance.Add(transaction);
        //    }

        //    var statement = new AccountStatementResponseModel
        //    {
        //        AccountId = account.Id,
        //        AccountName = account.Name,
        //        AccountType = account.Type,
        //        FromDate = request.FromDate,
        //        ToDate = request.ToDate,
        //        OpeningBalance = openingBalance,
        //        ClosingBalance = runningBalance,
        //        Transactions = transactionsWithBalance.ToList()
        //    };

        //    return statement;
        //}

    }
}
