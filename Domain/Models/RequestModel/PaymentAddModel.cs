using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models.RequestModel
{
    public class PaymentAddModel
    {
        [Required]
        [RegularExpression("^(Received|Paid)$", ErrorMessage = "Invalid TransactionFlow")]
        public string TransactionFlow { get; set; } = string.Empty;

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
