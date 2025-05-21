using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Domain.Models;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ResponseModel<PaymentResponseModel>> AddPayment(PaymentAddModel requestModel);

        Task<ResponseModel<PaymentResponseModel>> UpdatePayment(PaymentUpdateModel requestModel);

        Task<ResponseModel<PaymentResponseModel>> GetPayment(int id);

        Task<PaginatedResponseModel<PaymentResponseModel>> GetPayments(int page, int pageSize, int? customerId);

        Task<ResponseModel<bool>> DeletePayment(int paymentId);
    }
}
