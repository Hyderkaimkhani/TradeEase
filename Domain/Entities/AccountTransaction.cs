using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class AccountTransaction : AuditableEntity
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; } // Payment, Expense, Income, Transfer, Adjustment, Order, Supply
        
        [Required]
        [MaxLength(10)]
        public string TransactionDirection { get; set; } = "Credit"; // Debit, Credit
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Cash, Bank Transfer, Mobile Payment
        
        public string? Notes { get; set; }
        
        public int? EntityId { get; set; } // Customer/Supplier
        
        [Required]
        [MaxLength(20)]
        public string ReferenceType { get; set; } // Order, Supply, Expense, Payment, Transfer
        
        public int? ReferenceId { get; set; }
        
        [MaxLength(50)]
        public string? Category { get; set; } // Rent, Salary, Utilities, Bonus, Commission, etc.
        
        [MaxLength(100)]
        public string? Party { get; set; } // Paid To / Receive From

        public int? ToAccountId { get; set; } // Destination account for transfers
        
        // Navigation properties
        public virtual Account Account { get; set; }
        public virtual Account ToAccount { get; set; }
        public virtual Customer Customer { get; set; }

        public AccountTransaction()
        {
            TransactionType = string.Empty;
            ReferenceType = string.Empty;
        }
    }
}
