using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Domain.Models;

namespace Services.Interfaces
{
    public interface ISupplyService
    {
        Task<ResponseModel<SupplyResponseModel>> AddSupply(SupplyAddModel requestModel);

        Task<ResponseModel<SupplyResponseModel>> GetSupply(int id);

        Task<ResponseModel<List<SupplyResponseModel>>> GetAllSupplies();

        Task<PaginatedResponseModel<SupplyResponseModel>> GetSupplies(int page, int pageSize, int? fruitId, int? supplierId);

        Task<ResponseModel<SupplyResponseModel>> UpdateSupply(SupplyUpdateModel requestModel);

        Task<ResponseModel<bool>> DeleteSupply(int id);

        Task<ResponseModel<List<SupplyResponseModel>>> GetUnassignedSupplies(string? truckNumber);
    }
}
