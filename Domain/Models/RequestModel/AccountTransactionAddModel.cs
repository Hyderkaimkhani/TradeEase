using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models.RequestModel
{
    public class AccountTransactionAddModel
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression("Expense|Income|Transfer", ErrorMessage = "TransactionType must be Expense, Income, or Transfer.")]
        public string TransactionType { get; set; } // Payment, Expense, Income, Transfer, Adjustment, Order, Supply

        [MaxLength(10)]
        [JsonIgnore]
        public string TransactionDirection { get; set; } // Debit, Credit

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public DateTime? TransactionDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Cash, Bank Transfer, Mobile Payment

        public string? Notes { get; set; }

        public int? EntityId { get; set; } // Customer/Supplier

        [MaxLength(20)]
        [JsonIgnore]
        public string? ReferenceType { get; set; } // Order, Supply, Expense, Payment, Transfer

        [JsonIgnore]
        public int? ReferenceId { get; set; }

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