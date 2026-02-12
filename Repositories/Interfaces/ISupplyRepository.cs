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

        Task<List<Supply>> GetUnAssignedSupplies(int? truckId);

        Task<List<Supply>> GetUnAssignedSupplies(string? truckNumber = null);

        Task<PaginatedResponseModel<Supply>> GetSupplies(int page, int pageSize, int? fruitId, int? supplierId);

        Task<List<Supply>> GetUnpaidSupplies(int? supplierId);

        Task<List<Supply>> GetSuppliesBySupplier(int supplierId, string? paymentStatus = null);

        Task<List<Supply>> GetUnbilledSupplies(int customerId, DateTime from, DateTime to);

        Task<Supply?> GetSupplyByTruckAssignmentId(int id);


    }
}
