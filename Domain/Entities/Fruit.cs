namespace Domain.Entities
{
    public class Fruit : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty; // Allowed values: 'Kg', 'Dozen', 'Box', 'Man'

        public ICollection<Supply> Supplies { get; set; } = new List<Supply>();
    }
}
