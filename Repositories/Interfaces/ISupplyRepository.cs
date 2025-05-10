using Domain.Entities;
using Domain.Models;

namespace Repositories.Interfaces
{
    public interface ISupplyRepository
    {
        Task<Supply> AddSupply(Supply entity);

        Task<Supply?> GetSupply(int id);

        Task<List<Supply>> GetAllSupplies();

        Task<List<Supply>> GetSupplies(bool isActive);

        Task<PaginatedResponseModel<Supply>> GetSupplies(int page, int pageSize, int? fruitId, int? supplierId);

    }
}
