namespace Domain.Entities
{
    public class PaymentAllocation : AuditableEntity
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string ReferenceType { get; set; } // Order or Supply
        public int ReferenceId { get; set; }
        public decimal AllocatedAmount { get; set; }
        public Payment Payment { get; set; }
    }
}
