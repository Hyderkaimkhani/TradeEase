namespace Domain.Entities
{
    public class Supply : AuditableEntity
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int FruitId { get; set; }
        public int? TruckId { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";
        public DateTime SupplyDate { get; set; }
        public int? TruckAssignmentId { get; set; }
        public string? Notes { get; set; }
        public Customer Supplier { get; set; }
        public Fruit Fruit { get; set; }
        public Truck Truck { get; set; }
        public TruckAssignment TruckAssignment { get; set; }
    }
}
