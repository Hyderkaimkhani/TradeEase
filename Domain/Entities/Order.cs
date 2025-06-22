namespace Domain.Entities
{
    public class Order : AuditableEntity
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int CustomerId { get; set; }
        public int TruckId { get; set; }
        public int FruitId { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TotalPurchasePrice { get; set; } // Quantity*PurchasePrice
        public decimal TotalSellingPrice { get; set; } // Quantity*SellingPrice
        public decimal ProfitLoss { get; set; } // TotalSellingAmount-TotalPurchaseAmount                 
        public decimal AmountReceived { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? Deliverydate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending,Dispatched,Delivered,Canceled
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid,Partial,Paid
        public int? TruckAssignmentId { get; set; }
        public string? Notes { get; set; }

        public Customer Customer { get; set; }
        public Truck Truck { get; set; }
        public Fruit Fruit { get; set; }
        public TruckAssignment? TruckAssignment { get; set; }
    }
}
