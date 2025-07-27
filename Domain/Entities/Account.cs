using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Account : AuditableEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Type { get; set; } = null!; // Cash, Bank, Wallet, Other

        [MaxLength(50)]
        public string? AccountNumber { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        public decimal OpeningBalance { get; set; } = 0;

        public decimal CurrentBalance { get; set; }

    }

}
