using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.RepositoriesImpl
{
    public class BillRepository : IBillRepository
    {
        private readonly Context.Context _context;

        public BillRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<Bill> AddBill(Bill entity)
        {
            _context.Bill.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Bill?> GetBill(int id)
        {
            return await _context.Bill
                .Include(b => b.BillDetails)
                //.ThenInclude(x=>x.Order)
                //.Include(x=>x.Su)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Bill?> GetBillWithDetails(int id)
        {
            return await _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.BillDetails)    
                    .ThenInclude(bd=>bd.Order)
                    .ThenInclude(o => o.Fruit)
                .Include(b=>b.BillDetails)
                    .ThenInclude(bd=>bd.Supply)
                    .ThenInclude(s=>s.Fruit)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PaginatedResponseModel<Bill>> GetBills(FilterModel filter)
        {
            var query = _context.Bill.AsNoTracking().Where(b => b.IsActive);

            if (filter.EntityId.HasValue)
                query = query.Where(b => b.EntityId == filter.EntityId);

            if (filter.FromDate.HasValue)
                query = query.Where(b => b.FromDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(b => b.ToDate <= filter.ToDate.Value);

            if (!string.IsNullOrEmpty(filter.ReferenceNumber))
                query = query.Where(b => b.BillNumber == filter.ReferenceNumber);

            var totalCount = await query.CountAsync();

            var bills = await query
                .OrderByDescending(b => b.CreatedDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseModel<Bill>
            {
                Model = bills,
                TotalCount = totalCount
            };
        }

        public async Task<List<int>> GetBillIdsByReference(string referenceType, List<int> referenceIds)
        {
            return await _context.BillDetail
                .Where(b => b.ReferenceType == referenceType &&
                            ((referenceType == "Order" && referenceIds.Contains(b.OrderId.Value)) ||
                             (referenceType == "Supply" && referenceIds.Contains(b.SupplyId.Value))))
                .Select(b => b.BillId)
                .Distinct()
                .ToListAsync();
        }

    }
}
