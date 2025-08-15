using Domain.Entities;

namespace Domain.Models.ResponseModel
{
    public class PaymentResponseModel
    {
        public int Id { get; set; }
        public string TransactionFlow { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string PaymentBy { get; set; } = string.Empty;

        public List<PaymentAllocationResponseModel> PaymentAllocations { get; set; }
    }
}
