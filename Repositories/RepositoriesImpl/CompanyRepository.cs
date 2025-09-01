using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.RepositoriesImpl
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly Context.Context _context;

        public CompanyRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<Company> AddCompany(Company entity)
        {
            var company =  _context.Company.Add(entity);
            return await Task.FromResult(company.Entity!);
        }

        public async Task<Company?> GetCompany(int id)
        {
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
            return company;
        }

        public async Task<Company?> GetCompanyByName(string Name)
        {
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Name == Name && c.IsActive);
            return company;
        }

        public async Task<List<Company>> GetAllCompanies()
        {
            return await _context.Company.Where(c => c.IsActive).ToListAsync();
        }
    }
}