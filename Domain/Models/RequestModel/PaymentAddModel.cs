using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models.RequestModel
{
    public class PaymentAddModel
    {
        [RegularExpression("^(Debit|Credit)$", ErrorMessage = "Invalid TransactionDirection")]
        [JsonIgnore]
        public string TransactionDirection { get; set; } = string.Empty; // "Debit=>Incoming" or "Credit=>Outgoing"  

        [Required]
        [RegularExpression("^(Received|Paid)$", ErrorMessage = "Invalid TransactionType")]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        public int EntityId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
    }
}
