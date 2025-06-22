namespace Domain.Entities
{
    public class TruckAssignment
    {
        public int Id { get; set; }
        public int TruckId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string? Notes { get; set; }
        public Truck Truck { get; set; }
        public Supply Supply{ get; set; }
        public Order Order { get; set; }
    }
}
