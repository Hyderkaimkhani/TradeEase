using Domain.Entities;
using Domain.Models.RequestModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Repositories.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> AddBill(Bill entity);
        Task<Bill?> GetBill(int id);
        Task<List<Bill>> GetBills(int? entityId, string? entityType);
        Task<bool> UpdateBill(Bill entity);
        Task<bool> DeleteBill(int id);
    }
}
