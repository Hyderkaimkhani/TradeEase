using Domain.Entities;

namespace Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> AddCompany(Company entity);
        Task<Company?> GetCompany(int id);
        Task<Company?> GetCompanyByName(string Name);
        Task<List<Company>> GetAllCompanies();
    }
}
