using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.RepositoriesImpl
{
    public class AccountTransactionRepository : IAccountTransactionRepository
    {
        private readonly Context.Context _context;

        public AccountTransactionRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<AccountTransaction> AddAccountTransaction(AccountTransaction entity)
        {
            var accountTransaction = _context.AccountTransaction.Add(entity);
            return await Task.FromResult(accountTransaction.Entity!);
        }

        public async Task<AccountTransaction?> GetAccountTransaction(int id)
        {
            return await _context.AccountTransaction
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<AccountTransaction?> GetAccountTransactionWithDetails(int id)
        {
            return await _context.AccountTransaction
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PaginatedResponseModel<AccountTransaction>> GetAccountTransactions(FilterModel filter)
        {
            var query = _context.AccountTransaction.AsNoTracking();

            var totalCount = await query.CountAsync();

            var AccountTransactions = await query
                .OrderByDescending(b => b.CreatedDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseModel<AccountTransaction>
            {
                Model = AccountTransactions,
                TotalCount = totalCount
            };
        }
    }
}
