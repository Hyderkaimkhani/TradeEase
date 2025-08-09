using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class AccountStatementRequestModel
    {
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public DateTime FromDate { get; set; }
        
        [Required]
        public DateTime ToDate { get; set; }
        
        public int? EntityId { get; set; } // Optional: Filter by customer/supplier
        
        public string? TransactionType { get; set; } // Optional: Filter by transaction type
        
        public AccountStatementRequestModel()
        {
            FromDate = DateTime.Today.AddMonths(-1);
            ToDate = DateTime.Today;
        }
    }
} 