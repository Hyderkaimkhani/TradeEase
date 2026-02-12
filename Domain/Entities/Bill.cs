namespace Domain.Entities
{
    public class Bill : AuditableEntity
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? DueDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }

        public string PaymentStatus { get; set; } = "Unpaid"; // Allowed values: 'Unpaid', 'Partial', 'Paid'
        public DateTime? PaidDate { get; set; }

        public string? PdfPath { get; set; }
        public string? Notes { get; set; }
        public Customer? Customer { get; set; }
        public List<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    }
}
