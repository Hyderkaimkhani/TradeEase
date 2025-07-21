using Domain.Entities;
using Domain.Models;

namespace Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order entity);

        Task<Order?> GetOrder(int id);

        Task<List<Order>> GetOrdersByCustomer(int customerId, string? paymentStatus = null);

        Task<List<Order>> GetUnpaidOrders(int? customerId);

        Task<PaginatedResponseModel<Order>> GetOrders(int page, int pageSize, int? fruitId, int? customerId);

        Task<List<Order>> GetUnbilledOrders(int customerId, DateTime from, DateTime to);

    }
}
