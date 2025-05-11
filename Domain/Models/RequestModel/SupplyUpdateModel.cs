using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class SupplyUpdateModel
    {
        public int Id { get; set; }

        //public int SupplierId { get; set; }

        //[Required]
        //public int FruitId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public decimal Quantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Purchase Price must be at least 1")]
        public decimal PurchasePrice { get; set; }

        //[Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive value.")]
        //public decimal AmountPaid { get; set; }
        public DateTime SupplyDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string TruckNumber { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}
