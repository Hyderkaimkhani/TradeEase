using Domain.Entities;
using Domain.Models;

namespace Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> AddPayment(Payment entity);

        Task<Payment?> GetPayment(int id);

        Task<PaginatedResponseModel<Payment>> GetPayments(int page, int pageSize, int? entityId, DateTime? paymentDate);

        Task<PaymentAllocation> AddPaymentAllocation(PaymentAllocation entity);

        Task<List<PaymentAllocation>> GetPaymentAllocation(string referenctType, int referenceId);
    }
}
