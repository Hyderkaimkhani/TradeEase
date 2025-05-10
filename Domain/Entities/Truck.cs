namespace Domain.Entities
{
    public class Truck : AuditableEntity
    {
        public int Id { get; set; }

        public string TruckNumber { get; set; } = string.Empty;

        public decimal? Capacity { get; set; }

        public string? DriverName { get; set; }

        public string? DriverContact { get; set; }

        public string? Status { get; set; }

        public ICollection<Supply> Supplies { get; set; }
    }
}
