using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class AccountTransaction : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required, MaxLength(20)]
        public string TransactionType { get; set; } = null!; // Payment, Order, etc.

        [Required]
        public string TransactionDirection { get; set; } = string.Empty; // 'C' = Credit, 'D' = Debit

        [Required]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [MaxLength(20)]
        public string? ReferenceType { get; set; } // Order, Supply, Expense,Income
        public int? ReferenceId { get; set; }

        public int? EntityId { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }   // Rent, Salary, Utilities.Bonus, Commission, etc

        [MaxLength(100)]
        public string? Party { get; set; }      // PaidTo or ReceivedFrom

        public int? ToAccountId { get; set; } // For transfers

        public string? Notes { get; set; }

    }

}
