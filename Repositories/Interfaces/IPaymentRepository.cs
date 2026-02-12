using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;

namespace Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> AddPayment(Payment entity);

        Task<Payment?> GetPayment(int id);

        Task<PaginatedResponseModel<Payment>> GetPayments(FilterModel filterModel);

        Task<List<Payment>> GetUnallocatedPayments(int customerId);

        Task<PaymentAllocation> AddPaymentAllocation(PaymentAllocation entity);

        Task<List<PaymentAllocation>> GetPaymentAllocation(string referenctType, int referenceId);

        Task<List<PaymentAllocation>> GetPaymentAllocation(int transactionId);
    }
}
