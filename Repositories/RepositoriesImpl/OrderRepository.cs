using Common;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.RepositoriesImpl
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly Context.Context _context;

        internal OrderRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<Order> AddOrder(Order entity)
        {
            var order = _context.Order.Add(entity);
            return await Task.FromResult(order.Entity!);
        }

        public async Task<Order?> GetOrder(int id)
        {
            var order = await _context.Set<Order>().FirstOrDefaultAsync(x => x.Id == id);
            return order;
        }

        public async Task<List<Order>> GetOrdersByCustomer(int customerId, string? paymentStatus = null)
        {
            var query = _context.Set<Order>().AsNoTracking().Where(x => x.CustomerId == customerId && x.IsActive);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(x => x.PaymentStatus == paymentStatus);

            var orders = await query
                .OrderByDescending(s => s.OrderDate).ToListAsync();


            return orders;
        }

        public async Task<List<Order>> GetUnpaidOrders(int? customerId)
        {
            var query = _context.Set<Order>().AsNoTracking().Where(x=> x.PaymentStatus != PaymentStatus.Paid.ToString() && x.IsActive);

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerId == customerId.Value);

            var orders = await query
                .OrderBy(s => s.OrderDate).ToListAsync();

            return orders;
        }

        public async Task<PaginatedResponseModel<Order>> GetOrders(int page, int pageSize, int? fruitId, int? customerId)
        {
            var query = _context.Set<Order>().AsNoTracking().Where(x => x.IsActive);

            if (fruitId.HasValue)
                query = query.Where(x => x.FruitId == fruitId.Value);

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerId == customerId.Value);

            var totalCount = await query.CountAsync();

            var orders = await query
                .Include(s => s.Fruit)
                .Include(s => s.Truck)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponseModel<Order>
            {
                Model = orders,
                TotalCount = totalCount
            };
        }
    }
}
