using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Company : AuditableEntity
    {
        public int Id { get; set; }

        [NotMapped] // Tells EF to ignore this property
        public override int? CompanyId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Code { get; set; } = null!;

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        public byte[]? Logo { get; set; }  // Maps to VARBINARY(MAX)

        public int? MaxUsers { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(50)]
        public string? Timezone { get; set; }

        [MaxLength(10)]
        public string? CurrencySymbol { get; set; }

        [MaxLength(50)]
        public string? GSTNumber { get; set; }

    }


}
