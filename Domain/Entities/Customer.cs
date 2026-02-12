using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Customer : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? PaymentTerms { get; set; } = "Cash";

        //[Column(TypeName = "decimal(10,2)")]
        //public decimal TotalCredit { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal CreditBalance { get; set; } = 0;

        public Customer()
        {
            Name = string.Empty;
            Phone = string.Empty;
        }

    }

}
