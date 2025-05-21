using System.Data;

namespace Domain.Entities
{
    public class Payment : AuditableEntity
    {
        public int Id { get; set; }
        public string EntityType { get; set; } = string.Empty;// Customer / Supplier
        public int EntityId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
        public virtual Customer Customer { get; set; }
        public List<PaymentAllocation> PaymentAllocations { get; set; }
    }
}
