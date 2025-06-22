namespace Domain.Entities
{
    public class OrderSupplies
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int SupplyId { get; set; }

        public decimal QuantityUsed { get; set; }

        public Order Order { get; set; }

        public Supply Supply { get; set; }

    }
}
