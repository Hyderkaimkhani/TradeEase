using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public abstract class AuditableEntity
    {
        public virtual int? CompanyId { get; set; }
        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
