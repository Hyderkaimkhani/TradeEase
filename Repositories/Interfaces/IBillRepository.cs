using Domain.Entities;
using Domain.Models.RequestModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Models;

namespace Repositories.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> AddBill(Bill entity);
        Task<Bill?> GetBill(int id);
        Task<PaginatedResponseModel<Bill>> GetBills(BillFilterModel filter);
        Task<bool> UpdateBill(Bill entity);
        Task<bool> DeleteBill(int id);

        Task<List<int>> GetBillIdsByReference(string referenceType, List<int> referenceIds);
    }
}
