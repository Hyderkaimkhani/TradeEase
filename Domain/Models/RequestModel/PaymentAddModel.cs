using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class PaymentAddModel
    {
        [Required]
        [RegularExpression("^(Credit|Debit)$", ErrorMessage = "Invalid TransactionDirection")]
        public string TransactionDirection { get; set; } = string.Empty; // "Credit=>Incoming" or "Debit=>Outgoing"  

        [Required]
        public int EntityId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
    }
}
