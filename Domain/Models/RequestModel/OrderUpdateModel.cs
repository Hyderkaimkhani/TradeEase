using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class OrderUpdateModel
    {
        public int Id { get; set; }

        [Required]
        public int FruitId { get; set; }

        [Required]
        public string TruckNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SellingPrice must be at least 1")]
        public decimal SellingPrice { get; set; }
        //public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? DeliveryDate { get; set; }

        [RegularExpression("^(Pending|Dispatched|Delivered|Canceled)$", ErrorMessage = "Invalid Status")]
        public string? Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
