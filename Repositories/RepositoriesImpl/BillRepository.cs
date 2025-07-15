using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
        }

        public async Task<List<Bill>> GetBills(int? entityId, string? entityType)
        {
            var query = _context.Bill
                .Include(b => b.BillDetails)
                .Where(b => b.IsActive);

            if (entityId.HasValue)
                query = query.Where(b => b.EntityId == entityId.Value);

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(b => b.EntityType == entityType);

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateBill(Bill entity)
        {
            _context.Bill.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBill(int id)
        {
            var bill = await _context.Bill.FindAsync(id);
            if (bill == null) return false;
            bill.IsActive = false;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
