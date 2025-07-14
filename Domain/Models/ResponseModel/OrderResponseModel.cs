namespace Domain.Models.ResponseModel
{
    public class OrderResponseModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int TruckId { get; set; }
        public int FruitId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TotalPurchasePrice { get; set; }
        public decimal TotalSellingPrice { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal AmountReceived { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public int? TruckAssignmentId { get; set; }
        public string? Notes { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string TruckNumber { get; set; } = string.Empty;
        public string FruitName { get; set; } = string.Empty;
    }
}
