using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseModel<OrderResponseModel>> AddOrder(OrderAddModel requestModel);

        Task<ResponseModel<OrderResponseModel>> UpdateOrder(OrderUpdateModel requestModel);

        Task<ResponseModel<OrderResponseModel>> GetOrder(int id);

        Task<PaginatedResponseModel<OrderResponseModel>> GetOrders(FilterModel filter);

        Task<ResponseModel<bool>> DeleteOrder(int id);
    }
}
