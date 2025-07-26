using Domain.Entities;

namespace Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> AddCompany(Company entity);
        Task<Company?> GetCompany(int id);
        Task<List<Company>> GetAllCompanies();
    }
}
