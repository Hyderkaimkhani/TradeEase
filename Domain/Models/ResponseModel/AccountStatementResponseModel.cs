namespace Domain.Models.ResponseModel
{
    public class AccountStatementResponseModel
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }
        public string AccountType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string StatementTitle { get; set; } = string.Empty;
        public List<AccountTransactionResponseModel> Transactions { get; set; }
        
        public AccountStatementResponseModel()
        {
            AccountName = string.Empty;
            AccountType = string.Empty;
            Transactions = new List<AccountTransactionResponseModel>();
        }
    }

    public class StatementMetadata
    {
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string StatementTitle { get; set; }
    }
} 