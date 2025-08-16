using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models.RequestModel
{
    public class AccountStatementRequestModel
    {
        public int? AccountId { get; set; }

        [JsonIgnore]
        public int CompanyId { get; set; }

        [Required]
        public DateTime FromDate { get; set; } = DateTime.MinValue;
        
        [Required]
        public DateTime ToDate { get; set; } = DateTime.UtcNow;
        
        public int? EntityId { get; set; } // Optional: Filter by customer/supplier
        
        public string? TransactionType { get; set; } // Optional: Filter by transaction type
        
        public AccountStatementRequestModel()
        {
            FromDate = DateTime.Today.AddMonths(-1);
            ToDate = DateTime.Today;
        }
    }
} 