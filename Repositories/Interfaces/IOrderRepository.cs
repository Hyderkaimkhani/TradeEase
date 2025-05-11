using Domain.Entities;
using Domain.Models;

namespace Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order entity);

        Task<Order?> GetOrder(int id);

        Task<PaginatedResponseModel<Order>> GetOrders(int page, int pageSize, int? fruitId, int? customerId);

    }
}
