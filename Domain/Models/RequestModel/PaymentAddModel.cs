using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class PaymentAddModel
    {
        [Required]
        [RegularExpression("^(Customer|Supplier)$", ErrorMessage = "Invalid EntityType")]
        public string EntityType { get; set; } = string.Empty;// Customer / Supplier

        [Required]
        public int EntityId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
    }
}
