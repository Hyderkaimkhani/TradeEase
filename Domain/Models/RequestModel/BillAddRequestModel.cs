using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class BillAddRequestModel
    {
        [Required]
        [RegularExpression("^(Customer|Supplier)$", ErrorMessage = "Invalid EntityType")]
        public string EntityType { get; set; } = string.Empty;// Customer / Supplier

        [Required]
        public int EntityId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? DueDate { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? Notes { get; set; }
    }
}
