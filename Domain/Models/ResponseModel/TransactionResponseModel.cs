using System.Text.Json.Serialization;

namespace Domain.Models.ResponseModel
{
    public class TransactionResponseModel
    {
        public int Id { get; set; }

        [JsonIgnore]
        public int CompanyId { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionDirection { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        [JsonIgnore]
        public decimal SignedAmount { get; set; } // Positive for Debit, Negative for Credit
        public DateTime TransactionDate { get; set; }
        public string? PaymentMethod { get; set; }
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }

        [JsonIgnore]
        public string ReferenceType { get; set; } = string.Empty;

        [JsonIgnore]
        public int ReferenceId { get; set; }
        public string? Category { get; set; }
        public string? Party { get; set; }
        public int? ToAccountId { get; set; }
        public string? ToAccountName { get; set; }

        public decimal RunningBalance { get; set; } // Balance after this transaction

        [JsonIgnore]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        public decimal? Quantity { get; set; } // For transactions involving quantities, e.g., inventory or orders

        public decimal? Price { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
