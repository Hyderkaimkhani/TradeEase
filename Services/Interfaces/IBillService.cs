using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBillService
    {
        Task<ResponseModel<BillResponseModel>> AddBill(BillAddRequestModel model);
        Task<ResponseModel<BillResponseModel>> GetBill(int id);
        Task<PaginatedResponseModel<BillResponseModel>> GetBills(BillFilterModel filterModel);
        Task<ResponseModel<BillResponseModel>> UpdateBill(int id, BillAddRequestModel model);
        Task<ResponseModel<bool>> DeleteBill(int id);
        Task UpdateBillPaymentStatus(List<int> billIds);
    }
}
