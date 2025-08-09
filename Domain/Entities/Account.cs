using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Account : AuditableEntity
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Type { get; set; }
        
        [MaxLength(50)]
        public string? AccountNumber { get; set; }
        
        [MaxLength(100)]
        public string? BankName { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; } = 0;

        public Account()
        {
            Name = string.Empty;
            Type = string.Empty;
        }
    }
}
