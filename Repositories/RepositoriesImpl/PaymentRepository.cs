using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
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
            var payment = await _context.Set<Payment>().Include(p => p.PaymentAllocations).Include(p=>p.Customer).Include(p=>p.Account).FirstOrDefaultAsync(x => x.Id == id);
            return payment;
        }

        public async Task<PaginatedResponseModel<Payment>> GetPayments(FilterModel filter)
        {
            var query = _context.Set<Payment>().AsNoTracking();

            if (filter.EntityId.HasValue)
                query = query.Where(p => p.EntityId == filter.EntityId);

            if (filter.FromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(p => p.PaymentDate <= filter.ToDate.Value);


            var totalCount = await query.CountAsync();

            var payments = await query
                .Include(s => s.Customer)
                .OrderByDescending(s => s.PaymentDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseModel<Payment>
            {
                Model = payments,
                TotalCount = totalCount
            };
        }

        public async Task<List<Payment>> GetUnallocatedPayments(int customerId)
        {
            var unallocatedPayments = await _context.Payment.Include(p=>p.PaymentAllocations)
                .Where(p => p.EntityId == customerId && p.IsActive)
                .Where(p => (p.PaymentAllocations.Sum(a => (decimal?)a.AllocatedAmount) ?? 0m) < p.Amount)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();

            return unallocatedPayments;
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

        public async Task<List<PaymentAllocation>> GetPaymentAllocation(int paymentId)
        {
            var paymentAllocations = await _context.PaymentAllocation.Where(x => x.IsActive && x.PaymentId == paymentId).ToListAsync();
            return paymentAllocations;
        }
    }
}
