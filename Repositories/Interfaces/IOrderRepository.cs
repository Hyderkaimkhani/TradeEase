using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;

namespace Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order entity);

        Task<Order?> GetOrder(int id);

        Task<List<Order>> GetOrdersByCustomer(int customerId, string? paymentStatus = null);

        Task<List<Order>> GetUnpaidOrders(int? customerId);

        Task<PaginatedResponseModel<Order>> GetOrders(FilterModel filter);

        Task<List<Order>> GetUnbilledOrders(int customerId, DateTime from, DateTime to);

    }
}
