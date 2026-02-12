using System.Text.Json.Serialization;

namespace Domain.Models.ResponseModel
{
    public class AccountTransactionResponseModel
    {
        public int Id { get; set; }

        [JsonIgnore]
        public int CompanyId { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public string TransactionType { get; set; }
        public string TransactionDirection { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }

        [JsonIgnore]
        public string ReferenceType { get; set; }

        [JsonIgnore]
        public int ReferenceId { get; set; }
        public string? Category { get; set; }
        public string? Party { get; set; }
        public int? ToAccountId { get; set; }
        public string? ToAccountName { get; set; }

        [JsonIgnore]
        public bool IsActive { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        
        public AccountTransactionResponseModel()
        {
            AccountName = string.Empty;
            TransactionType = string.Empty;
            TransactionDirection = string.Empty;
            ReferenceType = string.Empty;
        }
    }
} 