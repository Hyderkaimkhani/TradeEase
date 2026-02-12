using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Services.Interfaces
{
    public interface IBillService
    {
        Task<ResponseModel<BillResponseModel>> AddBill(BillAddRequestModel model);
        Task<ResponseModel<BillResponseModel>> GetBill(int id);
        Task<PaginatedResponseModel<BillResponseModel>> GetBills(FilterModel filterModel);
        Task<ResponseModel<BillResponseModel>> UpdateBill(BillUpdateRequestModel model);
        Task<ResponseModel<bool>> DeleteBill(int id);
        Task UpdateBillPaymentStatus(List<int> billIds);
    }
}
