namespace Domain.Models.ResponseModel
{
    public class SupplyResponseModel
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int FruitId { get; set; }
        public int? TruckId { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime SupplyDate { get; set; }
        public int? TruckAssignmentId { get; set; }
        public string? Notes { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string Fruit { get; set; } = string.Empty;
        public string TruckNumber { get; set; } = string.Empty;
    }
}
