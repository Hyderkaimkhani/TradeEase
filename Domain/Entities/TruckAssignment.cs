namespace Domain.Entities
{
    public class TruckAssignment
    {
        public int Id { get; set; }
        public int TruckId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string? Notes { get; set; }
        public Truck Truck { get; set; }
        public ICollection<Supply> Supplies { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
