using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.RepositoriesImpl
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly Context.Context _context;

        internal PaymentRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<Payment> AddPayment(Payment entity)
        {
            var payment = _context.Payment.Add(entity);
            return await Task.FromResult(payment.Entity!);
        }

        public async Task<Payment?> GetPayment(int id)
        {
            var payment = await _context.Set<Payment>().Include(p => p.PaymentAllocations).Include(p=>p.Customer).FirstOrDefaultAsync(x => x.Id == id);
            return payment;
        }

        public async Task<PaginatedResponseModel<Payment>> GetPayments(int page, int pageSize, int? entityId, DateTime? paymentDate = null)
        {
            var query = _context.Set<Payment>().AsNoTracking();

            if (paymentDate.HasValue)
                query = query.Where(x => x.PaymentDate == paymentDate.Value);

            if (entityId.HasValue)
                query = query.Where(x => x.EntityId == entityId.Value);

            var totalCount = await query.CountAsync();

            var payments = await query
                .Include(s => s.Customer)
                .OrderByDescending(s => s.PaymentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponseModel<Payment>
            {
                Model = payments,
                TotalCount = totalCount
            };
        }

        public async Task<PaymentAllocation> AddPaymentAllocation(PaymentAllocation entity)
        {
            var paymentAllocation = _context.PaymentAllocation.Add(entity);
            return await Task.FromResult(paymentAllocation.Entity!);
        }

        public async Task<List<PaymentAllocation>> GetPaymentAllocation(string referenctType ,int referenceId)
        {
            var paymentAllocations = await _context.PaymentAllocation.Where(x=> x.IsActive && x.ReferenceType == referenctType && x.ReferenceId == referenceId ).ToListAsync();
            return paymentAllocations;
        }
    }
}
