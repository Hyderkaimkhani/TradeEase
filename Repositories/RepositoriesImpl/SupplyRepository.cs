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

        public async Task<List<Supply>> GetUnAssignedSupplies(int truckId)
        {
            var supplies = await _context.Set<Supply>()
                .Where(x => x.TruckId == truckId && x.IsActive == true && x.TruckAssignmentId == null)
                .OrderByDescending(s => s.SupplyDate).ToListAsync();

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
            var supply = await _context.Set<Supply>().FirstOrDefaultAsync(x => x.Id == id);
            return supply;
        }
    }
}
