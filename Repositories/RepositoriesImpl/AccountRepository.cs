using Domain.Entities;
using Domain.Models.RequestModel;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.RepositoriesImpl
{
    public class AccountRepository : IAccountRepository
    {

        private readonly Context.Context _context;

        public AccountRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<Account> AddAccount(Account entity)
        {
            _context.Account.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Account?> GetAccount(int id)
        {
            return await _context.Account
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Account?> GetAccountWithDetails(int id)
        {
            return await _context.Account
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PaginatedResponseModel<Account>> GetAccounts(FilterModel filter)
        {
            var query = _context.Account.AsNoTracking();

            var totalCount = await query.CountAsync();

            var Accounts = await query
                .OrderByDescending(b => b.CreatedDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseModel<Account>
            {
                Model = Accounts,
                TotalCount = totalCount
            };
        }
    }
}
