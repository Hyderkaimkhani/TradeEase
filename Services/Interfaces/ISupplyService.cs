using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Domain.Models;

namespace Services.Interfaces
{
    public interface ISupplyService
    {
        Task<ResponseModel<SupplyResponseModel>> AddSupply(SupplyAddRequest requestModel);

        Task<ResponseModel<SupplyResponseModel>> GetSupply(int id);

        Task<ResponseModel<List<SupplyResponseModel>>> GetAllSupplies();

        Task<PaginatedResponseModel<SupplyResponseModel>> GetSupplies(int page, int pageSize, int? fruitId, int? supplierId);

        Task<ResponseModel<SupplyResponseModel>> UpdateSupply(SupplyUpdateRequest requestModel);

        Task<ResponseModel<bool>> DeleteSupply(int id);
    }
}
