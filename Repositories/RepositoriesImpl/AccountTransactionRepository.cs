using Dapper;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.Interfaces;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            var balance = await context.AccountTransaction.Where(t => t.AccountId == accountId &&
                t.IsActive &&
                t.TransactionDate <= asOfDate)
                .SumAsync(t => t.TransactionDirection == "Debit" ? t.Amount : -t.Amount);

            return balance;
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

        public async Task<List<AccountTransaction>> GetTransactionsByReference(string referenceType, int referenceId)
        {
            var result = await context.AccountTransaction.Where(t => t.ReferenceType == referenceType && t.ReferenceId == referenceId).ToListAsync();

            return result;
        }

        public async Task<PaginatedResponseModel<AccountTransaction>> GetTransactions(FilterModel filter)
        {
            var query = context.AccountTransaction.Where(x => x.IsActive && x.TransactionType != "Payment" && x.TransactionType != "Order" && x.TransactionType != "Supply" && x.TransactionType != "Adjustment").AsNoTracking();

            if (filter.EntityId.HasValue)
                query = query.Where(a => a.EntityId == filter.EntityId);

            if (filter.AccountId.HasValue)
                query = query.Where(a => a.AccountId == filter.AccountId);

            if (!string.IsNullOrEmpty(filter.TransactionType))
                query = query.Where(b => b.TransactionType == filter.TransactionType);

            if (filter.FromDateUTC.HasValue)
                query = query.Where(b => b.TransactionDate >= filter.FromDateUTC.Value);

            if (filter.ToDateUTC.HasValue)
                query = query.Where(b => b.TransactionDate <= filter.ToDateUTC.Value);

            var totalCount = await query.CountAsync();

            var accountTransactions = await query.Include(a => a.Account).Include(a => a.ToAccount).Include(a => a.Customer)
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

        public async Task<List<UnallocatedTransaction>> GetUnallocatedTransactions(int entityId, int accountId, string transactionDirection, string referenceType)
        {
            var query =
                from at in context.AccountTransaction.AsNoTracking()
                where at.AccountId == accountId
                      && at.EntityId == entityId
                      && at.TransactionDirection == transactionDirection
                      && at.ReferenceType == referenceType
                join pa in context.PaymentAllocation
                    on at.Id equals pa.PaymentId into paGroup /// Fix to use TransactionId instead of PaymentId
                select new
                {
                    Transaction = at,
                    AllocatedSum = paGroup.Sum(p => (decimal?)p.AllocatedAmount) ?? 0
                };

            var result = await query
                .Where(x => (x.Transaction.Amount - x.AllocatedSum) > 0) // HAVING
                .OrderBy(x => x.Transaction.TransactionDate) // FIFO
                .Select(x => new UnallocatedTransaction
                {
                    TransactionId = x.Transaction.Id,
                    Amount = x.Transaction.Amount,
                    TransactionDate = x.Transaction.TransactionDate,
                    RemainingAmount = x.Transaction.Amount - x.AllocatedSum
                })
                .ToListAsync();

            return result;
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

        public async Task<AccountStatementResponseModel> GetAccountStatement(AccountStatementRequestModel request)
        {
            var result = new AccountStatementResponseModel();

            // Grab the connection EF Core is already using
            using var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            // Set up parameters
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", request.CompanyId, DbType.Int32);
            parameters.Add("@FromDate", request.FromDateUTC, DbType.Date);
            parameters.Add("@ToDate", request.ToDateUTC, DbType.Date);
            parameters.Add("@AccountId", request.AccountId, DbType.Int32);
            parameters.Add("@EntityId", request.EntityId, DbType.Int32);
            parameters.Add("@TransactionType", request.TransactionType, DbType.String);

            // Call stored procedure and get both result sets
            using (var multi = await connection.QueryMultipleAsync(
                "sp_GetAccountStatement",
                parameters,
                commandType: CommandType.StoredProcedure))
            {
                // First result set: transactions
                var transactions = (await multi.ReadAsync<AccountTransactionResponseModel>()).ToList();
                result.Transactions = transactions;

                // Second result set: metadata
                var meta = await multi.ReadFirstOrDefaultAsync<StatementMetadata>();
                if (meta != null)
                {
                    result.OpeningBalance = meta.OpeningBalance;
                    result.ClosingBalance = meta.ClosingBalance;
                    result.StatementTitle = meta.StatementTitle;

                    if (request.EntityId == null)
                        result.AccountName = meta.StatementTitle;
                    else
                        result.EntityName = meta.StatementTitle?.Replace(" (AP/AR Statement)", "");
                }
            }

            return result;
        }

        public async Task<AccountStatementResponseModel> GetAccountStatementADONET(AccountStatementRequestModel request)
        {
            var result = new AccountStatementResponseModel();
            context.Database.GetDbConnection();
            // Set up parameters
            var parameters = new[]
            {
                new SqlParameter("@CompanyId", request.CompanyId),
                new SqlParameter("@FromDate", request.FromDateUTC),
                new SqlParameter("@ToDate", request.ToDateUTC),
                new SqlParameter("@AccountId", request.AccountId ?? (object)DBNull.Value),
                new SqlParameter("@EntityId", request.EntityId ?? (object)DBNull.Value),
                new SqlParameter("@TransactionType", request.TransactionType ?? (object)DBNull.Value)
            };

            // Execute the stored procedure for transactions
            var transactions = await context.Set<AccountTransactionResponseModel>()
                .FromSqlRaw("EXEC sp_GetAccountStatement @CompanyId, @FromDate, @ToDate, @AccountId, @EntityId, @TransactionType", parameters)
                .ToListAsync();

            result.Transactions = transactions;

            // Get the metadata (second result set)
            var metadata = await context.Set<StatementMetadata>()
                .FromSqlRaw("EXEC sp_GetAccountStatement @CompanyId, @FromDate, @ToDate, @AccountId, @EntityId, @TransactionType", parameters)
            .ToListAsync();


            if (metadata.FirstOrDefault() is StatementMetadata meta)
            {
                result.OpeningBalance = meta.OpeningBalance;
                result.ClosingBalance = meta.ClosingBalance;
                result.StatementTitle = meta.StatementTitle;

                if (request.EntityId == null)
                {
                    result.AccountName = meta.StatementTitle;
                }
                else
                {
                    result.EntityName = meta.StatementTitle?.Replace(" (AP/AR Statement)", "");
                }
            }

            return result;
        }

        // Metadata model for the second result set


    }
}
