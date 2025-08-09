using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class AccountTransactionAddModel
    {
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; } // Payment, Expense, Income, Transfer, Adjustment, Order, Supply
        
        [Required]
        [MaxLength(10)]
        public string TransactionDirection { get; set; } // Debit, Credit
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        public DateTime? TransactionDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Cash, Bank Transfer, Mobile Payment
        
        public string? Notes { get; set; }
        
        public int? EntityId { get; set; } // Customer/Supplier
        
        [Required]
        [MaxLength(20)]
        public string ReferenceType { get; set; } // Order, Supply, Expense, Payment, Transfer
        
        [Required]
        public int ReferenceId { get; set; }
        
        [MaxLength(50)]
        public string? Category { get; set; } // Rent, Salary, Utilities, Bonus, Commission, etc.
        
        [MaxLength(100)]
        public string? Party { get; set; } // Who was paid for expenses
        
        public int? ToAccountId { get; set; } // Destination account for transfers
        
        public AccountTransactionAddModel()
        {
            TransactionType = string.Empty;
            TransactionDirection = "Credit";
            ReferenceType = string.Empty;
        }
    }
} 