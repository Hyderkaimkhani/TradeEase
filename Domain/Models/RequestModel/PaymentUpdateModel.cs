using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class PaymentUpdateModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
    }
}
