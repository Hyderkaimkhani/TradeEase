using Common;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.RepositoriesImpl
{
    public class SupplyRepository : ISupplyRepository
    {
        private readonly Context.Context _context;

        internal SupplyRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<Supply> AddSupply(Supply entity)
        {
            var supply = _context.Supply.Add(entity);
            return await Task.FromResult(supply.Entity!);
        }

        public async Task<Supply> UpdateSupply(Supply entity)
        {
            var supply = _context.Supply.Update(entity);
            return await Task.FromResult(supply.Entity!);
        }

        public async Task<List<Supply>> GetAllSupplies()
        {
            var supplies = await _context.Set<Supply>().AsNoTracking()
                .Include(s => s.Fruit)
                .Include(s => s.Truck)
                .Include(s => s.Supplier)
                .OrderByDescending(s => s.SupplyDate).ToListAsync();
            return supplies;
        }

        public async Task<List<Supply>> GetSupplies(bool isActive)
        {
            var supplies = await _context.Set<Supply>()
                .Where(x => x.IsActive == isActive)
                .AsNoTracking()
                .Include(s => s.Fruit)
                .Include(s => s.Truck)
                .Include(s => s.Supplier)
                .OrderByDescending(s => s.SupplyDate).ToListAsync();

            return supplies;
        }

        public async Task<List<Supply>> GetUnAssignedSupplies(int? truckId)
        {
            var query = _context.Set<Supply>()
               .Where(x => x.IsActive == true && x.TruckAssignmentId == null);

            if (truckId.HasValue)
                query = query.Where(x => x.TruckId == truckId);

            var supplies = await query.OrderByDescending(s => s.SupplyDate).ToListAsync();

            return supplies;
        }

        public async Task<List<Supply>> GetUnAssignedSupplies(string? truckNumber = null)
        {
            var query = _context.Set<Supply>()
               .Where(x => x.IsActive == true && x.TruckAssignmentId == null);

            if (!string.IsNullOrEmpty(truckNumber))
                query = query.Where(x => x.TruckNumber == truckNumber);

            query = query.AsNoTracking().Include(x => x.Supplier);

            var supplies = await query.OrderByDescending(s => s.SupplyDate).ToListAsync();

            return supplies;
        }

        public async Task<List<Supply>> GetUnpaidSupplies(int? supplierId)
        {
            var query = _context.Set<Supply>().Where(x => x.PaymentStatus != PaymentStatus.Paid.ToString() && x.IsActive);

            if (supplierId.HasValue)
                query = query.Where(x => x.SupplierId == supplierId.Value);

            var supplies = await query
                .OrderBy(s => s.SupplyDate).ToListAsync();

            return supplies;
        }

        public async Task<PaginatedResponseModel<Supply>> GetSupplies(int page, int pageSize, int? fruitId, int? supplierId)
        {
            var query = _context.Set<Supply>().AsNoTracking().Where(x => x.IsActive);

            if (fruitId.HasValue)
                query = query.Where(x => x.FruitId == fruitId.Value);

            if (supplierId.HasValue)
                query = query.Where(x => x.SupplierId == supplierId.Value);

            var totalCount = await query.CountAsync();

            var supplies = await query
                .Include(s => s.Fruit)
                .Include(s => s.Truck)
                .Include(s => s.Supplier)
                .OrderByDescending(s => s.SupplyDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponseModel<Supply>
            {
                Model = supplies,
                TotalCount = totalCount
            };
        }

        public async Task<Supply?> GetSupply(int id)
        {
            var supply = await _context.Set<Supply>().Include(x => x.Truck).Include(x => x.Fruit).FirstOrDefaultAsync(x => x.Id == id);
            return supply;
        }

        public async Task<List<Supply>> GetSuppliesBySupplier(int supplierId, string? paymentStatus = null)
        {
            var query = _context.Set<Supply>().AsNoTracking().Where(x => x.SupplierId == supplierId && x.IsActive);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(x => x.PaymentStatus == paymentStatus);

            var supplies = await query
                .OrderByDescending(s => s.SupplyDate).ToListAsync();


            return supplies;
        }

        public async Task<List<Supply>> GetUnbilledSupplies(int customerId, DateTime from, DateTime to)
        {
            return await _context.Set<Supply>().AsNoTracking()
                .Where(supply => supply.IsActive &&
                                supply.SupplierId == customerId &&
                                supply.SupplyDate >= from &&
                                supply.SupplyDate <= to &&
                                supply.PaymentStatus != PaymentStatus.Paid.ToString())
                //&&
                //                !_context.Set<BillDetail>().Any(bd =>bd.SupplyId == supply.Id))
                .ToListAsync();
        }

        public async Task<Supply?> GetSupplyByTruckAssignmentId(int id)
        {
            var supply = await _context.Set<Supply>().FirstOrDefaultAsync(x => x.TruckAssignmentId == id);
            return supply;
        }
    }
}
