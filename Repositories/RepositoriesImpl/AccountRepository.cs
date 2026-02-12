using Domain.Entities;
using Domain.Models.RequestModel;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Domain.Models.ResponseModel;

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
            var account = _context.Account.Add(entity);
            return await Task.FromResult(account.Entity!);
        }

        public async Task<Account?> GetAccount(int id)
        {
            return await _context.Account
                .FirstOrDefaultAsync(b => b.Id == id);
        }


        public async Task<Account?> GetAccount(string name)
        {
            return await _context.Account
                .FirstOrDefaultAsync(b => b.Name == name);
        }

        public async Task<Account?> GetAccountWithDetails(int id)
        {
            return await _context.Account
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Account?> GetAccountReceivable()
        {
            return await _context.Account
                .FirstOrDefaultAsync(b => b.Type == "Receivable");
        }

        public async Task<Account?> GetAccountPayable()
        {
            return await _context.Account
                .FirstOrDefaultAsync(b => b.Type == "Payable");
        }

        public async Task<PaginatedResponseModel<Account>> GetAccounts(FilterModel filter)
        {
            var query = _context.Account.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.ReferenceNumber))
                query = query.Where(b => EF.Functions.Like(b.AccountNumber, $"%{filter.ReferenceNumber}%"));

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

        public async Task<List<Account>> GetAccountsByType(string type)
        {
            var accounts = await _context.Account.Where(x => x.Type == type).ToListAsync();

            return accounts;
        }

        public async Task<List<DropDownModel>> GetAccounts()
        {
            var excludedTypes = new[] { "Receivable", "Payable" };

            var accounts = await _context.Set<Account>()
                .AsNoTracking()
                .Where(a => a.IsActive && !excludedTypes.Contains(a.Type))
                .Select(a => new DropDownModel
                {
                    Key = a.Id,
                    Value = a.Name
                })
                .OrderBy(a => a.Value)
                .ToListAsync();

            return accounts;
        }

    }
}
